using HeroesDataParser.Tests.Infrastructure.ImageWriters;

namespace HeroesDataParser.Infrastructure.ImageWriters.Tests;

[TestClass]
public class HeroAbilityImageWriterTests : ImageWriterBase
{
    private readonly ILogger<HeroAbilityImageWriter> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public HeroAbilityImageWriterTests()
    {
        _logger = Substitute.For<ILogger<HeroAbilityImageWriter>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = HeroesXmlLoader.LoadWithEmpty();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public async Task WriteImages_HasImages_NewImagesAreCreated()
    {
        // arrange
        const string testDirectory = "heroAbility";
        _options.Value.Returns(new RootOptions()
        {
            OutputDirectory = Path.Join(OutputBaseDirectory, testDirectory),
        });

        HeroAbilityImageWriter heroAbilityImageWriter = new(_logger, _options, _heroesXmlLoaderService);

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
        await heroAbilityImageWriter.WriteImages(elementsById);

        // assert
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "abilities", "ability1.png")).Should().BeTrue();
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "abilities", "ability2.png")).Should().BeTrue();
    }
}