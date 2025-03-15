using Heroes.Element.Models.AbilityTalents;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class HeroParserTests
{
    private readonly ILogger<HeroParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IDataParser<Unit> _unitDataParser;
    private readonly HeroesXmlLoader _heroesXmlLoader;

    public HeroParserTests()
    {
        _logger = Substitute.For<ILogger<HeroParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _unitDataParser = Substitute.For<IDataParser<Unit>>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
    }

    [TestMethod]
    public void Parse_FromUnitToHero_UnitDataSet()
    {
        // arrange
        string heroUnit = "HpTESTHero";
        string unitId = "HeroHpTESTHero";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        HeroParser heroParser = new(_logger, _heroesXmlLoaderService, _unitDataParser);

        _unitDataParser.Parse(unitId).Returns(new Unit(unitId)
        {
            Name = new TooltipDescription("Abathur"),
            SortName = new TooltipDescription("SortNameAbathur"),
            HyperlinkId = "Abathur",
            AttributeId = "Abat",
            Rarity = Rarity.Legendary,
            ReleaseDate = new DateOnly(1001, 2, 20),
            Category = "category",
            Event = "event",
            SearchText = new TooltipDescription("search"),
            Description = new TooltipDescription("description"),
            DamageType = ArmorSet.Hero,
            Radius = 1.1,
            InnerRadius = 0.6875,
            Sight = 10,
            Speed = 4.8398,
            KillXP = 2,
            Attributes = { "Zerg", "Swarm" },
            ScalingLinkIds = { "Abathur", "other" },
            Gender = Gender.Female,
            Life = new UnitLife
            {
                LifeMax = 766,
            },
            Energy = new UnitEnergy
            {
                EnergyMax = 500,
            },
            Shield = new UnitShield
            {
                ShieldMax = 1,
            },
            Armor =
            {
                {
                    ArmorSet.Monster, new UnitArmor()
                    {
                        SplashArmor = 1,
                    }
                },
            },
            HeroPlayStyles = { "sytle1" },
            Portraits = new UnitPortrait()
            {
                MiniMapIcon = "storm_ui_icon_miscrune_1.dds",
                TargetInfoPanel = "storm_ui_icon_miscrune_1.dds",
            },
            UnitIds = { "HeroUnit" },
            Weapons =
            {
                new UnitWeapon()
                {
                     Damage = 1,
                },
            },
            Abilities = { new Ability() },
        });

        // act
        Hero? hero = heroParser.Parse(heroUnit);

        // assert
        hero.Should().NotBeNull();
        hero.Name.Should().BeNull();
        hero.SortName.Should().BeNull();
        hero.HyperlinkId.Should().Be("HpTESTHero");
        hero.AttributeId.Should().Be("Abat");
        hero.Rarity.Should().Be(Rarity.Legendary);
        hero.ReleaseDate.Should().Be(new DateOnly(2014, 3, 13));
        hero.Category.Should().Be("category");
        hero.Event.Should().Be("event");
        hero.SearchText.Should().BeNull();
        hero.Description.Should().BeNull();
        hero.DamageType.Should().Be(ArmorSet.Hero);
        hero.Radius.Should().Be(1.1);
        hero.InnerRadius.Should().Be(0.6875);
        hero.Sight.Should().Be(10);
        hero.Speed.Should().Be(4.8398);
        hero.KillXP.Should().Be(2);
        hero.Attributes.Should()
            .Contain("Zerg").And
            .Contain("Swarm");
        hero.ScalingLinkIds.Should()
            .Contain("Abathur").And
            .Contain("other");
        hero.Gender.Should().Be(Gender.Female);
        hero.Life.LifeMax.Should().Be(766);
        hero.Energy.EnergyMax.Should().Be(500);
        hero.Shield.ShieldMax.Should().Be(1);
        hero.Armor.Should().ContainKey(ArmorSet.Monster);
        hero.HeroPlayStyles.Should().Contain("sytle1");
        hero.Portraits.MiniMapIcon.Should().Be("storm_ui_icon_miscrune_1.dds");
        hero.Portraits.TargetInfoPanel.Should().Be("storm_ui_icon_miscrune_1.dds");
        hero.UnitIds.Should().Contain("HeroUnit");
        hero.Weapons.Should().ContainSingle();
        hero.Abilities.Should().ContainSingle();
    }

    [TestMethod]
    public void Parse_AbathurData_ReturnHeroData()
    {
        // arrange
        string heroUnit = "Abathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        HeroParser heroParser = new(_logger, _heroesXmlLoaderService, _unitDataParser);

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
    }
}