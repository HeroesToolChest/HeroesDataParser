namespace HeroesDataParser.Core;

public interface IJsonGameStringFileWriterService
{
    Task WriteForMap(string mapName);

    Task Write(byte[] bytes);

    void SerializeOnly();
}
