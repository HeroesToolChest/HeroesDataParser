namespace HeroesDataParser.Infrastructure.ImageParsers;

public class ReplayPreviewImageParser : ImageParserBase<Map>
{
    public ReplayPreviewImageParser(ILogger<ReplayPreviewImageParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.ReplayPreview;

    protected override string SubDirectory => "replaypreviews";

    protected override void SetImages(Map element)
    {
        if (!string.IsNullOrWhiteSpace(element.ReplayPreviewImage) && !string.IsNullOrWhiteSpace(element.ReplayPreviewImagePath?.FilePath))
            AddImagePath(element.ReplayPreviewImage, new ImageRelativePath(element, element.ReplayPreviewImagePath));
    }
}
