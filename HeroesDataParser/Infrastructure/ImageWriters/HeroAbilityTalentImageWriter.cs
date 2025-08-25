namespace HeroesDataParser.Infrastructure.ImageWriters;

public class HeroAbilityTalentImageWriter : ImageWriterBase<Hero>
{
    private const string _abilityTalentsDirectory = "abilityTalents";

    private readonly Dictionary<string, ImageRelativePath> _heroAbilityTalentRelativePathsByFileName = new(StringComparer.OrdinalIgnoreCase);

    public HeroAbilityTalentImageWriter(ILogger<HeroAbilityTalentImageWriter> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.AbilityTalent;

    protected override void SetImages(Hero element)
    {
        AbilityTalentImages.SetAbilityImages(element, _heroAbilityTalentRelativePathsByFileName);
        AbilityTalentImages.SetTalentImages(element, _heroAbilityTalentRelativePathsByFileName);
    }

    protected override async Task SaveImages()
    {
        await SaveImagesFiles(_heroAbilityTalentRelativePathsByFileName, _abilityTalentsDirectory);
    }
}
