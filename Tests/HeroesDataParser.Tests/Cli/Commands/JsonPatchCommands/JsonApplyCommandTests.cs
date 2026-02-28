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
    public void JsonApplyCommand_OutputDirectoryIsExistingFile_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonApplyCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"), Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json"),
            "--output-path", Path.Join("TestJsonFiles", "announcerdata_96477_enus.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("is an existing file and not a directory");
    }

    [TestMethod]
    public async Task JsonApplyCommand_SameOutputDirectoryNoOverwrite_ReturnsError()
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
        result.ExitCode.Should().Be(1);
        result.Output.Should().Contain("already exists");
    }

    [TestMethod]
    public async Task JsonApplyCommand_HasDefaultOutputDirectoryWithOverwrite_ReturnsSuccess()
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
            "--overwrite",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonApplyOptions.JsonFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json")));
        jsonApplyOptions.JsonPatchFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json")));
        jsonApplyOptions.OutputFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json")));
        jsonApplyOptions.AllowOverwrite.Should().BeTrue();
        jsonApplyOptions.DeletePatchFile.Should().BeFalse();
    }

    [TestMethod]
    public async Task JsonApplyCommand_HasOutputDirectory_ReturnsSuccess()
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
        jsonApplyOptions.OutputFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestXmlFiles", "announcerdata_96477_enus.json")));
    }

    [TestMethod]
    public async Task JsonApplyCommand_DeletePatchFile_ReturnsSuccess()
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
            "--delete-patch-file",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonApplyOptions.DeletePatchFile.Should().BeTrue();
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
