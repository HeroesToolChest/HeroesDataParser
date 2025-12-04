using Serilog.Context;

namespace HeroesDataParser.Infrastructure.Processors;

public class MapProcessorService : IMapProcessorService
{
    private readonly ILogger<MapProcessorService> _logger;
    private readonly IProcessorService _processorService;
    private readonly IMapDataExtractorService _mapDataExtractorService;
    private readonly IJsonDataFileWriterService _jsonDataFileWriterService;
    private readonly IJsonGameStringFileWriterService _jsonGameStringFileWriterService;
    private readonly IEnumerable<IImageParser<Map>> _mapImageParsers;
    private readonly IImageWriterService _imageWriterService;
    private readonly IBaseGameStringMergeService _baseGameStringMergeService;

    private Func<Task>? _mapDataWriterTask;

    public MapProcessorService(
        ILogger<MapProcessorService> logger,
        IProcessorService processorService,
        IMapDataExtractorService mapDataExtractorService,
        IJsonDataFileWriterService jsonDataFileWriterService,
        IJsonGameStringFileWriterService jsonGameStringFileWriterService,
        IEnumerable<IImageParser<Map>> mapImageParsers,
        IImageWriterService imageWriterService,
        IBaseGameStringMergeService baseGameStringMergeService)
    {
        _logger = logger;
        _processorService = processorService;
        _mapDataExtractorService = mapDataExtractorService;
        _jsonDataFileWriterService = jsonDataFileWriterService;
        _jsonGameStringFileWriterService = jsonGameStringFileWriterService;
        _mapImageParsers = mapImageParsers;
        _imageWriterService = imageWriterService;
        _baseGameStringMergeService = baseGameStringMergeService;
    }

    public async Task Start()
    {
        await ProcessMapObject();
        await WriteMapDataFile();
        await WriteGameStrings();
    }

    private async Task WriteMapDataFile()
    {
        if (_mapDataWriterTask is not null)
            await _mapDataWriterTask();
    }

    private async Task WriteGameStrings()
    {
        byte[]? bytes = _baseGameStringMergeService.MergeWithMap();

        if (bytes is null)
        {
            _logger.LogInformation("No merged game strings to write.");
            return;
        }

        await _jsonGameStringFileWriterService.Write(bytes);
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

            // delay the write out the map data
            // done last, because of the loading of map xml mods, we only want to go through the map mods once
            _mapDataWriterTask = () => _jsonDataFileWriterService.Write(mapItemsToSerialize);

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
