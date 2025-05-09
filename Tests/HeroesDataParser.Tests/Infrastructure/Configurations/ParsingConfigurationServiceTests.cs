using HeroesDataParser.Options;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Text;

namespace HeroesDataParser.Infrastructure.Configurations.Tests;

[TestClass]
public class ParsingConfigurationServiceTests
{
    private readonly ILogger<ParsingConfigurationService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IFileProvider _fileProvider;

    public ParsingConfigurationServiceTests()
    {
        _logger = Substitute.For<ILogger<ParsingConfigurationService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _fileProvider = Substitute.For<IFileProvider>();
    }

    [TestMethod]
    public void FilterAllowedItems_DisallowExactItems_ReturnsAllowedItems()
    {
        // arrange
        List<string> items = ["item1", "item2", "item3", "item4", "item5"];

        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 1,
        });

        var fileInfo = CreateFileInfo("parsing.json");
        fileInfo.CreateReadStream().Returns(new MemoryStream(Encoding.UTF8.GetBytes(
            """
            {
              "XmlDataParsers": {
                "AnnouncerPack": {
                  "Disallow": {
                    "Exact": [
                      "item2",
                      "item5"
                    ],
                    "Regex": [
                    ]
                  }
                }
              }
            }
            """)));
        _fileProvider.GetFileInfo(Arg.Any<string>()).Returns(x => fileInfo);

        ParsingConfigurationService parsingConfigurationService = new(_logger, _options, _fileProvider);
        parsingConfigurationService.Load();

        // act
        List<string> result = parsingConfigurationService.FilterAllowedItems("AnnouncerPack", items).ToList();

        // assert
        result.Should().BeEquivalentTo(new List<string> { "item1", "item3", "item4" });
    }

    [TestMethod]
    public void FilterAllowedItems_DisallowRegexItems_ReturnsAllowedItems()
    {
        // arrange
        List<string> items = ["item1", "item2", "item3", "item4", "item5", "base1", "base2"];

        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 1,
        });

        var fileInfo = CreateFileInfo("parsing.json");
        fileInfo.CreateReadStream().Returns(new MemoryStream(Encoding.UTF8.GetBytes(
            """
            {
              "XmlDataParsers": {
                "AnnouncerPack": {
                  "Disallow": {
                    "Exact": [
                    ],
                    "Regex": [
                      "^item"
                    ]
                  }
                }
              }
            }
            """)));
        _fileProvider.GetFileInfo(Arg.Any<string>()).Returns(x => fileInfo);

        ParsingConfigurationService parsingConfigurationService = new(_logger, _options, _fileProvider);
        parsingConfigurationService.Load();

        // act
        List<string> result = parsingConfigurationService.FilterAllowedItems("AnnouncerPack", items).ToList();

        // assert
        result.Should().BeEquivalentTo(new List<string> { "base1", "base2" });
    }

    [TestMethod]
    public void FilterAllowedItems_NoDataObjectTypInJson_ReturnsAllItems()
    {
        // arrange
        List<string> items = ["item1", "item2", "item3"];

        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 1,
        });

        var fileInfo = CreateFileInfo("parsing.json");
        fileInfo.CreateReadStream().Returns(new MemoryStream(Encoding.UTF8.GetBytes(
            """
            {
              "XmlDataParsers": {
              }
            }
            """)));
        _fileProvider.GetFileInfo(Arg.Any<string>()).Returns(x => fileInfo);

        ParsingConfigurationService parsingConfigurationService = new(_logger, _options, _fileProvider);
        parsingConfigurationService.Load();

        // act
        List<string> result = parsingConfigurationService.FilterAllowedItems("AnnouncerPack", items).ToList();

        // assert
        result.Should().BeEquivalentTo(new List<string> { "item1", "item2", "item3" });
    }

    [TestMethod]
    public void FilterAllowedItems_BuildNumberLowerThanFiles_MatchesLowestBuild()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 500,
        });

        IDirectoryContents directoryContents = Substitute.For<IDirectoryContents>();
        directoryContents.Exists.Returns(true);

        directoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("parsing_11111.json"),
            CreateFileInfo("parsing_22222.json"),
            CreateFileInfo("parsing_33333.json"),
            CreateFileInfo("parsing.json"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Arg.Any<string>()).Returns(directoryContents);

        var fileInfo = CreateFileInfo("parsing_11111.json");
        fileInfo.CreateReadStream().Returns(new MemoryStream(Encoding.UTF8.GetBytes("""{}""")));
        _fileProvider.GetFileInfo(Arg.Any<string>()).Returns(x => fileInfo);

        ParsingConfigurationService parsingConfigurationService = new(_logger, _options, _fileProvider);
        parsingConfigurationService.Load();

        // act
        string? result = parsingConfigurationService.SelectedFilePath;

        // assert
        result.Should().Be(Path.Join(parsingConfigurationService.ParsingConfigurationDirectory, "parsing_11111.json"));
    }

    [TestMethod]
    public void FilterAllowedItems_BuildNumberHigherThanFiles_MatchesDefaultFile()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 99999,
        });

        IDirectoryContents directoryContents = Substitute.For<IDirectoryContents>();
        directoryContents.Exists.Returns(true);

        directoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("parsing_11111.json"),
            CreateFileInfo("parsing_22222.json"),
            CreateFileInfo("parsing_33333.json"),
            CreateFileInfo("parsing.json"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Arg.Any<string>()).Returns(directoryContents);

        var fileInfo = CreateFileInfo("parsing.json");
        fileInfo.CreateReadStream().Returns(new MemoryStream(Encoding.UTF8.GetBytes("""{}""")));
        _fileProvider.GetFileInfo(Arg.Any<string>()).Returns(x => fileInfo);

        ParsingConfigurationService parsingConfigurationService = new(_logger, _options, _fileProvider);
        parsingConfigurationService.Load();

        // act
        string? result = parsingConfigurationService.SelectedFilePath;

        // assert
        result.Should().Be(Path.Join(parsingConfigurationService.ParsingConfigurationDirectory, "parsing.json"));
    }

    [TestMethod]
    public void FilterAllowedItems_BuildNumberBetweenFiles_MatchesNextLowestBuild()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 33332,
        });

        IDirectoryContents directoryContents = Substitute.For<IDirectoryContents>();
        directoryContents.Exists.Returns(true);

        directoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("parsing_11111.json"),
            CreateFileInfo("parsing_22222.json"),
            CreateFileInfo("parsing_33333.json"),
            CreateFileInfo("parsing.json"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Arg.Any<string>()).Returns(directoryContents);

        var fileInfo = CreateFileInfo("parsing_22222.json");
        fileInfo.CreateReadStream().Returns(new MemoryStream(Encoding.UTF8.GetBytes("""{}""")));
        _fileProvider.GetFileInfo(Arg.Any<string>()).Returns(x => fileInfo);

        ParsingConfigurationService parsingConfigurationService = new(_logger, _options, _fileProvider);
        parsingConfigurationService.Load();

        // act
        string? result = parsingConfigurationService.SelectedFilePath;

        // assert
        result.Should().Be(Path.Join(parsingConfigurationService.ParsingConfigurationDirectory, "parsing_22222.json"));
    }

    [TestMethod]
    public void FilterAllowedItems_BuildNumberEqualsAFile_MatchesBuild()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 22222,
        });

        IDirectoryContents directoryContents = Substitute.For<IDirectoryContents>();
        directoryContents.Exists.Returns(true);

        directoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("parsing_11111.json"),
            CreateFileInfo("parsing_22222.json"),
            CreateFileInfo("parsing_33333.json"),
            CreateFileInfo("parsing.json"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Arg.Any<string>()).Returns(directoryContents);

        var fileInfo = CreateFileInfo("parsing_22222.json");
        fileInfo.CreateReadStream().Returns(new MemoryStream(Encoding.UTF8.GetBytes("""{}""")));
        _fileProvider.GetFileInfo(Arg.Any<string>()).Returns(x => fileInfo);

        ParsingConfigurationService parsingConfigurationService = new(_logger, _options, _fileProvider);
        parsingConfigurationService.Load();

        // act
        string? result = parsingConfigurationService.SelectedFilePath;

        // assert
        result.Should().Be(Path.Join(parsingConfigurationService.ParsingConfigurationDirectory, "parsing_22222.json"));
    }

    [TestMethod]
    public void FilterAllowedItems_NoFilesLoaded_SelectedFilePathShouldBeNull()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 22222,
        });

        IDirectoryContents directoryContents = Substitute.For<IDirectoryContents>();
        directoryContents.Exists.Returns(true);

        directoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Arg.Any<string>()).Returns(directoryContents);

        ParsingConfigurationService parsingConfigurationService = new(_logger, _options, _fileProvider);
        parsingConfigurationService.Load();

        // act
        string? result = parsingConfigurationService.SelectedFilePath;

        // assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void FilterAllowedItems_NoBuildSet_MatchesHighestBuild()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = null,
        });

        IDirectoryContents directoryContents = Substitute.For<IDirectoryContents>();
        directoryContents.Exists.Returns(true);

        directoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("parsing_11111.json"),
            CreateFileInfo("parsing_22222.json"),
            CreateFileInfo("parsing_33333.json"),
            CreateFileInfo("parsing.json"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Arg.Any<string>()).Returns(directoryContents);

        var fileInfo = CreateFileInfo("parsing.json");
        fileInfo.CreateReadStream().Returns(new MemoryStream(Encoding.UTF8.GetBytes("""{}""")));
        _fileProvider.GetFileInfo(Arg.Any<string>()).Returns(x => fileInfo);

        ParsingConfigurationService parsingConfigurationService = new(_logger, _options, _fileProvider);
        parsingConfigurationService.Load();

        // act
        string? result = parsingConfigurationService.SelectedFilePath;

        // assert
        result.Should().Be(Path.Join(parsingConfigurationService.ParsingConfigurationDirectory, "parsing.json"));
    }

    [TestMethod]
    public void FilterAllowedItems_NoBuildSetWithNoFiles_SelectedFilePathShouldBeNull()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = null,
        });

        IDirectoryContents directoryContents = Substitute.For<IDirectoryContents>();
        directoryContents.Exists.Returns(true);

        directoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Arg.Any<string>()).Returns(directoryContents);

        ParsingConfigurationService parsingConfigurationService = new(_logger, _options, _fileProvider);

        // act
        string? result = parsingConfigurationService.SelectedFilePath;

        // assert
        result.Should().BeNull();
    }

    private static IFileInfo CreateFileInfo(string name)
    {
        IFileInfo fileInfo = Substitute.For<IFileInfo>();
        fileInfo.Exists.Returns(true);
        fileInfo.Name.Returns(name);

        return fileInfo;
    }
}