using HeroesDataParser.Core;
using HeroesDataParser.Options;
using HeroesDataParser.Tests.TestHelpers;
using Microsoft.Extensions.Options;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers.Tests;

[TestClass]
public class AbilityParserTests
{
    private readonly ILogger<AbilityParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly ITooltipDescriptionService _tooltipDescriptionService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public AbilityParserTests()
    {
        _logger = Substitute.For<ILogger<AbilityParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _tooltipDescriptionService = Substitute.For<ITooltipDescriptionService>();

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

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AbathurSymbiote|AbathurSymbiote|Q");
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

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AbathurToxicNest|AbathurToxicNest|W");
        ability.Name!.RawDescription.Should().Be("Toxic Nest");
        ability.AbilityElementId.Should().Be("AbathurToxicNest");
        ability.ButtonElementId.Should().Be("AbathurToxicNest");
        ability.Icon.Should().Be("storm_ui_icon_abathur_toxicnest.png");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.W);
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText!.RawDescription.Should().Be("Charge Cooldown: 10 seconds");
        ability.Charges!.CountMax.Should().Be(3);
        ability.Charges!.CountStart.Should().Be(3);
        ability.Charges!.CountUse.Should().Be(1);
        ability.Charges.RecastCooldown.Should().Be(0.0625);
        ability.LifeText.Should().BeNull();
        ability.EnergyText.Should().BeNull();
        ability.ShortText!.RawDescription.Should().Be("Spawn a mine");
        ability.FullText!.RawDescription.Should().Be("Spawn a mine that becomes active...");
        ability.SummonedUnitIds.Should().Contain("AbathurToxicNest");
    }

    [TestMethod]
    public void GetAbility_AbathurLocustStrainCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("26");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AbathurSpawnLocusts|AbathurLocustStrain|Trait");
        ability.Name!.RawDescription.Should().Be("Locust Strain");
        ability.AbilityElementId.Should().Be("AbathurSpawnLocusts");
        ability.ButtonElementId.Should().Be("AbathurLocustStrain");
        ability.Icon.Should().Be("storm_ui_icon_abathur_spawnlocust.png");
        ability.IsActive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 15 seconds");
        ability.Charges.Should().BeNull();
        ability.LifeText.Should().BeNull();
        ability.EnergyText.Should().BeNull();
        ability.ShortText!.RawDescription.Should().Be("Spawn locusts that attack down the nearest lane");
        ability.FullText!.RawDescription.Should().Be("Spawns a Locust to attack down the nearest lane...");
        ability.SummonedUnitIds.Should().HaveCount(3).And
            .ContainInConsecutiveOrder("AbathurLocustAssaultStrain", "AbathurLocustBombardStrain", "AbathurLocustNormal");
    }

    [TestMethod]
    public void GetAbility_AbathurDeepTunnelCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("29");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AbathurDeepTunnel|AbathurDeepTunnel|Z");
        ability.Name!.RawDescription.Should().Be("Deep Tunnel");
        ability.AbilityElementId.Should().Be("AbathurDeepTunnel");
        ability.ButtonElementId.Should().Be("AbathurDeepTunnel");
        ability.Icon.Should().Be("storm_ui_icon_abathur_mount.png");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Mount);
        ability.AbilityType.Should().Be(AbilityType.Z);
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 30 seconds");
        ability.Charges.Should().BeNull();
        ability.LifeText.Should().BeNull();
        ability.EnergyText.Should().BeNull();
        ability.ShortText!.RawDescription.Should().Be("Tunnel to a location.");
        ability.FullText!.RawDescription.Should().Be("Quickly tunnel to a visible location.");
    }

    [TestMethod]
    public void GetAbility_AbathurEvolveMonstrosityComand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("23");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AbathurEvolveMonstrosity|AbathurEvolveMonstrosityHotbar|Heroic");
        ability.SummonedUnitIds.Should().ContainInConsecutiveOrder("AbathurEvolvedMonstrosity");
    }

    [TestMethod]
    public void GetAbility_AttackCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("1");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

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

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(abilArrayLinkValue);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AbathurSymbiote|AbathurSymbiote|Hidden");
        ability.Tier.Should().Be(AbilityTier.Hidden);
        ability.AbilityType.Should().Be(AbilityType.Hidden);
        ability.ParentAbilityElementId.Should().BeNull();

        AssertAbathurSymbioteAbility(ability);
    }

    [TestMethod]
    public void GetAbility_AbathurEvolveMonstrosityActiveSymbioteAbil_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        string abilArrayLinkValue = stormElement.DataValues.GetElementDataAt("AbilArray").GetElementDataAt("25").GetElementDataAt("Link").Value.GetString();

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(abilArrayLinkValue);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AbathurEvolveMonstrosityActiveSymbiote|EvolveMonstrosityActiveHotbar|Hidden");
        ability.Tier.Should().Be(AbilityTier.Hidden);
        ability.AbilityType.Should().Be(AbilityType.Hidden);
        ability.ParentAbilityElementId.Should().Be("AbathurEvolveMonstrosity");
    }

    [TestMethod]
    public void GetAbility_AbathurAssumingDirectControlCancelCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "AbathurSymbiote";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("0");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AbathurAssumingDirectControlCancel|AbathurSymbioteCancel|Heroic");
        ability.Name!.RawDescription.Should().Be("Cancel Symbiote");
        ability.Tier.Should().Be(AbilityTier.Heroic);
        ability.AbilityType.Should().Be(AbilityType.Heroic);
        ability.Icon.Should().Be("hud_btn_bg_ability_cancel.png");
        ability.IconPath!.FilePath.Should().NotBeNullOrWhiteSpace();
        ability.IconPath.MpqFilePath.Should().BeNull();
        ability.IsActive.Should().BeTrue();
        ability.FullText!.RawDescription.Should().Be("Cancels the Symbiote ability.");
        ability.ShortText.Should().BeNull();
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 1.5 seconds");
        ability.ToggleCooldown.Should().BeNull();
        ability.SummonedUnitIds.Should().BeEmpty();
    }

    [TestMethod]
    public void GetAbility_NoFaceCommand_ReturnsNull()
    {
        // arrange
        string heroUnit = "AbathurSymbiote";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("2");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

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

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("Hearthstone|HearthstoneNoMana|B");
        ability.Name!.RawDescription.Should().Be("Hearthstone");
        ability.AbilityElementId.Should().Be("Hearthstone");
        ability.ButtonElementId.Should().Be("HearthstoneNoMana");
        ability.Icon.Should().Be("storm_ui_icon_miscrune_1.png");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Hearth);
        ability.AbilityType.Should().Be(AbilityType.B);
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText.Should().BeNull();
        ability.Charges.Should().BeNull();
        ability.LifeText.Should().BeNull();
        ability.EnergyText.Should().BeNull();
        ability.ShortText.Should().BeNull();
        ability.FullText!.RawDescription.Should().Be("After Channeling for...");
    }

    [TestMethod]
    public void GetAbility_TeaseCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("13");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("stop|Tease|Taunt");
        ability.Name!.RawDescription.Should().Be("Taunt");
        ability.AbilityElementId.Should().Be("stop");
        ability.ButtonElementId.Should().Be("Tease");
        ability.Icon.Should().Be("btn-command-stop.png");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Taunt);
        ability.AbilityType.Should().Be(AbilityType.Taunt);
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText.Should().BeNull();
        ability.Charges.Should().BeNull();
        ability.LifeText.Should().BeNull();
        ability.EnergyText.Should().BeNull();
        ability.ShortText.Should().BeNull();
        ability.FullText.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_DanceCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("14");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("stop|Dance|Dance");
        ability.Name!.RawDescription.Should().Be("Dance");
        ability.AbilityElementId.Should().Be("stop");
        ability.ButtonElementId.Should().Be("Dance");
        ability.Icon.Should().Be("btn-command-stop.png");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Dance);
        ability.AbilityType.Should().Be(AbilityType.Dance);
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText.Should().BeNull();
        ability.Charges.Should().BeNull();
        ability.LifeText.Should().BeNull();
        ability.EnergyText.Should().BeNull();
        ability.ShortText.Should().BeNull();
        ability.FullText.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_LootSprayCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("20");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("LootSpray|LootSpray|Spray");
        ability.Name!.RawDescription.Should().Be("Quick Spray Expression");
        ability.AbilityElementId.Should().Be("LootSpray");
        ability.ButtonElementId.Should().Be("LootSpray");
        ability.Icon.Should().Be("storm_temp_war3_btnhealingspray.png");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Spray);
        ability.AbilityType.Should().Be(AbilityType.Spray);
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 3 seconds");
        ability.Charges.Should().BeNull();
        ability.LifeText.Should().BeNull();
        ability.EnergyText.Should().BeNull();
        ability.ShortText.Should().BeNull();
        ability.FullText!.RawDescription.Should().Be("Express yourself to other players by marking the ground with your selected spray.");
    }

    [TestMethod]
    public void GetAbility_LootYellVoiceLineCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("21");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("LootYellVoiceLine|LootYellVoiceLine|Voice");
        ability.Name!.RawDescription.Should().Be("Quick Voice Line Expression");
        ability.AbilityElementId.Should().Be("LootYellVoiceLine");
        ability.ButtonElementId.Should().Be("LootYellVoiceLine");
        ability.Icon.Should().Be("storm_btn_d3_barbarian_threateningshout.png");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Voice);
        ability.AbilityType.Should().Be(AbilityType.Voice);
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 7 seconds");
        ability.Charges.Should().BeNull();
        ability.LifeText.Should().BeNull();
        ability.EnergyText.Should().BeNull();
        ability.ShortText.Should().BeNull();
        ability.FullText!.RawDescription.Should().Be("Express yourself to other players by playing your selected Voice Line.");
    }

    [TestMethod]
    public void GetAbility_AlarakDeadlyChargeCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlarak";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("26");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AlarakDeadlyChargeActivate|AlarakDeadlyCharge|Heroic");
        ability.Name!.RawDescription.Should().Be("Deadly Charge");
        ability.AbilityElementId.Should().Be("AlarakDeadlyChargeActivate");
        ability.ButtonElementId.Should().Be("AlarakDeadlyCharge");
        ability.Icon.Should().Be("storm_ui_icon_alarak_recklesscharge.png");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Heroic);
        ability.AbilityType.Should().Be(AbilityType.Heroic);
        ability.ToggleCooldown.Should().Be(0.5);
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 45 seconds");
        ability.Charges.Should().BeNull();
        ability.LifeText.Should().BeNull();
        ability.EnergyText!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Mana: 60</s>");
        ability.ShortText!.RawDescription.Should().Be("Channel to charge a long distance");
        ability.FullText!.RawDescription.Should().Be("After channeling, Alarak charges forward...");
    }

    [TestMethod]
    public void GetAbility_AlarakSadismCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlarak";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("35");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be($"{AbilityTalentParserBase.PassiveAbilityElementId}|AlarakSadism|Trait");
        ability.Name!.RawDescription.Should().Be("Sadism");
        ability.AbilityElementId.Should().Be(AbilityTalentParserBase.PassiveAbilityElementId);
        ability.ButtonElementId.Should().Be("AlarakSadism");
        ability.Icon.Should().Be("storm_ui_icon_alarak_sadism.png");
        ability.IsActive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText.Should().BeNull();
        ability.Charges.Should().BeNull();
        ability.LifeText.Should().BeNull();
        ability.EnergyText.Should().BeNull();
        ability.ShortText!.RawDescription.Should().Be("Each point of Sadism increases Alarak's Ability damage...");
        ability.FullText!.RawDescription.Should().Be("Alarak's Ability damage and self-healing are increased...");
    }

    [TestMethod]
    public void GetAbility_AlarakLightningSurgeLightningBarrageCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlarak";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("30");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AlarakLightningSurgeLightningBarrage|AlarakLightningSurgeLightningBarrage|E");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.E);
        ability.ToggleCooldown.Should().Be(0.125);
        ability.CooldownText.Should().BeNull();
        ability.Charges.Should().BeNull();
        ability.LifeText.Should().BeNull();
        ability.EnergyText.Should().BeNull();
        ability.ToggleCooldown.Should().Be(0.125);
        ability.ParentAbilityElementId.Should().Be("AlarakLightningSurge");
    }

    [TestMethod]
    public void GetAbility_AlarakDeadlyChargeExecute2ndHeroicCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlarak";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("32");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AlarakDeadlyChargeExecute2ndHeroic|AlarakUnleashDeadlyCharge|Trait");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.ToggleCooldown.Should().Be(0.25);
        ability.ParentAbilityElementId.Should().Be("AlarakDeadlyChargeActivate2ndHeroic"); // extra
    }

    [TestMethod]
    public void GetAbility_AlarakLightningSurgeLightningBarrageAbil_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlarak";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        string abilArrayLinkValue = stormElement.DataValues.GetElementDataAt("AbilArray").GetElementDataAt("27").GetElementDataAt("Link").Value.GetString();

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(abilArrayLinkValue);

        // assert
        ability.Should().NotBeNull();
        ability.IsActive.Should().BeTrue();
        ability.ToggleCooldown.Should().Be(0.125);
        ability.Charges.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_AlexstraszaGiftOfLifeCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlexstrasza";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("25");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AlexstraszaGiftOfLife|AlexstraszaGiftOfLife|Q");
        ability.Name!.RawDescription.Should().Be("Gift of Life");
        ability.AbilityElementId.Should().Be("AlexstraszaGiftOfLife");
        ability.ButtonElementId.Should().Be("AlexstraszaGiftOfLife");
        ability.Icon.Should().Be("storm_ui_icon_alexstrasza_gift_of_life.png");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.Q);
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 7 seconds");
        ability.Charges.Should().BeNull();
        ability.LifeText!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Health: </s><s val=\"StandardTooltipDetails\">15%</s>");
        ability.EnergyText.Should().BeNull();
        ability.ShortText!.RawDescription.Should().Be("Give a portion of Health to an allied Hero");
        ability.FullText!.RawDescription.Should().Be("Sacrifice...");
    }

    [TestMethod]
    public void GetAbility_AlexstraszaAbundanceCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAlexstrasza";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("26");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AlexstraszaAbundance|AlexstraszaAbundance|W");
        ability.Name!.RawDescription.Should().Be("Abundance");
        ability.AbilityElementId.Should().Be("AlexstraszaAbundance");
        ability.ButtonElementId.Should().Be("AlexstraszaAbundance");
        ability.Icon.Should().Be("storm_ui_icon_alexstrasza_abundance.png");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.W);
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 14 seconds");
        ability.Charges.Should().BeNull();
        ability.LifeText.Should().BeNull();
        ability.EnergyText!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Mana: 75</s>");
        ability.ShortText!.RawDescription.Should().Be("Heal allied Heroes in an area");
        ability.FullText!.RawDescription.Should().Be("Plant a seed of healing that blooms after...");
    }

    [TestMethod]
    public void GetAbility_GuldanLifeTapCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroGuldan";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("32");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("GuldanLifeTap|GuldanLifeTap|Trait");
        ability.Name!.RawDescription.Should().Be("Life Tap");
        ability.AbilityElementId.Should().Be("GuldanLifeTap");
        ability.ButtonElementId.Should().Be("GuldanLifeTap");
        ability.Icon.Should().Be("storm_ui_icon_guldan_lifetap.png");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.ToggleCooldown.Should().Be(0.5);
        ability.CooldownText.Should().BeNull();
        ability.Charges.Should().BeNull();
        ability.LifeText!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Life: </s><s val=\"StandardTooltipDetails\">222</s>");
        ability.EnergyText.Should().BeNull();
        ability.ShortText!.RawDescription.Should().Be("Restore Mana at the cost of Health");
        ability.FullText!.RawDescription.Should().Be("Gul'dan does not regenerate Mana...");
    }

    [TestMethod]
    public void GetAbility_BarbarianSeismicSlamCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroBarbarian";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("29");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("BarbarianSeismicSlam|BarbarianSeismicSlam|W");
        ability.Name!.RawDescription.Should().Be("Seismic Slam");
        ability.AbilityElementId.Should().Be("BarbarianSeismicSlam");
        ability.ButtonElementId.Should().Be("BarbarianSeismicSlam");
        ability.Icon.Should().Be("storm_ui_icon_sonya_seismicslam.png");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Basic);
        ability.AbilityType.Should().Be(AbilityType.W);
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 1 second");
        ability.Charges.Should().BeNull();
        ability.LifeText.Should().BeNull();
        ability.EnergyText!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Fury: 25</s>");
        ability.ShortText!.RawDescription.Should().Be("Damage an enemy and splash damage behind them");
        ability.FullText!.RawDescription.Should().Be("Deals deals damage to...");
    }

    [TestMethod]
    public void GetAbility_UseVehicleAbil_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        string abilArrayLinkValue = stormElement.DataValues.GetElementDataAt("AbilArray").GetElementDataAt("4").GetElementDataAt("Link").Value.GetString();

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(abilArrayLinkValue);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("UseVehicle|UseVehicle|Hidden");
        ability.Name!.RawDescription.Should().Be("Use Vehicle");
        ability.Tier.Should().Be(AbilityTier.Hidden);
        ability.AbilityType.Should().Be(AbilityType.Hidden);
        ability.IsActive.Should().BeTrue();
        ability.Icon.Should().Be("storm_temp_war3_btnloaddwarf.png");
        ability.CooldownText.Should().BeNull();
        ability.ShortText.Should().BeNull();
        ability.FullText.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_MountCabooseSmartCommandUnitInteractionAbil_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAbathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        string abilArrayLinkValue = stormElement.DataValues.GetElementDataAt("AbilArray").GetElementDataAt("10").GetElementDataAt("Link").Value.GetString();

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(abilArrayLinkValue);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("MountCabooseSmartCommandUnitInteraction|MountCabooseSmartCommandUnitInteraction|Hidden");
        ability.Name.Should().BeNull();
        ability.Tier.Should().Be(AbilityTier.Hidden);
        ability.AbilityType.Should().Be(AbilityType.Hidden);
        ability.IsActive.Should().BeTrue();
        ability.Icon.Should().Be("storm_ui_temp_icon_cheatdeath.png");
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 4 seconds");
        ability.ShortText.Should().BeNull();
        ability.FullText.Should().BeNull();
    }

    [TestMethod]
    public void GetAbility_KelThuzadMasterOfTheColdDarkCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroKelThuzad";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("31");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be($"{AbilityTalentParserBase.PassiveAbilityElementId}|KelThuzadMasterOfTheColdDark|Trait");
        ability.IsActive.Should().BeFalse();
    }

    [TestMethod]
    public void GetAbility_SamuroIllusionMasterHeroicCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroSamuro";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("29");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("SamuroIllusionMaster|SamuroIllusionMaster|Heroic");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Heroic);
        ability.AbilityType.Should().Be(AbilityType.Heroic);
        ability.ShortText!.RawDescription.Should().Be("Mirror Images can be controlled");
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 8 seconds");
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

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("SamuroIllusionMaster|SamuroAdvancingStrikes|Trait");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.ShortText!.RawDescription.Should().Be("Increase Movement Speed when attacking Heroes");
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 14 seconds");
        ability.Icon.Should().Be("storm_ui_icon_samuro_flowingstrikes.png");
        ability.TooltipAppendersTalentElementIds.Should().HaveCount(3).And
            .Contain(["SamuroAdvancingStrikesDeflection", "SamuroPressTheAttack", "SamuroBlademastersPursuit"]);
    }

    [TestMethod]
    public void GetAbility_AnaEyeOfHorusActivateCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAna";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("28");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AnaEyeOfHorusActivate|AnaEyeOfHorusActivate|Heroic");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Heroic);
        ability.AbilityType.Should().Be(AbilityType.Heroic);
        ability.ToggleCooldown.Should().BeNull();
        ability.Charges!.CountMax.Should().Be(6);
        ability.Charges.CountStart.Should().Be(6);
        ability.Charges.CountUse.Should().Be(0);
        ability.Charges.HasCharges.Should().BeTrue();
        ability.Charges.IsCountHidden.Should().BeTrue();
        ability.Charges.RecastCooldown.Should().BeNull();
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 60 seconds");
    }

    [TestMethod]
    public void GetAbility_AnaEyeOfHorusAttackDummyCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroAna";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("26");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("AnaEyeOfHorusAttackDummy|AnaEyeOfHorusHighPoweredRound|Heroic");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Heroic);
        ability.AbilityType.Should().Be(AbilityType.Heroic);
        ability.ToggleCooldown.Should().BeNull();
        ability.Charges!.CountMax.Should().Be(6);
        ability.Charges.CountStart.Should().Be(6);
        ability.Charges.CountUse.Should().Be(1);
        ability.Charges.HasCharges.Should().BeTrue();
        ability.Charges.IsCountHidden.Should().BeFalse();
        ability.Charges.RecastCooldown.Should().Be(1);
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 1 second");
    }

    [TestMethod]
    public void GetAbility_DehakaEssenceCollectionCommand_ReturnsAbility()
    {
        // arrange
        string heroUnit = "HeroDehaka";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", heroUnit)!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("29");

        AbilityParser abilityParser = new(_logger, _options, _heroesXmlLoaderService, _tooltipDescriptionService);

        // act
        Ability? ability = abilityParser.GetAbility(layoutButtons);

        // assert
        ability.Should().NotBeNull();
        ability.LinkId.ToString().Should().Be("DehakaEssenceCollection|DehakaEssenceCollection|Trait");
        ability.IsActive.Should().BeTrue();
        ability.Tier.Should().Be(AbilityTier.Trait);
        ability.AbilityType.Should().Be(AbilityType.Trait);
        ability.ToggleCooldown.Should().BeNull();
        ability.Charges!.CountMax.Should().Be(50);
        ability.Charges.CountStart.Should().BeNull();
        ability.Charges.CountUse.Should().BeNull();
        ability.Charges.HasCharges.Should().BeTrue();
        ability.Charges.IsCountHidden.Should().BeFalse();
        ability.Charges.RecastCooldown.Should().BeNull();
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 5 seconds");
    }

    private static void AssertAbathurSymbioteAbility(Ability ability)
    {
        ability.Name!.RawDescription.Should().Be("Symbiote");
        ability.AbilityElementId.Should().Be("AbathurSymbiote");
        ability.ButtonElementId.Should().Be("AbathurSymbiote");
        ability.Icon.Should().Be("storm_ui_icon_abathur_symbiote.png");
        ability.IsActive.Should().BeTrue();
        ability.ToggleCooldown.Should().BeNull();
        ability.CooldownText!.RawDescription.Should().Be("Cooldown: 4 seconds");
        ability.LifeText.Should().BeNull();
        ability.EnergyText.Should().BeNull();
        ability.ShortText!.RawDescription.Should().Be("Assist an ally and gain new abilities");
        ability.FullText!.RawDescription.Should().Be("Spawn and attach a Symbiote...");

        // base on the number of validators
        ability.TooltipAppendersTalentElementIds.Should().HaveCount(3).And
            .Contain(["AbathurMasteryPressurizedGlands", "AbathurReinforcedCarapace", "AbathurMasteryRegenerativeMicrobes"]);
    }
}