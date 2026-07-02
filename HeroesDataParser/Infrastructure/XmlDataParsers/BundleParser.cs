namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class BundleParser : StoreItemParserBase<Bundle>
{
    public BundleParser(ILogger<BundleParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "Bundle";

    protected override void SetAdditionalProperties(Bundle collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("flags", out StormElementData? flagsData))
        {
            if (flagsData.TryGetElementDataAt("ShowDynamicProductContent", out StormElementData? dynamicProductContent))
            {
                if (dynamicProductContent.Value.GetInt32() == 1)
                    collectionObject.IsDynamicContent = true;
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("heroarray", out StormElementData? heroArrayData))
        {
            collectionObject.HeroIds.UnionWith(heroArrayData.GetElementData().Select(x => x.Value.Value.GetString()).ToHashSet());
        }

        if (stormElement.DataValues.TryGetElementDataAt("mountarray", out StormElementData? mountArrayData))
        {
            collectionObject.MountIds.UnionWith(mountArrayData.GetElementData().Select(x => x.Value.Value.GetString()).ToHashSet());
        }

        if (stormElement.DataValues.TryGetElementDataAt("skinarray", out StormElementData? skinArrayData))
        {
            foreach (var skinArrayElement in skinArrayData.GetElementData())
            {
                if (skinArrayElement.Value.TryGetElementDataAt("hero", out StormElementData? heroData) && skinArrayElement.Value.TryGetElementDataAt("skin", out StormElementData? skinData))
                {
                    if (collectionObject.HeroSkinIdsByHeroId.TryGetValue(heroData.Value.GetString(), out ISet<string>? currentSkinIds))
                        currentSkinIds.Add(skinData.Value.GetString());
                    else
                        collectionObject.HeroSkinIdsByHeroId[heroData.Value.GetString()] = new HashSet<string>(StringComparer.Ordinal) { skinData.Value.GetString() };
                }
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("boostbonus", out StormElementData? boostData))
        {
            collectionObject.BoostBonusId = boostData.Value.GetString();
        }

        if (stormElement.DataValues.TryGetElementDataAt("goldbonus", out StormElementData? goldData))
        {
            collectionObject.GoldBonus = goldData.Value.GetInt32();
        }

        if (stormElement.DataValues.TryGetElementDataAt("gemsbonus", out StormElementData? gemsData))
        {
            collectionObject.GemsBonus = gemsData.Value.GetInt32();
        }

        if (stormElement.DataValues.TryGetElementDataAt("lootchestbonus", out StormElementData? lootChestData))
        {
            collectionObject.LootChestBonus = lootChestData.Value.GetString();
        }
    }
}
