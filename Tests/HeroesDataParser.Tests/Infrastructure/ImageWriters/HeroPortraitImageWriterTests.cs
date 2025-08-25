using HeroesDataParser.Tests.Infrastructure.ImageWriters;

namespace HeroesDataParser.Infrastructure.ImageWriters.Tests;

[TestClass]
public class HeroPortraitImageWriterTests : ImageWriterBase
{
    private readonly ILogger<HeroPortraitImageWriter> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public HeroPortraitImageWriterTests()
    {
        _logger = Substitute.For<ILogger<HeroPortraitImageWriter>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = HeroesXmlLoader.LoadWithEmpty();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public async Task WriteImages_HasImages_NewImagesAreCreated()
    {
        // arrange
        const string testDirectory = "heroPortrait";
        _options.Value.Returns(new RootOptions()
        {
            OutputDirectory = Path.Join(OutputBaseDirectory, testDirectory),
        });

        HeroPortraitImageWriter heroPortraitImageWriter = new(_logger, _options, _heroesXmlLoaderService);

        Dictionary<string, Hero> elementsById = [];
        elementsById.Add("hero1", new Hero("id1")
        {
            HeroPortraits = new HeroPortrait()
            {
                HeroSelectPortrait = "heroSelectPortrait1.png",
                HeroSelectPortraitPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "hero_select_portrait1.dds") },
                LeaderboardPortrait = "leaderboardPortrait1.png",
                LeaderboardPortraitPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "leaderboard_portrait1.dds") },
                LoadingScreenPortrait = "loadingScreenPortrait1.png",
                LoadingScreenPortraitPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "loading_screen_portrait1.dds") },
                PartyPanelPortrait = "partyPanelPortrait1.png",
                PartyPanelPortraitPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "party_panel_portrait1.dds") },
                TargetPortrait = "targetPortrait1.png",
                TargetPortraitPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "target_portrait1.dds") },
                DraftScreen = "draftScreen1.png",
                DraftScreenPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "draft_screen1.dds") },
                MiniMapIcon = "miniMapIcon1.png",
                MiniMapIconPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "minimap_icon1.dds") },
                TargetInfoPanel = "targetInfoPanel1.png",
                TargetInfoPanelPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "target_info_panel1.dds") },
                PartyFrames = ["partyFrame1.png", "partyFrame2.png"],
                PartyFramePaths = [new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "party_frame1.dds") }, new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "party_frame2.dds") }],
            },
        });

        // act
        await heroPortraitImageWriter.WriteImages(elementsById);

        // assert
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "heroportraits", "heroSelectPortrait1.png")).Should().BeTrue();
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "heroportraits", "leaderboardPortrait1.png")).Should().BeTrue();
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "heroportraits", "targetPortrait1.png")).Should().BeTrue();
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "heroportraits", "draftScreen1.png")).Should().BeTrue();
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "heroportraits", "miniMapIcon1.png")).Should().BeTrue();
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "heroportraits", "targetInfoPanel1.png")).Should().BeTrue();
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "heroportraits", "partyFrame1.png")).Should().BeTrue();
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "heroportraits", "partyFrame2.png")).Should().BeTrue();
    }
}