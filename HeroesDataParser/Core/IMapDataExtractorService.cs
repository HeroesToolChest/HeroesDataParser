namespace HeroesDataParser.Core;

public interface IMapDataExtractorService
{
    Task<SortedDictionary<string, Map>> Extract(Func<Map, Task> dataParsers);
}
