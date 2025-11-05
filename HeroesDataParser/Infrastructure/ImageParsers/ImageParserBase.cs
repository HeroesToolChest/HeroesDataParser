namespace HeroesDataParser.Infrastructure.ImageParsers;

public abstract class ImageParserBase<TElement> : IImageParser<TElement>
    where TElement : IElementObject
{
    private readonly ILogger _logger;

    public ImageParserBase(ILogger logger)
    {
        _logger = logger;
    }

    public abstract ExtractImageOptions ExtractImageOption { get; }

    protected abstract string SubDirectory { get; }

    public HashSet<ImageWriterPath> GetImages(SortedDictionary<string, TElement> elementsById)
    {
        _logger.LogTrace("Saving images");

        if (elementsById.Count < 1)
        {
            _logger.LogInformation("No elements to find images");
            return [];
        }

        HashSet<ImageWriterPath> imagePaths = [];

        // get all the images
        foreach (TElement element in elementsById.Values)
        {
            SetImages(element, imagePaths);
        }

        _logger.LogTrace("Saving images complete");

        return imagePaths;
    }

    protected abstract void SetImages(TElement element, HashSet<ImageWriterPath> imagePaths);

    private protected void TryAddToFiles(HashSet<ImageWriterPath> imagePaths, string? fileName, RelativeFilePath? relativePath, TElement element)
    {
        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(relativePath?.FilePath))
            return;

        ImageWriterPath imageParserPath = new()
        {
            ElementId = element.Id,
            SubDirectoryPath = SubDirectory,
            FileName = fileName,
            RelativeFilePath = relativePath.FilePath,
            RelativeMpqFilePath = relativePath.MpqFilePath,
        };

        imagePaths.Add(imageParserPath);
    }
}
