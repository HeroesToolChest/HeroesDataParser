namespace HeroesDataParser.Tests.Infrastructure.ImageWriters;

public class ImageWriterBase
{
    public string TestImagesDirectory { get; set; } = "TestImages";

    public string OutputBaseDirectory { get; set; } = "UnitTestImageWriter";

    public string OutputImageDirectory { get; set; } = "images";
}
