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
If installed as a global tool, the app can be run with the following commands:
```
heroes-data -h
// or as...
dotnet heroes-data -h
```
If you download one of the zip files, run the following command from the extracted directory:
```
dotnet heroesdata.dll -h
```
Output of the -h option
```
(example)
```

Example command to create xml and json files from the `Heroes of the Storm` directory
```
(example)
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
Or a simpler way, extract these directorys and file (keep the directory paths)
- (LIST)

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
