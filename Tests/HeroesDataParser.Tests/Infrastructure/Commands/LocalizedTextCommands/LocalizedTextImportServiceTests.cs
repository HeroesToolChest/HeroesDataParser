namespace HeroesDataParser.Infrastructure.Commands.LocalizedTextCommands.Tests;

[TestClass]
public class LocalizedTextImportServiceTests
{
    private readonly ILogger<LocalizedTextImportService> _logger;
    private readonly IOptions<LocalizedTextImportOptions> _options;
    private readonly TestConsole _console;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    public LocalizedTextImportServiceTests()
    {
        _logger = Substitute.For<ILogger<LocalizedTextImportService>>();
        _options = Substitute.For<IOptions<LocalizedTextImportOptions>>();
        _console = new TestConsole();
        _jsonSerializerOptionService = Substitute.For<IJsonSerializerOptionService>();
    }

    [TestMethod]
    public async Task ImportGameStrings_MismatchedHeroesVersion_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "bannerdata_95774_empty.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "gamestrings_96477_enus.json"),
            OutputDirectory = TestConstants.TestDirectory,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("The Heroes of the Storm version of the data file does not match the");
    }

    [TestMethod]
    public async Task ImportGameStrings_MismatchedHDPVersion_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "bannerdata_95774_emtpy_diff_hdp_version.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "gamestrings_96477_enus.json"),
            OutputDirectory = TestConstants.TestDirectory,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("The HDP version of the data file does not match the version");
    }

    [TestMethod]
    public async Task ImportGameStrings_DataTypeNotInGameStringsFile_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "gamestrings_96477_no_data_types_.json"),
            OutputDirectory = TestConstants.TestDirectory,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("the data types in the gamestrings file does not contain the data type");
    }

    [TestMethod]
    public async Task ImportGameStrings_MismatchedMapName_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "matchawarddata_96881_enus_boe.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "gamestrings_96881_enus_alterac_pass.json"),
            OutputDirectory = TestConstants.TestDirectory,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("The map name of the data file does not match the map name");
    }

    [TestMethod]
    public async Task ImportGameStrings_HeroesData_ReturnsNewFile()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "herodata_96881.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "gamestrings_96881_enus.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ImportGameStrings_HeroesData_ReturnsNewFile)),
            IsNewFile = true,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created at");

        File.Exists(_options.Value.OutputFilePath).Should().BeTrue();

        string newFileContent = File.ReadAllText(_options.Value.OutputFilePath);
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "LocalizedTextImport", "herodata_96881_full.json"));

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task ImportGameStrings_EmoticonData_ReturnsNewFile()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "emoticondata_96881.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "gamestrings_96881_enus.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ImportGameStrings_EmoticonData_ReturnsNewFile)),
            IsNewFile = true,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created at");

        File.Exists(_options.Value.OutputFilePath).Should().BeTrue();

        string newFileContent = File.ReadAllText(_options.Value.OutputFilePath);
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "LocalizedTextImport", "emoticondata_96881_full.json"));

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task ImportGameStrings_MatchAwardData_ReturnsNewFile()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "matchawarddata_96881.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "gamestrings_96881_enus.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ImportGameStrings_MatchAwardData_ReturnsNewFile)),
            IsNewFile = true,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created at");

        File.Exists(_options.Value.OutputFilePath).Should().BeTrue();

        string newFileContent = File.ReadAllText(_options.Value.OutputFilePath);
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "LocalizedTextImport", "matchawarddata_96881_full.json"));

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task ImportGameStrings_MatchAwardDataMapBOE_ReturnsNewFile()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "matchawarddata_96881_boe.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "gamestrings_96881_enus_boe.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ImportGameStrings_MatchAwardDataMapBOE_ReturnsNewFile)),
            IsNewFile = true,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created at");

        File.Exists(_options.Value.OutputFilePath).Should().BeTrue();

        string newFileContent = File.ReadAllText(_options.Value.OutputFilePath);
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "LocalizedTextImport", "matchawarddata_96881_enus_boe_full.json"));

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task ImportGameStrings_MapData_ReturnsNewFile()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "mapdata_96881.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "gamestrings_mapdata_96881_enus.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ImportGameStrings_MatchAwardData_ReturnsNewFile)),
            IsNewFile = true,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created at");

        File.Exists(_options.Value.OutputFilePath).Should().BeTrue();

        string newFileContent = File.ReadAllText(_options.Value.OutputFilePath);
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "LocalizedTextImport", "mapdata_96881_full.json"));

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task ImportGameStrings_MapDataWithNewDEDE_ReturnsNewFile()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "mapdata_96881_full.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "gamestrings_mapdata_96881_dede.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ImportGameStrings_MapDataWithNewDEDE_ReturnsNewFile)),
            IsNewFile = true,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created at");

        File.Exists(_options.Value.OutputFilePath).Should().BeTrue();

        string newFileContent = File.ReadAllText(_options.Value.OutputFilePath);
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "LocalizedTextImport", "mapdata_96881_dede_full.json"));

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    // original is colored with scaling, imported gamestring file is raw text
    [TestMethod]
    public async Task ImportGameStrings_HeroDataColoredTextImportRawText_ReturnsNewFile()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "herodata_96881_enus_colorwithscaling.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "gamestrings_96881_enus.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ImportGameStrings_HeroDataColoredTextImportRawText_ReturnsNewFile)),
            IsNewFile = true,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created at");

        File.Exists(_options.Value.OutputFilePath).Should().BeTrue();

        string newFileContent = File.ReadAllText(_options.Value.OutputFilePath);
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "LocalizedTextImport", "herodata_96881_full.json"));

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    // original is raw text, imported gamestring file is colored with scaling
    [TestMethod]
    public async Task ImportGameStrings_HeroDataRawTextImportColoredTextScaling_ReturnsNewFile()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "herodata_96881_full.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "gamestrings_96881_enus_coloredwithscaling.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ImportGameStrings_HeroDataRawTextImportColoredTextScaling_ReturnsNewFile)),
            IsNewFile = true,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created at");

        File.Exists(_options.Value.OutputFilePath).Should().BeTrue();

        string newFileContent = File.ReadAllText(_options.Value.OutputFilePath);
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "LocalizedTextImport", "herodata_96881_enus_colorwithscaling.json"));

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task ImportGameStrings_IsNotIndented_ReturnsNewFileAsNotIndented()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "mapdata_96881.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "gamestrings_mapdata_96881_enus.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ImportGameStrings_IsNotIndented_ReturnsNewFileAsNotIndented)),
            IsNewFile = true,
            JsonIndent = false,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created at");

        File.Exists(_options.Value.OutputFilePath).Should().BeTrue();

        string newFileContent = File.ReadAllText(_options.Value.OutputFilePath);
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "LocalizedTextImport", "mapdata_96881_enus_full_noindent.json"));

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    // just tests the new file, no actually file is updated
    [TestMethod]
    public async Task ImportGameStrings_NotNewFile_FileIsUpdated()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextImportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "mapdata_96881.json"),
            GameStringsFilePath = Path.Combine("TestJsonFiles", "LocalizedTextImport", "gamestrings_mapdata_96881_enus.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ImportGameStrings_NotNewFile_FileIsUpdated)),
            IsNewFile = false,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextImportService localizedTextImportService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await localizedTextImportService.ImportGameStrings();

        // assert
        _console.Output.Should().Contain("Updated data file at");

        File.Exists(_options.Value.OutputFilePath).Should().BeTrue();

        string newFileContent = File.ReadAllText(_options.Value.OutputFilePath);
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "LocalizedTextImport", "mapdata_96881_full.json"));

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }
}
