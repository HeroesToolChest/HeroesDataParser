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

        Ability abathurUltimateEvolutionAbility = new()
        {
            AbilityElementId = "AbathurUltimateEvolution",
            ButtonElementId = "AbathurUltimateEvolution",
            AbilityType = AbilityType.Heroic,
        };
        abathurUltimateEvolutionAbility.TooltipAppendersTalentElementIds.Add("AbathurVolatileMutation");
        abathurUltimateEvolutionAbility.TooltipAppendersTalentElementIds.Add("AbathurHasVolatileMutation");

        Ability abathurSymbioteAbility = new()
        {
            AbilityElementId = "AbathurSymbiote",
            ButtonElementId = "AbathurSymbiote",
            AbilityType = AbilityType.Q,
        };
        abathurSymbioteAbility.TooltipAppendersTalentElementIds.Add("AbathurMasteryPressurizedGlands");

        _abilityParser.GetAbility(Arg.Is<StormElementData>(x => x.Field == "CardLayouts[0].LayoutButtons[24]")).Returns(abathurUltimateEvolutionAbility);
        _abilityParser.GetAbility(Arg.Is<StormElementData>(x => x.Field == "CardLayouts[0].LayoutButtons[27]")).Returns(abathurSymbioteAbility);

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        unit.Id.Should().Be("HeroAbathur");
        unit.Name!.RawDescription.Should().Be("Abathur");
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
        unit.UnitPortraits.MiniMapIcon.Should().Be("storm_ui_minimapicon_heros_infestor.png");
        unit.UnitPortraits.MiniMapIconPath!.FilePath.Should().StartWith("Assets").And.EndWith("storm_ui_minimapicon_heros_infestor.dds");
        unit.UnitPortraits.TargetInfoPanel.Should().Be("storm_ui_ingame_partyframe_abathur.png");
        unit.UnitPortraits.TargetInfoPanelPath!.FilePath.Should().StartWith("Assets").And.EndWith("storm_ui_ingame_partyframe_Abathur.dds");
        unit.UnitIds.Should().BeEmpty();
        unit.Weapons.Should().ContainSingle();
        unit.TooltipTalentElementIdCount.Should().Be(3);
        unit.GetTooltipAbilityLinkIdsByTalentElementId("AbathurVolatileMutation").Should().ContainSingle().And
            .Contain(new AbilityLinkId("AbathurUltimateEvolution", "AbathurUltimateEvolution", AbilityType.Heroic));

        UnitWeapon abathurWeapon1 = unit.Weapons.First();
        abathurWeapon1.Name!.RawDescription.Should().Be("Hero Abathur");
        abathurWeapon1.Range.Should().Be(1);
        abathurWeapon1.Period.Should().Be(0.7);
        abathurWeapon1.NameId.Should().Be("HeroAbathur");
        abathurWeapon1.Damage.Should().Be(26);
        abathurWeapon1.DamageScaling.Should().Be(0.04);
        abathurWeapon1.AttacksPerSecond.Should().BeApproximately(1.429, 3);

        // we are not mocking all the ability returns
        _abilityParser.Received(30).GetAbility(Arg.Any<StormElementData>());
        _abilityParser.Received(24).GetAbility(Arg.Any<string>());
    }

    [TestMethod]
    public void Parse_AbathurSymbiote_ReturnsData()
    {
        // arrange
        string unitId = "AbathurSymbiote";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        UnitParser unitParser = new(_logger, _heroesXmlLoaderService, _abilityParser);

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        unit.Id.Should().Be("AbathurSymbiote");
        unit.Name!.RawDescription.Should().Be("Symbiote");
        unit.Description!.RawDescription.Should().Be("Spawn and attach a Symbiote...");
        unit.Attributes.Should().HaveCount(6);

        _abilityParser.Received(10).GetAbility(Arg.Any<StormElementData>());
        _abilityParser.Received(7).GetAbility(Arg.Any<string>());
    }

    [TestMethod]
    public void Parse_AbathurEvolveMonstrosityActiveSymbioteCommand_ReturnAsSubAbility()
    {
        // arrange
        string unitId = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        UnitParser unitParser = new(_logger, _heroesXmlLoaderService, _abilityParser);

        _abilityParser.GetAbility(Arg.Is<StormElementData>(x => x.Field == "CardLayouts[0].LayoutButtons[25]")).Returns(new Ability()
        {
            AbilityElementId = "AbathurEvolveMonstrosityActiveSymbiote",
            ButtonElementId = "EvolveMonstrosityActiveHotbar",
            AbilityType = AbilityType.Heroic,
            ParentAbilityElementId = "AbathurEvolveMonstrosity",
        });
        _abilityParser.GetAbility(Arg.Is<StormElementData>(x => x.Field == "CardLayouts[0].LayoutButtons[26]")).Returns(new Ability()
        {
            AbilityElementId = "AbathurEvolveMonstrosity",
            ButtonElementId = "AbathurEvolveMonstrosityHotbar",
            AbilityType = AbilityType.Heroic,
        });

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        unit.SubAbilities.Should().ContainSingle();
        unit.SubAbilities.Should().ContainKey(new AbilityLinkId("AbathurEvolveMonstrosity", "AbathurEvolveMonstrosityHotbar", AbilityType.Heroic));
    }

    [TestMethod]
    public void Parse_AlarakDeadlyChargeExecute2ndHeroic_ReturnAsSubAbility()
    {
        // arrange
        string unitId = "HeroAlarak";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        UnitParser unitParser = new(_logger, _heroesXmlLoaderService, _abilityParser);

        _abilityParser.GetAbility(Arg.Is<StormElementData>(x => x.Field == "CardLayouts[0].LayoutButtons[32]")).Returns(new Ability()
        {
            AbilityElementId = "AlarakDeadlyChargeExecute2ndHeroic",
            ButtonElementId = "AlarakUnleashDeadlyCharge",
            AbilityType = AbilityType.Trait,
            Tier = AbilityTier.Trait,
            ParentAbilityElementId = "AlarakDeadlyChargeActivate2ndHeroic",
        });
        _abilityParser.GetAbility(Arg.Is<StormElementData>(x => x.Field == "CardLayouts[0].LayoutButtons[33]")).Returns(new Ability()
        {
            AbilityElementId = "AlarakDeadlyChargeActivate2ndHeroic",
            ButtonElementId = "AlarakDeadlyCharge2ndHeroicSadism",
            AbilityType = AbilityType.Trait,
            Tier = AbilityTier.Trait,
            ParentAbilityElementId = "AlarakDeadlyChargeSecondHeroic",
        });

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        unit.SubAbilities.Should().BeEmpty(); // should be empty because the subabilites are children to talent abilities
    }
}
