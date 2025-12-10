using HeroesDataParser.Infrastructure.XmlDataParsers;

namespace HeroesDataParser.Tests.Infrastructure.XmlDataParsers;

[TestClass]
public class AnnouncerParserTests
{
    private readonly ILogger<AnnouncerParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public AnnouncerParserTests()
    {
        _logger = Substitute.For<ILogger<AnnouncerParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_Adjutant_ReturnsAnnouncerData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("AnnouncerPack/Description/Adjutant").Returns(new GameStringText("adjustant description"));

        AnnouncerParser announcerParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Announcer? announcer = announcerParser.Parse("Adjutant");

        // assert
        announcer.Should().NotBeNull();
        announcer.AttributeId.Should().Be("AADJ");
        announcer.Category.Should().Be("Starcraft");
        announcer.Description!.RawText.Should().Be("adjustant description");
        announcer.Gender.Should().Be("Female");
        announcer.HeroId.Should().Be("AI");
        announcer.HyperlinkId.Should().Be("AdjutantAnnouncer");
        announcer.Id.Should().Be("Adjutant");
        announcer.Rarity.Should().Be(Rarity.Legendary);
        announcer.ReleaseDate.Should().Be(new DateOnly(2018, 3, 27));
        announcer.IsShownInStore.Should().BeTrue();
        announcer.Image.Should().Be("storm_ui_announcer_adjutant.png");
    }
}
