namespace HeroesDataParser.Infrastructure.ImageParsers;

public class LoadingScreenImageParser : ImageParserBase<Map>
{
    public LoadingScreenImageParser(ILogger<LoadingScreenImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.LoadingScreen;

    protected override string SubDirectory => "loadingscreens";

    protected override void SetImages(Map element)
    {
        if (!string.IsNullOrWhiteSpace(element.LoadingScreenImage) && !string.IsNullOrWhiteSpace(element.LoadingScreenImagePath?.FilePath))
        {
            TryAddToFiles(element.LoadingScreenImage, element.Id, async (directoryPath) =>
            {
                await ProcessBasicImage(element.LoadingScreenImage, element.LoadingScreenImagePath, directoryPath);
            });
        }
    }
}
