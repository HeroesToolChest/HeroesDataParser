namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class AnnouncerImageParserTests : ImageWriterBase
{
    private readonly ILogger<AnnouncerImageParser> _logger;

    public AnnouncerImageParserTests()
    {
        _logger = Substitute.For<ILogger<AnnouncerImageParser>>();
    }

    [TestMethod]
    public void SaveImages_HasImages_GetImagePaths()
    {
        // arrange
        AnnouncerImageParser announcerImageParser = new(_logger);

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
        HashSet<ImageWriterPath> imagePaths = announcerImageParser.GetImages(elementsById);

        // assert
        imagePaths.Should().ContainSingle();

        List<ImageWriterPath> imagePathsList = [.. imagePaths];

        ImageWriterPath path1 = imagePathsList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("announcer1.png");
        path1.RelativeFilePath.Should().Be(Path.Join("TestImages", "announcer1.png"));
        path1.SubDirectoryPath.Should().Be("announcers");
        path1.RelativeMpqFilePath.Should().BeNull();
    }
}
