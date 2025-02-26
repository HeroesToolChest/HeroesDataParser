using Heroes.Element.Models.AbilityTalents;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers;

public class TalentParser
{
    private readonly ILogger<AnnouncerParser> _logger;
    private readonly HeroesData _heroesData;

    public TalentParser(ILogger<AnnouncerParser> logger, HeroesData heroesData)
    {
        _logger = logger;
        _heroesData = heroesData;
    }

    //public void GetTalent(string id)
    //{
    //    StormElement? stormElement = _heroesData.GetCompleteStormElement("Talent", id);

    //    if (stormElement is null)
    //    {
    //        _logger.LogWarning("Talent element does not exist for id {Id}", id);
    //        return null;
    //    }

    //    Talent talent = new();

    //    //if (stormElement.DataValues.TryGetElementDataAt("talent", out StormElementData? talentData))
    //    //    talent.AbilityTalentId = talentData.Value.GetString();
    //}
}
