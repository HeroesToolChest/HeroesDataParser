namespace HeroesDataParser.Core;

public interface IProcessorService
{
    ExtractDataOptions ExtractDataOptions { get; }

    Task Start(StormLocale stormLocale);

    Task StartForMap(Map map);
}
