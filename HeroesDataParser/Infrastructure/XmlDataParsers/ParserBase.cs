using Heroes.Element.Models;
using Serilog.Context;

namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public abstract class ParserBase<T> : IDataParser<T>
    where T : ElementObject, IElementObject
{
    public const string ImageFileExtension = "png";

    private readonly ILogger _logger;
    private readonly HeroesData _heroesData;

    public ParserBase(ILogger logger, IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
    }

    public abstract string DataObjectType { get; }

    public virtual T? Parse(string id)
    {
        _logger.LogTrace("Parsing id {Id}", id);

        StormElement? stormElement = _heroesData.GetCompleteStormElement(DataObjectType, id);

        if (stormElement is null)
        {
            _logger.LogWarning("Could not find data for id {Id}", id);
            return null;
        }

        T? elementObject = (T?)Activator.CreateInstance(typeof(T), id);

        if (elementObject is null)
        {
            _logger.LogError("Failed to create instance of type {Type} for id {Id}", typeof(T), id);
            return null;
        }

        using (LogContext.PushProperty("XmlPaths", stormElement.OriginalXElements.Select(x => x.StormPath)))
        {
            SetProperties(elementObject, stormElement);

            _logger.LogTrace("Parsing id {Id} complete", id);

            return elementObject;
        }
    }

    protected abstract void SetProperties(T elementObject, StormElement stormElement);

    protected TooltipDescription? GetTooltipDescription(string id)
    {
        StormGameString? stormGameString = _heroesData.GetStormGameString(id);

        if (stormGameString is null)
            return null;
        else
            return _heroesData.ParseGameString(stormGameString);
    }

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
            string image = Path.ChangeExtension(Path.GetFileName(stormAssetFile.StormPath.Path), ImageFileExtension);
            RelativeFilePath imagePath = new()
            {
                FilePath = stormAssetFile.StormPath.Path,
            };

            return new ImageFilePath(image, imagePath);
        }
        else
        {
            _logger.LogWarning("Could not get storm asset file from {TileTexturePath}", tileTexturePath);
            return null;
        }
    }

    protected double GetScaleValue(string elementType, string id, string? elementName)
    {
        string? dataObjectType = _heroesData.GetDataObjectTypeByElementType(elementType);
        if (string.IsNullOrWhiteSpace(dataObjectType))
        {
            _logger.LogWarning("Could not get data object type for element type {ElementType}", elementType);
            return 0;
        }

        if (string.IsNullOrWhiteSpace(elementName))
        {
            _logger.LogWarning("Element name {ElementName} is emtpy or null", elementName);
            return 0;
        }

        return _heroesData.GetScalingValue(dataObjectType, id, elementName) ?? 0;
    }

    protected void SetNameProperty(T elementObject, StormElement stormElement)
    {
        if (elementObject is IName nameObject)
        {
            if (stormElement.DataValues.TryGetElementDataAt("name", out StormElementData? nameData))
                nameObject.Name = GetTooltipDescription(nameData.Value.GetString());
        }
    }

    protected void SetHyperlinkIdProperty(T elementObject, StormElement stormElement)
    {
        if (elementObject is IHyperlinkId hyperlinkIdObject)
        {
            if (stormElement.DataValues.TryGetElementDataAt("hyperlinkid", out StormElementData? hyperlinkIdData))
                hyperlinkIdObject.HyperlinkId = hyperlinkIdData.Value.GetString();
        }
    }

    protected void SetRarityProperty(T elementObject, StormElement stormElement)
    {
        if (elementObject is IRarity rarityObject)
        {
            if (stormElement.DataValues.TryGetElementDataAt("rarity", out StormElementData? rarityData) && Enum.TryParse(rarityData.Value.GetString(), out Rarity rarity))
                rarityObject.Rarity = rarity;
        }
    }

    protected void SetEventNameProperty(T elementObject, StormElement stormElement)
    {
        if (elementObject is IEventName eventNameObject)
        {
            if (stormElement.DataValues.TryGetElementDataAt("eventname", out StormElementData? eventNameData))
                eventNameObject.Event = eventNameData.Value.GetString();
        }
    }

    protected void SetDescriptionProperty(T elementObject, StormElement stormElement)
    {
        if (elementObject is IDescription descriptionObject)
        {
            if (stormElement.DataValues.TryGetElementDataAt("description", out StormElementData? descriptionData))
                descriptionObject.Description = GetTooltipDescription(descriptionData.Value.GetString());
        }
    }
}
