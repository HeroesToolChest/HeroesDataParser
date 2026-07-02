using Microsoft.Extensions.DependencyInjection;

namespace HeroesDataParser.Cli.Commands.LocalizedTextCommands.Tests;

[TestClass]
public class LocalizedTextImportCommandTests
{
    private readonly ILogger<LocalizedTextImportCommand> _logger;
    private readonly IOptions<LocalizedTextImportOptions> _options;
    private readonly ILocalizedTextImportService _localizedTextImportService;

    public LocalizedTextImportCommandTests()
    {
        _logger = Substitute.For<ILogger<LocalizedTextImportCommand>>();
        _options = Substitute.For<IOptions<LocalizedTextImportOptions>>();
        _localizedTextImportService = Substitute.For<ILocalizedTextImportService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void LocalizedTextImportCommand_MissingRequiredArgument_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<LocalizedTextImportCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "filepath.json",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("required argument 'gamestrings-file-path'");
    }

    [TestMethod]
    public void LocalizedTextImportCommand_InvalidDataFilePath_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<LocalizedTextImportCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "nonexistant.json",
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("<data-file-path> does not exist");
    }

    [TestMethod]
    public void LocalizedTextImportCommand_InvalidGameStringsFilePath_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<LocalizedTextImportCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            "nonexistant_gamestrings.json",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("<gamestring-file-path> does not exist");
    }

    [TestMethod]
    public void LocalizedTextImportCommand_OutputDirectoryIsAnExistingFile_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<LocalizedTextImportCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            "--output-path", Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("existing file and not a directory");
    }

    [TestMethod]
    public async Task LocalizedTextImportCommand_SameOutputDirectoryNoOverwrite_ReturnsError()
    {
        // arrange
        LocalizedTextImportOptions localizedTextImportOptions = new();
        _options.Value.Returns(localizedTextImportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<LocalizedTextImportCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            "--output-path", "TestJsonFiles",
        ],
        TestContext.CancellationToken);

        // assert
        result.ExitCode.Should().Be(1);
        result.Output.Should().Contain("Output file already exists");
    }

    [TestMethod]
    public async Task LocalizedTextImportCommand_HasDefaultOutputDirectoryWithOverwrite_ReturnsSuccess()
    {
        // arrange
        LocalizedTextImportOptions localizedTextImportOptions = new();
        _options.Value.Returns(localizedTextImportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<LocalizedTextImportCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            "--overwrite",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        localizedTextImportOptions.DataFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json")));
        localizedTextImportOptions.GameStringsFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json")));
        localizedTextImportOptions.OutputDirectory.Should().Be(Path.Combine(Path.GetFullPath("."), "TestJsonFiles"));
        localizedTextImportOptions.AllowOverwrite.Should().BeTrue();
        localizedTextImportOptions.JsonIndent.Should().BeTrue();
    }

    [TestMethod]
    public async Task LocalizedTextImportCommand_HasOutputDirectory_ReturnsSuccess()
    {
        // arrange
        LocalizedTextImportOptions localizedTextImportOptions = new();
        _options.Value.Returns(localizedTextImportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<LocalizedTextImportCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        localizedTextImportOptions.DataFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json")));
        localizedTextImportOptions.GameStringsFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json")));
        localizedTextImportOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
        localizedTextImportOptions.AllowOverwrite.Should().BeFalse();
    }

    [TestMethod]
    public async Task LocalizedTextImportCommand_NoIndentArgument_ReturnsSuccess()
    {
        // arrange
        LocalizedTextImportOptions localizedTextImportOptions = new();
        _options.Value.Returns(localizedTextImportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<LocalizedTextImportCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            "--overwrite",
            "--no-indent",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        localizedTextImportOptions.JsonIndent.Should().BeFalse();
    }

    [TestMethod]
    public async Task LocalizedTextImportCommand_OutputFileDoesNotExist_IsNewFileIsTrue()
    {
        // arrange
        LocalizedTextImportOptions localizedTextImportOptions = new();
        _options.Value.Returns(localizedTextImportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<LocalizedTextImportCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        localizedTextImportOptions.IsNewFile.Should().BeTrue();
    }

    [TestMethod]
    public async Task LocalizedTextImportCommand_OutputFileAlreadyExists_IsNewFileIsFalse()
    {
        // arrange
        LocalizedTextImportOptions localizedTextImportOptions = new();
        _options.Value.Returns(localizedTextImportOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<LocalizedTextImportCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"),
            "--output-path", "TestJsonFiles",
            "--overwrite",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        localizedTextImportOptions.IsNewFile.Should().BeFalse();
    }

    private ServiceCollection GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton(_logger);
        services.AddSingleton(_options);
        services.AddSingleton(_localizedTextImportService);

        return services;
    }

    private async Task AssertCommandSuccessful(CommandAppResult result)
    {
        result.ExitCode.Should().Be(0);
        await _localizedTextImportService.Received(1).ImportGameStrings();
    }
}