namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class MatchAwardParserTests
{
    private readonly ILogger<MatchAwardParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public MatchAwardParserTests()
    {
        _logger = Substitute.For<ILogger<MatchAwardParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_EndOfMatchAwardMostXPContributionBoolean_ReturnsMatchAwardData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("ScoreValue/Name/EndOfMatchAwardMostXPContributionBoolean").Returns(new GameStringText("<lang rule=\"gender\" word=\"%gender%\">Experienced,Experienced</lang>"));
        _gameStringTextService.GetGameStringTextFromId("ScoreValue/Tooltip/EndOfMatchAwardMostXPContributionBoolean").Returns(new GameStringText("High XP Contributed"));

        _gameStringTextService.GetGameStringTextFromId("UserData/EndOfMatchGeneralAward/Experienced_Award Name").Returns(new GameStringText("<lang rule=\"gender\" word=\"~HeroName~\">Experienced</lang>"));
        _gameStringTextService.GetGameStringTextFromId("UserData/EndOfMatchGeneralAward/Experienced_Description").Returns(new GameStringText("Contributed ~AwardValue~% of Team's XP"));
        _gameStringTextService.GetGameStringTextFromId("UserData/EndOfMatchGeneralAward/Experienced_Tooltip Text").Returns(new GameStringText("~TooltipAmount~ XP Contributed"));

        MatchAwardParser matchAwardParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        MatchAward? matchAward = matchAwardParser.Parse("EndOfMatchAwardMostXPContributionBoolean");

        // assert
        matchAward.Should().NotBeNull();

        matchAward.Id.Should().Be("MostXPContribution");
        matchAward.ScoreScreenName!.RawText.Should().Be("<lang rule=\"gender\" word=\"%gender%\">Experienced,Experienced</lang>");
        matchAward.ScoreScreenDescription!.RawText.Should().Be("High XP Contributed");
        matchAward.EndOfMatchName!.RawText.Should().Be("<lang rule=\"gender\" word=\"~HeroName~\">Experienced</lang>");
        matchAward.EndOfMatchDescription!.RawText.Should().Be("Contributed ~AwardValue~% of Team's XP");
        matchAward.EndOfMatchTooltipText!.RawText.Should().Be("~TooltipAmount~ XP Contributed");
        matchAward.GameLink.Should().Be("EndOfMatchAwardMostXPContributionBoolean");
        matchAward.MVPScreenImage.Should().Be("storm_ui_mvp_experienced_%color%.png");
        matchAward.MVPScreenImagePath!.FilePath.Should().Be(Path.Combine("Assets", "Textures", "storm_ui_mvp_icons_rewards_experienced.dds"));
        matchAward.ScoreScreenImage.Should().Be("storm_ui_scorescreen_mvp_experienced_%team%.png");
        matchAward.ScoreScreenImageBluePath!.FilePath.Should().Be(Path.Combine("Assets", "Textures", "storm_ui_scorescreen_mvp_experienced_blue.dds"));
        matchAward.ScoreScreenImageRedPath!.FilePath.Should().Be(Path.Combine("Assets", "Textures", "storm_ui_scorescreen_mvp_experienced_red.dds"));
    }

    [TestMethod]
    public void Parse_EndOfMatchAwardHighestKillStreakBoolean_ReturnsMatchAwardData()
    {
        // arrange
        MatchAwardParser matchAwardParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        MatchAward? matchAward = matchAwardParser.Parse("EndOfMatchAwardHighestKillStreakBoolean");

        // assert
        matchAward.Should().NotBeNull();
        matchAward.Id.Should().Be("HighestKillStreak");
        matchAward.GameLink.Should().Be("EndOfMatchAwardHighestKillStreakBoolean");
        matchAward.MVPScreenImage.Should().Be("storm_ui_mvp_dominator_%color%.png");
        matchAward.MVPScreenImagePath!.FilePath.Should().Be(Path.Combine("Assets", "Textures", "storm_ui_mvp_icons_rewards_dominator.dds"));
    }

    [TestMethod]
    public void Parse_EndOfMatchAwardMostAltarDamageDone_ReturnsMatchAwardData()
    {
        // arrange
        MatchAwardParser matchAwardParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        MatchAward? matchAward = matchAwardParser.Parse("EndOfMatchAwardMostAltarDamageDone");

        // assert
        matchAward.Should().NotBeNull();
        matchAward.Id.Should().Be("MostAltarDamage");
        matchAward.GameLink.Should().Be("EndOfMatchAwardMostAltarDamageDone");
    }

    [TestMethod]
    public void Parse_EndOfMatchAwardMostDragonShrinesCapturedBoolean_ReturnsMatchAwardData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("ScoreValue/Name/EndOfMatchAwardMostDragonShrinesCapturedBoolean").Returns(new GameStringText("<lang rule=\"gender\" word=\"%gender%\">Shriner,Shriner</lang>"));
        _gameStringTextService.GetGameStringTextFromId("ScoreValue/Tooltip/EndOfMatchAwardMostDragonShrinesCapturedBoolean").Returns(new GameStringText("High Shrines Captured"));

        _gameStringTextService.GetGameStringTextFromId("UserData/EndOfMatchMapSpecificAward/Shriner_Award Name").Returns(new GameStringText("<lang rule=\"gender\" word=\"~HeroName~\">Shriner</lang>"));
        _gameStringTextService.GetGameStringTextFromId("UserData/EndOfMatchMapSpecificAward/Shriner_Description").Returns(new GameStringText("~AwardValue~ Shrines Captured"));
        _gameStringTextService.GetGameStringTextFromId("UserData/EndOfMatchMapSpecificAward/Shriner_Tooltip Text").Returns(new GameStringText("~TooltipAmount~ Shrines Captured"));

        MatchAwardParser matchAwardParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        MatchAward? matchAward = matchAwardParser.Parse("EndOfMatchAwardMostDragonShrinesCapturedBoolean");

        // assert
        matchAward.Should().NotBeNull();
        matchAward.GameLink.Should().Be("EndOfMatchAwardMostDragonShrinesCapturedBoolean");
        matchAward.ScoreScreenName!.RawText.Should().Be("<lang rule=\"gender\" word=\"%gender%\">Shriner,Shriner</lang>");
        matchAward.ScoreScreenDescription!.RawText.Should().Be("High Shrines Captured");
        matchAward.EndOfMatchName!.RawText.Should().Be("<lang rule=\"gender\" word=\"~HeroName~\">Shriner</lang>");
        matchAward.EndOfMatchDescription!.RawText.Should().Be("~AwardValue~ Shrines Captured");
        matchAward.EndOfMatchTooltipText!.RawText.Should().Be("~TooltipAmount~ Shrines Captured");
    }
}
