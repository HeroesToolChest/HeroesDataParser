namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class VoiceLineParser : LoadoutItemParserBase<VoiceLine>
{
    public VoiceLineParser(ILogger<VoiceLineParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "VoiceLine";

    protected override void SetAdditionalProperties(VoiceLine collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("heroid", out StormElementData? heroIdData))
            collectionObject.HeroId = heroIdData.Value.GetString();
        if (string.IsNullOrEmpty(collectionObject.HeroId) && stormElement.DataValues.TryGetElementDataAt("hero", out StormElementData? heroData))
            collectionObject.HeroId = heroData.Value.GetString();
    }
}
