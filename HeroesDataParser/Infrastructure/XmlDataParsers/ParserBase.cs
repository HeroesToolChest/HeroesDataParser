namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public abstract class ParserBase
{
    public const string ImageFileExtension = "png";

    private readonly ILogger _logger;
    private readonly RootOptions _options;
    private readonly HeroesXmlLoader _heroesXmlLoader;
    private readonly HeroesData _heroesData;
    private readonly ITooltipDescriptionService _tooltipDescriptionService;

    protected ParserBase(ILogger logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, ITooltipDescriptionService tooltipDescriptionService)
    {
        _logger = logger;
        _options = options.Value;
        _heroesXmlLoader = heroesXmlLoaderService.HeroesXmlLoader;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
        _tooltipDescriptionService = tooltipDescriptionService;
    }

    protected ILogger Logger => _logger;

    protected RootOptions Options => _options;

    protected HeroesXmlLoader HeroesXmlLoader => _heroesXmlLoader;

    protected HeroesData HeroesData => _heroesData;

    protected ITooltipDescriptionService TooltipDescriptionService => _tooltipDescriptionService;

    protected ImageFilePath? GetImageFilePath(StormElementData data)
    {
        string tileTexturePath = data.Value.GetString();

        StormFile? stormAssetFile = _heroesData.GetStormAssetFile(tileTexturePath);
        if (stormAssetFile is not null)
        {
            Span<char> pathSpan = stackalloc char[stormAssetFile.StormPath.Path.Length];

            int size = Path.GetFileName(stormAssetFile.StormPath.Path.AsSpan()).ToLowerInvariant(pathSpan);

            string image = Path.ChangeExtension(pathSpan[..size].ToString(), ImageFileExtension);
            RelativeFilePath imagePath = new()
            {
                FilePath = stormAssetFile.StormPath.Path,
            };

            return new ImageFilePath(image, imagePath);
        }
        else
        {
            Logger.LogWarning("Could not find storm asset {TileTexturePath}", tileTexturePath);
            return null;
        }
    }

    protected double GetScaleValue(string elementType, string id, string? elementName)
    {
        string? dataObjectType = _heroesData.GetDataObjectTypeByElementType(elementType);
        if (string.IsNullOrWhiteSpace(dataObjectType))
        {
            Logger.LogWarning("Could not get data object type for element type {ElementType}", elementType);
            return 0;
        }

        if (string.IsNullOrWhiteSpace(elementName))
        {
            Logger.LogWarning("Element name {ElementName} is emtpy or null", elementName);
            return 0;
        }

        return _heroesData.GetScalingValue(dataObjectType, id, elementName) ?? 0;
    }
}
