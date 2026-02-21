namespace HeroesDataParser.Infrastructure.JsonFilePatch.Tests;

[TestClass]
public class JsonApplyServiceTests
{
    private readonly ILogger<JsonApplyService> _logger;
    private readonly IOptions<JsonApplyOptions> _options;
    private readonly TestConsole _console;

    public JsonApplyServiceTests()
    {
        _logger = Substitute.For<ILogger<JsonApplyService>>();
        _options = Substitute.For<IOptions<JsonApplyOptions>>();
        _console = new TestConsole();
    }

    [TestMethod]
    public async Task ApplyJsonPatch_InvalidJsonPatchFile_ReturnsError()
    {
        // arrange
        _options.Value.Returns(new JsonApplyOptions()
        {
            JsonFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            JsonPatchFilePath = Path.Combine("TestJsonFiles", "invalid_patch.json"),
            OutputDirectory = "TestOutput",
        });

        JsonApplyService jsonApplyService = new(_logger, _options, _console);

        // act
        await jsonApplyService.ApplyJsonPatch();

        // assert
        _console.Output.Should().Contain("Not a valid JSON patch");
    }

    [TestMethod]
    public async Task ApplyJsonPatch_ValidJsonPatchFile_ReturnsError()
    {
        // arrange
        _options.Value.Returns(new JsonApplyOptions()
        {
            JsonFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            JsonPatchFilePath = Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json"),
            OutputDirectory = "TestOutput",
        });

        JsonApplyService jsonApplyService = new(_logger, _options, _console);

        // act
        await jsonApplyService.ApplyJsonPatch();

        // assert
        _console.Output.Should().Contain("JSON patch applied successfully");

        string patchedText = File.ReadAllText(Path.Combine("TestOutput", "announcerdata_96477_enus.json"));
        string comparedToText = File.ReadAllText(Path.Combine("TestJsonFiles", "announcerdata_96477_enus_patched_map.json"));

        patchedText.Should().BeEquivalentTo(comparedToText);
    }
}
