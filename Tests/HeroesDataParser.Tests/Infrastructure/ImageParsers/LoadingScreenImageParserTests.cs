namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class LoadingScreenImageParserTests : ImageWriterBase
{
    private readonly ILogger<LoadingScreenImageParser> _logger;

    public LoadingScreenImageParserTests()
    {
        _logger = Substitute.For<ILogger<LoadingScreenImageParser>>();
    }

    [TestMethod]
    public void SaveImages_HasImages_GetImagePaths()
    {
        // arrange
        LoadingScreenImageParser loadingScreenImageParser = new(_logger);

        SortedDictionary<string, Map> elementsById = [];
        elementsById.Add("map1", new Map("id1")
        {
            LoadingScreenImage = "loadingscreen1.png",
            LoadingScreenImagePath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "loadingscreen1.dds") },
        });
        elementsById.Add("map2", new Map("id2")
        {
            LoadingScreenImage = "loadingscreen2.png",
            LoadingScreenImagePath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "loadingscreen2.dds") },
        });

        // act
        HashSet<ImageWriterPath> imagePaths = loadingScreenImageParser.GetImages(elementsById);

        // assert
        imagePaths.Should().HaveCount(2);

        List<ImageWriterPath> imagePathsList = [.. imagePaths];

        ImageWriterPath path1 = imagePathsList[0];
        path1.ElementId.Should().Be("id1");
        path1.RelativeFilePath.Should().Be(Path.Join("TestImages", "loadingscreen1.dds"));
        path1.RelativeMpqFilePath.Should().BeNull();
        path1.FileName.Should().Be("loadingscreen1.png");
        path1.SubDirectoryPath.Should().Be("loadingscreens");

        ImageWriterPath path2 = imagePathsList[1];
        path2.ElementId.Should().Be("id2");
        path2.RelativeFilePath.Should().Be(Path.Join("TestImages", "loadingscreen2.dds"));
        path2.RelativeMpqFilePath.Should().BeNull();
        path2.FileName.Should().Be("loadingscreen2.png");
        path2.SubDirectoryPath.Should().Be("loadingscreens");
    }
}
