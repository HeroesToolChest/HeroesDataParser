namespace HeroesDataParser.Core;

public interface IProcessorService
{
    Task Start();

    Task StartForMap(Map map);
}
