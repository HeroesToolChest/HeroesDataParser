namespace HeroesDataParser.Infrastructure.ImageWriters;

public class BundleImageWriter : ImageWriterBase<Bundle>
{
    private const string _bundleDirectory = "bundles";

    private readonly ILogger<BundleImageWriter> _logger;
    private readonly RootOptions _options;
    private readonly HashSet<string> _bundleFilePaths = new(StringComparer.OrdinalIgnoreCase);

    public BundleImageWriter(ILogger<BundleImageWriter> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
        _logger = logger;
        _options = options.Value;
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Bundle;

    protected override void GetImages(Bundle element)
    {
        if (!string.IsNullOrWhiteSpace(element.ImagePath))
            _bundleFilePaths.Add(element.ImagePath);
    }

    protected override async Task SaveImages()
    {
        await SaveImagesFiles(_bundleFilePaths, _bundleDirectory);
    }
}
