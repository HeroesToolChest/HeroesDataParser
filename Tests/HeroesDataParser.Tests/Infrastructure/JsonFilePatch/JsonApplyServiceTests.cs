using HeroesDataParser.Options.JsonFilePatchOptions;

namespace HeroesDataParser.Infrastructure.JsonFilePatch.Tests;

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
        string outputFilePath = Path.Combine("TestOutput", nameof(ApplyJsonPatch_ValidJsonDataPatchFile_NewPatchedFileCreated), "announcerdata_96477_enus.json");

        _options.Value.Returns(new JsonApplyOptions()
        {
            JsonFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            JsonPatchFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json"),
            OutputFilePath = outputFilePath,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(),
                new DoubleRoundingConverter(),
                new LinkIdConverter(),
                new AbilityLinkIdConverter(),
                new TalentLinkIdConverter(),
                new HeroesDataVersionConverter(),
            },
        });

        JsonApplyService jsonApplyService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await jsonApplyService.ApplyJsonPatch();

        // assert
        _console.Output.Should().Contain("JSON patch applied successfully");

        string patchedText = File.ReadAllText(outputFilePath);
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "announcerdata_96477_enus_patched_map.json"));

        patchedText.Should().BeEquivalentTo(comparedToText);
    }

    [TestMethod]
    public async Task ApplyJsonPatch_ValidJsonGameStringPatchFile_NewPatchedFileCreated()
    {
        // arrange
        string outputFilePath = Path.Combine("TestOutput", nameof(ApplyJsonPatch_ValidJsonGameStringPatchFile_NewPatchedFileCreated), "gamestrings_96477_enus.json");
        string patchFilePath = Path.Combine("TestJsonFiles", "gamestrings_96477_enus.patch.json");
        _options.Value.Returns(new JsonApplyOptions()
        {
            JsonFilePath = Path.Combine("TestJsonFiles", "gamestrings_96477_enus.json"),
            JsonPatchFilePath = patchFilePath,
            OutputFilePath = outputFilePath,
            DeletePatchFile = false,
        });

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(),
                new DoubleRoundingConverter(),
                new LinkIdConverter(),
                new AbilityLinkIdConverter(),
                new TalentLinkIdConverter(),
                new HeroesDataVersionConverter(),
            },
        });

        JsonApplyService jsonApplyService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await jsonApplyService.ApplyJsonPatch();

        // assert
        _console.Output.Should().Contain("JSON patch applied successfully");

        string patchedText = File.ReadAllText(outputFilePath);
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "gamestrings_96477_enus_patched_map.json"));

        patchedText.Should().BeEquivalentTo(comparedToText);

        File.Exists(patchFilePath).Should().BeTrue();
    }

    [TestMethod]
    public async Task ApplyJsonPatch_DeletePatchFile_PatchFileDeleted()
    {
        // arrange
        string testOuputDirectory = Path.Combine("TestOutput", nameof(ApplyJsonPatch_DeletePatchFile_PatchFileDeleted));
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

        _jsonSerializerOptionService.GeneralJsonSerializerOptions.Returns(new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(),
                new DoubleRoundingConverter(),
                new LinkIdConverter(),
                new AbilityLinkIdConverter(),
                new TalentLinkIdConverter(),
                new HeroesDataVersionConverter(),
            },
        });

        JsonApplyService jsonApplyService = new(_logger, _options, _console, _jsonSerializerOptionService);

        // act
        await jsonApplyService.ApplyJsonPatch();

        // assert
        _console.Output.Should().Contain("JSON patch applied successfully")
            .And.Contain("Deleted JSON patch file");

        File.Exists(clonedPatchFilePath).Should().BeFalse();
    }
}
