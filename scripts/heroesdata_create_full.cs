#!/usr/bin/env -S dotnet --

#:property PublishAot=false

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

const string HdpJsonFile = ".hdp.json";
const string DataDirectory = "data";
const string MapsDirectory = "maps";
const string GameStringsDirectory = "gamestrings";

if (args.Length < 3)
{
    Console.Error.WriteLine("Usage: <output_directory> <heroesdata_directory> <new_version>");
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

Console.WriteLine("Creating new version full...");

Dictionary<string, object> dataFileNamesByKey = [];
Dictionary<string, object> gameStringFileNamesByKey = [];

Console.WriteLine("Processing data files...");
IEnumerable<string> newJsonFilePaths = Directory.EnumerateFiles(outputDataDirectory, "*.json");

Directory.CreateDirectory(heroesdataVersionDataDirectory);

// loop through all the (base) .json files in the output data directory
foreach (string newJsonFilePath in newJsonFilePaths)
{
    string newJsonFileName = Path.GetFileName(newJsonFilePath);

    if (!TryGetKeyFromDataFileName(newJsonFileName, out string? dataType))
        continue;

    Console.WriteLine($"[{dataType}] ({newJsonFileName})");

    File.Copy(newJsonFilePath , Path.Combine(heroesdataVersionDataDirectory, newJsonFileName));
    dataFileNamesByKey[dataType] = newJsonFileName;
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

Directory.CreateDirectory(heroesdataVersionGameStringsDirectory);

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

        File.Copy(newGameStringJsonFilePath, Path.Combine(heroesdataVersionGameStringsDirectory, newGameStringJsonFileName));
        gameStringFileNamesByKey[mapdataLocale] = newGameStringJsonFileName;
    }
    else
    {
        Console.WriteLine($"[{locale}] ({newGameStringJsonFileName})");

        File.Copy(newGameStringJsonFilePath, Path.Combine(heroesdataVersionGameStringsDirectory, newGameStringJsonFileName));
        gameStringFileNamesByKey[locale] = newGameStringJsonFileName;
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
    Json = "full",
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

        IEnumerable<string> newJsonPatchFilePaths = Directory.EnumerateFiles(battlegroundDirectory, "*.patch.json");

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
            CreateNoWindow = true
        }
    };

    process.Start();

    string output = await process.StandardOutput.ReadToEndAsync();
    string error = await process.StandardError.ReadToEndAsync();

    await process.WaitForExitAsync();

    if (process.ExitCode != 0)
        Console.Error.Write($"Error: {error}");

    Console.Write(output);

    return output;
}

class HdpJsonFile
{
    [JsonPropertyName("hdp")]
    public string Hdp { get; set; } = string.Empty;

    [JsonPropertyName("json")]
    public string Json { get; set; } = string.Empty;

    [JsonPropertyName("extracted")]
    public bool Extracted { get; set; }

    [JsonPropertyName("files")]
    public HdpJsonFiles Files { get; set; } = new();
}

class HdpJsonFiles
{
    [JsonPropertyName("[data]")]
    public Dictionary<string, object> Data { get; set; } = new();

    [JsonPropertyName("[gamestrings]")]
    public Dictionary<string, object> GameStrings { get; set; } = new();
}