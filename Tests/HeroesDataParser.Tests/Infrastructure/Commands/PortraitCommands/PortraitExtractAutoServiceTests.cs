namespace HeroesDataParser.Infrastructure.Commands.PortraitCommands.Tests;

[TestClass]
public class PortraitExtractAutoServiceTests
{
    private readonly ILogger<PortraitExtractAutoService> _logger;
    private readonly IOptions<PortraitExtractAutoOptions> _options;
    private readonly TestConsole _console;

    public PortraitExtractAutoServiceTests()
    {
        _logger = Substitute.For<ILogger<PortraitExtractAutoService>>();
        _options = Substitute.For<IOptions<PortraitExtractAutoOptions>>();
        _console = new TestConsole();
    }

    [TestMethod]
    public void Extract_NoTextureSheets_ReturnsDisplay()
    {
        // arrange
        _options.Value.Returns(new PortraitExtractAutoOptions()
        {
            BattleNetCacheDirectory = "TestJsonFiles",
            XmlConfigFilePath = Path.Combine("TestConfigFiles", "portrait-extract-empty.xml"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(Extract_NoTextureSheets_ReturnsDisplay)),
        });

        PortraitExtractAutoService service = new(_logger, _options, _console);

        // act
        service.Extract();

        // assert
        _console.Output.Should().Contain("No texture sheets were found to auto-extract in the XML configuration");
    }

    [TestMethod]
    public void Extract_TextureSheets_ExtractsPortraits()
    {
        // arrange
        _options.Value.Returns(new PortraitExtractAutoOptions()
        {
            BattleNetCacheDirectory = Path.Combine("TestImages", "PortraitsCache"),
            RewardPortraitDataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
            XmlConfigFilePath = Path.Combine("TestConfigFiles", "portrait-extract-found.xml"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(Extract_TextureSheets_ExtractsPortraits)),
        });

        PortraitExtractAutoService service = new(_logger, _options, _console);

        // act
        service.Extract();

        // assert
        _console.Output.Should().Contain("There are 1 texture sheets to be found in the cache for auto-extraction");
        _console.Output.Should().Contain("There are 44 texture sheets found in the reward data json file");
        _console.Output.Should().Contain("Extracting portraits from texture sheet...");
        _console.Output.Should().Contain("storm_portrait_abathurepic.png");
        _console.Output.Should().Contain("storm_portrait_zeratulepic.png");
        _console.Output.Should().Contain("36 portrait images extracted to");
        _console.Output.Should().Contain("1 out of 1 texture sheets were found in the cache");
        _console.Output.Should().Contain("auto-extraction xml file (need to be added)");
        _console.Output.Should().Contain("ui_heroes_portraits_sheet9.png");

        Directory.GetFiles(Path.Combine(TestConstants.TestDirectory, nameof(Extract_TextureSheets_ExtractsPortraits)), "*.png").Should().HaveCount(36);
    }

    [TestMethod]
    public void Extract_TextureSheetsPartialFind_ExtractsPortraits()
    {
        // arrange
        _options.Value.Returns(new PortraitExtractAutoOptions()
        {
            BattleNetCacheDirectory = Path.Combine("TestImages", "PortraitsCache"),
            RewardPortraitDataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
            XmlConfigFilePath = Path.Combine("TestConfigFiles", "portrait-extract.xml"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(Extract_TextureSheetsPartialFind_ExtractsPortraits)),
        });

        PortraitExtractAutoService service = new(_logger, _options, _console);

        // act
        service.Extract();

        // assert
        _console.Output.Should().Contain("There are 2 texture sheets to be found in the cache for auto-extraction");
        _console.Output.Should().Contain("There are 44 texture sheets found in the reward data json file");
        _console.Output.Should().Contain("Could not find the file ");
        _console.Output.Should().Contain("Extracting portraits from texture sheet...");
        _console.Output.Should().Contain("storm_portrait_abathurepic.png");
        _console.Output.Should().Contain("storm_portrait_zeratulepic.png");
        _console.Output.Should().Contain("36 portrait images extracted to");
        _console.Output.Should().Contain("Only 1 out of 2 texture sheets were found in the cache");
        _console.Output.Should().Contain("auto-extraction xml file (need to be added)");
        _console.Output.Should().Contain("ui_heroes_portraits_sheet9.png");

        Directory.GetFiles(Path.Combine(TestConstants.TestDirectory, nameof(Extract_TextureSheetsPartialFind_ExtractsPortraits)), "*.png").Should().HaveCount(36);
    }

    [TestMethod]
    public void Extract_RewardPortraitsAllFound_ExtractsPortraits()
    {
        // arrange
        _options.Value.Returns(new PortraitExtractAutoOptions()
        {
            BattleNetCacheDirectory = Path.Combine("TestImages", "PortraitsCache"),
            RewardPortraitDataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus_1.json"),
            XmlConfigFilePath = Path.Combine("TestConfigFiles", "portrait-extract-found.xml"),
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(Extract_RewardPortraitsAllFound_ExtractsPortraits)),
        });

        PortraitExtractAutoService service = new(_logger, _options, _console);

        // act
        service.Extract();

        // assert
        _console.Output.Should().Contain("There are 1 texture sheets to be found in the cache for auto-extraction");
        _console.Output.Should().Contain("There are 1 texture sheets found in the reward data json file");
        _console.Output.Should().Contain("Extracting portraits from texture sheet...");
        _console.Output.Should().Contain("ui_heroes_portraits_sheet2.png");
        _console.Output.Should().Contain("storm_portrait_abathurepic.png");
        _console.Output.Should().Contain("1 portrait images extracted to ");
        _console.Output.Should().Contain("1 out of 1 texture sheets were found in the cache");
        _console.Output.Should().Contain("All texture sheets in the reward data json file were found in the cache");

        Directory.GetFiles(Path.Combine(TestConstants.TestDirectory, nameof(Extract_RewardPortraitsAllFound_ExtractsPortraits)), "*.png").Should().ContainSingle();
    }
}
