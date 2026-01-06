namespace HeroesDataParser.Infrastructure.ImageParsers;

public class UnitAbilityImageParser : UnitAbilityTalentImageParser
{
    public UnitAbilityImageParser(ILogger<UnitAbilityImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Ability;

    protected override string SubDirectory => "abilities";

    protected override void SetImages(Unit element)
    {
        SetAbilityImages(element);
    }
}
