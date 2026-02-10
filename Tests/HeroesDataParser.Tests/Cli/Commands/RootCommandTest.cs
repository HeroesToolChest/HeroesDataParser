using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace HeroesDataParser.Cli.Commands.Tests;

[TestClass]
public class RootCommandTest
{
    private readonly IOptions<RootOptions> _options;
    private readonly IAnsiConsole _console;
    private readonly IPreLoaderService _preLoaderService;
    private readonly IMainService _mainService;
    private readonly IPostCleanupService _postCleanupService;
    private readonly IResultSummaryService _resultSummaryService;

    public RootCommandTest()
    {
        _options = Substitute.For<IOptions<RootOptions>>();
        _console = Substitute.For<IAnsiConsole>();
        _preLoaderService = Substitute.For<IPreLoaderService>();
        _mainService = Substitute.For<IMainService>();
        _postCleanupService = Substitute.For<IPostCleanupService>();
        _resultSummaryService = Substitute.For<IResultSummaryService>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void RootCommand_MissingRequiredArguments_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "--storage-path", "TestXmlFiles",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("Missing required argument 'storage-type'");
    }

    [TestMethod]
    [DataRow("game")]
    [DataRow("mods")]
    public void RootCommand_MissingStoragePathOption_ReturnsError(string storageType)
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            $"{storageType}",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("--storage-path is required");
    }

    [TestMethod]
    public void RootCommand_OnlineWithStoragePath_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "online",
            "--storage-path", "TestXmlFiles",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("must not be specified");
    }

    [TestMethod]
    public void RootCommand_InvalidStoragePath_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "--storage-path", "aaa",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("does not exist");
    }

    [TestMethod]
    public void RootCommand_InvalidOutputPath_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "-p", "aaa",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("does not exist");
    }

    [TestMethod]
    [DataRow("game")]
    [DataRow("mods")]
    public void RootCommand_InvalidWithPtr_ReturnsError(string storageType)
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            $"{storageType}",
            "--storage-path", "TestXmlFiles",
            "--download-ptr",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("only valid when storage-type is 'online'");
    }

    [TestMethod]
    [DataRow("abc")]
    [DataRow("10")]
    [DataRow("4.5.3")]
    public void RootCommand_WithInvalidHeroesVersion_ReturnsError(string heroesVersion)
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--heroes-version", $"{heroesVersion}"
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("not in the correct format");
    }

    [TestMethod]
    public void RootCommand_WithInvalidExtractor_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--extractor", "hero",
            "--extractor", "aaa",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("invalid extractor");
    }

    [TestMethod]
    public void RootCommand_InvalidExtractorWithInvalidImageOption_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--extractor", "hero",
            "--extractor", "unit",
            "--extractor", "announcer:aa",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("invalid extractor option");
    }

    [TestMethod]
    public void RootCommand_InvalidLocale_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--localization", "enus",
            "--localization", "aaa",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("invalid locale");
    }

    [TestMethod]
    public void RootCommand_InvalidGameStringText_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--gamestring-text", "7",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("must be a value less");
    }

    [TestMethod]
    public void RootCommand_InvalidLocalizedText_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--localized-text", "3",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("must be a value less");
    }

    [TestMethod]
    public void RootCommand_InvalidMapJsonOutput_ReturnsError()
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--localized-text", "4",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("must be a value less");
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-2)]
    public void RootCommand_InvalidThreads_ReturnsError(int num)
    {
        // arrange
        CommandAppTester app = new();
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = app.Run(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--threads", $"{num}",
        ]);

        // assert
        result.ExitCode.Should().Be(-1);
        result.Output.Should().Contain("must be -1 or a positive");
    }

    [TestMethod]
    [DataRow("game")]
    [DataRow("mods")]
    public async Task RootCommand_WithDefaultValidArguments_ExecutesSuccessfully(string storageType)
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            $"{storageType}",
            "--storage-path", "TestXmlFiles",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.Extractors.Should().ContainKey(ExtractDataOptions.Hero);
        rootOptions.Extractors[ExtractDataOptions.Hero].IsEnabled.Should().BeTrue();
        rootOptions.Extractors[ExtractDataOptions.Hero].Images.Should().BeFalse();
        rootOptions.Localizations.Should().Contain(StormLocale.ENUS);
        rootOptions.StorageLoad.Ptr.Should().BeFalse();
        rootOptions.StorageLoad.Type.Should().Be(storageType == "game" ? StorageType.Game : StorageType.Mods);
        rootOptions.StorageLoad.Path.Should().EndWith("TestXmlFiles");
        rootOptions.OutputDirectory.Should().Be(".");
        rootOptions.GameStringText.Type.Should().Be(GameStringTextType.RawText);
        rootOptions.GameStringText.ReplaceFontStyles.Should().BeFalse();
        rootOptions.GameStringText.PreserveFont.PreserveFontStyleConstantVars.Should().BeFalse();
        rootOptions.GameStringText.PreserveFont.PreserveFontStyleVars.Should().BeFalse();
        rootOptions.LocalizedText.Should().Be(LocalizedTextOption.None);
        rootOptions.MapSpecificWriterJsonOutputType.Should().Be(MapSpecificWriterJsonOutputType.Diff);
        rootOptions.AllowEmptyMapSpecificDiffFiles.Should().BeFalse();
        rootOptions.AllowEmptyMapSpecificDirectories.Should().BeFalse();
        rootOptions.ShowLoadedCustomConfigFiles.Should().BeFalse();
    }

    [TestMethod]
    public async Task RootCommand_WithOnlineArgument_ExecutesSuccessfully()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "online",
            "--download-ptr",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.Extractors.Should().ContainKey(ExtractDataOptions.Hero);
        rootOptions.Extractors[ExtractDataOptions.Hero].IsEnabled.Should().BeTrue();
        rootOptions.Extractors[ExtractDataOptions.Hero].Images.Should().BeFalse();
        rootOptions.StorageLoad.Ptr.Should().BeTrue();
        rootOptions.StorageLoad.Type.Should().Be(StorageType.Online);
        rootOptions.StorageLoad.Path.Should().BeNull();
        rootOptions.OutputDirectory.Should().Be(".");
    }

    [TestMethod]
    public async Task RootCommand_WithOutputArgument_ExecutesSuccessfully()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "-o", "tests",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.Extractors.Should().ContainKey(ExtractDataOptions.Hero);
        rootOptions.Extractors[ExtractDataOptions.Hero].IsEnabled.Should().BeTrue();
        rootOptions.Extractors[ExtractDataOptions.Hero].Images.Should().BeFalse();
        rootOptions.StorageLoad.Ptr.Should().BeFalse();
        rootOptions.StorageLoad.Type.Should().Be(StorageType.Game);
        rootOptions.StorageLoad.Path.Should().EndWith("TestXmlFiles");
        rootOptions.OutputDirectory.Should().EndWith("tests");
    }

    [TestMethod]
    public async Task RootCommand_WithHeroesVersionArgument_ExecutesSuccessfully()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--heroes-version", "1.2.3.4_ptr",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.Extractors.Should().ContainKey(ExtractDataOptions.Hero);
        rootOptions.Extractors[ExtractDataOptions.Hero].IsEnabled.Should().BeTrue();
        rootOptions.Extractors[ExtractDataOptions.Hero].Images.Should().BeFalse();
        rootOptions.StorageLoad.Ptr.Should().BeFalse();
        rootOptions.StorageLoad.Type.Should().Be(StorageType.Game);
        rootOptions.StorageLoad.Path.Should().EndWith("TestXmlFiles");
        rootOptions.HeroesVersion.Major.Should().Be(1);
        rootOptions.HeroesVersion.Minor.Should().Be(2);
        rootOptions.HeroesVersion.Revision.Should().Be(3);
        rootOptions.HeroesVersion.Build.Should().Be(4);
        rootOptions.HeroesVersion.IsPtr.Should().BeTrue();
    }

    [TestMethod]
    public async Task RootCommand_WithExtractorArguments_ExecutesSuccessfully()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "-e", "unit",
            "-e", "announcer:images",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.Extractors.Should().ContainKeys(ExtractDataOptions.Unit, ExtractDataOptions.Announcer);
        rootOptions.Extractors[ExtractDataOptions.Unit].IsEnabled.Should().BeTrue();
        rootOptions.Extractors[ExtractDataOptions.Unit].Images.Should().BeFalse();
        rootOptions.Extractors[ExtractDataOptions.Announcer].IsEnabled.Should().BeTrue();
        rootOptions.Extractors[ExtractDataOptions.Announcer].Images.Should().BeTrue();
    }

    [TestMethod]
    public async Task RootCommand_WithExtractorArgumentAsAll_ExecutesSuccessfully()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "-e", "all:i",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        foreach (ExtractDataOptions option in Enum.GetValues<ExtractDataOptions>())
        {
            if (option == ExtractDataOptions.None)
                continue;

            rootOptions.Extractors[option].IsEnabled.Should().BeTrue();
            rootOptions.Extractors[option].Images.Should().BeTrue();
        }
    }

    [TestMethod]
    public async Task RootCommand_WithLocalizationArguments_ExecutesSuccessfully()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "-l", "frfr",
            "-l", "dede",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.Localizations.Should().Contain(StormLocale.FRFR);
        rootOptions.Localizations.Should().Contain(StormLocale.DEDE);
    }

    [TestMethod]
    public async Task RootCommand_WithGameStringTextArguments_ExecutesSuccessfully()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "-g", "PlainTextWithScalingWithNewlines",
            "--gamestring-replace-font-vars",
            "--gamestring-preserve-constant-vars",
            "--gamestring-preserve-style-vars",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.GameStringText.Type.Should().Be(GameStringTextType.PlainTextWithScalingWithNewlines);
        rootOptions.GameStringText.ReplaceFontStyles.Should().BeTrue();
        rootOptions.GameStringText.PreserveFont.PreserveFontStyleConstantVars.Should().BeTrue();
        rootOptions.GameStringText.PreserveFont.PreserveFontStyleVars.Should().BeTrue();
    }

    [TestMethod]
    public async Task RootCommand_OnlyPreserveConstantVarsArgument_ReplaceFontStylesSetsToTrue()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--gamestring-preserve-constant-vars",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.GameStringText.Type.Should().Be(GameStringTextType.RawText);
        rootOptions.GameStringText.ReplaceFontStyles.Should().BeTrue();
        rootOptions.GameStringText.PreserveFont.PreserveFontStyleConstantVars.Should().BeTrue();
        rootOptions.GameStringText.PreserveFont.PreserveFontStyleVars.Should().BeFalse();
    }

    [TestMethod]
    public async Task RootCommand_OnlyPreserveStyleVarsArgument_ReplaceFontStylesSetsToTrue()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--gamestring-preserve-style-vars",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.GameStringText.Type.Should().Be(GameStringTextType.RawText);
        rootOptions.GameStringText.ReplaceFontStyles.Should().BeTrue();
        rootOptions.GameStringText.PreserveFont.PreserveFontStyleConstantVars.Should().BeFalse();
        rootOptions.GameStringText.PreserveFont.PreserveFontStyleVars.Should().BeTrue();
    }

    [TestMethod]
    public async Task RootCommand_LocalizedTextArgument_ExecutesSuccessfully()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--localized-text", "extract",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.LocalizedText.Should().Be(LocalizedTextOption.Extract);
    }

    [TestMethod]
    public async Task RootCommand_MapSpecificArguments_ExecutesSuccessfully()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--map-specific-json-output", "Normal",
            "--map-specific-empty-diff",
            "--map-specific-empty-directories",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.MapSpecificWriterJsonOutputType.Should().Be(MapSpecificWriterJsonOutputType.Normal);
        rootOptions.AllowEmptyMapSpecificDiffFiles.Should().BeTrue();
        rootOptions.AllowEmptyMapSpecificDirectories.Should().BeTrue();
    }

    [TestMethod]
    public async Task RootCommand_CustomConfigArgument_ExecutesSuccessfully()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "--custom-configs",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.ShowLoadedCustomConfigFiles.Should().BeTrue();
    }

    [TestMethod]
    public async Task RootCommand_ThreadsArgument_ExecutesSuccessfully()
    {
        // arrange
        RootOptions rootOptions = new();
        _options.Value.Returns(rootOptions);

        TypeRegistrar registrar = new(GetServiceCollection());
        CommandAppTester app = new(registrar);
        app.SetDefaultCommand<RootCommand>();

        // act
        CommandAppResult result = await app.RunAsync(
        [
            "game",
            "--storage-path", "TestXmlFiles",
            "-t", "4",
        ],
        TestContext.CancellationToken);

        // assert
        await AssertCommandSuccessful(result);

        rootOptions.Threads.Should().Be(4);
    }

    private ServiceCollection GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton(_options);
        services.AddSingleton(_console);
        services.AddSingleton(_preLoaderService);
        services.AddSingleton(_mainService);
        services.AddSingleton(_postCleanupService);
        services.AddSingleton(_resultSummaryService);

        return services;
    }

    private async Task AssertCommandSuccessful(CommandAppResult result)
    {
        result.ExitCode.Should().Be(0);
        await _preLoaderService.Received(1).Load();
        await _mainService.Received(1).Start();
        _postCleanupService.Received(1).Start();
        _resultSummaryService.Received(1).PrintSummary();
    }
}
