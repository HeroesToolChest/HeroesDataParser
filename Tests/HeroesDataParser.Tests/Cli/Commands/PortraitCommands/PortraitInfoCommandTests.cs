using Microsoft.Extensions.DependencyInjection;

namespace HeroesDataParser.Cli.Commands.PortraitCommands.Tests;

[TestClass]
public class PortraitInfoCommandTests
{
    private readonly ILogger<PortraitInfoCommand> _logger;
    private readonly IOptions<PortraitInfoOptions> _options;
    private readonly IPortraitInfoService _portraitInfoService;

    public PortraitInfoCommandTests()
    {
        _logger = Substitute.For<ILogger<PortraitInfoCommand>>();
        _options = Substitute.For<IOptions<PortraitInfoOptions>>();
        _portraitInfoService = Substitute.For<IPortraitInfoService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void PortraitInfoCommand_FilePathDoesNotExist_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitInfoCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "nonexistent.json",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("<rewardportrait-file-path> does not exist");
    }

    [TestMethod]
    public void PortraitInfoCommand_IconSlotNegativeValue_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitInfoCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            "-s", "-1",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("--icon-slot value must be a non-negative integer");
    }

    [TestMethod]
    public async Task PortraitInfoCommand_WithDefaults_ExecutesSuccessfully()
    {
        // arrange
        PortraitInfoOptions portraitInfoOptions = new();
        _options.Value.Returns(portraitInfoOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitInfoCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitInfoOptions.RewardPortraitDataFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json")));
        portraitInfoOptions.ShowTextureSheetsFileNames.Should().BeFalse();
        portraitInfoOptions.ShowIconSlotFileNames.Should().BeNull();
        portraitInfoOptions.TextureSheetImageName.Should().BeNull();
    }

    [TestMethod]
    public async Task PortraitInfoCommand_WithTextureSheets_ExecutesSuccessfully()
    {
        // arrange
        PortraitInfoOptions portraitInfoOptions = new();
        _options.Value.Returns(portraitInfoOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitInfoCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            "-t",
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitInfoOptions.ShowTextureSheetsFileNames.Should().BeTrue();
    }

    [TestMethod]
    public async Task PortraitInfoCommand_WithIconSlot_ExecutesSuccessfully()
    {
        // arrange
        PortraitInfoOptions portraitInfoOptions = new();
        _options.Value.Returns(portraitInfoOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitInfoCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            "-s", "3",
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitInfoOptions.ShowIconSlotFileNames.Should().Be(3);
    }

    [TestMethod]
    public async Task PortraitInfoCommand_WithTextureSheetImage_ExecutesSuccessfully()
    {
        // arrange
        PortraitInfoOptions portraitInfoOptions = new();
        _options.Value.Returns(portraitInfoOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitInfoCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            "-i", "texturesheet_image",
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitInfoOptions.TextureSheetImageName.Should().Be("texturesheet_image");
    }

    private ServiceCollection GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton(_logger);
        services.AddSingleton(_options);
        services.AddSingleton(_portraitInfoService);

        return services;
    }

    private void AssertCommandSuccessful(CommandAppResult result)
    {
        result.ExitCode.Should().Be(0);
        _portraitInfoService.Received(1).DisplayInfo();
    }
}