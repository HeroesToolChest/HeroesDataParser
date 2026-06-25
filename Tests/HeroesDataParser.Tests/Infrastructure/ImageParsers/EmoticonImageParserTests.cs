namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class EmoticonImageParserTests : ImageWriterBase
{
    private readonly ILogger<EmoticonImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public EmoticonImageParserTests()
    {
        _logger = Substitute.For<ILogger<EmoticonImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        EmoticonImageParser emoticonImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, Emoticon> elementsById = [];
        Emoticon emoticon = new("id1")
        {
            Image = "emoticon1.png",
            TextureSheet = new()
            {
                Columns = 1,
                Rows = 1,
            },
            Width = 2,
        };

        (emoticon as IImagePath).ImagePath = new ImagePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "emoticon1.png"),
        };

        elementsById.Add("emoticon1", emoticon);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = emoticonImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().ContainSingle();

        List<ImageWriterFile> imageWriterFilesList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFilesList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("emoticon1.png");
        path1.SubDirectoryPath.Should().Be("emoticons");
    }

    [TestMethod]
    public async Task ProcessImageFile_StaticFileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(Emoticon));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Emoticon> elementsById = [];
        Emoticon emoticon = new("id1")
        {
            Image = "storm_emoji_greymane_sheet_5.png",
            Index = 5,
            Width = 29,
            TextureSheet = new TextureSheet
            {
                Columns = 4,
                Rows = 3,
            },
        };
        Emoticon emoticon2 = new("id2")
        {
            Image = "storm_emoji_greymane_sheet_0.png",
            Index = 0,
            Width = 30,
            TextureSheet = new TextureSheet
            {
                Columns = 4,
                Rows = 3,
            },
        };

        (emoticon as IImagePath).ImagePath = new ImagePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "storm_emoji_greymane_sheet.dds"),
        };
        (emoticon2 as IImagePath).ImagePath = new ImagePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "storm_emoji_greymane_sheet.dds"),
        };

        elementsById.Add("emoticon1", emoticon);
        elementsById.Add("emoticon2", emoticon2);

        EmoticonImageParser emoticonImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = emoticonImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_emoji_greymane_sheet_0.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_emoji_greymane_sheet_5.png")).Should().BeTrue();
    }

    [TestMethod]
    public async Task ProcessImageFile_AnimatedFileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(Emoticon));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Emoticon> elementsById = [];
        Emoticon emoticonAPNG = new("id1")
        {
            Image = "storm_emoji_cat_gleam_anim_sheet.apng",
            TextureSheet = new TextureSheet
            {
                Columns = 4,
                Rows = 7,
            },
            Animation = new EmoticonAnimation
            {
                Columns = 4,
                Rows = 7,
                Duration = 50,
                Frames = 25,
                Width = 34,
                Texture = "storm_emoji_cat_gleam_anim_sheet.png",
            },
        };
        Emoticon emoticonGif = new("id2")
        {
            Image = "storm_emoji_cat_gleam_anim_sheet.gif",
            TextureSheet = new TextureSheet
            {
                Columns = 4,
                Rows = 7,
            },
            Animation = new EmoticonAnimation
            {
                Columns = 4,
                Rows = 7,
                Duration = 50,
                Frames = 25,
                Width = 34,
                Texture = "storm_emoji_cat_gleam_anim_sheet.png",
            },
        };

        (emoticonAPNG as IImagePath).ImagePath = new ImagePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "storm_emoji_cat_gleam_anim_sheet.dds"),
        };
        (emoticonGif as IImagePath).ImagePath = new ImagePath
        {
            FilePath = Path.Combine(TestImagesDirectory, "storm_emoji_cat_gleam_anim_sheet.dds"),
        };

        elementsById.Add("emoticon1", emoticonAPNG);
        elementsById.Add("emoticon2", emoticonGif);

        EmoticonImageParser emoticonImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = emoticonImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_emoji_cat_gleam_anim_sheet.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_emoji_cat_gleam_anim_sheet.apng")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_emoji_cat_gleam_anim_sheet.gif")).Should().BeTrue();
    }
}
