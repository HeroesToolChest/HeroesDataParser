namespace HeroesDataParser.Infrastructure.ImageParsers.Tests;

[TestClass]
public class HeroTalentImageParserTests : ImageWriterBase
{
    private readonly ILogger<HeroTalentImageParser> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public HeroTalentImageParserTests()
    {
        _logger = Substitute.For<ILogger<HeroTalentImageParser>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
    }

    [TestMethod]
    public void GetImages_HasImages_GetImagePaths()
    {
        // arrange
        HeroTalentImageParser heroTalentImageParser = new(_logger, _heroesXmlLoaderService);

        SortedDictionary<string, Hero> elementsById = [];

        Hero hero = new("id1");
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
        HashSet<ImageWriterFile> imageWriterFiles = heroTalentImageParser.GetImages(elementsById);

        // assert
        imageWriterFiles.Should().HaveCount(2);

        List<ImageWriterFile> imageWriterFileList = [.. imageWriterFiles];

        ImageWriterFile talentPath1 = imageWriterFileList[0];
        talentPath1.ElementId.Should().Be("id1");
        talentPath1.FileName.Should().Be("talent1.png");
        talentPath1.SubDirectoryPath.Should().Be("talents");

        ImageWriterFile talentPath2 = imageWriterFileList[1];
        talentPath2.ElementId.Should().Be("id1");
    }
}