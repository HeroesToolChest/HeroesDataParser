namespace HeroesDataParser.Core.Models;

public class ImageFileNamePath
{
    internal ImageFileNamePath(string image, ImagePath imagePath)
    {
        Image = image;
        FilePath = imagePath;
    }

    public string Image { get; }

    internal ImagePath FilePath { get; }

    public override string ToString()
    {
        return Image;
    }
}
