namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class VoiceLineImageParserTests : ImageWriterBase
{
    private readonly ILogger<VoiceLineImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public VoiceLineImageParserTests()
    {
        _logger = Substitute.For<ILogger<VoiceLineImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        VoiceLineImageParser voiceLineImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, VoiceLine> elementsById = [];
        VoiceLine voiceLine = new("id1")
        {
            Image = "voiceline1.png",
        };

        (voiceLine as IImagePath).ImagePath = new RelativeFilePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "voiceline1.png"),
        };

        elementsById.Add("voiceline1", voiceLine);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = voiceLineImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().ContainSingle();

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFileList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("voiceline1.png");
        path1.SubDirectoryPath.Should().Be("voicelines");
    }
}
