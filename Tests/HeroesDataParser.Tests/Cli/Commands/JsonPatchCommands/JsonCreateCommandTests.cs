using Microsoft.Extensions.DependencyInjection;

namespace HeroesDataParser.Cli.Commands.JsonPatchCommands.Tests;

[TestClass]
public class JsonCreateCommandTests
{
    private readonly ILogger<JsonCreateCommand> _logger;
    private readonly IOptions<JsonCreateOptions> _options;
    private readonly IJsonCreateService _jsonCreateService;

    public JsonCreateCommandTests()
    {
        _logger = Substitute.For<ILogger<JsonCreateCommand>>();
        _options = Substitute.For<IOptions<JsonCreateOptions>>();
        _jsonCreateService = Substitute.For<IJsonCreateService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void JsonCreateCommand_MissingRequiredArgument_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonCreateCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "oldfile.json",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("required argument 'new-file-path'");
    }

    [TestMethod]
    public void JsonCreateCommand_OldFileDoesNotExist_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonCreateCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "some-file-that-doesnt-exist.json"),
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus_patched_map.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("<old-file-path> does not exist");
    }

    [TestMethod]
    public void JsonCreateCommand_NewFileDoesNotExist_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonCreateCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            Path.Combine("TestJsonFiles", "some-file-that-doesnt-exist.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("<new-file-path> does not exist");
    }

    [TestMethod]
    public void JsonCreateCommand_OutputDiretoryIsAnExistingFile_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonCreateCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus_patched_map.json"),
            "-o", Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("is an existing file and not");
    }

    [TestMethod]
    public async Task JsonApplyCommand_SameOutputDirectoryNoOverwrite_ReturnsError()
    {
        // arrange
        JsonCreateOptions jsonCreateOptions = new();
        _options.Value.Returns(jsonCreateOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonCreateCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json"),
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
        JsonCreateOptions jsonCreateOptions = new();
        _options.Value.Returns(jsonCreateOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonCreateCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json"),
            "--overwrite",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonCreateOptions.OldJsonFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json")));
        jsonCreateOptions.NewJsonFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json")));
        jsonCreateOptions.OutputFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json")));
        jsonCreateOptions.AllowOverwrite.Should().BeTrue();
    }

    [TestMethod]
    public async Task JsonApplyCommand_HasOutputDirectory_ReturnsSuccess()
    {
        // arrange
        JsonCreateOptions jsonCreateOptions = new();
        _options.Value.Returns(jsonCreateOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonCreateCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
            Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json"),
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonCreateOptions.OldJsonFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json")));
        jsonCreateOptions.NewJsonFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerdata_96477_enus.patch.json")));
        jsonCreateOptions.OutputFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestXmlFiles", "announcerdata_96477_enus.patch.json")));
        jsonCreateOptions.AllowOverwrite.Should().BeFalse();
    }

    private ServiceCollection GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton(_logger);
        services.AddSingleton(_options);
        services.AddSingleton(_jsonCreateService);

        return services;
    }

    private async Task AssertCommandSuccessful(CommandAppResult result)
    {
        result.ExitCode.Should().Be(0);
        await _jsonCreateService.Received(1).CreateJsonPatch();
    }
}
