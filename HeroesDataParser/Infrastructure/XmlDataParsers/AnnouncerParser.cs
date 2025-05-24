namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class AnnouncerParser : CollectionParserBase<Announcer>
{
    public AnnouncerParser(ILogger<AnnouncerParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, ITooltipDescriptionService tooltipDescriptionService)
        : base(logger, options, heroesXmlLoaderService, tooltipDescriptionService)
    {
    }

    public override string DataObjectType => "AnnouncerPack";

    protected override void SetAdditionalProperties(Announcer collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("gender", out StormElementData? genderData))
            collectionObject.Gender = genderData.Value.GetString();

        if (stormElement.DataValues.TryGetElementDataAt("tiletexture", out StormElementData? tileTextureData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(tileTextureData);
            if (imageFilePath is not null)
            {
                collectionObject.Image = imageFilePath.Image;
                collectionObject.ImagePath = imageFilePath.FilePath;
            }

            //string tileTexturePath = tileTextureData.Value.GetString();

            //StormFile? stormAssetFile = _heroesData.GetStormAssetFile(tileTexturePath);
            //if (stormAssetFile is not null)
            //{
            //    collectionObject.Image = Path.ChangeExtension(Path.GetFileName(stormAssetFile.StormPath.Path), ImageFileExtension);
            //    collectionObject.ImagePath = new RelativeFilePath()
            //    {
            //        FilePath = stormAssetFile.StormPath.Path,
            //    };
            //}
            //else
            //{
            //    _logger.LogWarning("Could not get storm asset file from {TileTexturePath}", tileTexturePath);
            //}
        }

        if (stormElement.DataValues.TryGetElementDataAt("heroid", out StormElementData? heroIdData))
            collectionObject.HeroId = heroIdData.Value.GetString();
        if (stormElement.DataValues.TryGetElementDataAt("hero", out StormElementData? heroData))
            collectionObject.HeroId = heroData.Value.GetString();
    }
}
