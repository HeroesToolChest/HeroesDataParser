namespace HeroesDataParser.Infrastructure.ImageParsers;

public class HeroAbilityTalentImageParser : ImageParserBase<Hero>
{
    public HeroAbilityTalentImageParser(ILogger<HeroAbilityTalentImageParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.AbilityTalent;

    protected override string SubDirectory => "abilityTalents";

    protected override void SetImages(Hero element)
    {
        AbilityTalentImages.SetAbilityImages(element, AddImagePath);
        AbilityTalentImages.SetTalentImages(element, AddImagePath);
    }
}
