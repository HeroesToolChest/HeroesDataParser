using Spectre.Console;

namespace HeroesDataParser.Infrastructure.JsonFileWriters.Tests;

[TestClass]
public class JsonGameStringFileWriterServiceTests
{
    private readonly ILogger<JsonGameStringFileWriterService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IAnsiConsole _console;
    private readonly IGameStringSerializerService _gameStringSerializerService;
    private readonly ISerializedDataStoreService _serializedDataStoreService;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;
    private readonly IResultSummaryService _resultSummaryService;

    public JsonGameStringFileWriterServiceTests()
    {
        _logger = Substitute.For<ILogger<JsonGameStringFileWriterService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _console = Substitute.For<IAnsiConsole>();
        _gameStringSerializerService = Substitute.For<IGameStringSerializerService>();
        _serializedDataStoreService = Substitute.For<ISerializedDataStoreService>();
        _jsonSerializerOptionService = Substitute.For<IJsonSerializerOptionService>();
        _resultSummaryService = Substitute.For<IResultSummaryService>();
    }

    [TestMethod]
    public async Task WriteMapSpecific_JsonOutputTypeIsNormal_CreatesJsonFile()
    {
        // arrange
        string mapName = "this! is a_map";

        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(WriteMapSpecific_JsonOutputTypeIsNormal_CreatesJsonFile)),
            CurrentLocale = StormLocale.ENUS,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = false,
            },
            AppVersion = "5.0.0",
            MapSpecificWriterJsonOutputType = MapSpecificWriterJsonOutputType.Normal,
        };

        _options.Value.Returns(rootOptions);

        List<string> dataTypes =
        [
            "AbilTalent",
            "Hero",
        ];

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "gamestrings", "maps", "this_is_amap", $"gamestrings_{rootOptions.BuildNumber}_enus.json");

        JsonSerializerOptionService jsonSerializerOptionService = new(_options, _gameStringSerializerService);

        byte[] expectedBytes = "test-serialized-data"u8.ToArray();
        _gameStringSerializerService.SerializeGameStrings(default!)
            .ReturnsForAnyArgs(expectedBytes);

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, _console, _gameStringSerializerService, _serializedDataStoreService, _jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.WriteMapSpecific(mapName);

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().GetJsonDataPatch(default!, default!);
        _gameStringSerializerService.Received(1).ClearStoredGameStrings();
        File.Exists(expectedFilePath).Should().BeTrue();
    }

    [TestMethod]
    public async Task WriteMapSpecific_JsonOutputTypeIsPatch_CreatesJsonFile()
    {
        // arrange
        string mapName = "this! is a_map";

        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(WriteMapSpecific_JsonOutputTypeIsPatch_CreatesJsonFile)),
            CurrentLocale = StormLocale.ENUS,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = false,
            },
            AppVersion = "5.0.0",
            MapSpecificWriterJsonOutputType = MapSpecificWriterJsonOutputType.Patch,
            AllowEmptyMapSpecificPatchFiles = true,
        };

        _options.Value.Returns(rootOptions);

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "gamestrings", "maps", "this_is_amap", $"gamestrings_{rootOptions.BuildNumber}_enus.patch.json");

        JsonSerializerOptionService jsonSerializerOptionService = new(_options, _gameStringSerializerService);

        byte[] expectedBytes = "test-serialized-data"u8.ToArray();
        _gameStringSerializerService.SerializeGameStrings(default!)
            .ReturnsForAnyArgs(expectedBytes);

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, _console, _gameStringSerializerService, _serializedDataStoreService, _jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.WriteMapSpecific(mapName);

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);
        _serializedDataStoreService.ReceivedWithAnyArgs(1).GetJsonDataPatch(default!, default!);
        _gameStringSerializerService.Received(1).ClearStoredGameStrings();
        File.Exists(expectedFilePath).Should().BeTrue();
    }

    [TestMethod]
    public async Task WriteMapSpecific_JsonOutputTypeIsNone_NoJsonFileCreated()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(WriteMapSpecific_JsonOutputTypeIsNone_NoJsonFileCreated)),
            CurrentLocale = StormLocale.DEDE,
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = -1,
                IsPtr = false,
            },
            AppVersion = "4.10.5",
            MapSpecificWriterJsonOutputType = MapSpecificWriterJsonOutputType.None,
        };

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _gameStringSerializerService);

        _options.Value.Returns(rootOptions);

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, _console, _gameStringSerializerService, _serializedDataStoreService, _jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.WriteMapSpecific("mapName");

        // assert
        _ = _resultSummaryService.DidNotReceive().GameStringFilesWritten;
        _ = _resultSummaryService.DidNotReceive().GameStringFilesTotal;
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().GetJsonDataPatch(default!, default!);
        _gameStringSerializerService.DidNotReceive().ClearStoredGameStrings();
    }

    [TestMethod]
    public async Task Write_NoBuildNumber_CreatesJsonFile()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(Write_NoBuildNumber_CreatesJsonFile)),
            CurrentLocale = StormLocale.DEDE,
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = -1,
                IsPtr = false,
            },
            AppVersion = "4.10.5",
            MapSpecificWriterJsonOutputType = MapSpecificWriterJsonOutputType.Normal,
        };

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "gamestrings", "maps", "mapname", "gamestrings_0_dede.json");

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _gameStringSerializerService);

        _options.Value.Returns(rootOptions);

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, _console, _gameStringSerializerService, _serializedDataStoreService, _jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.WriteMapSpecific("mapName");

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        File.Exists(expectedFilePath).Should().BeTrue();
    }

    [TestMethod]
    public async Task WriteBase_HasBuildNumber_CreatesJsonFile()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(WriteBase_HasBuildNumber_CreatesJsonFile)),
            CurrentLocale = StormLocale.ENUS,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = false,
            },
            AppVersion = "5.0.0",
        };

        _options.Value.Returns(rootOptions);

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "gamestrings", $"gamestrings_{rootOptions.BuildNumber}_enus.json");

        byte[] expectedBytes = "test-serialized-data"u8.ToArray();
        _gameStringSerializerService.SerializeGameStrings(default!)
            .ReturnsForAnyArgs(expectedBytes);

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, _console, _gameStringSerializerService, _serializedDataStoreService, _jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.WriteBase();

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        _serializedDataStoreService.ReceivedWithAnyArgs(2).AddSerializedData(default!, default!);
        _gameStringSerializerService.Received(1).ClearStoredGameStrings();
        File.Exists(expectedFilePath).Should().BeTrue();
    }

    [TestMethod]
    public async Task WriteBase_NoBuildNumber_CreatesJsonFile()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(WriteBase_NoBuildNumber_CreatesJsonFile)),
            CurrentLocale = StormLocale.DEDE,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = -1,
                IsPtr = false,
            },
            AppVersion = "4.10.5",
        };

        _options.Value.Returns(rootOptions);

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "gamestrings", "gamestrings_0_dede.json");

        byte[] expectedBytes = "test-base-data"u8.ToArray();
        _gameStringSerializerService.SerializeGameStrings(default!)
            .ReturnsForAnyArgs(expectedBytes);

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, _console, _gameStringSerializerService, _serializedDataStoreService, _jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.WriteBase();

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        _serializedDataStoreService.ReceivedWithAnyArgs(2).AddSerializedData(default!, default!);
        _gameStringSerializerService.Received(1).ClearStoredGameStrings();
        File.Exists(expectedFilePath).Should().BeTrue();
    }

    [TestMethod]
    public async Task WriteMap_HasBuildNumber_CreatesJsonFile()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(WriteMap_HasBuildNumber_CreatesJsonFile)),
            CurrentLocale = StormLocale.ENUS,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = false,
            },
            AppVersion = "5.0.0",
        };

        _options.Value.Returns(rootOptions);

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "gamestrings", $"gamestrings_mapdata_{rootOptions.BuildNumber}_enus.json");

        byte[] expectedBytes = "test-serialized-data"u8.ToArray();
        _gameStringSerializerService.SerializeGameStrings(default!)
            .ReturnsForAnyArgs(expectedBytes);

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, _console, _gameStringSerializerService, _serializedDataStoreService, _jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.WriteMap();

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        _serializedDataStoreService.ReceivedWithAnyArgs(1).AddSerializedData(default!, default!);
        _gameStringSerializerService.DidNotReceive().ClearStoredGameStrings();
        File.Exists(expectedFilePath).Should().BeTrue();
    }

    [TestMethod]
    public async Task WriteMap_NoBuildNumber_CreatesJsonFile()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(WriteMap_NoBuildNumber_CreatesJsonFile)),
            CurrentLocale = StormLocale.DEDE,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = -1,
                IsPtr = false,
            },
            AppVersion = "4.10.5",
        };

        _options.Value.Returns(rootOptions);

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "gamestrings", "gamestrings_mapdata_0_dede.json");

        byte[] expectedBytes = "test-map-data"u8.ToArray();
        _gameStringSerializerService.SerializeGameStrings(default!)
            .ReturnsForAnyArgs(expectedBytes);

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, _console, _gameStringSerializerService, _serializedDataStoreService, _jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.WriteMap();

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        _serializedDataStoreService.ReceivedWithAnyArgs(1).AddSerializedData(default!, default!);
        _gameStringSerializerService.DidNotReceive().ClearStoredGameStrings();
        File.Exists(expectedFilePath).Should().BeTrue();
    }
}