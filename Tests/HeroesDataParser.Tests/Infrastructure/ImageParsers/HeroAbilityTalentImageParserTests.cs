namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class HeroAbilityTalentImageParserTests : ImageWriterBase
{
    private readonly ILogger<HeroAbilityTalentImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public HeroAbilityTalentImageParserTests()
    {
        _logger = Substitute.For<ILogger<HeroAbilityTalentImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
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
            IconPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon1.dds") },
        });
        hero.AddAbility(new Ability()
        {
            Icon = "ability2.png",
            IconPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "ability_icon2.dds") },
        });
        hero.AddTalent(new Talent()
        {
            Icon = "talent1.png",
            IconPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "talent_icon1.dds") },
        });
        hero.AddTalent(new Talent()
        {
            Icon = "talent2.png",
            IconPath = new RelativeFilePath { FilePath = Path.Join(TestImagesDirectory, "talent_icon2.dds") },
        });

        elementsById.Add("hero1", hero);

        // act
        HashSet<ImageWriterFile> imageWriterFiles = heroAbilityTalentImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(4);

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile abilityPath1 = imageWriterFileList[0];
        abilityPath1.ElementId.Should().Be("id1");
        abilityPath1.FileName.Should().Be("ability1.png");
        abilityPath1.SubDirectoryPath.Should().Be("abilitytalents");

        ImageWriterFile abilityPath2 = imageWriterFileList[1];
        abilityPath2.ElementId.Should().Be("id1");
        abilityPath2.FileName.Should().Be("ability2.png");
        abilityPath2.SubDirectoryPath.Should().Be("abilitytalents");

        ImageWriterFile talentPath1 = imageWriterFileList[2];
        talentPath1.ElementId.Should().Be("id1");
        talentPath1.FileName.Should().Be("talent1.png");
        talentPath1.SubDirectoryPath.Should().Be("abilitytalents");

        ImageWriterFile talentPath2 = imageWriterFileList[3];
        talentPath2.ElementId.Should().Be("id1");
        talentPath2.FileName.Should().Be("talent2.png");
        talentPath2.SubDirectoryPath.Should().Be("abilitytalents");
    }
}