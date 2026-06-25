namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class BundleImageParserTests : ImageWriterBase
{
    private readonly ILogger<BundleImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public BundleImageParserTests()
    {
        _logger = Substitute.For<ILogger<BundleImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
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

        (bundle as IImagePath).ImagePath = new ImagePath
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

    [TestMethod]
    public async Task ProcessImageFile_FileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(Bundle));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Bundle> elementsById = [];
        Bundle bundle = new("id1")
        {
            Image = "storm_ui_bundle_archangeldiablo.png",
        };

        (bundle as IImagePath).ImagePath = new ImagePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "storm_ui_bundle_archangeldiablo.dds"),
        };

        elementsById.Add("bundle1", bundle);

        BundleImageParser bundleImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = bundleImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_bundle_archangeldiablo.png")).Should().BeTrue();
    }
}
