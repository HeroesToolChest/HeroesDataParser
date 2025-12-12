namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class BundleImageParserTests : ImageWriterBase
{
    private readonly ILogger<BundleImageParser> _logger;

    public BundleImageParserTests()
    {
        _logger = Substitute.For<ILogger<BundleImageParser>>();
    }

    [TestMethod]
    public void SaveImages_HasImages_GetImagePaths()
    {
        // arrange
        BundleImageParser bundleImageParser = new(_logger);

        SortedDictionary<string, Bundle> elementsById = [];
        elementsById.Add("bundle1", new Bundle("id1")
        {
            Image = "bundle1.png",
            ImagePath = new RelativeFilePath { FilePath = Path.Combine(TestImagesDirectory, "bundle1.png") },
        });

        // act
        HashSet<ImageWriterPath> imagePaths = bundleImageParser.GetImages(elementsById);

        // assert
        imagePaths.Should().ContainSingle();

        List<ImageWriterPath> imagePathsList = [.. imagePaths];

        ImageWriterPath path1 = imagePathsList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("bundle1.png");
        path1.RelativeFilePath.Should().Be(Path.Join("TestImages", "bundle1.png"));
        path1.SubDirectoryPath.Should().Be("bundles");
        path1.RelativeMpqFilePath.Should().BeNull();
    }
}
