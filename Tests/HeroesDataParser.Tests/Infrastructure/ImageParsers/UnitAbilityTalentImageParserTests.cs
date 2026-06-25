namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class UnitAbilityTalentImageParserTests : ImageWriterBase
{
    private readonly ILogger<UnitAbilityTalentImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public UnitAbilityTalentImageParserTests()
    {
        _logger = Substitute.For<ILogger<UnitAbilityTalentImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        UnitAbilityTalentImageParser unitAbilityTalentImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, Unit> elementsById = [];

        Unit unit = new("id1");
        unit.AddAbility(new Ability()
        {
            Icon = "ability1.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon1.dds") },
        });
        unit.AddAbility(new Ability()
        {
            Icon = "ability2.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon2.dds") },
        });

        elementsById.Add("unit1", unit);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = unitAbilityTalentImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(2);

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile abilityPath1 = imageWriterFileList[0];
        abilityPath1.ElementId.Should().Be("id1");
        abilityPath1.FileName.Should().Be("ability1.png");
        abilityPath1.SubDirectoryPath.Should().Be("abilitytalents");

        ImageWriterFile abilityPath2 = imageWriterFileList[1];
        abilityPath2.ElementId.Should().Be("id1");
        abilityPath2.FileName.Should().Be("ability2.png");
        abilityPath2.SubDirectoryPath.Should().Be("abilitytalents");
    }

    [TestMethod]
    public async Task ProcessImageFile_FileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(Unit));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Unit> elementsById = [];

        Unit unit = new("id1");
        unit.AddAbility(new Ability()
        {
            Icon = "storm_temp_war3_btnskeletonarcher.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "storm_temp_war3_btnskeletonarcher.dds") },
        });

        elementsById.Add("unit1", unit);

        UnitAbilityTalentImageParser unitAbilityTalentImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = unitAbilityTalentImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_temp_war3_btnskeletonarcher.png")).Should().BeTrue();
    }
}
