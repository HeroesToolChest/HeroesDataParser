using HeroesDataParser.Infrastructure;
using Spectre.Console;

namespace HeroesDataParser.Tests.Infrastructure;

[TestClass]
public class ConfigurationLoaderServiceTests
{
    private readonly ILogger<ConfigurationLoaderService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IAnsiConsole _console;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IParsingConfigurationService _parsingConfigurationService;
    private readonly ICustomConfigurationService _customConfigurationService;

    public ConfigurationLoaderServiceTests()
    {
        _logger = Substitute.For<ILogger<ConfigurationLoaderService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _console = Substitute.For<IAnsiConsole>();
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _parsingConfigurationService = Substitute.For<IParsingConfigurationService>();
        _customConfigurationService = Substitute.For<ICustomConfigurationService>();
    }

    [TestMethod]
    public void LoadConfiguration_ExtractOptions_SetsExtractDataOptions()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            StorageLoad = new StorageLoadOptions()
            {
                Type = StorageType.Mods,
                Path = "somepath",
            },
            Extractors =
            {
                { ExtractDataOptions.Announcer, new ExtractorOptions() { IsEnabled = false, Images = false } },
                { ExtractDataOptions.VoiceLine, new ExtractorOptions() { IsEnabled = true, Images = true } },
                { ExtractDataOptions.Boost, new ExtractorOptions() { IsEnabled = true, Images = false } },
                { ExtractDataOptions.Bundle, new ExtractorOptions() { IsEnabled = false, Images = true } },
                { ExtractDataOptions.Hero, new ExtractorOptions() { IsEnabled = true, Images = false } },
                { ExtractDataOptions.Map, new ExtractorOptions() { IsEnabled = true, Images = false } },
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

        ConfigurationLoaderService configurationLoaderService = new(
            _logger,
            _options,
            _console,
            _httpClientFactory,
            _parsingConfigurationService,
            _customConfigurationService);

        // act
        _ = configurationLoaderService.LoadConfiguration();

        // assert
        _options.Value.ExtractDataOptions.Should().Be(ExtractDataOptions.VoiceLine | ExtractDataOptions.Boost | ExtractDataOptions.Hero | ExtractDataOptions.Map);
        _options.Value.ExtractImageOptions.Should().Be(ExtractImageOptions.VoiceLine);
    }

    [TestMethod]
    public void LoadConfiguration_ExtractDataOptionsNoneSet_ExtractDataOptionsShouldBeNone()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            StorageLoad = new StorageLoadOptions()
            {
                Type = StorageType.Mods,
                Path = "somepath",
            },
            Extractors =
            {
                { ExtractDataOptions.Announcer, new ExtractorOptions() { IsEnabled = false, Images = false } },
            },
        });

        ConfigurationLoaderService configurationLoaderService = new(
            _logger,
            _options,
            _console,
            _httpClientFactory,
            _parsingConfigurationService,
            _customConfigurationService);

        // act
        _ = configurationLoaderService.LoadConfiguration();

        // assert
        _options.Value.ExtractDataOptions.Should().Be(ExtractDataOptions.None);
        _options.Value.ExtractImageOptions.Should().Be(ExtractImageOptions.None);
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
    public void LoadConfiguration_ExtractImageOptionsHeroSettings_SetsExtractImageOptionsForHero(bool heroPortraits, bool talents, bool abilities, bool abilityTalents, bool heroData, bool heroDataSplit, ExtractImageOptions resultOptions)
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            StorageLoad = new StorageLoadOptions()
            {
                Type = StorageType.Mods,
                Path = "somepath",
            },
            Extractors =
            {
                { ExtractDataOptions.Hero, new ExtractorOptions() { IsEnabled = true, Images = true } },
            },
            Hidden = new HiddenOptions()
            {
                Abilities = abilities,
                AbilityTalents = abilityTalents,
                HeroImages = new HeroImagesOptions()
                {
                    HeroPortraits = heroPortraits,
                    Talents = talents,

                    HeroData = heroData,
                    HeroDataSplit = heroDataSplit,
                },
            },
        });

        ConfigurationLoaderService configurationLoaderService = new(
            _logger,
            _options,
            _console,
            _httpClientFactory,
            _parsingConfigurationService,
            _customConfigurationService);

        // act
        _ = configurationLoaderService.LoadConfiguration();

        // assert
        _options.Value.ExtractImageOptions.Should().Be(resultOptions);
    }

    [TestMethod]
    [DataRow(true, false, false, ExtractImageOptions.ReplayPreview)]
    [DataRow(false, true, false, ExtractImageOptions.LoadingScreen)]
    [DataRow(false, false, true, ExtractImageOptions.MapObjectives)]
    [DataRow(true, true, true, ExtractImageOptions.ReplayPreview | ExtractImageOptions.LoadingScreen | ExtractImageOptions.MapObjectives)]
    [DataRow(false, false, false, ExtractImageOptions.None)]
    public void LoadConfiguration_ExtractImageOptionsMapSettings_SetsExtractImageOptionsForMap(bool replayPreviews, bool loadingScreens, bool mapObjectiveIcons, ExtractImageOptions resultOptions)
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            StorageLoad = new StorageLoadOptions()
            {
                Type = StorageType.Mods,
                Path = "somepath",
            },
            Extractors =
            {
                { ExtractDataOptions.Map, new ExtractorOptions() { IsEnabled = true, Images = true } },
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

        ConfigurationLoaderService configurationLoaderService = new(
            _logger,
            _options,
            _console,
            _httpClientFactory,
            _parsingConfigurationService,
            _customConfigurationService);

        // act
        _ = configurationLoaderService.LoadConfiguration();

        // assert
        _options.Value.ExtractImageOptions.Should().Be(resultOptions);
    }
}
