namespace HeroesDataParser.Infrastructure.Processors;

public class ImageParserProcessor : IImageParserProcessor
{
    private readonly ILogger<ImageParserProcessor> _logger;
    private readonly RootOptions _options;
    private readonly IKeyedServiceProvider _keyedServiceProvider;
    private readonly IImageWriterService _imageWriterService;

    public ImageParserProcessor(
        ILogger<ImageParserProcessor> logger,
        IOptions<RootOptions> options,
        IKeyedServiceProvider keyedServiceProvider,
        IImageWriterService imageWriterService)
    {
        _logger = logger;
        _options = options.Value;
        _keyedServiceProvider = keyedServiceProvider;
        _imageWriterService = imageWriterService;
    }

    // finds all available images in parsed data and saves the images for after data extraction/serialization
    public void SaveImages<TElementObject>(SortedDictionary<string, TElementObject> itemsToSerialize)
        where TElementObject : IElementObject
    {
        IEnumerable<IImageParser<TElementObject>> imageParsers = _keyedServiceProvider.GetKeyedServices<IImageParser<TElementObject>>(typeof(TElementObject));

        if (!imageParsers.Any())
        {
            _logger.LogInformation("No image writers available for {ElementType}", typeof(TElementObject).Name);

            return;
        }

        foreach (IImageParser<TElementObject> imageParser in imageParsers)
        {
            if (_options.ExtractImageOptions.HasFlag(imageParser.ExtractImageOption))
            {
                _imageWriterService.Save(imageParser.GetImages(itemsToSerialize));
            }
        }
    }
}
