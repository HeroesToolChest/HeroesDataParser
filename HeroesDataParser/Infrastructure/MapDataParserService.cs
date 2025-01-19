namespace HeroesDataParser.Infrastructure;

// for the map data, since we use different ids and have to parse all the elements
public class MapDataParserService : IMapDataParserService
{
    //private readonly ILogger<MapDataParserService> _logger;
    //private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IMapDataExtractorService _mapDataExtractorService;
    private readonly IProcessorService _processorService;
    private readonly IJsonFileWriterService _jsonFileWriterService;

    public MapDataParserService(ILogger<MapDataParserService> logger, IHeroesXmlLoaderService heroesXmlLoaderService, IMapDataExtractorService mapDataExtractorService, IProcessorService processorService, IJsonFileWriterService jsonFileWriterService)
    {
        //_logger = logger;
        //_heroesXmlLoaderService = heroesXmlLoaderService;
        _mapDataExtractorService = mapDataExtractorService;
        _processorService = processorService;
        _jsonFileWriterService = jsonFileWriterService;
    }

    public async Task ParseAndWriteData(IDataParser<Map> parser, StormLocale stormLocale)
    {
        var mapItemsToSerialize = await _mapDataExtractorService.Extract(parser, _processorService.StartForMap);

        //_logger.LogInformation("{Count} items to serialize", mapItemsToSerialize.Keys.Count);

        await _jsonFileWriterService.Write(mapItemsToSerialize, stormLocale);





        //_logger.LogInformation("Starting data extractor for data object type {DataObjectType}", parser.DataObjectType);

        ////Dictionary<string, TElement> parsedItems = [];

        //IEnumerable<string> mapTitles = _heroesXmlLoaderService.HeroesXmlLoader.GetMapTitles().OrderBy(x => x);

        //_logger.LogTrace("Map ids: {@MapIds}", mapTitles);

        //foreach (string mapTitle in mapTitles)
        //{
        //    using (LogContext.PushProperty("MapId", mapTitle))
        //    {
        //        _heroesXmlLoaderService.HeroesXmlLoader.LoadMapMod(mapTitle);
        //        Map? map = parser.Parse(mapTitle);

        //        if (map is not null)
        //        {
        //            _processorService.RunElementProcessors(true);
        //        }
        //        else
        //        {
        //            _logger.LogWarning("Unable to parse map id {id}", mapTitle);
        //        }
        //    }
        //}

        //_heroesXmlLoaderService.HeroesXmlLoader.UnloadMapMod();
        //_logger.LogInformation("Data extractor complete for data object type {DataObjectType}", parser.DataObjectType);
    }
}
