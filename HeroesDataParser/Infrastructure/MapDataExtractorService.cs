using Serilog.Context;

namespace HeroesDataParser.Infrastructure;

public class MapDataExtractorService : IMapDataExtractorService
{
    private readonly ILogger<MapDataExtractorService> _logger;
    private readonly IHeroesDataLoaderService _heroesDataLoaderService;

    public MapDataExtractorService(ILogger<MapDataExtractorService> logger, IHeroesDataLoaderService heroesDataLoaderService)
    {
        _logger = logger;
        _heroesDataLoaderService = heroesDataLoaderService;
    }

    public async Task<Dictionary<string, Map>> Extract(IDataParser<Map> parser, Func<Map, Task> elementParsersForMap)
    {
        _logger.LogInformation("Starting data extractor for data object type {DataObjectType}", parser.DataObjectType);

        Dictionary<string, Map> parsedMaps = [];

        IEnumerable<string> mapTitles = _heroesDataLoaderService.HeroesXmlLoader.GetMapTitles().OrderBy(x => x);

        _logger.LogTrace("Map ids: {@MapIds}", mapTitles);

        foreach (string mapTitle in mapTitles)
        {
            using (LogContext.PushProperty("MapId", mapTitle))
            {
                _heroesDataLoaderService.HeroesXmlLoader.LoadMapMod(mapTitle);
                Map? map = parser.Parse(mapTitle);

                if (map is not null)
                {
                    parsedMaps.Add(mapTitle, map);

                    _logger.LogInformation("Running element processors for {MapId}", mapTitle);

                    await elementParsersForMap.Invoke(map);

                    _logger.LogInformation("Completed element processors for {MapId}", mapTitle);
                }
                else
                {
                    _logger.LogWarning("Unable to parse map id {id}", mapTitle);
                }
            }
        }

        _heroesDataLoaderService.HeroesXmlLoader.UnloadMapMod();

        _logger.LogInformation("Data extractor complete for data object type {DataObjectType}", parser.DataObjectType);

        return parsedMaps;
    }
}
