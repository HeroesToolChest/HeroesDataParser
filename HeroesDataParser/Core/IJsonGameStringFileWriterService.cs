namespace HeroesDataParser.Core;

public interface IJsonGameStringFileWriterService
{
    Task WriteMapSpecific(Map map);

    Task WriteBase();

    Task WriteMap();
}
