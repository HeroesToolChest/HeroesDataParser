namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers.Tests;

[TestClass]
public class TalentParserTests
{
    private readonly ILogger<TalentParser> _talentLogger;
    private readonly ILogger<TooltipDescriptionService> _tooltipLogger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly ITooltipDescriptionService _tooltipDescriptionService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public TalentParserTests()
    {
        _talentLogger = Substitute.For<ILogger<TalentParser>>();
        _tooltipLogger = Substitute.For<ILogger<TooltipDescriptionService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
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
    public void GetTalent_AbathurMasteryPressurizedGlands_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Abathur";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("0");

        Hero hero = new(heroUnit);
        Unit abathurSymbioteUnit = new("AbathurSymbiote");
        abathurSymbioteUnit.AddAbility(new Ability()
        {
            AbilityElementId = "AbathurSymbioteSpikeBurst",
            ButtonElementId = "AbathurSymbioteSpikeBurst",
            AbilityType = AbilityType.W,
        });

        hero.HeroUnits.Add(abathurSymbioteUnit.Id, abathurSymbioteUnit);

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.LinkId.ToString().Should().Be("AbathurMasteryPressurizedGlands|AbathurSymbiotePressurizedGlandsTalent|W");
        talent.TalentElementId.Should().Be("AbathurMasteryPressurizedGlands");
        talent.ButtonElementId.Should().Be("AbathurSymbiotePressurizedGlandsTalent");
        talent.Icon.Should().Be("storm_ui_icon_abathur_spikeburst.png");
        talent.IconPath!.FilePath.Should().NotBeNullOrWhiteSpace();
        talent.IconPath!.MpqFilePath.Should().BeNull();
        talent.Name!.RawDescription.Should().Be("Pressurized Glands");
        talent.Column.Should().Be(1);
        talent.ShortText!.RawDescription.Should().Be("Increases Spike Burst range and decreases cooldown");
        talent.FullText!.RawDescription.Should().Be("Increases the range of Symbiote's Spike Burst by");
        talent.Charges.Should().BeNull();
        talent.CooldownText.Should().BeNull();
        talent.EnergyText.Should().BeNull();
        talent.LifeText.Should().BeNull();
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.W);
        talent.Tier.Should().Be(TalentTier.Level1);
        talent.TooltipAppendersTalentElementIds.Should().BeEmpty();
    }

    [TestMethod]
    public void GetTalent_AbathurMasteryEnvenomedNestsToxicNest_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Abathur";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("1");

        Hero hero = new(heroUnit);
        hero.AddAbility(new Ability()
        {
            AbilityElementId = "AbathurToxicNest",
            ButtonElementId = "AbathurToxicNest",
            AbilityType = AbilityType.W,
        });

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.LinkId.ToString().Should().Be("AbathurMasteryEnvenomedNestsToxicNest|AbathurToxicNestEnvenomedNestTalent|W");
        talent.TalentElementId.Should().Be("AbathurMasteryEnvenomedNestsToxicNest");
        talent.ButtonElementId.Should().Be("AbathurToxicNestEnvenomedNestTalent");
        talent.Icon.Should().Be("storm_ui_icon_abathur_toxicnest.png");
        talent.Name!.RawDescription.Should().Be("Envenomed Nest");
        talent.Column.Should().Be(2);
        talent.ShortText!.RawDescription.Should().Be("Toxic Nests deal more damage, reduce Armor");
        talent.FullText!.RawDescription.Should().Be("Toxic Nests deal");
        talent.Charges.Should().BeNull();
        talent.CooldownText.Should().BeNull();
        talent.EnergyText.Should().BeNull();
        talent.LifeText.Should().BeNull();
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.W);
        talent.Tier.Should().Be(TalentTier.Level1);
    }

    [TestMethod]
    public void GetTalent_AbathurCombatStyleSurvivalInstincts_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Abathur";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("3");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.TalentElementId.Should().Be("AbathurCombatStyleSurvivalInstincts");
        talent.ButtonElementId.Should().Be("AbathurLocustStrainSurvivalInstinctsTalent");
        talent.Icon.Should().Be("storm_ui_icon_abathur_spawnlocust.png");
        talent.Name!.RawDescription.Should().Be("Survival Instincts");
        talent.Column.Should().Be(4);
        talent.ShortText!.RawDescription.Should().Be("Increases Locust Health and damage");
        talent.FullText!.RawDescription.Should().Be("Increases Locust's Health by");
        talent.Charges.Should().BeNull();
        talent.CooldownText.Should().BeNull();
        talent.EnergyText.Should().BeNull();
        talent.LifeText.Should().BeNull();
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Trait);
        talent.IsActive.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level1);
    }

    [TestMethod]
    public void GetTalent_AbathurHeroicAbilityUltimateEvolution_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Abathur";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("10");

        Hero hero = new(heroUnit);
        hero.AddAbility(new Ability()
        {
            AbilityElementId = "AbathurUltimateEvolution",
            ButtonElementId = "AbathurUltimateEvolution",
            AbilityType = AbilityType.Heroic,
        });

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.TalentElementId.Should().Be("AbathurHeroicAbilityUltimateEvolution");
        talent.ButtonElementId.Should().Be("AbathurUltimateEvolution");
        talent.Column.Should().Be(1);
        talent.Charges.Should().BeNull();
        talent.CooldownText!.RawDescription.Should().Be("Cooldown: 70 seconds");
        talent.EnergyText.Should().BeNull();
        talent.LifeText.Should().BeNull();
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Heroic);
        talent.IsActive.Should().BeTrue();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level10);
    }

    [TestMethod]
    public void GetTalent_AbathurHeroicAbilityEvolveMonstrosityPrerequisiteTalentIds_GetsPrerequisiteTalentIds()
    {
        // arrange
        string heroUnit = "Abathur";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("20");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.Tier.Should().Be(TalentTier.Level20);
        talent.PrerequisiteTalentIds.Should().ContainSingle().And
            .Contain("AbathurHeroicAbilityEvolveMonstrosity");

        _talentLogger.Received(1).Log(LogLevel.Warning, 0, Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());
    }

    [TestMethod]
    public void GetTalent_GenericTalentCalldownMULE_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Abathur";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("9");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.TalentElementId.Should().Be("GenericTalentCalldownMULE");
        talent.ButtonElementId.Should().Be("GenericCalldownMule");
        talent.Name!.RawDescription.Should().Be("Calldown: MULE");
        talent.Icon.Should().Be("storm_ui_icon_talent_mule.png");
        talent.Column.Should().Be(3);
        talent.Charges.Should().BeNull();
        talent.CooldownText!.RawDescription.Should().Be("Cooldown: 60 seconds");
        talent.ShortText!.RawDescription.Should().Be("Activate to heal Structures");
        talent.FullText!.RawDescription.Should().Be("Activate to calldown a Mule that repairs Structures");
        talent.EnergyText.Should().BeNull();
        talent.LifeText.Should().BeNull();
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Active);
        talent.IsActive.Should().BeTrue();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level7);
    }

    [TestMethod]
    public void GetTalent_MuradinMasteryPassiveStoneform_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Muradin";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("16");

        Hero hero = new(heroUnit);
        hero.AddAbility(new Ability()
        {
            AbilityElementId = "Stoneform",
            ButtonElementId = "MuradinSecondWindActivateable",
            AbilityType = AbilityType.Trait,
        });

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.TalentElementId.Should().Be("MuradinMasteryPassiveStoneform");
        talent.ButtonElementId.Should().Be("MuradinSecondWindStoneformTalent");
        talent.Icon.Should().Be("storm_ui_icon_muradin_secondwind.png");
        talent.Column.Should().Be(3);
        talent.Charges.Should().BeNull();
        talent.CooldownText!.RawDescription.Should().Be("Cooldown: 60 seconds");
        talent.EnergyText.Should().BeNull();
        talent.LifeText.Should().BeNull();
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Trait);
        talent.IsActive.Should().BeTrue();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level16);
    }

    [TestMethod]
    public void GetTalent_AlarakExtendedLightning_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Alarak";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("2");

        Hero hero = new(heroUnit);
        hero.AddAbility(new Ability()
        {
            AbilityElementId = "AlarakLightningSurge",
            ButtonElementId = "AlarakLightningSurge",
            AbilityType = AbilityType.E,
        });

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.TalentElementId.Should().Be("AlarakExtendedLightning");
        talent.ButtonElementId.Should().Be("AlarakExtendedLightning");
        talent.Column.Should().Be(3);
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.E);
        talent.IsActive.Should().BeFalse();
        talent.IsQuest.Should().BeTrue();
        talent.Tier.Should().Be(TalentTier.Level1);
    }

    [TestMethod]
    public void GetTalent_AlarakRiteofRakShir_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Alarak";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("13");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.LinkId.ToString().Should().Be("AlarakRiteofRakShir|AlarakRiteofRakShir|Trait");
        talent.Column.Should().Be(3);
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Trait);
        talent.IsActive.Should().BeTrue();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level13);
    }

    [TestMethod]
    public void GetTalent_AlarakShowofForce_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Alarak";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("5");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.LinkId.ToString().Should().Be("AlarakShowofForce|AlarakShowofForce|Trait");
        talent.Column.Should().Be(3);
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Trait);
        talent.IsActive.Should().BeFalse();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level4);
    }

    [TestMethod]
    public void GetTalent_GarroshArmorUpBodyCheck_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Garrosh";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("2");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.LinkId.ToString().Should().Be("GarroshArmorUpBodyCheck|GarroshArmorUpBodyCheck|Active");
        talent.TalentElementId.Should().Be("GarroshArmorUpBodyCheck");
        talent.ButtonElementId.Should().Be("GarroshArmorUpBodyCheck");
        talent.Column.Should().Be(3);
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Active);
        talent.IsActive.Should().BeTrue();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level1);
        talent.Charges!.CountMax.Should().Be(1);
        talent.Charges.CountStart.Should().Be(1);
        talent.Charges.CountUse.Should().Be(1);
        talent.Charges.HasCharges.Should().BeTrue();
        talent.Charges.IsCountHidden.Should().BeTrue();
        talent.Charges.RecastCooldown.Should().Be(1);
        talent.CooldownText!.RawDescription.Should().Be("Cooldown: 15 seconds");
        talent.TooltipAppendersTalentElementIds.Should().HaveCount(2).And
            .Contain(["GarroshBodyCheckBruteForce", "GarroshArmorUpInnerRage"]);
    }

    [TestMethod]
    public void GetTalent_BarbarianBattleRage_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Barbarian";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("8");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.LinkId.ToString().Should().Be("BarbarianBattleRage|BarbarianBattleRage|Active");
        talent.Column.Should().Be(3);
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Active);
        talent.IsActive.Should().BeTrue();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level7);
        talent.ToggleCooldown.Should().BeNull();
        talent.Charges!.CountMax.Should().Be(2);
        talent.Charges.CountStart.Should().BeNull();
        talent.Charges.CountUse.Should().Be(1);
        talent.Charges.HasCharges.Should().BeTrue();
        talent.Charges.IsCountHidden.Should().BeFalse();
        talent.Charges.RecastCooldown.Should().Be(8);
        talent.CooldownText!.RawDescription.Should().Be("Charge Cooldown: 30 seconds");
    }

    [TestMethod]
    public void GetTalent_ThrallAncestralWrath_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Thrall";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("7");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.LinkId.ToString().Should().Be("ThrallAncestralWrath|ThrallAncestralWrath|Active");
        talent.Column.Should().Be(2);
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Active);
        talent.IsActive.Should().BeTrue();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level7);
        talent.ToggleCooldown.Should().BeNull();
        talent.Charges!.CountMax.Should().Be(8);
        talent.Charges.CountStart.Should().BeNull();
        talent.Charges.CountUse.Should().Be(8);
        talent.Charges.HasCharges.Should().BeTrue();
        talent.Charges.IsCountHidden.Should().BeFalse();
        talent.Charges.RecastCooldown.Should().Be(1);
        talent.CooldownText.Should().BeNull();
    }

    [TestMethod]
    public void GetTalent_GallKeepMoving_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Gall";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("0");

        Hero hero = new(heroUnit);
        hero.AddAbility(new Ability()
        {
            AbilityElementId = "GallShove",
            ButtonElementId = "GallShoveHotbar",
            AbilityType = AbilityType.Z,
        });

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.LinkId.ToString().Should().Be("GallKeepMoving|GallKeepMoving|Z");
        talent.Column.Should().Be(1);
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Z);
        talent.IsActive.Should().BeFalse();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level1);
        talent.ToggleCooldown.Should().BeNull();
        talent.Charges.Should().BeNull();
        talent.CooldownText.Should().BeNull();
    }

    [TestMethod]
    public void GetTalent_AnubarakCombatStyleLegionOfBeetles_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Anubarak";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("1");

        Hero hero = new(heroUnit);
        hero.AddAbility(new Ability()
        {
            AbilityElementId = "AnubarakLegionOfBeetlesToggle",
            ButtonElementId = AbilityTalentParserBase.NoButtonElementId,
            AbilityType = AbilityType.Hidden,
        });
        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.LinkId.ToString().Should().Be("AnubarakCombatStyleLegionOfBeetles|AnubarakLegionOfBeetlesTalent|Trait");
        talent.Column.Should().Be(2);
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Trait);
        talent.IsActive.Should().BeFalse();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level1);
        talent.ToggleCooldown.Should().BeNull();
        talent.Charges.Should().BeNull();
    }

    [TestMethod]
    public void GetTalent_DVaLiquidCooling_ReturnsTalent()
    {
        // arrange
        string heroUnit = "DVa";

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("2");

        Hero hero = new(heroUnit);
        hero.AddAbility(new Ability()
        {
            AbilityElementId = "DVaLiquidCoolingAbility",
            ButtonElementId = "DVaLiquidCooling",
            AbilityType = AbilityType.Active,
        });

        TalentParser talentParser = new(_talentLogger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.LinkId.ToString().Should().Be("DVaLiquidCooling|DVaLiquidCooling|Active");
        talent.Column.Should().Be(3);
        talent.SummonedUnitIds.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Active);
        talent.IsActive.Should().BeTrue();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level1);
        talent.ToggleCooldown.Should().BeNull();
        talent.Charges.Should().BeNull();
        talent.CooldownText!.RawDescription.Should().Be("Cooldown: 50 seconds");
    }
}
