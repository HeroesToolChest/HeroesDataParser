namespace HeroesDataParser.Infrastructure.ImageParsers;

public class ReplayPreviewImageParser : ImageParserBase<Map>
{
    public ReplayPreviewImageParser(ILogger<ReplayPreviewImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.ReplayPreview;

    protected override string Subdirectory => "replaypreviews";

    protected override void SetImages(Map element)
    {
        if (!string.IsNullOrWhiteSpace(element.ReplayPreviewImage) && !string.IsNullOrWhiteSpace(element.ReplayPreviewImagePath?.FilePath))
        {
            AddToFiles(element.ReplayPreviewImage, element.Id, async (directoryPath) =>
            {
                await ProcessStaticImage(element.ReplayPreviewImage, element.ReplayPreviewImagePath, directoryPath);
            });
        }
    }
}
