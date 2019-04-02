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

### Zip File Download - Framework-Dependent
Download and install the [.NET Core 2.2 Runtime or SDK](https://www.microsoft.com/net/download/windows) or newer. 

Download and extract the latest `HeroesDataParser.*-fdd-any.zip` file from the [releases](https://github.com/koliva8245/HeroesDataParser/releases) page.

***

### Zip File Download - Self-Contained
No runtime or SDK is required.

Download and extract the latest `HeroesDataParser.*-scd-x64.zip` file from the [releases](https://github.com/koliva8245/HeroesDataParser/releases) page for a selected operating system.

This zip file contains everything that is needed to run the dotnet core app without .NET Core being installed, so the zip file is quite large.

## Usage
If installed as a global tool, the app can be run with one of the following commands:
```
dotnet heroes-data -h
dotnet-heroes-data -h
```
If one of the zip files was downloaded, run one of the following commands from the extracted directory:
```
dotnet heroesdata.dll -h

// only for scd
./heroesdata -h
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
  -i|--extract-images <VALUE>       Extracts image files, only available using the Heroes of the Storm game directory.
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
  extract
  read

Use " [command] --help" for more information about a command.
```

Example command to create xml and json files from the `Heroes of the Storm` directory. Since no `-o|--outputDirectory` option is set, it defaults to the install directory.
```
dotnet heroes-data -s 'D:\Games\Heroes of the Storm Public Test' --xml --json
```

## Validation Warnings
All the warnings do not need to be fixed, they are shown for awareness.  
**Tooltip strings that fail to parse will show up __empty__** in the xml or json files and thus will be a valid warning.  
Warnings can be shown to the console using the option `--warnings`.  
Ignored warnings are in `verifyignore.txt`.  
Ignored warnings only work for a majority of english strings.  

## Options
### Storage Path (-s|--storage-path) 
There are two types of paths that can be provided for this option. One is the directory path of the `Heroes of the Storm` directory and the other is an already extracted `mods` directory.

The `extract` command is available to use to extract the mods directory and all required files.

The `mods` directory can also have a build suffix in its name. [More info](https://github.com/koliva8245/HeroesDataParser/tree/master#mods-suffix-directory).

***

### Output Directory (-o|--output-directory)
If this option is not provided, it will default to the install directory under the directory `output`.
 
***
 
### Description (-d|--description)
Sets the description/tooltip output type (0 - 6)

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

Extracts to `<OUTPUT-DIRECTORY>/<json and/or xml>`  

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

Example seleting multiple data extractions
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
`abilitytalents` - extracts both ability and talent icons into the same directory  
`matchawards` - extracts match award icons  
`sprays` - extracts spray images  
`announcers` - extracts announcer images  
`voicelines` - extracts voiceline images  
`emoticons` - extracts emoji icons  

Notes:
- This option only works if a `Heroes of the Storm` directory path is provided for the `-s|--storage-path` option
- Static image files are extracted in `.png` format
- Animated image files are extracted in `.gif` format
  - Sprays and emoticons are the only ones with animated images
- Due to the quality limitations of gifs, the texture files used for the creation of the gifs are also extracted in `.png` format
- Information about creating the animations can be found in the [wiki](https://github.com/koliva8245/HeroesDataParser/wiki/Animated-Images)

Example selecting multiple image extractions
```
-i abilities -i talents -i sprays
```
***

### Localization (-l|--localization)
Sets the game string localization (descriptions/tooltips). Multiple are allowed, use `all` to select all. The application will parse all game strings and hero data for each locale selected.

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

Example selecting multiple locales
```
-l enus -l dede -l kokr
```

***

### Localized Text (--localized-text)
Strings that are localized are removed from the XML and JSON file(s) and are instead put into a text file to allow easy swapping between localizations. The file(s) are sorted alphabetically and each line can be read in as a key-value pair (split on `=`). 

The gamestring text file(s) are located at `<OUTPUT-DIRECTORY>/gamestrings/`

The following are all localized strings that are removed:
- Hero/Unit: `name`, `difficulty`, `type`, `role`, `description`
- Ability/Talent: `name`, `lifeTooltip`, `energyTooltip`, `cooldownTooltip`, `shortTooltip`, `fullTooltip`
- MatchAwards: `name`, `description`

The format of the strings in the text file are the following:
- `unit/name/[hero.shortname]=[value]`
- `unit/difficulty/[hero.shortname]=[value]`
- `unit/type/[hero.shortname]=[value]`
- `unit/role/[hero.shortname]=[value] (comma delimited if more than 1 role)`
- `unit/description/[hero.shortname]=[value]`
- `unit/title/[hero.shortname]=[value]`
- `unit/searchtext/[hero.shortname]=[value]`
- `abiltalent/name/[nameId]=[value]`
- `tooltip/life/[nameId]=[value]`
- `tooltip/energy/[nameId]=[value]`
- `tooltip/cooldown/[nameId]=[value]`
- `tooltip/short/[shortTooltipId]=[value]`
- `tooltip/full/[fullTooltipId]=[value]`
- `award/name/[shortname]=[value]`
- `award/description/[shortname]=[value]`


## Commands
### Read
```
Options:
  -?|-h|--help               Show help information
  -f|--file-name <filename>  The filename of the file to read and display on the console.
  -v|--valid-files           Show all available files to read.
```

## Advanced Features
### Mods suffix directory
The `mods` directory may have a `_<build number>` suffix in its name. The build number determines the hero overrides file to load. If the overrides file does not exist and the build number is greater than the highest overrides file then it will load the default overrides file `hero-overrides.xml` otherwise it will load next **lowest** overrides file.

Example:
```
directory to load: mods_13500

HeroOverrides files:
hero-overrides.xml
hero-overrides_12000.xml
hero-overrides_13000.xml <--- will be loaded
hero-overrides_14000.xml

directory to load: mods_14100

HeroOverrides files:
hero-overrides.xml <--- will be loaded
hero-overrides_12000.xml
hero-overrides_13000.xml 
hero-overrides_14000.xml
```

***

### Multi-mods directory
You can have multiple mods directories with the suffix `_<build number>` in the same directory.  If you select the parent directory as the storage path, the highest build number suffix diretory will be parsed.

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
When using a `Heroes of the Storm` directory, it will load the equivalent hero overrides file, just like in the [mods suffix directory](https://github.com/koliva8245/HeroesDataParser/tree/master#mods-suffix-directory).

***

## Developing
To build and compile the code, it is recommended to use the latest version of Visual Studio 2017.

Another option is to download and install the [.NET Core 2.2 SDK](https://www.microsoft.com/net/download/windows) or newer. 

## License
[MIT license](/LICENSE)
