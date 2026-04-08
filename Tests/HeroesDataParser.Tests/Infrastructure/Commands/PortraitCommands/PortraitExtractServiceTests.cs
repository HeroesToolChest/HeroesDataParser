namespace HeroesDataParser.Infrastructure.Commands.PortraitCommands.Tests;

[TestClass]
public class PortraitExtractServiceTests
{
    private readonly ILogger<PortraitExtractService> _logger;
    private readonly IOptions<PortraitExtractOptions> _options;
    private readonly TestConsole _console;

    public PortraitExtractServiceTests()
    {
        _logger = Substitute.For<ILogger<PortraitExtractService>>();
        _options = Substitute.For<IOptions<PortraitExtractOptions>>();
        _console = new TestConsole();
    }

    [TestMethod]
    public void DisplayAvailablePortraits_DisplayAvailablePortraits_Displays()
    {
        // arrange
        _options.Value.Returns(new PortraitExtractOptions()
        {
            RewardPortraitDataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
            RewardPortraitTextureSheetImage = "ui_heroes_portraits_sheet5.png",
        });

        PortraitExtractService service = new(_logger, _options, _console);

        // act
        service.DisplayAvailablePortraits();

        // assert
        _console.Output.Should().Contain("There are 36 images in 'ui_heroes_portraits_sheet5.png'");
        _console.Output.Should().Contain("│ Slot │ Name                              │ Id                                │");
        _console.Output.Should().Contain("35 │ Preseason Team League Portrait");
    }

    [TestMethod]
    public void DisplayInfo_DisplayPortraitsFromTextureSheetImageWithNoPortraits_Displays()
    {
        // arrange
        _options.Value.Returns(new PortraitExtractOptions()
        {
            RewardPortraitDataFilePath = Path.Combine("TestJsonFiles", "rewardportraitdata_96477_enus.json"),
            RewardPortraitTextureSheetImage = "other.png",
        });

        PortraitExtractService portraitExtractService = new(_logger, _options, _console);

        // act
        portraitExtractService.DisplayAvailablePortraits();

        // assert
        _console.Output.Should().Contain("There are 0 images in 'other.png'");
    }
}
