namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class BoostParserTests
{
    private readonly ILogger<BoostParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public BoostParserTests()
    {
        _logger = Substitute.For<ILogger<BoostParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_360DayStimpack_ReturnsBoostData()
    {
        // arrange
        BoostParser boostParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Boost? boost = boostParser.Parse("360DayStimpack");

        // assert
        boost.Should().NotBeNull();
        boost.Category.Should().BeNull();
        boost.Franchise.Should().BeNull();
        boost.Event.Should().Be("LTO");
        boost.Rarity.Should().BeNull();
        boost.HyperlinkId.Should().Be("360DayStimpack");
        boost.Id.Should().Be("360DayStimpack");
        boost.ReleaseDate.Should().Be(new DateOnly(2016, 11, 22));
        boost.SortName.Should().BeNull();
        boost.Name.Should().BeNull();
        boost.Description.Should().BeNull();
    }
}
