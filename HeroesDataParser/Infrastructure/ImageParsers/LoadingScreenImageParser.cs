namespace HeroesDataParser.Infrastructure.ImageParsers;

public class LoadingScreenImageParser : ImageParserBase<Map>
{
    public LoadingScreenImageParser(ILogger<LoadingScreenImageParser> logger)
        : base(logger)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.LoadingScreen;

    protected override string SubDirectory => "loadingscreens";

    protected override void SetImages(Map element, HashSet<ImageWriterPath> imagePaths)
    {
        if (!string.IsNullOrWhiteSpace(element.LoadingScreenImage) && !string.IsNullOrWhiteSpace(element.LoadingScreenImagePath?.FilePath))
            TryAddToFiles(imagePaths, element.LoadingScreenImage, element.LoadingScreenImagePath, element);
    }
}
