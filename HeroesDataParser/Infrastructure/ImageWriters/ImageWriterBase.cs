using SixLabors.ImageSharp;

namespace HeroesDataParser.Infrastructure.ImageWriters;

public abstract class ImageWriterBase<TElement> : IImageWriter<TElement>
    where TElement : IElementObject
{
    protected const string ImageDirectory = "images";

    private readonly ILogger _logger;
    private readonly RootOptions _options;
    private readonly HeroesXmlLoader _heroesXmlLoader;

    public ImageWriterBase(ILogger logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _options = options.Value;
        _logger = logger;
        _heroesXmlLoader = heroesXmlLoaderService.HeroesXmlLoader;
    }

    public abstract ExtractImageOptions ExtractImageOption { get; }

    public async Task WriteImages(Dictionary<string, TElement> elementsById)
    {
        _logger.LogTrace("Writing images");

        if (elementsById.Count < 1)
        {
            _logger.LogInformation("No elements to find images");
            return;
        }

        foreach (TElement element in elementsById.Values)
        {
            SetImages(element);
        }

        await SaveImages();

        _logger.LogTrace("Writing images complete");
    }

    protected abstract void SetImages(TElement element);

    protected abstract Task SaveImages();

    /// <summary>
    /// Saves the images files to the output directory.
    /// </summary>
    /// <param name="relativePathsByFileName">A dictionary of relative file paths to the original image file by the key which is the final file name.</param>
    /// <param name="directory">The output (sub)directory.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    private protected async Task SaveImagesFiles(IDictionary<string, ImageRelativePath> relativePathsByFileName, string directory)
    {
        string typeElementName = typeof(TElement).Name;

        if (relativePathsByFileName.Count < 1)
        {
            _logger.LogInformation("No {Type} file paths found", typeof(TElement));
            return;
        }

        _logger.LogInformation("{Count} {Type} images to save", relativePathsByFileName.Count, typeElementName);
        _logger.LogTrace("{Type} file paths: {@FilePaths}", typeElementName, relativePathsByFileName);

        string outputDirectory = Path.Combine(_options.OutputDirectory, ImageDirectory, directory);

        Directory.CreateDirectory(outputDirectory);

        _logger.LogInformation("Saving {Type} images to {OutputDirectory}", typeElementName, outputDirectory);

        List<Task> tasks = new(relativePathsByFileName.Count);

        foreach (KeyValuePair<string, ImageRelativePath> path in relativePathsByFileName)
        {
            tasks.Add(SaveStaticImageFile(path.Key, path.Value, outputDirectory));
        }

        await Task.WhenAll(tasks);
    }

    private Task SaveStaticImageFile(string fileName, ImageRelativePath imageRelativeFilePath, string outputDirectory)
    {
        if (!_heroesXmlLoader.FileExists(imageRelativeFilePath.FilePath, imageRelativeFilePath.MpqFilePath))
        {
            _logger.LogWarning("Unable to save {FileName} because {@RelativeFilePath} does not exist ", fileName, imageRelativeFilePath);
            return Task.CompletedTask;
        }

        using Stream stream = _heroesXmlLoader.GetFile(imageRelativeFilePath.FilePath, imageRelativeFilePath.MpqFilePath);

        string outputFilePath = Path.Combine(outputDirectory, fileName);

        _logger.LogTrace("Saving image file {@RelativeFilePath} to {OutputFilePath}", imageRelativeFilePath, outputFilePath);

        if (imageRelativeFilePath.FilePath.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
        {
            using DDSImage ddsImage = new(stream);

            return ddsImage.Save(outputFilePath);
        }
        else
        {
            using Image image = Image.Load(stream);

            return image.SaveAsync(outputFilePath);
        }
    }
}
