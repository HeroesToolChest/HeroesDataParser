namespace HeroesDataParser.Infrastructure;

public class MapProcessorService : IMapProcessorService
{
    private readonly ILogger<MapProcessorService> _logger;
    private readonly IProcessorService _processorService;
    private readonly IDataParser<Map> _mapDataParser;
    private readonly IMapDataParserService _mapDataParserService;

    private StormLocale _stormLocale;

    public MapProcessorService(ILogger<MapProcessorService> logger, IProcessorService processorService, IDataParser<Map> mapDataParser, IMapDataParserService mapDataParserService)
    {
        _logger = logger;
        _processorService = processorService;
        _mapDataParser = mapDataParser;
        _mapDataParserService = mapDataParserService;
    }

    public async Task Start(StormLocale stormLocale)
    {
        _stormLocale = stormLocale;

        ExtractDataOptions extractDataOptions = _processorService.ExtractDataOptions;

        if (extractDataOptions.HasFlag(ExtractDataOptions.Map))
        {
            await ProcessMapObject();
        }
    }

    private async Task ProcessMapObject()
    {
        _logger.LogInformation("Start action processor for {MapObject} using parser {Parser}", typeof(Map).Name, typeof(MapParser).Name);

        await _mapDataParserService.ParseAndWriteData(_mapDataParser, _stormLocale);

        _logger.LogInformation("Action processor complete for {MapObject} using parser {Parser}", typeof(Map).Name, typeof(MapParser).Name);
    }
}
