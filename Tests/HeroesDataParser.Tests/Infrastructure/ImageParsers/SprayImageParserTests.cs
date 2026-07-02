namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class SprayImageParserTests : ImageWriterBase
{
    private readonly ILogger<SprayImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public SprayImageParserTests()
    {
        _logger = Substitute.For<ILogger<SprayImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        SprayImageParser sprayImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, Spray> elementsById = [];
        Spray spray = new("id1")
        {
            Image = "spray1.png",
        };

        (spray as IImagePath).ImagePath = new ImagePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "spray1.png"),
        };

        elementsById.Add("spray1", spray);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = sprayImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().ContainSingle();

        List<ImageWriterFile> imageWriterFilesList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFilesList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("spray1.png");
        path1.SubDirectoryPath.Should().Be("sprays");
    }

    [TestMethod]
    public async Task ProcessImageFile_StaticFileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(Spray));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Spray> elementsById = [];
        Spray spray = new("id1")
        {
            Image = "classic21_diablo_spray.png",
        };

        (spray as IImagePath).ImagePath = new ImagePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "classic21_diablo_spray.dds"),
        };

        elementsById.Add("spray1", spray);

        SprayImageParser sprayImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = sprayImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "classic21_diablo_spray.png")).Should().BeTrue();
    }

    [TestMethod]
    public async Task ProcessImageFile_AnimatedFileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(Spray));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Spray> elementsById = [];
        Spray sprayAPNG = new("id1")
        {
            Image = "storm_lootspray_animated_craft20_lowbattery.apng",
            Animation = new SprayAnimation
            {
                Texture = "storm_lootspray_animated_craft20_lowbattery.png",
                Frames = 2,
                Duration = 2000,
            },
        };
        Spray sprayGIF = new("id2")
        {
            Image = "storm_lootspray_animated_craft20_lowbattery.gif",
            Animation = new SprayAnimation
            {
                Texture = "storm_lootspray_animated_craft20_lowbattery.png",
                Frames = 2,
                Duration = 2000,
            },
        };

        (sprayAPNG as IImagePath).ImagePath = new ImagePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "storm_lootspray_animated_craft20_lowbattery.dds"),
        };
        (sprayGIF as IImagePath).ImagePath = new ImagePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "storm_lootspray_animated_craft20_lowbattery.dds"),
        };

        elementsById.Add("spray1", sprayAPNG);
        elementsById.Add("spray2", sprayGIF);

        SprayImageParser sprayImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = sprayImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_lootspray_animated_craft20_lowbattery.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_lootspray_animated_craft20_lowbattery.apng")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_lootspray_animated_craft20_lowbattery.gif")).Should().BeTrue();
    }
}
