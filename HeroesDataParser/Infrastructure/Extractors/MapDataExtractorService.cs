using Serilog.Context;

namespace HeroesDataParser.Infrastructure.Extractors;

public class MapDataExtractorService : IMapDataExtractorService
{
    private readonly ILogger<MapDataExtractorService> _logger;
    private readonly RootOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IDataParser<Map> _mapDataParser;
    private readonly IResultSummaryService _resultSummaryService;
    private readonly Stopwatch _stopwatch = new();

    public MapDataExtractorService(
        ILogger<MapDataExtractorService> logger,
        IOptions<RootOptions> options,
        IAnsiConsole console,
        IHeroesXmlLoaderService heroesXmlLoaderService,
        IDataParser<Map> mapDataParser,
        IResultSummaryService resultSummaryService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _heroesXmlLoaderService = heroesXmlLoaderService;
        _mapDataParser = mapDataParser;
        _resultSummaryService = resultSummaryService;
    }

    public async Task<SortedDictionary<string, Map>> Extract(Func<Map, Task> dataParsers)
    {
        _stopwatch.Restart();

        int currentXmlCount = _heroesXmlLoaderService.HeroesXmlLoader.GetCountOfXmlDataFiles();
        int currentFontStyleCount = _heroesXmlLoaderService.HeroesXmlLoader.GetCountOfFontStyleFiles();
        int currentGameStringCount = _heroesXmlLoaderService.HeroesXmlLoader.GetCountOfGameStringsFiles();

        _logger.LogDebug("Starting data extractor for data object type {DataObjectType}", _mapDataParser.DataObjectType);

        SortedDictionary<string, Map> parsedMaps = new(StringComparer.Ordinal);

        IEnumerable<string> mapTitles = _heroesXmlLoaderService.HeroesXmlLoader.GetMapTitles()
            .OrderBy(x => x);

        _logger.LogDebug("Map ids: {@MapIds}", mapTitles);

        int totalCount = 0;

        foreach (string mapTitle in mapTitles)
        {
            totalCount++;

            using (LogContext.PushProperty("MapId", mapTitle))
            {
                _console.MarkupLineInterpolated($"[DarkSeaGreen2]{(_options.StorageLoad.Type == StorageType.Online ? "Downloading" : "Loading")} '{mapTitle}' mod[/]...");

                _heroesXmlLoaderService.HeroesXmlLoader.LoadMapMod(mapTitle);

                _console.MarkupLineInterpolated($"{_heroesXmlLoaderService.HeroesXmlLoader.GetCountOfXmlDataFiles() - currentXmlCount,6} xml files loaded");
                _console.MarkupLineInterpolated($"{_heroesXmlLoaderService.HeroesXmlLoader.GetCountOfFontStyleFiles() - currentFontStyleCount,6} storm style files loaded");
                _console.MarkupLineInterpolated($"{_heroesXmlLoaderService.HeroesXmlLoader.GetCountOfGameStringsFiles() - currentGameStringCount,6} gamestring files loaded");

                Map? map = null;

                try
                {
                    map = _mapDataParser.Parse(mapTitle);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing map title {MapTitle}", mapTitle);
                    continue;
                }

                if (map is not null)
                {
                    parsedMaps.Add(mapTitle, map);

                    if (_options.DisableMapSpecificJson)
                    {
                        _logger.LogWarning("Map specific JSON generation is disabled, skipping data processors for {MapId}", mapTitle);
                        continue;
                    }

                    _logger.LogDebug("Running data processors for {MapId}", mapTitle);

                    await dataParsers.Invoke(map);

                    _logger.LogDebug("Completed data processors for {MapId}", mapTitle);
                }
                else
                {
                    totalCount--;
                    _logger.LogTrace("Return is null, map title {MapTitle} not adding", mapTitle);
                }
            }
        }

        _heroesXmlLoaderService.HeroesXmlLoader.UnloadMapMod();

        _stopwatch.Stop();
        _logger.LogDebug("Map data extractor complete for data object type {DataObjectType}", _mapDataParser.DataObjectType);

        if (totalCount < 1)
        {
            _console.MarkupLine("[yellow]No map items found to parse[/]");
            _console.WriteLine();

            return parsedMaps;
        }

        _console.Markup("Finished parsing map data...");

        _resultSummaryService.AddSummaryDataItem("MapData", parsedMaps.Count, totalCount, stormLocale: _options.CurrentLocale);

        string message = $"{parsedMaps.Count} / {totalCount} ({_stopwatch.Elapsed.TotalSeconds:0}s {_stopwatch.Elapsed.Milliseconds:0}ms)";
        if (parsedMaps.Count == totalCount)
            _console.MarkupLine(message);
        else
            _console.MarkupLineInterpolated($"[yellow]:warning:  {message}[/]");

        return parsedMaps;
    }
}
