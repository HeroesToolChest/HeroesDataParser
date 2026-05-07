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
- [Heroes Data](https://github.com/HeroesToolChest/heroes-data2) contains the extracted data files
- [Heroes Images](https://github.com/HeroesToolChest/heroes-images) complements Heroes Data by providing the extracted image files
- [Heroes Element](https://github.com/HeroesToolChest/Heroes.Element) is a .NET library to parse the extracted JSON files
- [Heroes XmlData](https://github.com/HeroesToolChest/Heroes.XmlData) is a .NET library which is used to parse the Heroes of the Storm CASC storage and extract the raw data files

## Installation
### Dotnet Global Tool (Recommended)
Download and install the latest [.NET SDK](https://dotnet.microsoft.com/download). 

Once installed, run the following command:
```
dotnet tool install --global HeroesDataParser
```

To update to a newer version, run the following command:
```
dotnet tool update --global HeroesDataParser
```

### Zip File Download - Framework-Dependent Deployment (fdd)
Portable to any operating system.

Download and install the latest [.NET Runtime or SDK](https://dotnet.microsoft.com/download). 

Download and extract the latest `HeroesDataParser.*-fdd-any` archive file from the [releases](https://github.com/HeroesToolChest/HeroesDataParser/releases) page.

### Zip File Download - Framework-Dependent Executable (fde)
Runs only on a selected operating system and architecture.

Download and install the latest [.NET Runtime or SDK](https://dotnet.microsoft.com/download). 

Download and extract the latest `HeroesDataParser.*-fde-<OS>-<ARCH>` archive file from the [releases](https://github.com/HeroesToolChest/HeroesDataParser/releases) page for a selected operating system and architecture.

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
# Windows (cmd):
HeroesDataParser -h

# Windows (powershell):
.\HeroesDataParser -h 

# macOS or Linux:
./HeroesDataParser -h
```

Output of the `-h` option
```
USAGE:
    heroesdataparser <storage-type> [OPTIONS] [COMMAND]

ARGUMENTS:
    <storage-type>    Storage type to load from (mods, game, or online)

OPTIONS:
                                             DEFAULT
    -h, --help                                          Prints help information
    -v, --version                                       Prints version information
    -s, --storage-path <PATH>                           Path to the Heroes of the Storm directory or an already extracted mods directory
        --download-ptr                                  Download from the PTR server instead of live (online storage-type only)
    -e, --extractor <EXTRACTOR>              Hero       Extractors to enable, add :i or :images to enable image extraction (can be specified multiple times)
    -l, --localization <LOCALE>              enUS       Locales for gamestrings to process (can be specified multiple times)
    -g, --gamestring-text <FORMAT>           RawText    Format of the gamestrings
        --gs-replace-constant-vars                      Replace constant variables in gamestrings with color hex values
        --gs-replace-style-vars                         Replace font style variables in gamestrings with color hex values
        --gs-preserve-constant-vars                     Preserve constant variables in gamestrings
        --gs-preserve-style-vars                        Preserve style variables in gamestrings
        --localized-text <OPTION>            None       Action for gamestring property extraction
        --no-map-specific                               Disable map-specific JSON file creation when map extractor is specified
        --map-specific-json-output <TYPE>    Patch      Action to handle map-specific JSON file creation
        --map-specific-empty-patch                      Allow map-specific patch files without item changes to be created
        --map-specific-empty-directories                Allow empty map-specific directories to be created
        --custom-configs                                Display loaded custom config files
        --no-indent                                     Disable indentation in output JSON files
    -t, --threads <NUMBER>                   -1         Number of threads for data parsing and image writing (defaults to max processors)
    -o, --output-path <PATH>                            Output directory for created files (defaults to current directory)
        --set-heroes-version <VERSION>                  Manually set the Heroes of the Storm version as major.minor.revision.build[_ptr] (e.g. 1.2.3.4 or 1.2.3.4_ptr)
        --heroes-version                                Display the Heroes of the Storm version

COMMANDS:
    casc-extract <storage-type>    Extract data files from a Heroes of the Storm directory or from online
    json-patch                     JSON patching operations
    gamestring-text                Gamestring text formatting and conversion operations
    localized-text                 Localized text operations
    schema                         JSON schema operations
    portrait                       Reward portrait data operations
```

Example command to extract the `hero` data from the `Heroes of the Storm` directory. Since no `-o|--output-path` option is set, it defaults to the current directory.
```
dotnet heroesdataparser.dll game --storage-path "D:\Heroes of the Storm" -e hero
```

## Arguments
### Storage Type
Specify either `mods`, `game`, or `online` to indicate the type of storage to load from.

`mods` - Already extracted `Heroes of the Storm` game data with `mods` as its root directory (see [`casc-extract`](#casc-extract) command)  
`game` - The `Heroes of the Storm` installation  
`online` - Download the game data from the live or PTR servers

## Options
### -s, --storage-path
If the `storage-type` argument is set to `game` or `mods`, then this option is required to specify the path of the `Heroes of the Storm` directory or an already extracted `mods` directory. This option is not used with the `online` storage type.

If on `Linux` or `macOS` and providing an extracted `mods` directory, ensure that all directories and files are in lowercase characters.

See the [`casc-extract`](#casc-extract) command for more information about extracting the data files.

***

### --download-ptr 
If the `storage-type` argument is set to `online`, this option downloads from the PTR server instead of the live server.

***

### -e, --extractor
The extractors to enable for data and image extraction. Can be specified multiple times to enable multiple extractors. Default is `hero`.

Data JSON files will be created in the `data` subdirectory and image files will be created in the `images` subdirectory of the output directory.

Enabling the extractor `map` will run all other enabled extractors for each found map, unless the [`--no-map-specific`](#--no-map-specific) option is enabled.

Map specific JSON files will be created in the `maps` subdirectory of the `data` subdirectory which by default will be JSON patch files. To change this, set the [`--map-specific-json-output`](#--map-specific-json-output) option.

`all` - extracts all data files  
`hero` - extracts hero data  
`unit` - extracts unit data  
`matchaward` - extracts match awards  
`skin` - extracts hero skins  
`mount` - extracts mounts  
`banner` - extracts banners  
`spray` - extracts sprays  
`announcer` - extracts announcers  
`voiceline` - extracts voicelines  
`portraitpack` - extracts portrait packs  
`rewardportrait` - extracts reward portraits  
`emoticon` - extracts emojis  
`emoticonpack` - extracts emoji packs  
`veterancy` - extracts veterancy data  
`bundle` - extracts bundles  
`boost` - extracts boosts  
`lootchest` - extracts loot chests  
`typedescription` - extracts type description data  
`map` - extracts map data

To extract images, if available for the data type, add `:i` or `:images` to the extractor name (e.g. `hero:i` or `hero:images`).

Example selecting multiple data extractors along with spray images:
```
-e hero -e spray:i -e emoticon
```
The output directory structure:
```
output-directory/
├── data/
│   ├── emoticondata_20000_enus.json
│   ├── herodata_20000_enus.json
│   └── spraydata_20000_enus.json
└── images/
    └── sprays/
        ├── image1.png
        ├── image2.png
        ├── ...
```
Example of having map extractor along with two other data extractors:
```
-e hero -e announcer -e map
```
The output directory structure:
```
output-directory/
└── data/
    ├── maps/
    │   ├── blackhearts_revenge/
    │   │   ├── announcerdata_20000_enus.patch.json
    │   │   └── herodata_20000_enus.patch.json
    │   ├── lost_cavern/
    │   │   └── herodata_20000_enus.patch.json
    │   ├── ...
    ├── announcerdata_20000_enus.json
    ├── herodata_20000_enus.json
    └── mapdata_20000_enus.json

```

Notes:
- Images for hero and unit include the portraits, ability, and talent icons
- Static image files are extracted in `.png` format
- Animated image files are extracted in `.apng` format
  - Sprays and emoticons are the only ones with animated images

***

### -l, --localization
Specifies the gamestring localization to parse. Can be specified multiple times to select multiple localizations. Default is `enUS`.

All enabled extractors will run for each specified localization.

`all` - selects all locales  
`enUS` - English   
`deDE` - German  
`esES` - Spanish (EU)  
`esMX` - Spanish (AL)  
`frFR` - French  
`itIT` - Italian  
`koKR` - Korean  
`plPL` - Polish  
`ptBR` - Portuguese  
`ruRU` - Russian  
`zhCN` - Chinese  
`zhTW` - Chinese (TW)  

Example selecting multiple locales.
```
-l enus -l dede -l kokr
```

***

### -g, --gamestring-text
Specifies the format of the strings that are parsed from the gamestring files. Default is `RawText`.

`RawText` is the recommended choice as it can be converted to a different format later (see [`gamestring-text format`](#gamestring-text-format) command) or be converted with a custom parser.

`ColoredText` is the other recommended choice, as it is the ingame format.

Some of these may require additional parsing for a friendly readable output. Visit the [wiki page](https://github.com/HeroesToolChest/HeroesDataParser/wiki/Parsing-GameStrings) for parsing tips.

`0` - `RawText`  
The raw output of the gamestring. Contains the colored tags `<c>` (constant) or `<s>` (style), scaling data `~~x~~`, and newlines `<n/>`. It can also contain error tags `##ERROR##`.

Example:  
```
Fires a laser that deals <c val="#TooltipNumbers">200</c><c val="#ColorGray">~~0.04~~</c> damage.<n/>Does not affect minions.
```

`1` - `PlainText`   
Plain text without any colored tags, scaling info, or newlines. Newlines are replaced with a single space.

Example:  
```
Fires a laser that deals 200 damage. Does not affect minions.
```

`2` - `PlainTextWithNewlines`    
Same as `PlainText` but contains newlines.

Example:  
```
Fires a laser that deals 200 damage.<n/>Does not affect minions.
```

`3` - `PlainTextWithScaling`    
Same as `PlainText` but contains the scaling info `(+x% per level)`.

Example:  
```
Fires a laser that deals 200 (+4% per level) damage. Does not affect minions.
```

`4` - `PlainTextWithScalingWithNewlines`    
Same as `PlainTextWithScaling` but contains the newlines.

Example:  
```
Fires a laser that deals 200 (+4% per level) damage.<n/>Does not affect minions.
```

`5` - `ColoredText`    
Contains the color tags and newlines, when parsed this is what appears ingame for text and tooltips.

Example:  
```
Fires a laser that deals <c val="#TooltipNumbers">200</c> damage.<n/>Does not affect minions.
```

`6` - `ColoredTextWithScaling`    
Same as `ColoredText` but contains the scaling info with a custom constant tag.

Example:  
```
Fires a laser that deals <c val="#TooltipNumbers">200</c><c val="#ColorGray"> (+4% per level)</c> damage.<n/>Does not affect minions.
```

***

### --gs-replace-constant-vars 
For all the constant tags `<c>` in a gamestring, the variable in the `val` attribute value will be replaced with the color text hex value.

For example, `<c val="#TooltipNumbers">200</c>` would be changed to `<c val="bfd4fd">200</c>`.

***

### --gs-replace-style-vars
For all the style tags `<s>` in a gamestring, the variable in the `val` attribute value will be replaced with the color text hex value.

For example, `<s val="StandardTooltipDetails">Mana: 50</s>` would be changed to `<s val="bfd4fd">Mana: 50</s>`.

***

### --gs-preserve-constant-vars
`--gs-replace-constant-vars` is required to enable this option.

For all the constant tags `<c>` in a gamestring, the replaced variable will be preserved in a custom `hlt-name` attribute in the tag.

For example, `<c val="#TooltipNumbers">200</c>` would be changed to `<c val="bfd4fd" hlt-name="#TooltipNumbers">200</c>`.

***

### --gs-preserve-style-vars
`--gs-replace-style-vars` is required to enable this option.

For all the style tags `<s>` in a gamestring, the replaced variable will be preserved in a custom `hlt-name` attribute in the tag.

For example, `<s val="StandardTooltipDetails">Mana: 50</s>` would be changed to `<s val="bfd4fd" hlt-name="StandardTooltipDetails">Mana: 50</s>`.
***

### --localized-text
Can be set to `None`, `Extract`, or `Copy`. Default is `None`.

`None` - Gamestrings will be in the data JSON files.  
`Extract` - Gamestrings will NOT be in the data JSON files and instead the gamestrings will be saved in a created gamestrings JSON file.  
`Copy` - Gamestrings will be in both the data JSON files and a created gamestrings JSON file. 

The gamestrings JSON file will be created in the `gamestrings` subdirectory of the output directory.

If set to `Extract` or `Copy` and the `map` extractor is enabled, the map gamestrings will get its own gamestrings JSON file. The map-specific gamestrings JSON files will be created in the `maps` subdirectory of the `gamestrings` subdirectory.

Also see [localized-text import](#localized-text-import) and [localized-text export](#localized-text-export) commands.

***

### --no-map-specific
This option applies when the `map` extractor is enabled.

Normally if the `map` extractor is enabled, map-specific JSON files will be created in the `maps` subdirectories of the `data` and `gamestrings` subdirectories for each map. 
Enabling this option will disable the creation of these map-specific JSON files.

***

### --map-specific-json-output
This option applies when the `map` extractor is enabled and affects only the map-specific JSON files.

Can be set to `Normal`, `Patch`, or `All`. Default is `Patch`.

`Normal` - The full JSON files are created.  
`Patch` - JSON patch files are created.  
`All` - Both the full JSON files and JSON patch files are created.

For the patch files, the original or base files are the JSON files in the `data` or `gamestrings` subdirectories. 
The map patch files contain the differences between the base files and the map-specific files, so the full map-specific JSON files can be recreated by applying the patch files to the base files.

The command [`json-patch apply`](#json-patch-apply) can be used to apply the patch files.

## Commands
### casc-extract
```
DESCRIPTION:
Extract data files from a Heroes of the Storm directory or from online

USAGE:
    heroesdataparser casc-extract <storage-type> [OPTIONS]

ARGUMENTS:
    <storage-type>    Storage type to load from (game or online)

OPTIONS:
                                 DEFAULT
    -h, --help                              Prints help information
    -s, --storage-path <PATH>               Path to the Heroes of the Storm directory
        --download-ptr                      Download from the PTR server instead of live (online storage-type only)
    -d, --directory <PATH>       mods       Directory and its subdirectories to extract, path must start with mods (can be specified multiple times)
    -f, --filter <EXT>           *          Filter files by extension (can be specified multiple times)
    -t, --threads <NUMBER>       -1         Number of threads for file extraction (defaults to max processors)
    -o, --output-path <PATH>                Output directory for extracted files (defaults to current directory)
```
`-s, --storage-path` is required if the `storage-type` argument is set to `game`.

If the `storage-type` argument is set to `online`, the `--download-ptr` option can be used to specify whether to download from the PTR server instead of live.

Use the `-d, --directory` option to specify the directories to be extracted from the CASC storage. Ensure the provided directory paths use the correct path separators for the operating system being used.

Use the `-f, --filter` option to specify file extensions (with or without leading period) to filter by during extraction.

The output directory structure of the extracted files will mirror the directory structure in the CASC storage.
If `-o, --output-path` is not specified, then the extracted files will be in the `mods` directory in the current directory, otherwise the `mods` directory will be a subdirectory of the specified output directory.

A `hdp.info` JSON file will be created in the `mods` directory with information about the extraction.
This file is used for the root command when `mods` is specified for the `storage-type` argument.

All directories and files that are extracted will be in lowercase characters.

Example command on Windows that specifies two directories (`thefirelords.stormmod` and `tracer.stormmod`) and filters by `xml` and `txt` file extensions:
```
casc-extract game -p "E:\Games\Heroes of the Storm" -d "mods\heromods\thefirelords.stormmod" -d "mods\heromods\tracer.stormmod" -f xml -f txt 
```

Example command on Linux that specifies the same two directories and filters by the same file extensions but with `online` storage type:
```
casc-extract online -d "mods/heromods/thefirelords.stormmod" -d "mods/heromods/tracer.stormmod" -f xml -f txt 
```

***

### json-patch apply
```
DESCRIPTION:
Patch a JSON file with a JSON patch file

USAGE:
    heroesdataparser json-patch apply <file-path> <patch-file-path> [OPTIONS]

ARGUMENTS:
    <file-path>          Path to the original JSON file
    <patch-file-path>    Path to the JSON patch file

OPTIONS:
    -h, --help                  Prints help information
    -o, --output-path <PATH>    Output directory for the created file (defaults to the patch file's directory)
        --overwrite             Allow the created file to overwrite an existing file
        --delete-patch-file     Delete the patch file after applying it
        --no-indent             Disable indentation in output JSON files
```
The file name of the created (patched) file is based on the patch file name. It will be the same file name but with `.patch` removed from the name.

Example command of applying a patch where the patch file is in a different directory:
```
json-patch apply "path\to\announcerdata_20000_enus.json" "path\to\patch\announcerdata_96477_enus.patch.json"
```

***

### json-patch create
```
DESCRIPTION:
Create a JSON patch file from two JSON files

USAGE:
    heroesdataparser json-patch create <old-file-path> <new-file-path> [OPTIONS]

ARGUMENTS:
    <old-file-path>    Path to the original JSON file
    <new-file-path>    Path to the updated JSON file

OPTIONS:
    -h, --help                  Prints help information
    -o, --output-path <PATH>    Output directory for the created patch file (defaults to the new file path directory)
        --overwrite             Allow the created patch file to overwrite an existing file
        --no-indent             Disable indentation in output JSON files
```
The file name of the created patch file is based on the new file name. It will be the same file name but with `.patch` inserted before the file extension.

Example command of creating JSON patch file where the new file is in a different directory than the old file:
```
json-patch create "path\to\old\announcerdata_20000_enus.json" "path\to\new\announcerdata_20000_enus.json"
```

***

### gamestring-text format 
```
DESCRIPTION:
Format gamestring text in a data or gamestring file

USAGE:
    heroesdataparser gamestring-text format <file-path> <type> [OPTIONS]

ARGUMENTS:
    <file-path>    Path to the data or gamestring file
    <type>         Target format for the gamestrings

OPTIONS:
                                      DEFAULT
    -h, --help                                   Prints help information
        --hlt-constant-mode <MODE>    None       Mode for removing hlt-name attributes from constant tags
        --hlt-style-mode <MODE>       None       Mode for removing hlt-name attributes from style tags
    -o, --output-path <PATH>                     Output directory for the created file (defaults to the input file's directory)
        --overwrite                              Allow the created file to overwrite an existing file
        --no-indent                              Disable indentation in output JSON files
```
For the argument `<type>`, the options are the same as the [`-g, --gamestring-text`](#-g---gamestring-text) option in the root command.

For options `--hlt-constant-mode` and `--hlt-style-mode`, the modes are as follows:

`0` - `None` - No changes are made to the `hlt-name` attributes in the tags.  
`1` - `Remove` - The `hlt-name` attributes are removed from the tags.  
`2` - `RemoveAndUndo` - The `val` attribute value is replaced with the `hlt-name` attribute value and the `hlt-name` attribute is removed.

Overwriting an existing file with the created file is not allowed by default, enable the `--overwrite` option to allow overwriting or specify a different output directory with the `-o, --output-path` option.

> [!NOTE]
> Although it is allowed to format _from_ and _to_ any gamestring format types, some conversions are not reversible or may not produce the expected result. 
> For example, if a file with gamestrings in `RawText` format is converted to `PlainText`, the gamestrings cannot be converted back to `RawText` since the necessary information (such as color tags and scaling data) is lost during the conversion.
> Likewise, converting from `ColoredText` to `ColoredTextWithScaling` will not add scaling data since that information is not present in the `ColoredText` format.
> This command is therefore best used to convert from `RawText` to any other format.

Example command of gamestrings file of `RawText` format being converted to `PlainText` format:
```
gamestring-text format "path\to\gamestrings_96477_enus.json" 1 --overwrite
```

Example command of gamestrings file of `RawText` keeping the same format, but with `hlt-name` attributes removed:
```
gamestring-text format "path\to\gamestrings_96477_enus.json" 0 --hlt-constant-mode 1 --hlt-style-mode 1
```
***

### schema export data 
```
DESCRIPTION:
Export JSON schema for data files

USAGE:
    heroesdataparser schema export data [OPTIONS]

OPTIONS:
    -h, --help                     Prints help information
    -e, --extractor <EXTRACTOR>    Extractor types for the JSON schema export (can be specified multiple times)
    -o, --output-path <PATH>       Output directory for the created files (defaults to current directory)
        --overwrite                Allow created files to overwrite existing files
        --no-indent                Disable indentation in output JSON files
```
For option `-e, --extractor` at least extractor one is required and the extractor types are the same as the ones in the [`-e, --extractor`](#-e---extractor) option in the root command.

The data JSON schema files will be created in the `schema` subdirectory of the output directory.

Example command of exporting the hero and announcer data schema:
```
schema export data -e hero -e announcer
```

Example command of exporting all the data schemas:
```
schema export data -e all
```

***

### schema export gamestrings 
```
DESCRIPTION:
Export JSON schema for the gamestrings file

USAGE:
    heroesdataparser schema export gamestrings [OPTIONS]

OPTIONS:
    -h, --help                  Prints help information
    -o, --output-path <PATH>    Output directory for the created files (defaults to current directory)
        --overwrite             Allow created files to overwrite existing files
        --no-indent             Disable indentation in output JSON files
```
The gamestrings JSON schema file will be created in the `schema` subdirectory of the output directory.

There is only one gamestrings JSON schema file.

Example command:
```
schema export gamestrings
```

***

### localized-text import
```
DESCRIPTION:
Copy over gamestrings from a gamestrings file to a data file

USAGE:
    heroesdataparser localized-text import <data-file-path> <gamestrings-file-path> [OPTIONS]

ARGUMENTS:
    <data-file-path>          Path to the data file
    <gamestring-file-path>    Path to the gamestrings file

OPTIONS:
    -h, --help                  Prints help information
    -o, --output-path <PATH>    Output directory for the created file (defaults to the input data file's directory)
        --overwrite             Allow the created file to overwrite an existing file
        --no-indent             Disable indentation in output JSON files
```
The data file does not need to contain the gamestring properties. If it already contains the gamestring properties, they will be overwritten.

The `heroesVersion`, `hdpVersion`, and `mapName` properties must match between the data file and the gamestring file.

The gamestring file's `dataTypes` property must contain the data file's `dataType`.

> [!NOTE]
> The created file will have the same name as the input data file.
> If the file name contains the locale (e.g., `enus`) but the gamestring file has a different locale, the file name will remain unchanged.

Example command of importing gamestrings from a gamestrings file to a data file:
```
localized-text import "path\to\herodata_96477.json" "path\to\gamestrings_96477_enus.json" -o "path\to\output\directory"
```

***

### localized-text export
```
DESCRIPTION:
Copy over or extract gamestrings from a data file to a gamestrings file

USAGE:
    heroesdataparser localized-text export <data-file-path> <extract-type> [OPTIONS]

ARGUMENTS:
    <data-file-path>    Path to the data file
    <extract-type>      Action to perform on gamestring properties

OPTIONS:
    -h, --help                            Prints help information
    -g, --gamestrings-file-path <PATH>    Path to the gamestrings file
    -o, --output-path <PATH>              Output directory for the created files (defaults to the input data file's directory)
        --overwrite                       Allow the created files to overwrite existing files
        --no-indent                       Disable indentation in output JSON files
```
For argument `<extract-type>`, the types are as follows:

`0` - `Copy` - Gamestring properties will be copied to a gamestrings file but will remain in the data file..  
`1` - `Extract` - Gamestring properties will be extracted to a gamestrings file and removed from the data file.  
`2` - `Remove` - Gamestring properties will be removed from the data file but will NOT be saved to a gamestrings file.

If option `-g, --gamestrings-file-path` is not specified, a new gamestrings file will be created. If it is specified, then the gamestrings file will be updated with the gamestrings from the data file.

If option `-g, --gamestrings-file-path` is specified, the `heroesVersion`, `hdpVersion`, `mapName`, and `gameStringText` properties must match between the data file and the gamestrings file.
Also, the `dataTypes` property of the gamestrings file must NOT already contain the `dataType` of the data file.

Example command of extracting the gamestrings from the data file, two files will be created at the output directory:
```
localized-text export "path\to\herodata_96477_enus.json" extract -o "path\to\output\directory"
```

Example command of copying the gamestrings from the data file to an existing gamestrings file, the gamestrings file will be updated:
```
localized-text export "path\to\spraydata_96881_enus.json" copy -g "path\to\gamestrings_96881_enus.json" --overwrite
```

***

### portrait info
```
DESCRIPTION:
Display information about a reward portraits data file

USAGE:
    heroesdataparser portrait info <rewardportrait-file-path> [OPTIONS]

ARGUMENTS:
    <rewardportrait-file-path>    Path to the rewardportrait data JSON file

OPTIONS:
    -h, --help                           Prints help information
    -t, --texture-sheets                 Display all reward portrait texture sheet file names
    -s, --icon-slot <SLOT>               Display all icon slot names along with the texture sheet image file name for the given slot
    -i, --texture-sheet-image <IMAGE>    Display all reward portrait names and their icon slots for the specified texture sheet image
```
Specify at least one of the options to display the corresponding information about the reward portraits data file.

Ensure that the provided reward portrait data JSON file contains gamestrings (i.e., it was not created with `--localized-text` set to `Extract`).

Example command:
```
portrait info "path\to\rewardportraitdata_96477_enus.json" -s 0
```

***

### portrait battlenet-cache
```
DESCRIPTION:
Copy .wafl files from the Battle.net cache directory or a custom directory to the output directory

USAGE:
    heroesdataparser portrait battlenet-cache [OPTIONS]

OPTIONS:
    -h, --help                      Prints help information
    -c, --battlenet-cache <PATH>    Path to the Battle.net cache directory
    -o, --output-path <PATH>        Output directory for copied texture sheets (defaults to current directory)
```
If the `-c, --battlenet-cache` option is not specified, then an attempt will be made to find the Battle.net cache directory automatically.

The file extensions of the files are automatically converted into its proper image format (`.dds`, `.png`, `.jpg`, etc).

> [!NOTE]
> This copies ALL .wafl files, which Starcraft II uses as well.

Example command:
```
portrait battlenet-cache -c "path\to\battlenet\cache"
```

***

### portrait extract
```
DESCRIPTION:
Extract reward portraits from texture sheets

USAGE:
    heroesdataparser portrait extract <rewardportrait-file-path> [OPTIONS]

ARGUMENTS:
    <rewardportrait-file-path>    Path to the rewardportrait data JSON file

OPTIONS:
    -h, --help                                    Prints help information
    -i, --texture-sheet-image <IMAGE>             Texture sheet image name from the rewardportrait data JSON file
    -c, --cache-texture-sheet-image <FILEPATH>    Path to the texture sheet image file to extract portraits from
        --delete-texture-sheet                    Delete texture sheet after extracting portraits
    -o, --output-path <PATH>                      Output directory for extracted portraits (defaults to current directory)
```
If either `-i, --texture-sheet-image` or `-c, --cache-texture-sheet-image` option is not specified, the user will be prompted for the missing option.

Example command:
```
portrait extract "path\to\rewardportraitdata_96477_enus.json" -i "portrait_texture_sheet_0.png" -c "path\to\cache\3f2859d0a3e0e3d9ad00331a6fe5e164aef1e8a2539b3a1730677ecf45283f3f.dds"
```

***

### portrait extract-auto
```
DESCRIPTION:
Automatically extract reward portraits from texture sheets

USAGE:
    heroesdataparser portrait extract-auto <rewardportrait-file-path> [OPTIONS]

ARGUMENTS:
    <rewardportrait-file-path>    Path to the rewardportrait data JSON file

OPTIONS:
    -h, --help                      Prints help information
    -c, --battlenet-cache <PATH>    Path to the Battle.net cache directory
    -x, --xml-config <FILEPATH>     XML config file used for auto extraction
        --delete-texture-sheet      Delete texture sheet after extracting portraits
    -o, --output-path <PATH>        Output directory for extracted portraits (defaults to current directory)
```
If the `-c, --battlenet-cache` option is not specified, then an attempt will be made to find the Battle.net cache directory automatically.

`-x, --xml-config` is for an optional XML config file, by default it will use the built-in XML config file.

Example command:
```
portrait extract-auto "path\to\rewardportraitdata_96477_enus.json" -c "path\to\battlenet\cache"
```

## Developing
To build and compile the code, it is recommended to use the latest version of [Visual Studio 2026 or Visual Studio Code](https://visualstudio.microsoft.com/downloads/).

Another option is to use the dotnet CLI tools from the latest [.NET SDK](https://dotnet.microsoft.com/download).

## License
[MIT license](/LICENSE)
