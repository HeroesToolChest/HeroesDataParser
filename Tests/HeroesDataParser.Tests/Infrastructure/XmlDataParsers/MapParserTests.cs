namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class MapParserTests
{
    private readonly ILogger<MapParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public MapParserTests()
    {
        _logger = Substitute.For<ILogger<MapParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_AlteracPass_ReturnsMapData()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            CurrentLocale = StormLocale.ENUS,
        };

        _options.Value.Returns(rootOptions);

        _gameStringTextService.GetGameStringTextFromId("UI/MapLoadingScreen/AlteracValley/Title1").Returns(new GameStringText("Cavalry"));
        _gameStringTextService.GetGameStringTextFromId("UI/MapLoadingScreen/AlteracValley/Title2").Returns(new GameStringText("Cavalry2"));
        _gameStringTextService.GetGameStringTextFromId("UI/MapLoadingScreen/AlteracValley/Title3").Returns(new GameStringText("Cavalry3"));
        _gameStringTextService.GetGameStringTextFromId("UI/MapLoadingScreen/AlteracValley/Description1").Returns(new GameStringText("Capture the enemy Prison Camp..."));
        _gameStringTextService.GetGameStringTextFromId("UI/MapLoadingScreen/AlteracValley/Description2").Returns(new GameStringText("Capture the enemy Prison Camp2..."));
        _gameStringTextService.GetGameStringTextFromId("UI/MapLoadingScreen/AlteracValley/Description3").Returns(new GameStringText("Capture the enemy Prison Camp3..."));

        MapParser mapParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Map? map = mapParser.Parse("Alterac Pass");

        // assert
        map.Should().NotBeNull();
        map.Id.Should().Be("Alterac Pass");
        map.LoadingScreenImage.Should().Be("storm_ui_homescreenbackground_wcav.png");
        map.ReplayPreviewImage.Should().Be("test-image2.png");
        map.MapId.Should().Be("AlteracPass");
        map.MapLink.Should().Be("AlteracPass");
        map.Name!.RawText.Should().Be("Alterac Pass");
        map.MapSize!.Value.X.Should().Be(256);
        map.MapSize.Value.Y.Should().Be(216);
        map.MapObjectives.Should().HaveCount(3);
        map.MapObjectives[0].Title!.RawText.Should().Be("Cavalry");
        map.MapObjectives[0].Description!.RawText.Should().Be("Capture the enemy Prison Camp...");
        map.MapObjectives[0].Icons!.Should().HaveCount(2);
        map.MapObjectives[0].Icons![0].Height.Should().BeNull();
        map.MapObjectives[0].Icons![0].Image.Should().Be("ui_ingame_mapmechanic_loadscreen_WCAV_icon1_horde.png");
        map.MapObjectives[0].Icons![0].ScaleWidth.Should().BeFalse();
        map.MapObjectives[0].Icons![1].Height.Should().BeNull();
        map.MapObjectives[0].Icons![1].Image.Should().Be("ui_ingame_mapmechanic_loadscreen_WCAV_icon1_alliance.png");
        map.MapObjectives[0].Icons![1].ScaleWidth.Should().BeFalse();

        map.MapObjectives[1].Title!.RawText.Should().Be("Cavalry2");
        map.MapObjectives[1].Description!.RawText.Should().Be("Capture the enemy Prison Camp2...");
        map.MapObjectives[1].Icons!.Should().HaveCount(2);
        map.MapObjectives[1].Icons![0].Height.Should().BeNull();
        map.MapObjectives[1].Icons![0].Image.Should().Be("ui_ingame_mapmechanic_loadscreen_WCAV_icon2_alliance.png");
        map.MapObjectives[1].Icons![0].ScaleWidth.Should().BeFalse();
        map.MapObjectives[1].Icons![1].Height.Should().BeNull();
        map.MapObjectives[1].Icons![1].Image.Should().Be("ui_ingame_mapmechanic_loadscreen_WCAV_icon2_horde.png");
        map.MapObjectives[1].Icons![1].ScaleWidth.Should().BeFalse();

        map.MapObjectives[2].Title!.RawText.Should().Be("Cavalry3");
        map.MapObjectives[2].Description!.RawText.Should().Be("Capture the enemy Prison Camp3...");
        map.MapObjectives[2].Icons!.Should().HaveCount(2);
        map.MapObjectives[2].Icons![0].Height.Should().BeNull();
        map.MapObjectives[2].Icons![0].Image.Should().Be("ui_ingame_mapmechanic_loadscreen_WCAV_icon3_alliance.png");
        map.MapObjectives[2].Icons![0].ScaleWidth.Should().BeFalse();
        map.MapObjectives[2].Icons![1].Height.Should().BeNull();
        map.MapObjectives[2].Icons![1].Image.Should().Be("ui_ingame_mapmechanic_loadscreen_WCAV_icon3_horde.png");
        map.MapObjectives[2].Icons![1].ScaleWidth.Should().BeFalse();
    }

    [TestMethod]
    public void Parse_BattlefieldOfEternity_ReturnsMapData()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            CurrentLocale = StormLocale.DEDE,
        };

        _options.Value.Returns(rootOptions);

        MapParser mapParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Map? map = mapParser.Parse("Battlefield of Eternity");

        // assert
        map.Should().NotBeNull();
        map.Id.Should().Be("Battlefield of Eternity");
        map.MapId.Should().Be("BattlefieldOfEternity");
        map.MapLink.Should().Be("BoE");
        map.Name!.RawText.Should().Be("Schlachtfeld der Ewigkeit");
        map.MapSize!.Value.X.Should().Be(248);
        map.MapSize.Value.Y.Should().Be(208);
        map.MapObjectives.Should().HaveCount(3);
        map.MapObjectives[0].Icons!.Should().ContainSingle();
        map.MapObjectives[0].Icons![0].Height.Should().Be(165);
        map.MapObjectives[0].Icons![0].ScaleWidth.Should().BeTrue();
    }

    [TestMethod]
    public void Parse_NotFound_ReturnsNull()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            CurrentLocale = StormLocale.ENUS,
        };

        _options.Value.Returns(rootOptions);

        MapParser mapParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Map? map = mapParser.Parse("NotFound");

        // assert
        map.Should().BeNull();
    }

    [TestMethod]
    public void Parse_ImageTesting_ReturnsMapData()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            CurrentLocale = StormLocale.ENUS,
        };

        _options.Value.Returns(rootOptions);

        MapParser mapParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Map? map = mapParser.Parse("Test1");

        // assert
        map.Should().NotBeNull();
        map.LoadingScreenImage.Should().Be("storm_ui_homescreenbackground.png");
        map.ReplayPreviewImage.Should().BeNull();
    }

    [TestMethod]
    public void Parse_MpqReplayPreviewInBaseStormAssets_ReturnsMapData()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            CurrentLocale = StormLocale.ENUS,
        };

        _options.Value.Returns(rootOptions);

        MapParser mapParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Map? map = mapParser.Parse("MpqTest1");

        // assert
        map.Should().NotBeNull();
        map.ReplayPreviewImage.Should().Be("ReplaysPreviewImage_mpqtest1.png");
    }
}
