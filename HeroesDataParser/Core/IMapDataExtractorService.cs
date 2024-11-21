namespace HeroesDataParser.Core;

public interface IMapDataExtractorService
{
    Task<Dictionary<string, Map>> Extract(IDataParser<Map> parser, Func<Map, Task> elementParsersForMap);
}
