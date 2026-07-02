namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class PortraitPackParserTests
{
    private readonly ILogger<PortraitPackParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public PortraitPackParserTests()
    {
        _logger = Substitute.For<ILogger<PortraitPackParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_AbathurCarbotsPortrait_ReturnsPortraitPackData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("Reward/Name/AbathurCarbotsPortrait").Returns(new GameStringText("Carbot Arthas Portrait"));
        _gameStringTextService.GetGameStringTextFromId("Reward/SortName/AbathurCarbotsPortrait").Returns(new GameStringText("abathur carborts sortname"));

        PortraitPackParser portraitPackParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        PortraitPack? portraitPack = portraitPackParser.Parse("AbathurCarbotsPortrait");

        // assert
        portraitPack.Should().NotBeNull();
        portraitPack.Name!.RawText.Should().Be("Carbot Arthas Portrait");
        portraitPack.SortName!.RawText.Should().Be("abathur carborts sortname");
        portraitPack.Description.Should().BeNull();
        portraitPack.Category.Should().BeNull();
        portraitPack.HyperlinkId.Should().Be("AbathurCarbotsPortrait");
        portraitPack.Id.Should().Be("AbathurCarbotsPortrait");
        portraitPack.Rarity.Should().Be(Rarity.Common);
        portraitPack.ReleaseDate.Should().BeNull();
        portraitPack.Event.Should().BeNull();
        portraitPack.RewardPortraitIds.Should().ContainSingle()
            .And.ContainInOrder("AbathurCarbotsPortrait");
    }

    [TestMethod]
    public void Parse_AdmiralKrakenovPortrait_ReturnsPortraitPackData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("Reward/Name/StukovPortraitPirate").Returns(new GameStringText("Admiral Krakenov Portrait"));
        _gameStringTextService.GetGameStringTextFromId("Reward/SortName/AdmiralKrakenovPortrait").Returns(new GameStringText("Admiral Krakenov Portrait sortname"));

        PortraitPackParser portraitPackParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        PortraitPack? portraitPack = portraitPackParser.Parse("AdmiralKrakenovPortrait");

        // assert
        portraitPack.Should().NotBeNull();
        portraitPack.Name!.RawText.Should().Be("Admiral Krakenov Portrait");
        portraitPack.SortName!.RawText.Should().Be("Admiral Krakenov Portrait sortname");
        portraitPack.Description.Should().BeNull();
        portraitPack.Category.Should().BeNull();
        portraitPack.HyperlinkId.Should().Be("AdmiralKrakenovPortrait");
        portraitPack.Id.Should().Be("AdmiralKrakenovPortrait");
        portraitPack.Rarity.Should().Be(Rarity.Common);
        portraitPack.ReleaseDate.Should().BeNull();
        portraitPack.Event.Should().BeNull();
        portraitPack.RewardPortraitIds.Should().ContainSingle()
            .And.ContainInOrder("StukovPortraitPirate");
    }

    [TestMethod]
    public void Parse_ArthasCrimsonCountHallowsEndPortrait_ReturnsPortraitPackData()
    {
        // arrange
        PortraitPackParser portraitPackParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        PortraitPack? portraitPack = portraitPackParser.Parse("ArthasCrimsonCountHallowsEndPortrait");

        // assert
        portraitPack.Should().NotBeNull();
        portraitPack.HyperlinkId.Should().Be("ArthasCrimsonCountHallowsEndPortrait");
        portraitPack.Id.Should().Be("ArthasCrimsonCountHallowsEndPortrait");
        portraitPack.Rarity.Should().Be(Rarity.Common);
        portraitPack.Event.Should().Be("HallowsEnd");
        portraitPack.RewardPortraitIds.Should().ContainSingle()
            .And.ContainInOrder("ArthasCrimsonCountHallowsEndPortrait");
    }
}
