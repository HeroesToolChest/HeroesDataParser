namespace HeroesDataParser.Infrastructure.ImageParsers;

public class HeroTalentImageParser : ImageParserBase<Hero>
{
    public HeroTalentImageParser(ILogger<HeroTalentImageParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Talent;

    protected override string SubDirectory => "talents";

    protected override void SetImages(Hero element)
    {
        AbilityTalentImages.SetTalentImages(element, AddImagePath);
    }
}
