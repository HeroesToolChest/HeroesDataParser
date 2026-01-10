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

    protected abstract string SubDirectory { get; }

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
            SubDirectoryPath = SubDirectory,
            FileName = imageName,
            ProcessImageFile = imageProcesser,
        };

        _imageWriterFiles.Add(imageWriterFile);
    }

    private protected void VerifyFileExists(RelativeFilePath relativeFilePath)
    {
        if (!HeroesXmlLoaderService.HeroesXmlLoader.FileExists(relativeFilePath.FilePath, relativeFilePath.MpqFilePath))
        {
            Logger.LogWarning("Unable to to create file because {@ImageWriterPath} does not exist", relativeFilePath);
            throw new FileNotFoundException("Image file not found.", relativeFilePath.FilePath);
        }
    }

    /// <summary>
    /// Processes a static image.
    /// </summary>
    /// <param name="imageName">The file name (no path) of the created image.</param>
    /// <param name="relativeFilePath">The path that points to the location in CASC or on disk.</param>
    /// <param name="directoryPath">The output directory of the create image (no file name).</param>
    /// <returns>A <see cref="Task"/>.</returns>
    private protected Task ProcessStaticImage(string imageName, RelativeFilePath relativeFilePath, string directoryPath)
    {
        return ProcessImageInternal(imageName, relativeFilePath, directoryPath, (filePath, stream) =>
        {
            if (relativeFilePath.FilePath.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
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
    /// <param name="relativeFilePath">The path that points to the location in CASC or on disk.</param>
    /// <param name="directoryPath">The output directory of the create image (no file name).</param>
    /// <param name="processImage">The function that processes the image with the inputs <paramref name="ddsImage"/> and <paramref name="outputFilePath"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    private protected Task ProcessImage(string imageName, RelativeFilePath relativeFilePath, string directoryPath, Func<DDSImage, string, Task> processImage)
    {
        return ProcessImageInternal(imageName, relativeFilePath, directoryPath, (outputFilePath, stream) =>
        {
            using DDSImage ddsImage = new(stream);

            return processImage.Invoke(ddsImage, outputFilePath);
        });
    }

    private Task ProcessImageInternal(string imageName, RelativeFilePath relativeFilePath, string directoryPath, Func<string, Stream, Task> processImage)
    {
        VerifyFileExists(relativeFilePath);

        string filePath = Path.Combine(directoryPath, imageName);

        using Stream stream = _heroesXmlLoaderService.HeroesXmlLoader.GetFile(relativeFilePath.FilePath, relativeFilePath.MpqFilePath);

        _logger.LogTrace("Writing image file {@RelativeFilePath} to {OutputFilePath}", relativeFilePath, filePath);

        return processImage.Invoke(filePath, stream);
    }
}
