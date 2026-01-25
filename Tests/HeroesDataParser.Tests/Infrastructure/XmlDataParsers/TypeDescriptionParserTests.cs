namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class TypeDescriptionParserTests
{
    private readonly ILogger<TypeDescriptionParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public TypeDescriptionParserTests()
    {
        _logger = Substitute.For<ILogger<TypeDescriptionParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_LootChestSummer2018Epic_ReturnsLootChestData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("TypeDescription/Name/LootChestSummer2018Epic").Returns(new GameStringText("name"));

        TypeDescriptionParser typeDescriptionParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        TypeDescription? typeDescription = typeDescriptionParser.Parse("LootChestSummer2018Epic");

        // assert
        typeDescription.Should().NotBeNull();
        typeDescription.Id.Should().Be("LootChestSummer2018Epic");
        typeDescription.Name!.RawText.Should().Be("name");
        typeDescription.IconSlot.Should().Be(37);
        typeDescription.TextureSheet.Columns.Should().Be(5);
        typeDescription.TextureSheet.Rows.Should().Be(12);
        typeDescription.TextureSheet.Image.Should().Be("Assets\\Textures\\storm_ui_heroes_rewardicons_sheet.dds");
        typeDescription.LargeIcon.Should().Be("storm_ui_profile_hero_progression_icon_epicsummerlootchest.png");
        typeDescription.LargeIconPath!.FilePath.Should().Be("Assets\\Textures\\storm_ui_profile_hero_progression_icon_epicsummerlootchest.dds");
        typeDescription.RewardIcon.Should().Be("storm_ui_heroes_reward_icon_lootchestsummer2018epic.png");
        typeDescription.RewardIconPath!.FilePath.Should().Be("Assets\\Textures\\storm_ui_heroes_rewardicons_sheet.dds");
    }
}
