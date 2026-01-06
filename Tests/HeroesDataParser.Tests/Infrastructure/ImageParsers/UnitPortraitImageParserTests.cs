namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class UnitPortraitImageParserTests : ImageWriterBase
{
    private readonly ILogger<UnitPortraitImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public UnitPortraitImageParserTests()
    {
        _logger = Substitute.For<ILogger<UnitPortraitImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        UnitPortraitImageParser unitPortraitImageWriter = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, Unit> elementsById = [];
        elementsById.Add("unit1", new Unit("id1")
        {
            UnitPortraits = new UnitPortrait()
            {
                MiniMapIcon = "miniMapIcon1.png",
                MiniMapIconPath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "minimap_icon1.dds") },
                TargetInfoPanel = "targetInfoPanel1.png",
                TargetInfoPanelPath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "target_info_panel1.dds") },
            },
        });

        // act
        HashSet<ImageWriterFile> imageWriterFiles = unitPortraitImageWriter.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(2);

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFileList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("miniMapIcon1.png");
        path1.SubDirectoryPath.Should().Be("unitportraits");

        ImageWriterFile path2 = imageWriterFileList[1];
        path2.ElementId.Should().Be("id1");
        path2.FileName.Should().Be("targetInfoPanel1.png");
        path2.SubDirectoryPath.Should().Be("unitportraits");
    }

    [TestMethod]
    public async Task ProcessImageFile_FileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(UnitPortrait));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Unit> elementsById = [];
        elementsById.Add("unit1", new Unit("id1")
        {
            UnitPortraits = new UnitPortrait()
            {
                MiniMapIcon = "storm_ui_ingame_targetinfopanel_unit_abathur_toxicnest.png",
                MiniMapIconPath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "storm_ui_ingame_targetinfopanel_unit_abathur_toxicnest.dds") },
                TargetInfoPanel = "storm_ui_hud_volskaya_minimap_obj_b.png",
                TargetInfoPanelPath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "storm_ui_hud_volskaya_minimap_obj_b.dds") },
            },
        });

        UnitPortraitImageParser unitPortraitImageWriter = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = unitPortraitImageWriter.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_ingame_targetinfopanel_unit_abathur_toxicnest.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_hud_volskaya_minimap_obj_b.png")).Should().BeTrue();
    }
}
