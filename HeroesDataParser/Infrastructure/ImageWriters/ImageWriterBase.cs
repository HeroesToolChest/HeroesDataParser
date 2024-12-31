using Heroes.XmlData;

namespace HeroesDataParser.Infrastructure.ImageWriters;

public abstract class ImageWriterBase<TElement> : IImageWriter<TElement>
    where TElement : IElementObject
{
    protected const string ImageDirectory = "images";

    private readonly ILogger _logger;
    private readonly RootOptions _options;
    private readonly HeroesXmlLoader _heroesXmlLoader;
    private readonly HeroesData _heroesData;

    public ImageWriterBase(ILogger logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _options = options.Value;
        _logger = logger;
        _heroesXmlLoader = heroesXmlLoaderService.HeroesXmlLoader;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
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
            GetImages(element);
        }

        await SaveImages();

        _logger.LogTrace("Writing images complete");
    }

    protected abstract void GetImages(TElement element);

    protected abstract Task SaveImages();

    protected async Task SaveStaticImageFile(string relativeFilePath, string outputDirectory)
    {
        if (!_heroesXmlLoader.FileExists(relativeFilePath))
        {
            _logger.LogWarning("File {RelativeFilePath} does not exist", relativeFilePath);
            return;
        }

        using Stream stream = _heroesXmlLoader.GetFile(relativeFilePath);
        using DDSImage ddsImage = new(stream);

        string outputFilePath = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(relativeFilePath)}.png");

        _logger.LogTrace("Saving image file {RelativeFilePath} to {OutputFilePath}", relativeFilePath, outputFilePath);

        await ddsImage.Save(outputFilePath);
    }

    private void SaveImageFile()
    {
        
    }
}
