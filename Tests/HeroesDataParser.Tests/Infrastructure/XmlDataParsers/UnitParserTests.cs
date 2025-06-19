namespace HeroesDataParser.Tests.Infrastructure.XmlDataParsers;

[TestClass]
public class UnitParserTests
{
    private readonly ILogger<UnitParser> _logger;
    private readonly ILogger<TooltipDescriptionService> _tooltipLogger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IAbilityParser _abilityParser;
    private readonly ITooltipDescriptionService _tooltipDescriptionService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public UnitParserTests()
    {
        _logger = Substitute.For<ILogger<UnitParser>>();
        _tooltipLogger = Substitute.For<ILogger<TooltipDescriptionService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _abilityParser = Substitute.For<IAbilityParser>();
        _tooltipDescriptionService = Substitute.For<ITooltipDescriptionService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
            },
        });

        // allow the real instance
        _tooltipDescriptionService = new TooltipDescriptionService(_tooltipLogger, _options, _heroesXmlLoaderService);
    }

    [TestMethod]
    public void Parse_HeroAbathur_ReturnsData()
    {
        // arrange
        string unitId = "HeroAbathur";

        UnitParser unitParser = new(_logger, _options, _heroesXmlLoaderService, _abilityParser, _tooltipDescriptionService);

        Ability abathurUltimateEvolutionAbility = new()
        {
            AbilityElementId = "AbathurUltimateEvolution",
            ButtonElementId = "AbathurUltimateEvolution",
            AbilityType = AbilityType.Heroic,
            TooltipAppendersTalentElementIds =
            {
                "AbathurVolatileMutation",
                "AbathurHasVolatileMutation",
            },
        };

        Ability abathurSymbioteAbility = new()
        {
            AbilityElementId = "AbathurSymbiote",
            ButtonElementId = "AbathurSymbiote",
            AbilityType = AbilityType.Q,
            TooltipAppendersTalentElementIds =
            {
                "AbathurMasteryPressurizedGlands",
            },
        };

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
        unit.SummonedUnitIds.Should().BeEmpty();
        unit.Weapons.Should().ContainSingle();
        unit.TooltipTalentElementIdCount.Should().Be(3);
        unit.GetTooltipAbilityLinkIdsByTalentElementId("AbathurVolatileMutation").Should().ContainSingle().And
            .Contain(new AbilityLinkId("AbathurUltimateEvolution", "AbathurUltimateEvolution", AbilityType.Heroic));

        UnitWeapon abathurWeapon1 = unit.Weapons.First();
        abathurWeapon1.IsDisabled.Should().BeFalse();
        abathurWeapon1.MinimumRange.Should().Be(0);
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

        UnitParser unitParser = new(_logger, _options, _heroesXmlLoaderService, _abilityParser, _tooltipDescriptionService);

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

        UnitParser unitParser = new(_logger, _options, _heroesXmlLoaderService, _abilityParser, _tooltipDescriptionService);

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
            SummonedUnitIds = { "AbathurEvolveMonstrosity" },
        });

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        unit.SubAbilities.Should().ContainSingle();
        unit.SubAbilities.Should().ContainKey(new AbilityLinkId("AbathurEvolveMonstrosity", "AbathurEvolveMonstrosityHotbar", AbilityType.Heroic));
        unit.SummonedUnitIds.Should().ContainInConsecutiveOrder("AbathurEvolveMonstrosity");
    }

    [TestMethod]
    public void Parse_AlarakDeadlyChargeExecute2ndHeroic_ReturnAsSubAbility()
    {
        // arrange
        string unitId = "HeroAlarak";

        UnitParser unitParser = new(_logger, _options, _heroesXmlLoaderService, _abilityParser, _tooltipDescriptionService);

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

    [TestMethod]
    public void Parse_DisallowHiddenAbilities_GetAbilityIsntReceived()
    {
        // arrange
        string unitId = "HeroAmazon";

        UnitParser unitParser = new(_logger, _options, _heroesXmlLoaderService, _abilityParser, _tooltipDescriptionService)
        {
            AllowHiddenAbilities = false,
        };

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        _abilityParser.DidNotReceive().GetAbility(Arg.Any<string>());
    }

    [TestMethod]
    [DataRow(true, 4)]
    [DataRow(false, 0)]
    public void Parse_SpecialAbilities_AbilitiesAdded(bool allow, int count)
    {
        // arrange
        string unitId = "HeroAmazon";

        UnitParser unitParser = new(_logger, _options, _heroesXmlLoaderService, _abilityParser, _tooltipDescriptionService)
        {
            AllowSpecialAbilities = allow,
        };

        _abilityParser.GetAbility(Arg.Is<StormElementData>(x => x.Field == "CardLayouts[0].LayoutButtons[13]")).Returns(new Ability()
        {
            AbilityElementId = "stop",
            ButtonElementId = "Tease",
            AbilityType = AbilityType.Taunt,
            Tier = AbilityTier.Taunt,
        });
        _abilityParser.GetAbility(Arg.Is<StormElementData>(x => x.Field == "CardLayouts[0].LayoutButtons[14]")).Returns(new Ability()
        {
            AbilityElementId = "stop",
            ButtonElementId = "Dance",
            AbilityType = AbilityType.Dance,
            Tier = AbilityTier.Dance,
        });
        _abilityParser.GetAbility(Arg.Is<StormElementData>(x => x.Field == "CardLayouts[0].LayoutButtons[20]")).Returns(new Ability()
        {
            AbilityElementId = "LootSpray",
            ButtonElementId = "LootSpray",
            AbilityType = AbilityType.Spray,
            Tier = AbilityTier.Spray,
        });
        _abilityParser.GetAbility(Arg.Is<StormElementData>(x => x.Field == "CardLayouts[0].LayoutButtons[21]")).Returns(new Ability()
        {
            AbilityElementId = "LootYellVoiceLine",
            ButtonElementId = "LootYellVoiceLine",
            AbilityType = AbilityType.Voice,
            Tier = AbilityTier.Voice,
        });

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        unit.Abilities.Should().HaveCount(count);
    }

    [TestMethod]
    [DataRow(true, 1)]
    [DataRow(false, 0)]
    public void Parse_AbilityHiddenCommand_GetAbility(bool allow, int count)
    {
        // arrange
        string unitId = "HeroHpTESTHero";

        UnitParser unitParser = new(_logger, _options, _heroesXmlLoaderService, _abilityParser, _tooltipDescriptionService)
        {
            AllowHiddenAbilities = allow,
        };

        _abilityParser.GetAbility(Arg.Is<StormElementData>(x => x.Field == "CardLayouts[0].LayoutButtons[0]")).Returns(new Ability()
        {
            AbilityElementId = "Test",
            ButtonElementId = "Test",
            AbilityType = AbilityType.Hidden,
            Tier = AbilityTier.Hidden,
        });

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        _abilityParser.Received().GetAbility(Arg.Any<StormElementData>());
        unit.Abilities.Should().HaveCount(count);
    }

    [TestMethod]
    public void Parse_ChoWeapons_ReturnsWeapons()
    {
        // arrange
        string unitId = "HeroCho";

        UnitParser unitParser = new(_logger, _options, _heroesXmlLoaderService, _abilityParser, _tooltipDescriptionService);

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        UnitWeapon choWeapon1 = unit.Weapons.ToList()[0];
        choWeapon1.IsDisabled.Should().BeFalse();
        choWeapon1.MinimumRange.Should().Be(0);
        choWeapon1.Range.Should().Be(1);
        choWeapon1.Period.Should().Be(1.1);

        UnitWeapon choWeapon2 = unit.Weapons.ToList()[1];
        choWeapon2.IsDisabled.Should().BeFalse();
        choWeapon2.MinimumRange.Should().Be(1);
        choWeapon2.Range.Should().Be(2);
        choWeapon2.Period.Should().Be(1.1);

        UnitWeapon choWeapon3 = unit.Weapons.ToList()[2];
        choWeapon3.IsDisabled.Should().BeTrue();
        choWeapon3.MinimumRange.Should().Be(2);
        choWeapon3.Range.Should().Be(5);
        choWeapon3.Period.Should().Be(1.1);
    }

    [TestMethod]
    public void Parse_TracerWeapons_ReturnsWeapons()
    {
        // arrange
        string unitId = "HeroTracer";

        UnitParser unitParser = new(_logger, _options, _heroesXmlLoaderService, _abilityParser, _tooltipDescriptionService);

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        UnitWeapon weapon1 = unit.Weapons.ToList()[0];
        weapon1.IsDisabled.Should().BeFalse();
        weapon1.MinimumRange.Should().Be(0);
        weapon1.Range.Should().Be(5);
        weapon1.Period.Should().Be(0.125);
        weapon1.VitalCost[VitalType.Energy].Should().Be(2);
    }

    [TestMethod]
    public void Parse_DeathwingBehaviorAbility_ReturnsBehaviorAbility()
    {
        // arrange
        string unitId = "HeroDeathwing";

        UnitParser unitParser = new(_logger, _options, _heroesXmlLoaderService, _abilityParser, _tooltipDescriptionService);

        _abilityParser.GetBehaviorAbility(Arg.Is<StormElementData>(x => x.Field == "Buttons")).Returns(new Ability()
        {
            AbilityElementId = "Test",
            ButtonElementId = "Test",
            AbilityType = AbilityType.Active,
            Tier = AbilityTier.Activable,
        });

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        unit.Abilities.Should().ContainSingle();
        unit.Abilities[AbilityTier.Activable].Should().ContainSingle();
        unit.Abilities[AbilityTier.Activable][0].AbilityElementId.Should().Be("Test");
    }

    [TestMethod]
    public void Parse_AbilityIsChildToBehaviorAbility_ReturnsAbilityAsSubAbility()
    {
        // arrange
        string unitId = "HeroDeathwing";

        UnitParser unitParser = new(_logger, _options, _heroesXmlLoaderService, _abilityParser, _tooltipDescriptionService);

        Ability deathwingLavaBurstAbility = new()
        {
            AbilityElementId = "DeathwingLavaBurst",
            ButtonElementId = "DeathwingLavaBurst",
            AbilityType = AbilityType.W,
            Tier = AbilityTier.Basic,
            ParentAbilityElementId = "DeathwingFormSwitch",
            TooltipAppendersTalentElementIds =
            {
                "DeathwingHasDragonSoulTalent",
            },
        };

        _abilityParser.GetBehaviorAbility(Arg.Is<StormElementData>(x => x.Field == "Buttons")).Returns(new Ability()
        {
            AbilityElementId = "DeathwingFormSwitch",
            ButtonElementId = "DeathwingFormSwitch",
            AbilityType = AbilityType.Active,
            Tier = AbilityTier.Activable,
        });

        _abilityParser.GetAbility(Arg.Is<StormElementData>(x => x.Field == "CardLayouts[0].LayoutButtons[29]")).Returns(deathwingLavaBurstAbility);

        // act
        Unit? unit = unitParser.Parse(unitId);

        // assert
        unit.Should().NotBeNull();
        unit.Abilities.Should().ContainSingle();
        unit.Abilities[AbilityTier.Activable].Should().ContainSingle();
        unit.Abilities[AbilityTier.Activable][0].AbilityElementId.Should().Be("DeathwingFormSwitch");
        unit.SubAbilities.Should().ContainKey(new AbilityLinkId("DeathwingFormSwitch", "DeathwingFormSwitch", AbilityType.Active));

        // we are not mocking all the ability returns
        _abilityParser.Received(35).GetAbility(Arg.Any<StormElementData>());
        _abilityParser.Received(30).GetAbility(Arg.Any<string>());
    }
}
