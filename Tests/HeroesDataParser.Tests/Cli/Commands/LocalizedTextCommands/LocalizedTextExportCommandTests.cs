using Microsoft.Extensions.DependencyInjection;

namespace HeroesDataParser.Cli.Commands.LocalizedTextCommands.Tests;

[TestClass]
public class LocalizedTextExportCommandTests
{
    private readonly ILogger<LocalizedTextExportCommand> _logger;
    private readonly IOptions<LocalizedTextExportOptions> _options;
    private readonly ILocalizedTextExportService _localizedTextExportService;

    public LocalizedTextExportCommandTests()
    {
        _logger = Substitute.For<ILogger<LocalizedTextExportCommand>>();
        _options = Substitute.For<IOptions<LocalizedTextExportOptions>>();
        _localizedTextExportService = Substitute.For<ILocalizedTextExportService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void LocalizedTextExportCommand_MissingRequiredArgument_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<LocalizedTextExportCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "filepath.json",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("required argument 'extract-type'");
    }

    [TestMethod]
    public void LocalizedTextExportCommand_InvalidDataFilePath_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<LocalizedTextExportCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "nonexistant.json", "0",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("<data-file-path> does not exist");
    }

    [TestMethod]
    public void LocalizedTextExportCommand_InvalidExtractType_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<LocalizedTextExportCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "4",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("must be a value less than 4");
    }

    [TestMethod]
    public void LocalizedTextExportCommand_InvalidGameStringFilePath_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<LocalizedTextExportCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "0",
            "--gamestrings-file-path", "nonexistant_gamestrings.json",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("--gamestring-file-path does not exist");
    }

    [TestMethod]
    public void LocalizedTextExportCommand_ExtractTypeRemoveWithGameStringFilePath_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<LocalizedTextExportCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "2",
            "--gamestrings-file-path", Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("--gamestring-file-path cannot be provided when <extract-type> is set to Remove");
    }

    [TestMethod]
    public void LocalizedTextExportCommand_OutputDirectoryIsAnExistingFile_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<LocalizedTextExportCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "0",
            "--output-path", Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("existing file and not a directory");
    }

    [TestMethod]
    public async Task LocalizedTextExportCommand_HasDefaultOutputDirectory_ReturnsSuccess()
    {
        // arrange
        LocalizedTextExportOptions localizedTextExportOptions = new();
        _options.Value.Returns(localizedTextExportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<LocalizedTextExportCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "0",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        localizedTextExportOptions.DataFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json")));
        localizedTextExportOptions.GameStringFilePath.Should().BeNull();
        localizedTextExportOptions.ExtractType.Should().Be(ExtractType.Copy);
        localizedTextExportOptions.OutputDirectory.Should().Be(Path.Combine(Path.GetFullPath("."), "TestJsonFiles"));
        localizedTextExportOptions.AllowOverwrite.Should().BeFalse();
        localizedTextExportOptions.JsonIndent.Should().BeTrue();
    }

    [TestMethod]
    public async Task LocalizedTextExportCommand_HasOutputDirectory_ReturnsSuccess()
    {
        // arrange
        LocalizedTextExportOptions localizedTextExportOptions = new();
        _options.Value.Returns(localizedTextExportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<LocalizedTextExportCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "0",
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        localizedTextExportOptions.DataFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json")));
        localizedTextExportOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
        localizedTextExportOptions.AllowOverwrite.Should().BeFalse();
    }

    [TestMethod]
    public async Task LocalizedTextExportCommand_WithOverwrite_ReturnsSuccess()
    {
        // arrange
        LocalizedTextExportOptions localizedTextExportOptions = new();
        _options.Value.Returns(localizedTextExportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<LocalizedTextExportCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "1",
            "--overwrite",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        localizedTextExportOptions.ExtractType.Should().Be(ExtractType.Extract);
        localizedTextExportOptions.AllowOverwrite.Should().BeTrue();
    }

    [TestMethod]
    public async Task LocalizedTextExportCommand_WithGameStringFilePath_ReturnsSuccess()
    {
        // arrange
        LocalizedTextExportOptions localizedTextExportOptions = new();
        _options.Value.Returns(localizedTextExportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<LocalizedTextExportCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "0",
            "--gamestrings-file-path", Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        localizedTextExportOptions.GameStringFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json")));
    }

    [TestMethod]
    public async Task LocalizedTextExportCommand_NoIndentArgument_ReturnsSuccess()
    {
        // arrange
        LocalizedTextExportOptions localizedTextExportOptions = new();
        _options.Value.Returns(localizedTextExportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<LocalizedTextExportCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "0",
            "--overwrite",
            "--no-indent",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        localizedTextExportOptions.JsonIndent.Should().BeFalse();
    }

    private ServiceCollection GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton(_logger);
        services.AddSingleton(_options);
        services.AddSingleton(_localizedTextExportService);

        return services;
    }

    private async Task AssertCommandSuccessful(CommandAppResult result)
    {
        result.ExitCode.Should().Be(0);
        await _localizedTextExportService.Received(1).ExportGameStrings();
    }
}