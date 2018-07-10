# Heroes Data Parser
[![Build status](https://ci.appveyor.com/api/projects/status/g3linacec0a4kqkn/branch/master?svg=true)](https://ci.appveyor.com/project/koliva8245/heroesdataparser/branch/master)  [![Build Status](https://travis-ci.org/koliva8245/HeroesDataParser.svg?branch=master)](https://travis-ci.org/koliva8245/HeroesDataParser)

Heroes Data Parser is a cross platform (Windows/MacOS/Linux) command line tool that extracts Heroes of the Storm game data into XML or JSON files. Extracts hero information along with all abilities, talents, and their respective portraits and icons.

## Installation
- Windows 7 SP1 (x64 and x86) or higher 
- Linux (x64)
- macOS 10.12 and later versions

### Dotnet Global Tool (Recommended)
Download and install the [.NET Core 2.1 SDK](https://www.microsoft.com/net/download/windows) or newer. 

Once installed, run the following command:
```
dotnet tool install --global HeroesDataParser
```

Installing via this method also allows easy updating to future versions using the following command:
```
dotnet tool update --global HeroesDataParser
```

### Zip File Download - Framework-Dependent
Download and install the [.NET Core 2.1 Runtime or SDK](https://www.microsoft.com/net/download/windows) or newer. 

Download and extract the latest `HeroesDataParser.*-fdd-any.zip` file from the [releases](https://github.com/koliva8245/HeroesDataParser/releases) page.

### Zip File Download - Self-Contained
No runtime or SDK is required.

Download and extract the latest `HeroesDataParser.*-scd-*.zip` file from the [releases](https://github.com/koliva8245/HeroesDataParser/releases) page for your Operating System.

This zip file contains everything that is needed to run the dotnet core app without .NET Core being installed, so the zip file is quite large.

## Usage
If installed as a global tool, the app can be run with one of the following commands:
```
heroes-data -h
dotnet heroes-data -h
dotnet-heroes-data -h
```
If you download one of the zip files, run the following command from the extracted directory:
```
dotnet heroesdata.dll -h
```
Output of the -h option
```
Heroes Data Parser (VERSION)

Usage:  [options]

Options:
  -?|-h|--help                 Show help information
  -v|--version                 Show version information
  -s|--storagePath <filePath>  The 'Heroes of the Storm' directory or an already extracted 'mods' directory
  -t|--threads <amount>        Limits the maximum amount of threads to use
  -e|--extract <value(s)>      Extracts images, available values: all|portraits|talents. Available only in -s|--storagePath mode
  -f|--fileSplit <boolean>     Sets the file output type, if true, creates a file for each hero parsed.  Default 'false'
  -d|--description <value>     Sets the description output type (0 - 6). Default 0.
  --xml                        Create xml output
  --json                       Create json output
  --invalidFull                Show all invalid full tooltips
  --invalidShort               Show all invalid short tooltips
  --invalidHero                Show all invalid hero tooltips
  --heroWarnings               Shows all hero warnings
```

Example command to create xml and json files from the `Heroes of the Storm` directory
```
dotnet heroes-data -s 'D:\Games\Heroes of the Storm Public Test' --xml --json
```

## Hero Warnings
- Please be mindful of the hero warnings, especially on a build with a new hero or re-worked hero
- All the warnings do not need to be fixed, they are shown for awareness
- Tooltip strings that fail to parsed will show up **empty** in the xml or json files and thus will be a valid warning
- Fix the warning yourself (and possibly create an issue or pull request) or wait for an update
- Hero warnings can be shown to the console using the option `--heroWarnings`
- Ignored warnings are in `VerifyIgnore.txt`

## Advanced Features
### Storage Path (-s|--storagePath)
There are two types of paths you can provide for the app. One is the directory path of the `Heroes of the Storm` directory, the other is an already extracted `mods` directory containing the following file structure:
```
mods/
|--core.stormmod/base.stormdata/GameData/
   |--(ALL FILES)
|--heroesdata.stormmod/
   |--base.stormdata/GameData/
      |--Heroes/
         |--(ALL FILES)
   |--enus.stormdata/LocalizedData/
      |--GameStrings.txt
|--heromods/
   |--(ALL FILES)
```
Or a simpler way, extract these directories and file (keep the directory paths)

`mods/core.stormmod/base.stormdata/GameData/`

`mods/heroesData.stormmod/base.stormdata/GameData/` 

`mods/heroesData.stormmod/enus.stormdata/LocalizedData/GameStrings.txt`

`mods/heromods/`

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
Setting `modFolders` as the storage path will have the app parsed the `mods_22388` directory.

### File Split (-f|--fileSplit)
If true, one xml and json file will be created for each hero.
If false (default), a single xml and json file will be created.

### Description (-d|--description)
Sets the description/tooltip output type (0 - 6)

`0 (Default)` - RawDescription

`1` - PlainText

`2` - PlainTextWithNewlines

`3` - PlainTextWithScaling

`4` - PlainTextWithScalingWithNewlines

`5` - ColoredText

`6` - ColoredTextWithScaling

### Advanced File Configuration

## License
[MIT license](/LICENSE)
