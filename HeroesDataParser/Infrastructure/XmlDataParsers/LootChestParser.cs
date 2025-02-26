namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class LootChestParser : DataParser<LootChest>
{
    public LootChestParser(ILogger<LootChestParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override string DataObjectType => "LootChest";

    protected override void SetProperties(LootChest elementObject, StormElement stormElement)
    {
        try
        { 
        SetNameProperty(elementObject, stormElement);
        SetHyperlinkIdProperty(elementObject, stormElement);
        SetRarityProperty(elementObject, stormElement);
        SetEventNameProperty(elementObject, stormElement);

        if (stormElement.DataValues.TryGetElementDataAt("maxrerolls", out StormElementData? maxRerollsData) && maxRerollsData.Value.TryGetInt32(out int maxRerollsValue))
            elementObject.MaxRerolls = maxRerollsValue;

        if (stormElement.DataValues.TryGetElementDataAt("typedescription", out StormElementData? typeDescriptionData))
            elementObject.TypeDescription = typeDescriptionData.Value.GetString();

        SetDescriptionProperty(elementObject, stormElement);
    }
        catch (Exception ex)
        {
            string x = "";
            //_logger.LogError(ex, "Failed to parse LootChest");
        }
    }
}
