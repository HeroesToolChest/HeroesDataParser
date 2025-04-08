using HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers.Tests;

[TestClass]
public class TalentParserTests
{
    private readonly ILogger<TalentParser> _talentLogger;
    private readonly ILogger<AbilityParser> _abilityLogger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public TalentParserTests()
    {
        _talentLogger = Substitute.For<ILogger<TalentParser>>();
        _abilityLogger = Substitute.For<ILogger<AbilityParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
    }

    [TestMethod]
    public void GetTalent_AbathurMasteryPressurizedGlands_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Abathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("0");

        Hero hero = new(heroUnit);

        Unit abathurSymbioteUnit = new("AbathurSymbiote");
        abathurSymbioteUnit.AddAbility(new Ability()
        {
            NameId = "AbathurSymbioteSpikeBurst",
            ButtonId = "AbathurSymbioteSpikeBurst",
            AbilityType = AbilityType.W,
            IsPassive = false,
        });

        hero.HeroUnits.Add(abathurSymbioteUnit.Id, abathurSymbioteUnit);

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.NameId.Should().Be("AbathurMasteryPressurizedGlands");
        talent.ButtonId.Should().Be("AbathurSymbiotePressurizedGlandsTalent");
        talent.Icon.Should().Be("storm_ui_icon_abathur_spikeburst.png");
        talent.Name!.RawDescription.Should().Be("Pressurized Glands");
        talent.Column.Should().Be(1);
        talent.Tooltip.ShortText!.RawDescription.Should().Be("Increases Spike Burst range and decreases cooldown");
        talent.Tooltip.FullText!.RawDescription.Should().Be("Increases the range of Symbiote's Spike Burst by");
        talent.Tooltip.Charges.Should().BeNull();
        talent.Tooltip.CooldownText.Should().BeNull();
        talent.Tooltip.EnergyText.Should().BeNull();
        talent.Tooltip.LifeText.Should().BeNull();
        talent.CreateUnits.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.W);
        talent.Tier.Should().Be(TalentTier.Level1);
        //talent.TooltipAppenderTalentIds.Should().HaveCount(2);
    }

    [TestMethod]
    public void GetTalent_AbathurMasteryEnvenomedNestsToxicNest_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Abathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("1");

        Hero hero = new(heroUnit);
        hero.AddAbility(new Ability()
        {
            NameId = "AbathurToxicNest",
            ButtonId = "AbathurToxicNest",
            AbilityType = AbilityType.W,
            IsPassive = false,
        });

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.NameId.Should().Be("AbathurMasteryEnvenomedNestsToxicNest");
        talent.ButtonId.Should().Be("AbathurToxicNestEnvenomedNestTalent");
        talent.Icon.Should().Be("storm_ui_icon_abathur_toxicnest.png");
        talent.Name!.RawDescription.Should().Be("Envenomed Nest");
        talent.Column.Should().Be(2);
        talent.Tooltip.ShortText!.RawDescription.Should().Be("Toxic Nests deal more damage, reduce Armor");
        talent.Tooltip.FullText!.RawDescription.Should().Be("Toxic Nests deal");
        talent.Tooltip.Charges.Should().BeNull();
        talent.Tooltip.CooldownText.Should().BeNull();
        talent.Tooltip.EnergyText.Should().BeNull();
        talent.Tooltip.LifeText.Should().BeNull();
        talent.CreateUnits.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.W);
        talent.Tier.Should().Be(TalentTier.Level1);
        //talent.TooltipAppenderTalentIds.Should().HaveCount(2);
    }

    [TestMethod]
    public void GetTalent_AbathurCombatStyleSurvivalInstincts_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Abathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("3");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.NameId.Should().Be("AbathurCombatStyleSurvivalInstincts");
        talent.ButtonId.Should().Be("AbathurLocustStrainSurvivalInstinctsTalent");
        talent.Icon.Should().Be("storm_ui_icon_abathur_spawnlocust.png");
        talent.Name!.RawDescription.Should().Be("Survival Instincts");
        talent.Column.Should().Be(4);
        talent.Tooltip.ShortText!.RawDescription.Should().Be("Increases Locust Health and damage");
        talent.Tooltip.FullText!.RawDescription.Should().Be("Increases Locust's Health by");
        talent.Tooltip.Charges.Should().BeNull();
        talent.Tooltip.CooldownText.Should().BeNull();
        talent.Tooltip.EnergyText.Should().BeNull();
        talent.Tooltip.LifeText.Should().BeNull();
        talent.CreateUnits.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Trait);
        talent.IsActive.Should().BeFalse();
        talent.IsPassive.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level1);
        //talent.TooltipAppenderTalentIds.Should().HaveCount(2);
    }

    [TestMethod]
    public void GetTalent_AbathurHeroicAbilityUltimateEvolution_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Abathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("10");

        Hero hero = new(heroUnit);
        hero.AddAbility(new Ability()
        {
            NameId = "AbathurUltimateEvolution",
            ButtonId = "AbathurUltimateEvolution",
            AbilityType = AbilityType.Heroic,
            IsPassive = false,
        });

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.NameId.Should().Be("AbathurHeroicAbilityUltimateEvolution");
        talent.ButtonId.Should().Be("AbathurUltimateEvolution");
        talent.Column.Should().Be(1);
        talent.Tooltip.Charges.Should().BeNull();
        talent.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 70 seconds");
        talent.Tooltip.EnergyText.Should().BeNull();
        talent.Tooltip.LifeText.Should().BeNull();
        talent.CreateUnits.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Heroic);
        talent.IsActive.Should().BeTrue();
        talent.IsPassive.Should().BeFalse();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level10);
        //talent.TooltipAppenderTalentIds.Should().HaveCount(2);
    }

    [TestMethod]
    public void GetTalent_GenericTalentCalldownMULE_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Abathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("9");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.NameId.Should().Be("GenericTalentCalldownMULE");
        talent.ButtonId.Should().Be("GenericCalldownMule");
        talent.Name!.RawDescription.Should().Be("Calldown: MULE");
        talent.Icon.Should().Be("storm_ui_icon_talent_mule.png");
        talent.Column.Should().Be(3);
        talent.Tooltip.Charges.Should().BeNull();
        talent.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 60 seconds");
        talent.Tooltip.ShortText!.RawDescription.Should().Be("Activate to heal Structures");
        talent.Tooltip.FullText!.RawDescription.Should().Be("Activate to calldown a Mule that repairs Structures");
        talent.Tooltip.EnergyText.Should().BeNull();
        talent.Tooltip.LifeText.Should().BeNull();
        talent.CreateUnits.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Active);
        talent.IsActive.Should().BeTrue();
        talent.IsPassive.Should().BeFalse();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level7);
    }

    [TestMethod]
    public void GetTalent_MuradinMasteryPassiveStoneform_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Muradin";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("16");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.NameId.Should().Be("MuradinMasteryPassiveStoneform");
        talent.ButtonId.Should().Be("MuradinSecondWindStoneformTalent");
        talent.Icon.Should().Be("storm_ui_icon_Muradin_SecondWind.png");
        talent.Column.Should().Be(3);
        talent.Tooltip.Charges.Should().BeNull();
        talent.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 60 seconds");
        talent.Tooltip.EnergyText.Should().BeNull();
        talent.Tooltip.LifeText.Should().BeNull();
        talent.CreateUnits.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Trait);
        talent.IsActive.Should().BeTrue();
        talent.IsPassive.Should().BeFalse();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level16);
        //talent.TooltipAppenderTalentIds.Should().HaveCount(2);
    }

    [TestMethod]
    public void GetTalent_AlarakExtendedLightning_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Alarak";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("2");

        Hero hero = new(heroUnit);
        hero.AddAbility(new Ability()
        {
            NameId = "AlarakLightningSurge",
            ButtonId = "AlarakLightningSurge",
            AbilityType = AbilityType.E,
            IsPassive = false,
        });

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.NameId.Should().Be("AlarakExtendedLightning");
        talent.ButtonId.Should().Be("AlarakExtendedLightning");
        talent.Column.Should().Be(3);
        talent.CreateUnits.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.E);
        talent.IsActive.Should().BeFalse();
        talent.IsPassive.Should().BeFalse();
        talent.IsQuest.Should().BeTrue();
        talent.Tier.Should().Be(TalentTier.Level1);
        //talent.TooltipAppenderTalentIds.Should().HaveCount(2);
    }
}
