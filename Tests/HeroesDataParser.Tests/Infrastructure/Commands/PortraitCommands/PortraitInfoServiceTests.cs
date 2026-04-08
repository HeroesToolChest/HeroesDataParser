using HeroesDataParser.Infrastructure.Commands.PortraitCommands;

namespace HeroesDataParser.Tests.Infrastructure.Commands.PortraitCommands;

[TestClass]
public class PortraitInfoServiceTests
{
    private readonly ILogger<PortraitInfoService> _logger;
    private readonly IOptions<PortraitInfoOptions> _options;
    private readonly TestConsole _console;

    public PortraitInfoServiceTests()
    {
        _logger = Substitute.For<ILogger<PortraitInfoService>>();
        _options = Substitute.For<IOptions<PortraitInfoOptions>>();
        _console = new TestConsole();
    }

    [TestMethod]
    public void DisplayInfo_DisplayTextureSheetImageFileNames_Displays()
    {
        // arrange
        _options.Value.Returns(new PortraitInfoOptions()
        {
            RewardPortraitDataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
            ShowTextureSheetsFileNames = true,
        });

        PortraitInfoService portraitInfoService = new(_logger, _options, _console);

        // act
        portraitInfoService.DisplayInfo();

        // assert
        _console.Output.Should().Contain("There are 44 texture sheet image file names in the reward portrait data json");
        _console.Output.Should().Contain("ui_heroes_portraits_sheet19.png");
        _console.Output.Should().Contain("ui_heroes_portraits_sheet36.png");
        _console.Output.Should().Contain("ui_heroes_portraits_sheet9.png");
    }

    [TestMethod]
    public void DisplayInfo_DisplayTextureSheetImageFileNamesWithEmptyRewardData_Displays()
    {
        // arrange
        _options.Value.Returns(new PortraitInfoOptions()
        {
            RewardPortraitDataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus_empty.json"),
            ShowTextureSheetsFileNames = true,
        });

        PortraitInfoService portraitInfoService = new(_logger, _options, _console);

        // act
        portraitInfoService.DisplayInfo();

        // assert
        _console.Output.Should().Contain("There are 0 texture sheet image files in the reward portrait data json file");
    }

    [TestMethod]
    public void DisplayInfo_DisplayFileNamesWithIconSlot_Displays()
    {
        // arrange
        _options.Value.Returns(new PortraitInfoOptions()
        {
            RewardPortraitDataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
            ShowIconSlotFileNames = 0,
        });

        PortraitInfoService portraitInfoService = new(_logger, _options, _console);

        // act
        portraitInfoService.DisplayInfo();

        // assert
        _console.Output.Should().Contain("There are 4 texture sheet image file names that do not have the icon slot 0");
        _console.Output.Should().Contain("ui_heroes_portraits_sheet41.png");
        _console.Output.Should().Contain("ui_heroes_portraits_sheet44.png");
        _console.Output.Should().Contain("All the 40 texture sheet image file names that have icon slot 0:");
        _console.Output.Should().Contain("│ Name                     │ Id                      │ Texture Sheet           │");
        _console.Output.Should().Contain("Zergling Stocking");
    }

    [TestMethod]
    public void DisplayInfo_DisplayFileNamesWithIconSlotWithEmptyRewardData_Displays()
    {
        // arrange
        _options.Value.Returns(new PortraitInfoOptions()
        {
            RewardPortraitDataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus_empty.json"),
            ShowIconSlotFileNames = 0,
        });

        PortraitInfoService portraitInfoService = new(_logger, _options, _console);

        // act
        portraitInfoService.DisplayInfo();

        // assert
        _console.Output.Should().Contain("There are no texture sheet image file names that have icon slot 0");
    }

    [TestMethod]
    public void DisplayInfo_DisplayFileNamesWithIconSlotWithLocalizedTextExtractRewardData_DisplaysWarning()
    {
        // arrange
        _options.Value.Returns(new PortraitInfoOptions()
        {
            RewardPortraitDataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus_empty_lt_extract.json"),
            ShowIconSlotFileNames = 0,
        });

        PortraitInfoService portraitInfoService = new(_logger, _options, _console);

        // act
        portraitInfoService.DisplayInfo();

        // assert
        _console.Output.Should().Contain("Localized text is set to 'Extract'");
    }

    [TestMethod]
    public void DisplayInfo_DisplayPortraitsFromTextureSheetImage_Displays()
    {
        // arrange
        _options.Value.Returns(new PortraitInfoOptions()
        {
            RewardPortraitDataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
            TextureSheetImageName = "ui_heroes_portraits_sheet9.png",
        });

        PortraitInfoService portraitInfoService = new(_logger, _options, _console);

        // act
        portraitInfoService.DisplayInfo();

        // assert
        _console.Output.Should().Contain("There are 36 images in 'ui_heroes_portraits_sheet9.png'");
        _console.Output.Should().Contain("│ Slot │ Name                         │ Id                                   │");
        _console.Output.Should().Contain("│   35 │ 2016 S3 Team League Portrait │ Season3TeamLeaguePortraitBronze      │");
    }

    [TestMethod]
    public void DisplayInfo_DisplayPortraitsFromTextureSheetImageWithNoPortraits_Displays()
    {
        // arrange
        _options.Value.Returns(new PortraitInfoOptions()
        {
            RewardPortraitDataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
            TextureSheetImageName = "other.png",
        });

        PortraitInfoService portraitInfoService = new(_logger, _options, _console);

        // act
        portraitInfoService.DisplayInfo();

        // assert
        _console.Output.Should().Contain("There are 0 images in 'other.png'");
    }
}
