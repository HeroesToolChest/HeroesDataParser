namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class BannerParser : CollectionParserBase<Banner>
{
    public BannerParser(ILogger<BannerParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, ITooltipDescriptionService tooltipDescriptionService)
        : base(logger, options, heroesXmlLoaderService, tooltipDescriptionService)
    {
    }

    public override string DataObjectType => "Banner";

    protected override void SetAdditionalProperties(Banner collectionObject, StormElement stormElement)
    {
        return;
    }
}
