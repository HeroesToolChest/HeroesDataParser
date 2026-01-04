namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class AnnouncerImageParserTests : ImageWriterBase
{
    private readonly ILogger<AnnouncerImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public AnnouncerImageParserTests()
    {
        _logger = Substitute.For<ILogger<AnnouncerImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        AnnouncerImageParser announcerImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, Announcer> elementsById = [];
        Announcer announcer = new("id1")
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
}
