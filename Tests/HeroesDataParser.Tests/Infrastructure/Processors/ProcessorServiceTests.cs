namespace HeroesDataParser.Infrastructure.Processors.Tests;

[TestClass]
public class ProcessorServiceTests
{
    private readonly ILogger<ProcessorService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDataExtractorService _dataExtractorService;
    private readonly IJsonDataFileWriterService _jsonDataFileWriterService;
    private readonly IJsonGameStringFileWriterService _jsonGameStringFileWriterService;
    private readonly IImageWriterService _imageWriterService;

    public ProcessorServiceTests()
    {
        _logger = Substitute.For<ILogger<ProcessorService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _dataExtractorService = Substitute.For<IDataExtractorService>();
        _jsonDataFileWriterService = Substitute.For<IJsonDataFileWriterService>();
        _jsonGameStringFileWriterService = Substitute.For<IJsonGameStringFileWriterService>();
        _imageWriterService = Substitute.For<IImageWriterService>();
    }

    [TestMethod]
    public void ProcessorServiceConstructor_ExtractOptions_SetsExtractOptions()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            Extractors =
            {
                { "Announcer", new ExtractorOptions() { IsEnabled = false, Images = false } },
                { "VoiceLine", new ExtractorOptions() { IsEnabled = true, Images = true } },
                { "Boost", new ExtractorOptions() { IsEnabled = true, Images = false } },
                { "Bundle", new ExtractorOptions() { IsEnabled = false, Images = true } },
                { "hero", new ExtractorOptions() { IsEnabled = true, Images = false } },
                { "map", new ExtractorOptions() { IsEnabled = true, Images = false } },
                { "aba", new ExtractorOptions() { IsEnabled = true, Images = true } },
            },
            Hidden = new HiddenOptions()
            {
                HeroImages = new HeroImagesOptions()
                {
                    HeroData = true,
                },
                MapImages = new MapImagesOptions()
                {
                    ReplayPreviews = true,
                },
            },
        });

        // act
        ProcessorService processorService = new(_logger, _options, _serviceProvider, _dataExtractorService, _jsonDataFileWriterService, _jsonGameStringFileWriterService, _imageWriterService);

        // assert
        processorService.ExtractDataOptions.Should().Be(ExtractDataOptions.VoiceLine | ExtractDataOptions.Boost | ExtractDataOptions.Hero | ExtractDataOptions.Map);
        processorService.ExtractImageOptions.Should().Be(ExtractImageOptions.VoiceLine);
    }

    [TestMethod]
    public void ProcessorServiceConstructor_ExtractDataOptionsNoneSet_SetsExtractOptions()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            Extractors =
            {
                { "Announcer", new ExtractorOptions() { IsEnabled = false, Images = false } },
            },
        });

        // act
        ProcessorService processorService = new(_logger, _options, _serviceProvider, _dataExtractorService, _jsonDataFileWriterService, _jsonGameStringFileWriterService, _imageWriterService);

        // assert
        processorService.ExtractDataOptions.Should().Be(ExtractDataOptions.None);
        processorService.ExtractImageOptions.Should().Be(ExtractImageOptions.None);
    }

    [TestMethod]
    [DataRow(true, false, false, false, false, false, ExtractImageOptions.HeroPortrait)]
    [DataRow(false, true, false, false, false, false, ExtractImageOptions.Talent)]
    [DataRow(false, false, true, false, false, false, ExtractImageOptions.Ability)]
    [DataRow(false, false, false, true, false, false, ExtractImageOptions.AbilityTalent)]
    [DataRow(false, false, false, false, true, false, ExtractImageOptions.HeroData)]
    [DataRow(false, false, false, false, false, true, ExtractImageOptions.HeroDataSplit)]
    [DataRow(true, false, false, true, false, false, ExtractImageOptions.HeroPortrait | ExtractImageOptions.AbilityTalent)]
    [DataRow(false, true, true, false, false, false, ExtractImageOptions.Talent | ExtractImageOptions.Ability)]
    [DataRow(false, false, false, false, false, false, ExtractImageOptions.None)]
    public void ProcessorServiceConstructor_ExtractImageOptionsHeroSettings_SetsExtractImageOptionsForHero(bool heroPortraits, bool talents, bool abilities, bool abilityTalents, bool heroData, bool heroDataSplit, ExtractImageOptions resultOptions)
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            Extractors =
            {
                { "hero", new ExtractorOptions() { IsEnabled = true, Images = true } },
            },
            Hidden = new HiddenOptions()
            {
                HeroImages = new HeroImagesOptions()
                {
                    HeroPortraits = heroPortraits,
                    Talents = talents,
                    Abilities = abilities,
                    AbilityTalents = abilityTalents,
                    HeroData = heroData,
                    HeroDataSplit = heroDataSplit,
                },
            },
        });

        // act
        ProcessorService processorService = new(_logger, _options, _serviceProvider, _dataExtractorService, _jsonDataFileWriterService, _jsonGameStringFileWriterService, _imageWriterService);

        // assert
        processorService.ExtractImageOptions.Should().Be(resultOptions);
    }

    [TestMethod]
    [DataRow(true, false, false, ExtractImageOptions.ReplayPreview)]
    [DataRow(false, true, false, ExtractImageOptions.LoadingScreen)]
    [DataRow(false, false, true, ExtractImageOptions.MapObjectives)]
    [DataRow(true, true, true, ExtractImageOptions.ReplayPreview | ExtractImageOptions.LoadingScreen | ExtractImageOptions.MapObjectives)]
    [DataRow(false, false, false, ExtractImageOptions.None)]
    public void ProcessorServiceConstructor_ExtractImageOptionsMapSettings_SetsExtractImageOptionsForMap(bool replayPreviews, bool loadingScreens, bool mapObjectiveIcons, ExtractImageOptions resultOptions)
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            Extractors =
            {
                { "map", new ExtractorOptions() { IsEnabled = true, Images = true } },
            },
            Hidden = new HiddenOptions()
            {
                MapImages = new MapImagesOptions()
                {
                    ReplayPreviews = replayPreviews,
                    LoadingScreens = loadingScreens,
                    MapObjectiveIcons = mapObjectiveIcons,
                },
            },
        });

        // act
        ProcessorService processorService = new(_logger, _options, _serviceProvider, _dataExtractorService, _jsonDataFileWriterService, _jsonGameStringFileWriterService, _imageWriterService);

        // assert
        processorService.ExtractImageOptions.Should().Be(resultOptions);
    }
}
