namespace HeroesDataParser.Infrastructure.ImageParsers;

public class HeroTalentImageParser : HeroAbilityTalentImageParser
{
    public HeroTalentImageParser(ILogger<HeroTalentImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Talent;

    protected override string Subdirectory => "talents";

    protected override void SetImages(Hero element)
    {
        SetTalentImages(element);
    }
}
