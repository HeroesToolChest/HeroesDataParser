namespace HeroesDataParser.Core;

public interface IDataExtractorService
{
    SortedDictionary<string, TElement> Extract<TElement, TParser>(TParser parser, Map? map = null)
        where TElement : IElementObject
        where TParser : IDataParser<TElement>;
}
