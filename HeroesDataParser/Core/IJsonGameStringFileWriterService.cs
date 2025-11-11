namespace HeroesDataParser.Core;

public interface IJsonGameStringFileWriterService
{
    Task Write(GameStringItemDictionary gameStringItemDictionary, IEnumerable<string> dataTypes);
}
