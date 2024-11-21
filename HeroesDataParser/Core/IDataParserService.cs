namespace HeroesDataParser.Core;

public interface IDataParserService
{
    Task ParseAndWriteData<TElement, TParser>(TParser parser, StormLocale stormLocale, Map? map = null)
        where TElement : IElementObject
        where TParser : IDataParser<TElement>;
}
