using Serilog.Context;

namespace HeroesDataParser.Infrastructure;

public class DataExtractorService : IDataExtractorService
{
    private readonly ILogger<DataExtractorService> _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public DataExtractorService(ILogger<DataExtractorService> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesXmlLoaderService = heroesXmlLoaderService;
    }

    public Dictionary<string, TElement> Extract<TElement, TParser>(TParser parser, Map? map = null)
        where TElement : IElementObject
        where TParser : IDataParser<TElement>
    {
        _logger.LogInformation("Starting data extractor for data object type {DataObjectType}", parser.DataObjectType);

        Dictionary<string, TElement> parsedItems = [];

        IEnumerable<string> itemIds = _heroesXmlLoaderService.HeroesXmlLoader.HeroesData
            .GetStormElementIds(parser.DataObjectType, map is null ? StormCacheType.All : StormCacheType.Map)
                .OrderBy(x => x);

        _logger.LogTrace("Element ids: {@ItemIds}", itemIds);

        foreach (string id in itemIds)
        {
            using (LogContext.PushProperty("Id", id))
            {
                TElement? element = parser.Parse(id);
                if (element is not null)
                    parsedItems.Add(id, element);
                else
                    _logger.LogWarning("Unable to parse id {id}", id);
            }
        }

        _logger.LogInformation("Data extractor complete for data object type {DataObjectType}", parser.DataObjectType);

        return parsedItems;
    }
}
// this is the basic extractor service that will be used for all data types, except for maps which has its own service
//public class DataExtractorService<TElement, TParser> : IDataExtractorService<TElement, TParser>
//    where TElement : IElementObject
//    where TParser : IDataParser<TElement>
//{
//    private readonly ILogger<DataExtractorService<TElement, TParser>> _logger;
//    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

//    public DataExtractorService(ILogger<DataExtractorService<TElement, TParser>> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
//    {
//        _logger = logger;
//        _heroesXmlLoaderService = heroesXmlLoaderService;
//    }

//    public virtual Dictionary<string, TElement> Extract(TParser parser, bool isForMap = false)
//    {
//        _logger.LogInformation("Starting data extractor for data object type {DataObjectType}", parser.DataObjectType);

//        Dictionary<string, TElement> parsedItems = [];

//        IEnumerable<string> itemIds = GetIdsForParsing(parser, isForMap);

//        _logger.LogTrace("Element ids: {@ItemIds}", itemIds);

//        foreach (string id in itemIds)
//        {
//            using (LogContext.PushProperty("Id", id))
//            {
//                TElement? element = parser.Parse(id);
//                if (element is not null)
//                    parsedItems.Add(id, element);
//                else
//                    _logger.LogWarning("Unable to parse id {id}", id);
//            }
//        }

//        _logger.LogInformation("Data extractor complete for data object type {DataObjectType}", parser.DataObjectType);

//        return parsedItems;
//    }

//    protected virtual IEnumerable<string> GetIdsForParsing(TParser parser, bool isForMap)
//    {
//        if (isForMap)
//            return _heroesXmlLoaderService.HeroesXmlLoader.HeroesData.GetStormElementIds(parser.DataObjectType, StormCacheType.Map).OrderBy(x => x);
//        else
//            return _heroesXmlLoaderService.HeroesXmlLoader.HeroesData.GetStormElementIds(parser.DataObjectType).OrderBy(x => x);
//    }
//}
