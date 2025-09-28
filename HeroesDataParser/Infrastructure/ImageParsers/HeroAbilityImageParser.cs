namespace HeroesDataParser.Infrastructure.ImageParsers;

public class HeroAbilityImageParser : ImageParserBase<Hero>
{
    public HeroAbilityImageParser(ILogger<HeroAbilityImageParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Ability;

    protected override string SubDirectory => "abilities";

    protected override void SetImages(Hero element)
    {
        AbilityTalentImages.SetAbilityImages(element, AddImagePath);
    }
}
