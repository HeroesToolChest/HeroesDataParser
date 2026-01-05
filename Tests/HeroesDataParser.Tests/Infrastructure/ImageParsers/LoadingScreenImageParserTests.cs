namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class LoadingScreenImageParserTests : ImageWriterBase
{
    private readonly ILogger<LoadingScreenImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public LoadingScreenImageParserTests()
    {
        _logger = Substitute.For<ILogger<LoadingScreenImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        LoadingScreenImageParser loadingScreenImageParser = new(_logger, _heroesXmlLoaderService);

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
        HashSet<ImageWriterFile> imageWriterFiles = loadingScreenImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(2);

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFileList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("loadingscreen1.png");
        path1.SubDirectoryPath.Should().Be("loadingscreens");

        ImageWriterFile path2 = imageWriterFileList[1];
        path2.ElementId.Should().Be("id2");
        path2.FileName.Should().Be("loadingscreen2.png");
        path2.SubDirectoryPath.Should().Be("loadingscreens");
    }

    [TestMethod]
    public async Task ProcessImageFile_FileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(Map));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Map> elementsById = [];
        elementsById.Add("map1", new Map("id1")
        {
            LoadingScreenImage = "storm_ui_homescreenbackground_trialgrounds.png",
            LoadingScreenImagePath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "storm_ui_homescreenbackground_trialgrounds.dds") },
        });

        LoadingScreenImageParser loadingScreenImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = loadingScreenImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_homescreenbackground_trialgrounds.png")).Should().BeTrue();
    }
}
