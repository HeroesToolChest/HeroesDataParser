namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class SkinParserTests
{
    private readonly ILogger<SkinParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public SkinParserTests()
    {
        _logger = Substitute.For<ILogger<SkinParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_MephistoToys19Var4_ReturnsSkinData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("Skin/Info/MephistoToys19").Returns(new GameStringText("info meph toys"));
        _gameStringTextService.GetGameStringTextFromId("Skin/AdditionalSearchText/MephistoToys19Var4").Returns(new GameStringText("mephtoy1 mephtoy2"));

        SkinParser skinParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Skin? skin = skinParser.Parse("MephistoToys19Var4");

        // assert
        skin.Should().NotBeNull();
        skin.AttributeId.Should().Be("Mep3");
        skin.Id.Should().Be("MephistoToys19Var4");
        skin.Rarity.Should().Be(Rarity.Legendary);
        skin.ReleaseDate.Should().Be(new DateOnly(2019, 12, 17));
        skin.Category.Should().BeNull();
        skin.Event.Should().Be("WinterVeil");
        skin.HyperlinkId.Should().Be("CuddlyTickleMephisto");
        skin.Features.Should().HaveCount(2)
            .And.ContainInConsecutiveOrder("AlteredVO", "ThemedAbilities");
        skin.VoiceLineIds.Should().HaveCount(5)
            .And.ContainInConsecutiveOrder(
                "MephistoToys19_VoiceLine01",
                "MephistoToys19_VoiceLine02",
                "MephistoToys19_VoiceLine03",
                "MephistoToys19_VoiceLine04",
                "MephistoToys19_VoiceLine05");
        skin.VariationSkinIds.Should().BeEmpty();
        skin.InfoText!.RawText.Should().Be("info meph toys");
        skin.Franchise.Should().Be(Franchise.Nexus);
        skin.SearchText!.RawText.Should().Be("mephtoy1 mephtoy2");
    }

    [TestMethod]
    public void Parse_AbathurMecha_ReturnsSkinData()
    {
        // arrange
        SkinParser skinParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Skin? skin = skinParser.Parse("AbathurMecha");

        // assert
        skin.Should().NotBeNull();
        skin.AttributeId.Should().Be("AbaD");
        skin.Id.Should().Be("AbathurMecha");
        skin.Rarity.Should().Be(Rarity.Legendary);
        skin.ReleaseDate.Should().Be(new DateOnly(2018, 1, 16));
        skin.Category.Should().BeNull();
        skin.Event.Should().BeNull();
        skin.HyperlinkId.Should().Be("XenotechAbathur");
        skin.Features.Should().HaveCount(3);
        skin.VoiceLineIds.Should().HaveCount(5);
        skin.VariationSkinIds.Should().HaveCount(5)
            .And.ContainInConsecutiveOrder(
                "AbathurMechaVar1",
                "AbathurMechaVar2",
                "AbathurMechaVar3",
                "AbathurMechaVar4",
                "AbathurMechaVar5");
        skin.Franchise.Should().Be(Franchise.Nexus);
    }
}
