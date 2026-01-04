namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class MatchAwardImageParserTests : ImageWriterBase
{
    private readonly ILogger<MatchAwardImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public MatchAwardImageParserTests()
    {
        _logger = Substitute.For<ILogger<MatchAwardImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        MatchAwardImageParser matchAwardImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, MatchAward> elementsById = [];

        MatchAward matchAward1 = new("id1")
        {
            MVPScreenImage = "mvp_screen1.png",
            MVPScreenImagePath = new RelativeFilePath
            {
                FilePath = Path.Combine(TestImagesDirectory, "mvp_screen1.dds"),
            },
            ScoreScreenImage = "score_screen1.png",
            ScoreScreenImageBluePath = new RelativeFilePath
            {
                FilePath = Path.Combine(TestImagesDirectory, "score_screen1_blue.dds"),
            },
            ScoreScreenImageRedPath = new RelativeFilePath
            {
                FilePath = Path.Combine(TestImagesDirectory, "score_screen1_red.dds"),
            },
        };

        MatchAward matchAward2 = new("id2")
        {
            MVPScreenImage = "mvp_screen2.png",
            MVPScreenImagePath = new RelativeFilePath
            {
                FilePath = Path.Combine(TestImagesDirectory, "mvp_screen2.dds"),
            },
            ScoreScreenImage = "score_screen2.png",
            ScoreScreenImageBluePath = new RelativeFilePath
            {
                FilePath = Path.Combine(TestImagesDirectory, "score_screen2_blue.dds"),
            },
        };

        elementsById.Add("matchaward1", matchAward1);
        elementsById.Add("matchaward2", matchAward2);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = matchAwardImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(5);

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFileList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("score_screen1.png (blue)");
        path1.SubDirectoryPath.Should().Be("matchawards");

        ImageWriterFile path2 = imageWriterFileList[1];
        path2.ElementId.Should().Be("id1");
        path2.FileName.Should().Be("score_screen1.png (red)");
        path2.SubDirectoryPath.Should().Be("matchawards");

        ImageWriterFile path3 = imageWriterFileList[2];
        path3.ElementId.Should().Be("id1");
        path3.FileName.Should().Be("mvp_screen1.png");
        path3.SubDirectoryPath.Should().Be("matchawards");

        ImageWriterFile path4 = imageWriterFileList[3];
        path4.ElementId.Should().Be("id2");
        path4.FileName.Should().Be("score_screen2.png (blue)");
        path4.SubDirectoryPath.Should().Be("matchawards");

        ImageWriterFile path5 = imageWriterFileList[4];
        path5.ElementId.Should().Be("id2");
        path5.FileName.Should().Be("mvp_screen2.png");
        path5.SubDirectoryPath.Should().Be("matchawards");
    }
}
