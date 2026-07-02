namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class HeroPortraitImageParserTests : ImageWriterBase
{
    private readonly ILogger<HeroPortraitImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public HeroPortraitImageParserTests()
    {
        _logger = Substitute.For<ILogger<HeroPortraitImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
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
                HeroSelectPortraitPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "hero_select_portrait1.dds") },
                LeaderboardPortrait = "leaderboardPortrait1.png",
                LeaderboardPortraitPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "leaderboard_portrait1.dds") },
                LoadingScreenPortrait = "loadingScreenPortrait1.png",
                LoadingScreenPortraitPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "loading_screen_portrait1.dds") },
                PartyPanelPortrait = "partyPanelPortrait1.png",
                PartyPanelPortraitPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "party_panel_portrait1.dds") },
                TargetPortrait = "targetPortrait1.png",
                TargetPortraitPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "target_portrait1.dds") },
                DraftScreen = "draftScreen1.png",
                DraftScreenPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "draft_screen1.dds") },
                MiniMapIcon = "miniMapIcon1.png",
                MiniMapIconPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "minimap_icon1.dds") },
                TargetInfoPanel = "targetInfoPanel1.png",
                TargetInfoPanelPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "target_info_panel1.dds") },
                PartyFrames = ["partyFrame1.png", "partyFrame2.png"],
                PartyFramePaths = [new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "party_frame1.dds") }, new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "party_frame2.dds") }],
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

    [TestMethod]
    public async Task ProcessImageFile_FileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(HeroPortrait));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Hero> elementsById = [];
        elementsById.Add("hero1", new Hero("id1")
        {
            HeroPortraits = new HeroPortrait()
            {
                HeroSelectPortrait = "storm_ui_ingame_heroselect_btn_anduin.png",
                HeroSelectPortraitPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "storm_ui_ingame_heroselect_btn_anduin.dds") },
                LeaderboardPortrait = "storm_ui_ingame_hero_leaderboard_zuljin.png",
                LeaderboardPortraitPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "storm_ui_ingame_hero_leaderboard_zuljin.dds") },
                LoadingScreenPortrait = "storm_ui_hero_loading_screen_firebat.png",
                LoadingScreenPortraitPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "storm_ui_hero_loading_screen_firebat.dds") },
                PartyPanelPortrait = "storm_ui_ingame_partypanel_btn_ana.png",
                PartyPanelPortraitPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "storm_ui_ingame_partypanel_btn_ana.dds") },
                TargetPortrait = "ui_targetportrait_hero_artanis.png",
                TargetPortraitPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "ui_targetportrait_hero_artanis.dds") },
                DraftScreen = "storm_ui_glues_draft_portrait_abathur.png",
                DraftScreenPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "storm_ui_glues_draft_portrait_abathur.dds") },
                MiniMapIcon = "storm_ui_minimapicon_alarak.png",
                MiniMapIconPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "storm_ui_minimapicon_alarak.dds") },
                TargetInfoPanel = "storm_ui_ingame_hero_loadingscreen_d2amazonf.png",
                TargetInfoPanelPath = new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "storm_ui_ingame_hero_loadingscreen_d2amazonf.dds") },
                PartyFrames = ["storm_ui_ingame_partyframe_alexstrasza.png"],
                PartyFramePaths = [new ImagePath { FilePath = Path.Combine(TestImagesDirectory, "storm_ui_ingame_partyframe_alexstrasza.dds") }],
            },
        });

        HeroPortraitImageParser heroPortraitImageWriter = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = heroPortraitImageWriter.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_ingame_heroselect_btn_anduin.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_ingame_hero_leaderboard_zuljin.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_hero_loading_screen_firebat.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_ingame_partypanel_btn_ana.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "ui_targetportrait_hero_artanis.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_glues_draft_portrait_abathur.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_minimapicon_alarak.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_ingame_partyframe_alexstrasza.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_ingame_hero_loadingscreen_d2amazonf.png")).Should().BeTrue();
    }
}