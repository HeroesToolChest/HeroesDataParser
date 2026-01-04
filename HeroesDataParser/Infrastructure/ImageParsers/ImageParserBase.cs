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

    protected void AddBasicImage(TElement element)
    {
        if (element is not IImage imageObject || element is not IImagePath imagePathObject)
            return;

        if (!string.IsNullOrWhiteSpace(imageObject.Image) && !string.IsNullOrWhiteSpace(imagePathObject.ImagePath?.FilePath))
            TryAddToFiles(imageObject.Image, element.Id, async (directoryPath) => await ProcessBasicImage(imageObject.Image, imagePathObject.ImagePath, directoryPath));
    }

    private protected void TryAddToFiles(string? imageName, string elementId, Func<string, Task> imageProcesser)
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
            throw new FileNotFoundException("Image file not found.");
        }
    }

    // find and save a basic image to the output directory
    private protected Task ProcessBasicImage(string imageName, RelativeFilePath relativeFilePath, string directoryPath)
    {
        VerifyFileExists(relativeFilePath);

        string filePath = Path.Combine(directoryPath, imageName);

        using Stream stream = _heroesXmlLoaderService.HeroesXmlLoader.GetFile(relativeFilePath.FilePath, relativeFilePath.MpqFilePath);

        _logger.LogTrace("Writing image file {@RelativeFilePath} to {OutputFilePath}", relativeFilePath, filePath);

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
    }
}
