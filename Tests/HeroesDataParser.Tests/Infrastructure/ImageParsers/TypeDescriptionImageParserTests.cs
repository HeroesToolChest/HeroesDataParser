namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class TypeDescriptionImageParserTests : ImageWriterBase
{
    private readonly ILogger<TypeDescriptionImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public TypeDescriptionImageParserTests()
    {
        _logger = Substitute.For<ILogger<TypeDescriptionImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        TypeDescriptionImageParser typeDescriptionImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, TypeDescription> elementsById = [];
        TypeDescription typeDescription = new("id1")
        {
            RewardIcon = "typeDescriptionReward1.png",
            RewardIconPath = new RelativeFilePath
            {
                FilePath = Path.Combine(TestImagesDirectory, "typeDescriptionReward1.png"),
            },
            TextureSheet = new()
            {
                Columns = 1,
                Rows = 1,
            },
            LargeIcon = "typeDescriptionLarge1.png",
            LargeIconPath = new RelativeFilePath
            {
                FilePath = Path.Combine(TestImagesDirectory, "typeDescriptionLarge1.png"),
            },
        };

        elementsById.Add("typeDescription1", typeDescription);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = typeDescriptionImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(2);

        List<ImageWriterFile> imageWriterFilesList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFilesList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("typeDescriptionReward1.png");
        path1.SubDirectoryPath.Should().Be("typedescriptions");

        ImageWriterFile path2 = imageWriterFilesList[1];
        path2.ElementId.Should().Be("id1");
        path2.FileName.Should().Be("typeDescriptionLarge1.png");
        path2.SubDirectoryPath.Should().Be("typedescriptions");
    }

    [TestMethod]
    public async Task ProcessImageFile_FileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(TypeDescription));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, TypeDescription> elementsById = [];
        TypeDescription typeDescription = new("id1")
        {
            RewardIcon = "storm_ui_heroes_reward_icon_lootchestsummer2018epic.png",
            RewardIconPath = new RelativeFilePath
            {
                FilePath = Path.Combine(TestImagesDirectory, "storm_ui_heroes_rewardicons_sheet.dds"),
            },
            TextureSheet = new()
            {
                Columns = 5,
                Rows = 12,
            },
            IconSlot = 37,
            LargeIcon = "storm_ui_profile_hero_progression_icon_epicsummerlootchest.png",
            LargeIconPath = new RelativeFilePath
            {
                FilePath = Path.Combine(TestImagesDirectory, "storm_ui_profile_hero_progression_icon_epicsummerlootchest.dds"),
            },
        };

        elementsById.Add("typeDescription1", typeDescription);

        TypeDescriptionImageParser typeDescriptionImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = typeDescriptionImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_heroes_reward_icon_lootchestsummer2018epic.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_profile_hero_progression_icon_epicsummerlootchest.png")).Should().BeTrue();
    }
}
