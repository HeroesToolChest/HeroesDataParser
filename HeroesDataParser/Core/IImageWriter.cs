using System.Xml.Linq;

namespace HeroesDataParser.Core;

public interface IImageWriter<T>
    where T : IElementObject
{
    ExtractImageOptions ExtractImageOption { get; }

    Task WriteImages(Dictionary<string, T> elementsById);
}
