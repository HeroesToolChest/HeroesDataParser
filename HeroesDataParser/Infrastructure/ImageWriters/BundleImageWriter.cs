namespace HeroesDataParser.Infrastructure.ImageWriters;

public class BundleImageWriter : ImageWriterBase<Bundle>
{
    private const string _bundleDirectory = "bundles";

    private readonly Dictionary<string, ImageRelativePath> _bundleRelativePathsByFileName = new(StringComparer.OrdinalIgnoreCase);

    public BundleImageWriter(ILogger<BundleImageWriter> logger, IOptions<RootOptions> options, IHeroesDataLoaderService heroesDataLoaderService)
        : base(logger, options, heroesDataLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Bundle;

    protected override void SetImages(Bundle element)
    {
        if (!string.IsNullOrWhiteSpace(element.Image) && !string.IsNullOrWhiteSpace(element.ImagePath?.FilePath))
            _bundleRelativePathsByFileName.TryAdd(element.Image, new ImageRelativePath(element, element.ImagePath));
    }

    protected override async Task SaveImages()
    {
        await SaveImagesFiles(_bundleRelativePathsByFileName, _bundleDirectory);
    }
}
