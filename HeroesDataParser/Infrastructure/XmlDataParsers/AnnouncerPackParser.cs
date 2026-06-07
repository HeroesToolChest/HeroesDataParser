namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class AnnouncerPackParser : LoadoutItemParserBase<AnnouncerPack>
{
    public AnnouncerPackParser(ILogger<AnnouncerPackParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "AnnouncerPack";

    protected override void SetAdditionalProperties(AnnouncerPack collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("gender", out StormElementData? genderData))
            collectionObject.Gender = genderData.Value.GetString();

        if (stormElement.DataValues.TryGetElementDataAt("heroid", out StormElementData? heroIdData))
            collectionObject.HeroId = heroIdData.Value.GetString();
        if (string.IsNullOrEmpty(collectionObject.HeroId) && stormElement.DataValues.TryGetElementDataAt("hero", out StormElementData? heroData))
            collectionObject.HeroId = heroData.Value.GetString();
    }
}
