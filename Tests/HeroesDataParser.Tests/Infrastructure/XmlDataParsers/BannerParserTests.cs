namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class BannerParserTests
{
    private readonly ILogger<BannerParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public BannerParserTests()
    {
        _logger = Substitute.For<ILogger<BannerParser>>();
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
        _gameStringTextService.GetGameStringTextFromId("Banner/SortName/BannerDFEsportsWarChestTeam18").Returns(new GameStringText("sort name"));
        _gameStringTextService.GetGameStringTextFromId("Banner/Name/BannerDFEsportsWarChest18RareBallistix").Returns(new GameStringText("name"));
        _gameStringTextService.GetGameStringTextFromId("Banner/Description/BannerDFEsportsWarChestRareEsports").Returns(new GameStringText("description"));

        BannerParser bannerParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Banner? banner = bannerParser.Parse("BannerDFEsportsWarChest18RareBallistix");

        // assert
        banner.Should().NotBeNull();
        banner.AttributeId.Should().Be("BN9a");
        banner.Category.Should().Be("Default");
        banner.Franchise.Should().BeNull();
        banner.Event.Should().BeNull();
        banner.Rarity.Should().Be(Rarity.None);
        banner.HyperlinkId.Should().Be("Ballistix2018PhaseOneWarbanner");
        banner.Id.Should().Be("BannerDFEsportsWarChest18RareBallistix");
        banner.ReleaseDate.Should().Be(new DateOnly(2018, 5, 22));
        banner.SortName!.RawText.Should().Be("sort name");
        banner.Name!.RawText.Should().Be("name");
        banner.Description!.RawText.Should().Be("description");
    }
}
