using HeroesDataParser.Infrastructure.XmlDataParsers;

namespace HeroesDataParser.Tests.Infrastructure.XmlDataParsers;

[TestClass]
public class UnitParserTests
{
    private readonly ILogger<UnitParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IAbilityParser _abilityParser;
    private readonly HeroesXmlLoader _heroesXmlLoader;

    public UnitParserTests()
    {
        _logger = Substitute.For<ILogger<UnitParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _abilityParser = Substitute.For<IAbilityParser>();
        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
    }

    [TestMethod]
    public void Parse_HeroAbathur_ReturnsData()
    {
        // arrange
        string unitId = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        UnitParser unitParser = new(_logger, _heroesXmlLoaderService, _abilityParser);

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        unit.Name.Should().BeNull();
        unit.SortName.Should().BeNull();
        unit.HyperlinkId.Should().BeNull();
        unit.AttributeId.Should().BeNull();
        unit.Franchise.Should().BeNull();
        unit.Rarity.Should().BeNull();
        unit.ReleaseDate.Should().BeNull();
        unit.Category.Should().BeNull();
        unit.Event.Should().BeNull();
        unit.SearchText.Should().BeNull();
        unit.Description.Should().BeNull();

        unit.Gender.Should().Be(Gender.Neutral);
        unit.DamageType.Should().BeNull();
        unit.Radius.Should().Be(0.75);
        unit.InnerRadius.Should().Be(0.75);
        unit.Sight.Should().Be(12);
        unit.Speed.Should().Be(4.8398);
        unit.KillXP.Should().BeNull();
        unit.Attributes.Should().ContainSingle().And
            .Contain("Heroic");
        unit.ScalingLinkIds.Should().ContainSingle().And
            .Contain("HeroDummyVeterancy");
        unit.Life.LifeMax.Should().Be(685);
        unit.Life.LifeMaxScaling.Should().Be(0.04);
        unit.Life.LifeRegenerationRate.Should().Be(1.4257);
        unit.Life.LifeRegenerationRateScaling.Should().Be(0.04);
        unit.Life.LifeType!.RawDescription.Should().Be("Health");
        unit.Energy.EnergyMax.Should().Be(0);
        unit.Energy.EnergyRegenerationRate.Should().Be(0);
        unit.Energy.EnergyType.Should().BeNull();
        unit.Shield.ShieldMax.Should().Be(0);
        unit.Shield.ShieldMaxScaling.Should().Be(0);
        unit.Shield.ShieldRegenerationRate.Should().Be(0);
        unit.Shield.ShieldRegenerationRateScaling.Should().Be(0);
        unit.Shield.ShieldType!.RawDescription.Should().Be("Shields");
        unit.Armor.Should().BeEmpty();
        unit.Portraits.MiniMapIcon.Should().Be("storm_ui_minimapicon_heros_infestor.png");
        unit.Portraits.TargetInfoPanel.Should().Be("storm_ui_ingame_partyframe_abathur.png");
        unit.UnitIds.Should().BeEmpty();
        unit.Weapons.Should().ContainSingle();

        UnitWeapon abathurWeapon1 = unit.Weapons.First();
        abathurWeapon1.Name!.RawDescription.Should().Be("Hero Abathur");
        abathurWeapon1.Range.Should().Be(1);
        abathurWeapon1.Period.Should().Be(0.7);
        abathurWeapon1.WeaponNameId.Should().Be("HeroAbathur");
        abathurWeapon1.Damage.Should().Be(26);
        abathurWeapon1.DamageScaling.Should().Be(0.04);
        abathurWeapon1.AttacksPerSecond.Should().BeApproximately(1.429, 3);

        _abilityParser.Received(30).GetAbility(Arg.Any<StormElementData>());
        _abilityParser.Received(6).GetAbility(Arg.Any<string>());

    }
}
