# Heroes Data Parser
[![Build status](https://ci.appveyor.com/api/projects/status/g3linacec0a4kqkn/branch/master?svg=true)](https://ci.appveyor.com/project/koliva8245/heroesdataparser/branch/master)  [![Build Status](https://travis-ci.org/koliva8245/HeroesDataParser.svg?branch=master)](https://travis-ci.org/koliva8245/HeroesDataParser)

Heroes Data Parser is a cross platform (Windows/MacOS/Linux) command line tool that extracts Heroes of the Storm game data into XML or JSON files. Extracts hero information along with all abilities, talents, and their respective portraits and icons.

## Installation
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

## License
[MIT license](/LICENSE)
