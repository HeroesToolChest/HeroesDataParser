namespace HeroesDataParser.Core;

public interface IProcessorService
{
    ExtractDataOptions ExtractDataOptions { get; }

    ExtractImageOptions ExtractImageOptions { get; }

    Task Start();

    Task StartForMap(Map map);
}
