namespace HeroesDataParser.Infrastructure.ImageParsers;

public class EmoticonImageParser : ImageParserBase<Emoticon>
{
    public EmoticonImageParser(ILogger<EmoticonImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
    : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Emoticon;

    protected override string SubDirectory => "emoticons";

    protected override void SetImages(Emoticon element)
    {
        if (element.Animation is null)
        {
            SetStaticImage(element);

            return;
        }

        SetAnimatedImage(element);
    }

    private void SetStaticImage(Emoticon element)
    {
        if (string.IsNullOrWhiteSpace(element.Image) || element is not IImagePath elementImagePath || string.IsNullOrWhiteSpace(elementImagePath.ImagePath?.FilePath))
            return;

        if (!element.Width.HasValue || !element.TextureSheet.Columns.HasValue || !element.TextureSheet.Rows.HasValue)
            return;

        AddToFiles(element.Image, element.Id, async (outputDirectoryPath) =>
        {
            RelativeFilePath relativeFilePath = elementImagePath.ImagePath;

            await ProcessImage(element.Image, relativeFilePath, outputDirectoryPath, (ddsImage, outputFilePath) =>
            {
                int imageWidth = ddsImage.Width / element.TextureSheet.Columns.Value;
                int imageHeight = ddsImage.Height / element.TextureSheet.Rows.Value;

#pragma warning disable SA1407 // Arithmetic expressions should declare precedence
                int xPos = element.Index % element.TextureSheet.Columns.Value * imageWidth;
#pragma warning restore SA1407 // Arithmetic expressions should declare precedence
                int yPos = element.Index / element.TextureSheet.Columns.Value * imageHeight;

                return ddsImage.Save(outputFilePath, new SixLabors.ImageSharp.Point(xPos, yPos), new SixLabors.ImageSharp.Size(element.Width.Value, imageHeight));
            });
        });
    }

    private void SetAnimatedImage(Emoticon element)
    {
        if (element.Animation is null)
            return;

        SetOriginalTextureImage(element);

        if (!element.TextureSheet.Columns.HasValue || !element.TextureSheet.Rows.HasValue)
            return;

        if (!string.IsNullOrWhiteSpace(element.Image) && element is IImagePath elementImagePath && !string.IsNullOrWhiteSpace(elementImagePath.ImagePath?.FilePath))
        {
            AddToFiles(element.Image, element.Id, async (outputDirectoryPath) =>
            {
                RelativeFilePath relativeFilePath = elementImagePath.ImagePath;

                await ProcessImage(element.Image, relativeFilePath, outputDirectoryPath, (ddsImage, outputFilePath) =>
                {
                    int imageWidth = ddsImage.Width / element.TextureSheet.Columns.Value;
                    int imageHeight = ddsImage.Height / element.TextureSheet.Rows.Value;

                    AnimatedImage animatedImage = new()
                    {
                        OutputFileName = outputFilePath,
                        Size = new SixLabors.ImageSharp.Size(element.Animation.Width, imageHeight),
                        MaxSize = new SixLabors.ImageSharp.Size(imageWidth, imageHeight),
                        Frames = element.Animation.Frames,
                        FrameDelay = element.Animation.Duration,
                    };

                    return SaveAnimatedImage(ddsImage, animatedImage, outputFilePath);
                });
            });
        }
    }

    // for the animated image, we create a static image from the original texture
    private void SetOriginalTextureImage(Emoticon element)
    {
        if (element.Animation is null || element.Animation.Texture is null)
            return;

        if (!string.IsNullOrWhiteSpace(element.Image) && element is IImagePath elementImagePath && !string.IsNullOrWhiteSpace(elementImagePath.ImagePath?.FilePath))
        {
            AddToFiles(element.Animation.Texture, element.Id, async (outputDirectoryPath) =>
            {
                RelativeFilePath relativeFilePath = elementImagePath.ImagePath;

                await ProcessStaticImage(element.Animation.Texture, relativeFilePath, outputDirectoryPath);
            });
        }
    }
}
