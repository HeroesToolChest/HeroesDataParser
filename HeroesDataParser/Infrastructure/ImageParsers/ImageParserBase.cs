namespace HeroesDataParser.Infrastructure.ImageParsers;

public abstract class ImageParserBase<TElement> : IImageWriter<TElement>
    where TElement : IElementObject
{
    protected const string ImageDirectory = "images";

    private readonly ILogger _logger;
    private readonly RootOptions _options;
    private readonly HeroesXmlLoader _heroesXmlLoader;

    private readonly Dictionary<string, ImageRelativePath> _relativePathsByFileNamePath = new(StringComparer.OrdinalIgnoreCase);

    public ImageParserBase(ILogger logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _options = options.Value;
        _logger = logger;
        _heroesXmlLoader = heroesXmlLoaderService.HeroesXmlLoader;
    }

    public abstract ExtractImageOptions ExtractImageOption { get; }

    /// <summary>
    /// Gets the relative mpq paths by the file name (with extension) path that starts with the image directory (still needs the output directory).
    /// </summary>
    internal Dictionary<string, ImageRelativePath> RelativePathsByFileNamePath => _relativePathsByFileNamePath;

    protected abstract string SubDirectory { get; }

    public void SaveImages(Dictionary<string, TElement> elementsById)
    {
        _logger.LogTrace("Saving images");

        if (elementsById.Count < 1)
        {
            _logger.LogInformation("No elements to find images");
            return;
        }

        // get all the images
        foreach (TElement element in elementsById.Values)
        {
            SetImages(element);
        }

        _logger.LogTrace("Saving images complete");
    }

    protected abstract void SetImages(TElement element);

    private protected void AddImagePath(string fileName, ImageRelativePath imageRelativePath)
    {
        string subPathFileName = Path.Combine(ImageDirectory, SubDirectory, fileName);

        _relativePathsByFileNamePath.TryAdd(subPathFileName, imageRelativePath);
    }

    //protected abstract Task SaveImages();

    ///// <summary>
    ///// Saves the images files to the output directory.
    ///// </summary>
    ///// <param name="relativePathsByFileName">A dictionary of relative file paths to the original image file by the key which is the final file name.</param>
    ///// <param name="directory">The output (sub)directory.</param>
    ///// <returns>A <see cref="Task"/>.</returns>
    //private protected async Task SaveImagesFiles(IDictionary<string, ImageRelativePath> relativePathsByFileName, string directory)
    //{
    //    string typeElementName = typeof(TElement).Name;

    //    if (relativePathsByFileName.Count < 1)
    //    {
    //        _logger.LogInformation("No {Type} file paths found", typeof(TElement));
    //        return;
    //    }

    //    _logger.LogInformation("{Count} {Type} images to save", relativePathsByFileName.Count, typeElementName);
    //    _logger.LogTrace("{Type} file paths: {@FilePaths}", typeElementName, relativePathsByFileName);

    //    string outputDirectory = Path.Combine(_options.OutputDirectory, ImageDirectory, directory);

    //    Directory.CreateDirectory(outputDirectory);

    //    _logger.LogInformation("Saving {Type} images to {OutputDirectory}", typeElementName, outputDirectory);

    //    List<Task> tasks = new(relativePathsByFileName.Count);

    //    foreach (KeyValuePair<string, ImageRelativePath> path in relativePathsByFileName)
    //    {
    //        tasks.Add(SaveStaticImageFile(path.Key, path.Value, outputDirectory));
    //    }

    //    await Task.WhenAll(tasks);
    //}

    //private Task SaveStaticImageFile(string fileName, ImageRelativePath imageRelativeFilePath, string outputDirectory)
    //{
    //    if (!_heroesXmlLoader.FileExists(imageRelativeFilePath.FilePath, imageRelativeFilePath.MpqFilePath))
    //    {
    //        _logger.LogWarning("Unable to save {FileName} because {@RelativeFilePath} does not exist ", fileName, imageRelativeFilePath);
    //        return Task.CompletedTask;
    //    }

    //    using Stream stream = _heroesXmlLoader.GetFile(imageRelativeFilePath.FilePath, imageRelativeFilePath.MpqFilePath);

    //    string outputFilePath = Path.Combine(outputDirectory, fileName);

    //    _logger.LogTrace("Saving image file {@RelativeFilePath} to {OutputFilePath}", imageRelativeFilePath, outputFilePath);

    //    if (imageRelativeFilePath.FilePath.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
    //    {
    //        using DDSImage ddsImage = new(stream);

    //        return ddsImage.Save(outputFilePath);
    //    }
    //    else
    //    {
    //        using Image image = Image.Load(stream);

    //        return image.SaveAsync(outputFilePath);
    //    }
    //}
}
