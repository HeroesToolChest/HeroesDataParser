namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class HeroAbilityImageParserTests : ImageWriterBase
{
    private readonly ILogger<HeroAbilityImageParser> _logger;

    public HeroAbilityImageParserTests()
    {
        _logger = Substitute.For<ILogger<HeroAbilityImageParser>>();
    }

    [TestMethod]
    public void SaveImages_HasImages_GetImagePaths()
    {
        // arrange
        HeroAbilityImageParser heroAbilityImageParser = new(_logger);

        Dictionary<string, Hero> elementsById = [];

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
        HashSet<ImageWriterPath> imagePaths = heroAbilityImageParser.GetImages(elementsById);

        // assert
        imagePaths.Should().HaveCount(2);

        List<ImageWriterPath> imagePathsList = [.. imagePaths];

        ImageWriterPath path1 = imagePathsList[0];
        path1.ElementId.Should().Be("id1");
        path1.RelativeFilePath.Should().Be(Path.Join("TestImages", "ability_icon1.dds"));
        path1.RelativeMpqFilePath.Should().BeNull();
        path1.FileName.Should().Be("ability1.png");
        path1.SubDirectoryPath.Should().Be("abilities");

        ImageWriterPath path2 = imagePathsList[1];
        path2.ElementId.Should().Be("id1");
        path2.RelativeFilePath.Should().Be(Path.Join("TestImages", "ability_icon2.dds"));
        path2.RelativeMpqFilePath.Should().BeNull();
        path2.FileName.Should().Be("ability2.png");
        path2.SubDirectoryPath.Should().Be("abilities");
    }
}