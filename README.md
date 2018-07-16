# Heroes Data Parser
[![Build status](https://ci.appveyor.com/api/projects/status/g3linacec0a4kqkn/branch/master?svg=true)](https://ci.appveyor.com/project/koliva8245/heroesdataparser/branch/master)  [![Build Status](https://travis-ci.org/koliva8245/HeroesDataParser.svg?branch=master)](https://travis-ci.org/koliva8245/HeroesDataParser)

Heroes Data Parser is a cross platform (Windows/MacOS/Linux) command line tool that extracts Heroes of the Storm game data into XML or JSON files. Extracts hero information along with all abilities, talents, and their respective portraits and icons.

## Installation
### Supported Operating Systems
- Windows 7 SP1 (x86 and x64) or higher 
- Linux (x64)
- macOS 10.12 and later versions

### For Linux and macOS users
To use the -e|--extract option, `libgdiplus` is required 

Ubuntu (or Linux distro equivalent)
```
sudo apt-get install -y libgdiplus
```

macOS
```
brew install mono-libgdiplus
```

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

Download and extract the latest `HeroesDataParser.*-scd-*.zip` file from the [releases](https://github.com/koliva8245/HeroesDataParser/releases) page for a selected operating system.

This zip file contains everything that is needed to run the dotnet core app without .NET Core being installed, so the zip file is quite large.

## Usage
If installed as a global tool, the app can be run with one of the following commands:
```
heroes-data -h
dotnet heroes-data -h
dotnet-heroes-data -h
```
If one of the zip files was downloaded, run the following command from the extracted directory:
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
  -b|--build                   Sets the override build file. Available only in -s|--storagePath mode in CASC mode
  -o|--outputDirectory         Sets the output directory
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
Please be mindful of the hero warnings, especially on a build with a new hero or re-worked hero.  
All the warnings do not need to be fixed, they are shown for awareness.  
**Tooltip strings that fail to parse will show up __empty__** in the xml or json files and thus will be a valid warning.  
Hero warnings can be shown to the console using the option `--heroWarnings`.  
Ignored warnings are in `VerifyIgnore.txt`.  

## Options
### Storage Path (-s|--storagePath)
There are two types of paths that can be provided for this option. One is the directory path of the `Heroes of the Storm` directory and the other is an already extracted `mods` directory containing the following file structure:
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

The `mods` directory can also have a build suffix in its name. [More info](./#Mods-suffix-directory).

### File Split (-f|--fileSplit)
If true, one xml and json file will be created for each hero.  
If false (default), a single xml and json file will be created.

### Description (-d|--description)
Sets the description/tooltip output type (0 - 6)

Some of these may require parsing for a readable output. Visit the [wiki page](https://github.com/koliva8245/HeroesDataParser/wiki/Parsing-Descriptions) for parsing tips.

`0` - RawDescription (Default)  
The raw output of the description. Contains the color tags `<c val=\"#TooltipNumbers\"></c>`, scaling data `~~0.04~~`, and newlines `<n/>`.

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
Same as PlainText but contains the scaling info `(+X% per level)`.

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

### Extract (-e|--extract)
Extracts images that have been referenced for a hero or ability/talent in the xml and json file(s).

Parameters  
_all_: all hero portraits and ability/talent icons are extracted  
_portraits_: only hero portraits are extracted  
_talents_: only ability/talent icons are extracted  

Notes:
- This option only works if a `Heroes of the Storm` directory path is provided for the `-s|--storagePath` option
- Images are always extracted in `.png` format

## Advanced Features
### Mods suffix directory
The `mods` directory may have a `_<build number>` suffix in its name. The build number determines the hero overrides file to load. If the overrides file does not exist and the build number is greater than the highest overrides file then it will load the default overrides file `HeroesOverrides.xml` otherwise it will load next **lowest** overrides file.

Example:
```
directory to load: mods_13500

HeroOverrides files:
HeroOverrides.xml
HeroOverrdies_12000.xml
HeroOverrdies_13000.xml <--- will be loaded
HeroOverrdies_14000.xml

directory to load: mods_14100

HeroOverrides files:
HeroOverrides.xml <--- will be loaded
HeroOverrdies_12000.xml
HeroOverrdies_13000.xml 
HeroOverrdies_14000.xml
```

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

### CASC HeroOverrides loading
When using a `Heroes of the Storm` directory, it will load the equivalent hero overrides file, just like in the [mods suffix directory](./#Mods-suffix-directory).

### Advanced File Configuration
For more advanced file configurations, edit the file `WriterConfig.xml`.  Options in the console override the options in the config file.

## License
[MIT license](/LICENSE)
