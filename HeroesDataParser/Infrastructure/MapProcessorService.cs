using Serilog.Context;

namespace HeroesDataParser.Infrastructure;

public class MapProcessorService : IMapProcessorService
{
    private readonly ILogger<MapProcessorService> _logger;
    private readonly IProcessorService _processorService;
    private readonly IDataParser<Map> _mapDataParser;
    private readonly IMapDataExtractorService _mapDataExtractorService;
    private readonly IJsonFileWriterService _jsonFileWriterService;
    private readonly IImageWriter<Map> _imageWriter;

    public MapProcessorService(
        ILogger<MapProcessorService> logger,
        IProcessorService processorService,
        IDataParser<Map> mapDataParser,
        IMapDataExtractorService mapDataExtractorService,
        IJsonFileWriterService jsonFileWriterService,
        IImageWriter<Map> imageWriter)
    {
        _logger = logger;
        _processorService = processorService;
        _mapDataParser = mapDataParser;
        _mapDataExtractorService = mapDataExtractorService;
        _jsonFileWriterService = jsonFileWriterService;
        _imageWriter = imageWriter;
    }

    public async Task Start()
    {
        ExtractDataOptions extractDataOptions = _processorService.ExtractDataOptions;

        if (extractDataOptions.HasFlag(ExtractDataOptions.Map))
        {
            await ProcessMapObject();
        }
    }

    private async Task ProcessMapObject()
    {
        string typeOfMapName = typeof(Map).Name;
        string typeOfParserName = typeof(MapParser).Name;

        using (LogContext.PushProperty("ElementType", typeOfMapName))
        using (LogContext.PushProperty("Parser", typeOfParserName))
        {
            _logger.LogInformation("Start action processor for {MapObject} using parser {Parser}", typeOfMapName, typeOfParserName);

            // parses through all the maps for the data type
            Dictionary<string, Map> mapItemsToSerialize = await _mapDataExtractorService.Extract(_mapDataParser, _processorService.StartForMap);

            // then write out the map data
            // done last, because of the loading of map xml mods, we only want to go through the map mods once
            await _jsonFileWriterService.Write(mapItemsToSerialize);

            if (_processorService.ExtractImageOptions.HasFlag(_imageWriter.ExtractImageOption))
            {
                await _imageWriter.WriteImages(mapItemsToSerialize);
            }

            _logger.LogInformation("Action processor complete for {MapObject} using parser {Parser}", typeOfMapName, typeOfParserName);
        }
    }
}
