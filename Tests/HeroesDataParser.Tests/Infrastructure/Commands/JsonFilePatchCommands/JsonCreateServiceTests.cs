namespace HeroesDataParser.Infrastructure.Commands.JsonFilePatchCommands.Tests;

[TestClass]
public class JsonCreateServiceTests
{
    private readonly ILogger<JsonCreateService> _logger;
    private readonly IOptions<JsonCreateOptions> _options;
    private readonly TestConsole _console;

    public JsonCreateServiceTests()
    {
        _logger = Substitute.For<ILogger<JsonCreateService>>();
        _options = Substitute.For<IOptions<JsonCreateOptions>>();
        _console = new TestConsole();
    }

    [TestMethod]
    public async Task CreateJsonPatch_OldFileIsNull_ReturnsError()
    {
        // arrange
        string outputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(CreateJsonPatch_OldFileIsNull_ReturnsError));
        Directory.CreateDirectory(outputDirectory);

        using (StreamWriter streamWriter = File.CreateText(Path.Combine(outputDirectory, "null_json.json")))
        {
            streamWriter.WriteLine("null");
        }

        _options.Value.Returns(new JsonCreateOptions()
        {
            OldJsonFilePath = Path.Combine(outputDirectory, "null_json.json"),
            NewJsonFilePath = Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus_patched_map.json"),
        });
        JsonCreateService jsonCreateService = new(_logger, _options, _console);

        // act
        await jsonCreateService.CreateJsonPatch();

        // assert
        _console.Output.Should().Contain("Either JSON file is the 'null' json value");
    }

    [TestMethod]
    public async Task CreateJsonPatch_NewFileIsNull_ReturnsError()
    {
        // arrange
        string outputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(CreateJsonPatch_NewFileIsNull_ReturnsError));
        Directory.CreateDirectory(outputDirectory);

        using (StreamWriter streamWriter = File.CreateText(Path.Combine(outputDirectory, "null_json.json")))
        {
            streamWriter.WriteLine("null");
        }

        _options.Value.Returns(new JsonCreateOptions()
        {
            OldJsonFilePath = Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            NewJsonFilePath = Path.Combine(outputDirectory, "null_json.json"),
        });
        JsonCreateService jsonCreateService = new(_logger, _options, _console);

        // act
        await jsonCreateService.CreateJsonPatch();

        // assert
        _console.Output.Should().Contain("Either JSON file is the 'null' json value");
    }

    [TestMethod]
    public async Task CreateJsonPatch_NoDifferences_ReturnsNoPatch()
    {
        // arrange
        _options.Value.Returns(new JsonCreateOptions()
        {
            OldJsonFilePath = Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            NewJsonFilePath = Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
        });
        JsonCreateService jsonCreateService = new(_logger, _options, _console);

        // act
        await jsonCreateService.CreateJsonPatch();

        // assert
        _console.Output.Should().Contain("No differences found between the old and new");
    }

    [TestMethod]
    public async Task CreateJsonPatch_HasDifferences_CreatesPatchFile()
    {
        string outputFilePath = Path.Combine(TestConstants.TestDirectory, nameof(CreateJsonPatch_HasDifferences_CreatesPatchFile), "announcerpackdata_96477_enus_patched_map.patch.json");

        // arrange
        _options.Value.Returns(new JsonCreateOptions()
        {
            OldJsonFilePath = Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            NewJsonFilePath = Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus_patched_map.json"),
            OutputFilePath = outputFilePath,
            AllowOverwrite = true,
            JsonIndent = true,
        });
        JsonCreateService jsonCreateService = new(_logger, _options, _console);

        // act
        await jsonCreateService.CreateJsonPatch();

        // assert
        _console.Output.Should().Contain("JSON patch created successfully");

        File.Exists(outputFilePath).Should().BeTrue();

        string patchText = File.ReadAllText(outputFilePath).ReplaceLineEndings("\n");
        string compareText = File.ReadAllText(Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.patch.json")).ReplaceLineEndings("\n");
        patchText.Should().Be(compareText);
    }
}
