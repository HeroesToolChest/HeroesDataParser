# Heroes Data Parser
[![Build](https://github.com/HeroesToolChest/HeroesDataParser/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/HeroesToolChest/HeroesDataParser/actions/workflows/build.yml)
[![Release](https://img.shields.io/github/release/koliva8245/HeroesDataParser.svg)](https://github.com/koliva8245/HeroesDataParser/releases/latest)
[![NuGet](https://img.shields.io/nuget/v/HeroesDataParser.svg)](https://www.nuget.org/packages/HeroesDataParser/)

Heroes Data Parser is a .NET command line tool that extracts Heroes of the Storm game data into JSON files, including gamestrings and images.

Extracts the following:
 - Heroes (includes images)
 - Units (includes images)  
 - Match Awards (includes images)
 - Hero Skins
 - Mounts
 - Banners
 - Sprays (includes images)
 - Announcers (includes images)
 - Voice Lines (includes images)
 - Portrait Packs
 - Reward Portraits (images involve manual extraction, read [wiki](https://github.com/HeroesToolChest/HeroesDataParser/wiki/Portrait-Icon-Extraction))
 - Emoticons (includes images)
 - Emoticon Packs
 - Veterancy data
 - Bundles (includes images)
 - Boosts
 - Loot Chests
 - Type Description data (includes images)
 - Maps (includes images)

Visit the [wiki](https://github.com/HeroesToolChest/HeroesDataParser/wiki) for more information.

### Other Helpful Repos
- [Heroes Data](https://github.com/HeroesToolChest/heroes-data) contains the extracted data files
- [Heroes Images](https://github.com/HeroesToolChest/heroes-images) complements Heroes Data by providing the extracted image files
- [Heroes Element](https://github.com/HeroesToolChest/Heroes.Element) is a .NET library...
- [Heroes XmlData](https://github.com/HeroesToolChest/Heroes.XmlData) is a .NET library...

## Installation
### Dotnet Global Tool (Recommended)
Download and install the latests [.NET SDK](https://dotnet.microsoft.com/download). 

Once installed, run the following command:
```
dotnet tool install --global HeroesDataParser
```

To update to a newer version, run the following command:
```
dotnet tool update --global HeroesDataParser
```

***

### Zip File Download - Framework-Dependent Deployment (fdd)
Portable to any operating system.

Download and install the latest [.NET Runtime or SDK](https://dotnet.microsoft.com/download). 

Download and extract the latest `HeroesDataParser.*-fdd-any` archive file from the [releases](https://github.com/HeroesToolChest/HeroesDataParser/releases) page.

***

### Zip File Download - Framework-Dependent Executable (fde)
Runs only on a selected operating system and architecture.

Download and install the latest [.NET Runtime or SDK](https://dotnet.microsoft.com/download). 

Download and extract the latest `HeroesDataParser.*-fde-<OS>-<ARCH>` archive file from the [releases](https://github.com/HeroesToolChest/HeroesDataParser/releases) page for a selected operating system and architecture.

***

### Zip File Download - Self-Contained Deployment (scd)
Runs only on a selected operating system and architecture. No runtime or SDK is required.

Download and extract the latest `HeroesDataParser.*-scd-<OS>-<ARCH>` archive file from the [releases](https://github.com/HeroesToolChest/HeroesDataParser/releases) page for a selected operating system and architecture.

This archive file contains everything that is needed to run the dotnet app without .NET being installed, so the archive file is larger.

## Usage
If installed as a Dotnet Global Tool, the app can be run with one of the following commands:
```
dotnet heroes-data-parser -h
dotnet-heroes-data-parser -h
```

If installed as a Framework-Dependent Deployment (fdd), run the following command from the extracted directory:
```
dotnet HeroesDataParser.dll -h
```

If installed as a Framework-Dependent Executable (fde) or Self-Contained Deployment (scd), run one of the following commands from the extracted directory:
```
windows (cmd): HeroesDataParser -h
windows (powershell): .\HeroesDataParser -h 
macOS or Linux: ./HeroesDataParser -h
```

Output of the -h option
```
USAGE:
    heroesdataparser <storage-type> [OPTIONS] [COMMAND]

ARGUMENTS:
    <storage-type>    The type of storage to load from ('mods', 'game', or 'online')

OPTIONS:
                                             DEFAULT
    -h, --help                                          Prints help information
    -v, --version                                       Prints version information
    -p, --storage-path <PATH>                           The path of 'Heroes of the Storm' directory or an already extracted 'mods' directory
        --download-ptr                                  Download from the ptr server instead of live ('online' storage-type only)
    -e, --extractor <EXTRACTOR>              Hero       Extractors to enable, add ':i|:images' to enabled image extraction (can be specified multiple
                                                        times)
    -l, --localization <LOCALE>              enUS       Localizations for gamestrings to process (can be specified multiple times)
    -g, --gamestring-text <FORMAT>           RawText    The format of the gamestrings
        --gs-replace-constant-vars                      Replace constant variables in gamestrings with the color text hex values
        --gs-replace-style-vars                         Replace font variables in gamestrings with the color text hex values
        --gs-preserve-constant-vars                     Preserve constant variables in gamestrings
        --gs-preserve-style-vars                        Preserve style variables in gamestrings
        --localized-text <OPTION>            None       Specifies how to handle gamestring properties during JSON serialization
        --no-map-specific                               Disable the map specific JSON file creation when 'map' extractor is specified
        --map-specific-json-output <TYPE>    Patch      Specifies how to handle the map specific JSON file creation
        --map-specific-empty-patch                      Allows map specific patch files without any item changes to be created
        --map-specific-empty-directories                Allows map specific empty directories to be created
        --custom-configs                                Display the loaded custom config files
        --no-indent                                     Disable indentation in the output JSON files
    -t, --threads <NUMBER>                   -1         The number of threads to use for data parsing and image writing (defaults to max number of
                                                        processors)
    -o, --output-path <PATH>                            The path of the output directory (defaults to the current directory)
        --heroes-version                                Manually set the 'Heroes of the Storm' version in the format of major.minor.revision.build<_ptr>
                                                        (e.g. 1.2.3.4 or 1.2.3.4_ptr)

COMMANDS:
    casc-extract <storage-type>    Extract data files from a 'Heroes of the Storm' directory or from online
    json-patch                     Json-patching operations
    gamestring-text                Gamestring text operations
    schema                         Json schema operations
    portrait                       Reward portrait data operations
```

Example command to extract the `hero` data from the `Heroes of the Storm` directory. Since no `-o|--output-path` option is set, it defaults to the current directory.
```powershell
dotnet heroesdataparser.dll game --storage-path 'D:\Heroes of the Storm' -e hero
```
**Note: When using command prompt on Windows, use double quotes instead of single quotes when specifying filepaths.**

## Arguments
### Storage Type
Specify either `mods`, `game`, or `online` to indicate the type of storage to load from.

`mods` - Already extracted `Heroes of the Storm` game data with `mods` as it's root directory  
`game` - `Heroes of the Storm` installation game directory  
`online` - To download the gamedata from the live or ptr servers

## Options

## Commands
### casc-extract
```
DESCRIPTION:
Extract data files from a 'Heroes of the Storm' directory or from online

USAGE:
    heroesdataparser casc-extract <storage-type> [OPTIONS]

ARGUMENTS:
    <storage-type>    The type of storage to load from ('game' or 'online')

OPTIONS:
                                 DEFAULT
    -h, --help                              Prints help information
    -p, --storage-path <PATH>               The path of 'Heroes of the Storm' directory
        --download-ptr                      Download from the ptr server instead of live ('online' storage-type only)
    -d, --directory <PATH>       mods       The directory and it's subdirectories to be extracted, path must start with 'mods' (can be specified
                                            multiple times)
    -f, --filter <EXT>           *          Filter files by extension (can be specified multiple times)
    -t, --threads <NUMBER>       -1         The number of threads to use for file extraction (defaults to max number of processors)
    -o, --output-path <PATH>                The path of the output directory (defaults to current directory)
```
`-p, --storage-path` is required if the `storage-type` argument is set to `game`.

If the `storage-type` argument is set to `online`, the `--download-ptr` option can be used to specify whether to download from the ptr server instead of live.

Use the `-d, --directory` option to specify the directories to be extracted from the casc storage. Ensure the provided directory paths use the correct path seperators for the operating system being used.

Use the `-f, --filter` option to specify file extensions (with or without leading period) to filter by during extraction.

The output directory structure of the extracted files will mirror the directory structure in the casc storage. 
If `-o, --output-path` is not specified, then the extracted files will be in the `mods` directory in the current directory, otherwise the `mods` directory will be a subdirectory of the specified output directory.

A `hdp.info` json file will be created in the `mods` directory with information about the extraction.
This file is used for the root command when `mods` is specified for the `storage-type` argument.

Example command on Windows that specifies two directories (`thefirelords.stormmod` and `tracer.stormmod`) and filters by `xml` and `txt` file extensions:
```powershell
casc-extract game -p 'E:\Games\Heroes of the Storm' -d 'mods\heromods\thefirelords.stormmod' -d 'mods\heromods\tracer.stormmod' -f xml -f txt 
```

Example command on Linux that specifies the same two directories and filters by the same file extensions but with `online` storage type:
```powershell
casc-extract online -d 'mods/heromods/thefirelords.stormmod' -d 'mods/heromods/tracer.stormmod' -f xml -f txt 
```

## Developing
To build and compile the code, it is recommended to use the latest version of [Visual Studio 2026 or Visual Studio Code](https://visualstudio.microsoft.com/downloads/).

Another option is to use the dotnet CLI tools from the latest [.NET SDK](https://dotnet.microsoft.com/download).

## License
[MIT license](/LICENSE)
