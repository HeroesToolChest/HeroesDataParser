namespace HeroesDataParser.Core;

public interface IProcessorService
{
    ExtractDataOptions ExtractDataOptions { get; }

    ExtractImageOptions ExtractImageOptions { get; }

    Task Start(StormLocale stormLocale);

    Task StartForMap(Map map);
}
