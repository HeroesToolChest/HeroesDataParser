namespace HeroesDataParser.Infrastructure.ImageParsers;

public class TypeDescriptionImageParser : ImageParserBase<TypeDescription>
{
    public TypeDescriptionImageParser(ILogger<TypeDescriptionImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.TypeDescription;

    protected override string Subdirectory => "typedescriptions";

    protected override void SetImages(TypeDescription element)
    {
        SetRewardIcon(element);
        SetLargeIcon(element);
    }

    private void SetRewardIcon(TypeDescription element)
    {
        if (string.IsNullOrWhiteSpace(element.RewardIcon) || string.IsNullOrWhiteSpace(element.RewardIconPath?.FilePath))
            return;

        if (!element.TextureSheet.Columns.HasValue || !element.TextureSheet.Rows.HasValue)
            return;

        AddToFiles(element.RewardIcon, element.Id, async (outputDirectoryPath) =>
        {
            ImagePath imagePath = element.RewardIconPath;

            await ProcessImage(element.RewardIcon, imagePath, outputDirectoryPath, (ddsImage, outputFilePath) =>
            {
                int imageWidth = ddsImage.Width / element.TextureSheet.Columns.Value;
                int imageHeight = ddsImage.Height / element.TextureSheet.Rows.Value;

                int xPos = (element.IconSlot % element.TextureSheet.Columns.Value) * imageWidth;
                int yPos = element.IconSlot / element.TextureSheet.Columns.Value * imageHeight;

                return ddsImage.Save(outputFilePath, new SixLabors.ImageSharp.Point(xPos, yPos), new SixLabors.ImageSharp.Size(imageWidth, imageHeight));
            });
        });
    }

    private void SetLargeIcon(TypeDescription element)
    {
        if (string.IsNullOrWhiteSpace(element.LargeIcon) || string.IsNullOrWhiteSpace(element.LargeIconPath?.FilePath))
            return;

        AddToFiles(element.LargeIcon, element.Id, async (outputDirectoryPath) =>
        {
            await ProcessStaticImage(element.LargeIcon, element.LargeIconPath, outputDirectoryPath);
        });
    }
}
