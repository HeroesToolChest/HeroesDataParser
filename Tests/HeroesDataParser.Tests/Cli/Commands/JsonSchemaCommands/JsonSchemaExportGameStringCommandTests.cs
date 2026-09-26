namespace HeroesDataParser.Cli.Commands.JsonSchemaCommands.Tests;

[TestClass]
public class JsonSchemaExportGameStringCommandTests
{
    private readonly ILogger<JsonSchemaExportGameStringCommand> _logger;
    private readonly IOptions<JsonSchemaExportOptions> _options;
    private readonly IJsonSchemaExporterService _jsonSchemaExporterService;

    public JsonSchemaExportGameStringCommandTests()
    {
        _logger = Substitute.For<ILogger<JsonSchemaExportGameStringCommand>>();
        _options = Substitute.For<IOptions<JsonSchemaExportOptions>>();
        _jsonSchemaExporterService = Substitute.For<IJsonSchemaExporterService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void JsonSchemaExportGameStringCommand_OutputDirectoryIsAnExistingFile_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonSchemaExportGameStringCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "--output-path", Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("existing file and not a directory");
    }

    [TestMethod]
    public async Task JsonSchemaExportGameStringCommand_HasDefaultOutputDirectory_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportOptions jsonSchemaExportOptions = new();
        _options.Value.Returns(jsonSchemaExportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonSchemaExportGameStringCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonSchemaExportOptions.OutputDirectory.Should().Be(Path.Combine(Path.GetFullPath("."), "schema"));
        jsonSchemaExportOptions.AllowOverwrite.Should().BeFalse();
        jsonSchemaExportOptions.JsonIndent.Should().BeTrue();
        jsonSchemaExportOptions.Version.Should().NotBe("0.0.0");
    }

    [TestMethod]
    public async Task JsonSchemaExportGameStringCommand_HasDefaultOutputDirectoryWithOverwrite_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportOptions jsonSchemaExportOptions = new();
        _options.Value.Returns(jsonSchemaExportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonSchemaExportGameStringCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "--overwrite",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonSchemaExportOptions.OutputDirectory.Should().Be(Path.Combine(Path.GetFullPath("."), "schema"));
        jsonSchemaExportOptions.AllowOverwrite.Should().BeTrue();
        jsonSchemaExportOptions.JsonIndent.Should().BeTrue();
    }

    [TestMethod]
    public async Task JsonSchemaExportGameStringCommand_HasOutputDirectory_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportOptions jsonSchemaExportOptions = new();
        _options.Value.Returns(jsonSchemaExportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonSchemaExportGameStringCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonSchemaExportOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
        jsonSchemaExportOptions.AllowOverwrite.Should().BeFalse();
        jsonSchemaExportOptions.JsonIndent.Should().BeTrue();
    }

    [TestMethod]
    public async Task JsonSchemaExportGameStringCommand_NoIndentArgument_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportOptions jsonSchemaExportOptions = new();
        _options.Value.Returns(jsonSchemaExportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonSchemaExportGameStringCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "--no-indent",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonSchemaExportOptions.OutputDirectory.Should().Be(Path.Combine(Path.GetFullPath("."), "schema"));
        jsonSchemaExportOptions.AllowOverwrite.Should().BeFalse();
        jsonSchemaExportOptions.JsonIndent.Should().BeFalse();
    }

    private ServiceCollection GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton(_logger);
        services.AddSingleton(_options);
        services.AddSingleton(_jsonSchemaExporterService);

        return services;
    }

    private async Task AssertCommandSuccessful(CommandAppResult result)
    {
        result.ExitCode.Should().Be(0);
        await _jsonSchemaExporterService.Received(1).ExportGameStringSchema();
    }
}