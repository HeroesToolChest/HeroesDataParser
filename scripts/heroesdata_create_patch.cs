#!/usr/bin/env -S dotnet --

#:property PublishAot=false

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

const string VersionFile = ".version.json";
const string HdpJsonFile = ".hdp.json";
const string DataDirectory = "data";
const string MapsDirectory = "maps";
const string GameStringsDirectory = "gamestrings";
const string PatchJsonExtension = ".patch.json";
const string PrevDirectory = "prev"; // temp directory for patching up

if (args.Length < 3)
{
    Console.Error.WriteLine("Usage: <output_directory> <heroesdata_directory> <new_version_name>");
    return 1;
}

string outputDirectory = args[0];
string heroesdataDirectory = args[1]; // heroesdata directory in heroes-data2 repository
string newVersionName = args[2]; // e.g 2.55.16.96881

string outputDataDirectory = Path.Combine(outputDirectory, DataDirectory);
string outputDataMapsDirectory = Path.Combine(outputDirectory, DataDirectory, MapsDirectory);
string outputGameStringDirectory = Path.Combine(outputDirectory, GameStringsDirectory);
string outputGameStringMapsDirectory = Path.Combine(outputDirectory, GameStringsDirectory, MapsDirectory);

string heroesdataVersionDirectory = Path.Combine(heroesdataDirectory, newVersionName);
string heroesdataVersionDataDirectory = Path.Combine(heroesdataVersionDirectory, DataDirectory);
string heroesdataVersionDataMapsDirectory = Path.Combine(heroesdataVersionDirectory, DataDirectory, MapsDirectory);
string heroesdataVersionGameStringsDirectory = Path.Combine(heroesdataVersionDirectory, GameStringsDirectory);
string heroesdataVersionGameStringMapsDirectory = Path.Combine(heroesdataVersionDirectory, GameStringsDirectory, MapsDirectory);

string outputPrevDirectory = Path.Combine(outputDirectory, PrevDirectory);

string versionFile = Path.Combine(heroesdataDirectory, VersionFile);

Console.WriteLine("Creating new version patch...");

using FileStream fileStream = new(versionFile, FileMode.Open, FileAccess.Read);
using JsonDocument versionDocument = JsonDocument.Parse(fileStream);

JsonElement rootElement = versionDocument.RootElement;

string latestDirectory = rootElement.GetProperty("latest").GetString()!;
string latestFullDirectory = rootElement.GetProperty("latest-full").GetString()!;

Console.WriteLine($"New version: {newVersionName}");
Console.WriteLine($"Latest version directory: {latestDirectory}");
Console.WriteLine($"Latest full (root) version directory: {latestFullDirectory}");

List<string> versionsDirectories = [];

JsonElement versionsArray = rootElement.GetProperty("versions");

foreach (JsonElement item in versionsArray.EnumerateArray())
{
    versionsDirectories.Add(item.GetString()!);
}

// verify both latest and latest-full are in versions array
if (!versionsDirectories.Contains(latestDirectory))
{
    Console.WriteLine("Error: 'latest' not found in versions array");
    return 1;
}
if (!versionsDirectories.Contains(latestFullDirectory))
{
    Console.WriteLine("Error: 'latest-full' not found in versions array");
    return 1;
}

// verify latest-full is before or equal to latest
int latestIndex = versionsDirectories.IndexOf(latestDirectory);
int latestFullIndex = versionsDirectories.IndexOf(latestFullDirectory);

if (latestFullIndex > latestIndex)
{
    Console.WriteLine("Error: 'latest-full' is after 'latest' in versions array");
    return 1;
}

// the source directory for the original files to patch up from, either the heroesdata root directory if no patching up needed, or the output 'prev' directory after patching up
string rootDirectory = heroesdataDirectory;
bool isPatchedUp = false;

// check if needing to patch up from latest-full to latest
// patches up the existing heroesdata and creates the patched files in the output 'prev' directory
if (latestFullIndex < latestIndex)
{
    bool firstRun = true;
    for (int i = latestFullIndex + 1; i < latestIndex + 1; i++)
    {
        Console.WriteLine($"Applying patch version (to root): {versionsDirectories[i]}");

        string targetVersionName = versionsDirectories[i];
        string sourceDirectory;

        if (firstRun)
        {
            sourceDirectory = heroesdataDirectory;
            firstRun = false;
        }
        else
        {
            sourceDirectory = outputPrevDirectory;
        }

        // patch up
        await PatchUp(sourceDirectory, latestFullDirectory, targetVersionName); // creates patched files in output directory 'prev'
    }

    isPatchedUp = true;
    rootDirectory = outputPrevDirectory;
}


string hdpJsonFilePath = Path.Combine(heroesdataDirectory, latestDirectory, HdpJsonFile);
HdpJsonFile hdpJsonFile = JsonSerializer.Deserialize<HdpJsonFile>(File.ReadAllText(hdpJsonFilePath))!;

// for .hdp.json
Dictionary<string, object> dataFileNamesByKey = [];
Dictionary<string, object> gameStringFileNamesByKey = [];

Console.WriteLine("Processing data files...");
IEnumerable<string> newJsonFilePaths = Directory.EnumerateFiles(outputDataDirectory, "*.json");

// loop through all the (base) .json files in the output data directory
foreach (string newJsonFilePath in newJsonFilePaths)
{
    string newJsonFileName = Path.GetFileName(newJsonFilePath);

    if (!TryGetKeyFromDataFileName(newJsonFileName, out string? dataType))
        continue;

    Console.WriteLine($"[{dataType}] ({newJsonFileName})");

    string dataTypeFileName = hdpJsonFile.Files.Data[dataType].ToString()!;

    // change original file location, maybe at the output directory in data/prev, gamestrings/prev
    string latestDataTypeFilePath = Path.Combine(rootDirectory, latestFullDirectory, DataDirectory, GetFileName(dataTypeFileName, isPatchedUp));

    await ExecuteHDP($"json-patch create \"{latestDataTypeFilePath}\" \"{newJsonFilePath}\" -o \"{heroesdataVersionDataDirectory}\"");

    dataFileNamesByKey[dataType] = Path.ChangeExtension(newJsonFileName, PatchJsonExtension);
}

Console.WriteLine("Processing data map files...");
await CopyOverMapFiles(outputDataMapsDirectory, heroesdataVersionDataMapsDirectory, dataFileNamesByKey, (newJsonPatchFileName) =>
{
    if (TryGetKeyFromDataFileName(newJsonPatchFileName, out string? dataType))
        return dataType;

    return null;
});

Console.WriteLine("Processing gamestring files...");
IEnumerable<string> newGameStringJsonFilePaths = Directory.EnumerateFiles(outputGameStringDirectory, "*.json");

// loop through all the (base) .json files in the output gamestring directory
foreach (string newGameStringJsonFilePath in newGameStringJsonFilePaths)
{
    string newGameStringJsonFileName = Path.GetFileName(newGameStringJsonFilePath);

    if (!TryGetKeysFromGameStringFileName(Path.GetFileNameWithoutExtension(newGameStringJsonFileName), out string? locale, out string? mapdata))
    {
        Console.WriteLine($"Error: unexpected gamestring file name format '{newGameStringJsonFileName}'");
        continue;
    }

    if (!string.IsNullOrWhiteSpace(mapdata))
    {
        if (mapdata != "mapdata")
        {
            Console.WriteLine($"Error: unexpected gamestring file name format '{newGameStringJsonFileName}'");
            continue;
        }

        string mapdataLocale = $"mapdata|{locale}";
        Console.WriteLine($"[{mapdataLocale}] ({newGameStringJsonFileName})");

        string gamestringLocaleFileName = hdpJsonFile.Files.GameStrings[mapdataLocale].ToString()!;
        await CreateGameStringPatchFile(newGameStringJsonFilePath, GetFileName(gamestringLocaleFileName, isPatchedUp));

        gameStringFileNamesByKey[mapdataLocale] = Path.ChangeExtension(newGameStringJsonFileName, PatchJsonExtension);
    }
    else
    {
        Console.WriteLine($"[{locale}] ({newGameStringJsonFileName})");

        string gamestringLocaleFileName = hdpJsonFile.Files.GameStrings[locale].ToString()!;
        await CreateGameStringPatchFile(newGameStringJsonFilePath, GetFileName(gamestringLocaleFileName, isPatchedUp));

        gameStringFileNamesByKey[locale] = Path.ChangeExtension(newGameStringJsonFileName, PatchJsonExtension);
    }
}

Console.WriteLine("Processing gamestring map files...");
await CopyOverMapFiles(outputGameStringMapsDirectory, heroesdataVersionGameStringMapsDirectory, gameStringFileNamesByKey, (newJsonPatchFileName) =>
{
    if (TryGetKeysFromGameStringFileName(Path.GetFileNameWithoutExtension(newJsonPatchFileName), out string? locale, out _))
        return locale;

    return null;
});

// get HeroesDataParser version
Console.Write("HDP version: ");
string version = await ExecuteHDP("--version");

// create .hdp.json file
HdpJsonFile hdpJson = new()
{
    Hdp = version.TrimEnd('\r', '\n'),
    Json = "patch",
    DependsOn = latestDirectory,
    RootVersion = latestFullDirectory,
    Extracted = true,
    Files = new HdpJsonFiles
    {
        Data = dataFileNamesByKey.OrderBy(kvp => kvp.Key == $"[{MapsDirectory}]")
            .ThenBy(kvp => kvp.Key)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

        GameStrings = gameStringFileNamesByKey.OrderBy(kvp =>
        {
            if (kvp.Key == $"[{MapsDirectory}]")
                return 2;

            if (kvp.Key.StartsWith("mapdata"))
                return 1;

            return 0;
        })
        .ThenBy(kvp => kvp.Key)
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
    }
};

await using FileStream newHdpJsonFileStream = File.Create(Path.Combine(heroesdataVersionDirectory, HdpJsonFile));
await JsonSerializer.SerializeAsync(newHdpJsonFileStream, hdpJson, new JsonSerializerOptions
{
    WriteIndented = true,
    NewLine = "\n",
});

Console.WriteLine($"Created {HdpJsonFile} file for version '{newVersionName}'");
Console.WriteLine("Done.");

return 0;

async Task CreateGameStringPatchFile(string newGameStringJsonFilePath, string gamestringLocaleFileName)
{
    string latestGameStringLocaleFilePath = Path.Combine(rootDirectory, latestFullDirectory, GameStringsDirectory, gamestringLocaleFileName);

    await ExecuteHDP($"json-patch create \"{latestGameStringLocaleFilePath}\" \"{newGameStringJsonFilePath}\" -o \"{heroesdataVersionGameStringsDirectory}\"");
}

async Task CopyOverMapFiles(string outputMapsDirectory, string heroesdataMapsDirectory, Dictionary<string, object> fileNamesByKey, Func<string, string?> getKey)
{
    SortedDictionary<string, object> patchFilesByBattleground = [];

    IEnumerable<string> battlegroundDirectories = Directory.EnumerateDirectories(outputMapsDirectory);

    // loop through all the map directories in the ouput maps directory
    foreach (string battlegroundDirectory in battlegroundDirectories)
    {
        SortedDictionary<string, string> patchFileNameByType = [];

        string battlegroundDirectoryName = Path.GetFileName(battlegroundDirectory);

        Console.WriteLine($"[{battlegroundDirectoryName}]");

        IEnumerable<string> newJsonPatchFilePaths = Directory.EnumerateFiles(battlegroundDirectory, $"*{PatchJsonExtension}");

        // loop through all the .patch.json files in the map directory
        foreach (string newJsonPatchFilePath in newJsonPatchFilePaths)
        {
            string newJsonPatchFileName = Path.GetFileName(newJsonPatchFilePath);

            Console.WriteLine($"  ({newJsonPatchFileName})");

            string? key = getKey.Invoke(newJsonPatchFileName);

            if (string.IsNullOrWhiteSpace(key))
                continue;

            // copy them over
            string newbattlegroundDirectory = Path.Combine(heroesdataMapsDirectory, battlegroundDirectoryName);

            Directory.CreateDirectory(newbattlegroundDirectory);
            File.Copy(newJsonPatchFilePath, Path.Combine(newbattlegroundDirectory, newJsonPatchFileName), false);

            patchFileNameByType.Add(key, newJsonPatchFileName);
        }

        patchFilesByBattleground.Add(battlegroundDirectoryName, patchFileNameByType);
    }

    fileNamesByKey.Add("[maps]", patchFilesByBattleground);
}

async Task PatchUp(string sourceDirectory, string sourceVersionName, string targetVersionName)
{
    string sourceVersionDirectory = Path.Combine(sourceDirectory, sourceVersionName);
    string heroesdataTargetVersionDirectory = Path.Combine(heroesdataDirectory, targetVersionName);

    string hdpJsonTargetFilePath = Path.Combine(heroesdataTargetVersionDirectory, HdpJsonFile);

    HdpJsonFile hdpJsonTargetFile = JsonSerializer.Deserialize<HdpJsonFile>(File.ReadAllText(hdpJsonTargetFilePath))!;

    Console.WriteLine($"Patching up from version '{sourceVersionName}' to '{targetVersionName}'...");

    if (hdpJsonTargetFile.Extracted is false)
    {
        Console.WriteLine($"Target version '{targetVersionName}' is not extracted, skipping");
        return;
    }
    
    Console.WriteLine("Patching up data files...");
    await ApplyDataPatchUp(Path.Combine(sourceVersionDirectory, DataDirectory), sourceVersionName, Path.Combine(heroesdataTargetVersionDirectory, DataDirectory), hdpJsonTargetFile);

    Console.WriteLine("Patching up gamestring files...");
    await ApplyGameStringPatchUp(Path.Combine(sourceVersionDirectory, GameStringsDirectory), sourceVersionName, Path.Combine(heroesdataTargetVersionDirectory, GameStringsDirectory), hdpJsonTargetFile);
}

async Task ApplyDataPatchUp(string sourceDataDirectory, string sourceVersionName, string targetDataDirectory, HdpJsonFile hdpJsonTargetFile)
{
    IEnumerable<string> originalFilePaths = Directory.EnumerateFiles(sourceDataDirectory, "*.json");

    foreach (string originalFilePath in originalFilePaths)
    {
        string originalFileName = Path.GetFileName(originalFilePath);

        if (!TryGetKeyFromDataFileName(originalFileName, out string? key))
        {
            Console.WriteLine($"Error: unexpected data file name format '{originalFileName}'");
            continue;
        }

        if (!hdpJsonTargetFile.Files.Data.TryGetValue(key, out object? dataPatchFileName))
        {
            Console.WriteLine($"Error: key '{key}' not found in target .hdp.json");
            continue;
        }

        await ExecuteHDP($"json-patch apply \"{originalFilePath}\" \"{Path.Combine(targetDataDirectory, ((JsonElement)dataPatchFileName).GetString()!)}\" -o \"{Path.Combine(outputPrevDirectory, sourceVersionName, DataDirectory)}\"");
    }
}

async Task ApplyGameStringPatchUp(string sourceGameStringsDirectory, string sourceVersionName, string targetGameStringsDirectory, HdpJsonFile hdpJsonTargetFile)
{
    IEnumerable<string> originalFilePaths = Directory.EnumerateFiles(sourceGameStringsDirectory, "*.json");

    foreach (string originalFilePath in originalFilePaths)
    {
        string originalFileName = Path.GetFileName(originalFilePath);

        if (!TryGetKeysFromGameStringFileName(Path.GetFileNameWithoutExtension(originalFileName), out string? locale, out string? mapdata))
        {
            Console.WriteLine($"Error: unexpected gamestring file name format '{originalFileName}'");
            continue;
        }

        string gamestringPatchFileName;

        if (!string.IsNullOrWhiteSpace(mapdata))
            gamestringPatchFileName = hdpJsonTargetFile.Files.GameStrings[$"mapdata|{locale}"].ToString()!;
        else
            gamestringPatchFileName = hdpJsonTargetFile.Files.GameStrings[locale].ToString()!;

        await ExecuteHDP($"json-patch apply \"{originalFilePath}\" \"{Path.Combine(targetGameStringsDirectory, gamestringPatchFileName)}\" -o \"{Path.Combine(outputPrevDirectory, sourceVersionName, GameStringsDirectory)}\"");
    }
}

static bool TryGetKeyFromDataFileName(string fileName, [NotNullWhen(true)] out string? dataType)
{
    int index = fileName.IndexOf('_');
    if (index < 2)
    {
        dataType = string.Empty;
        return false;
    }

    dataType = fileName[..index];
    return true;
}

static bool TryGetKeysFromGameStringFileName(ReadOnlySpan<char> fileNameWithoutExtension, [NotNullWhen(true)] out string? locale, [NotNullWhen(true)] out string? mapdata)
{
    Span<Range> ranges = stackalloc Range[4];
    int count = fileNameWithoutExtension.Split(ranges, '_');

    if (count == 4)
    {
        locale = StripEndPatch(fileNameWithoutExtension[ranges[3]]);
        mapdata = fileNameWithoutExtension[ranges[1]].ToString();

        return true;
    }
    else if (count == 3)
    {
        locale = StripEndPatch(fileNameWithoutExtension[ranges[2]]);
        mapdata = string.Empty;

        return true;
    }
    else
    {
        locale = string.Empty;
        mapdata = string.Empty;

        return false;
    }
}

static string GetFileName(string fileName, bool isPatchedUp)
{
    if (isPatchedUp)
    {
        if (fileName.EndsWith(PatchJsonExtension, StringComparison.OrdinalIgnoreCase))
            return $"{fileName[..^PatchJsonExtension.Length]}.json";

        return fileName;
    }
    else
    {
        return fileName;
    }
}

static string StripEndPatch(ReadOnlySpan<char> fileName)
{
    if (fileName.EndsWith(".patch", StringComparison.OrdinalIgnoreCase))
        return fileName[..^6].ToString();

    return fileName.ToString();
}

static async Task<string> ExecuteHDP(string arguments)
{
    Process process = new()
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "./HeroesDataParser",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        }
    };

    process.Start();

    string output = await process.StandardOutput.ReadToEndAsync();
    string error = await process.StandardError.ReadToEndAsync();

    await process.WaitForExitAsync();

    if (process.ExitCode != 0)
    {
        Console.Error.Write($"Error: {error}");
        Console.Write(output);
        Environment.Exit(1);
    }
    else
    {
        Console.Write(output);
    }

    return output;
}

class HdpJsonFile
{
    [JsonPropertyName("hdp")]
    public string Hdp { get; set; } = string.Empty;

    [JsonPropertyName("json")]
    public string Json { get; set; } = string.Empty;

    [JsonPropertyName("depends-on")]
    public string DependsOn { get; set; } = string.Empty;

    [JsonPropertyName("root-version")]
    public string RootVersion { get; set; } = string.Empty;

    [JsonPropertyName("extracted")]
    public bool Extracted { get; set; }

    [JsonPropertyName("files")]
    public HdpJsonFiles Files { get; set; } = new();
}

class HdpJsonFiles
{
    [JsonPropertyName("[data]")]
    public Dictionary<string, object> Data { get; set; } = [];

    [JsonPropertyName("[gamestrings]")]
    public Dictionary<string, object> GameStrings { get; set; } = [];
}
