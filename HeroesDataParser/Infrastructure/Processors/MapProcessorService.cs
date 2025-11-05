using Serilog.Context;

namespace HeroesDataParser.Infrastructure.Processors;

public class MapProcessorService : IMapProcessorService
{
    private readonly ILogger<MapProcessorService> _logger;
    private readonly IProcessorService _processorService;
    private readonly IMapDataExtractorService _mapDataExtractorService;
    private readonly IJsonDataFileWriterService _jsonDataFileWriterService;
    private readonly IEnumerable<IImageParser<Map>> _mapImageParsers;
    private readonly IImageWriterService _imageWriterService;

    public MapProcessorService(
        ILogger<MapProcessorService> logger,
        IProcessorService processorService,
        IMapDataExtractorService mapDataExtractorService,
        IJsonDataFileWriterService jsonDataFileWriterService,
        IEnumerable<IImageParser<Map>> mapImageParsers,
        IImageWriterService imageWriterService)
    {
        _logger = logger;
        _processorService = processorService;
        _mapDataExtractorService = mapDataExtractorService;
        _jsonDataFileWriterService = jsonDataFileWriterService;
        _mapImageParsers = mapImageParsers;
        _imageWriterService = imageWriterService;
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

            // parses through all the maps for the data type(s)
            SortedDictionary<string, Map> mapItemsToSerialize = await _mapDataExtractorService.Extract(_processorService.StartForMap);

            // then write out the map data
            // done last, because of the loading of map xml mods, we only want to go through the map mods once
            await _jsonDataFileWriterService.Write(mapItemsToSerialize);

            // extract and save images
            foreach (IImageParser<Map> mapImageParser in _mapImageParsers)
            {
                if (_processorService.ExtractImageOptions.HasFlag(mapImageParser.ExtractImageOption))
                {
                    _imageWriterService.Save(mapImageParser.GetImages(mapItemsToSerialize));
                }
            }

            _logger.LogInformation("Action processor complete for {MapObject} using parser {Parser}", typeOfMapName, typeOfParserName);
        }
    }
}
