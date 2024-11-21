namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class BannerParser : CollectionParserBase<Banner>
{
    public BannerParser(ILogger<BannerParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override string DataObjectType => "Banner";

    protected override void SetAdditionalProperties(Banner collectionObject, StormElement stormElement)
    {
        return;
    }
}
