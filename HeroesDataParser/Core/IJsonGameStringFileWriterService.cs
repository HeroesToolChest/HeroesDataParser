namespace HeroesDataParser.Core;

public interface IJsonGameStringFileWriterService
{
    Task Write(GameStringElementName gameStringElements, IEnumerable<string> dataTypes);
}
