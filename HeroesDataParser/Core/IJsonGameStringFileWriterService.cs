namespace HeroesDataParser.Core;

public interface IJsonGameStringFileWriterService
{
    Task WriteMapSpecific(string mapName);

    Task WriteBase();

    Task WriteMap();
}
