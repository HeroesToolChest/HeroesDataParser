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
    public void GetTalent_AbathurMasteryPressurizedGlands_ReturnsAbility()
    {
        // arrange
        string heroUnit = "Abathur";

        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);

        StormElement stormElement = _heroesXmlLoader.HeroesData.GetCompleteStormElement("Hero", heroUnit)!;
        StormElementData talentTreeArray = stormElement.DataValues.GetElementDataAt("TalentTreeArray").GetElementDataAt("0");

        Hero hero = new(heroUnit);
        hero.AddAbility(new Ability()
        {
            ButtonId = "AbathurSymbiotePressurizedGlandsTalent",
            AbilityType = AbilityType.W,
        });

        TalentParser talentParser = new(_talentLogger, _heroesXmlLoaderService, new AbilityParser(_abilityLogger, _heroesXmlLoaderService));

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
}
