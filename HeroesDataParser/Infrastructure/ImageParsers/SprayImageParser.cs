namespace HeroesDataParser.Infrastructure.ImageParsers;

public class SprayImageParser : ImageParserBase<Spray>
{
    public SprayImageParser(ILogger<SprayImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Spray;

    protected override string SubDirectory => "sprays";

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

        SetStaticImage(element);

        if (!string.IsNullOrWhiteSpace(element.Image) && element is IImagePath elementImagePath && !string.IsNullOrWhiteSpace(elementImagePath.ImagePath?.FilePath))
        {
            AddToFiles(element.Image, element.Id, async (outputDirectoryPath) =>
            {
                RelativeFilePath relativeFilePath = elementImagePath.ImagePath;

                await ProcessImage(element.Image, relativeFilePath, outputDirectoryPath, (ddsImage, outputFilePath) =>
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

                    if (outputFilePath.EndsWith($".{ParserBase.GifImageFileExtension}", StringComparison.OrdinalIgnoreCase))
                    {
                        return ddsImage.SaveAsGif(outputFilePath, animatedImage.Size, animatedImage.MaxSize, animatedImage.Frames, animatedImage.FrameDelay);
                    }
                    else if (outputFilePath.EndsWith($".{ParserBase.APngImageFileExtension}", StringComparison.OrdinalIgnoreCase))
                    {
                        return ddsImage.SaveAsAPNG(outputFilePath, animatedImage.Size, animatedImage.MaxSize, animatedImage.Frames, animatedImage.FrameDelay);
                    }
                    else
                    {
                        Logger.LogError("Animated image format not supported for file {OutputFilePath}", outputFilePath);
                        throw new NotSupportedException($"Animated image format not supported for file {outputFilePath}");
                    }
                });
            });
        }
    }

    // for the animated image, we still need the static image but this one points to the animation texture
    private void SetStaticImage(Spray element)
    {
        if (!string.IsNullOrWhiteSpace(element.Image) && element is IImagePath elementImagePath && !string.IsNullOrWhiteSpace(elementImagePath.ImagePath?.FilePath))
        {
            AddToFiles(element.Animation!.Texture, element.Id, async (outputDirectoryPath) =>
            {
                RelativeFilePath relativeFilePath = elementImagePath.ImagePath;

                await ProcessStaticImage(element.Animation.Texture!, relativeFilePath, outputDirectoryPath);
            });
        }
    }
}
