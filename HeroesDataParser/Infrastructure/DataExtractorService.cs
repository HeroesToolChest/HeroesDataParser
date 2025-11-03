using Serilog.Context;

namespace HeroesDataParser.Infrastructure;

public class DataExtractorService : IDataExtractorService
{
    private readonly ILogger<DataExtractorService> _logger;
    private readonly RootOptions _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IParsingConfigurationService _parsingConfigurationService;
    private readonly IResultSummaryService _resultSummaryService;
    private readonly Stopwatch _stopwatch = new();

    public DataExtractorService(
        ILogger<DataExtractorService> logger,
        IOptions<RootOptions> options,
        IHeroesXmlLoaderService heroesXmlLoaderService,
        IParsingConfigurationService parsingConfigurationService,
        IResultSummaryService resultSummaryService)
    {
        _logger = logger;
        _options = options.Value;
        _heroesXmlLoaderService = heroesXmlLoaderService;
        _parsingConfigurationService = parsingConfigurationService;
        _resultSummaryService = resultSummaryService;
    }

    public Dictionary<string, TElement> Extract<TElement, TParser>(TParser parser, Map? map = null)
        where TElement : IElementObject
        where TParser : IDataParser<TElement>
    {
        _stopwatch.Restart();

        _logger.LogInformation("Starting data extractor for data object type {DataObjectType}", parser.DataObjectType);
        AnsiConsole.WriteLine($"Parsing '{typeof(TElement).Name}' data...");

        Dictionary<string, TElement> parsedItems = [];

        IEnumerable<string> itemIds = _heroesXmlLoaderService.HeroesXmlLoader.HeroesData.GetStormElementIds(parser.DataObjectType, StormCacheType.All);

        itemIds = _parsingConfigurationService.FilterAllowedItems(parser.DataObjectType, itemIds)
            .OrderBy(x => x);

        _logger.LogDebug("Element ids: {@ItemIds}", itemIds);

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
                    {
                        parsedItems.Add(id, element);
                    }
                    else
                    {
                        totalCount--;
                        _logger.LogTrace("Return is null, Id {id} not adding", id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing id {Id} for data object type {DataObjectType}", id, parser.DataObjectType);
                }
            }
        }

        _stopwatch.Stop();
        _logger.LogInformation("Data extractor complete for data object type {DataObjectType}", parser.DataObjectType);

        if (totalCount < 1)
        {
            AnsiConsole.MarkupLine("[yellow]No items found to parse[/]");
            AnsiConsole.WriteLine();

            return parsedItems;
        }

        _resultSummaryService.AddSummaryDataItem(parser.DataObjectType, parsedItems.Count, totalCount, _options.CurrentLocale, map?.Name?.PlainText);

        string message = $"{parsedItems.Count,6} / {totalCount} successfully parsed in {_stopwatch.Elapsed.TotalSeconds:0.###} seconds";
        if (parsedItems.Count == totalCount)
            AnsiConsole.MarkupLine(message);
        else
            AnsiConsole.MarkupLineInterpolated($"[yellow]{message}[/]");

        return parsedItems;
    }
}
