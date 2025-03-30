using Heroes.Element.Models.AbilityTalents;
using Heroes.Element.Types;
using System.Runtime.CompilerServices;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers;

public class TalentParser : AbilityTalentParserBase, ITalentParser
{
    private readonly ILogger<TalentParser> _logger;
    private readonly HeroesData _heroesData;
    private readonly IAbilityParser _abilityParser;

    public TalentParser(ILogger<TalentParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService, IAbilityParser abilityParser)
        : base(logger, heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
        _abilityParser = abilityParser;
    }

    // <TalentTreeArray Talent="SamuroWayOfIllusion" Tier="1" Column="1" />
    public Talent? GetTalent(Hero hero, StormElementData talentTreeData)
    {
        string? talentValue = null;
        string? tierValue = null;
        string? columnValue = null;

        if (talentTreeData.TryGetElementDataAt("Talent", out StormElementData? talentData))
            talentValue = talentData.Value.GetString();
        if (string.IsNullOrEmpty(talentValue))
            return null;

        if (talentTreeData.TryGetElementDataAt("Tier", out StormElementData? tierData))
            tierValue = tierData.Value.GetString();
        if (string.IsNullOrEmpty(tierValue))
            return null;

        if (talentTreeData.TryGetElementDataAt("Column", out StormElementData? columnData))
            columnValue = columnData.Value.GetString();
        if (string.IsNullOrEmpty(columnValue))
            return null;

        Talent talent = new()
        {
            NameId = talentValue,
            Column = int.Parse(columnValue),
        };

        SetTalentData(hero, talent);
        SetTalentTier(talent, tierValue);

        return talent;
    }

    private static void SetTalentTier(Talent talent, string tier)
    {
        if (tier == "1")
            talent.Tier = TalentTier.Level1;
        else if (tier == "2")
            talent.Tier = TalentTier.Level4;
        else if (tier == "3")
            talent.Tier = TalentTier.Level7;
        else if (tier == "4")
            talent.Tier = TalentTier.Level10;
        else if (tier == "5")
            talent.Tier = TalentTier.Level13;
        else if (tier == "6")
            talent.Tier = TalentTier.Level16;
        else if (tier == "7")
            talent.Tier = TalentTier.Level20;
        else
            talent.Tier = TalentTier.Unknown;
    }

    private void SetTalentData(Hero hero, Talent talent)
    {
        StormElement? talentElement = _heroesData.GetCompleteStormElement("Talent", (string)talent.NameId);
        if (talentElement is null)
            return;

        StormElementData talentDataValues = talentElement.DataValues;

        if (talentDataValues.TryGetElementDataAt("Face", out StormElementData? faceData))
        {
            talent.ButtonId = faceData.Value.GetString();

            if (hero.GetAbilityTypeByButtonId(talent.ButtonId, out AbilityType abilityType))
                talent.AbilityType = abilityType;
            else
                talent.AbilityType = AbilityType.Unknown;

            SetButtonData(talent);
        }

        //// TODO: Trait

        //// TODO: ACtive
    }


}
