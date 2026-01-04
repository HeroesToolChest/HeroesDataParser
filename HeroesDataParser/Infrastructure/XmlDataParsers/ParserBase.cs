namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public abstract class ParserBase
{
    public const string ImageFileExtension = "png";

    private readonly ILogger _logger;
    private readonly RootOptions _options;
    private readonly HeroesXmlLoader _heroesXmlLoader;
    private readonly HeroesData _heroesData;
    private readonly IGameStringTextService _gameStringTextService;

    protected ParserBase(ILogger logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
    {
        _logger = logger;
        _options = options.Value;
        _heroesXmlLoader = heroesXmlLoaderService.HeroesXmlLoader;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
        _gameStringTextService = gameStringTextService;
    }

    protected ILogger Logger => _logger;

    protected RootOptions Options => _options;

    protected HeroesXmlLoader HeroesXmlLoader => _heroesXmlLoader;

    protected HeroesData HeroesData => _heroesData;

    protected IGameStringTextService GameStringTextService => _gameStringTextService;

    // path should start with Assets\Textures\
    // output image file name for the json files
    protected string GetImageOutputFileName(string filePath)
    {
        Span<char> pathSpan = stackalloc char[filePath.Length];

        int size = Path.GetFileName(filePath.AsSpan()).ToLowerInvariant(pathSpan);

        return Path.ChangeExtension(pathSpan[..size].ToString(), ImageFileExtension);
    }

    // path should start with Assets\Textures\
    protected ImageFilePath? GetImageFilePath(string assetsTexturePath)
    {
        StormFile? stormAssetFile = _heroesData.GetStormAssetFile(assetsTexturePath);
        if (stormAssetFile is null)
            return null;

        string image = GetImageOutputFileName(stormAssetFile.StormPath.Path);

        RelativeFilePath imagePath = new()
        {
            FilePath = stormAssetFile.StormPath.Path,
        };

        return new ImageFilePath(image, imagePath);
    }

    protected ImageFilePath? GetImageFilePath(StormElementData data)
    {
        return GetImageFilePath(data.Value.GetString());
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
