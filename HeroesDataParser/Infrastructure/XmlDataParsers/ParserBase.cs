namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public abstract class ParserBase
{
    public const string StaticImageFileExtension = "png";
    public const string GifImageFileExtension = "gif";
    public const string APngImageFileExtension = "apng";

    private readonly ILogger _logger;
    private readonly RootOptions _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    protected ParserBase(ILogger logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
    {
        _logger = logger;
        _options = options.Value;
        _heroesXmlLoaderService = heroesXmlLoaderService;
        _gameStringTextService = gameStringTextService;
    }

    protected ILogger Logger => _logger;

    protected RootOptions Options => _options;

    protected HeroesXmlLoader HeroesXmlLoader => _heroesXmlLoaderService.HeroesXmlLoader;

    protected HeroesData HeroesData => _heroesXmlLoaderService.HeroesXmlLoader.HeroesData;

    protected IGameStringTextService GameStringTextService => _gameStringTextService;

    /// <summary>
    /// Gets the image output file name for the given file path.
    /// </summary>
    /// <param name="filePath">Path should start with Assets\Textures\.</param>
    /// <returns>The filename in lowercase and as a .png.</returns>
    protected string GetStaticImageOutputFileName(string? filePath)
    {
        return GetImageOutputFileName(filePath, StaticImageFileExtension);
    }

    /// <summary>
    /// Gets the static image file path for the given assets texture path.
    /// </summary>
    /// <param name="assetsTexturePath">Path should start with Assets\Textures\.</param>
    /// <returns>An <see cref="ImageFilePath"/> if the texture exists; otherwise, null.</returns>
    protected ImageFilePath? GetStaticImageFilePath(string? assetsTexturePath)
    {
        return GetImageFilePath(assetsTexturePath, StaticImageFileExtension);
    }

    /// <summary>
    /// Gets the animated image file path for the given assets texture path.
    /// </summary>
    /// <param name="assetsTexturePath">Path should start with Assets\Textures\.</param>
    /// <returns>An <see cref="ImageFilePath"/> if the texture exists; otherwise, null.</returns>
    protected ImageFilePath? GetAnimatedImageFilePath(string? assetsTexturePath)
    {
        return Options.Hidden.AnimatedImageType switch
        {
            AnimatedImageType.APNG => GetImageFilePath(assetsTexturePath, APngImageFileExtension),
            AnimatedImageType.GIF => GetImageFilePath(assetsTexturePath, GifImageFileExtension),
            _ => GetImageFilePath(assetsTexturePath, APngImageFileExtension),
        };
    }

    protected ImageFilePath? GetImageFilePath(StormElementData data)
    {
        return GetStaticImageFilePath(data.Value.GetString());
    }

    protected double GetScaleValue(string elementType, string id, string? elementName)
    {
        string? dataObjectType = HeroesData.GetDataObjectTypeByElementType(elementType);
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

        return HeroesData.GetScalingValue(dataObjectType, id, elementName) ?? 0;
    }

    private ImageFilePath? GetImageFilePath(string? assetsTexturePath, string fileExtension)
    {
        StormFile? stormAssetFile = HeroesData.GetStormAssetFile(assetsTexturePath);
        if (stormAssetFile is null)
            return null;

        string image = GetImageOutputFileName(stormAssetFile.StormPath.Path, fileExtension);

        RelativeFilePath imagePath = new()
        {
            FilePath = stormAssetFile.StormPath.Path,
        };

        return new ImageFilePath(image, imagePath);
    }

    private string GetImageOutputFileName(ReadOnlySpan<char> filePath, string fileExtension)
    {
        if (filePath.IsEmpty)
            return string.Empty;

        Span<char> pathSpan = stackalloc char[filePath.Length];
        filePath.CopyTo(pathSpan);

        PathHelper.NormalizePath(pathSpan);

        return Path.ChangeExtension(Path.GetFileName(pathSpan).ToString(), fileExtension);
    }
}
