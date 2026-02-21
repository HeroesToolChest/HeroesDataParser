using Microsoft.Extensions.DependencyInjection;

namespace HeroesDataParser.Cli.Commands.JsonPatchCommands.Tests;

[TestClass]
public class JsonApplyCommandTests
{
    private readonly ILogger<JsonApplyCommand> _logger;
    private readonly IOptions<JsonApplyOptions> _options;
    private readonly IJsonApplyService _jsonApplyService;

    public JsonApplyCommandTests()
    {
        _logger = Substitute.For<ILogger<JsonApplyCommand>>();
        _options = Substitute.For<IOptions<JsonApplyOptions>>();
        _jsonApplyService = Substitute.For<IJsonApplyService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void JsonApplyCommand_MissingRequiredArgument_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonApplyCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "filepath.json",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("required argument 'patch-file-path'");
    }

    [TestMethod]
    public void JsonApplyCommand_InvalidFilePath_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonApplyCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "nonexistant.json", Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json")
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("<file-path> does not exist");
    }

    [TestMethod]
    public void JsonApplyCommand_InvalidPatchFilePath_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonApplyCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"), "nonexistent_patch.json"
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("<patch-file-path> does not exist");
    }

    [TestMethod]
    public void JsonApplyCommand_InvalidOutputDirectory_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonApplyCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"), Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json"),
            "--output-path", "nonexistent_directory",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("--output-path does not exist");
    }

    [TestMethod]
    public async Task JsonApplyCommand_HasDefaultOutputDirectory_ReturnsError()
    {
        // arrange
        JsonApplyOptions jsonApplyOptions = new();
        _options.Value.Returns(jsonApplyOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonApplyCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"), Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json"),
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonApplyOptions.JsonFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json")));
        jsonApplyOptions.JsonPatchFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json")));
        jsonApplyOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestJsonFiles"));
    }

    [TestMethod]
    public async Task JsonApplyCommand_HasOutputDirectory_ReturnsError()
    {
        // arrange
        JsonApplyOptions jsonApplyOptions = new();
        _options.Value.Returns(jsonApplyOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonApplyCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"), Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json"),
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonApplyOptions.JsonFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json")));
        jsonApplyOptions.JsonPatchFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json")));
        jsonApplyOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
    }

    private ServiceCollection GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton(_logger);
        services.AddSingleton(_options);
        services.AddSingleton(_jsonApplyService);

        return services;
    }

    private async Task AssertCommandSuccessful(CommandAppResult result)
    {
        result.ExitCode.Should().Be(0);
        await _jsonApplyService.Received(1).ApplyJsonPatch();
    }
}
