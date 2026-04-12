namespace HeroesDataParser.Core;

public interface IImageParserProcessor
{
    void SaveImages<TElementObject>(SortedDictionary<string, TElementObject> itemsToSerialize)
        where TElementObject : IElementObject;
}
