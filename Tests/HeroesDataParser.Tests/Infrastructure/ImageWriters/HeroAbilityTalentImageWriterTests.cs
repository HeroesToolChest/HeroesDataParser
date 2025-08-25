using HeroesDataParser.Tests.Infrastructure.ImageWriters;

namespace HeroesDataParser.Infrastructure.ImageWriters.Tests;

[TestClass]
public class HeroAbilityTalentImageWriterTests : ImageWriterBase
{
    private readonly ILogger<HeroAbilityTalentImageWriter> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public HeroAbilityTalentImageWriterTests()
    {
        _logger = Substitute.For<ILogger<HeroAbilityTalentImageWriter>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = HeroesXmlLoader.LoadWithEmpty();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public async Task WriteImages_HasImages_NewImagesAreCreated()
    {
        // arrange
        const string testDirectory = "heroAbilityTalent";
        _options.Value.Returns(new RootOptions()
        {
            OutputDirectory = Path.Join(OutputBaseDirectory, testDirectory),
        });

        HeroAbilityTalentImageWriter heroAbilityTalentImageWriter = new(_logger, _options, _heroesXmlLoaderService);

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
        await heroAbilityTalentImageWriter.WriteImages(elementsById);

        // assert
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "abilityTalents", "ability1.png")).Should().BeTrue();
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "abilityTalents", "ability2.png")).Should().BeTrue();
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "abilityTalents", "talent1.png")).Should().BeTrue();
        File.Exists(Path.Join(OutputBaseDirectory, testDirectory, OutputImageDirectory, "abilityTalents", "talent2.png")).Should().BeTrue();
    }
}