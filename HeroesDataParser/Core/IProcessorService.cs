namespace HeroesDataParser.Core;

public interface IProcessorService
{
    ExtractDataOptions ExtractDataOptions { get; }

    ExtractImageOptions ExtractImageOptions { get; }

    void Start();

    void StartForMap(Map map);

    Task WriteDataFiles();
}
