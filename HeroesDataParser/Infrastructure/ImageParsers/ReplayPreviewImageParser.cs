namespace HeroesDataParser.Infrastructure.ImageParsers;

public class ReplayPreviewImageParser : ImageParserBase<Map>
{
    public ReplayPreviewImageParser(ILogger<ReplayPreviewImageParser> logger)
        : base(logger)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.ReplayPreview;

    protected override string SubDirectory => "replaypreviews";

    protected override void SetImages(Map element, HashSet<ImageWriterPath> imagePaths)
    {
        if (!string.IsNullOrWhiteSpace(element.ReplayPreviewImage) && !string.IsNullOrWhiteSpace(element.ReplayPreviewImagePath?.FilePath))
            TryAddToFiles(imagePaths, element.ReplayPreviewImage, element.ReplayPreviewImagePath, element);
    }
}
