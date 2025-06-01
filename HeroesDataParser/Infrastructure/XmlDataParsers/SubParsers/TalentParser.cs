namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers;

public class TalentParser : AbilityTalentParserBase, ITalentParser
{
    public TalentParser(ILogger<TalentParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, ITooltipDescriptionService tooltipDescriptionService)
        : base(logger, options, heroesXmlLoaderService, tooltipDescriptionService)
    {
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
            TalentElementId = talentValue,
            Column = int.Parse(columnValue),
        };

        SetTalentData(hero, talent);
        SetTalentTier(talent, tierValue);

        if (talentTreeData.TryGetElementDataAt("PrerequisiteTalentArray", out StormElementData? prerequisiteTalentArrayDataArray))
        {
            foreach (string index in prerequisiteTalentArrayDataArray.GetElementDataIndexes())
            {
                StormElementData prerequisiteTalentArrayData = prerequisiteTalentArrayDataArray.GetElementDataAt(index);

                string value = prerequisiteTalentArrayData.Value.GetString();
                if (!string.IsNullOrEmpty(value) && HeroesData.StormElementExists("Talent", value))
                    talent.PrerequisiteTalentIds.Add(value);
                else
                    Logger.LogWarning("Talent {Talent} has an unknown prerequisite talent id {PrerequisiteTalentId}.", talentValue, value);
            }
        }

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
        StormElement? talentElement = HeroesData.GetCompleteStormElement("Talent", talent.TalentElementId);
        if (talentElement is null)
            return;

        StormElementData talentDataValues = talentElement.DataValues;

        if (talentDataValues.TryGetElementDataAt("QuestData", out StormElementData? questData) &&
            questData.TryGetElementDataAt("StackBehavior", out StormElementData? stackBehaviorData) &&
            !string.IsNullOrEmpty(stackBehaviorData.Value.GetString()))
        {
            talent.IsQuest = true;
        }

        // set the IsActive, we do not know if it's an active abilityType yet
        if (talentDataValues.TryGetElementDataAt("Active", out StormElementData? activeData) && activeData.Value.GetString() == "1")
            talent.IsActive = true;

        if (talentDataValues.TryGetElementDataAt("Abil", out StormElementData? abilityData))
        {
            talent.AbilityElementId = abilityData.Value.GetString();

            // find the (first) matching ability we have for the hero
            if (hero.GetAbilityTypeByNameId(talent.AbilityElementId, out AbilityType abilityType))
            {
                talent.AbilityType = abilityType;
            }
            else
            {
                // search through all hero units
                foreach (Unit heroUnit in hero.HeroUnits.Values)
                {
                    if (heroUnit.GetAbilityTypeByNameId(talent.AbilityElementId, out abilityType))
                    {
                        talent.AbilityType = abilityType;
                    }
                }
            }

            if (talent.IsActive)
                SetAbilityData(talent.AbilityElementId, talent);
        }

        // the trait check should come after the ability data is set
        // hidden means that it was an ability that is in the AbilArray but not in the command array
        if (((talent.IsActive is true && (talent.AbilityType == AbilityType.Unknown)) ||
            (talent.IsActive is false && (talent.AbilityType == AbilityType.Unknown || talent.AbilityType == AbilityType.Hidden))) &&
            talentDataValues.TryGetElementDataAt("Trait", out StormElementData? traitData) && traitData.Value.GetString() == "1")
        {
            talent.AbilityType = AbilityType.Trait;
        }

        // button data should come after ability data
        if (talentDataValues.TryGetElementDataAt("Face", out StormElementData? faceData))
        {
            talent.ButtonElementId = faceData.Value.GetString();

            SetButtonData(talent);
        }

        // if not set, set to the passive abilityId
        if (string.IsNullOrEmpty(talent.AbilityElementId))
            talent.AbilityElementId = PassiveAbilityElementId;

        // if not set, set to the button element id to none
        if (string.IsNullOrEmpty(talent.ButtonElementId))
            talent.ButtonElementId = NoButtonElementId;

        // if it's not an IsActive and if the abilityType is still Unknown or Hidden, then set it to a Passive abilityType
        if (talent is { IsActive: false, AbilityType: AbilityType.Unknown or AbilityType.Hidden })
            talent.AbilityType = AbilityType.Passive;

        // if it's an IsActive and if the abilityType is still Unknown or Hidden, then set it to an Active abilityType
        if (talent is { IsActive: true, AbilityType: AbilityType.Unknown or AbilityType.Hidden })
            talent.AbilityType = AbilityType.Active;
    }

    private void SetAbilityData(string abilityId, AbilityTalentBase abilityTalent, string? abilCmdIndex = null)
    {
        StormElement? abilityElement = HeroesData.GetCompleteStormElement("Abil", abilityId);
        if (abilityElement is null)
            return;

        SetAbilityData(abilityElement, abilityTalent, abilCmdIndex);
    }
}
