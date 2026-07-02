using Spectre.Console;

namespace HeroesDataParser.Infrastructure.Tests;

[TestClass]
public class MainServiceTests
{
    private readonly ILogger<MainService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IAnsiConsole _console;
    private readonly IMainLocalePreProcessor _mainLocalePreProcess;
    private readonly IMainLocaleProcessor _mainLocaleProcess;
    private readonly IImageWriterService _imageWriterService;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly RootOptions _rootOptions;

    public MainServiceTests()
    {
        _logger = Substitute.For<ILogger<MainService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _console = new TestConsole();
        _mainLocalePreProcess = Substitute.For<IMainLocalePreProcessor>();
        _mainLocaleProcess = Substitute.For<IMainLocaleProcessor>();
        _imageWriterService = Substitute.For<IImageWriterService>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _rootOptions = new RootOptions();
        _options.Value.Returns(_rootOptions);

        HeroesXmlLoader heroesXmlLoader = HeroesXmlLoader.LoadWithEmpty();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(heroesXmlLoader);
    }

    [TestMethod]
    public async Task Start_NoLocalizations_ShouldOnlyWriteImages()
    {
        // arrange
        _rootOptions.Localizations = [];
        MainService service = CreateService();

        // act
        await service.Start();

        // assert
        _mainLocalePreProcess.DidNotReceive().Run();
        await _mainLocaleProcess.DidNotReceive().Run();
        await _imageWriterService.Received(1).Write();
    }

    [TestMethod]
    public async Task Start_SingleLocale_ShouldProcessOnce()
    {
        // arrange
        _rootOptions.Localizations = [StormLocale.ENUS];
        MainService service = CreateService();

        // act
        await service.Start();

        // assert
        _mainLocalePreProcess.Received(1).Run();
        await _mainLocaleProcess.Received(1).Run();
        await _imageWriterService.Received(1).Write();
        _rootOptions.CurrentLocale.Should().Be(StormLocale.ENUS);
    }

    [TestMethod]
    public async Task Start_MultipleLocales_ShouldProcessEach()
    {
        // arrange
        _rootOptions.Localizations = [StormLocale.ENUS, StormLocale.DEDE, StormLocale.KOKR];
        MainService service = CreateService();

        // act
        await service.Start();

        // assert
        _mainLocalePreProcess.Received(3).Run();
        await _mainLocaleProcess.Received(3).Run();
        await _imageWriterService.Received(1).Write();
    }

    [TestMethod]
    public async Task Start_LocalizedTextExtract_ShouldSetIsLocalizedExtractFirstRunThenClear()
    {
        // arrange
        _rootOptions.LocalizedText = LocalizedTextOption.Extract;
        _rootOptions.Localizations = [StormLocale.ENUS, StormLocale.DEDE];
        MainService service = CreateService();

        // act
        await service.Start();

        // assert
        _rootOptions.IsLocalizedExtractFirstRun.Should().BeFalse();
    }

    [TestMethod]
    public async Task Start_LocalizedTextNotExtract_ShouldNotSetFirstRun()
    {
        // arrange
        _rootOptions.LocalizedText = LocalizedTextOption.None;
        _rootOptions.IsLocalizedExtractFirstRun = false;
        _rootOptions.Localizations = [StormLocale.ENUS];
        MainService service = CreateService();

        // act
        await service.Start();

        // assert
        _rootOptions.IsLocalizedExtractFirstRun.Should().BeFalse();
    }

    [TestMethod]
    public async Task Start_MultipleLocales_CurrentLocaleShouldBeLastLocale()
    {
        // arrange
        _rootOptions.Localizations = [StormLocale.ENUS, StormLocale.DEDE, StormLocale.KOKR];
        MainService service = CreateService();

        // act
        await service.Start();

        // assert
        _rootOptions.CurrentLocale.Should().Be(StormLocale.KOKR);
    }

    private MainService CreateService() => new(
        _logger,
        _options,
        _console,
        _mainLocalePreProcess,
        _mainLocaleProcess,
        _imageWriterService,
        _heroesXmlLoaderService);
}