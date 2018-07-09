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

For more advanced features, visit the [wiki](https://github.com/koliva8245/HeroesDataParser/wiki)

## License
[MIT license](/LICENSE)
