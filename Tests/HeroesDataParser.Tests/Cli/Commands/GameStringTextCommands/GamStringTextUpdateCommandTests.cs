using Microsoft.Extensions.DependencyInjection;

namespace HeroesDataParser.Cli.Commands.GameStringTextCommands.Tests;

[TestClass]
public class GamStringTextUpdateCommandTests
{
    private readonly ILogger<GameStringTextFormatCommand> _logger;
    private readonly IOptions<GameStringTextFormatOptions> _options;
    private readonly IGameStringTextUpdateService _gameStringTextUpdateService;

    public GamStringTextUpdateCommandTests()
    {
        _logger = Substitute.For<ILogger<GameStringTextFormatCommand>>();
        _options = Substitute.For<IOptions<GameStringTextFormatOptions>>();
        _gameStringTextUpdateService = Substitute.For<IGameStringTextUpdateService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void GameStringTextUpdateCommand_MissingRequiredArgument_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<GameStringTextFormatCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "filepath.json",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("required argument 'type'");
    }

    [TestMethod]
    public void GameStringTextUpdateCommand_InvalidFilePath_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<GameStringTextFormatCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "nonexistant.json", "1"
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("<file-path> does not exist");
    }

    [TestMethod]
    public void GameStringTextUpdateCommand_InvalidGameStringText_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<GameStringTextFormatCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "7"
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("must be a value less than 7");
    }

    [TestMethod]
    public void GameStringTextUpdateCommand_InvalidHtlStyle_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<GameStringTextFormatCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "1",
            "--hlt-style-mode", "3"
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("must be a value less than 3");
    }

    [TestMethod]
    public void GameStringTextUpdateCommand_InvalidHtlConstant_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<GameStringTextFormatCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "1",
            "--hlt-constant-mode", "3"
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("must be a value less than 3");
    }

    [TestMethod]
    public void GameStringTextUpdateCommand_OutputDirectoryIsAnExistingFile_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<GameStringTextFormatCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "1",
            "--output-path", Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json")
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("existing file and not a directory");
    }

    [TestMethod]
    public async Task GameStringTextUpdateCommand_SameOutputDirectoryNoOverwrite_ReturnsError()
    {
        // arrange
        GameStringTextFormatOptions gameStringTextUpdateOptions = new();
        _options.Value.Returns(gameStringTextUpdateOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<GameStringTextFormatCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "1",
            "--output-path", "TestJsonFiles",
        ],
        TestContext.CancellationToken);

        // assert
        result.ExitCode.Should().Be(1);
        result.Output.Should().Contain("Output file already exists");
    }

    [TestMethod]
    public async Task GameStringTextUpdateCommand_HasDefaultOutputDirectoryWithOverwrite_ReturnsSuccess()
    {
        // arrange
        GameStringTextFormatOptions gameStringTextUpdateOptions = new();
        _options.Value.Returns(gameStringTextUpdateOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<GameStringTextFormatCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "1",
            "--overwrite"
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        gameStringTextUpdateOptions.FilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json")));
        gameStringTextUpdateOptions.GameStringTextType.Should().Be(GameStringTextType.PlainText);
        gameStringTextUpdateOptions.OutputDirectory.Should().Be(".");
        gameStringTextUpdateOptions.AllowOverwrite.Should().BeTrue();
        gameStringTextUpdateOptions.GameStringTextHltConstantRemoveMode.Should().Be(GameStringTextHltRemoveMode.None);
        gameStringTextUpdateOptions.GameStringTextHltStyleRemoveMode.Should().Be(GameStringTextHltRemoveMode.None);
        gameStringTextUpdateOptions.JsonIndent.Should().BeTrue();
    }

    [TestMethod]
    public async Task GameStringTextUpdateCommand_HasOutputDirectory_ReturnsSuccess()
    {
        // arrange
        GameStringTextFormatOptions gameStringTextUpdateOptions = new();
        _options.Value.Returns(gameStringTextUpdateOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<GameStringTextFormatCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "1",
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        gameStringTextUpdateOptions.FilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json")));
        gameStringTextUpdateOptions.GameStringTextType.Should().Be(GameStringTextType.PlainText);
        gameStringTextUpdateOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
        gameStringTextUpdateOptions.AllowOverwrite.Should().BeFalse();
        gameStringTextUpdateOptions.GameStringTextHltConstantRemoveMode.Should().Be(GameStringTextHltRemoveMode.None);
        gameStringTextUpdateOptions.GameStringTextHltStyleRemoveMode.Should().Be(GameStringTextHltRemoveMode.None);
    }

    [TestMethod]
    public async Task GameStringTextUpdateCommand_HasHltOptions_ReturnsSuccess()
    {
        // arrange
        GameStringTextFormatOptions gameStringTextUpdateOptions = new();
        _options.Value.Returns(gameStringTextUpdateOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<GameStringTextFormatCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "1",
            "--hlt-constant-mode", "1",
            "--hlt-style-mode", "2",
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        gameStringTextUpdateOptions.FilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json")));
        gameStringTextUpdateOptions.GameStringTextType.Should().Be(GameStringTextType.PlainText);
        gameStringTextUpdateOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
        gameStringTextUpdateOptions.AllowOverwrite.Should().BeFalse();
        gameStringTextUpdateOptions.GameStringTextHltConstantRemoveMode.Should().Be(GameStringTextHltRemoveMode.Remove);
        gameStringTextUpdateOptions.GameStringTextHltStyleRemoveMode.Should().Be(GameStringTextHltRemoveMode.RemoveAndUndo);
    }

    [TestMethod]
    public async Task GameStringTextUpdateCommand_NoIndentArgument_ReturnsSuccess()
    {
        // arrange
        GameStringTextFormatOptions gameStringTextUpdateOptions = new();
        _options.Value.Returns(gameStringTextUpdateOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<GameStringTextFormatCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "herodata_96477_enus_rawtext.json"), "1",
            "--no-indent",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        gameStringTextUpdateOptions.JsonIndent.Should().BeFalse();
    }

    private ServiceCollection GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton(_logger);
        services.AddSingleton(_options);
        services.AddSingleton(_gameStringTextUpdateService);

        return services;
    }

    private async Task AssertCommandSuccessful(CommandAppResult result)
    {
        result.ExitCode.Should().Be(0);
        await _gameStringTextUpdateService.Received(1).FormatGameStringText();
    }
}
