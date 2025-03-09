using HeroesDataParser.Tests.TestHelpers;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers.Tests;

[TestClass]
public class AbilityParserTests
{
    private readonly ILogger<AbilityParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public AbilityParserTests()
    {
        _logger = Substitute.For<ILogger<AbilityParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
    }

    [TestMethod]
    public void GetAbility_AbathurSymbiote_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("27");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Symbiote");
        ability.NameId.Should().Be("AbathurSymbiote");
        ability.ButtonId.Should().Be("AbathurSymbiote");
        ability.Icon.Should().Be("storm_ui_icon_abathur_symbiote.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.Q);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip!.RawDescription.Should().Be("Cooldown: 4 seconds");
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip.Should().BeNull();
        ability.Tooltip.ShortTooltip!.RawDescription.Should().Be("Assist an ally and gain new abilities");
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("Spawn and attach a Symbiote...");
    }

    [TestMethod]
    public void GetAbility_AbathurToxicNest_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("28");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Toxic Nest");
        ability.NameId.Should().Be("AbathurToxicNest");
        ability.ButtonId.Should().Be("AbathurToxicNest");
        ability.Icon.Should().Be("storm_ui_icon_abathur_toxicnest.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.W);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip!.RawDescription.Should().Be("Charge Cooldown: 10 seconds");
        ability.Tooltip.Charges!.CountMax.Should().Be(3);
        ability.Tooltip.Charges!.CountStart.Should().Be(3);
        ability.Tooltip.Charges!.CountUse.Should().Be(1);
        ability.Tooltip.Charges.RecastCooldown.Should().Be(0.0625);
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip.Should().BeNull();
        ability.Tooltip.ShortTooltip!.RawDescription.Should().Be("Spawn a mine");
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("Spawn a mine that becomes active...");
    }

    [TestMethod]
    public void GetAbility_AbathurLocustStrain_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("26");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Locust Strain");
        ability.NameId.Should().Be("AbathurSpawnLocusts");
        ability.ButtonId.Should().Be("AbathurLocustStrain");
        ability.Icon.Should().Be("storm_ui_icon_abathur_spawnlocust.png");
        ability.IsActive.Should().BeFalse();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip!.RawDescription.Should().Be("Cooldown: 15 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip.Should().BeNull();
        ability.Tooltip.ShortTooltip!.RawDescription.Should().Be("Spawn locusts that attack down the nearest lane");
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("Spawns a Locust to attack down the nearest lane...");
        ability.CreateUnits.Should().HaveCount(3).And
            .SatisfyRespectively(
                first => first.Should().Be("AbathurLocustAssaultStrain"),
                second => second.Should().Be("AbathurLocustBombardStrain"),
                third => third.Should().Be("AbathurLocustNormal"));
    }

    [TestMethod]
    public void GetAbility_AbathurDeepTunnel_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("29");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Deep Tunnel");
        ability.NameId.Should().Be("AbathurDeepTunnel");
        ability.ButtonId.Should().Be("AbathurDeepTunnel");
        ability.Icon.Should().Be("storm_ui_icon_abathur_mount.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Mount);
        ability.AbilityType.Should().Be(AbilityType.Z);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip!.RawDescription.Should().Be("Cooldown: 30 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip.Should().BeNull();
        ability.Tooltip.ShortTooltip!.RawDescription.Should().Be("Tunnel to a location.");
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("Quickly tunnel to a visible location.");
    }

    [TestMethod]
    public void GetAbility_HearthstoneNoMana_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("25");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Hearthstone");
        ability.NameId.Should().Be("Hearthstone");
        ability.ButtonId.Should().Be("HearthstoneNoMana");
        ability.Icon.Should().Be("storm_ui_icon_miscrune_1.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Hearth);
        ability.AbilityType.Should().Be(AbilityType.B);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip.Should().BeNull();
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip.Should().BeNull();
        ability.Tooltip.ShortTooltip.Should().BeNull();
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("After Channeling for...");
    }

    [TestMethod]
    public void GetAbility_Tease_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("13");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Taunt");
        ability.NameId.Should().Be("stop");
        ability.ButtonId.Should().Be("Tease");
        ability.Icon.Should().Be("btn-command-stop.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Taunt);
        ability.AbilityType.Should().Be(AbilityType.Taunt);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip.Should().BeNull();
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip.Should().BeNull();
        ability.Tooltip.ShortTooltip.Should().BeNull();
        ability.Tooltip.FullTooltip.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_Dance_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("14");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Dance");
        ability.NameId.Should().Be("stop");
        ability.ButtonId.Should().Be("Dance");
        ability.Icon.Should().Be("btn-command-stop.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Dance);
        ability.AbilityType.Should().Be(AbilityType.Dance);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip.Should().BeNull();
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip.Should().BeNull();
        ability.Tooltip.ShortTooltip.Should().BeNull();
        ability.Tooltip.FullTooltip.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_LootSpray_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("20");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Quick Spray Expression");
        ability.NameId.Should().Be("LootSpray");
        ability.ButtonId.Should().Be("LootSpray");
        ability.Icon.Should().Be("storm_temp_war3_btnhealingspray.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Spray);
        ability.AbilityType.Should().Be(AbilityType.Spray);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip!.RawDescription.Should().Be("Cooldown: 3 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip.Should().BeNull();
        ability.Tooltip.ShortTooltip.Should().BeNull();
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("Express yourself to other players by marking the ground with your selected spray.");
    }

    [TestMethod]
    public void GetAbility_LootYellVoiceLine_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("21");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Quick Voice Line Expression");
        ability.NameId.Should().Be("LootYellVoiceLine");
        ability.ButtonId.Should().Be("LootYellVoiceLine");
        ability.Icon.Should().Be("storm_btn_d3_barbarian_threateningshout.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Voice);
        ability.AbilityType.Should().Be(AbilityType.Voice);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip!.RawDescription.Should().Be("Cooldown: 7 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip.Should().BeNull();
        ability.Tooltip.ShortTooltip.Should().BeNull();
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("Express yourself to other players by playing your selected Voice Line.");
    }

    [TestMethod]
    public void GetAbility_AlarakDeadlyCharge_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlarak";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("26");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Deadly Charge");
        ability.NameId.Should().Be("AlarakDeadlyChargeActivate");
        ability.ButtonId.Should().Be("AlarakDeadlyCharge");
        ability.Icon.Should().Be("storm_ui_icon_alarak_recklesscharge.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Heroic);
        ability.AbilityType.Should().Be(AbilityType.Heroic);
        ability.ToggleCooldown.Should().Be(0.5);
        ability.Tooltip.CooldownTooltip!.RawDescription.Should().Be("Cooldown: 45 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Mana: 60</s>");
        ability.Tooltip.ShortTooltip!.RawDescription.Should().Be("Channel to charge a long distance");
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("After channeling, Alarak charges forward...");
    }

    [TestMethod]
    public void GetAbility_AlarakSadism_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlarak";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("35");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Sadism");
        ability.NameId.Should().Be("AlarakSadism");
        ability.ButtonId.Should().Be("AlarakSadism");
        ability.Icon.Should().Be("storm_ui_icon_alarak_sadism.png");
        ability.IsActive.Should().BeFalse();
        ability.IsPassive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip.Should().BeNull();
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip.Should().BeNull();
        ability.Tooltip.ShortTooltip!.RawDescription.Should().Be("Each point of Sadism increases Alarak's Ability damage...");
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("Alarak's Ability damage and self-healing are increased...");
    }

    [TestMethod]
    public void GetAbility_AlexstraszaGiftOfLife_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlexstrasza";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("25");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Gift of Life");
        ability.NameId.Should().Be("AlexstraszaGiftOfLife");
        ability.ButtonId.Should().Be("AlexstraszaGiftOfLife");
        ability.Icon.Should().Be("storm_ui_icon_alexstrasza_gift_of_life.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.Q);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip!.RawDescription.Should().Be("Cooldown: 7 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Health: </s><s val=\"StandardTooltipDetails\">15%</s>");
        ability.Tooltip.EnergyTooltip.Should().BeNull();
        ability.Tooltip.ShortTooltip!.RawDescription.Should().Be("Give a portion of Health to an allied Hero");
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("Sacrifice...");
    }

    [TestMethod]
    public void GetAbility_AlexstraszaAbundance_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlexstrasza";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("26");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Abundance");
        ability.NameId.Should().Be("AlexstraszaAbundance");
        ability.ButtonId.Should().Be("AlexstraszaAbundance");
        ability.Icon.Should().Be("storm_ui_icon_alexstrasza_abundance.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.W);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip!.RawDescription.Should().Be("Cooldown: 14 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Mana: 75</s>");
        ability.Tooltip.ShortTooltip!.RawDescription.Should().Be("Heal allied Heroes in an area");
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("Plant a seed of healing that blooms after...");
    }

    [TestMethod]
    public void GetAbility_GuldanLifeTap_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroGuldan";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("32");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Life Tap");
        ability.NameId.Should().Be("GuldanLifeTap");
        ability.ButtonId.Should().Be("GuldanLifeTap");
        ability.Icon.Should().Be("storm_ui_icon_guldan_lifetap.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.ToggleCooldown.Should().Be(0.5);
        ability.Tooltip.CooldownTooltip.Should().BeNull();
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Life: </s><s val=\"StandardTooltipDetails\">222</s>");
        ability.Tooltip.EnergyTooltip.Should().BeNull();
        ability.Tooltip.ShortTooltip!.RawDescription.Should().Be("Restore Mana at the cost of Health");
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("Gul'dan does not regenerate Mana...");
    }

    [TestMethod]
    public void GetAbility_BarbarianSeismicSlam_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroBarbarian";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("29");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(heroUnit, layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Name!.RawDescription.Should().Be("Seismic Slam");
        ability.NameId.Should().Be("BarbarianSeismicSlam");
        ability.ButtonId.Should().Be("BarbarianSeismicSlam");
        ability.Icon.Should().Be("storm_ui_icon_sonya_seismicslam.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.W);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip!.RawDescription.Should().Be("Cooldown: 1 second");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Fury: 25</s>");
        ability.Tooltip.ShortTooltip!.RawDescription.Should().Be("Damage an enemy and splash damage behind them");
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("Deals deals damage to...");
    }
}