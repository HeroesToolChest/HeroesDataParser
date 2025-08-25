namespace HeroesDataParser.Infrastructure.ImageWriters;

public class HeroTalentImageWriter : ImageWriterBase<Hero>
{
    private const string _talentsDirectory = "talents";

    private readonly Dictionary<string, ImageRelativePath> _heroTalentRelativePathsByFileName = new(StringComparer.OrdinalIgnoreCase);

    public HeroTalentImageWriter(ILogger<HeroTalentImageWriter> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Talent;

    protected override void SetImages(Hero element)
    {
        AbilityTalentImages.SetTalentImages(element, _heroTalentRelativePathsByFileName);
    }

    protected override async Task SaveImages()
    {
        await SaveImagesFiles(_heroTalentRelativePathsByFileName, _talentsDirectory);
    }
}
