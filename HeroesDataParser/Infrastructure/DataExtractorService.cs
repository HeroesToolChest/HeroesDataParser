using Serilog.Context;
using System.Diagnostics;

namespace HeroesDataParser.Infrastructure;

public class DataExtractorService : IDataExtractorService
{
    private readonly ILogger<DataExtractorService> _logger;
    private readonly RootOptions _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IParsingConfigurationService _parsingConfigurationService;
    private readonly Stopwatch _stopwatch = new();

    public DataExtractorService(ILogger<DataExtractorService> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IParsingConfigurationService parsingConfigurationService)
    {
        _logger = logger;
        _options = options.Value;
        _heroesXmlLoaderService = heroesXmlLoaderService;
        _parsingConfigurationService = parsingConfigurationService;
    }

    public Dictionary<string, TElement> Extract<TElement, TParser>(TParser parser, Map? map = null)
        where TElement : IElementObject
        where TParser : IDataParser<TElement>
    {
        _stopwatch.Restart();
        _logger.LogInformation("Starting data extractor for data object type {DataObjectType}", parser.DataObjectType);
        AnsiConsole.MarkupLineInterpolated($"Parsing '{typeof(TElement).Name}' data...");

        Dictionary<string, TElement> parsedItems = [];

        IEnumerable<string> itemIds = _heroesXmlLoaderService.HeroesXmlLoader.HeroesData
            .GetStormElementIds(parser.DataObjectType, map is null ? StormCacheType.All : StormCacheType.Map);

        itemIds = _parsingConfigurationService.FilterAllowedItems(parser.DataObjectType, itemIds)
            .OrderBy(x => x);

        _logger.LogTrace("Element ids: {@ItemIds}", itemIds);

        int totalCount = 0;

        foreach (string id in itemIds)
        {
            totalCount++;

            using (LogContext.PushProperty("Id", id))
            using (LogContext.PushProperty("Locale", _options.CurrentLocale))
            {
                try
                {
                    TElement? element = parser.Parse(id);
                    if (element is not null)
                        parsedItems.Add(id, element);
                    else
                        _logger.LogWarning("Unable to parse id {id}", id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing id {Id} for data object type {DataObjectType}", id, parser.DataObjectType);
                }
            }
        }

        _stopwatch.Stop();
        _logger.LogInformation("Data extractor complete for data object type {DataObjectType}", parser.DataObjectType);

        string message = $"{parsedItems.Count,6} / {totalCount} successfully parsed in {_stopwatch.Elapsed.TotalSeconds:0.###} seconds";
        if (parsedItems.Count == totalCount)
            AnsiConsole.MarkupLine(message);
        else
            AnsiConsole.MarkupLineInterpolated($"[yellow]{message}[/]");

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
