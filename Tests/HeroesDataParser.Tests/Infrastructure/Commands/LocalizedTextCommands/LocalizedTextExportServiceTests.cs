namespace HeroesDataParser.Infrastructure.Commands.LocalizedTextCommands.Tests;

[TestClass]
public class LocalizedTextExportServiceTests
{
    private readonly ILogger<LocalizedTextExportService> _logger;
    private readonly IOptions<LocalizedTextExportOptions> _options;
    private readonly TestConsole _console;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;
    private readonly IGameStringSerializerService _gameStringSerializerService;

    public LocalizedTextExportServiceTests()
    {
        _logger = Substitute.For<ILogger<LocalizedTextExportService>>();
        _options = Substitute.For<IOptions<LocalizedTextExportOptions>>();
        _console = new TestConsole();
        _jsonSerializerOptionService = Substitute.For<IJsonSerializerOptionService>();
        _gameStringSerializerService = new GameStringSerializerService(Substitute.For<ILogger<GameStringSerializerService>>());
    }

    [TestMethod]
    public async Task ExportGameStrings_AlreadyExtracted_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus_empty_lt_extract.json"),
            OutputDirectory = TestConstants.TestDirectory,
            JsonIndent = true,
        });

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("The provided data file already has gamestrings extracted");
    }

    [TestMethod]
    public async Task ExportGameStrings_DataFileAlreadyExists_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            OutputDirectory = "TestJsonFiles",
            JsonIndent = true,
            AllowOverwrite = false,
        });

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("Output file already exists");
    }

    [TestMethod]
    public async Task ExportGameStrings_GameStringFileAlreadyExists_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_GameStringFileAlreadyExists_DoesNotComplete)),
            JsonIndent = true,
            AllowOverwrite = false,
        });

        Directory.CreateDirectory(_options.Value.OutputDirectory);
        File.Copy(Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96477_enus_ann.json"), Path.Combine(_options.Value.OutputDirectory, "gamestrings_96477_enus.json"));

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("Output file already exists");
    }

    [TestMethod]
    public async Task ExportGameStrings_ProvidedGameStringFileAlreadyExists_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            GameStringFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96477_enus_ann.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_ProvidedGameStringFileAlreadyExists_DoesNotComplete)),
            JsonIndent = true,
            AllowOverwrite = false,
        });

        Directory.CreateDirectory(_options.Value.OutputDirectory);
        File.Copy(Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96477_enus_ann.json"), Path.Combine(_options.Value.OutputDirectory, "gamestrings_96477_enus_ann.json"));

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("Output file already exists");
    }

    [TestMethod]
    public async Task ExportGameStrings_MismatchHdpVersion_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "bannerdata_95774_emtpy_diff_hdp_version.json"),
            GameStringFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96881_enus_ann_hero.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_MismatchHdpVersion_DoesNotComplete)),
            ExtractType = ExtractType.Extract,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("The HDP version of the data file does not match");
    }

    [TestMethod]
    public async Task ExportGameStrings_MismatchHeroesVersion_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            GameStringFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96881_enus_ann_hero.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_MismatchHeroesVersion_DoesNotComplete)),
            ExtractType = ExtractType.Extract,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("The Heroes of the Storm version of the data file does not match");
    }

    [TestMethod]
    public async Task ExportGameStrings_GameStringsFileAlreadyContainsDataType_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "announcerdata_96881_enus.json"),
            GameStringFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96881_enus_alterac_pass.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_GameStringsFileAlreadyContainsDataType_DoesNotComplete)),
            ExtractType = ExtractType.Extract,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("The gamestrings file already contains the data type");
    }

    [TestMethod]
    public async Task ExportGameStrings_NoMapNameInData_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "spraydata_96881_enus.json"),
            GameStringFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96881_enus_alterac_pass.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_NoMapNameInData_DoesNotComplete)),
            ExtractType = ExtractType.Extract,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("The map name of the data file does not match");
    }

    [TestMethod]
    public async Task ExportGameStrings_NoMapNameInGameStrings_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "matchawarddata_96881_enus_alterac_pass.json"),
            GameStringFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96881_enus_ann_hero.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_NoMapNameInGameStrings_DoesNotComplete)),
            ExtractType = ExtractType.Extract,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("The map name of the data file does not match");
    }

    [TestMethod]
    public async Task ExportGameStrings_MismatchedMapNames_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "spraydata_96881_enus_alterac_pass.json"),
            GameStringFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96881_enus_boe.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_MismatchedMapNames_DoesNotComplete)),
            ExtractType = ExtractType.Extract,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("The map name of the data file does not match");
    }

    [TestMethod]
    public async Task ExportGameStrings_DataHasNoGamestringTextProperties_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "spraydata_96881_enus_alterac_pass_no_gst.json"),
            GameStringFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96881_enus_boe.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_DataHasNoGamestringTextProperties_DoesNotComplete)),
            ExtractType = ExtractType.Extract,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("'gameStringText' properties is missing");
    }

    [TestMethod]
    public async Task ExportGameStrings_GameStringsTextPropertiesDoNotMatch_DoesNotComplete()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "spraydata_96881_enus_all_true.json"),
            GameStringFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96881_enus_ann_hero.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_GameStringsTextPropertiesDoNotMatch_DoesNotComplete)),
            ExtractType = ExtractType.Extract,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("The 'gameStringText' properties of the data file does not match");
    }

    [TestMethod]
    public async Task ExportGameStrings_ExtractTypeRemove_DataFileCreated()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_ExtractTypeRemove_DataFileCreated)),
            ExtractType = ExtractType.Remove,
            JsonIndent = true,
            AllowOverwrite = false,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created");

        FileCompare.ShouldBeEqual(_options.Value.OutputDataFilePath, Path.Combine("TestJsonFiles", "LocalizedTextExport", "announcerdata_96477_enus_extracted.json"));
    }

    [TestMethod]
    public async Task ExportGameStrings_ExtractTypeRemoveFileOverwritten_DataFileUpdated()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_ExtractTypeRemoveFileOverwritten_DataFileUpdated)),
            ExtractType = ExtractType.Remove,
            JsonIndent = true,
            AllowOverwrite = true,
        });

        Directory.CreateDirectory(_options.Value.OutputDirectory);
        File.Copy(_options.Value.DataFilePath, Path.Combine(_options.Value.OutputDirectory, "announcerdata_96477_enus.json"));

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("Updated data file");

        FileCompare.ShouldBeEqual(_options.Value.OutputDataFilePath, Path.Combine("TestJsonFiles", "LocalizedTextExport", "announcerdata_96477_enus_extracted.json"));
    }

    [TestMethod]
    public async Task ExportGameStrings_ExtractTypeExtractWithNewGameStringsFile_DataFileAndGameStringsFileCreated()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_ExtractTypeExtractWithNewGameStringsFile_DataFileAndGameStringsFileCreated)),
            ExtractType = ExtractType.Extract,
            JsonIndent = true,
            AllowOverwrite = false,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created");
        _console.Output.Should().Contain("New gamestring file created");

        FileCompare.ShouldBeEqual(_options.Value.OutputDataFilePath, Path.Combine("TestJsonFiles", "LocalizedTextExport", "announcerdata_96477_enus_extracted.json"));
        FileCompare.ShouldBeEqual(Path.Combine(_options.Value.OutputDirectory, "gamestrings_96477_enus.json"), Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96477_enus_ann.json"));
    }

    [TestMethod]
    public async Task ExportGameStrings_ExtractTypeExtractWithExistingGameStringsFile_DataFileCreatedAndGameStringsFileUpdated()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_ExtractTypeExtractWithExistingGameStringsFile_DataFileCreatedAndGameStringsFileUpdated)),
            ExtractType = ExtractType.Extract,
            JsonIndent = true,
            AllowOverwrite = true,
        });

        Directory.CreateDirectory(_options.Value.OutputDirectory);

        File.Copy(Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96881_enus_boe.json"), Path.Combine(_options.Value.OutputDirectory, "gamestrings_96477_enus.json"));

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created");
        _console.Output.Should().Contain("Updated gamestring file");

        FileCompare.ShouldBeEqual(_options.Value.OutputDataFilePath, Path.Combine("TestJsonFiles", "LocalizedTextExport", "announcerdata_96477_enus_extracted.json"));
        FileCompare.ShouldBeEqual(Path.Combine(_options.Value.OutputDirectory, "gamestrings_96477_enus.json"), Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96477_enus_ann.json"));
    }

    [TestMethod]
    public async Task ExportGameStrings_UpdateGameStringFileWithCopiedGameStrings_DataFileCreatedAndGameStringsFileUpdated()
    {
        // arrange
        _options.Value.Returns(new LocalizedTextExportOptions()
        {
            DataFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "herodata_96881_enus_plaintext_all_true.json"),
            GameStringFilePath = Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96881_enus_plaintext_all_true_ann.json"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStrings_UpdateGameStringFileWithCopiedGameStrings_DataFileCreatedAndGameStringsFileUpdated)),
            ExtractType = ExtractType.Copy,
            JsonIndent = true,
            AllowOverwrite = false,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        LocalizedTextExportService localizedTextExportService = new(_logger, _options, _console, _jsonSerializerOptionService, _gameStringSerializerService);

        // act
        await localizedTextExportService.ExportGameStrings();

        // assert
        _console.Output.Should().Contain("New data file created");
        _console.Output.Should().Contain("New gamestring file");

        FileCompare.ShouldBeEqual(_options.Value.OutputDataFilePath, Path.Combine("TestJsonFiles", "LocalizedTextExport", "herodata_96881_enus_plaintext_all_true.json"));
        FileCompare.ShouldBeEqual(Path.Combine(_options.Value.OutputDirectory, "gamestrings_96881_enus_plaintext_all_true_ann.json"), Path.Combine("TestJsonFiles", "LocalizedTextExport", "gamestrings_96881_enus_plaintext_all_true_ann_hero.json"));
    }
}
