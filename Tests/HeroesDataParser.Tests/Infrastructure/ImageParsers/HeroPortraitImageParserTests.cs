namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class HeroPortraitImageParserTests : ImageWriterBase
{
    private readonly ILogger<HeroPortraitImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public HeroPortraitImageParserTests()
    {
        _logger = Substitute.For<ILogger<HeroPortraitImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        HeroPortraitImageParser heroPortraitImageWriter = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, Hero> elementsById = [];
        elementsById.Add("hero1", new Hero("id1")
        {
            HeroPortraits = new HeroPortrait()
            {
                HeroSelectPortrait = "heroSelectPortrait1.png",
                HeroSelectPortraitPath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "hero_select_portrait1.dds") },
                LeaderboardPortrait = "leaderboardPortrait1.png",
                LeaderboardPortraitPath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "leaderboard_portrait1.dds") },
                LoadingScreenPortrait = "loadingScreenPortrait1.png",
                LoadingScreenPortraitPath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "loading_screen_portrait1.dds") },
                PartyPanelPortrait = "partyPanelPortrait1.png",
                PartyPanelPortraitPath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "party_panel_portrait1.dds") },
                TargetPortrait = "targetPortrait1.png",
                TargetPortraitPath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "target_portrait1.dds") },
                DraftScreen = "draftScreen1.png",
                DraftScreenPath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "draft_screen1.dds") },
                MiniMapIcon = "miniMapIcon1.png",
                MiniMapIconPath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "minimap_icon1.dds") },
                TargetInfoPanel = "targetInfoPanel1.png",
                TargetInfoPanelPath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "target_info_panel1.dds") },
                PartyFrames = ["partyFrame1.png", "partyFrame2.png"],
                PartyFramePaths = [new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "party_frame1.dds") }, new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "party_frame2.dds") }],
            },
        });

        // act
        HashSet<ImageWriterFile> imageWriterFiles = heroPortraitImageWriter.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(10);

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFileList[0];
        path1.ElementId.Should().Be("id1");
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("heroSelectPortrait1.png");
        path1.SubDirectoryPath.Should().Be("heroportraits");

        ImageWriterFile path2 = imageWriterFileList[1];
        path2.ElementId.Should().Be("id1");

        ImageWriterFile path3 = imageWriterFileList[2];
        path3.ElementId.Should().Be("id1");

        ImageWriterFile path4 = imageWriterFileList[3];
        path4.ElementId.Should().Be("id1");

        ImageWriterFile path5 = imageWriterFileList[4];
        path5.ElementId.Should().Be("id1");

        ImageWriterFile path6 = imageWriterFileList[5];
        path6.ElementId.Should().Be("id1");

        ImageWriterFile path7 = imageWriterFileList[6];
        path7.ElementId.Should().Be("id1");

        ImageWriterFile path8 = imageWriterFileList[7];
        path8.ElementId.Should().Be("id1");

        ImageWriterFile path9 = imageWriterFileList[8];
        path9.ElementId.Should().Be("id1");

        ImageWriterFile path10 = imageWriterFileList[9];
        path10.ElementId.Should().Be("id1");
    }
}