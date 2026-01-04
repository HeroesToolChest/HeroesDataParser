namespace HeroesDataParser.Core;

public interface IImageParser<T>
    where T : IElementObject
{
    ExtractImageOptions ExtractImageOption { get; }

    HashSet<ImageWriterFile> GetImages(SortedDictionary<string, T> elementsById);
}
