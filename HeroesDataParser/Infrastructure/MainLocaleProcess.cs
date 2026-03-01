namespace HeroesDataParser.Infrastructure;

public class MainLocaleProcess : IMainLocaleProcess
{
    private readonly ILogger<MainLocaleProcess> _logger;
    private readonly RootOptions _options;
    private readonly IProcessorService _processorService;
    private readonly IMapProcessorService _mapProcessorService;

    public MainLocaleProcess(
        ILogger<MainLocaleProcess> logger,
        IOptions<RootOptions> options,
        IProcessorService processorService,
        IMapProcessorService mapProcessorService)
    {
        _logger = logger;
        _options = options.Value;
        _processorService = processorService;
        _mapProcessorService = mapProcessorService;
    }

    public async Task Run(StormLocale locale)
    {
        _logger.LogInformation("Starting processor service for {Locale}", locale);

        await _processorService.Start();

        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Map))
        {
            _logger.LogInformation("Starting map processor service for {Locale}", locale);
            await _mapProcessorService.Start();
        }
    }
}
