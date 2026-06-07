namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class AnnouncerImageParserTests : ImageWriterBase
{
    private readonly ILogger<AnnouncerPackImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public AnnouncerImageParserTests()
    {
        _logger = Substitute.For<ILogger<AnnouncerPackImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        AnnouncerPackImageParser announcerImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, AnnouncerPack> elementsById = [];
        AnnouncerPack announcer = new("id1")
        {
            Image = "announcer1.png",
        };

        (announcer as IImagePath).ImagePath = new RelativeFilePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "announcer1.png"),
        };

        elementsById.Add("announcer1", announcer);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = announcerImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().ContainSingle();

        List<ImageWriterFile> imageWriterFilesList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFilesList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("announcer1.png");
        path1.SubDirectoryPath.Should().Be("announcers");
    }

    [TestMethod]
    public async Task ProcessImageFile_FileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(AnnouncerPack));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, AnnouncerPack> elementsById = [];
        AnnouncerPack announcer = new("id1")
        {
            Image = "storm_ui_announcer_adjutant.png",
        };

        (announcer as IImagePath).ImagePath = new RelativeFilePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "storm_ui_announcer_adjutant.dds"),
        };

        elementsById.Add("announcer1", announcer);

        AnnouncerPackImageParser announcerImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = announcerImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_announcer_adjutant.png")).Should().BeTrue();
    }
}
