namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class RewardLicenseParserTests
{
    private readonly ILogger<RewardLicenseParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public RewardLicenseParserTests()
    {
        _logger = Substitute.For<ILogger<RewardLicenseParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    [DataRow("Adjutant", 28708L)] // CRewardAnnouncerPack
    [DataRow("BannerDFEsportsWarChest18RareBallistix", 30103L)] // CRewardBanner
    [DataRow("AbathurEmoticonPack", 19110L)] // CRewardEmoticonPack
    [DataRow("Abathur", 10219L)] // CRewardHero
    [DataRow("MountBearRank17Base", 23823L)] // CRewardMount
    [DataRow("AbathurToys18Portrait", 35888L)] // CRewardPortrait
    [DataRow("AbathurMecha", 26728L)] // CRewardSkin
    [DataRow("Classic21Diablospray", 266134L)] // CRewardSpray
    [DataRow("AbathurBase_VoiceLine02", 18736L)] // CRewardVoiceLine
    public void Parse_RewardWithLicense_ReturnsRewardLicenseData(string id, long licenseId)
    {
        // arrange
        RewardLicenseParser rewardLicenseParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        RewardLicense? rewardLicense = rewardLicenseParser.Parse(id);

        // assert
        rewardLicense.Should().NotBeNull();
        rewardLicense.Id.Should().Be(id);
        rewardLicense.LicenseId.Should().Be(licenseId);
    }

    [TestMethod]
    public void Parse_SylvanasBasicRewardHasNoLicense_ReturnsNull()
    {
        // arrange
        RewardLicenseParser rewardLicenseParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        RewardLicense? rewardLicense = rewardLicenseParser.Parse("SylvanasBasic");

        // assert
        rewardLicense.Should().BeNull();
    }

    [TestMethod]
    public void Parse_DefaultRewardElement_ReturnsNull()
    {
        // arrange
        RewardLicenseParser rewardLicenseParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        RewardLicense? rewardLicense = rewardLicenseParser.Parse("PortraitParent");

        // assert
        rewardLicense.Should().BeNull();
    }

    [TestMethod]
    public void Parse_RewardDoesNotExist_ReturnsNull()
    {
        // arrange
        RewardLicenseParser rewardLicenseParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        RewardLicense? rewardLicense = rewardLicenseParser.Parse("MountCloudVar1");

        // assert
        rewardLicense.Should().BeNull();
    }
}
