using Serilog.Context;

namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public abstract class DataParser<T> : ParserBase, IDataParser<T>
    where T : ElementObject, IElementObject
{
    public DataParser(ILogger logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public abstract string DataObjectType { get; }

    /// <summary>
    /// Gets the allowed element types for this parser. Leave empty to allow all types.
    /// </summary>
    protected virtual string[] AllowedElementTypes { get; } = [];

    public virtual T? Parse(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Logger.LogTrace("Id is null or empty");
            return null;
        }

        Logger.LogTrace("Parsing id {Id}", id);

        StormElement? stormElement = HeroesData.GetCompleteStormElement(DataObjectType, id);

        if (stormElement is null)
        {
            Logger.LogWarning("Could not find data for id {Id}", id);
            return null;
        }

        if (AllowedElementTypes.Length > 0 && !AllowedElementTypes.Contains(stormElement.ElementType))
        {
            Logger.LogTrace("Element type {ElementType} for id {Id} is not allowed", stormElement.ElementType, id);
            return null;
        }

        if (stormElement.IsDefault)
        {
            Logger.LogTrace("Element for id {Id} is a default element", id);
            return null;
        }

        T? elementObject = (T?)Activator.CreateInstance(typeof(T), id) ?? throw new InvalidOperationException($"Failed to create instance of type {typeof(T)} for id {id}");

        using (LogContext.PushProperty("XmlPaths", stormElement.OriginalXElements.Select(x => x.StormPath)))
        {
            SetProperties(elementObject, stormElement);

            Logger.LogTrace("Parsing id {Id} complete", id);

            return elementObject;
        }
    }

    /// <summary>
    /// Sets the properties of the <paramref name="elementObject"/> from the <paramref name="stormElement"/>.
    /// </summary>
    /// <param name="elementObject">The type of the element.</param>
    /// <param name="stormElement">The source of data.</param>
    protected abstract void SetProperties(T elementObject, StormElement stormElement);

    protected void SetNameProperty(T elementObject, StormElement stormElement)
    {
        if (elementObject is not IName nameObject)
            return;

        if (stormElement.DataValues.TryGetElementDataAt("name", out StormElementData? nameData))
            nameObject.Name = GameStringTextService.GetGameStringTextFromId(nameData.Value.GetString());
    }

    protected void SetHyperlinkIdProperty(T elementObject, StormElement stormElement)
    {
        if (elementObject is not IHyperlinkId hyperlinkIdObject)
            return;

        if (stormElement.DataValues.TryGetElementDataAt("hyperlinkid", out StormElementData? hyperlinkIdData))
            hyperlinkIdObject.HyperlinkId = hyperlinkIdData.Value.GetString();
    }

    protected void SetRarityProperty(T elementObject, StormElement stormElement)
    {
        if (elementObject is not IRarity rarityObject)
            return;

        if (stormElement.DataValues.TryGetElementDataAt("rarity", out StormElementData? rarityData) && Enum.TryParse(rarityData.Value.GetString(), out Rarity rarity))
            rarityObject.Rarity = rarity;
    }

    protected void SetEventNameProperty(T elementObject, StormElement stormElement)
    {
        if (elementObject is not IEventName eventNameObject)
            return;

        if (stormElement.DataValues.TryGetElementDataAt("eventname", out StormElementData? eventNameData))
            eventNameObject.Event = eventNameData.Value.GetString();
    }

    protected void SetDescriptionProperty(T elementObject, StormElement stormElement)
    {
        if (elementObject is not IDescription descriptionObject)
            return;

        if (stormElement.DataValues.TryGetElementDataAt("description", out StormElementData? descriptionData))
            descriptionObject.Description = GameStringTextService.GetGameStringTextFromId(descriptionData.Value.GetString());
    }

    protected void SetInfoTextProperty(T elementObject, StormElement stormElement)
    {
        if (elementObject is not IInfoText infoTextObject)
            return;

        if (stormElement.DataValues.TryGetElementDataAt("infotext", out StormElementData? infoTextData))
            infoTextObject.InfoText = GameStringTextService.GetGameStringTextFromId(infoTextData.Value.GetString());
    }
}
