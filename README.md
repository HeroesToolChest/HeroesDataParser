# Heroes Data Parser
[![Build Status](https://dev.azure.com/kevinkoliva/Heroes%20of%20the%20Storm%20Projects/_apis/build/status/koliva8245.HeroesDataParser?branchName=master)](https://dev.azure.com/kevinkoliva/Heroes%20of%20the%20Storm%20Projects/_build/latest?definitionId=1) [![Release](https://img.shields.io/github/release/koliva8245/HeroesDataParser.svg)](https://github.com/koliva8245/HeroesDataParser/releases/latest)
[![NuGet](https://img.shields.io/nuget/v/HeroesDataParser.svg)](https://www.nuget.org/packages/HeroesDataParser/)

Heroes Data Parser is a .NET Core command line tool that extracts Heroes of the Storm game data into XML and JSON files. Extracts hero data along with all abilities, talents, and their respective portraits and icons.

Also extracts the following:
 - Match Awards
 
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
dotnet HeroesData.dll -h
./HeroesData -h
```
Output of the -h option
```
Heroes Data Parser (VERSION)

Usage:  [options] [command]

Options:
  -?|-h|--help                      Show help information
  -v|--version                      Show version information
  -s|--storage-path <filePath>      The 'Heroes of the Storm' directory or an already extracted 'mods' directory.
  -t|--threads <amount>             Limits the maximum amount of threads to use.
  -e|--extract <value>              Extracts images, available only in -s|--storage-path mode using the Hots directory.
  -d|--description <value>          Set the description output type (0 - 6) - Default 0.
  -b|--build <number>               Set the override build file.
  -o|--output-directory <filePath>  Set the output directory.
  -l|--localization <locale>        Set the gamestring localization(s) - Default: enUS.
  -f|--file-split                   Split the XML and JSON file(s) into multiple files.
  --xml                             Create xml output.
  --json                            Create json output.
  --localized-text                  Extract localized gamestrings from the XML and JSON file(s) into a text file.
  --hero-warnings                   Display all hero warnings.
  --exclude-awards                  Exclude match award parsing.
  --minify                          Create .min file(s) along with current output file(s).

Commands:
  read

Use " [command] --help" for more information about a command.
```

Example command to create xml and json files from the `Heroes of the Storm` directory. Since no `-o|--outputDirectory` option is set, it defaults to the directory of HDP.
```
dotnet heroes-data -s 'D:\Games\Heroes of the Storm Public Test' --xml --json
```

## Hero Warnings
Please be aware of the hero warnings, especially on a build with a new hero or re-worked hero.  
All the warnings do not need to be fixed, they are shown for awareness.  
**Tooltip strings that fail to parse will show up __empty__** in the xml or json files and thus will be a valid warning.  
Hero warnings can be shown to the console using the option `--hero-warnings`.  
Ignored warnings are in `VerifyIgnore.txt`.  
Ignored warnings only work for english strings.  

## Options
### Storage Path (-s|--storage-path)
There are two types of paths that can be provided for this option. One is the directory path of the `Heroes of the Storm` directory and the other is an already extracted `mods` directory containing the following file structure:  
**Note:** `enus.stormdata` is for the localization

```
mods/
|--core.stormmod/
   |--base.stormdata/gamedata/
      |--(ALL FILES)
   |--enus.stormdata/localizeddata/
      |--gamestrings.txt
|--heroesdata.stormmod/
   |--base.stormdata/gamedata/
      |--heroes/
         |--(ALL FILES)
   |--enus.stormdata/localizeddata/
      |--gamestrings.txt
|--heroesmapmods/battlegroundmapmods/
   |--(ALL FILES)
|--heromods/
   |--(ALL FILES)
```
Or a simpler way, extract these directories and file (keep the directory paths)

`mods/core.stormmod/base.stormdata/gamedata/`  
`mods/core.stormmod/enus.stormdata/localizeddata/gamestrings.txt`  
`mods/heroesdata.stormmod/base.stormdata/gamedata/`   
`mods/heroesdata.stormmod/enus.stormdata/localizeddata/gamestrings.txt`  
`mods/heroesmapmods/battlegroundmapmods/`  
`mods/heromods/`

The `mods` directory can also have a build suffix in its name. [More info](https://github.com/koliva8245/HeroesDataParser/tree/master#mods-suffix-directory).

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

### Extract (-e|--extract)
Extracts portraits and abilityTalent icons that have been referenced for a hero in the xml and json file(s). Multiple are allowed.

The extracted images are located at `<OUTPUT-DIRECTORY>/images/`

`portraits` - extracts hero portraits (HeroSelect, Leaderboard, Loading, PartyPanel, and Target portraits)  
`abilities` - extracts ability icons  
`talents` - extracts talent icons  
`abilityTalents` - extracts both ability and talent icons into the same directory  
`awards` - extracts match award icons  
`all` - performs `portraits`, `abilityTalents`, and `awards`

Notes:
- This option only works if a `Heroes of the Storm` directory path is provided for the `-s|--storage-path` option
- Files are extracted in `.png` format

Example selecting multiple extractions
```
-e abilities -e talents
```

***

### Game String Localization (-l|--localization)
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
The `mods` directory may have a `_<build number>` suffix in its name. The build number determines the hero overrides file to load. If the overrides file does not exist and the build number is greater than the highest overrides file then it will load the default overrides file `HeroesOverrides.xml` otherwise it will load next **lowest** overrides file.

Example:
```
directory to load: mods_13500

HeroOverrides files:
HeroOverrides.xml
HeroOverrides_12000.xml
HeroOverrides_13000.xml <--- will be loaded
HeroOverrides_14000.xml

directory to load: mods_14100

HeroOverrides files:
HeroOverrides.xml <--- will be loaded
HeroOverrides_12000.xml
HeroOverrides_13000.xml 
HeroOverrides_14000.xml
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

### Advanced File Configuration
For more advanced file configurations, edit the file `WriterConfig.xml`.  Options in the console override the options in the config file. Typically this file should not be modified at all.

## Developing
To build and compile the code, it is recommended to use the latest version of Visual Studio 2017.

Another option is to download and install the [.NET Core 2.2 SDK](https://www.microsoft.com/net/download/windows) or newer. 

## License
[MIT license](/LICENSE)
