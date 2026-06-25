namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class TypeDescriptionParser : DataParser<TypeDescription>
{
    public TypeDescriptionParser(ILogger<TypeDescriptionParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "TypeDescription";

    protected override void SetProperties(TypeDescription elementObject, StormElement stormElement)
    {
        SetNameProperty(elementObject, stormElement);

        if (stormElement.DataValues.TryGetElementDataAt("RewardIcon", out StormElementData? rewardIconData))
        {
            if (rewardIconData.TryGetElementDataAt("Index", out StormElementData? indexAttribute))
                elementObject.IconSlot = indexAttribute.Value.GetInt();

            if (rewardIconData.TryGetElementDataAt("TextureSheet", out StormElementData? textureSheetData))
            {
                string texture = textureSheetData.Value.GetString();

                SetTextureSheetProperties(elementObject, texture);
                SetRewardIcon(elementObject, elementObject.TextureSheet.ImagePath);
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("LargeIcon", out StormElementData? largeIconData))
        {
            string texture = largeIconData.Value.GetString();
            ImageFileNamePath? imageFilePath = GetStaticImageFilePath(texture);

            if (imageFilePath is not null)
            {
                elementObject.LargeIcon = imageFilePath.Image;
                elementObject.LargeIconPath = imageFilePath.FilePath;
            }
            else if (!string.IsNullOrWhiteSpace(texture))
            {
                if (Logger.IsEnabled(LogLevel.Warning))
                    Logger.LogWarning("Could not get storm asset file from {texture}", texture);
            }
        }
    }

    private void SetRewardIcon(TypeDescription elementObject, string? texture)
    {
        ImageFileNamePath? imageFilePath = GetStaticImageFilePath(texture);

        if (imageFilePath is not null)
        {
            elementObject.RewardIcon = $"storm_ui_heroes_reward_icon_{elementObject.Id.ToLowerInvariant()}.{StaticImageFileExtension}";
            elementObject.RewardIconPath = imageFilePath.FilePath;
        }
        else if (!string.IsNullOrWhiteSpace(texture))
        {
            if (Logger.IsEnabled(LogLevel.Warning))
                Logger.LogWarning("Could not get storm asset file from {texture}", texture);
        }
    }

    private void SetTextureSheetProperties(TypeDescription elementObject, string textureSheetId)
    {
        StormElement? textureSheetElement = HeroesData.GetCompleteStormElement("TextureSheet", textureSheetId);

        if (textureSheetElement is null)
            return;

        if (textureSheetElement.DataValues.TryGetElementDataAt("Image", out StormElementData? imageData))
            elementObject.TextureSheet.ImagePath = imageData.Value.GetString();

        if (textureSheetElement.DataValues.TryGetElementDataAt("Rows", out StormElementData? rowsData))
            elementObject.TextureSheet.Rows = rowsData.Value.GetInt();

        if (textureSheetElement.DataValues.TryGetElementDataAt("Columns", out StormElementData? columnsData))
            elementObject.TextureSheet.Columns = columnsData.Value.GetInt();
    }
}
