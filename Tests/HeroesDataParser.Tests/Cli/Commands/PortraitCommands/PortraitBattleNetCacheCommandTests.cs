using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

namespace HeroesDataParser.Cli.Commands.PortraitCommands.Tests;

[TestClass]
public class PortraitBattleNetCacheCommandTests
{
    private readonly ILogger<PortraitBattleNetCacheCommand> _logger;
    private readonly IOptions<PortraitBattleNetCacheOptions> _options;
    private readonly IPortraitBattleNetCacheService _portraitBattleNetCacheService;

    public PortraitBattleNetCacheCommandTests()
    {
        _logger = Substitute.For<ILogger<PortraitBattleNetCacheCommand>>();
        _options = Substitute.For<IOptions<PortraitBattleNetCacheOptions>>();
        _portraitBattleNetCacheService = Substitute.For<IPortraitBattleNetCacheService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void PortraitBattleNetCacheCommand_CacheDirectoryDoesNotExist_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitBattleNetCacheCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "-c", "nonexistent-directory",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("does not exist");
    }

    [TestMethod]
    public void PortraitBattleNetCacheCommand_CacheDirectoryIsAnExistingFile_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitBattleNetCacheCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "-c", Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("is an existing file and not a directory");
    }

    [TestMethod]
    public void PortraitBattleNetCacheCommand_OutputDirectoryIsAnExistingFile_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<PortraitBattleNetCacheCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "-o", Path.Combine("TestJsonFiles", "announcerdata_96477_enus.json"),
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("is an existing file and not a directory");
    }

    [TestMethod]
    public async Task PortraitBattleNetCacheCommand_NoCacheDirectory_ExecutesSuccessfully()
    {
        // arrange
        PortraitBattleNetCacheOptions portraitBattleNetCacheOptions = new();
        _options.Value.Returns(portraitBattleNetCacheOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitBattleNetCacheCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [],
        TestContext.CancellationToken);

        // assert
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            result.ExitCode.Should().Be(1);
            result.Output.Should().Contain("Could not find");
        }
    }

    [TestMethod]
    public async Task PortraitBattleNetCacheCommand_WithCacheDirectory_ExecutesSuccessfully()
    {
        // arrange
        PortraitBattleNetCacheOptions portraitBattleNetCacheOptions = new();
        _options.Value.Returns(portraitBattleNetCacheOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitBattleNetCacheCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "-c", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitBattleNetCacheOptions.BattleNetCacheDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
        portraitBattleNetCacheOptions.OutputDirectory.Should().Be(Path.Combine(Path.GetFullPath("."), "battle.net-cache"));
    }

    [TestMethod]
    public async Task PortraitBattleNetCacheCommand_WithCacheAndOutputDirectory_ExecutesSuccessfully()
    {
        // arrange
        PortraitBattleNetCacheOptions portraitBattleNetCacheOptions = new();
        _options.Value.Returns(portraitBattleNetCacheOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<PortraitBattleNetCacheCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "-c", "TestXmlFiles",
            "-o", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        AssertCommandSuccessful(result);

        portraitBattleNetCacheOptions.BattleNetCacheDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
        portraitBattleNetCacheOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
    }

    private ServiceCollection GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton(_logger);
        services.AddSingleton(_options);
        services.AddSingleton(_portraitBattleNetCacheService);

        return services;
    }

    private void AssertCommandSuccessful(CommandAppResult result)
    {
        result.ExitCode.Should().Be(0);
        _portraitBattleNetCacheService.Received(1).CopyWaflFiles();
    }
}