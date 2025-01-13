namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class BoostParser : CollectionParserBase<Boost>
{
    public BoostParser(ILogger<BoostParser> logger, IHeroesDataLoaderService heroesDataLoaderService)
        : base(logger, heroesDataLoaderService)
    {
    }

    public override string DataObjectType => "Boost";

    protected override void SetAdditionalProperties(Boost collectionObject, StormElement stormElement)
    {
        return;
    }
}
