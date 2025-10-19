namespace HeroesDataParser.Infrastructure.Processors;

// used to serialize the gamestringtext to a separate file
public class GameStringFileProcessorService : IGameStringFileProcessorService
{
    private readonly ILogger<GameStringFileProcessorService> _logger;
    private readonly RootOptions _options;
    private readonly ISavedGameStringsService _savedGameStringsService;
    private readonly IJsonGameStringFileWriterService _jsonGameStringFileWriterService;
    private readonly ISerializedElementsService _serializedElementsService;

    public GameStringFileProcessorService(
        ILogger<GameStringFileProcessorService> logger,
        IOptions<RootOptions> options,
        ISavedGameStringsService savedGameStringsService,
        IJsonGameStringFileWriterService jsonGameStringFileWriterService,
        ISerializedElementsService serializedElementsService)
    {
        _logger = logger;
        _options = options.Value;
        _savedGameStringsService = savedGameStringsService;
        _jsonGameStringFileWriterService = jsonGameStringFileWriterService;
        _serializedElementsService = serializedElementsService;
    }

    public async Task Start()
    {
        if (_options.LocalizedText == LocalizedTextOption.None)
        {
            _logger.LogInformation("LocalizedText option is set to {LocalizedTextOption}. Skipping.", _options.LocalizedText);
            return;
        }

        _logger.LogInformation("LocalizedText option is set to {LocalizedTextOption}. Starting GameString file processor...", _options.LocalizedText);

        GameStringElementName gameStringElements = _savedGameStringsService.GameStringElements;

        await _jsonGameStringFileWriterService.Write(gameStringElements, _serializedElementsService.GetDataTypes());
    }
}
