namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class VoiceLineImageParserTests : ImageWriterBase
{
    private readonly ILogger<VoiceLineImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public VoiceLineImageParserTests()
    {
        _logger = Substitute.For<ILogger<VoiceLineImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
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

    [TestMethod]
    public async Task ProcessImageFile_FileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(VoiceLine));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, VoiceLine> elementsById = [];
        VoiceLine voiceLine = new("id1")
        {
            Image = "storm_ui_voice_abathur.png",
        };

        (voiceLine as IImagePath).ImagePath = new RelativeFilePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "storm_ui_voice_abathur.dds"),
        };

        elementsById.Add("voiceline1", voiceLine);

        VoiceLineImageParser voiceLineImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = voiceLineImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_voice_abathur.png")).Should().BeTrue();
    }
}
