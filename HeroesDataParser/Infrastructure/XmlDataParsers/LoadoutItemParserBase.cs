namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public abstract class LoadoutItemParserBase<T> : StoreItemParserBase<T>
    where T : ElementObject, IElementObject, ILoadoutItem
{
    public LoadoutItemParserBase(ILogger logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    protected override void SetProperties(T collectionObject, StormElement stormElement)
    {
        base.SetProperties(collectionObject, stormElement);
    }

    protected override void SetCommonProperties(T collectionObject, StormElement stormElement)
    {
        base.SetCommonProperties(collectionObject, stormElement);

        if (stormElement.DataValues.TryGetElementDataAt("attributeid", out StormElementData? attributeIdData))
            collectionObject.AttributeId = attributeIdData.Value.GetString();
    }
}