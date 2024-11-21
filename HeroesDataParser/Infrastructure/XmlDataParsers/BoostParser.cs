namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class BoostParser : CollectionParserBase<Boost>
{
    public BoostParser(ILogger<BoostParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override string DataObjectType => "Boost";

    protected override void SetAdditionalProperties(Boost collectionObject, StormElement stormElement)
    {
        return;
    }
}
