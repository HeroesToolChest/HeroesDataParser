namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class BundleParserTests
{
    private readonly ILogger<BundleParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public BundleParserTests()
    {
        _logger = Substitute.For<ILogger<BundleParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_BattleBundle_ReturnsBundleData()
    {
        // arrange
        BundleParser bundleParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Bundle? bundle = bundleParser.Parse("BattleBundle");

        // assert
        bundle.Should().NotBeNull();
        bundle.HyperlinkId.Should().Be("BattleBundle");
        bundle.Id.Should().Be("BattleBundle");
        bundle.Franchise.Should().Be(Franchise.Nexus);
        bundle.GoldBonus.Should().Be(2500);
        bundle.BoostBonusId.Should().BeNull();
        bundle.Category.Should().BeNull();
        bundle.Description.Should().BeNull();
        bundle.Event.Should().BeNull();
        bundle.IsDynamicContent.Should().BeFalse();
        bundle.HeroIds.Should().HaveCount(3)
            .And.ContainInOrder("Diablo", "Raynor", "Tyrande");
        bundle.HeroSkinIdsByHeroId.Should().HaveCount(3);
        bundle.HeroSkinIdsByHeroId.Should().ContainKeys("Diablo", "Raynor", "Tyrande");
        bundle.HeroSkinIdsByHeroId["Diablo"].Should().ContainSingle()
            .And.ContainInOrder("DiabloMurkablo");
        bundle.HeroSkinIdsByHeroId["Raynor"].Should().ContainSingle()
            .And.ContainInOrder("RaynorCommander");
        bundle.HeroSkinIdsByHeroId["Tyrande"].Should().ContainSingle()
            .And.ContainInOrder("TyrandeBloodElfBase");
        bundle.MountIds.Should().ContainSingle()
            .And.ContainInOrder("CyberWolfGold");
        bundle.ReleaseDate.Should().Be(new DateOnly(2014, 3, 13));
    }

    [TestMethod]
    public void Parse_AlteredWarBundle_ReturnsBundleData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("Bundle/Name/AlteredWarBundle").Returns(new GameStringText("name"));
        _gameStringTextService.GetGameStringTextFromId("Bundle/SortName/AlteredWarBundle").Returns(new GameStringText("sortname"));

        BundleParser bundleParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Bundle? bundle = bundleParser.Parse("AlteredWarBundle");

        // assert
        bundle.Should().NotBeNull();
        bundle.HyperlinkId.Should().Be("AlteredWarBundle");
        bundle.Id.Should().Be("AlteredWarBundle");
        bundle.Franchise.Should().Be(Franchise.Nexus);
        bundle.GoldBonus.Should().BeNull();
        bundle.BoostBonusId.Should().Be("7Day");
        bundle.Category.Should().BeNull();
        bundle.Description.Should().BeNull();
        bundle.Event.Should().BeNull();
        bundle.IsDynamicContent.Should().BeTrue();
        bundle.HeroIds.Should().BeEmpty();
        bundle.HeroSkinIdsByHeroId.Should().BeEmpty();
        bundle.MountIds.Should().BeEmpty();
        bundle.ReleaseDate.Should().Be(new DateOnly(2016, 11, 22));
        bundle.Image.Should().Be("storm_ui_bundles_h22_alteredwar.png");
        bundle.Name!.RawText.Should().Be("name");
        bundle.SortName!.RawText.Should().Be("sortname");
        bundle.GemsBonus.Should().Be(800);
        bundle.LootChestBonus.Should().Be("CommonProgChest");
    }
}
