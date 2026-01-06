namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class UnitAbilityImageParserTests : ImageWriterBase
{
    private readonly ILogger<UnitAbilityImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public UnitAbilityImageParserTests()
    {
        _logger = Substitute.For<ILogger<UnitAbilityImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        UnitAbilityImageParser unitAbilityImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, Unit> elementsById = [];

        Unit unit = new("id1");
        unit.AddAbility(new Ability()
        {
            Icon = "ability1.png",
            IconPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon1.dds") },
        });
        unit.AddAbility(new Ability()
        {
            Icon = "ability2.png",
            IconPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon2.dds") },
        });

        elementsById.Add("unit1", unit);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = unitAbilityImageParser.GetImages(elementsById);

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

        SortedDictionary<string, Unit> elementsById = [];

        Unit unit = new("id1");
        unit.AddAbility(new Ability()
        {
            Icon = "storm_temp_war3_btnskeletonwarrior.png",
            IconPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "storm_temp_war3_btnskeletonwarrior.dds") },
        });

        elementsById.Add("unit1", unit);

        UnitAbilityImageParser unitAbilityImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = unitAbilityImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_temp_war3_btnskeletonwarrior.png")).Should().BeTrue();
    }
}
