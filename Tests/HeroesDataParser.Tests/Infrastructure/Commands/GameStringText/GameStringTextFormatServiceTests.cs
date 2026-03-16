namespace HeroesDataParser.Infrastructure.Commands.GameStringText.Tests;

[TestClass]
public class GameStringTextFormatServiceTests
{
    private readonly ILogger<GameStringTextFormatService> _logger;
    private readonly IOptions<GameStringTextFormatOptions> _options;
    private readonly TestConsole _console;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    public GameStringTextFormatServiceTests()
    {
        _logger = Substitute.For<ILogger<GameStringTextFormatService>>();
        _options = Substitute.For<IOptions<GameStringTextFormatOptions>>();
        _console = new TestConsole();
        _jsonSerializerOptionService = Substitute.For<IJsonSerializerOptionService>();
    }

    [TestMethod]
    public async Task FormatGameStringText_NoRootMeta_ReturnsError()
    {
        // arrange
        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.ColoredTextWithScaling,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.None,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.None,
            FilePath = Path.Combine("TestJsonFiles", "invalid_patch.json"),
            OutputDirectory = "TestOutput",
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("Missing 'meta' property in JSON");
    }

    [TestMethod]
    public async Task FormatGameStringText_InvalidItemsType_ReturnsError()
    {
        // arrange
        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.ColoredTextWithScaling,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.None,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.None,
            FilePath = Path.Combine("TestJsonFiles", "invalid_items_type.json"),
            OutputDirectory = "TestOutput",
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("Not a valid 'itemsType' value in JSON:");
    }

    [TestMethod]
    public async Task FormatGameStringText_DataLocalizedTextIsExtract_ReturnsError()
    {
        // arrange
        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.ColoredTextWithScaling,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.None,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.None,
            FilePath = Path.Combine("TestJsonFiles", "GameStringTextFormat", "herodata_96477_enus_data_extracted.json"),
            OutputDirectory = "TestOutput",
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("'localizedText' value in JSON is");
    }

    [TestMethod]
    public async Task FormatGameStringText_DataRawTo_CTS_CRU_SR_ReturnNewFile()
    {
        // arrange
        string inputFileName = "original_herodata_96477_enus_rawtext_rc_rs_pc_ps.json";
        string outputFileDirectory = Path.Combine("TestOutput", nameof(FormatGameStringText_DataRawTo_CTS_CRU_SR_ReturnNewFile));

        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.ColoredTextWithScaling,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.RemoveAndUndo,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.Remove,
            FilePath = Path.Combine("TestJsonFiles", "GameStringTextFormat", inputFileName),
            OutputDirectory = outputFileDirectory,
            IsNewFile = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("New formatted data file created at");

        string newOutputFile = Path.Combine(outputFileDirectory, inputFileName);
        File.Exists(newOutputFile).Should().BeTrue();

        string newFileContent = File.ReadAllText(newOutputFile).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "GameStringTextFormat", "herodata_96477_enus_rawtext_to_colortextscaling_rs.json")).ReplaceLineEndings("\n");

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task FormatGameStringText_DataRawTo_CT_ReturnNewFile()
    {
        // arrange
        string inputFileName = "original_herodata_96477_enus_rawtext_rc_rs_pc_ps.json";
        string outputFileDirectory = Path.Combine("TestOutput", nameof(FormatGameStringText_DataRawTo_CT_ReturnNewFile));

        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.ColoredText,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.None,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.None,
            FilePath = Path.Combine("TestJsonFiles", "GameStringTextFormat", inputFileName),
            OutputDirectory = outputFileDirectory,
            IsNewFile = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("New formatted data file created at");

        string newOutputFile = Path.Combine(outputFileDirectory, inputFileName);
        File.Exists(newOutputFile).Should().BeTrue();

        string newFileContent = File.ReadAllText(newOutputFile).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "GameStringTextFormat", "herodata_96477_enus_rawtext_to_coloredtext_rc_rs_pc_ps.json")).ReplaceLineEndings("\n");

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task FormatGameStringText_DataRawTo_PT_CRU_SRU_ReturnNewFile()
    {
        // arrange
        string inputFileName = "original_herodata_96477_enus_rawtext_rc_rs_pc_ps.json";
        string outputFileDirectory = Path.Combine("TestOutput", nameof(FormatGameStringText_DataRawTo_PT_CRU_SRU_ReturnNewFile));

        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.PlainText,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.RemoveAndUndo,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.RemoveAndUndo,
            FilePath = Path.Combine("TestJsonFiles", "GameStringTextFormat", inputFileName),
            OutputDirectory = outputFileDirectory,
            IsNewFile = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("New formatted data file created at");

        string newOutputFile = Path.Combine(outputFileDirectory, inputFileName);
        File.Exists(newOutputFile).Should().BeTrue();

        string newFileContent = File.ReadAllText(newOutputFile).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "GameStringTextFormat", "herodata_96477_enus_rawtext_to_plaintext.json")).ReplaceLineEndings("\n");

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task FormatGameStringText_DataRawTo_PT_CR_SR_ReturnNewFile()
    {
        // arrange
        string inputFileName = "original_herodata_96477_enus_rawtext_rc_rs_pc_ps.json";
        string outputFileDirectory = Path.Combine("TestOutput", nameof(FormatGameStringText_DataRawTo_PT_CR_SR_ReturnNewFile));

        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.PlainText,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.Remove,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.Remove,
            FilePath = Path.Combine("TestJsonFiles", "GameStringTextFormat", inputFileName),
            OutputDirectory = outputFileDirectory,
            IsNewFile = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("New formatted data file created at");

        string newOutputFile = Path.Combine(outputFileDirectory, inputFileName);
        File.Exists(newOutputFile).Should().BeTrue();

        string newFileContent = File.ReadAllText(newOutputFile).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "GameStringTextFormat", "herodata_96477_enus_rawtext_to_plaintext_rc_rs.json")).ReplaceLineEndings("\n");

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task FormatGameStringText_DataPlaintTextTo_CT_CR_SR_ReturnNewFile()
    {
        // arrange
        string inputFileName = "original_herodata_96477_enus_plaintext.json";
        string outputFileDirectory = Path.Combine("TestOutput", nameof(FormatGameStringText_DataPlaintTextTo_CT_CR_SR_ReturnNewFile));

        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.ColoredText,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.Remove,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.Remove,
            FilePath = Path.Combine("TestJsonFiles", "GameStringTextFormat", inputFileName),
            OutputDirectory = outputFileDirectory,
            IsNewFile = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("New formatted data file created at");

        string newOutputFile = Path.Combine(outputFileDirectory, inputFileName);
        File.Exists(newOutputFile).Should().BeTrue();

        string newFileContent = File.ReadAllText(newOutputFile).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "GameStringTextFormat", "herodata_96477_enus_plaintext_to_coloredtext_rc_rs.json")).ReplaceLineEndings("\n");

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    // original is raw but has no constant or style replace or preserve
    [TestMethod]
    public async Task FormatGameStringText_DataPlaintTextTo_SR_ReturnNewFile()
    {
        // arrange
        string inputFileName = "original_herodata_96477_enus_rawtext.json";
        string outputFileDirectory = Path.Combine("TestOutput", nameof(FormatGameStringText_DataPlaintTextTo_SR_ReturnNewFile));

        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.ColoredText,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.RemoveAndUndo,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.Remove,
            FilePath = Path.Combine("TestJsonFiles", "GameStringTextFormat", inputFileName),
            OutputDirectory = outputFileDirectory,
            IsNewFile = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("New formatted data file created at");

        string newOutputFile = Path.Combine(outputFileDirectory, inputFileName);
        File.Exists(newOutputFile).Should().BeTrue();

        string newFileContent = File.ReadAllText(newOutputFile).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "GameStringTextFormat", "herodata_96477_enus_rawtext_to_coloredtext_rs.json")).ReplaceLineEndings("\n");

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    // self update to self, nothing should change
    [TestMethod]
    public async Task FormatGameStringText_DataPlaintTextTo_PT_CR_SR_UpdatedFile()
    {
        // arrange
        string inputFileName = "original_herodata_96477_enus_plaintext_self.json";
        string outputFileDirectory = Path.Combine("TestOutput", nameof(FormatGameStringText_DataPlaintTextTo_PT_CR_SR_UpdatedFile));

        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.PlainText,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.RemoveAndUndo,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.RemoveAndUndo,
            FilePath = Path.Combine("TestJsonFiles", "GameStringTextFormat", inputFileName),
            OutputDirectory = outputFileDirectory,
            IsNewFile = false,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("Updated formatted data file at");

        string newOutputFile = Path.Combine(outputFileDirectory, inputFileName);
        File.Exists(newOutputFile).Should().BeTrue();

        string newFileContent = File.ReadAllText(newOutputFile).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "GameStringTextFormat", "original_herodata_96477_enus_plaintext_self.json")).ReplaceLineEndings("\n");

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task FormatGameStringText_GameStringPlaintTextTo_PT_SR_ReturnNewFile()
    {
        // arrange
        string inputFileName = "original_gamestrings_96477_enus_rawtext.json";
        string outputFileDirectory = Path.Combine("TestOutput", nameof(FormatGameStringText_GameStringPlaintTextTo_PT_SR_ReturnNewFile));
        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.PlainText,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.None,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.None,
            FilePath = Path.Combine("TestJsonFiles", "GameStringTextFormat", inputFileName),
            OutputDirectory = outputFileDirectory,
            IsNewFile = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("New formatted gamestring file created at");

        string newOutputFile = Path.Combine(outputFileDirectory, inputFileName);
        File.Exists(newOutputFile).Should().BeTrue();

        string newFileContent = File.ReadAllText(newOutputFile).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "GameStringTextFormat", "gamestrings_96477_enus_rawtext_to_plaintext_rc_rs_pc_ps.json")).ReplaceLineEndings("\n");

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task FormatGameStringText_GameStringPlaintTextTo_CTS_ReturnNewFile()
    {
        // arrange
        string inputFileName = "original_gamestrings_96477_enus_rawtext.json";
        string outputFileDirectory = Path.Combine("TestOutput", nameof(FormatGameStringText_GameStringPlaintTextTo_CTS_ReturnNewFile));

        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.ColoredTextWithScaling,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.RemoveAndUndo,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.RemoveAndUndo,
            FilePath = Path.Combine("TestJsonFiles", "GameStringTextFormat", inputFileName),
            OutputDirectory = outputFileDirectory,
            IsNewFile = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("New formatted gamestring file created at");

        string newOutputFile = Path.Combine(outputFileDirectory, inputFileName);
        File.Exists(newOutputFile).Should().BeTrue();

        string newFileContent = File.ReadAllText(newOutputFile).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "GameStringTextFormat", "gamestrings_96477_enus_rawtext_to_colortextscaling.json")).ReplaceLineEndings("\n");

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    // self update to self, nothing should change
    [TestMethod]
    public async Task FormatGameStringText_GameStringPlainTextScalingNewlineTo_PTSN_CR_SR_UpdatedFile()
    {
        // arrange
        string inputFileName = "original_gamestrings_96477_enus_plaintextscalingnewlines_rc_rs_self.json";
        string outputFileDirectory = Path.Combine("TestOutput", nameof(FormatGameStringText_GameStringPlainTextScalingNewlineTo_PTSN_CR_SR_UpdatedFile));
        _options.Value.Returns(new GameStringTextFormatOptions()
        {
            GameStringTextType = GameStringTextType.PlainTextWithScalingWithNewlines,
            GameStringTextHltConstantRemoveMode = GameStringTextHltRemoveMode.Remove,
            GameStringTextHltStyleRemoveMode = GameStringTextHltRemoveMode.Remove,
            FilePath = Path.Combine("TestJsonFiles", "GameStringTextFormat", inputFileName),
            OutputDirectory = outputFileDirectory,
            IsNewFile = false,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        GameStringTextFormatService gameStringTextFormatService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await gameStringTextFormatService.FormatGameStringText();

        // assert
        _console.Output.Should().Contain("Updated formatted gamestring file at");

        string newOutputFile = Path.Combine(outputFileDirectory, inputFileName);
        File.Exists(newOutputFile).Should().BeTrue();

        string newFileContent = File.ReadAllText(newOutputFile).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "GameStringTextFormat", "original_gamestrings_96477_enus_plaintextscalingnewlines_rc_rs_self.json")).ReplaceLineEndings("\n");

        newFileContent.Should().BeEquivalentTo(comparedToText);
    }
}
