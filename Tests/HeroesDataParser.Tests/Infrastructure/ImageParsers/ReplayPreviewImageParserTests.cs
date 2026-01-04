namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class ReplayPreviewImageParserTests : ImageWriterBase
{
    private readonly ILogger<ReplayPreviewImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public ReplayPreviewImageParserTests()
    {
        _logger = Substitute.For<ILogger<ReplayPreviewImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        ReplayPreviewImageParser replayPreviewImageParser = new(_logger, _heroesXmlLoaderService);

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
        HashSet<ImageWriterFile> imageWriterFiles = replayPreviewImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(2);

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFileList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("replaypreview1.png");
        path1.SubDirectoryPath.Should().Be("replaypreviews");

        ImageWriterFile path2 = imageWriterFileList[1];
        path2.ElementId.Should().Be("id2");
        path2.FileName.Should().Be("replaypreview2.png");
        path2.SubDirectoryPath.Should().Be("replaypreviews");
    }
}
