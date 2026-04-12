namespace HeroesDataParser.Core;

public interface IJsonDataFileWriterService
{
    Task Write<TElement>(SortedDictionary<string, TElement> elementsById)
        where TElement : IElementObject;

    Task WriteToMapSpecific<TElement>(string mapDirectory, SortedDictionary<string, TElement> elementsById)
        where TElement : IElementObject;
}
