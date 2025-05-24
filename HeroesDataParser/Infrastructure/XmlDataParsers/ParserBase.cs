using HeroesDataParser.Core;

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

    ///// <summary>
    ///// Gets a tooltip description from an id. Looks up the id in the gamestrings and parses it.
    ///// </summary>
    ///// <param name="id">The id.</param>
    ///// <returns>A <see cref="TooltipDescription"/>.</returns>
    //protected TooltipDescription? GetTooltipDescriptionFromId(string id)
    //{
    //    StormGameString? stormGameString = _heroesData.GetStormGameString(id);

    //    try
    //    {
    //        if (stormGameString is null)
    //            return null;
    //        else
    //            return _heroesData.ParseGameString(stormGameString);
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger.LogError(ex, "Could not parse gamestring {Id}", id);
    //        throw;
    //    }
    //}

    /// <summary>
    /// Gets a tooltip description from a <see cref="StormGameString"/>.
    /// </summary>
    /// <param name="gamestring">A <see cref="StormGameString"/>, which is an unparsed gamestring.</param>
    /// <returns>A <see cref="TooltipDescription"/>.</returns>
    protected TooltipDescription? GetTooltipDescriptionFromGameString(StormGameString gamestring)
    {
        return _heroesData.ParseGameString(gamestring);
    }

    /// <summary>
    /// Gets a tooltip description from an unparsed gamestring.
    /// </summary>
    /// <param name="gamestring">An unparsed gamestring.</param>
    /// <returns>A <see cref="TooltipDescription"/>.</returns>
    protected TooltipDescription? GetTooltipDescriptionFromGameString(string gamestring)
    {
        return _heroesData.ParseGameString(gamestring);
    }

    /// <summary>
    /// Gets an unparsed gamestring from an id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns>An unparsed gamestring or <see langword="null"/> if not found.</returns>
    protected string? GetStormGameString(string id)
    {
        StormGameString? stormGameString = _heroesData.GetStormGameString(id);

        if (stormGameString is null)
            return null;
        else
            return stormGameString.Value;
    }

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
            Logger.LogWarning("Could not get storm asset file from {TileTexturePath}", tileTexturePath);
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
