using Microsoft.Extensions.DependencyInjection;

namespace HeroesDataParser.Cli.Commands.Tests;

[TestClass]
public class CASCExtractCommandTests
{
    private readonly ILogger<CASCExtractCommand> _logger;
    private readonly IOptions<CASCExtractOptions> _options;
    private readonly ICASCExtractorService _cascExtractorService;

    public CASCExtractCommandTests()
    {
        _logger = Substitute.For<ILogger<CASCExtractCommand>>();
        _options = Substitute.For<IOptions<CASCExtractOptions>>();
        _cascExtractorService = Substitute.For<ICASCExtractorService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void CASCExtractCommand_InvalidStorageTypeRequiredArgument_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<CASCExtractCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "mods",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("'mods' is not supported");
    }

    [TestMethod]
    public void CASCExtractCommand_MissingRequiredArgument_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<CASCExtractCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "--storage-path", "aaa",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("Missing required argument 'storage-type'");
    }

    [TestMethod]
    public void CASCExtractCommand_MissingStoragePathOption_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<CASCExtractCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("required when storage-type is 'game'");
    }

    [TestMethod]
    public void CASCExtractCommand_InvalidStoragePathOptionForOnline_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<CASCExtractCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "online",
            "--storage-path", "TestXmlFiles"
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("must not be specified when storage-type is 'online'");
    }

    [TestMethod]
    public void CASCExtractCommand_StoragePathDoesNotExists_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<CASCExtractCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "-p", "aaa"
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("does not exist");
    }

    [TestMethod]
    public void CASCExtractCommand_InvalidOptionForStorageType_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<CASCExtractCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "--download-ptr",
            "-p", "TestXmlFiles"
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("only valid when storage-type is 'online'");
    }

    [TestMethod]
    [DataRow("core", "first directory must be 'mods'")]
    [DataRow("mods_234", "first directory must be 'mods'")]
    [DataRow("modsmods", "first directory must be 'mods'")]
    [DataRow("mods.xml", "must be a directory path")]
    public void CASCExtractCommand_InvalidDirectoryOption_ReturnsError(string directory, string errorMessage)
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<CASCExtractCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "-p", "TestXmlFiles",
            "-d", "mods",
            "-d", $"{directory}"
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain(errorMessage);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-2)]
    public void CASCExtractCommand_InvalidThreads_ReturnsError(int num)
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<CASCExtractCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "online",
            "-d", "mods",
            "--threads", $"{num}",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("must be -1 or a positive");
    }

    [TestMethod]
    public void CASCExtractCommand_OutputDirectoryDoesNotExists_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<CASCExtractCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "-p", "TestXmlFiles",
            "-d", "mods",
            "-o", "aaa"
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("--output-path does not exist");
    }

    [TestMethod]
    public async Task CASCExtractCommand_GameStorage_ExecutesSuccessfully()
    {
        // arrange
        CASCExtractOptions cascExtractOptions = new();
        _options.Value.Returns(cascExtractOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<CASCExtractCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "-p", "TestXmlFiles",
            "-d", "mods",
            "-o", "TestXmlFiles"
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        cascExtractOptions.StorageLoad.Type.Should().Be(StorageType.Game);
        cascExtractOptions.StorageLoad.Path.Should().Be(Path.GetFullPath("TestXmlFiles"));
        cascExtractOptions.StorageLoad.Ptr.Should().BeFalse();
        cascExtractOptions.Directories.Should().ContainSingle().And.Contain("mods");
        cascExtractOptions.FileFilters.Should().ContainSingle().And.Contain("*");
        cascExtractOptions.Threads.Should().Be(-1);
        cascExtractOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
    }

    [TestMethod]
    public async Task CASCExtractCommand_OnlineWithPtr_ExecutesSuccessfully()
    {
        // arrange
        CASCExtractOptions cascExtractOptions = new();
        _options.Value.Returns(cascExtractOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<CASCExtractCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "online",
            "--download-ptr",
            "-d", "mods",
            "-o", "TestXmlFiles"
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        cascExtractOptions.StorageLoad.Type.Should().Be(StorageType.Online);
        cascExtractOptions.StorageLoad.Path.Should().BeNull();
        cascExtractOptions.StorageLoad.Ptr.Should().BeTrue();
        cascExtractOptions.Directories.Should().ContainSingle().And.Contain("mods");
        cascExtractOptions.FileFilters.Should().ContainSingle().And.Contain("*");
        cascExtractOptions.Threads.Should().Be(-1);
        cascExtractOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
    }

    [TestMethod]
    public async Task CASCExtractCommand_MultipleDirectories_ExecutesSuccessfully()
    {
        // arrange
        CASCExtractOptions cascExtractOptions = new();
        _options.Value.Returns(cascExtractOptions);

        TypeRegistrar registrar = new(GetServiceCollection());

        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<CASCExtractCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "online",
            "--download-ptr",
            "-d", "mods",
            "-d", $"{Path.Combine("mods", "someother")}",
            "-d", $"{Path.DirectorySeparatorChar}{Path.Combine("mods", "someotherdir")}",
            "-o", "TestXmlFiles"
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        cascExtractOptions.StorageLoad.Type.Should().Be(StorageType.Online);
        cascExtractOptions.StorageLoad.Path.Should().BeNull();
        cascExtractOptions.StorageLoad.Ptr.Should().BeTrue();
        cascExtractOptions.Directories.Should().HaveCount(3).And.Contain("mods", Path.Combine("mods", "someother"), Path.Combine("mods", "someotherdir"));
        cascExtractOptions.FileFilters.Should().ContainSingle().And.Contain("*");
        cascExtractOptions.Threads.Should().Be(-1);
        cascExtractOptions.OutputDirectory.Should().Be(Path.GetFullPath("TestXmlFiles"));
    }

    private ServiceCollection GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton(_logger);
        services.AddSingleton(_options);
        services.AddSingleton(_cascExtractorService);

        return services;
    }

    private async Task AssertCommandSuccessful(CommandAppResult result)
    {
        result.ExitCode.Should().Be(0);
        await _cascExtractorService.Received(1).RootDirectoryExtract();
    }
}
