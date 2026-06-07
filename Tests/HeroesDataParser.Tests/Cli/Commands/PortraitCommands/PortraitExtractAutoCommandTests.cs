using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

namespace HeroesDataParser.Cli.Commands.PortraitCommands.Tests;

[TestClass]
public class PortraitExtractAutoCommandTests
{
    private readonly ILogger<PortraitExtractAutoCommand> _logger;
    private readonly IOptions<PortraitExtractAutoOptions> _options;
    private readonly IPortraitExtractAutoService _portraitExtractAutoService;

    public PortraitExtractAutoCommandTests()
    {
        _logger = Substitute.For<ILogger<PortraitExtractAutoCommand>>();
        _options = Substitute.For<IOptions<PortraitExtractAutoOptions>>();
        _portraitExtractAutoService = Substitute.For<IPortraitExtractAutoService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void PortraitExtractAutoCommand_MissingRequiredArgument_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitExtractAutoCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "-c", "TestXmlFiles",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("Missing required argument 'rewardportrait-file-path'");
    }

    [TestMethod]
    public void PortraitExtractAutoCommand_FilePathDoesNotExist_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitExtractAutoCommand>();

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
    public void PortraitExtractAutoCommand_CacheDirectoryDoesNotExist_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitExtractAutoCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            "-c", "nonexistent-directory",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("does not exist");
    }

    [TestMethod]
    public void PortraitExtractAutoCommand_CacheDirectoryIsAnExistingFile_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitExtractAutoCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            "-c", Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("is an existing file and not a directory");
    }

    [TestMethod]
    public void PortraitExtractAutoCommand_XmlConfigFileDoesNotExist_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitExtractAutoCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            "-x", "nonexistent.xml",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("--xml-config file does not exist");
    }

    [TestMethod]
    public void PortraitExtractAutoCommand_OutputDirectoryIsAnExistingFile_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitExtractAutoCommand>();

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
    public async Task PortraitExtractAutoCommand_NoCacheDirectory_ExecutesSuccessfully()
    {
        // arrange
        PortraitExtractAutoOptions portraitExtractAutoOptions = new();
        _options.Value.Returns(portraitExtractAutoOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitExtractAutoCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
        ],
        TestContext.CancellationToken);

        // assert
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            result.ExitCode.Should().Be(1);
            result.Output.Should().Contain("Could not find");
        }
    }

    [TestMethod]
    public async Task PortraitExtractAutoCommand_WithCacheDirectory_ExecutesSuccessfully()
    {
        // arrange
        PortraitExtractAutoOptions portraitExtractAutoOptions = new();
        _options.Value.Returns(portraitExtractAutoOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitExtractAutoCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            "-c", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitExtractAutoOptions.BattleNetCacheDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
        portraitExtractAutoOptions.RewardPortraitDataFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json")));
        portraitExtractAutoOptions.XmlConfigFilePath.Should().Be(Path.Combine(Constants.ConfigFilesDirectory, "portrait-extract.xml"));
        portraitExtractAutoOptions.OutputDirectory.Should().Be(Path.Combine(Path.GetFullPath("."), Constants.ImagesDirectory, Constants.PortraitRewardsDirectory));
        portraitExtractAutoOptions.DeleteTextureSheet.Should().BeFalse();
    }

    [TestMethod]
    public async Task PortraitExtractAutoCommand_WithCacheAndOutputDirectory_ExecutesSuccessfully()
    {
        // arrange
        PortraitExtractAutoOptions portraitExtractAutoOptions = new();
        _options.Value.Returns(portraitExtractAutoOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitExtractAutoCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            "-c", "TestXmlFiles",
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitExtractAutoOptions.BattleNetCacheDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
        portraitExtractAutoOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
    }

    [TestMethod]
    public async Task PortraitExtractAutoCommand_WithXmlConfig_ExecutesSuccessfully()
    {
        // arrange
        PortraitExtractAutoOptions portraitExtractAutoOptions = new();
        _options.Value.Returns(portraitExtractAutoOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitExtractAutoCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            "-c", "TestXmlFiles",
            "-x", Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitExtractAutoOptions.XmlConfigFilePath.Should().Be(Path.GetFullPath(Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json")));
    }

    [TestMethod]
    public async Task PortraitExtractAutoCommand_WithDeleteTextureSheet_ExecutesSuccessfully()
    {
        // arrange
        PortraitExtractAutoOptions portraitExtractAutoOptions = new();
        _options.Value.Returns(portraitExtractAutoOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitExtractAutoCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            Path.Combine("TestJsonFiles", "announcerpackdata_96477_enus.json"),
            "-c", "TestXmlFiles",
            "--delete-texture-sheet",
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitExtractAutoOptions.DeleteTextureSheet.Should().BeTrue();
    }

    private ServiceCollection GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton(_logger);
        services.AddSingleton(_options);
        services.AddSingleton(_portraitExtractAutoService);

        return services;
    }

    private void AssertCommandSuccessful(CommandAppResult result)
    {
        result.ExitCode.Should().Be(0);
        _portraitExtractAutoService.Received(1).Extract();
    }
}