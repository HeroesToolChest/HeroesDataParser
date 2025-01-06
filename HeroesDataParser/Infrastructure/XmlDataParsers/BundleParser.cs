namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class BundleParser : CollectionParserBase<Bundle>
{
    private readonly ILogger<BundleParser> _logger;

    private readonly HeroesData _heroesData;

    public BundleParser(ILogger<BundleParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
    }

    public override string DataObjectType => "Bundle";

    protected override void SetAdditionalProperties(Bundle collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("tiletexture", out StormElementData? tileTextureData))
        {
            string tileTexturePath = tileTextureData.Value.GetString();

            StormFile? stormAssetFile = _heroesData.GetStormAssetFile(tileTexturePath);
            if (stormAssetFile is not null)
            {
                collectionObject.Image = Path.ChangeExtension(Path.GetFileName(stormAssetFile.StormPath.Path), ImageFileExtension);
                collectionObject.ImagePath = new RelativeFilePath()
                {
                    FilePath = stormAssetFile.StormPath.Path,
                };
             }
            else
            {
                _logger.LogWarning("Could not get storm asset file from {TileTexturePath}", tileTexturePath);
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("flags", out StormElementData? flagsData))
        {
            if (flagsData.TryGetElementDataAt("ShowDynamicProductContent", out StormElementData? dynamicProductContent))
            {
                if (dynamicProductContent.Value.GetAsInt() == 1)
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
                    if (collectionObject.HeroSkinsByHeroId.TryGetValue(heroData.Value.GetString(), out SortedSet<string>? currentSkinIds))
                        currentSkinIds.Add(skinData.Value.GetString());
                    else
                        collectionObject.HeroSkinsByHeroId[heroData.Value.GetString()] = [skinData.Value.GetString()];
                }
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("boostbonus", out StormElementData? boostData))
        {
            collectionObject.BoostBonusId = boostData.Value.GetString();
        }

        if (stormElement.DataValues.TryGetElementDataAt("goldbonus", out StormElementData? goldData))
        {
            collectionObject.GoldBonus = goldData.Value.GetAsInt();
        }

        if (stormElement.DataValues.TryGetElementDataAt("gemsbonus", out StormElementData? gemsData))
        {
            collectionObject.GemsBonus = gemsData.Value.GetAsInt();
        }

        if (stormElement.DataValues.TryGetElementDataAt("lootchestbonus", out StormElementData? lootChestData))
        {
            collectionObject.LootChestBonus = lootChestData.Value.GetString();
        }

        SetFranchiseProperty(collectionObject, stormElement);
    }
}
