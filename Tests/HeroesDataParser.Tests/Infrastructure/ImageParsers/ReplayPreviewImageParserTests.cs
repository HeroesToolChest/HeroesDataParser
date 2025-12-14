namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class ReplayPreviewImageParserTests : ImageWriterBase
{
    private readonly ILogger<ReplayPreviewImageParser> _logger;

    public ReplayPreviewImageParserTests()
    {
        _logger = Substitute.For<ILogger<ReplayPreviewImageParser>>();
    }

    [TestMethod]
    public void SaveImages_HasImages_GetImagePaths()
    {
        // arrange
        ReplayPreviewImageParser replayPreviewImageParser = new(_logger);

        SortedDictionary<string, Map> elementsById = [];
        elementsById.Add("map1", new Map("id1")
        {
            ReplayPreviewImage = "replaypreview1.png",
            ReplayPreviewImagePath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "replaypreview1.dds") },
        });
        elementsById.Add("map2", new Map("id2")
        {
            ReplayPreviewImage = "replaypreview2.png",
            ReplayPreviewImagePath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "replaypreview2.dds") },
        });

        // act
        HashSet<ImageWriterPath> imagePaths = replayPreviewImageParser.GetImages(elementsById);

        // assert
        imagePaths.Should().HaveCount(2);

        List<ImageWriterPath> imagePathsList = [.. imagePaths];

        ImageWriterPath path1 = imagePathsList[0];
        path1.ElementId.Should().Be("id1");
        path1.RelativeFilePath.Should().Be(Path.Join("TestImages", "replaypreview1.dds"));
        path1.RelativeMpqFilePath.Should().BeNull();
        path1.FileName.Should().Be("replaypreview1.png");
        path1.SubDirectoryPath.Should().Be("replaypreviews");

        ImageWriterPath path2 = imagePathsList[1];
        path2.ElementId.Should().Be("id2");
        path2.RelativeFilePath.Should().Be(Path.Join("TestImages", "replaypreview2.dds"));
        path2.RelativeMpqFilePath.Should().BeNull();
        path2.FileName.Should().Be("replaypreview2.png");
        path2.SubDirectoryPath.Should().Be("replaypreviews");
    }
}
