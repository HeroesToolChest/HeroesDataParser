using HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            AbilityElementId = "AbathurSymbioteSpikeBurst",
            ButtonElementId = "AbathurSymbioteSpikeBurst",
            AbilityType = AbilityType.W,
        });

        hero.HeroUnits.Add(abathurSymbioteUnit.Id, abathurSymbioteUnit);

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

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
        talent.Tooltip.ShortText!.RawDescription.Should().Be("Increases Spike Burst range and decreases cooldown");
        talent.Tooltip.FullText!.RawDescription.Should().Be("Increases the range of Symbiote's Spike Burst by");
        talent.Tooltip.Charges.Should().BeNull();
        talent.Tooltip.CooldownText.Should().BeNull();
        talent.Tooltip.EnergyText.Should().BeNull();
        talent.Tooltip.LifeText.Should().BeNull();
        talent.CreateUnits.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.W);
        talent.Tier.Should().Be(TalentTier.Level1);
        talent.TooltipAppendersTalentElementIds.Should().BeEmpty();
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
            AbilityElementId = "AbathurToxicNest",
            ButtonElementId = "AbathurToxicNest",
            AbilityType = AbilityType.W,
        });

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

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
        talent.Tooltip.ShortText!.RawDescription.Should().Be("Toxic Nests deal more damage, reduce Armor");
        talent.Tooltip.FullText!.RawDescription.Should().Be("Toxic Nests deal");
        talent.Tooltip.Charges.Should().BeNull();
        talent.Tooltip.CooldownText.Should().BeNull();
        talent.Tooltip.EnergyText.Should().BeNull();
        talent.Tooltip.LifeText.Should().BeNull();
        talent.CreateUnits.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.W);
        talent.Tier.Should().Be(TalentTier.Level1);
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
        talent.TalentElementId.Should().Be("AbathurCombatStyleSurvivalInstincts");
        talent.ButtonElementId.Should().Be("AbathurLocustStrainSurvivalInstinctsTalent");
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
        talent.Tier.Should().Be(TalentTier.Level1);
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
            AbilityElementId = "AbathurUltimateEvolution",
            ButtonElementId = "AbathurUltimateEvolution",
            AbilityType = AbilityType.Heroic,
        });

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.TalentElementId.Should().Be("AbathurHeroicAbilityUltimateEvolution");
        talent.ButtonElementId.Should().Be("AbathurUltimateEvolution");
        talent.Column.Should().Be(1);
        talent.Tooltip.Charges.Should().BeNull();
        talent.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 70 seconds");
        talent.Tooltip.EnergyText.Should().BeNull();
        talent.Tooltip.LifeText.Should().BeNull();
        talent.CreateUnits.Should().BeEmpty();
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

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("20");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

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

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("9");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.TalentElementId.Should().Be("GenericTalentCalldownMULE");
        talent.ButtonElementId.Should().Be("GenericCalldownMule");
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
        talent.TalentElementId.Should().Be("MuradinMasteryPassiveStoneform");
        talent.ButtonElementId.Should().Be("MuradinSecondWindStoneformTalent");
        talent.Icon.Should().Be("storm_ui_icon_Muradin_SecondWind.png");
        talent.Column.Should().Be(3);
        talent.Tooltip.Charges.Should().BeNull();
        talent.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 60 seconds");
        talent.Tooltip.EnergyText.Should().BeNull();
        talent.Tooltip.LifeText.Should().BeNull();
        talent.CreateUnits.Should().BeEmpty();
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

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("2");

        Hero hero = new(heroUnit);
        hero.AddAbility(new Ability()
        {
            AbilityElementId = "AlarakLightningSurge",
            ButtonElementId = "AlarakLightningSurge",
            AbilityType = AbilityType.E,
        });

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.TalentElementId.Should().Be("AlarakExtendedLightning");
        talent.ButtonElementId.Should().Be("AlarakExtendedLightning");
        talent.Column.Should().Be(3);
        talent.CreateUnits.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.E);
        talent.IsActive.Should().BeFalse();
        talent.IsQuest.Should().BeTrue();
        talent.Tier.Should().Be(TalentTier.Level1);
    }

    [TestMethod]
    public void GetTalent_GarroshArmorUpBodyCheck_ReturnsTalent()
    {
        // arrange
        string heroUnit = "Garrosh";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("2");

        Hero hero = new(heroUnit);

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService);

        // act
        Talent? talent = talentParser.GetTalent(hero, talentTreeArray);

        // assert
        talent.Should().NotBeNull();
        talent.LinkId.ToString().Should().Be("GarroshArmorUpBodyCheck|GarroshArmorUpBodyCheck|Active");
        talent.TalentElementId.Should().Be("GarroshArmorUpBodyCheck");
        talent.ButtonElementId.Should().Be("GarroshArmorUpBodyCheck");
        talent.Column.Should().Be(3);
        talent.CreateUnits.Should().BeEmpty();
        talent.AbilityType.Should().Be(AbilityType.Active);
        talent.IsActive.Should().BeTrue();
        talent.IsQuest.Should().BeFalse();
        talent.Tier.Should().Be(TalentTier.Level1);
        talent.Tooltip.Charges!.CountMax.Should().Be(1);
        talent.Tooltip.Charges.CountStart.Should().Be(1);
        talent.Tooltip.Charges.CountUse.Should().Be(1);
        talent.Tooltip.Charges.HasCharges.Should().BeTrue();
        talent.Tooltip.Charges.IsHideCount.Should().BeTrue();
        talent.Tooltip.Charges.RecastCooldown.Should().Be(1);
        talent.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 15 seconds");
        talent.TooltipAppendersTalentElementIds.Should().HaveCount(2).And
            .Contain(["GarroshBodyCheckBruteForce", "GarroshArmorUpInnerRage"]);
    }
}
