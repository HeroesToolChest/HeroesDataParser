namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class EmoticonPackParserTests
{
    private readonly ILogger<EmoticonPackParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public EmoticonPackParserTests()
    {
        _logger = Substitute.For<ILogger<EmoticonPackParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_AbathurEmoticonPack_ReturnsEmoticonPackData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("EmoticonPack/Name/AbathurEmoticonPack").Returns(new GameStringText("Abathur Pack 1"));
        _gameStringTextService.GetGameStringTextFromId("EmoticonPack/SortName/AbathurEmoticonPack").Returns(new GameStringText("abathur sortname"));
        _gameStringTextService.GetGameStringTextFromId("EmoticonPack/Description/AbathurEmoticonPack").Returns(new GameStringText("abathur desc"));

        EmoticonPackParser emoticonPackParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        EmoticonPack? emoticonPack = emoticonPackParser.Parse("AbathurEmoticonPack");

        // assert
        emoticonPack.Should().NotBeNull();
        emoticonPack.Name!.RawText.Should().Be("Abathur Pack 1");
        emoticonPack.SortName!.RawText.Should().Be("abathur sortname");
        emoticonPack.Description!.RawText.Should().Be("abathur desc");
        emoticonPack.Category.Should().Be("Starcraft");
        emoticonPack.HyperlinkId.Should().Be("AbathurEmoticonPack");
        emoticonPack.Id.Should().Be("AbathurEmoticonPack");
        emoticonPack.Rarity.Should().Be(Rarity.Common);
        emoticonPack.ReleaseDate.Should().Be(new DateOnly(2017, 3, 14));
        emoticonPack.Event.Should().BeNull();
        emoticonPack.EmoticonIds.Should().HaveCount(5)
            .And.ContainInOrder("abathur_happy", "abathur_rofl", "abathur_sad", "abathur_silly", "abathur_speechless");
    }

    [TestMethod]
    public void Parse_SkeletalHandSymbolPack1_ReturnsEmoticonPackData()
    {
        // arrange
        EmoticonPackParser emoticonPackParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        EmoticonPack? emoticonPack = emoticonPackParser.Parse("SkeletalHandSymbolPack1");

        // assert
        emoticonPack.Should().NotBeNull();
        emoticonPack.Category.Should().Be("SeasonalEvents");
        emoticonPack.HyperlinkId.Should().Be("SkeletalHandSymbolPack1");
        emoticonPack.Id.Should().Be("SkeletalHandSymbolPack1");
        emoticonPack.Rarity.Should().Be(Rarity.Common);
        emoticonPack.ReleaseDate.Should().Be(new DateOnly(2017, 10, 17));
        emoticonPack.Event.Should().Be("HallowsEnd");
        emoticonPack.EmoticonIds.Should().HaveCount(5)
            .And.ContainInOrder("hand_thumbsup_skeletal", "hand_thumbsdown_skeletal", "hand_fistbump_skeletal", "hand_hangloose_skeletal", "hand_rockon_skeletal");
    }
}
