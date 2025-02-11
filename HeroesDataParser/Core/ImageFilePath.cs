namespace HeroesDataParser.Core;

public class ImageFilePath
{
    internal ImageFilePath(string image, RelativeFilePath filePath)
    {
        Image = image;
        FilePath = filePath;
    }

    public string Image { get; }

    internal RelativeFilePath FilePath { get; }

    public override string ToString()
    {
        return Image;
    }
}
