namespace HeroesDataParser.Core;

public interface IImageWriterService
{
    void Save(HashSet<ImageWriterFile> imagePaths);

    Task Write();
}
