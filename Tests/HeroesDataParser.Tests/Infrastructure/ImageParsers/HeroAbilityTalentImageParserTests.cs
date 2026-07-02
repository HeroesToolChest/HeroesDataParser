namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class HeroAbilityTalentImageParserTests : ImageWriterBase
{
    private readonly ILogger<HeroAbilityTalentImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public HeroAbilityTalentImageParserTests()
    {
        _logger = Substitute.For<ILogger<HeroAbilityTalentImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        HeroAbilityTalentImageParser heroAbilityTalentImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, Hero> elementsById = [];

        Hero hero = new("id1");
        hero.AddAbility(new Ability()
        {
            Icon = "ability1.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon1.dds") },
        });
        hero.AddAbility(new Ability()
        {
            Icon = "ability2.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon2.dds") },
        });
        hero.AddTalent(new Talent()
        {
            Icon = "talent1.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "talent_icon1.dds") },
        });
        hero.AddTalent(new Talent()
        {
            Icon = "talent2.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "talent_icon2.dds") },
        });

        Unit unit = new("unitId1");
        unit.AddAbility(new Ability()
        {
            Icon = "unitAbility1.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "unitAbility_icon1.dds") },
        });
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

        hero.HeroUnits.Add(unit.Id, unit);
        elementsById.Add("hero1", hero);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = heroAbilityTalentImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(6);

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile abilityPath1 = imageWriterFileList[0];
        abilityPath1.ElementId.Should().Be("id1");
        abilityPath1.FileName.Should().Be("ability1.png");
        abilityPath1.SubDirectoryPath.Should().Be("abilitytalents");

        ImageWriterFile abilityPath2 = imageWriterFileList[1];
        abilityPath2.ElementId.Should().Be("id1");
        abilityPath2.FileName.Should().Be("ability2.png");
        abilityPath2.SubDirectoryPath.Should().Be("abilitytalents");

        ImageWriterFile abilityPath3 = imageWriterFileList[2];
        abilityPath3.ElementId.Should().Be("id1");
        abilityPath3.FileName.Should().Be("unitAbility1.png");
        abilityPath3.SubDirectoryPath.Should().Be("abilitytalents");

        ImageWriterFile abilityPath4 = imageWriterFileList[3];
        abilityPath4.ElementId.Should().Be("id1");
        abilityPath4.FileName.Should().Be("unitAbility2.png");
        abilityPath4.SubDirectoryPath.Should().Be("abilitytalents");

        ImageWriterFile talentPath1 = imageWriterFileList[4];
        talentPath1.ElementId.Should().Be("id1");
        talentPath1.FileName.Should().Be("talent1.png");
        talentPath1.SubDirectoryPath.Should().Be("abilitytalents");

        ImageWriterFile talentPath2 = imageWriterFileList[5];
        talentPath2.ElementId.Should().Be("id1");
        talentPath2.FileName.Should().Be("talent2.png");
        talentPath2.SubDirectoryPath.Should().Be("abilitytalents");
    }

    [TestMethod]
    public async Task ProcessImageFile_FileExists_ImagesAreCreated()
    {
        // arrange
        string outputImageDirectory = Path.Combine(OutputBaseDirectory, OutputImageDirectory, nameof(Hero));
        Directory.CreateDirectory(outputImageDirectory);

        SortedDictionary<string, Hero> elementsById = [];

        Hero hero = new("id1");
        hero.AddAbility(new Ability()
        {
            Icon = "storm_ui_icon_abathur_toxicnest.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "storm_ui_icon_abathur_toxicnest.dds") },
        });
        hero.AddTalent(new Talent()
        {
            Icon = "storm_ui_icon_alexstrasza_dragon_queen.png",
            IconPath = new ImagePath { FilePath = Path.Join(TestImagesDirectory, "storm_ui_icon_alexstrasza_dragon_queen.dds") },
        });

        elementsById.Add("hero1", hero);

        HeroAbilityTalentImageParser heroAbilityTalentImageParser = new(_logger, _heroesXmlLoaderService);
        HashSet<ImageWriterFile> imageWriterFiles = heroAbilityTalentImageParser.GetImages(elementsById);

        // act
        foreach (ImageWriterFile imageWriterFile in imageWriterFiles)
            await imageWriterFile.ProcessImageFile.Invoke(outputImageDirectory);

        // assert
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_icon_abathur_toxicnest.png")).Should().BeTrue();
        File.Exists(Path.Combine(outputImageDirectory, "storm_ui_icon_alexstrasza_dragon_queen.png")).Should().BeTrue();
    }
}