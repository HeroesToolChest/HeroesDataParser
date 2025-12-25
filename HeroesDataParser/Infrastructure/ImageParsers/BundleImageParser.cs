namespace HeroesDataParser.Infrastructure.ImageParsers;

public class BundleImageParser : ImageParserBase<Bundle>
{
    public BundleImageParser(ILogger<BundleImageParser> logger)
        : base(logger)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Bundle;

    protected override string SubDirectory => "bundles";

    protected override void SetImages(Bundle element, HashSet<ImageWriterPath> imagePaths)
    {
        AddBasicImage(element, imagePaths);
    }
}
