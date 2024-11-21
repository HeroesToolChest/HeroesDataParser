namespace HeroesDataParser.Core;

public interface IMapDataParserService
{
    Task ParseAndWriteData(IDataParser<Map> parser, StormLocale stormLocale);
}
