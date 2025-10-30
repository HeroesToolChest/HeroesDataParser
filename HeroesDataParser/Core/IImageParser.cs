namespace HeroesDataParser.Core;

public interface IImageParser<T>
    where T : IElementObject
{
    ExtractImageOptions ExtractImageOption { get; }

    HashSet<ImageWriterPath> GetImages(Dictionary<string, T> elementsById);
}
