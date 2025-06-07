namespace HeroesDataParser.Core;

public interface IJsonFileWriterService
{
    Task Write<TElement>(Dictionary<string, TElement> elementsById)
        where TElement : IElementObject;

    Task WriteToMaps<TElement>(string mapDirectory, Dictionary<string, TElement> elementsById)
        where TElement : IElementObject;
}
