namespace HeroesDataParser.Core;

public interface IJsonDataFileWriterService
{
    Task Write<TElement>(SortedDictionary<string, TElement> elementsById)
        where TElement : IElementObject;

    Task WriteToMaps<TElement>(string mapDirectory, SortedDictionary<string, TElement> elementsById)
        where TElement : IElementObject;
}
