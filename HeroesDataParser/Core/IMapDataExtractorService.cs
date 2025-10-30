namespace HeroesDataParser.Core;

public interface IMapDataExtractorService
{
    Task<Dictionary<string, Map>> Extract(Func<Map, Task> elementParsersForMap);
}
