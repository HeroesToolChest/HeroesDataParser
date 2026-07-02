namespace HeroesDataParser.Infrastructure.Processors;

public class MainLocaleProcessor : IMainLocaleProcessor
{
    private readonly ILogger<MainLocaleProcessor> _logger;
    private readonly RootOptions _options;
    private readonly IProcessorService _processorService;
    private readonly IMapProcessorService _mapProcessorService;

    public MainLocaleProcessor(
        ILogger<MainLocaleProcessor> logger,
        IOptions<RootOptions> options,
        IProcessorService processorService,
        IMapProcessorService mapProcessorService)
    {
        _logger = logger;
        _options = options.Value;
        _processorService = processorService;
        _mapProcessorService = mapProcessorService;
    }

    public async Task Run()
    {
        _logger.LogDebug("Starting processor service for {Locale}", _options.CurrentLocale);

        await _processorService.Start();

        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Map))
        {
            _logger.LogDebug("Starting map processor service for {Locale}", _options.CurrentLocale);
            await _mapProcessorService.Start();
        }
    }
}
