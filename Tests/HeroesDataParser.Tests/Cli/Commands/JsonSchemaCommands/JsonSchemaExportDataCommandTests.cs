using Microsoft.Extensions.DependencyInjection;

namespace HeroesDataParser.Cli.Commands.JsonSchemaCommands.Tests;

[TestClass]
public class JsonSchemaExportDataCommandTests
{
    private readonly ILogger<JsonSchemaExportDataCommand> _logger;
    private readonly IOptions<JsonSchemaExportOptions> _options;
    private readonly IJsonSchemaExporterService _jsonSchemaExporterService;

    public JsonSchemaExportDataCommandTests()
    {
        _logger = Substitute.For<ILogger<JsonSchemaExportDataCommand>>();
        _options = Substitute.For<IOptions<JsonSchemaExportOptions>>();
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
        JsonSchemaExportOptions jsonSchemaExportOptions = new();
        _options.Value.Returns(jsonSchemaExportOptions);

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

        jsonSchemaExportOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.All);
        jsonSchemaExportOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.Hero);
        jsonSchemaExportOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.Map);
        jsonSchemaExportOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.None);
        jsonSchemaExportOptions.JsonIndent.Should().BeTrue();
        jsonSchemaExportOptions.Version.Should().NotBe("0.0.0");
    }

    [TestMethod]
    public async Task JsonSchemaExportDataCommand_MultipleExtractorOptions_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportOptions jsonSchemaExportOptions = new();
        _options.Value.Returns(jsonSchemaExportOptions);

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

        jsonSchemaExportOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.Hero);
        jsonSchemaExportOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.MatchAward);
        jsonSchemaExportOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.Announcer);
        jsonSchemaExportOptions.JsonIndent.Should().BeTrue();
        jsonSchemaExportOptions.Version.Should().NotBe("0.0.0");
    }

    [TestMethod]
    public async Task JsonSchemaExportDataCommand_HasDefaultOutputDirectoryWithOverwrite_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportOptions jsonSchemaExportOptions = new();
        _options.Value.Returns(jsonSchemaExportOptions);

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

        jsonSchemaExportOptions.OutputDirectory.Should().Be(".");
        jsonSchemaExportOptions.AllowOverwrite.Should().BeTrue();
        jsonSchemaExportOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.Unit);
        jsonSchemaExportOptions.JsonIndent.Should().BeTrue();
    }

    [TestMethod]
    public async Task JsonSchemaExportDataCommand_HasOutputDirectory_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportOptions jsonSchemaExportOptions = new();
        _options.Value.Returns(jsonSchemaExportOptions);

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

        jsonSchemaExportOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
        jsonSchemaExportOptions.AllowOverwrite.Should().BeFalse();
        jsonSchemaExportOptions.ExtractDataOptions.Should().HaveFlag(ExtractDataOptions.Map);
        jsonSchemaExportOptions.JsonIndent.Should().BeTrue();
    }

    [TestMethod]
    public async Task JsonSchemaExportDataCommand_NoIndentArgument_ReturnsSuccess()
    {
        // arrange
        JsonSchemaExportOptions jsonSchemaExportOptions = new();
        _options.Value.Returns(jsonSchemaExportOptions);

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

        jsonSchemaExportOptions.OutputDirectory.Should().Be(".");
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
        await _jsonSchemaExporterService.Received(1).ExportDataSchema();
    }
}
