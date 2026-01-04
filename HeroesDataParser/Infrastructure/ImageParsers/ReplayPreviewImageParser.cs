namespace HeroesDataParser.Infrastructure.ImageParsers;

public class ReplayPreviewImageParser : ImageParserBase<Map>
{
    public ReplayPreviewImageParser(ILogger<ReplayPreviewImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.ReplayPreview;

    protected override string SubDirectory => "replaypreviews";

    protected override void SetImages(Map element)
    {
        if (!string.IsNullOrWhiteSpace(element.ReplayPreviewImage) && !string.IsNullOrWhiteSpace(element.ReplayPreviewImagePath?.FilePath))
        {
            TryAddToFiles(element.ReplayPreviewImage, element.Id, async (directoryPath) =>
            {
                await ProcessBasicImage(element.ReplayPreviewImage, element.ReplayPreviewImagePath, directoryPath);
            });
        }
    }
}
