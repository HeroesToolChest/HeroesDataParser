namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class SkinParser : LoadoutItemParserBase<Skin>
{
    public SkinParser(ILogger<SkinParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "Skin";

    protected override void SetAdditionalProperties(Skin collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("FeatureArray", out StormElementData? featureArrayData))
        {
            IEnumerable<string> featureArrayIndexes = featureArrayData.GetElementDataIndexes();

            foreach (string featureArrayIndex in featureArrayIndexes)
            {
                collectionObject.Features.Add(featureArrayData[featureArrayIndex].Value.GetString());
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("VariationArray", out StormElementData? variationArrayData))
        {
            IEnumerable<string> variationArrayIndexes = variationArrayData.GetElementDataIndexes();

            foreach (string variationArrayIndex in variationArrayIndexes)
            {
                collectionObject.VariationSkinIds.Add(variationArrayData[variationArrayIndex].Value.GetString());
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("VoiceLineArray", out StormElementData? voiceLineArrayData))
        {
            IEnumerable<string> voiceLineArrayIndexes = voiceLineArrayData.GetElementDataIndexes();

            foreach (string voiceLineArrayIndex in voiceLineArrayIndexes)
            {
                collectionObject.VoiceLineIds.Add(voiceLineArrayData[voiceLineArrayIndex].Value.GetString());
            }
        }
    }
}
