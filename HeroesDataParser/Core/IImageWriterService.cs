namespace HeroesDataParser.Core;

public interface IImageWriterService
{
    void Save(HashSet<ImageWriterPath> imagePaths);

    Task Write();
}
