namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class BannerParser : LoadoutItemParserBase<Banner>
{
    public BannerParser(ILogger<BannerParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "Banner";

    protected override void SetAdditionalProperties(Banner collectionObject, StormElement stormElement)
    {
        return;
    }
}
