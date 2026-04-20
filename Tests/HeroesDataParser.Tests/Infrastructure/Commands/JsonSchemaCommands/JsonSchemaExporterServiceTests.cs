namespace HeroesDataParser.Infrastructure.Commands.JsonSchemaCommands.Tests;

[TestClass]
public class JsonSchemaExporterServiceTests
{
    private readonly ILogger<JsonSchemaExporterService> _logger;
    private readonly IOptions<JsonSchemaExportOptions> _options;
    private readonly TestConsole _console;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    public JsonSchemaExporterServiceTests()
    {
        _logger = Substitute.For<ILogger<JsonSchemaExporterService>>();
        _options = Substitute.For<IOptions<JsonSchemaExportOptions>>();
        _console = new TestConsole();
        _jsonSerializerOptionService = Substitute.For<IJsonSerializerOptionService>();
    }

    [TestMethod]
    public async Task ExportDataSchema_FileExistsNoOverwrite_ReturnError()
    {
        // arrange
        _options.Value.Returns(new JsonSchemaExportOptions
        {
            OutputDirectory = Path.Combine("TestJsonFiles", "Schema"),
            AllowOverwrite = false,
            JsonIndent = true,
            ExtractDataOptions = ExtractDataOptions.Hero,
            Version = "5.0.0",
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        JsonSchemaExporterService service = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await service.ExportDataSchema();

        // assert
        _console.Output.Should().Contain("output file already exists");
    }

    [TestMethod]
    [DataRow(ExtractDataOptions.Hero)]
    [DataRow(ExtractDataOptions.Unit)]
    [DataRow(ExtractDataOptions.Announcer)]
    [DataRow(ExtractDataOptions.Banner)]
    [DataRow(ExtractDataOptions.Boost)]
    [DataRow(ExtractDataOptions.Bundle)]
    [DataRow(ExtractDataOptions.Emoticon)]
    [DataRow(ExtractDataOptions.EmoticonPack)]
    [DataRow(ExtractDataOptions.LootChest)]
    [DataRow(ExtractDataOptions.Map)]
    [DataRow(ExtractDataOptions.Mount)]
    [DataRow(ExtractDataOptions.PortraitPack)]
    [DataRow(ExtractDataOptions.RewardPortrait)]
    [DataRow(ExtractDataOptions.Skin)]
    [DataRow(ExtractDataOptions.Spray)]
    [DataRow(ExtractDataOptions.TypeDescription)]
    [DataRow(ExtractDataOptions.Veterancy)]
    [DataRow(ExtractDataOptions.VoiceLine)]
    public async Task ExportDataSchema_DataType_ReturnJsonSchema(ExtractDataOptions dataType)
    {
        // arrange
        string outputFileDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportDataSchema_DataType_ReturnJsonSchema));

        _options.Value.Returns(new JsonSchemaExportOptions
        {
            OutputDirectory = outputFileDirectory,
            AllowOverwrite = true,
            JsonIndent = true,
            ExtractDataOptions = dataType,
            Version = "5.0.0",
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        JsonSchemaExporterService service = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await service.ExportDataSchema();

        // assert
        AssertSchemaFile(dataType, outputFileDirectory);
    }

    [TestMethod]
    public async Task ExportDataSchema_MultipleDataType_ReturnJsonSchema()
    {
        // arrange
        string outputFileDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportDataSchema_MultipleDataType_ReturnJsonSchema));

        _options.Value.Returns(new JsonSchemaExportOptions
        {
            OutputDirectory = outputFileDirectory,
            AllowOverwrite = true,
            JsonIndent = true,
            ExtractDataOptions = ExtractDataOptions.Hero | ExtractDataOptions.Unit | ExtractDataOptions.Announcer,
            Version = "5.0.0",
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        JsonSchemaExporterService service = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await service.ExportDataSchema();

        // assert
        AssertSchemaFile(ExtractDataOptions.Hero, outputFileDirectory);
        AssertSchemaFile(ExtractDataOptions.Unit, outputFileDirectory);
        AssertSchemaFile(ExtractDataOptions.Announcer, outputFileDirectory);
    }

    [TestMethod]
    public async Task ExportGameStringSchema_GameStrings_ReturnJsonSchema()
    {
        // arrange
        string outputFileDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ExportGameStringSchema_GameStrings_ReturnJsonSchema));

        _options.Value.Returns(new JsonSchemaExportOptions
        {
            OutputDirectory = outputFileDirectory,
            AllowOverwrite = true,
            JsonIndent = true,
            Version = "5.0.0",
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        JsonSchemaExporterService service = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await service.ExportGameStringSchema();

        // assert
        _console.Output.Should().Contain($"Exported 'gamestrings' JSON schema");

        string newOutputFile = Path.Combine(outputFileDirectory, "gamestrings_5.0.0.schema.json");
        File.Exists(newOutputFile).Should().BeTrue();

        string newFileContent = File.ReadAllText(newOutputFile).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "Schema", "gamestrings_5.0.0.schema.json")).ReplaceLineEndings("\n");
        newFileContent.Should().BeEquivalentTo(comparedToText);
    }

    private void AssertSchemaFile(ExtractDataOptions dataType, string outputFileDirectory)
    {
        string dataTypeName = dataType.ToString().ToLower();
        _console.Output.Should().Contain($"Exported '{dataTypeName}' JSON schema");

        string newOutputFile = Path.Combine(outputFileDirectory, $"{dataTypeName}data_5.0.0.schema.json");
        File.Exists(newOutputFile).Should().BeTrue();

        string newFileContent = File.ReadAllText(newOutputFile).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "Schema", $"{dataTypeName}data_5.0.0.schema.json")).ReplaceLineEndings("\n");
        newFileContent.Should().BeEquivalentTo(comparedToText);
    }
}
