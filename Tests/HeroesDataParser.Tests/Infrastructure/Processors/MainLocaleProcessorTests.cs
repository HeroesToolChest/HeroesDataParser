namespace HeroesDataParser.Infrastructure.Processors.Tests;

[TestClass]
public class MainLocaleProcessorTests
{
    private readonly ILogger<MainLocaleProcessor> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IProcessorService _processorService;
    private readonly IMapProcessorService _mapProcessorService;

    public MainLocaleProcessorTests()
    {
        _logger = Substitute.For<ILogger<MainLocaleProcessor>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _processorService = Substitute.For<IProcessorService>();
        _mapProcessorService = Substitute.For<IMapProcessorService>();
    }

    [TestMethod]
    public async Task Run_ExtractDataOptionsDoesNotIncludeMap_StartsProcessorServiceOnly()
    {
        // arrange
        _options.Value.Returns(new RootOptions
        {
            ExtractDataOptions = ExtractDataOptions.Hero,
        });

        MainLocaleProcessor processor = new(_logger, _options, _processorService, _mapProcessorService);

        // act
        await processor.Run(StormLocale.ENUS);

        // assert
        await _processorService.Received(1).Start();
        await _mapProcessorService.DidNotReceive().Start();
    }

    [TestMethod]
    public async Task Run_ExtractDataOptionsIncludesMap_StartsBothProcessorAndMapProcessorServices()
    {
        // arrange
        _options.Value.Returns(new RootOptions
        {
            ExtractDataOptions = ExtractDataOptions.Map,
        });

        MainLocaleProcessor processor = new(_logger, _options, _processorService, _mapProcessorService);

        // act
        await processor.Run(StormLocale.ENUS);

        // assert
        await _processorService.Received(1).Start();
        await _mapProcessorService.Received(1).Start();
    }

    [TestMethod]
    public async Task Run_ExtractDataOptionsIncludesMapAmongOthers_StartsMapProcessorService()
    {
        // arrange
        _options.Value.Returns(new RootOptions
        {
            ExtractDataOptions = ExtractDataOptions.Hero | ExtractDataOptions.Map | ExtractDataOptions.Skin,
        });

        MainLocaleProcessor processor = new(_logger, _options, _processorService, _mapProcessorService);

        // act
        await processor.Run(StormLocale.ENUS);

        // assert
        await _processorService.Received(1).Start();
        await _mapProcessorService.Received(1).Start();
    }

    [TestMethod]
    public async Task Run_ExtractDataOptionsIsNone_StartsProcessorServiceOnly()
    {
        // arrange
        _options.Value.Returns(new RootOptions
        {
            ExtractDataOptions = ExtractDataOptions.None,
        });

        MainLocaleProcessor processor = new(_logger, _options, _processorService, _mapProcessorService);

        // act
        await processor.Run(StormLocale.ENUS);

        // assert
        await _processorService.Received(1).Start();
        await _mapProcessorService.DidNotReceive().Start();
    }
}
