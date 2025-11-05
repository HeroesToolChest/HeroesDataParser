namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class HeroAbilityTalentImageParserTests : ImageWriterBase
{
    private readonly ILogger<HeroAbilityTalentImageParser> _logger;

    public HeroAbilityTalentImageParserTests()
    {
        _logger = Substitute.For<ILogger<HeroAbilityTalentImageParser>>();
    }

    [TestMethod]
    public void SaveImages_HasImages_GetImagePaths()
    {
        // arrange
        HeroAbilityTalentImageParser heroAbilityTalentImageParser = new(_logger);

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
        HashSet<ImageWriterPath> imagePaths = heroAbilityTalentImageParser.GetImages(elementsById);

        // assert
        imagePaths.Should().HaveCount(4);

        List<ImageWriterPath> imagePathsList = [.. imagePaths];

        ImageWriterPath abilityPath1 = imagePathsList[0];
        abilityPath1.ElementId.Should().Be("id1");
        abilityPath1.RelativeFilePath.Should().Be(Path.Join("TestImages", "ability_icon1.dds"));
        abilityPath1.RelativeMpqFilePath.Should().BeNull();
        abilityPath1.FileName.Should().Be("ability1.png");
        abilityPath1.SubDirectoryPath.Should().Be("abilitytalents");

        ImageWriterPath abilityPath2 = imagePathsList[1];
        abilityPath2.ElementId.Should().Be("id1");
        abilityPath2.RelativeFilePath.Should().Be(Path.Join("TestImages", "ability_icon2.dds"));
        abilityPath2.RelativeMpqFilePath.Should().BeNull();
        abilityPath2.FileName.Should().Be("ability2.png");
        abilityPath2.SubDirectoryPath.Should().Be("abilitytalents");

        ImageWriterPath talentPath1 = imagePathsList[2];
        talentPath1.ElementId.Should().Be("id1");
        talentPath1.RelativeFilePath.Should().Be(Path.Join("TestImages", "talent_icon1.dds"));
        talentPath1.RelativeMpqFilePath.Should().BeNull();
        talentPath1.FileName.Should().Be("talent1.png");
        talentPath1.SubDirectoryPath.Should().Be("abilitytalents");

        ImageWriterPath talentPath2 = imagePathsList[3];
        talentPath2.ElementId.Should().Be("id1");
        talentPath2.RelativeFilePath.Should().Be(Path.Join("TestImages", "talent_icon2.dds"));
        talentPath2.RelativeMpqFilePath.Should().BeNull();
        talentPath2.FileName.Should().Be("talent2.png");
        talentPath2.SubDirectoryPath.Should().Be("abilitytalents");
    }
}