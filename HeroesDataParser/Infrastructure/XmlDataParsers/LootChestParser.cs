namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class LootChestParser : DataParser<LootChest>
{
    public LootChestParser(ILogger<LootChestParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "LootChest";

    protected override void SetProperties(LootChest elementObject, StormElement stormElement)
    {
        SetNameProperty(elementObject, stormElement);
        SetHyperlinkIdProperty(elementObject, stormElement);
        SetRarityProperty(elementObject, stormElement);
        SetEventNameProperty(elementObject, stormElement);
        SetDescriptionProperty(elementObject, stormElement);

        if (stormElement.DataValues.TryGetElementDataAt("maxrerolls", out StormElementData? maxRerollsData) && maxRerollsData.Value.TryGetInt32(out int maxRerollsValue))
            elementObject.MaxRerolls = maxRerollsValue;

        if (stormElement.DataValues.TryGetElementDataAt("typedescription", out StormElementData? typeDescriptionData))
            elementObject.TypeDescriptionId = typeDescriptionData.Value.GetString();
    }
}
