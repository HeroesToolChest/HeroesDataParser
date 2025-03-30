using Heroes.Element.Models;
using Heroes.Element.Models.AbilityTalents;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class HeroParserTests
{
    private readonly ILogger<HeroParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IUnitParser _unitParser;
    private readonly ITalentParser _talentParser;
    private readonly HeroesXmlLoader _heroesXmlLoader;

    public HeroParserTests()
    {
        _logger = Substitute.For<ILogger<HeroParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _unitParser = Substitute.For<IUnitParser>();
        _talentParser = Substitute.For<ITalentParser>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
    }

    [TestMethod]
    public void Parse_UnitParsingForHeroUnit_UnitParsed()
    {
        // arrange
        string heroUnit = "HpTESTHero";
        string unitId = "HeroHpTESTHero";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        HeroParser heroParser = new(_logger, _heroesXmlLoaderService, _unitParser, _talentParser);

        Hero hero = new(heroUnit)
        {
            UnitId = unitId,
        };

        // act
        _ = heroParser.Parse(hero.Id);

        // assert
        _unitParser.Received().Parse(hero, hero.UnitId);
    }

    [TestMethod]
    public void Parse_AbathurData_ReturnHeroData()
    {
        // arrange
        string heroUnit = "Abathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        HeroParser heroParser = new(_logger, _heroesXmlLoaderService, _unitParser, _talentParser);

        _unitParser.Parse("AbathurSymbiote").Returns(new Unit("AbathurSymbiote"));

        // act
        Hero? hero = heroParser.Parse(heroUnit);

        // assert
        hero.Should().NotBeNull();
        hero.Name!.RawDescription.Should().Be("Abathur");
        hero.SortName.Should().BeNull();
        hero.HyperlinkId.Should().Be("Abathur");
        hero.AttributeId.Should().Be("Abat");
        hero.Franchise.Should().Be(Franchise.Starcraft);
        hero.Rarity.Should().Be(Rarity.Legendary);
        hero.ReleaseDate.Should().Be(new DateOnly(2014, 3, 13));
        hero.Category.Should().BeNull();
        hero.Event.Should().BeNull();
        hero.SearchText!.RawDescription.Should().Be("Abathur Zerg Swarm HotS Heart of the Swarm StarCraft II 2 SC2 Star2 Starcraft2 SC slug Double Soak");
        hero.Description!.RawDescription.Should().Be("A unique Hero that can manipulate the battle from anywhere on the map.");
        hero.UnitId.Should().Be("HeroAbathur");
        hero.Title!.RawDescription.Should().Be("Evolution Master");
        hero.Difficulty!.RawDescription.Should().Be("Very Hard");
        hero.IsMelee.Should().BeTrue();
        hero.DefaultMountId.Should().BeNull();
        hero.Roles.Should().ContainSingle().And.Contain(new TooltipDescription("Specialist"));
        hero.ExpandedRole!.RawDescription.Should().Be("Support");
        hero.Ratings.Damage.Should().Be(3);
        hero.Ratings.Complexity.Should().Be(9);
        hero.Ratings.Survivability.Should().Be(1);
        hero.Ratings.Utility.Should().Be(7);
        hero.SkinIds.Should().HaveCount(4).And
            .SatisfyRespectively(
                first => first.Should().Be("AbathurMecha"),
                second => second.Should().Be("AbathurPajamathur"),
                third => third.Should().Be("AbathurSkelethur"),
                fourth => fourth.Should().Be("AbathurUltimate"));
        hero.VariationSkinIds.Should().HaveCount(3).And
            .SatisfyRespectively(
                first => first.Should().Be("AbathurBaseVar3"),
                second => second.Should().Be("AbathurBone"),
                third => third.Should().Be("AbathurChar"));
        hero.VoiceLineIds.Should().HaveCount(5).And
            .SatisfyRespectively(
                first => first.Should().Be("AbathurBase_VoiceLine01"),
                second => second.Should().Be("AbathurBase_VoiceLine02"),
                third => third.Should().Be("AbathurBase_VoiceLine03"),
                fourth => fourth.Should().Be("AbathurBase_VoiceLine04"),
                fifth => fifth.Should().Be("AbathurBase_VoiceLine05"));
        hero.MountCategoryIds.Should().BeEmpty();
        hero.InfoText!.RawDescription.Should().Be("Abathur, the Evolution Master of Kerrigan's Swarm, works ceaselessly...");
        hero.HeroUnits.Should().ContainSingle();
    }
}