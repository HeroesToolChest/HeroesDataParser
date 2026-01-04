namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class BundleImageParserTests : ImageWriterBase
{
    private readonly ILogger<BundleImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public BundleImageParserTests()
    {
        _logger = Substitute.For<ILogger<BundleImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        BundleImageParser bundleImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, Bundle> elementsById = [];
        Bundle bundle = new("id1")
        {
            Image = "bundle1.png",
        };

        (bundle as IImagePath).ImagePath = new RelativeFilePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "bundle1.png"),
        };

        elementsById.Add("bundle1", bundle);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = bundleImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().ContainSingle();

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFileList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("bundle1.png");
        path1.SubDirectoryPath.Should().Be("bundles");
    }
}
