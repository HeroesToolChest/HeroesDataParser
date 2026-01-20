namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class PortraitPackParser : StoreItemParserBase<PortraitPack>
{
    public PortraitPackParser(ILogger<PortraitPackParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "PortraitPack";

    protected override void SetAdditionalProperties(PortraitPack collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("PortraitArray", out StormElementData? portraitArrayData))
        {
            IEnumerable<string> portraitArrayIndexes = portraitArrayData.GetElementDataIndexes();

            foreach (string portraitArrayIndex in portraitArrayIndexes)
            {
                collectionObject.RewardPortraitIds.Add(portraitArrayData[portraitArrayIndex].Value.GetString());
            }
        }
    }
}
