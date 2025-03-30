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
    public void GetAbility_AbathurSymbioteCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("27");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("AbathurSymbiote|AbathurSymbiote|Q|False");
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.Q);

        AssertAbathurSymbioteAbility(ability);
    }

    [TestMethod]
    public void GetAbility_AbathurToxicNestCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("28");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("AbathurToxicNest|AbathurToxicNest|W|False");
        ability.Name!.RawDescription.Should().Be("Toxic Nest");
        ability.NameId.Should().Be("AbathurToxicNest");
        ability.ButtonId.Should().Be("AbathurToxicNest");
        ability.Icon.Should().Be("storm_ui_icon_abathur_toxicnest.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.W);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Charge Cooldown: 10 seconds");
        ability.Tooltip.Charges!.CountMax.Should().Be(3);
        ability.Tooltip.Charges!.CountStart.Should().Be(3);
        ability.Tooltip.Charges!.CountUse.Should().Be(1);
        ability.Tooltip.Charges.RecastCooldown.Should().Be(0.0625);
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText.Should().BeNull();
        ability.Tooltip.ShortText!.RawDescription.Should().Be("Spawn a mine");
        ability.Tooltip.FullText!.RawDescription.Should().Be("Spawn a mine that becomes active...");
    }

    [TestMethod]
    public void GetAbility_AbathurLocustStrainCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("26");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("AbathurSpawnLocusts|AbathurLocustStrain|Trait|False");
        ability.Name!.RawDescription.Should().Be("Locust Strain");
        ability.NameId.Should().Be("AbathurSpawnLocusts");
        ability.ButtonId.Should().Be("AbathurLocustStrain");
        ability.Icon.Should().Be("storm_ui_icon_abathur_spawnlocust.png");
        ability.IsActive.Should().BeFalse();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 15 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText.Should().BeNull();
        ability.Tooltip.ShortText!.RawDescription.Should().Be("Spawn locusts that attack down the nearest lane");
        ability.Tooltip.FullText!.RawDescription.Should().Be("Spawns a Locust to attack down the nearest lane...");
        ability.CreateUnits.Should().HaveCount(3).And
            .SatisfyRespectively(
                first => first.Should().Be("AbathurLocustAssaultStrain"),
                second => second.Should().Be("AbathurLocustBombardStrain"),
                third => third.Should().Be("AbathurLocustNormal"));
    }

    [TestMethod]
    public void GetAbility_AbathurDeepTunnelCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("29");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("AbathurDeepTunnel|AbathurDeepTunnel|Z|False");
        ability.Name!.RawDescription.Should().Be("Deep Tunnel");
        ability.NameId.Should().Be("AbathurDeepTunnel");
        ability.ButtonId.Should().Be("AbathurDeepTunnel");
        ability.Icon.Should().Be("storm_ui_icon_abathur_mount.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Mount);
        ability.AbilityType.Should().Be(AbilityType.Z);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 30 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText.Should().BeNull();
        ability.Tooltip.ShortText!.RawDescription.Should().Be("Tunnel to a location.");
        ability.Tooltip.FullText!.RawDescription.Should().Be("Quickly tunnel to a visible location.");
    }

    [TestMethod]
    public void GetAbility_AttackCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("1");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_AbathurSymbioteAbil_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        string abilArrayLinkValue = stormElement.DataValues.GetElementDataAt("AbilArray").GetElementDataAt("18").GetElementDataAt("Link").Value.GetString();

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(abilArrayLinkValue);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("AbathurSymbiote|AbathurSymbiote|Hidden|False");
        ability.Tier.Should().Be(AbilityTier.Hidden);
        ability.AbilityType.Should().Be(AbilityType.Hidden);

        AssertAbathurSymbioteAbility(ability);
    }

    [TestMethod]
    public void GetAbility_AbathurAssumingDirectControlCancelCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "AbathurSymbiote";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("0");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("AbathurAssumingDirectControlCancel|AbathurSymbioteCancel|Heroic|False");
        ability.Name!.RawDescription.Should().Be("Cancel Symbiote");
        ability.Tier.Should().Be(AbilityTier.Heroic);
        ability.AbilityType.Should().Be(AbilityType.Heroic);
        ability.Icon.Should().Be("hud_btn_bg_ability_cancel.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tooltip.FullText!.RawDescription.Should().Be("Cancels the Symbiote ability.");
        ability.Tooltip.ShortText.Should().BeNull();
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 1.5 seconds");
        ability.ToggleCooldown.Should().BeNull();
        ability.CreateUnits.Should().BeEmpty();
    }

    [TestMethod]
    public void GetAbility_NoFaceCommand_ReturnsNull()
    {
        // arrange
        string heroUnit = "AbathurSymbiote";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("2");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_HearthstoneNoManaCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("25");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("Hearthstone|HearthstoneNoMana|B|False");
        ability.Name!.RawDescription.Should().Be("Hearthstone");
        ability.NameId.Should().Be("Hearthstone");
        ability.ButtonId.Should().Be("HearthstoneNoMana");
        ability.Icon.Should().Be("storm_ui_icon_miscrune_1.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Hearth);
        ability.AbilityType.Should().Be(AbilityType.B);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText.Should().BeNull();
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText.Should().BeNull();
        ability.Tooltip.ShortText.Should().BeNull();
        ability.Tooltip.FullText!.RawDescription.Should().Be("After Channeling for...");
    }

    [TestMethod]
    public void GetAbility_TeaseCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("13");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("stop|Tease|Taunt|False");
        ability.Name!.RawDescription.Should().Be("Taunt");
        ability.NameId.Should().Be("stop");
        ability.ButtonId.Should().Be("Tease");
        ability.Icon.Should().Be("btn-command-stop.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Taunt);
        ability.AbilityType.Should().Be(AbilityType.Taunt);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText.Should().BeNull();
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText.Should().BeNull();
        ability.Tooltip.ShortText.Should().BeNull();
        ability.Tooltip.FullText.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_DanceCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("14");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("stop|Dance|Dance|False");
        ability.Name!.RawDescription.Should().Be("Dance");
        ability.NameId.Should().Be("stop");
        ability.ButtonId.Should().Be("Dance");
        ability.Icon.Should().Be("btn-command-stop.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Dance);
        ability.AbilityType.Should().Be(AbilityType.Dance);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText.Should().BeNull();
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText.Should().BeNull();
        ability.Tooltip.ShortText.Should().BeNull();
        ability.Tooltip.FullText.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_LootSprayCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("20");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("LootSpray|LootSpray|Spray|False");
        ability.Name!.RawDescription.Should().Be("Quick Spray Expression");
        ability.NameId.Should().Be("LootSpray");
        ability.ButtonId.Should().Be("LootSpray");
        ability.Icon.Should().Be("storm_temp_war3_btnhealingspray.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Spray);
        ability.AbilityType.Should().Be(AbilityType.Spray);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 3 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText.Should().BeNull();
        ability.Tooltip.ShortText.Should().BeNull();
        ability.Tooltip.FullText!.RawDescription.Should().Be("Express yourself to other players by marking the ground with your selected spray.");
    }

    [TestMethod]
    public void GetAbility_LootYellVoiceLineCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("21");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("LootYellVoiceLine|LootYellVoiceLine|Voice|False");
        ability.Name!.RawDescription.Should().Be("Quick Voice Line Expression");
        ability.NameId.Should().Be("LootYellVoiceLine");
        ability.ButtonId.Should().Be("LootYellVoiceLine");
        ability.Icon.Should().Be("storm_btn_d3_barbarian_threateningshout.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Voice);
        ability.AbilityType.Should().Be(AbilityType.Voice);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 7 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText.Should().BeNull();
        ability.Tooltip.ShortText.Should().BeNull();
        ability.Tooltip.FullText!.RawDescription.Should().Be("Express yourself to other players by playing your selected Voice Line.");
    }

    [TestMethod]
    public void GetAbility_AlarakDeadlyChargeCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlarak";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("26");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("AlarakDeadlyChargeActivate|AlarakDeadlyCharge|Heroic|False");
        ability.Name!.RawDescription.Should().Be("Deadly Charge");
        ability.NameId.Should().Be("AlarakDeadlyChargeActivate");
        ability.ButtonId.Should().Be("AlarakDeadlyCharge");
        ability.Icon.Should().Be("storm_ui_icon_alarak_recklesscharge.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Heroic);
        ability.AbilityType.Should().Be(AbilityType.Heroic);
        ability.ToggleCooldown.Should().Be(0.5);
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 45 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Mana: 60</s>");
        ability.Tooltip.ShortText!.RawDescription.Should().Be("Channel to charge a long distance");
        ability.Tooltip.FullText!.RawDescription.Should().Be("After channeling, Alarak charges forward...");
    }

    [TestMethod]
    public void GetAbility_AlarakSadismCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlarak";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("35");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("AlarakSadism|AlarakSadism|Trait|True");
        ability.Name!.RawDescription.Should().Be("Sadism");
        ability.NameId.Should().Be("AlarakSadism");
        ability.ButtonId.Should().Be("AlarakSadism");
        ability.Icon.Should().Be("storm_ui_icon_alarak_sadism.png");
        ability.IsActive.Should().BeFalse();
        ability.IsPassive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText.Should().BeNull();
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText.Should().BeNull();
        ability.Tooltip.ShortText!.RawDescription.Should().Be("Each point of Sadism increases Alarak's Ability damage...");
        ability.Tooltip.FullText!.RawDescription.Should().Be("Alarak's Ability damage and self-healing are increased...");
    }

    [TestMethod]
    public void GetAbility_AlexstraszaGiftOfLifeCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlexstrasza";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("25");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("AlexstraszaGiftOfLife|AlexstraszaGiftOfLife|Q|False");
        ability.Name!.RawDescription.Should().Be("Gift of Life");
        ability.NameId.Should().Be("AlexstraszaGiftOfLife");
        ability.ButtonId.Should().Be("AlexstraszaGiftOfLife");
        ability.Icon.Should().Be("storm_ui_icon_alexstrasza_gift_of_life.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.Q);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 7 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Health: </s><s val=\"StandardTooltipDetails\">15%</s>");
        ability.Tooltip.EnergyText.Should().BeNull();
        ability.Tooltip.ShortText!.RawDescription.Should().Be("Give a portion of Health to an allied Hero");
        ability.Tooltip.FullText!.RawDescription.Should().Be("Sacrifice...");
    }

    [TestMethod]
    public void GetAbility_AlexstraszaAbundanceCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlexstrasza";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("26");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("AlexstraszaAbundance|AlexstraszaAbundance|W|False");
        ability.Name!.RawDescription.Should().Be("Abundance");
        ability.NameId.Should().Be("AlexstraszaAbundance");
        ability.ButtonId.Should().Be("AlexstraszaAbundance");
        ability.Icon.Should().Be("storm_ui_icon_alexstrasza_abundance.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.W);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 14 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Mana: 75</s>");
        ability.Tooltip.ShortText!.RawDescription.Should().Be("Heal allied Heroes in an area");
        ability.Tooltip.FullText!.RawDescription.Should().Be("Plant a seed of healing that blooms after...");
    }

    [TestMethod]
    public void GetAbility_GuldanLifeTapCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroGuldan";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("32");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("GuldanLifeTap|GuldanLifeTap|Trait|False");
        ability.Name!.RawDescription.Should().Be("Life Tap");
        ability.NameId.Should().Be("GuldanLifeTap");
        ability.ButtonId.Should().Be("GuldanLifeTap");
        ability.Icon.Should().Be("storm_ui_icon_guldan_lifetap.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.ToggleCooldown.Should().Be(0.5);
        ability.Tooltip.CooldownText.Should().BeNull();
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Life: </s><s val=\"StandardTooltipDetails\">222</s>");
        ability.Tooltip.EnergyText.Should().BeNull();
        ability.Tooltip.ShortText!.RawDescription.Should().Be("Restore Mana at the cost of Health");
        ability.Tooltip.FullText!.RawDescription.Should().Be("Gul'dan does not regenerate Mana...");
    }

    [TestMethod]
    public void GetAbility_BarbarianSeismicSlamCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroBarbarian";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("29");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("BarbarianSeismicSlam|BarbarianSeismicSlam|W|False");
        ability.Name!.RawDescription.Should().Be("Seismic Slam");
        ability.NameId.Should().Be("BarbarianSeismicSlam");
        ability.ButtonId.Should().Be("BarbarianSeismicSlam");
        ability.Icon.Should().Be("storm_ui_icon_sonya_seismicslam.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.W);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 1 second");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Fury: 25</s>");
        ability.Tooltip.ShortText!.RawDescription.Should().Be("Damage an enemy and splash damage behind them");
        ability.Tooltip.FullText!.RawDescription.Should().Be("Deals deals damage to...");
    }

    [TestMethod]
    public void GetAbility_UseVehicleAbil_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        string abilArrayLinkValue = stormElement.DataValues.GetElementDataAt("AbilArray").GetElementDataAt("4").GetElementDataAt("Link").Value.GetString();

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(abilArrayLinkValue);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("UseVehicle|UseVehicle|Hidden|False");
        ability.Name!.RawDescription.Should().Be("Use Vehicle");
        ability.Tier.Should().Be(AbilityTier.Hidden);
        ability.AbilityType.Should().Be(AbilityType.Hidden);
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Icon.Should().Be("storm_temp_war3_btnloaddwarf.png");
        ability.Tooltip.CooldownText.Should().BeNull();
        ability.Tooltip.ShortText.Should().BeNull();
        ability.Tooltip.FullText.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_MountCabooseSmartCommandUnitInteractionAbil_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        string abilArrayLinkValue = stormElement.DataValues.GetElementDataAt("AbilArray").GetElementDataAt("10").GetElementDataAt("Link").Value.GetString();

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(abilArrayLinkValue);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("MountCabooseSmartCommandUnitInteraction|MountCabooseSmartCommandUnitInteraction|Hidden|False");
        ability.Name.Should().BeNull();
        ability.Tier.Should().Be(AbilityTier.Hidden);
        ability.AbilityType.Should().Be(AbilityType.Hidden);
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Icon.Should().Be("storm_ui_temp_icon_cheatdeath.png");
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 4 seconds");
        ability.Tooltip.ShortText.Should().BeNull();
        ability.Tooltip.FullText.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_KelThuzadMasterOfTheColdDarkCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroKelThuzad";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("31");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("KelThuzadMasterOfTheColdDark|KelThuzadMasterOfTheColdDark|Trait|True");
        ability.IsActive.Should().BeFalse();
        ability.IsPassive.Should().BeTrue();
    }

    [TestMethod]
    public void GetAbility_SamuroIllusionMasterHeroicCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroSamuro";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("29");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("SamuroIllusionMaster|SamuroIllusionMaster|Heroic|False");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Heroic);
        ability.AbilityType.Should().Be(AbilityType.Heroic);
        ability.Tooltip.ShortText!.RawDescription.Should().Be("Mirror Images can be controlled");
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 8 seconds");
        ability.Icon.Should().Be("storm_ui_icon_samuro_illusiondancer.png");
    }

    [TestMethod]
    public void GetAbility_SamuroIllusionMasterTraitCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroSamuro";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("34");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.Id.ToString().Should().Be("SamuroIllusionMaster|SamuroAdvancingStrikes|Trait|False");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.Tooltip.ShortText!.RawDescription.Should().Be("Increase Movement Speed when attacking Heroes");
        ability.Icon.Should().Be("storm_ui_icon_samuro_flowingstrikes.png");
        ability.TooltipAppenderTalentIds.Should().HaveCount(3)
            .And.Contain(new TalentId("SamuroAdvancingStrikesDeflection", "SamuroDeflectionTalent"))
            .And.Contain(new TalentId("SamuroPressTheAttack", "SamuroPressTheAttack"))
            .And.Contain(new TalentId("SamuroBlademastersPursuit", "SamuroBlademastersPursuitTalent"));
    }

    private static void AssertAbathurSymbioteAbility(Ability ability)
    {
        ability.Name!.RawDescription.Should().Be("Symbiote");
        ability.NameId.Should().Be("AbathurSymbiote");
        ability.ButtonId.Should().Be("AbathurSymbiote");
        ability.Icon.Should().Be("storm_ui_icon_abathur_symbiote.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownText!.RawDescription.Should().Be("Cooldown: 4 seconds");
        ability.Tooltip.LifeText.Should().BeNull();
        ability.Tooltip.EnergyText.Should().BeNull();
        ability.Tooltip.ShortText!.RawDescription.Should().Be("Assist an ally and gain new abilities");
        ability.Tooltip.FullText!.RawDescription.Should().Be("Spawn and attach a Symbiote...");
    }
}