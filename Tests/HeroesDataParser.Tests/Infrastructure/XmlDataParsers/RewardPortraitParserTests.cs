namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class RewardPortraitParserTests
{
    private readonly ILogger<RewardPortraitParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public RewardPortraitParserTests()
    {
        _logger = Substitute.For<ILogger<RewardPortraitParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_SylvanasBasic_ReturnsRewardPortraitData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("Reward/Description/HeroPortraitGeneric").Returns(new GameStringText("Unlocks this Hero's Portrait in your Collection"));
        _gameStringTextService.GetGameStringTextFromId("Reward/DescriptionUnearned/HeroPortraitGeneric").Returns(new GameStringText("You have unlocked this Hero's Portrait in your Collection"));

        RewardPortraitParser rewardPortraitParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        RewardPortrait? rewardPortrait = rewardPortraitParser.Parse("SylvanasBasic");

        // assert
        rewardPortrait.Should().NotBeNull();
        rewardPortrait.Id.Should().Be("SylvanasBasic");
        rewardPortrait.HyperlinkId.Should().Be("SylvanasBasic");
        rewardPortrait.Category.Should().Be("PortraitProgression1");
        rewardPortrait.HeroId.Should().Be("Sylvanas");
        rewardPortrait.Rarity.Should().Be(Rarity.Common);
        rewardPortrait.Description!.RawText.Should().Be("Unlocks this Hero's Portrait in your Collection");
        rewardPortrait.DescriptionUnearned!.RawText.Should().Be("You have unlocked this Hero's Portrait in your Collection");
        rewardPortrait.TextureSheet.Image.Should().Be("ui_heroes_portraits_sheet3.png");
        rewardPortrait.TextureSheet.Columns.Should().Be(6);
        rewardPortrait.TextureSheet.Rows.Should().Be(6);
        rewardPortrait.Image.Should().Be("storm_portrait_sylvanasbasic.png");
        rewardPortrait.IconSlot.Should().Be(7);
        rewardPortrait.PortraitPackId.Should().BeNull();
    }

    [TestMethod]
    public void Parse_AbathurToys18Portrait_ReturnsRewardPortraitData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("Reward/Description/LootBoxPortraitGeneric").Returns(new GameStringText("Unlocks this Hero's Portrait in your Collection"));
        _gameStringTextService.GetGameStringTextFromId("Reward/DescriptionUnearned/LootBoxPortraitGeneric").Returns(new GameStringText("You have unlocked this Hero's Portrait in your Collection"));

        RewardPortraitParser rewardPortraitParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        RewardPortrait? rewardPortrait = rewardPortraitParser.Parse("AbathurToys18Portrait");

        // assert
        rewardPortrait.Should().NotBeNull();
        rewardPortrait.Id.Should().Be("AbathurToys18Portrait");
        rewardPortrait.HyperlinkId.Should().Be("CaterpillathurPortrait");
        rewardPortrait.Category.Should().Be("SeasonalEvents");
        rewardPortrait.HeroId.Should().BeNull();
        rewardPortrait.Rarity.Should().Be(Rarity.Common);
        rewardPortrait.Description!.RawText.Should().Be("Unlocks this Hero's Portrait in your Collection");
        rewardPortrait.DescriptionUnearned!.RawText.Should().Be("You have unlocked this Hero's Portrait in your Collection");
        rewardPortrait.TextureSheet.Image.Should().Be("ui_heroes_portraits_sheet33.png");
        rewardPortrait.TextureSheet.Columns.Should().Be(6);
        rewardPortrait.TextureSheet.Rows.Should().Be(6);
        rewardPortrait.Image.Should().Be("storm_portrait_abathurtoys18portrait.png");
        rewardPortrait.IconSlot.Should().Be(15);
        rewardPortrait.PortraitPackId.Should().Be("AbathurToys18Portrait");
    }
}
