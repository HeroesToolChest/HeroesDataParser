#!/usr/bin/env -S dotnet --

#:property PublishAot=false

using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

const string VersionFile = ".version.json";

if (args.Length < 6)
{
    Console.Error.WriteLine("Usage: <installed_heroes_directory> <output_directory> <heroesdata_directory> <heroesimages_directory> <json_create> <use_tool>");
    return 1;
}

string installedHeroesDirectory = args[0]; // Heroes of the Storm installation directory
string outputDirectory = args[1]; // Output directory for the new generated files from HPD
string heroesdataDirectory = args[2]; // the subdirectory in the heroes-data2 repo
string heroesimagesDirectory = args[3]; // the heroes-images repo
string jsonCreate = args[4]; // 'full' or 'patch'; what are we creating
bool useTool = args[5] == "true" || args[5] == "True" || args[5] == "1"; // whether to use the hdp global tool or not

if (!Directory.Exists(installedHeroesDirectory))
{
    Console.WriteLine("Error: The specified Heroes of the Storm installation directory does not exist.");
    return 1;
}

if (!Directory.Exists(heroesdataDirectory))
{
    Console.WriteLine("Error: The specified heroesdata directory does not exist.");
    return 1;
}

if (!Directory.Exists(heroesimagesDirectory))
{
    Console.WriteLine("Error: The specified heroes-images directory does not exist.");
    return 1;
}

if (jsonCreate != "full" && jsonCreate != "patch")
{
    Console.WriteLine("Error: The json_create argument must be either 'full' or 'patch'.");
    return 1;
}

// get the Heroes of the Storm version using HDP
Console.Write("Heroes of the Storm version: ");
string hotsVersion = await ExecuteHDP($"game -s \"{installedHeroesDirectory}\" --heroes-version");
hotsVersion = hotsVersion.TrimEnd('\r', '\n');

if (hotsVersion == "unknown")
{
    Console.WriteLine("Error: Could not determine the Heroes of the Storm version.");
    return 1;
}

string outputHeroesVersionDirectory = Path.Combine(outputDirectory, $"heroes_{hotsVersion}"); // subdirectory for the output
string versionFile = Path.Combine(heroesdataDirectory, VersionFile);

if (Directory.Exists(outputHeroesVersionDirectory))
{
    Console.WriteLine($"Error: '{outputHeroesVersionDirectory}' already exists");
    return 1;
}

Directory.CreateDirectory(outputHeroesVersionDirectory);
Console.WriteLine($"Created subdirectory for output: '{outputHeroesVersionDirectory}'");

JsonNode versionJsonNode = JsonNode.Parse(File.ReadAllText(versionFile))!;

string latestDirectory = versionJsonNode["latest"]!.GetValue<string>();
string lastestFullDirectory = versionJsonNode["latest-full"]!.GetValue<string>();

Console.WriteLine($"Latest version directory: {latestDirectory}");
Console.WriteLine($"Latest full version directory: {lastestFullDirectory}");

// in heroesdata directory create the verison subdirectory
string heroesdataVersionDirectory = Path.Combine(heroesdataDirectory, hotsVersion);
if (Directory.Exists(heroesdataVersionDirectory))
{
    Console.WriteLine($"Error: '{heroesdataVersionDirectory}' already exists");
    return 1;
}
Directory.CreateDirectory(heroesdataVersionDirectory);
Console.WriteLine($"Created heroesdata version directory: {heroesdataVersionDirectory}");

Console.WriteLine();

// create everything
//await ExecuteHDPNoCapture($"game -s \"{installedHeroesDirectory}\" -e all:i -l all --gs-replace-constant-vars --gs-replace-style-vars --gs-preserve-constant-vars --gs-preserve-style-vars --localized-text extract -o \"{outputHeroesVersionDirectory}\"");
await ExecuteHDPNoCapture($"game -s \"{installedHeroesDirectory}\" -e announcer -e map -l enus -l dede --gs-replace-constant-vars --gs-replace-style-vars --gs-preserve-constant-vars --gs-preserve-style-vars --localized-text extract -o \"{outputHeroesVersionDirectory}\"");

if (jsonCreate == "full")
    await ExecuteHeroesDataCreateFull($"{outputHeroesVersionDirectory} {heroesdataDirectory} {hotsVersion}");
else if (jsonCreate == "patch")
    await ExecuteHeroesDataCreatePatch($"{outputHeroesVersionDirectory} {heroesdataDirectory} {hotsVersion}");

// update the version file with the new version
versionJsonNode["date"] = DateTimeOffset.UtcNow.ToString("O");
versionJsonNode["latest"] = hotsVersion;
versionJsonNode["versions"]!.AsArray().Add(hotsVersion);
if (jsonCreate == "full")
    versionJsonNode["latest-full"] = hotsVersion;

await File.WriteAllTextAsync(versionFile, versionJsonNode.ToJsonString(new JsonSerializerOptions
{ 
    WriteIndented = true,
    NewLine = "\n",
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,    
}));

Console.WriteLine($"Updated version file: {versionFile}");
Console.WriteLine($"- latest: {versionJsonNode["latest"]!.GetValue<string>()}");
Console.WriteLine($"- latest full: {versionJsonNode["latest-full"]!.GetValue<string>()}");
Console.WriteLine($"- date: {versionJsonNode["date"]!.GetValue<string>()}");

Console.WriteLine($"Hdp auto done.");
return 0;

async Task<string> ExecuteHDP(string arguments)
{
    Process process = new()
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = useTool ? "dotnet" : "./HeroesDataParser",
            Arguments = useTool ? $"heroes-data-parser {arguments}" : arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };

    return await ProcessCommand(process);
}

async Task ExecuteHDPNoCapture(string arguments)
{
    Process process = new()
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = useTool ? "dotnet" : "./HeroesDataParser",
            Arguments = useTool ? $"heroes-data-parser {arguments}" : arguments,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };

    process.Start();
    await process.WaitForExitAsync();
}

static async Task ExecuteHeroesDataCreateFull(string arguments)
{
    Process process = new()
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "./heroesdata_create_full.cs",
            Arguments = arguments,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };

    process.Start();
    await process.WaitForExitAsync();
}

static async Task ExecuteHeroesDataCreatePatch(string arguments)
{
    Process process = new()
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "./heroesdata_create_patch.cs",
            Arguments = arguments,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };

    process.Start();
    await process.WaitForExitAsync();
}

static async Task<string> ProcessCommand(Process process)
{
    process.Start();

    string output = await process.StandardOutput.ReadToEndAsync();
    string error = await process.StandardError.ReadToEndAsync();

    await process.WaitForExitAsync();

    if (process.ExitCode != 0)
        Console.Error.Write($"Error: {error}");

    Console.Write(output);

    return output;
}