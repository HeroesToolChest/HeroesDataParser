namespace HeroesDataParser.Core;

public interface IProcessorService
{
    Task Start();

    Task StartForMapSpecific(Map map);
}
