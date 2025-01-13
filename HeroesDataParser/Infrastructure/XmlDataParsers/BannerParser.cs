namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class BannerParser : CollectionParserBase<Banner>
{
    public BannerParser(ILogger<BannerParser> logger, IHeroesDataLoaderService heroesDataLoaderService)
        : base(logger, heroesDataLoaderService)
    {
    }

    public override string DataObjectType => "Banner";

    protected override void SetAdditionalProperties(Banner collectionObject, StormElement stormElement)
    {
        return;
    }
}
