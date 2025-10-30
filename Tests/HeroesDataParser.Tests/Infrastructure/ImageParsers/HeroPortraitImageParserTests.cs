namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class HeroPortraitImageParserTests : ImageWriterBase
{
    private readonly ILogger<HeroPortraitImageParser> _logger;

    public HeroPortraitImageParserTests()
    {
        _logger = Substitute.For<ILogger<HeroPortraitImageParser>>();
    }

    [TestMethod]
    public void SaveImages_HasImages_GetImagePaths()
    {
        // arrange
        HeroPortraitImageParser heroPortraitImageWriter = new(_logger);

        Dictionary<string, Hero> elementsById = [];
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
        HashSet<ImageWriterPath> imagePaths = heroPortraitImageWriter.GetImages(elementsById);

        // assert
        imagePaths.Should().HaveCount(10);

        List<ImageWriterPath> imagePathsList = [.. imagePaths];

        ImageWriterPath path1 = imagePathsList[0];
        path1.ElementId.Should().Be("id1");
        path1.RelativeFilePath.Should().Be(Path.Join("TestImages", "hero_select_portrait1.dds"));
        path1.RelativeMpqFilePath.Should().BeNull();
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("heroSelectPortrait1.png");
        path1.SubDirectoryPath.Should().Be("heroportraits");

        ImageWriterPath path2 = imagePathsList[1];
        path2.ElementId.Should().Be("id1");
        path2.RelativeFilePath.Should().Be(Path.Join("TestImages", "leaderboard_portrait1.dds"));
        path2.RelativeMpqFilePath.Should().BeNull();

        ImageWriterPath path3 = imagePathsList[2];
        path3.ElementId.Should().Be("id1");
        path3.RelativeFilePath.Should().Be(Path.Join("TestImages", "loading_screen_portrait1.dds"));
        path3.RelativeMpqFilePath.Should().BeNull();

        ImageWriterPath path4 = imagePathsList[3];
        path4.ElementId.Should().Be("id1");
        path4.RelativeFilePath.Should().Be(Path.Join("TestImages", "party_panel_portrait1.dds"));
        path4.RelativeMpqFilePath.Should().BeNull();

        ImageWriterPath path5 = imagePathsList[4];
        path5.ElementId.Should().Be("id1");
        path5.RelativeFilePath.Should().Be(Path.Join("TestImages", "target_portrait1.dds"));
        path5.RelativeMpqFilePath.Should().BeNull();

        ImageWriterPath path6 = imagePathsList[5];
        path6.ElementId.Should().Be("id1");
        path6.RelativeFilePath.Should().Be(Path.Join("TestImages", "draft_screen1.dds"));
        path6.RelativeMpqFilePath.Should().BeNull();

        ImageWriterPath path7 = imagePathsList[6];
        path7.ElementId.Should().Be("id1");
        path7.RelativeFilePath.Should().Be(Path.Join("TestImages", "minimap_icon1.dds"));
        path7.RelativeMpqFilePath.Should().BeNull();

        ImageWriterPath path8 = imagePathsList[7];
        path8.ElementId.Should().Be("id1");
        path8.RelativeFilePath.Should().Be(Path.Join("TestImages", "target_info_panel1.dds"));
        path8.RelativeMpqFilePath.Should().BeNull();

        ImageWriterPath path9 = imagePathsList[8];
        path9.ElementId.Should().Be("id1");
        path9.RelativeFilePath.Should().Be(Path.Join("TestImages", "party_frame1.dds"));
        path9.RelativeMpqFilePath.Should().BeNull();

        ImageWriterPath path10 = imagePathsList[9];
        path10.ElementId.Should().Be("id1");
        path10.RelativeFilePath.Should().Be(Path.Join("TestImages", "party_frame2.dds"));
        path10.RelativeMpqFilePath.Should().BeNull();
    }
}