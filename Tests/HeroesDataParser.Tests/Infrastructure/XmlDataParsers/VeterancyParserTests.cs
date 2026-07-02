namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class VeterancyParserTests
{
    private readonly ILogger<VeterancyParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public VeterancyParserTests()
    {
        _logger = Substitute.For<ILogger<VeterancyParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_BaseDefenderTowerScaling_ReturnVeterancyData()
    {
        // arrange
        VeterancyParser veterancyParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Veterancy? veterancy = veterancyParser.Parse("BaseDefenderTowerScaling");

        // assert
        veterancy.Should().NotBeNull();
        veterancy.Id.Should().Be("BaseDefenderTowerScaling");
        veterancy.CombineModifications.Should().BeTrue();
        veterancy.CombineXP.Should().BeTrue();
        veterancy.VeterancyLevels.Should().HaveCount(61);
        veterancy.VeterancyLevels[0].MinimumVeterancyXP.Should().Be(0);
        veterancy.VeterancyLevels.Skip(1).Should().OnlyContain(x => x.MinimumVeterancyXP == 1);
        veterancy.VeterancyLevels[0].VeterancyModification.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification.Should().NotBeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.DamageDealtFraction.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.DamageDealtScaled!.Ability.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.DamageDealtScaled!.Basic.Should().Be(25);
        veterancy.VeterancyLevels[1].VeterancyModification!.DamageDealtScaled!.Splash.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.KillXpBonus.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalMaxValue.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalMaxFraction.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalRegeneration.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalRegenerationFraction.Should().BeNull();
        veterancy.VeterancyLevels[2].VeterancyModification!.KillXpBonus.Should().Be(2);
    }

    [TestMethod]
    public void Parse_SoulEaterMinionScaling_ReturnVeterancyData()
    {
        // arrange
        VeterancyParser veterancyParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Veterancy? veterancy = veterancyParser.Parse("SoulEaterMinionScaling");

        // assert
        veterancy.Should().NotBeNull();
        veterancy.Id.Should().Be("SoulEaterMinionScaling");
        veterancy.CombineModifications.Should().BeTrue();
        veterancy.CombineXP.Should().BeTrue();
        veterancy.VeterancyLevels[0].MinimumVeterancyXP.Should().Be(0);
        veterancy.VeterancyLevels[1].VeterancyModification.Should().NotBeNull();
        veterancy.VeterancyLevels[1].MinimumVeterancyXP.Should().Be(1);
        veterancy.VeterancyLevels[1].VeterancyModification!.DamageDealtFraction.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.DamageDealtScaled!.Ability.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.DamageDealtScaled!.Basic.Should().Be(4);
        veterancy.VeterancyLevels[1].VeterancyModification!.DamageDealtScaled!.Splash.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.KillXpBonus.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalMaxValue!.Life.Should().Be(10);
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalMaxFraction.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalRegeneration.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalRegenerationFraction.Should().BeNull();
    }

    [TestMethod]
    public void Parse_BioticEmitterScaling_ReturnVeterancyData()
    {
        // arrange
        VeterancyParser veterancyParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Veterancy? veterancy = veterancyParser.Parse("BioticEmitterScaling");

        // assert
        veterancy.Should().NotBeNull();
        veterancy.Id.Should().Be("BioticEmitterScaling");
        veterancy.CombineModifications.Should().BeTrue();
        veterancy.CombineXP.Should().BeTrue();
        veterancy.VeterancyLevels[0].MinimumVeterancyXP.Should().Be(0);
        veterancy.VeterancyLevels[1].VeterancyModification.Should().NotBeNull();
        veterancy.VeterancyLevels[1].MinimumVeterancyXP.Should().Be(1);
        veterancy.VeterancyLevels[1].VeterancyModification!.DamageDealtFraction.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.DamageDealtScaled.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.KillXpBonus.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalMaxValue.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalMaxFraction!.Life.Should().Be(0.05);
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalRegeneration.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalRegenerationFraction.Should().BeNull();
    }

    [TestMethod]
    public void Parse_ExcellentMana_ReturnVeterancyData()
    {
        // arrange
        VeterancyParser veterancyParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Veterancy? veterancy = veterancyParser.Parse("ExcellentMana");

        // assert
        veterancy.Should().NotBeNull();
        veterancy.Id.Should().Be("ExcellentMana");
        veterancy.CombineModifications.Should().BeTrue();
        veterancy.CombineXP.Should().BeTrue();
        veterancy.VeterancyLevels[1].VeterancyModification.Should().NotBeNull();
        veterancy.VeterancyLevels[1].MinimumVeterancyXP.Should().Be(2010);
        veterancy.VeterancyLevels[1].VeterancyModification!.DamageDealtFraction.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.DamageDealtScaled.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.KillXpBonus.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalMaxValue!.Energy.Should().Be(10);
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalMaxFraction.Should().BeNull();
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalRegeneration!.Energy.Should().Be(0.0976);
        veterancy.VeterancyLevels[1].VeterancyModification!.VitalRegenerationFraction.Should().BeNull();
    }

    [TestMethod]
    public void Parse_TestAllModifications_ReturnVeterancyData()
    {
        // arrange
        VeterancyParser veterancyParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Veterancy? veterancy = veterancyParser.Parse("TestAllModifications");

        // assert
        veterancy.Should().NotBeNull();
        veterancy.Id.Should().Be("TestAllModifications");
        veterancy.VeterancyLevels[0].VeterancyModification.Should().NotBeNull();
        veterancy.VeterancyLevels[0].VeterancyModification!.DamageDealtScaled!.Basic.Should().Be(25);
        veterancy.VeterancyLevels[0].VeterancyModification!.DamageDealtScaled!.Ability.Should().Be(35);
        veterancy.VeterancyLevels[0].VeterancyModification!.DamageDealtScaled!.Splash.Should().Be(45);
        veterancy.VeterancyLevels[0].VeterancyModification!.DamageDealtFraction!.Basic.Should().Be(0.55);
        veterancy.VeterancyLevels[0].VeterancyModification!.DamageDealtFraction!.Ability.Should().Be(0.65);
        veterancy.VeterancyLevels[0].VeterancyModification!.DamageDealtFraction!.Splash.Should().Be(0.75);
        veterancy.VeterancyLevels[0].VeterancyModification!.VitalMaxValue!.Life.Should().Be(10);
        veterancy.VeterancyLevels[0].VeterancyModification!.VitalMaxValue!.Energy.Should().Be(20);
        veterancy.VeterancyLevels[0].VeterancyModification!.VitalMaxValue!.Shield.Should().Be(30);
        veterancy.VeterancyLevels[0].VeterancyModification!.VitalMaxFraction!.Life.Should().Be(0.05);
        veterancy.VeterancyLevels[0].VeterancyModification!.VitalMaxFraction!.Energy.Should().Be(0.15);
        veterancy.VeterancyLevels[0].VeterancyModification!.VitalMaxFraction!.Shield.Should().Be(0.25);
        veterancy.VeterancyLevels[0].VeterancyModification!.VitalRegeneration!.Life.Should().Be(0.0976);
        veterancy.VeterancyLevels[0].VeterancyModification!.VitalRegeneration!.Energy.Should().Be(0.1976);
        veterancy.VeterancyLevels[0].VeterancyModification!.VitalRegeneration!.Shield.Should().Be(0.2976);
        veterancy.VeterancyLevels[0].VeterancyModification!.VitalRegenerationFraction!.Life.Should().Be(1.0976);
        veterancy.VeterancyLevels[0].VeterancyModification!.VitalRegenerationFraction!.Energy.Should().Be(2.1976);
        veterancy.VeterancyLevels[0].VeterancyModification!.VitalRegenerationFraction!.Shield.Should().Be(3.2976);
    }
}
