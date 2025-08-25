namespace HeroesDataParser.Infrastructure.ImageWriters;

public class HeroAbilityImageWriter : ImageWriterBase<Hero>
{
    private const string _abilitiesDirectory = "abilities";

    private readonly Dictionary<string, ImageRelativePath> _heroAbilityRelativePathsByFileName = new(StringComparer.OrdinalIgnoreCase);

    public HeroAbilityImageWriter(ILogger<HeroAbilityImageWriter> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Ability;

    protected override void SetImages(Hero element)
    {
        AbilityTalentImages.SetAbilityImages(element, _heroAbilityRelativePathsByFileName);
    }

    protected override async Task SaveImages()
    {
        await SaveImagesFiles(_heroAbilityRelativePathsByFileName, _abilitiesDirectory);
    }
}
