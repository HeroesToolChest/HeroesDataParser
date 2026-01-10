namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class SprayParserTests
{
    private readonly ILogger<SprayParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public SprayParserTests()
    {
        _logger = Substitute.For<ILogger<SprayParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_SprayAnimatedCraft20LowBattery_ReturnsSprayData()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            Hidden =
            {
                 AnimatedImageType = AnimatedImageType.APNG,
            },
        });

        _gameStringTextService.GetGameStringTextFromId("Spray/Name/SprayAnimatedCraft20LowBattery").Returns(new GameStringText("Low Battery"));

        SprayParser sprayParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Spray? spray = sprayParser.Parse("SprayAnimatedCraft20LowBattery");

        // assert
        spray.Should().NotBeNull();
        spray.Name!.RawText.Should().Be("Low Battery");
        spray.AttributeId.Should().Be("SY8O");
        spray.Category.Should().Be("SeasonalEvents");
        spray.Description.Should().BeNull();
        spray.HyperlinkId.Should().Be("LowBattery");
        spray.Id.Should().Be("SprayAnimatedCraft20LowBattery");
        spray.Rarity.Should().Be(Rarity.Rare);
        spray.ReleaseDate.Should().Be(new DateOnly(2020, 9, 8));
        spray.Image.Should().Be("storm_lootspray_animated_craft20_lowbattery.apng");
        spray.Animation!.Duration.Should().Be(2000);
        spray.Animation.Frames.Should().Be(2);
        spray.Animation.Texture.Should().Be("storm_lootspray_animated_craft20_lowbattery.png");
    }

    [TestMethod]
    public void Parse_SprayAnimatedCraft20LowBatteryAsGif_ReturnsSprayData()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            Hidden =
            {
                 AnimatedImageType = AnimatedImageType.GIF,
            },
        });

        SprayParser sprayParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Spray? spray = sprayParser.Parse("SprayAnimatedCraft20LowBattery");

        // assert
        spray.Should().NotBeNull();
        spray.Image.Should().Be("storm_lootspray_animated_craft20_lowbattery.gif");
        spray.Animation!.Texture.Should().Be("storm_lootspray_animated_craft20_lowbattery.png");
    }

    [TestMethod]
    public void Parse_Classic21Diablospray_ReturnsSprayData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("Spray/Name/Classic21Diablospray").Returns(new GameStringText("Battle Ready Diablo"));

        SprayParser sprayParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Spray? spray = sprayParser.Parse("Classic21Diablospray");

        // assert
        spray.Should().NotBeNull();
        spray.Name!.RawText.Should().Be("Battle Ready Diablo");
        spray.AttributeId.Should().Be("SY9D");
        spray.Category.Should().Be("Default");
        spray.Description.Should().BeNull();
        spray.HyperlinkId.Should().Be("Classic21Diablospray");
        spray.Id.Should().Be("Classic21Diablospray");
        spray.Rarity.Should().Be(Rarity.Common);
        spray.ReleaseDate.Should().Be(new DateOnly(2021, 11, 9));
        spray.Image.Should().Be("classic21_diablo_spray.png");
        spray.Animation.Should().BeNull();
    }
}
