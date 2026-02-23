using Serilog.Context;

namespace HeroesDataParser.Infrastructure.Processors;

public class MapProcessorService : IMapProcessorService
{
    private readonly ILogger<MapProcessorService> _logger;
    private readonly RootOptions _options;
    private readonly IProcessorService _processorService;
    private readonly IMapDataExtractorService _mapDataExtractorService;
    private readonly IJsonDataFileWriterService _jsonDataFileWriterService;
    private readonly IJsonGameStringFileWriterService _jsonGameStringFileWriterService;
    private readonly IEnumerable<IImageParser<Map>> _mapImageParsers;
    private readonly IImageWriterService _imageWriterService;

    public MapProcessorService(
        ILogger<MapProcessorService> logger,
        IOptions<RootOptions> options,
        IProcessorService processorService,
        IMapDataExtractorService mapDataExtractorService,
        IJsonDataFileWriterService jsonDataFileWriterService,
        IJsonGameStringFileWriterService jsonGameStringFileWriterService,
        IEnumerable<IImageParser<Map>> mapImageParsers,
        IImageWriterService imageWriterService)
    {
        _logger = logger;
        _options = options.Value;
        _processorService = processorService;
        _mapDataExtractorService = mapDataExtractorService;
        _jsonDataFileWriterService = jsonDataFileWriterService;
        _jsonGameStringFileWriterService = jsonGameStringFileWriterService;
        _mapImageParsers = mapImageParsers;
        _imageWriterService = imageWriterService;
    }

    public async Task Start()
    {
        await ProcessMapObject();
        await WriteBaseMapGameStrings();
    }

    private async Task WriteBaseMapGameStrings()
    {
        await _jsonGameStringFileWriterService.WriteMap();
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

            // write out the map data file
            await _jsonDataFileWriterService.Write(mapItemsToSerialize);

            // extract and save images
            foreach (IImageParser<Map> mapImageParser in _mapImageParsers)
            {
                if (_options.ExtractImageOptions.HasFlag(mapImageParser.ExtractImageOption))
                {
                    _imageWriterService.Save(mapImageParser.GetImages(mapItemsToSerialize));
                }
            }

            _logger.LogInformation("Action processor complete for {MapObject} using parser {Parser}", typeOfMapName, typeOfParserName);
        }
    }
}
