namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers;

public class AbilityParser : AbilityTalentParserBase, IAbilityParser
{
    public AbilityParser(ILogger<AbilityParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, ITooltipDescriptionService tooltipDescriptionService)
        : base(logger, options, heroesXmlLoaderService, tooltipDescriptionService)
    {
    }

    public static (string AbilityId, string Index) GetAbilCmdSplit(string abilCmdValue)
    {
        ReadOnlySpan<char> abilCmdSpan = abilCmdValue.AsSpan();
        ReadOnlySpan<char> firstPartAbilCmdSpan;
        ReadOnlySpan<char> indexPartAbilCmdSpan;

        int index = abilCmdSpan.IndexOf(',');

        if (index > 0)
        {
            firstPartAbilCmdSpan = abilCmdSpan[..index];
            indexPartAbilCmdSpan = abilCmdSpan[(index + 1)..];
        }
        else
        {
            firstPartAbilCmdSpan = abilCmdSpan;
            indexPartAbilCmdSpan = string.Empty;
        }

        return (firstPartAbilCmdSpan.ToString(), indexPartAbilCmdSpan.ToString());
    }

    public Ability? GetAbility(StormElementData layoutButtonData)
    {
        string? faceValue = null;
        string? typeValue = null;
        string? abilCmdValue = null;
        string? requirementsValue = null;
        string slotsValue = string.Empty;

        if (layoutButtonData.TryGetElementDataAt("Face", out StormElementData? faceData))
            faceValue = faceData.Value.GetString();
        if (string.IsNullOrEmpty(faceValue))
            return null;

        if (layoutButtonData.TryGetElementDataAt("Slot", out StormElementData? slotsData))
            slotsValue = slotsData.Value.GetString();
        if (IsSlotValueMatch(slotsValue))
            return null;

        if (layoutButtonData.TryGetElementDataAt("Type", out StormElementData? typeData))
            typeValue = typeData.Value.GetString();
        if (!string.IsNullOrWhiteSpace(typeValue) && IsTypeValueMatch(typeValue))
            return null;

        if (layoutButtonData.TryGetElementDataAt("Requirements", out StormElementData? requirementsData))
            requirementsValue = requirementsData.Value.GetString();
        if (!string.IsNullOrWhiteSpace(requirementsValue) && IsRequirementsValueMatch(requirementsValue))
            return null;

        if (layoutButtonData.TryGetElementDataAt("AbilCmd", out StormElementData? abilCmdData))
            abilCmdValue = abilCmdData.Value.GetString();

        Ability ability = new()
        {
            ButtonElementId = faceValue,
        };

        // set the ability type
        SetAbilityTypeFromSlot(ability, slotsValue, false);

        // if type is not a type we want, return
        if (IgnoreAbilityType(ability))
            return null;

        // set the ability tier
        SetAbilityTierFromAbilityType(ability);

        // if tier is not a tier we want, return
        if (IgnoreAbilityTier(ability))
            return null;

        // passive "ability", actually just a dummy button
        if (typeValue.AsSpan().Equals("Passive", StringComparison.OrdinalIgnoreCase))
        {
            ability.AbilityElementId = PassiveAbilityElementId;
            ability.ButtonElementId = faceValue;
            ability.IsActive = false;

            SetButtonData(ability);
        }
        else if (!string.IsNullOrEmpty(abilCmdValue)) // non-passive
        {
            (string abilityId, string index) = GetAbilCmdSplit(abilCmdValue);

            ability.AbilityElementId = abilityId;

            SetAbilityData(ability, index);
        }
        else
        {
            // doesn't have an abilcmd value set, so this is just a dummy button that doesn't do anything
            // most likely still has an ability set in the ability array but wasn't used for the abilcmd value
            return null;
        }

        //// if no NameId and it is not a passive ability, return null
        //if (string.IsNullOrEmpty(ability.NameId) && ability.IsPassive is not true)
        //    return null;

        // if no NameId and it is a passive ability, set the NameId to the ButtonId
        //if (string.IsNullOrEmpty(ability.NameId) && ability.IsPassive is true)
        //   ability.NameId = ability.ButtonId;

        return ability;
    }

    public Ability? GetAbility(string abilityId)
    {
        Ability ability = new()
        {
            AbilityElementId = abilityId,
        };

        SetAbilityData(ability);

        // default
        ability.Tier = AbilityTier.Hidden;
        ability.AbilityType = AbilityType.Hidden;

        // if not set, set to none
        if (string.IsNullOrEmpty(ability.ButtonElementId))
            ability.ButtonElementId = NoButtonElementId;

        return ability;
    }

    private static bool IgnoreAbilityType(Ability ability) => ability.AbilityType == AbilityType.Attack ||
        ability.AbilityType == AbilityType.Stop ||
        ability.AbilityType == AbilityType.Hold ||
        ability.AbilityType == AbilityType.Cancel ||
        ability.AbilityType == AbilityType.Interact ||
        ability.AbilityType == AbilityType.ForceMove;

    private static bool IgnoreAbilityTier(Ability ability) => ability.Tier == AbilityTier.MapMechanic ||
        ability.Tier == AbilityTier.Interact;

    private static bool IsSlotValueMatch(string slotsValue) =>
        slotsValue.Equals("Hidden1", StringComparison.OrdinalIgnoreCase) ||
        slotsValue.Equals("Hidden2", StringComparison.OrdinalIgnoreCase) ||
        slotsValue.Equals("Hidden3", StringComparison.OrdinalIgnoreCase);

    private static bool IsRequirementsValueMatch(string requirementsValue) =>
        requirementsValue.Equals("UltimateNotUnlocked", StringComparison.OrdinalIgnoreCase);

    private static bool IsTypeValueMatch(string typeValue) =>
        typeValue.Equals("CancelTargetMode", StringComparison.OrdinalIgnoreCase);

    private static void SetAbilityTypeFromSlot(Ability ability, string slot, bool isBehaviorAbility)
    {
        if (string.IsNullOrEmpty(slot))
        {
            if (isBehaviorAbility)
                ability.AbilityType = AbilityType.Active;
            else
                ability.AbilityType = AbilityType.Attack;

            return;
        }

        if (slot.StartsWith("Ability1", StringComparison.OrdinalIgnoreCase))
            ability.AbilityType = AbilityType.Q;
        else if (slot.StartsWith("Ability2", StringComparison.OrdinalIgnoreCase))
            ability.AbilityType = AbilityType.W;
        else if (slot.StartsWith("Ability3", StringComparison.OrdinalIgnoreCase))
            ability.AbilityType = AbilityType.E;
        else if (slot.StartsWith("Mount", StringComparison.OrdinalIgnoreCase))
            ability.AbilityType = AbilityType.Z;
        else if (slot.StartsWith("Heroic", StringComparison.OrdinalIgnoreCase))
            ability.AbilityType = AbilityType.Heroic;
        else if (slot.StartsWith("Hearth", StringComparison.OrdinalIgnoreCase))
            ability.AbilityType = AbilityType.B;
        else if (slot.StartsWith("Trait", StringComparison.OrdinalIgnoreCase))
            ability.AbilityType = AbilityType.Trait;
        else if (Enum.TryParse(slot, true, out AbilityType abilityType))
            ability.AbilityType = abilityType;
        else
            ability.AbilityType = AbilityType.Unknown;
    }

    private static void SetAbilityTierFromAbilityType(Ability ability)
    {
        if (ability.AbilityType == AbilityType.Q || ability.AbilityType == AbilityType.W || ability.AbilityType == AbilityType.E)
            ability.Tier = AbilityTier.Basic;
        else if (ability.AbilityType == AbilityType.Heroic)
            ability.Tier = AbilityTier.Heroic;
        else if (ability.AbilityType == AbilityType.Z)
            ability.Tier = AbilityTier.Mount;
        else if (ability.AbilityType == AbilityType.Trait)
            ability.Tier = AbilityTier.Trait;
        else if (ability.AbilityType == AbilityType.B)
            ability.Tier = AbilityTier.Hearth;
        else if (ability.AbilityType == AbilityType.Active)
            ability.Tier = AbilityTier.Activable;
        else if (ability.AbilityType == AbilityType.Taunt)
            ability.Tier = AbilityTier.Taunt;
        else if (ability.AbilityType == AbilityType.Dance)
            ability.Tier = AbilityTier.Dance;
        else if (ability.AbilityType == AbilityType.Spray)
            ability.Tier = AbilityTier.Spray;
        else if (ability.AbilityType == AbilityType.Voice)
            ability.Tier = AbilityTier.Voice;
        else if (ability.AbilityType == AbilityType.MapMechanic)
            ability.Tier = AbilityTier.MapMechanic;
        else if (ability.AbilityType == AbilityType.Interact)
            ability.Tier = AbilityTier.Interact;
        else if (ability.AbilityType == AbilityType.Attack ||
            ability.AbilityType == AbilityType.Stop ||
            ability.AbilityType == AbilityType.Hold ||
            ability.AbilityType == AbilityType.Cancel ||
            ability.AbilityType == AbilityType.ForceMove)
            ability.Tier = AbilityTier.Action;
        else
            ability.Tier = AbilityTier.Unknown;
    }

    private void SetAbilityData(AbilityTalentBase abilityTalent, string? abilCmdIndex = null)
    {
        StormElement? abilityElement = HeroesData.GetCompleteStormElement("Abil", abilityTalent.AbilityElementId);
        if (abilityElement is null)
            return;

        SetAbilityData(abilityElement, abilityTalent, abilCmdIndex);
    }

    //private void SetTooltipDescriptions(AbilityTalentBase abilityTalent, StormElement buttonElement)
    //{
    //    StormElementData buttonDataValues = buttonElement.DataValues;

    //    if (buttonDataValues.TryGetElementDataAt("Name", out StormElementData? nameData))
    //        abilityTalent.Name = GetTooltipDescriptionFromId(nameData.Value.GetString());

    //    //if (abilityTalent.Name is null)
    //    // TODO: if name is still null, try to get it from the ability element, need to have it passed down

    //    if (buttonDataValues.TryGetElementDataAt("SimpleDisplayText", out StormElementData? simpleDisplayTextData))
    //        abilityTalent.ShortTooltip = GetTooltipDescriptionFromId(simpleDisplayTextData.Value.GetString());
    //    if (buttonDataValues.TryGetElementDataAt("Tooltip", out StormElementData? tooltipData))
    //        abilityTalent.FullTooltip = GetTooltipDescriptionFromId(tooltipData.Value.GetString());

    //}
}
