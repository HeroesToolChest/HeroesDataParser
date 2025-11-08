namespace HeroesDataParser.Core;

public interface IMapProcessorService
{
    void Start();

    Task WriteMapDataFile();
}
