namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class MountParser : LoadoutItemParserBase<Mount>
{
    public MountParser(ILogger<MountParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
    : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "Mount";

    protected override void SetAdditionalProperties(Mount collectionObject, StormElement stormElement)
    {
        SetImageProperty(collectionObject, stormElement);

        if (stormElement.DataValues.TryGetElementDataAt("MountCategory", out StormElementData? mountCategoryData))
        {
            collectionObject.MountCategory = mountCategoryData.Value.GetString();
        }

        if (stormElement.DataValues.TryGetElementDataAt("VariationArray", out StormElementData? variationArrayData))
        {
            IEnumerable<string> variationArrayIndexes = variationArrayData.GetElementDataIndexes();

            foreach (string variationArrayIndex in variationArrayIndexes)
            {
                collectionObject.VariationMountIds.Add(variationArrayData[variationArrayIndex].Value.GetString());
            }
        }
    }
}
