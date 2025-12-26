namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class VoiceLineImageParserTests : ImageWriterBase
{
    private readonly ILogger<VoiceLineImageParser> _logger;

    public VoiceLineImageParserTests()
    {
        _logger = Substitute.For<ILogger<VoiceLineImageParser>>();
    }

    [TestMethod]
    public void SaveImages_HasImages_GetImagePaths()
    {
        // arrange
        VoiceLineImageParser voiceLineImageParser = new(_logger);

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
        HashSet<ImageWriterPath> imagePaths = voiceLineImageParser.GetImages(elementsById);

        // assert
        imagePaths.Should().ContainSingle();

        List<ImageWriterPath> imagePathsList = [.. imagePaths];

        ImageWriterPath path1 = imagePathsList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("voiceline1.png");
        path1.RelativeFilePath.Should().Be(Path.Join("TestImages", "voiceline1.png"));
        path1.SubDirectoryPath.Should().Be("voicelines");
        path1.RelativeMpqFilePath.Should().BeNull();
    }
}
