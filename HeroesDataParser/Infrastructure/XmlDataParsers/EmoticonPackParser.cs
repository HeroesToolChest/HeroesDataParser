namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class EmoticonPackParser : StoreItemParserBase<EmoticonPack>
{
    public EmoticonPackParser(ILogger<EmoticonPackParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "EmoticonPack";

    protected override void SetAdditionalProperties(EmoticonPack collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("EmoticonArray", out StormElementData? emoticonArrayData))
        {
            IEnumerable<string> emoticonArrayIndexes = emoticonArrayData.GetElementDataIndexes();

            foreach (string emoticonArrayIndex in emoticonArrayIndexes)
            {
                collectionObject.EmoticonIds.Add(emoticonArrayData[emoticonArrayIndex].Value.GetString());
            }
        }
    }
}
