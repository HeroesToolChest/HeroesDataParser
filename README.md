# Heroes Data Parser
[![Build Status](https://dev.azure.com/kevinkoliva/Heroes%20of%20the%20Storm%20Projects/_apis/build/status/koliva8245.HeroesDataParser?branchName=master)](https://dev.azure.com/kevinkoliva/Heroes%20of%20the%20Storm%20Projects/_build/latest?definitionId=1) [![Release](https://img.shields.io/github/release/koliva8245/HeroesDataParser.svg)](https://github.com/koliva8245/HeroesDataParser/releases/latest)
[![NuGet](https://img.shields.io/nuget/v/HeroesDataParser.svg)](https://www.nuget.org/packages/HeroesDataParser/)

Heroes Data Parser is a .NET Core command line tool that extracts Heroes of the Storm game data into XML and JSON files. Extracts hero data along with all abilities, talents, and their respective portraits and icons.

Also extracts the following:
 - Match Awards (includes images)
 - Hero Skins
 - Mounts
 - Banners
 - Sprays (includes images)
 - Announcers (includes images)
 - Voice Lines (includes images)
 - Portraits
 - Emoticons (includes images)
 - Emoticon Packs
 
Visit the [wiki](https://github.com/koliva8245/HeroesDataParser/wiki) for some more information and examples of XML and JSON output.

## Installation
### Supported Operating Systems
- Windows 7 SP1 (x64) and later
- Linux (x64)
- macOS 10.12 (x64) and later versions

***

### Dotnet Global Tool (Recommended)
Download and install the [.NET Core 2.2 SDK](https://www.microsoft.com/net/download/windows) or newer. 

Once installed, run the following command:
```
dotnet tool install --global HeroesDataParser
```

Installing via this method also allows easy updating to future versions using the following command:
```
dotnet tool update --global HeroesDataParser
```

***

### Zip File Download - Framework-Dependent Deployment

Download and install the [.NET Core 2.2 Runtime or SDK](https://www.microsoft.com/net/download/windows) or newer. 

Download and extract the latest `HeroesDataParser.*-fdd-any.zip` file from the [releases](https://github.com/koliva8245/HeroesDataParser/releases) page.

***

### Zip File Download - Framework-Dependent Executable
Download and install the [.NET Core 2.2 Runtime or SDK](https://www.microsoft.com/net/download/windows) or newer. 

Download and extract the latest `HeroesDataParser.*-fde-<OS>-x64.zip` file from the [releases](https://github.com/koliva8245/HeroesDataParser/releases) page for a selected operating system.

***

### Zip File Download - Self-Contained Deployment
No runtime or SDK is required.

Download and extract the latest `HeroesDataParser.*-scd-<OS>-x64.zip` file from the [releases](https://github.com/koliva8245/HeroesDataParser/releases) page for a selected operating system.

This zip file contains everything that is needed to run the dotnet core app without .NET Core being installed, so the zip file is quite large.

## Usage
If installed as a Dotnet Global Tool, the app can be run with one of the following commands:
```
dotnet heroes-data -h
dotnet-heroes-data -h
```

If installed as a Framework-Dependent Deployment, run the following command from the extracted directory:
```
dotnet heroesdata.dll -h
```

If installed as a Framework-Dependent Executable or Self-Contained Deployment, run one of the following commands from the extracted directory:
```
./heroesdata -h
dotnet heroesdata.dll -h
```

Output of the -h option
```
Heroes Data Parser (VERSION)

Usage:  [options] [command]

Options:
  -?|-h|--help                      Show help information
  -v|--version                      Show version information
  -s|--storage-path <FILEPATH>      The 'Heroes of the Storm' directory or an already extracted 'mods' directory.
  -o|--output-directory <FILEPATH>  Sets the output directory.
  -d|--description <VALUE>          Sets the description output type (0 - 6) - Default: 0.
  -e|--extract-data <VALUE>         Extracts data files - Default: herodata.
  -i|--extract-images <VALUE>       Extracts image files.
  -l|--localization <LOCALE>        Sets the gamestring localization(s) - Default: enUS.
  -b|--build <NUMBER>               Sets the override build file(s).
  -t|--threads <NUMBER>             Limits the maximum amount of threads to use.
  --xml                             Creates xml output.
  --json                            Creates json output.
  --file-split                      Splits the XML and JSON file(s) into multiple files.
  --localized-text                  Extracts localized gamestrings from the XML and JSON file(s) into a text file.
  --minify                          Creates .min file(s) along with current output file(s).
  --warnings                        Displays all validation warnings.

Commands:
  extract  Extracts all required files from the `Heroes of the Storm` directory.
  image    Performs image processing.
  list     Displays .txt, .xml, and .json files in the local directory.
  read     Reads a .txt, .xml, or .json file and displays its contents on screen.

Use " [command] --help" for more information about a command.
```

Example command to create xml and json files from the `Heroes of the Storm` directory. Since no `-o|--outputDirectory` option is set, it defaults to the local directory.
```
dotnet heroes-data -s 'D:\Games\Heroes of the Storm' --xml --json
```

## Validation Warnings
All the warnings do not need to be fixed, they are shown for awareness.  
**Tooltip strings that fail to parse will show up __empty__** in the xml or json files and thus will be a valid warning.  
Warnings can be shown to the console using the option `--warnings`.  
Ignored warnings are in `verifyignore.txt`.  
Ignored warnings only work for a majority of english strings.  

## Options
### Storage Path (-s|--storage-path) 
There are two types of paths that can be provided for this option. One is the `Heroes of the Storm` directory and the other is an already extracted `mods` directory.

If this option is not provided, it will look for the `Heroes of the Storm` files in the local directory or an extracted `mods` directory.

The `extract` command is available to use to extract the mods directory and all required files.

The `mods` directory can also have a build suffix in its name. [More info](https://github.com/koliva8245/HeroesDataParser/tree/master#mods-suffix-directory).

***

### Output Directory (-o|--output-directory)
If this option is not provided, it will default to the install directory under the directory `output`.
 
***
 
### Description (-d|--description)
Sets the description/tooltip output type (0 - 6).  
 - Default is 0 and is the recommended choice as it can be parsed to suit multiple verbiage
 - 5 is the other recommended choice, as it is the ingame verbiage

Some of these may require parsing for a readable output. Visit the [wiki page](https://github.com/koliva8245/HeroesDataParser/wiki/Parsing-Descriptions) for parsing tips.

`0` - RawDescription (Default)  
The raw output of the description. Contains the color tags `<c val=\"#TooltipNumbers\"></c>`, scaling data `~~x~~`, and newlines `<n/>`.

Example:  
```
Fires a laser that deals <c val=\"#TooltipNumbers\">200~~0.04~~</c> damage.<n/>Does not affect minions.
```

`1` - PlainText  
Plain text without any color tags, scaling info, or newlines.  Newlines are replaced with a double space.

Example:  
```
Fires a laser that deals 200 damage.  Does not affect minions.
```

`2` - PlainTextWithNewlines  
Same as PlainText but contains newlines.

Example:  
```
Fires a laser that deals 200 damage.<n/>Does not affect minions.
```

`3` - PlainTextWithScaling  
Same as PlainText but contains the scaling info `(+x% per level)`.

Example:  
```
Fires a laser that deals 200 (+4% per level) damage.  Does not affect minions.
```

`4` - PlainTextWithScalingWithNewlines  
Same as PlainTextWithScaling but contains the newlines.

Example:  
```
Fires a laser that deals 200 (+4% per level) damage.<n/>Does not affect minions.
```

`5` - ColoredText  
Contains the color tags and newlines. When parsed, this is what appears ingame for tooltips.

Example:  
```
Fires a laser that deals <c val=\"#TooltipNumbers\">200</c> damage.<n/>Does not affect minions.
```

`6` - ColoredTextWithScaling  
Same as ColoredText but contains the scaling info.

Example:  
```
Fires a laser that deals <c val=\"#TooltipNumbers\">200 (+4% per level)</c> damage.<n/>Does not affect minions.
```

***

### Extract Data (-e|--extract-data)
Extracts the data files. Multiple are allowed. Default is `herodata`.  

Extracts to `<OUTPUT-DIRECTORY>/<json and/or xml>`.

`all` - extracts all data files  
`herodata` - extracts hero data    
`matchawards` - extracts match awards  
`heroskins` - extracts hero skins  
`banners` - extracts banners  
`sprays` - extracts sprays  
`announcers` - extracts announcers  
`voicelines` - extracts voicelines  
`portraits` - extracts portraits  
`emoticons` - extracts emojis  
`emoticonpacks` - extracts the emoji packs  

Example seleting multiple data extractions.
```
-e herodata -e sprays -e emoticons
```
***

### Extract Images (-i|--extract-images)
Extracts the images that were referenced in the xml or json file(s) from the `-e|--extract-data` option. Multiple are allowed.  

Extracts to `<OUTPUT-DIRECTORY>/images/<image-type>`  

`all` - extracts all images files  
`heroportraits` - extracts hero portrait images (HeroSelect, Leaderboard, Loading, PartyPanel, and Target portraits)  
`abilities` - extracts ability icons  
`talents` - extracts talent icons  
`abilitytalents` - extracts both ability and talent icons into the same directory (overrides `abilities` and `talents` choices)  
`matchawards` - extracts match award icons  
`sprays` - extracts spray images  
`announcers` - extracts announcer images  
`voicelines` - extracts voiceline images  
`emoticons` - extracts emoji icons

`all-split` - sets all options except for `abilityTalents`  
`herodata` - sets `heroportraits`, `abilitytalents`  
`herodata-split` - sets `heroportraits`, `abilities`, `talents`

Notes:
- Static image files are extracted in `.png` format
- Animated image files are extracted in `.gif` format
  - Sprays and emoticons are the only ones with animated images
- Due to the quality limitations of gifs, the texture files used for the creation of the gifs are also extracted in `.png` format
  - Information about creating animations can be found in the [wiki](https://github.com/koliva8245/HeroesDataParser/wiki/Animated-Images)

Example selecting multiple image extractions.
```
-i abilities -i talents -i sprays
```
***

### Localization (-l|--localization)
Sets the game string localization (descriptions/tooltips) parsing. Multiple are allowed, default is `enUS`.

`all` - selects all locales  
`enUS` - English (Default)  
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

### Localized Text (--localized-text)
Strings that are localized are removed from the XML and JSON file(s) and are instead put into a text file to allow easy swapping between localizations. The file(s) are sorted alphabetically and each line can be read in as a key-value pair (split on `=`). 

The gamestring text file(s) are located at `<OUTPUT-DIRECTORY>/gamestrings/`

The format of the strings in the text file are the following:
- `abiltalent/name/[nameId]=[value]`
- `announcer/name/[Id]=[value]`
- `announcer/description/[Id]=[value]`
- `announcer/sortname/[Id]=[value]`
- `award/name/[Id]=[value]`
- `award/description/[Id]=[value]`
- `banner/name/[Id]=[value]`
- `banner/description/[Id]=[value]`
- `banner/sortname/[Id]=[value]`
- `emoticon/alias/[Id]=[value]`
- `emoticon/description/[Id]=[value]`
- `emoticon/name/[Id]=[value]`
- `emoticon/searchtext/[Id]=[value]`
- `emoticonpack/description/[Id]=[value]`
- `emoticonpack/name/[Id]=[value]`
- `emoticonpack/sortname/[Id]=[value]`
- `heroskin/info/[Id]=[value]`
- `heroskin/name/[Id]=[value]`
- `heroskin/searchtext/[Id]=[value]`
- `heroskin/sortname/[Id]=[value]`
- `mount/info/[Id]=[value]`
- `mount/name/[Id]=[value]`
- `mount/searchtext/[Id]=[value]`
- `mount/sortname/[Id]=[value]`
- `portrait/name/[Id]=[value]`
- `portrait/sortname/[Id]=[value]`
- `spray/description/[Id]=[value]`
- `spray/name/[Id]=[value]`
- `spray/searchtext/[Id]=[value]`
- `spray/sortname/[Id]=[value]`
- `tooltip/cooldown/[nameId]=[value]`
- `tooltip/energy/[nameId]=[value]`
- `tooltip/full/[fullTooltipId]=[value]`
- `tooltip/life/[nameId]=[value]`
- `tooltip/short/[shortTooltipId]=[value]`
- `unit/description/[Id]=[value]`
- `unit/difficulty/[Id]=[value]`
- `unit/expandedRole/[Id]=[value]`
- `unit/name/[Id]=[value]`
- `unit/role/[Id]=[value] (comma delimited if more than 1 role)`
- `unit/searchtext/[Id]=[value]`
- `unit/title/[Id]=[value]`
- `unit/type/[Id]=[value]`
- `voiceline/description/[Id]=[value]`
- `voiceline/name/[Id]=[value]`
- `voiceline/sortname/[Id]=[value]`

## Commands
### Extract
```
Usage:  extract [arguments] [options]

Arguments:
  storage-path  The 'Heroes of the Storm' directory

Options:
  -?|-h|--help                      Show help information
  -o|--output-directory <FILEPATH>  Sets the output directory.
  --textures                        Includes extracting all textures (.dds).
```

Extracts all required files from the `Heroes of the Storm` directory which can be used for the `-s|--storage-path` option.  

If the `-o|--output-directory` is not set, the local directory will be used. A `mods` directory will be created as the base directory.

Example command that will extract all required files including the textures.
```
extract 'D:\Games\Heroes of the Storm' --textures
```

***

### Image
```
Usage:  image [arguments] [options]

Arguments:
  file-name  The filename, file path, or directory containing the images to process.

Options:
  -?|-h|--help                      Show help information
  --width <VALUE>                   Sets the new width.
  --height <VALUE>                  Sets the new height.
  --png-compress                    Sets an png image bit depth to 8 bits
  -o|--output-directory <FILEPATH>  Sets the output directory.
```

Performs image processing (`.png`, `.jpg`, or `.gif`) to a single file or multiple files in a directory.

By default, if the `-o|--output-directory` option is not set the new processed images will be saved in the local directory, overriding the existing image.

Example commands. First command compresses a single image.  Second command resizes all images in the `.\Images` subdirectory and saves them in the `.\Images\New` subdirectory.
```
image storm_ui_icon_abathur_spawnlocust.png --png-compress
image '.\Images' -o '.\Images\New' --width 64 --length 64
```
***

### List
```
Usage:  list [options]

Options:
  -?|-h|--help         Show help information
  -f|--files           Displays all files.
  -d|--directories     Displays all directories.
  -s| --set-directory  Sets a relative directory to display
```

Displays `.txt`, `.xml`, and `.json` in the local directory.  Use the option `-s|--set-directory` to view subdirectories.

Example command that displays all files and directories.
```
list -f -d
```

***

### Read
```
Usage:  read [arguments] [options]

Arguments:
  file-name  The filename or relative file path to read and display on the console. Must be a .txt, .xml, or .json file.

Options:
  -?|-h|--help  Show help information
```

Reads a `.txt`, `.xml`, or `.json` file and displays its contents on screen.

Example command that reads and displays the `parserhelper.xml` file.
```
read .\parserhelper.xml
```

## Advanced Features
### Mods suffix directory
The `mods` directory may have a `_<build number>` suffix in its name. The build number determines the overrides file(s) to load. If the overrides file does not exist and the build number is greater than the highest overrides file then it will load the default overrides file otherwise it will load next **lowest** overrides file.

Example:
```
directory to load: mods_13500

hero overrides files:
hero-overrides.xml
hero-overrides_12000.xmls
hero-overrides_13000.xml <--- will be loaded
hero-overrides_14000.xml

directory to load: mods_14100

hero overrides files:
hero-overrides.xml <--- will be loaded
hero-overrides_12000.xml
hero-overrides_13000.xml 
hero-overrides_14000.xml
```

***

### Multi-mods directory
There can be multiple mods directories with the suffix `_<build number>` in the same directory.  If the selected parent directory is the storage path, the highest build number suffix diretory will be parsed.

For example, with this directory:
```
modFolders/
|--mods_22000/
   |--(FILES)
|--mods_22100/
   |--(FILES)
|--mods_22388/
   |--(FILES)
```
Setting `modFolders` as the storage path will have the `mods_22388` directory parsed.

***

### CASC HeroOverrides loading
When using a `Heroes of the Storm` directory, it will load the equivalent overrides file(s) based on the build version, just like in the [mods suffix directory](https://github.com/koliva8245/HeroesDataParser/tree/master#mods-suffix-directory).

***

## Developing
To build and compile the code, it is recommended to use the latest version of [Visual Studio 2019 or Visual Studio Code](https://visualstudio.microsoft.com/downloads/).

Another option is to use the dotnet CLI tools from the [.NET Core 2.2 SDK](https://www.microsoft.com/net/download/windows).

The main project is `HeroesData.csproj` and the main entry point is `Program.cs`.

Both the `CASCLibCore.csproj` and `Heroes.Models.csproj` projects are submodules. Any code changes should be commited to those respective repositories.

## License
[MIT license](/LICENSE)
