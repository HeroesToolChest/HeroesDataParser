using Serilog.Context;

namespace HeroesDataParser.Infrastructure.Extractors;

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

    public SortedDictionary<string, TElement> Extract<TElement, TParser>(TParser parser, Map? map = null)
        where TElement : IElementObject
        where TParser : IDataParser<TElement>
    {
        ConcurrentDictionary<string, TElement> parsedItems = new(StringComparer.Ordinal);

        int totalCount = ExtractInternal<TElement, TParser>(parser, typeof(TElement).Name, (id, element) =>
        {
            parsedItems.TryAdd(id, element);
        });

        DisplayExtractionSummary(totalCount, parsedItems.Count, parser.DataObjectType, map);

        return new SortedDictionary<string, TElement>(parsedItems);
    }

    private int ExtractInternal<TElement, TParser>(TParser parser, string nameOfElement, Action<string, TElement> itemAdder)
        where TElement : IElementObject
        where TParser : IDataParser<TElement>
    {
        _stopwatch.Restart();

        _logger.LogInformation("Starting data extractor for data object type {DataObjectType}", parser.DataObjectType);
        AnsiConsole.Write($"Parsing '{nameOfElement}' data...");

        IEnumerable<string> itemIds = _heroesXmlLoaderService.HeroesXmlLoader.HeroesData.GetStormElementIds(parser.DataObjectType, StormCacheType.All);

        itemIds = _parsingConfigurationService.FilterAllowedItems(nameOfElement, itemIds)
            .OrderBy(x => x);

        _logger.LogDebug("Element ids: {@ItemIds}", itemIds);

        int totalCount = 0;

        Parallel.ForEach(itemIds, new ParallelOptions() { MaxDegreeOfParallelism = _options.Threads }, id =>
        {
            Interlocked.Increment(ref totalCount);

            using (LogContext.PushProperty("Id", id))
            using (LogContext.PushProperty("Locale", _options.CurrentLocale))
            {
                try
                {
                    TElement? element = parser.Parse(id);
                    if (element is not null)
                    {
                        itemAdder.Invoke(element.Id, element);
                    }
                    else
                    {
                        Interlocked.Decrement(ref totalCount);
                        _logger.LogTrace("Return is null, Id {id} not adding", id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing id {Id} for data object type {DataObjectType}", id, parser.DataObjectType);
                }
            }
        });

        _stopwatch.Stop();
        _logger.LogInformation("Data extractor complete for data object type {DataObjectType}", parser.DataObjectType);

        return totalCount;
    }

    private void DisplayExtractionSummary(int totalCount, int parsedItemsCount, string dataObjectType, Map? map)
    {
        if (totalCount < 1)
        {
            AnsiConsole.MarkupLine("[yellow]No items found to parse[/]");
            AnsiConsole.WriteLine();
        }
        else
        {
            _resultSummaryService.AddSummaryDataItem(dataObjectType, parsedItemsCount, totalCount, _options.CurrentLocale, map?.Name?.PlainText);

            if (parsedItemsCount == totalCount)
                AnsiConsole.MarkupInterpolated($"{parsedItemsCount} / {totalCount}");
            else
                AnsiConsole.MarkupInterpolated($"[yellow]{parsedItemsCount} / {totalCount}[/]");

            AnsiConsole.WriteLine($" (in {_stopwatch.Elapsed.TotalSeconds:0}s {_stopwatch.Elapsed.Milliseconds:0}ms)");
        }
    }
}
