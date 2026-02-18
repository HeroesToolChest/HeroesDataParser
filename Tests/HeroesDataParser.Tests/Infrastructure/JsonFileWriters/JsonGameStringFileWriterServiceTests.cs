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
    public async Task WriteForMap_JsonOutputTypeIsNormal_CreatesJsonFile()
    {
        // arrange
        string mapName = "this! is a_map";

        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(WriteForMap_JsonOutputTypeIsNormal_CreatesJsonFile)),
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
        _gameStringSerializerService.SerializeGameStrings(default!, default!)
            .ReturnsForAnyArgs(expectedBytes);

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, _console, _gameStringSerializerService, _serializedDataStoreService, _jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.WriteForMap(mapName);

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().GetJsonDataPatch(default!, default!);
        _gameStringSerializerService.Received(1).ClearStoredGameStrings();
        File.Exists(expectedFilePath).Should().BeTrue();
    }

    [TestMethod]
    public async Task WriteForMap_JsonOutputTypeIsDiff_CreatesJsonFile()
    {
        // arrange
        string mapName = "this! is a_map";

        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(WriteForMap_JsonOutputTypeIsDiff_CreatesJsonFile)),
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

        List<string> dataTypes =
        [
            "AbilTalent",
            "Hero",
        ];

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "gamestrings", "maps", "this_is_amap", $"gamestrings_{rootOptions.BuildNumber}_enus.diff.json");

        JsonSerializerOptionService jsonSerializerOptionService = new(_options, _gameStringSerializerService);

        byte[] expectedBytes = "test-serialized-data"u8.ToArray();
        _gameStringSerializerService.SerializeGameStrings(default!, default!)
            .ReturnsForAnyArgs(expectedBytes);

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, _console, _gameStringSerializerService, _serializedDataStoreService, _jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.WriteForMap(mapName);

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);
        _serializedDataStoreService.ReceivedWithAnyArgs(1).GetJsonDataPatch(default!, default!);
        _gameStringSerializerService.Received(1).ClearStoredGameStrings();
        File.Exists(expectedFilePath).Should().BeTrue();
    }

    [TestMethod]
    public async Task WriteForMap_JsonOutputTypeIsNone_NoJsonFileCreated()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(WriteForMap_JsonOutputTypeIsNone_NoJsonFileCreated)),
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
        await jsonGameStringFileWriterService.WriteForMap("mapName");

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
        await jsonGameStringFileWriterService.WriteForMap("mapName");

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        File.Exists(expectedFilePath).Should().BeTrue();
    }

    [TestMethod]
    public async Task Write_WithValidBytes_CreatesJsonFile()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(Write_WithValidBytes_CreatesJsonFile)),
            CurrentLocale = StormLocale.ENUS,
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 55,
                Revision = 1234,
                Build = 98765,
                IsPtr = false,
            },
        };

        _options.Value.Returns(rootOptions);

        byte[] testBytes = "{\"test\":\"data\"}"u8.ToArray();
        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "gamestrings", $"gamestrings_{rootOptions.BuildNumber}_enus.json");

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, _console, _gameStringSerializerService, _serializedDataStoreService, _jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.Write(testBytes);

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        File.Exists(expectedFilePath).Should().BeTrue();
        File.ReadAllBytes(expectedFilePath).Should().BeEquivalentTo(testBytes);
    }

    [TestMethod]
    public void SerializeOnly_HasGameStringElements_StoresSerializedData()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(SerializeOnly_HasGameStringElements_StoresSerializedData)),
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
                Major = 3,
                Minor = 45,
                Revision = 6789,
                Build = 12345,
                IsPtr = false,
            },
            AppVersion = "6.1.0",
        };

        _options.Value.Returns(rootOptions);

        List<string> dataTypes =
        [
            "Hero",
            "Unit",
        ];

        _serializedDataStoreService.GetDataTypesFromData().Returns(dataTypes);

        byte[] expectedBytes = "test-serialized-data"u8.ToArray();
        _gameStringSerializerService.SerializeGameStrings(default!, default!)
            .ReturnsForAnyArgs(expectedBytes);

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, _console, _gameStringSerializerService, _serializedDataStoreService, _jsonSerializerOptionService, _resultSummaryService);

        // act
        jsonGameStringFileWriterService.SerializeOnly();

        // assert
        _serializedDataStoreService.Received(1).AddSerializedData(Constants.GameStringFilePrefix, expectedBytes);
        _gameStringSerializerService.Received(1).ClearStoredGameStrings();
    }
}