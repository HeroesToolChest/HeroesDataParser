using Heroes.Element.Types;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers.Tests;

[TestClass]
public class AbilityParserTests
{
    private readonly ILogger<AbilityParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public AbilityParserTests()
    {
        _logger = Substitute.For<ILogger<AbilityParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
    }

    [TestMethod]
    public void GetAbility_AbathurSymbiote_ReturnsAbility()
    {
        // arrange
        HeroesXmlLoader heroesXmlLoader = GetArrangedHeroesXmlLoader();

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(heroesXmlLoader);

        StormElement stormElement = heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", "HeroAbathur")!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("27");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability ability = abilityParser.GetAbility("HeroAbathur", layoutButtons)!;

        // assert
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
        HeroesXmlLoader heroesXmlLoader = GetArrangedHeroesXmlLoader();

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(heroesXmlLoader);

        StormElement stormElement = heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", "HeroAbathur")!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("28");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability ability = abilityParser.GetAbility("HeroAbathur", layoutButtons)!;

        // assert
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
    public void GetAbility_AlarakDeadlyCharge_ReturnsAbility()
    {
        // arrange
        HeroesXmlLoader heroesXmlLoader = GetArrangedHeroesXmlLoader();

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(heroesXmlLoader);

        StormElement stormElement = heroesXmlLoader.HeroesData.GetCompleteStormElement("Unit", "HeroAlarak")!;
        StormElementData layoutButtons = stormElement.DataValues.GetElementDataAt("CardLayouts").GetElementDataAt("0").GetElementDataAt("LayoutButtons").GetElementDataAt("26");

        AbilityParser abilityParser = new(_logger, _heroesXmlLoaderService);

        // act
        Ability ability = abilityParser.GetAbility("HeroAlarak", layoutButtons)!;

        // assert
        ability.Name!.RawDescription.Should().Be("Deadly Charge");
        ability.NameId.Should().Be("AlarakDeadlyChargeActivate");
        ability.ButtonId.Should().Be("AlarakDeadlyCharge");
        ability.Icon.Should().Be("storm_ui_icon_alarak_recklesscharge.png");
        ability.IsActive.Should().BeTrue();
        ability.IsPassive.Should().BeFalse();
        ability.Tier.Should().Be(AbilityTier.Heroic);
        ability.AbilityType.Should().Be(AbilityType.Heroic);
        ability.ToggleCooldown.Should().BeNull();
        ability.Tooltip.CooldownTooltip!.RawDescription.Should().Be("Cooldown: 45 seconds");
        ability.Tooltip.Charges.Should().BeNull();
        ability.Tooltip.LifeTooltip.Should().BeNull();
        ability.Tooltip.EnergyTooltip!.RawDescription.Should().Be("<s val=\"StandardTooltipDetails\">Mana: 60</s>");
        ability.Tooltip.ShortTooltip!.RawDescription.Should().Be("Channel to charge a long distance");
        ability.Tooltip.FullTooltip!.RawDescription.Should().Be("After channeling, Alarak charges forward...");
    }

    private static XDocument NewMethod(string file)
    {
        return XDocument.Load(File.OpenRead(Path.Join("Infrastructure", "XmlTestFiles", file)));
    }

    private HeroesXmlLoader GetArrangedHeroesXmlLoader()
    {
        XDocument unitDocument = NewMethod("Unit.xml");
        XDocument buttonDocument = NewMethod("Button.xml");
        XDocument abilEffectTargetDocument = NewMethod("AbilEffectTarget.xml");

        return HeroesXmlLoader.LoadWithEmpty()
            .LoadCustomMod(new ManualModLoader("test")
                .AddBaseElementTypes([
                    ("abil", "CAbilEffectTarget"),
                    ("Unit", "CUnit"),
                    ("Button", "CButton"),
                ])
                .AddElements(unitDocument.Root!.Elements())
                .AddElements(buttonDocument.Root!.Elements())
                .AddElements(abilEffectTargetDocument.Root!.Elements())
                .AddGameStrings(
                    [
                        "e_gameUIStringChargeCooldownColon=Charge Cooldown: ",
                        "e_gameUIStringCooldownColon=Cooldown: ",
                        "UI/AbilTooltipCooldown=Cooldown: %1 second",
                        "UI/AbilTooltipCooldownPlural=Cooldown: %1 seconds",
                        "Abil/Name/AbathurSymbiote=Symbiote",
                        "Abil/Name/AbathurToxicNest=Toxic Nest",
                        "Abil/AlarakDeadlyChargeButtonVitalCostOverride=60",
                        "Abil/AlarakDeadlyChargeButtonCooldownCostOverride=45 seconds",
                        "Button/Name/AbathurSymbiote=Symbiote",
                        "Button/Name/AbathurToxicNest=Toxic Nest",
                        "Button/Name/AlarakDeadlyCharge=Deadly Charge",
                        "Button/SimpleDisplayText/AbathurSymbiote=Assist an ally and gain new abilities",
                        "Button/SimpleDisplayText/AbathurToxicNest=Spawn a mine",
                        "Button/SimpleDisplayText/AlarakDeadlyCharge=Channel to charge a long distance",
                        "Button/Tooltip/AbathurSymbiote=Spawn and attach a Symbiote...",
                        "Button/Tooltip/AbathurToxicNest=Spawn a mine that becomes active...",
                        "Button/Tooltip/AlarakDeadlyCharge=After channeling, Alarak charges forward...",
                        "UI/Tooltip/Abil/Mana=<s val=\"StandardTooltipDetails\">Mana: %1</s>",
                    ],
                    StormLocale.ENUS)
                .AddAssetFilePaths([
                    Path.Join("Assets", "Textures", "storm_ui_icon_abathur_symbiote.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_abathur_toxicnest.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_alarak_recklesscharge.dds"),
                    ]))
            .LoadGameStrings();
    }
}