namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class HeroAbilityImageParserTests : ImageWriterBase
{
    private readonly ILogger<HeroAbilityImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public HeroAbilityImageParserTests()
    {
        _logger = Substitute.For<ILogger<HeroAbilityImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        HeroAbilityImageParser heroAbilityImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, Hero> elementsById = [];

        Hero hero = new("id1");
        hero.AddAbility(new Ability()
        {
            Icon = "ability1.png",
            IconPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon1.dds") },
        });
        hero.AddAbility(new Ability()
        {
            Icon = "ability2.png",
            IconPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon2.dds") },
        });

        elementsById.Add("hero1", hero);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = heroAbilityImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(2);

        List<ImageWriterFile> imageWriterFilesList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFilesList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("ability1.png");
        path1.SubDirectoryPath.Should().Be("abilities");

        ImageWriterFile path2 = imageWriterFilesList[1];
        path2.ElementId.Should().Be("id1");
        path2.FileName.Should().Be("ability2.png");
        path2.SubDirectoryPath.Should().Be("abilities");
    }

    [TestMethod]
    public async Task ProcessImageFile_FileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(Ability));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Hero> elementsById = [];

        Hero hero = new("id1");
        hero.AddAbility(new Ability()
        {
            Icon = "storm_ui_icon_alexstrasza_dragon_queen.png",
            IconPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "storm_ui_icon_alexstrasza_dragon_queen.dds") },
        });

        elementsById.Add("hero1", hero);

        HeroAbilityImageParser heroAbilityImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = heroAbilityImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_icon_alexstrasza_dragon_queen.png")).Should().BeTrue();
    }
}