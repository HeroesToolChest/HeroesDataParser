namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class VoiceLineParserTests
{
    private readonly ILogger<VoiceLineParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public VoiceLineParserTests()
    {
        _logger = Substitute.For<ILogger<VoiceLineParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_AbathurBase_VoiceLine01_ReturnsVoiceLineData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("VoiceLine/Description/AbathurBase_VoiceLine01").Returns(new GameStringText("aba voiceline1"));

        VoiceLineParser voiceLineParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        VoiceLine? voiceLine = voiceLineParser.Parse("AbathurBase_VoiceLine01");

        // assert
        voiceLine.Should().NotBeNull();
        voiceLine.AttributeId.Should().Be("AB01");
        voiceLine.Category.Should().BeNull();
        voiceLine.Description!.RawText.Should().Be("aba voiceline1");
        voiceLine.HeroId.Should().Be("Abathur");
        voiceLine.HyperlinkId.Should().Be("AbathurVoiceLine01");
        voiceLine.Id.Should().Be("AbathurBase_VoiceLine01");
        voiceLine.Rarity.Should().Be(Rarity.Common);
        voiceLine.ReleaseDate.Should().Be(new DateOnly(2014, 3, 13));
        voiceLine.Image.Should().Be("storm_ui_voice_abathur.png");
    }
}
