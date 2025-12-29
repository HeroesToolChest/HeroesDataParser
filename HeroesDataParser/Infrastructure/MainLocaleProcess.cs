namespace HeroesDataParser.Infrastructure;

public class MainLocaleProcess : IMainLocaleProcess
{
    private readonly ILogger<MainLocaleProcess> _logger;
    private readonly IProcessorService _processorService;
    private readonly IMapProcessorService _mapProcessorService;
    private readonly ISerializedDataStoreService _serializedDataStoreService;
    private readonly IJsonGameStringFileWriterService _jsonGameStringFileWriterService;

    public MainLocaleProcess(
        ILogger<MainLocaleProcess> logger,
        IProcessorService processorService,
        IMapProcessorService mapProcessorService,
        ISerializedDataStoreService serializedDataStoreService,
        IJsonGameStringFileWriterService jsonGameStringFileWriterService)
    {
        _logger = logger;
        _processorService = processorService;
        _mapProcessorService = mapProcessorService;
        _serializedDataStoreService = serializedDataStoreService;
        _jsonGameStringFileWriterService = jsonGameStringFileWriterService;
    }

    public async Task Run(StormLocale locale)
    {
        _logger.LogInformation("Starting processor service for {Locale}", locale);

        await _processorService.Start();

        if (_processorService.ExtractDataOptions.HasFlag(ExtractDataOptions.Map))
        {
            _logger.LogInformation("Starting map processor service for {Locale}", locale);
            await _mapProcessorService.Start();
        }
        else if (_serializedDataStoreService.TryGetSerializedData(Constants.GameStringFilePrefix, out byte[]? gameStringData))
        {
            await _jsonGameStringFileWriterService.Write(gameStringData);
        }
    }
}
