using SixLabors.ImageSharp;

namespace HeroesDataParser.Infrastructure.ImageParsers;

public abstract class ImageParserBase<TElement> : IImageParser<TElement>
    where TElement : IElementObject
{
    private readonly ILogger _logger;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HashSet<ImageWriterFile> _imageWriterFiles = [];

    public ImageParserBase(ILogger logger, IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesXmlLoaderService = heroesXmlLoaderService;
    }

    public abstract ExtractImageOptions ExtractImageOption { get; }

    protected abstract string Subdirectory { get; }

    protected ILogger Logger => _logger;

    protected IHeroesXmlLoaderService HeroesXmlLoaderService => _heroesXmlLoaderService;

    public HashSet<ImageWriterFile> GetImages(SortedDictionary<string, TElement> elementsById)
    {
        Logger.LogTrace("Saving images");

        if (elementsById.Count < 1)
        {
            Logger.LogInformation("No elements to find images");
            return [];
        }

        // get all the images
        foreach (TElement element in elementsById.Values)
        {
            SetImages(element);
        }

        Logger.LogTrace("Saving images complete");

        return _imageWriterFiles;
    }

    protected abstract void SetImages(TElement element);

    protected void AddStaticImage(TElement element)
    {
        if (element is not IImage imageObject || element is not IImagePath imagePathObject)
            return;

        if (!string.IsNullOrWhiteSpace(imageObject.Image) && !string.IsNullOrWhiteSpace(imagePathObject.ImagePath?.FilePath))
            AddToFiles(imageObject.Image, element.Id, async (directoryPath) => await ProcessStaticImage(imageObject.Image, imagePathObject.ImagePath, directoryPath));
    }

    protected Task SaveAnimatedImage(DDSImage ddsImage, AnimatedImage animatedImage, string outputFilePath)
    {
        if (outputFilePath.EndsWith($".{ParserBase.APngImageFileExtension}", StringComparison.OrdinalIgnoreCase))
        {
            return ddsImage.SaveAsAPNG(outputFilePath, animatedImage.Size, animatedImage.MaxSize, animatedImage.Frames, animatedImage.FrameDelay);
        }
        else if (outputFilePath.EndsWith($".{ParserBase.GifImageFileExtension}", StringComparison.OrdinalIgnoreCase))
        {
            return ddsImage.SaveAsGif(outputFilePath, animatedImage.Size, animatedImage.MaxSize, animatedImage.Frames, animatedImage.FrameDelay);
        }
        else
        {
            Logger.LogError("Animated image format not supported for file {OutputFilePath}", outputFilePath);
            throw new NotSupportedException($"Animated image format not supported for file {outputFilePath}");
        }
    }

    /// <summary>
    /// Adds the iamge to be process later with the <paramref name="imageProcesser"/>.
    /// </summary>
    /// <param name="imageName">The file name (no path) of the created image.</param>
    /// <param name="elementId">The id of the element that the image belongs to.</param>
    /// <param name="imageProcesser">The function that processes the image with the input of the output directory (no file name).</param>
    private protected void AddToFiles(string? imageName, string elementId, Func<string, Task> imageProcesser)
    {
        if (string.IsNullOrWhiteSpace(imageName))
            return;

        ImageWriterFile imageWriterFile = new()
        {
            ElementId = elementId,
            SubDirectoryPath = Subdirectory,
            FileName = imageName,
            ProcessImageFile = imageProcesser,
        };

        _imageWriterFiles.Add(imageWriterFile);
    }

    private protected void VerifyFileExists(ImagePath imagePath)
    {
        if (!HeroesXmlLoaderService.HeroesXmlLoader.FileExists(imagePath.FilePath, imagePath.MpqEntryPath))
        {
            Logger.LogWarning("Unable to to create file because {@ImagePath} does not exist", imagePath);
            throw new FileNotFoundException("Image file not found.", imagePath.FilePath);
        }
    }

    /// <summary>
    /// Processes a static image.
    /// </summary>
    /// <param name="imageName">The file name (no path) of the created image.</param>
    /// <param name="imagePath">The path that points to the location in CASC or on disk.</param>
    /// <param name="directoryPath">The output directory of the create image (no file name).</param>
    /// <returns>A <see cref="Task"/>.</returns>
    private protected Task ProcessStaticImage(string imageName, ImagePath imagePath, string directoryPath)
    {
        return ProcessImageInternal(imageName, imagePath, directoryPath, (filePath, stream) =>
        {
            string pathToImage = imagePath.IsMpqEntry ? imagePath.MpqEntryPath : imagePath.FilePath;

            if (pathToImage.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
            {
                using DDSImage ddsImage = new(stream);

                return ddsImage.Save(filePath);
            }
            else
            {
                using Image image = Image.Load(stream);

                return image.SaveAsync(filePath);
            }
        });
    }

    /// <summary>
    /// Process a general image.
    /// </summary>
    /// <param name="imageName">The file name (no path) of the created image.</param>
    /// <param name="imagePath">The path that points to the location in CASC or on disk.</param>
    /// <param name="directoryPath">The output directory of the create image (no file name).</param>
    /// <param name="processImage">The function that processes the image with the inputs <paramref name="ddsImage"/> and <paramref name="outputFilePath"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    private protected Task ProcessImage(string imageName, ImagePath imagePath, string directoryPath, Func<DDSImage, string, Task> processImage)
    {
        return ProcessImageInternal(imageName, imagePath, directoryPath, (outputFilePath, stream) =>
        {
            using DDSImage ddsImage = new(stream);

            return processImage.Invoke(ddsImage, outputFilePath);
        });
    }

    private Task ProcessImageInternal(string imageName, ImagePath imagePath, string directoryPath, Func<string, Stream, Task> processImage)
    {
        VerifyFileExists(imagePath);

        string filePath = Path.Combine(directoryPath, imageName);

        using Stream stream = _heroesXmlLoaderService.HeroesXmlLoader.GetFile(imagePath.FilePath, imagePath.MpqEntryPath);

        _logger.LogTrace("Writing image file {@ImagePath} to {OutputFilePath}", imagePath, filePath);

        return processImage.Invoke(filePath, stream);
    }
}
