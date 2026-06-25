namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class MapObjectiveIconImageParserTests : ImageWriterBase
{
    private readonly ILogger<MapObjectiveIconImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public MapObjectiveIconImageParserTests()
    {
        _logger = Substitute.For<ILogger<MapObjectiveIconImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        MapObjectiveIconImageParser mapObjectiveIconImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, Map> elementsById = [];

        Map map = new("id1");

        MapObjective objective1 = new()
        {
            Icons =
            [
                new MapObjectiveIcon
                {
                    Image = "objective_icon1.png",
                    ImagePath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "objective_icon1.dds") },
                },
                new MapObjectiveIcon
                {
                    Image = "objective_icon2.png",
                    ImagePath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "objective_icon2.dds") },
                },
            ],
        };

        MapObjective objective2 = new()
        {
            Icons =
            [
                new MapObjectiveIcon
                {
                    Image = "objective_icon3.png",
                    ImagePath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "objective_icon3.dds") },
                },
            ],
        };

        map.MapObjectives.Add(objective1);
        map.MapObjectives.Add(objective2);

        elementsById.Add("map1", map);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = mapObjectiveIconImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(3);

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFileList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("objective_icon1.png");
        path1.SubDirectoryPath.Should().Be("mapobjectives");

        ImageWriterFile path2 = imageWriterFileList[1];
        path2.ElementId.Should().Be("id1");
        path2.FileName.Should().Be("objective_icon2.png");
        path2.SubDirectoryPath.Should().Be("mapobjectives");

        ImageWriterFile path3 = imageWriterFileList[2];
        path3.ElementId.Should().Be("id1");
        path3.FileName.Should().Be("objective_icon3.png");
        path3.SubDirectoryPath.Should().Be("mapobjectives");
    }

    [TestMethod]
    public async Task ProcessImageFile_FileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(Map));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Map> elementsById = [];

        Map map = new("id1");

        MapObjective objective1 = new()
        {
            Icons =
            [
                new MapObjectiveIcon
                {
                    Image = "storm_ui_loadscreen_industrial_district_icon_02.png",
                    ImagePath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "storm_ui_loadscreen_industrial_district_icon_02.dds") },
                },
                new MapObjectiveIcon
                {
                    Image = "storm_ui_loadscreen_industrial_district_icon_03.png",
                    ImagePath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "storm_ui_loadscreen_industrial_district_icon_03.dds") },
                },
            ],
        };

        map.MapObjectives.Add(objective1);

        elementsById.Add("map1", map);

        MapObjectiveIconImageParser mapObjectiveIconImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = mapObjectiveIconImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_loadscreen_industrial_district_icon_02.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_loadscreen_industrial_district_icon_03.png")).Should().BeTrue();
    }
}
