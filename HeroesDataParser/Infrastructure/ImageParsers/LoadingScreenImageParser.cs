namespace HeroesDataParser.Infrastructure.ImageParsers;

public class LoadingScreenImageParser : ImageParserBase<Map>
{
    public LoadingScreenImageParser(ILogger<LoadingScreenImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.LoadingScreen;

    protected override string Subdirectory => "loadingscreens";

    protected override void SetImages(Map element)
    {
        if (!string.IsNullOrWhiteSpace(element.LoadingScreenImage) && !string.IsNullOrWhiteSpace(element.LoadingScreenImagePath?.FilePath))
        {
            AddToFiles(element.LoadingScreenImage, element.Id, async (directoryPath) =>
            {
                await ProcessStaticImage(element.LoadingScreenImage, element.LoadingScreenImagePath, directoryPath);
            });
        }
    }
}
