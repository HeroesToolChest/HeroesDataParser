namespace HeroesDataParser.Infrastructure.ImageParsers;

public class BundleImageParser : ImageParserBase<Bundle>
{
    public BundleImageParser(ILogger<BundleImageParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Bundle;

    protected override string SubDirectory => "bundles";

    protected override void SetImages(Bundle element)
    {
        if (!string.IsNullOrWhiteSpace(element.Image) && !string.IsNullOrWhiteSpace(element.ImagePath?.FilePath))
            AddImagePath(element.Image, new ImageRelativePath(element, element.ImagePath));
    }
}
