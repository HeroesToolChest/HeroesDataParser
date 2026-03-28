using Microsoft.Extensions.DependencyInjection;

namespace HeroesDataParser.Cli.Commands.JsonSchemaCommands.Tests;

[TestClass]
public class JsonSchemaExportDataCommandTests
{
    private readonly ILogger<JsonSchemaExportDataCommand> _logger;
    private readonly IOptions<JsonSchemaExportDataOptions> _options;
    private readonly IJsonSchemaExporterService _jsonSchemaExporterService;

    public JsonSchemaExportDataCommandTests()
    {
        _logger = Substitute.For<ILogger<JsonSchemaExportDataCommand>>();
        _options = Substitute.For<IOptions<JsonSchemaExportDataOptions>>();
        _jsonSchemaExporterService = Substitute.For<IJsonSchemaExporterService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void JsonSchemaExportDataCommand_MissingRequiredOption_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonSchemaExportDataCommand>();

        // act
        CommandAppResult result = app.Run(
        [
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("At least one --extractor must be specified");
    }

    [TestMethod]
    public void JsonSchemaExportDataCommand_InvalidExtractorOption_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonSchemaExportDataCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "--extractor", "hero",
            "--extractor", "aaaaaaaa",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("has an invalid extractor 'aaaaaaaa'");
    }

    [TestMethod]
    public void JsonSchemaExportDataCommand_OutputDirectoryIsAnExistingFile_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<JsonSchemaExportDataCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "--extractor", "hero",
            "--output-path", Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("existing file and not a directory");
    }

    [TestMethod]
    public async Task JsonSchemaExportDataCommand_HasExtractorAllOption_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportDataOptions jsonSchemaExportDataOptions = new();
        _options.Value.Returns(jsonSchemaExportDataOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonSchemaExportDataCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "--extractor", "all",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonSchemaExportDataOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.All);
        jsonSchemaExportDataOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.Hero);
        jsonSchemaExportDataOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.Map);
        jsonSchemaExportDataOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.None);
        jsonSchemaExportDataOptions.JsonIndent.Should().BeTrue();
        jsonSchemaExportDataOptions.Version.Should().NotBe("0.0.0");
    }

    [TestMethod]
    public async Task JsonSchemaExportDataCommand_MultipleExtractorOptions_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportDataOptions jsonSchemaExportDataOptions = new();
        _options.Value.Returns(jsonSchemaExportDataOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonSchemaExportDataCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "--extractor", "hero",
            "--extractor", "matchaward",
            "-e", "announcer",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonSchemaExportDataOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.Hero);
        jsonSchemaExportDataOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.MatchAward);
        jsonSchemaExportDataOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.Announcer);
        jsonSchemaExportDataOptions.JsonIndent.Should().BeTrue();
        jsonSchemaExportDataOptions.Version.Should().NotBe("0.0.0");
    }

    [TestMethod]
    public async Task JsonSchemaExportDataCommand_HasDefaultOutputDirectoryWithOverwrite_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportDataOptions jsonSchemaExportDataOptions = new();
        _options.Value.Returns(jsonSchemaExportDataOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonSchemaExportDataCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "--extractor", "unit",
            "--overwrite"
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonSchemaExportDataOptions.OutputDirectory.Should().Be(".");
        jsonSchemaExportDataOptions.AllowOverwrite.Should().BeTrue();
        jsonSchemaExportDataOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.Unit);
        jsonSchemaExportDataOptions.JsonIndent.Should().BeTrue();
    }

    [TestMethod]
    public async Task JsonSchemaExportDataCommand_HasOutputDirectory_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportDataOptions jsonSchemaExportDataOptions = new();
        _options.Value.Returns(jsonSchemaExportDataOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonSchemaExportDataCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "--extractor", "map",
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonSchemaExportDataOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
        jsonSchemaExportDataOptions.AllowOverwrite.Should().BeFalse();
        jsonSchemaExportDataOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.Map);
        jsonSchemaExportDataOptions.JsonIndent.Should().BeTrue();
    }

    [TestMethod]
    public async Task JsonSchemaExportDataCommand_NoIndentArgument_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportDataOptions jsonSchemaExportDataOptions = new();
        _options.Value.Returns(jsonSchemaExportDataOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<JsonSchemaExportDataCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "--extractor", "map",
            "--no-indent",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        jsonSchemaExportDataOptions.OutputDirectory.Should().Be(".");
        jsonSchemaExportDataOptions.AllowOverwrite.Should().BeFalse();
        jsonSchemaExportDataOptions.JsonIndent.Should().BeFalse();
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
        await _jsonSchemaExporterService.Received(1).ExportDataSchema();
    }
}
