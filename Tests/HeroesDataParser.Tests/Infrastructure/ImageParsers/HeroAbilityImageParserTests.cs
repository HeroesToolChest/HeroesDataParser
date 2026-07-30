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

        Ability heroAbility1 = new()
        {
            Icon = "ability1.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon1.dds") },
        };

        hero.AddAbility(heroAbility1);

        hero.AddAbility(new Ability()
        {
            Icon = "ability2.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon2.dds") },
        });

        hero.AssignSubAbilityToLink(
            new Ability()
            {
                Icon = "heroSubAbility1.png",
                IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "heroSubAbility_icon1.dds") },
            },
            heroAbility1.LinkId);

        hero.AssignSubAbilityToLink(
            new Ability()
            {
                Icon = "heroSubAbility2.png",
                IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "heroSubAbility_icon2.dds") },
            },
            heroAbility1.LinkId);

        Unit unit = new("unitId1");

        Ability unitAbility1 = new()
        {
            Icon = "unitAbility1.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "unitAbility_icon1.dds") },
        };
        unit.AddAbility(unitAbility1);

        unit.AddAbility(new Ability()
        {
            Icon = "unitAbility2.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "unitAbility_icon2.dds") },
        });

        unit.AddAbility(new Ability()
        {
            Icon = "ability1.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon1.dds") },
        });

        unit.AssignSubAbilityToLink(
            new Ability()
            {
                Icon = "unitSubAbility1.png",
                IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "unitSubAbility_icon1.dds") },
            },
            unitAbility1.LinkId);

        hero.HeroUnits.Add(unit.Id, unit);
        elementsById.Add("hero1", hero);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = heroAbilityImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(7);

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile path1 = imageWriterFileList[0];
        path1.ElementId.Should().Be("id1");
        path1.FileName.Should().Be("ability1.png");
        path1.SubDirectoryPath.Should().Be("abilities");

        ImageWriterFile path2 = imageWriterFileList[1];
        path2.ElementId.Should().Be("id1");
        path2.FileName.Should().Be("ability2.png");
        path2.SubDirectoryPath.Should().Be("abilities");

        ImageWriterFile heroSubAbilityPath1 = imageWriterFileList[2];
        heroSubAbilityPath1.ElementId.Should().Be("id1");
        heroSubAbilityPath1.FileName.Should().Be("heroSubAbility1.png");
        heroSubAbilityPath1.SubDirectoryPath.Should().Be("abilities");

        ImageWriterFile heroSubAbilityPath2 = imageWriterFileList[3];
        heroSubAbilityPath2.ElementId.Should().Be("id1");
        heroSubAbilityPath2.FileName.Should().Be("heroSubAbility2.png");
        heroSubAbilityPath2.SubDirectoryPath.Should().Be("abilities");

        ImageWriterFile path3 = imageWriterFileList[4];
        path3.ElementId.Should().Be("id1");
        path3.FileName.Should().Be("unitAbility1.png");
        path3.SubDirectoryPath.Should().Be("abilities");

        ImageWriterFile path4 = imageWriterFileList[5];
        path4.ElementId.Should().Be("id1");
        path4.FileName.Should().Be("unitAbility2.png");
        path4.SubDirectoryPath.Should().Be("abilities");

        ImageWriterFile unitSubAbilityPath1 = imageWriterFileList[6];
        unitSubAbilityPath1.ElementId.Should().Be("id1");
        unitSubAbilityPath1.FileName.Should().Be("unitSubAbility1.png");
        unitSubAbilityPath1.SubDirectoryPath.Should().Be("abilities");
    }

    [TestMethod]
    public async Task ProcessImageFile_FileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(Ability));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Hero> elementsById = [];

        Hero hero = new("id1");

        Ability heroAbility = new()
        {
            Icon = "storm_ui_icon_alexstrasza_dragon_queen.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "storm_ui_icon_alexstrasza_dragon_queen.dds") },
        };

        hero.AddAbility(heroAbility);

        hero.AssignSubAbilityToLink(
            new Ability()
            {
                Icon = "storm_ui_icon_abathur_toxicnest.png",
                IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "storm_ui_icon_abathur_toxicnest.dds") },
            },
            heroAbility.LinkId);

        elementsById.Add("hero1", hero);

        HeroAbilityImageParser heroAbilityImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = heroAbilityImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_icon_alexstrasza_dragon_queen.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_icon_abathur_toxicnest.png")).Should().BeTrue();
    }
}