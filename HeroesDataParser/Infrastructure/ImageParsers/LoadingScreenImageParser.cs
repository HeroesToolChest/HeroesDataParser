namespace HeroesDataParser.Infrastructure.ImageParsers;

public class LoadingScreenImageParser : ImageParserBase<Map>
{
    public LoadingScreenImageParser(ILogger<LoadingScreenImageParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.LoadingScreen;

    protected override string SubDirectory => "loadingscreens";

    protected override void SetImages(Map element)
    {
        if (!string.IsNullOrWhiteSpace(element.LoadingScreenImage) && !string.IsNullOrWhiteSpace(element.LoadingScreenImagePath?.FilePath))
            AddImagePath(element.LoadingScreenImage, new ImageRelativePath(element, element.LoadingScreenImagePath));
    }
}
