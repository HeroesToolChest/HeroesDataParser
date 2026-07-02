namespace HeroesDataParser.Infrastructure.ImageParsers;

public class SprayImageParser : ImageParserBase<Spray>
{
    public SprayImageParser(ILogger<SprayImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Spray;

    protected override string Subdirectory => "sprays";

    protected override void SetImages(Spray element)
    {
        if (element.Animation is null)
        {
            AddStaticImage(element);

            return;
        }

        SetAnimatedImage(element);
    }

    private void SetAnimatedImage(Spray element)
    {
        if (element.Animation is null)
            return;

        SetOriginalTextureImage(element);

        if (string.IsNullOrWhiteSpace(element.Image) || element is not IImagePath elementImagePath || string.IsNullOrWhiteSpace(elementImagePath.ImagePath?.FilePath))
            return;

        AddToFiles(element.Image, element.Id, async (outputDirectoryPath) =>
        {
            ImagePath imagePath = elementImagePath.ImagePath;

            await ProcessImage(element.Image, imagePath, outputDirectoryPath, (ddsImage, outputFilePath) =>
            {
                int ddsImageWidth = ddsImage.Width;

                if (element.Animation.Frames > 0)
                    ddsImageWidth /= element.Animation.Frames;

                AnimatedImage animatedImage = new()
                {
                    OutputFileName = outputFilePath,
                    Size = new SixLabors.ImageSharp.Size(ddsImageWidth, ddsImage.Height),
                    MaxSize = new SixLabors.ImageSharp.Size(ddsImageWidth, ddsImage.Height),
                    Frames = element.Animation.Frames,
                    FrameDelay = element.Animation.Duration / 2,
                };

                return SaveAnimatedImage(ddsImage, animatedImage, outputFilePath);
            });
        });
    }

    // for the animated image, we create a static image from the original texture
    private void SetOriginalTextureImage(Spray element)
    {
        if (string.IsNullOrWhiteSpace(element.Image) || element is not IImagePath elementImagePath || string.IsNullOrWhiteSpace(elementImagePath.ImagePath?.FilePath))
            return;

        AddToFiles(element.Animation!.Texture, element.Id, async (outputDirectoryPath) =>
        {
            ImagePath imagePath = elementImagePath.ImagePath;

            await ProcessStaticImage(element.Animation.Texture!, imagePath, outputDirectoryPath);
        });
    }
}
