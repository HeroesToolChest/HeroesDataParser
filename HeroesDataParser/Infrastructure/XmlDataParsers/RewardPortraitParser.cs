namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class RewardPortraitParser : StoreItemParserBase<RewardPortrait>
{
    public RewardPortraitParser(ILogger<RewardPortraitParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "Reward";

    protected override string[] AllowedElementTypes => ["CRewardPortrait"];

    protected override void SetAdditionalProperties(RewardPortrait collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("DescriptionUnearned", out StormElementData? descriptionUnearnedData))
            collectionObject.DescriptionUnearned = GameStringTextService.GetGameStringTextFromId(descriptionUnearnedData.Value.GetString());

        if (stormElement.DataValues.TryGetElementDataAt("PortraitPack", out StormElementData? portraitPackIdData))
            collectionObject.PortraitPackId = portraitPackIdData.Value.GetString();

        if (stormElement.DataValues.TryGetElementDataAt("Hero", out StormElementData? heroIdData))
            collectionObject.HeroId = heroIdData.Value.GetString();

        if (stormElement.DataValues.TryGetElementDataAt("IconSlot", out StormElementData? iconSlotData))
            collectionObject.IconSlot = iconSlotData.Value.GetInt32();

        if (stormElement.DataValues.TryGetElementDataAt("IconFile", out StormElementData? iconFileData))
        {
            string filePath = iconFileData.Value.GetString();

            collectionObject.TextureSheet.ImagePath = filePath;
            collectionObject.TextureSheet.Image = Path.ChangeExtension(filePath, StaticImageFileExtension);
        }

        if (stormElement.DataValues.TryGetElementDataAt("IconCols", out StormElementData? iconColsData))
            collectionObject.TextureSheet.Columns = iconColsData.Value.GetInt32();

        if (stormElement.DataValues.TryGetElementDataAt("IconRows", out StormElementData? iconRowsData))
            collectionObject.TextureSheet.Rows = iconRowsData.Value.GetInt32();

        collectionObject.Image = $"storm_portrait_{collectionObject.Id.ToLowerInvariant()}.{StaticImageFileExtension}";
    }
}
