namespace HeroesDataParser.Infrastructure.Commands.PortraitCommands.Tests;

[TestClass]
public class PortraitBattleNetCacheServiceTests
{
    private readonly ILogger<PortraitBattleNetCacheService> _logger;
    private readonly IOptions<PortraitBattleNetCacheOptions> _options;
    private readonly TestConsole _console;

    public PortraitBattleNetCacheServiceTests()
    {
        _logger = Substitute.For<ILogger<PortraitBattleNetCacheService>>();
        _options = Substitute.For<IOptions<PortraitBattleNetCacheOptions>>();
        _console = new TestConsole();
    }

    [TestMethod]
    public void CopyWaflFiles_NoWaflFiles_NothingCopied()
    {
        // arrange
        _options.Value.Returns(new PortraitBattleNetCacheOptions()
        {
            BattleNetCacheDirectory = "TestJsonFiles",
            OutputDirectory = Path.Combine("TestOutput", nameof(CopyWaflFiles_NoWaflFiles_NothingCopied)),
        });

        PortraitBattleNetCacheService portraitBattleNetCacheService = new(_logger, _options, _console);

        // act
        portraitBattleNetCacheService.CopyWaflFiles();

        // assert
        _console.Output.Should().Contain("No .wafl files found");
    }

    [TestMethod]
    public void CopyWaflFiles_HasWaflFiles_CopiesFiles()
    {
        // arrange
        _options.Value.Returns(new PortraitBattleNetCacheOptions()
        {
            BattleNetCacheDirectory = Path.Combine("TestImages", "PortraitsCache"),
            OutputDirectory = Path.Combine("TestOutput", nameof(CopyWaflFiles_HasWaflFiles_CopiesFiles)),
        });

        PortraitBattleNetCacheService portraitBattleNetCacheService = new(_logger, _options, _console);

        // act
        portraitBattleNetCacheService.CopyWaflFiles();

        // assert
        _console.Output.Should().Contain("All files copied successfully");

        Directory.GetFiles(Path.Combine("TestOutput", nameof(CopyWaflFiles_HasWaflFiles_CopiesFiles)), "*.*").Should().ContainSingle();
    }

    [TestMethod]
    public void CopyWaflFiles_HasWaflWithNonImageFiles_CopiesFiles()
    {
        // arrange
        _options.Value.Returns(new PortraitBattleNetCacheOptions()
        {
            BattleNetCacheDirectory = Path.Combine("TestImages", "PortraitsCacheNonImage"),
            OutputDirectory = Path.Combine("TestOutput", nameof(CopyWaflFiles_HasWaflWithNonImageFiles_CopiesFiles)),
        });

        PortraitBattleNetCacheService portraitBattleNetCacheService = new(_logger, _options, _console);

        // act
        portraitBattleNetCacheService.CopyWaflFiles();

        // assert
        _console.Output.Should().Contain("portraits_cache_not_image.wafl is not a valid");
        _console.Output.Should().Contain("2 out of 3 were copied successfully");

        Directory.GetFiles(Path.Combine("TestOutput", nameof(CopyWaflFiles_HasWaflWithNonImageFiles_CopiesFiles)), "*.*").Should().HaveCount(2);
    }
}
