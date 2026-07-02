using Microsoft.Extensions.DependencyInjection;

namespace HeroesDataParser.Cli.Commands.PortraitCommands.Tests;

[TestClass]
public class PortraitExtractCommandTests
{
    private readonly ILogger<PortraitExtractCommand> _logger;
    private readonly IOptions<PortraitExtractOptions> _options;
    private readonly IPortraitExtractService _portraitExtractService;

    public PortraitExtractCommandTests()
    {
        _logger = Substitute.For<ILogger<PortraitExtractCommand>>();
        _options = Substitute.For<IOptions<PortraitExtractOptions>>();
        _portraitExtractService = Substitute.For<IPortraitExtractService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void PortraitExtractCommand_FilePathDoesNotExist_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitExtractCommand>();

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
    public void PortraitExtractCommand_CacheTextureSheetImageDoesNotExist_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitExtractCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
            "-c", "nonexistent.png",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("--cache-texture-sheet-image file does not exist");
    }

    [TestMethod]
    public void PortraitExtractCommand_OutputDirectoryIsAnExistingFile_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitExtractCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            "-o", Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("is an existing file and not a directory");
    }

    [TestMethod]
    public async Task PortraitExtractCommand_WithAllOptions_ExecutesSuccessfully()
    {
        // arrange
        PortraitExtractOptions portraitExtractOptions = new();
        _options.Value.Returns(portraitExtractOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitExtractCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
            "-i", "texturesheet_image.png",
            "-c", Path.Combine("TestImages", "PortraitsCache", "c6c454cfd845c7367d09eb26b9273feb76fefc7344eecadb6932e35ceabba46f.dds"),
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitExtractOptions.RewardPortraitDataFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json")));
        portraitExtractOptions.RewardPortraitTextureSheetImage.Should().Be("texturesheet_image.png");
        portraitExtractOptions.CacheTextureSheetImageFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestImages", "PortraitsCache", "c6c454cfd845c7367d09eb26b9273feb76fefc7344eecadb6932e35ceabba46f.dds")));
        portraitExtractOptions.OutputDirectory.Should().Be(Path.Combine(Path.GetFullPath("."), Constants.ImagesDirectory, Constants.PortraitRewardsDirectory));
        portraitExtractOptions.DeleteTextureSheet.Should().BeFalse();
    }

    [TestMethod]
    public async Task PortraitExtractCommand_WithOutputDirectory_ExecutesSuccessfully()
    {
        // arrange
        PortraitExtractOptions portraitExtractOptions = new();
        _options.Value.Returns(portraitExtractOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitExtractCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
            "-i", "texturesheet_image",
            "-c", Path.Combine("TestImages", "PortraitsCache", "c6c454cfd845c7367d09eb26b9273feb76fefc7344eecadb6932e35ceabba46f.dds"),
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitExtractOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
    }

    [TestMethod]
    public async Task PortraitExtractCommand_WithDeleteTextureSheet_ExecutesSuccessfully()
    {
        // arrange
        PortraitExtractOptions portraitExtractOptions = new();
        _options.Value.Returns(portraitExtractOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitExtractCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
            "-i", "texturesheet_image",
            "-c", Path.Combine("TestImages", "PortraitsCache", "c6c454cfd845c7367d09eb26b9273feb76fefc7344eecadb6932e35ceabba46f.dds"),
            "--delete-texture-sheet",
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitExtractOptions.DeleteTextureSheet.Should().BeTrue();
    }

    [TestMethod]
    public async Task PortraitExtractCommand_Interactive_ExecutesSuccessfully()
    {
        // arrange
        PortraitExtractOptions portraitExtractOptions = new();
        _options.Value.Returns(portraitExtractOptions);

        TestConsole console = new();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("\"ui_heroes_portraits_sheet5.png\"");
        console.Input.PushTextWithEnter($"\"{Path.Combine("TestImages", "PortraitsCache", "c6c454cfd845c7367d09eb26b9273feb76fefc7344eecadb6932e35ceabba46f.dds")}\"");

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar, console: console);
        app.SetDefaultCommand<PortraitExtractCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitExtractOptions.RewardPortraitDataFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json")));
        portraitExtractOptions.RewardPortraitTextureSheetImage.Should().Be("ui_heroes_portraits_sheet5.png");
        portraitExtractOptions.CacheTextureSheetImageFilePath.Should().Be(Path.Combine("TestImages", "PortraitsCache", "c6c454cfd845c7367d09eb26b9273feb76fefc7344eecadb6932e35ceabba46f.dds"));
        portraitExtractOptions.OutputDirectory.Should().Be(Path.Combine(Path.GetFullPath("."), Constants.ImagesDirectory, Constants.PortraitRewardsDirectory));
        portraitExtractOptions.DeleteTextureSheet.Should().BeFalse();
    }

    [TestMethod]
    public async Task PortraitExtractCommand_InteractiveCacheTextureSheetDoesNotExist_ExecutesSuccessfully()
    {
        // arrange
        PortraitExtractOptions portraitExtractOptions = new();
        _options.Value.Returns(portraitExtractOptions);

        TestConsole console = new();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("\"ui_heroes_portraits_sheet5.png\"");
        console.Input.PushTextWithEnter($"\"{Path.Combine("TestImages", "PortraitsCache", "doesnotexists.dds")}\"");
        console.Input.PushTextWithEnter($"'{Path.Combine("TestImages", "PortraitsCache", "c6c454cfd845c7367d09eb26b9273feb76fefc7344eecadb6932e35ceabba46f.dds")}'");

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar, console: console);
        app.SetDefaultCommand<PortraitExtractCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitExtractOptions.RewardPortraitDataFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json")));
        portraitExtractOptions.RewardPortraitTextureSheetImage.Should().Be("ui_heroes_portraits_sheet5.png");
        portraitExtractOptions.CacheTextureSheetImageFilePath.Should().Be(Path.Combine("TestImages", "PortraitsCache", "c6c454cfd845c7367d09eb26b9273feb76fefc7344eecadb6932e35ceabba46f.dds"));
        portraitExtractOptions.OutputDirectory.Should().Be(Path.Combine(Path.GetFullPath("."), Constants.ImagesDirectory, Constants.PortraitRewardsDirectory));
        portraitExtractOptions.DeleteTextureSheet.Should().BeFalse();
    }

    private ServiceCollection GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton(_logger);
        services.AddSingleton(_options);
        services.AddSingleton(_portraitExtractService);

        return services;
    }

    private void AssertCommandSuccessful(CommandAppResult result)
    {
        result.ExitCode.Should().Be(0);
        _portraitExtractService.Received(1).Extract();
    }
}