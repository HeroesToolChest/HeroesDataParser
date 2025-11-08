namespace HeroesDataParser.Core;

public interface IMapDataExtractorService
{
    SortedDictionary<string, Map> Extract(Action<Map> elementParsersForMap);
}
