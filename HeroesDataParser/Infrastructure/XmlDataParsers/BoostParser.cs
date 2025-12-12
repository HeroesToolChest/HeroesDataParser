namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class BoostParser : StoreItemParserBase<Boost>
{
    public BoostParser(ILogger<BoostParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "Boost";

    protected override void SetAdditionalProperties(Boost collectionObject, StormElement stormElement)
    {
        return;
    }
}
