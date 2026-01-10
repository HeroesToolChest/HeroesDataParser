namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class SprayParser : LoadoutItemParserBase<Spray>
{
    public SprayParser(ILogger<SprayParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
    : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "Spray";

    protected override void SetAdditionalProperties(Spray collectionObject, StormElement stormElement)
    {
        int animCount = 0;
        int animDuration = 0;

        string texture = string.Empty;

        if (stormElement.DataValues.TryGetElementDataAt("AnimCount", out StormElementData? animCountData))
            animCount = animCountData.Value.GetInt();

        if (stormElement.DataValues.TryGetElementDataAt("AnimDuration", out StormElementData? animDurationData))
            animDuration = animDurationData.Value.GetInt();

        if (stormElement.DataValues.TryGetElementDataAt("Texture", out StormElementData? textureData))
            texture = textureData.Value.GetString();

        if (animCount > 0 && animDuration > 0)
        {
            collectionObject.Animation = new SprayAnimation
            {
                Texture = GetStaticImageOutputFileName(texture),
                Frames = animCount,
                Duration = animDuration,
            };
        }

        ImageFilePath? imageFilePath;

        if (collectionObject.Animation is not null)
            imageFilePath = GetAnimatedImageFilePath(texture);
        else
            imageFilePath = GetStaticImageFilePath(texture);

        if (imageFilePath is not null)
        {
            collectionObject.Image = imageFilePath.Image;
            if (collectionObject is IImagePath imagePathObject)
                imagePathObject.ImagePath = imageFilePath.FilePath;
        }
        else
        {
            if (Logger.IsEnabled(LogLevel.Warning))
                Logger.LogWarning("Could not get storm asset file from {texture}", texture);
        }
    }
}
