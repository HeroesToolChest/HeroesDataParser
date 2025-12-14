namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class LootChestParserTests
{
    private readonly ILogger<LootChestParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public LootChestParserTests()
    {
        _logger = Substitute.For<ILogger<LootChestParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_AlarakChest_ReturnsLootChestData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("LootChest/Name/AlarakChest").Returns(new GameStringText("name"));

        LootChestParser lootChestParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        LootChest? lootChest = lootChestParser.Parse("AlarakChest");

        // assert
        lootChest.Should().NotBeNull();
        lootChest.HyperlinkId.Should().Be("AlarakChest");
        lootChest.Id.Should().Be("AlarakChest");
        lootChest.Description.Should().BeNull();
        lootChest.Event.Should().BeNull();
        lootChest.MaxRerolls.Should().Be(3);
        lootChest.Name!.RawText.Should().Be("name");
        lootChest.Rarity.Should().Be(Rarity.Common);
        lootChest.TypeDescriptionId.Should().Be("LootChestCommon");
    }

    [TestMethod]
    public void Parse_Halloween2018EpicLootChest_ReturnsLootChestData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("LootChest/Name/Halloween2018EpicLootChest").Returns(new GameStringText("name"));

        LootChestParser lootChestParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        LootChest? lootChest = lootChestParser.Parse("Halloween2018EpicLootChest");

        // assert
        lootChest.Should().NotBeNull();
        lootChest.HyperlinkId.Should().Be("Halloween2018EpicLootChest");
        lootChest.Id.Should().Be("Halloween2018EpicLootChest");
        lootChest.Description.Should().BeNull();
        lootChest.Event.Should().Be("Halloween18");
        lootChest.MaxRerolls.Should().Be(5);
        lootChest.Name!.RawText.Should().Be("name");
        lootChest.Rarity.Should().Be(Rarity.Epic);
        lootChest.TypeDescriptionId.Should().Be("Halloween2018EpicLootChest");
    }
}
