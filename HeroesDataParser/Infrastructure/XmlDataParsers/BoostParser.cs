namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class BoostParser : CollectionParserBase<Boost>
{
    public BoostParser(ILogger<BoostParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, ITooltipDescriptionService tooltipDescriptionService)
        : base(logger, options, heroesXmlLoaderService, tooltipDescriptionService)
    {
    }

    public override string DataObjectType => "Boost";

    protected override void SetAdditionalProperties(Boost collectionObject, StormElement stormElement)
    {
        return;
    }
}
