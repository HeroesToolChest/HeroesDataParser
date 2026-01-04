namespace HeroesDataParser.Infrastructure.ImageParsers;

public class HeroAbilityImageParser : HeroAbilityTalentImageParser
{
    public HeroAbilityImageParser(ILogger<HeroAbilityImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Ability;

    protected override string SubDirectory => "abilities";

    protected override void SetImages(Hero element)
    {
        SetAbilityImages(element);
    }
}
