namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class MapObjectiveIconImageParserTests : ImageWriterBase
{
    private readonly ILogger<MapObjectiveIconImageParser> _logger;

    public MapObjectiveIconImageParserTests()
    {
        _logger = Substitute.For<ILogger<MapObjectiveIconImageParser>>();
    }

    [TestMethod]
    public void SaveImages_HasImages_GetImagePaths()
    {
        // arrange
        MapObjectiveIconImageParser mapObjectiveIconImageParser = new(_logger);

        SortedDictionary<string, Map> elementsById = [];

        Map map = new("id1");

        MapObjective objective1 = new()
        {
            Icons =
            [
                new MapObjectiveIcon
                {
                    Image = "objective_icon1.png",
                    ImagePath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "objective_icon1.dds") },
                },
                new MapObjectiveIcon
                {
                    Image = "objective_icon2.png",
                    ImagePath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "objective_icon2.dds") },
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
                    ImagePath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "objective_icon3.dds") },
                },
            ],
        };

        map.MapObjectives.Add(objective1);
        map.MapObjectives.Add(objective2);

        elementsById.Add("map1", map);

        // act
        HashSet<ImageWriterPath> imagePaths = mapObjectiveIconImageParser.GetImages(elementsById);

        // assert
        imagePaths.Should().HaveCount(3);

        List<ImageWriterPath> imagePathsList = [.. imagePaths];

        ImageWriterPath path1 = imagePathsList[0];
        path1.ElementId.Should().Be("id1");
        path1.RelativeFilePath.Should().Be(Path.Join("TestImages", "objective_icon1.dds"));
        path1.RelativeMpqFilePath.Should().BeNull();
        path1.FileName.Should().Be("objective_icon1.png");
        path1.SubDirectoryPath.Should().Be("mapobjectives");

        ImageWriterPath path2 = imagePathsList[1];
        path2.ElementId.Should().Be("id1");
        path2.RelativeFilePath.Should().Be(Path.Join("TestImages", "objective_icon2.dds"));
        path2.RelativeMpqFilePath.Should().BeNull();
        path2.FileName.Should().Be("objective_icon2.png");
        path2.SubDirectoryPath.Should().Be("mapobjectives");

        ImageWriterPath path3 = imagePathsList[2];
        path3.ElementId.Should().Be("id1");
        path3.RelativeFilePath.Should().Be(Path.Join("TestImages", "objective_icon3.dds"));
        path3.RelativeMpqFilePath.Should().BeNull();
        path3.FileName.Should().Be("objective_icon3.png");
        path3.SubDirectoryPath.Should().Be("mapobjectives");
    }
}
