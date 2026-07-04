namespace HeroesDataParser.Infrastructure.Commands.JsonFilePatchCommands.Tests;

[TestClass]
public class JsonApplyServiceTests
{
    private readonly ILogger<JsonApplyService> _logger;
    private readonly IOptions<JsonApplyOptions> _options;
    private readonly TestConsole _console;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    public JsonApplyServiceTests()
    {
        _logger = Substitute.For<ILogger<JsonApplyService>>();
        _options = Substitute.For<IOptions<JsonApplyOptions>>();
        _console = new TestConsole();
        _jsonSerializerOptionService = Substitute.For<IJsonSerializerOptionService>();
    }

    [TestMethod]
    public async Task ApplyJsonPatch_InvalidJsonPatchFile_ReturnsError()
    {
        // arrange
        _options.Value.Returns(new JsonApplyOptions()
        {
            JsonFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            JsonPatchFilePath = Path.Combine("TestJsonFiles", "invalid_patch.json"),
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        });

        JsonApplyService jsonApplyService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await jsonApplyService.ApplyJsonPatch();

        // assert
        _console.Output.Should().Contain("Not a valid JSON patch");
    }

    [TestMethod]
    public async Task ApplyJsonPatch_ValidJsonDataPatchFile_NewPatchedFileCreated()
    {
        // arrange
        string outputFilePath = Path.Combine(TestConstants.TestDirectory, nameof(ApplyJsonPatch_ValidJsonDataPatchFile_NewPatchedFileCreated), "announcerdata_96477_enus.json");

        _options.Value.Returns(new JsonApplyOptions()
        {
            JsonFilePath = Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            JsonPatchFilePath = Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.patch.json"),
            OutputFilePath = outputFilePath,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        JsonApplyService jsonApplyService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await jsonApplyService.ApplyJsonPatch();

        // assert
        _console.Output.Should().Contain("JSON patch applied successfully");

        string patchedText = File.ReadAllText(outputFilePath).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus_patched_map.json")).ReplaceLineEndings("\n");

        patchedText.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task ApplyJsonPatch_ValidJsonGameStringPatchFile_NewPatchedFileCreated()
    {
        // arrange
        string outputFilePath = Path.Combine(TestConstants.TestDirectory, nameof(ApplyJsonPatch_ValidJsonGameStringPatchFile_NewPatchedFileCreated), "gamestrings_96477_enus.json");
        string patchFilePath = Path.Combine("TestJsonFiles", "gamestrings_96477_enus.patch.json");
        _options.Value.Returns(new JsonApplyOptions()
        {
            JsonFilePath = Path.Combine("TestJsonFiles", "gamestrings_96477_enus.json"),
            JsonPatchFilePath = patchFilePath,
            OutputFilePath = outputFilePath,
            DeletePatchFile = false,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        JsonApplyService jsonApplyService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await jsonApplyService.ApplyJsonPatch();

        // assert
        _console.Output.Should().Contain("JSON patch applied successfully");

        string patchedText = File.ReadAllText(outputFilePath).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "gamestrings_96477_enus_patched_map.json")).ReplaceLineEndings("\n");

        patchedText.Should().BeEquivalentTo(comparedToText);

        File.Exists(patchFilePath).Should().BeTrue();
    }

    [TestMethod]
    public async Task ApplyJsonPatch_DeletePatchFile_PatchFileDeleted()
    {
        // arrange
        string testOuputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(ApplyJsonPatch_DeletePatchFile_PatchFileDeleted));
        string clonedPatchFilePath = Path.Combine(testOuputDirectory, "gamestrings_96477_enus.patch.json");
        string outputFilePath = Path.Combine(testOuputDirectory, "gamestrings_96477_enus.json");

        string patchFileContent = File.ReadAllText(Path.Combine("TestJsonFiles", "gamestrings_96477_enus.patch.json"));
        Directory.CreateDirectory(testOuputDirectory);
        using (StreamWriter streamWriter = File.CreateText(clonedPatchFilePath))
        {
            streamWriter.Write(patchFileContent);
        }

        _options.Value.Returns(new JsonApplyOptions()
        {
            JsonFilePath = Path.Combine("TestJsonFiles", "gamestrings_96477_enus.json"),
            JsonPatchFilePath = clonedPatchFilePath,
            OutputFilePath = outputFilePath,
            DeletePatchFile = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        JsonApplyService jsonApplyService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await jsonApplyService.ApplyJsonPatch();

        // assert
        _console.Output.Should().Contain("JSON patch applied successfully")
            .And.Contain("Deleted JSON patch file");

        File.Exists(clonedPatchFilePath).Should().BeFalse();
    }

    [TestMethod]
    public async Task ApplyJsonPatch_HasEmptyCollections_PatchedCorrectly()
    {
        // arrange
        string outputFilePath = Path.Combine(TestConstants.TestDirectory, nameof(ApplyJsonPatch_HasEmptyCollections_PatchedCorrectly), "bundledata_97407.json");

        _options.Value.Returns(new JsonApplyOptions()
        {
            JsonFilePath = Path.Combine("TestJsonFiles", "bundledata_97039.json"),
            JsonPatchFilePath = Path.Combine("TestJsonFiles", "bundledata_97407.patch.json"),
            OutputFilePath = outputFilePath,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        JsonApplyService jsonApplyService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await jsonApplyService.ApplyJsonPatch();

        // assert
        _console.Output.Should().Contain("JSON patch applied successfully");

        string patchedText = File.ReadAllText(outputFilePath).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "bundledata_97407_patched.json")).ReplaceLineEndings("\n");

        patchedText.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task ApplyJsonPatch__PatchedCorrectly()
    {
        // arrange
        string outputFilePath = Path.Combine(TestConstants.TestDirectory, nameof(ApplyJsonPatch__PatchedCorrectly), "herodata_97407.json");

        _options.Value.Returns(new JsonApplyOptions()
        {
            JsonFilePath = Path.Combine("TestJsonFiles", "herodata_97039_extracted.json"),
            JsonPatchFilePath = Path.Combine("TestJsonFiles", "herodata_97407.patch.json"),
            OutputFilePath = outputFilePath,
            JsonIndent = true,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

        JsonApplyService jsonApplyService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await jsonApplyService.ApplyJsonPatch();

        // assert
        _console.Output.Should().Contain("JSON patch applied successfully");

        string patchedText = File.ReadAllText(outputFilePath).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "herodata_97407_patched.json")).ReplaceLineEndings("\n");

        patchedText.Should().BeEquivalentTo(comparedToText);
    }
}
