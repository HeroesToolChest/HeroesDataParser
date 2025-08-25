using HeroesDataParser.Tests.Infrastructure.ImageWriters;

namespace HeroesDataParser.Infrastructure.ImageWriters.Tests;

[TestClass]
public class HeroTalentImageWriterTests : ImageWriterBase
{
    private readonly ILogger<HeroTalentImageWriter> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public HeroTalentImageWriterTests()
    {
        _logger = Substitute.For<ILogger<HeroTalentImageWriter>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = HeroesXmlLoader.LoadWithEmpty();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public async Task WriteImages_HasImages_NewImagesAreCreated()
    {
        // arrange
        const string testDirectory = "heroTalent";
        _options.Value.Returns(new RootOptions()
        {
            OutputDirectory = Path.Join(OutputBaseDirectory, testDirectory),
        });

        HeroTalentImageWriter heroTalentImageWriter = new(_logger, _options, _heroesXmlLoaderService);

        Dictionary<string, Hero> elementsById = [];

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
        await heroTalentImageWriter.WriteImages(elementsById);

        // assert
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "talents", "talent1.png")).Should().BeTrue();
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "talents", "talent2.png")).Should().BeTrue();
    }
}