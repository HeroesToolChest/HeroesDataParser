using Heroes.Element.Models.AbilityTalents;
using Heroes.Element.Types;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers;

public class AbilityParser : AbilityTalentParserBase, IAbilityParser
{
    private readonly ILogger<AbilityParser> _logger;
    private readonly HeroesData _heroesData;

    public AbilityParser(ILogger<AbilityParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
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
            ButtonId = faceValue,
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
            ability.ButtonId = faceValue;
            ability.NameId = faceValue;
            ability.IsPassive = true;
            ability.IsActive = false;

            SetButtonData(ability);
        }
        else if (!string.IsNullOrEmpty(abilCmdValue)) // non-passive
        {
            (string abilityId, string index) = GetAbilCmdSplit(abilCmdValue);

            ability.NameId = abilityId;

            SetAbilityData(ability, index);
        }
        else
        {
            // doesn't have an abilcmd value set, so this is just a dummy button that doesn't do anything
            // most likely still has an ability set in the ability array but wasn't set for the abilcmd value
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
            NameId = abilityId,
        };

        SetAbilityData(ability);

        // default
        ability.Tier = AbilityTier.Hidden;
        ability.AbilityType = AbilityType.Hidden;

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

    private static bool TryGetCmdButtonArrayData(StormElementData stormElementData, out StormElementData? cmdButtonArrayData)
    {
        return stormElementData.TryGetElementDataAt("CmdButtonArray", out cmdButtonArrayData);
    }
    //private void SetTooltipDescriptions(AbilityTalentBase abilityTalent, StormElement buttonElement)
    //{
    //    StormElementData buttonDataValues = buttonElement.DataValues;

    //    if (buttonDataValues.TryGetElementDataAt("Name", out StormElementData? nameData))
    //        abilityTalent.Name = GetTooltipDescriptionFromId(nameData.Value.GetString());

    //    //if (abilityTalent.Name is null)
    //    // TODO: if name is still null, try to get it from the ability element, need to have it passed down

    //    if (buttonDataValues.TryGetElementDataAt("SimpleDisplayText", out StormElementData? simpleDisplayTextData))
    //        abilityTalent.Tooltip.ShortTooltip = GetTooltipDescriptionFromId(simpleDisplayTextData.Value.GetString());
    //    if (buttonDataValues.TryGetElementDataAt("Tooltip", out StormElementData? tooltipData))
    //        abilityTalent.Tooltip.FullTooltip = GetTooltipDescriptionFromId(tooltipData.Value.GetString());

    //}

    private void SetAbilityData(Ability ability, string? abilCmdIndex = null)
    {
        StormElement? abilityElement = _heroesData.GetCompleteStormElement("Abil", ability.NameId);
        if (abilityElement is null)
            return;

        StormElementData abilityDataValues = abilityElement.DataValues;

        // it's important to have the cost data set first (before the button data) because the tooltips need the cost data
        if (abilityDataValues.TryGetElementDataAt("Cost", out StormElementData? costData))
            SetCostData(ability, costData);

        if (abilityDataValues.TryGetElementDataAt("Effect", out StormElementData? effectData))
            SetEffectData(ability, effectData);

        if (abilityDataValues.TryGetElementDataAt("Name", out StormElementData? nameData))
            ability.Name = GetTooltipDescriptionFromId(nameData.Value.GetString());

        if (abilityDataValues.TryGetElementDataAt("ProducedUnitArray", out StormElementData? producedUnitArrayData))
        {
            foreach (string item in producedUnitArrayData.GetElementDataIndexes())
            {
                string value = producedUnitArrayData.GetElementDataAt(item).Value.GetString();

                if (_heroesData.StormElementExists("Unit", value))
                    ability.CreateUnits.Add(value);
            }
        }

        if (abilityDataValues.TryGetElementDataAt("ParentAbil", out StormElementData? parentAbilData))
        {
            ability.ParentAbililtyId = parentAbilData.Value.GetString();
        }

        if (abilityDataValues.TryGetElementDataAt("Flags", out StormElementData? flagsData))
        {
            // TODO: flags
        }

        // must be done last
        if (abilityDataValues.TryGetElementDataAt("CmdButtonArray", out StormElementData? cmdButtonArrayData))
        {
            if (abilCmdIndex is not null && cmdButtonArrayData.TryGetElementDataAt(abilCmdIndex, out StormElementData? abilCmdData))
            {
                SetCmdButtonArrayData(ability, abilCmdData);
            }
            else if (cmdButtonArrayData.TryGetElementDataAt("Execute", out StormElementData? executeData))
            {
                SetCmdButtonArrayData(ability, executeData);
            }
        }
    }

    private void SetCostData(Ability ability, StormElementData costElementData)
    {
        if (costElementData.TryGetElementDataAt("0", out StormElementData? costInnerData))
        {
            if (costInnerData.TryGetElementDataAt("Charge", out StormElementData? chargeData))
            {
                if (chargeData.TryGetElementDataAt("0", out StormElementData? chargeInnerData) &&
                    (chargeInnerData.ContainsIndex("CountMax") ||
                    chargeInnerData.ContainsIndex("CountStart") ||
                    chargeInnerData.ContainsIndex("CountUse") ||
                    chargeInnerData.ContainsIndex("HideCount") ||
                    chargeInnerData.ContainsIndex("TimeUse")))
                {
                    ability.Tooltip.Charges ??= new TooltipCharges();

                    if (chargeInnerData.TryGetElementDataAt("CountMax", out StormElementData? countMaxData))
                        ability.Tooltip.Charges.CountMax = countMaxData.Value.GetInt();

                    if (chargeInnerData.TryGetElementDataAt("CountStart", out StormElementData? countStartData))
                        ability.Tooltip.Charges.CountStart = countStartData.Value.GetInt();

                    if (chargeInnerData.TryGetElementDataAt("CountUse", out StormElementData? countUseData))
                        ability.Tooltip.Charges.CountUse = countUseData.Value.GetInt();

                    if (chargeInnerData.TryGetElementDataAt("HideCount", out StormElementData? hideCountData))
                        ability.Tooltip.Charges.IsHideCount = hideCountData.Value.GetInt() == 1;

                    if (chargeInnerData.TryGetElementDataAt("TimeUse", out StormElementData? timeUseData))
                    {
                        string? timeUseValue = timeUseData.Value.GetString();

                        string? replaceText;
                        if (ability.Tooltip.Charges.CountMax.HasValue && ability.Tooltip.Charges.CountMax.Value > 1)
                            replaceText = GetStormGameString(GameStringConstants.StringChargeCooldownColon); // Charge Cooldown:<space>
                        else
                            replaceText = GetStormGameString(GameStringConstants.StringCooldownColon); // Cooldown:<space>

                        if (string.IsNullOrEmpty(replaceText))
                            _logger.LogWarning("{ReplaceText} was not found", replaceText);

                        string? cooldownTooltip;
                        if (timeUseValue == "1")
                            cooldownTooltip = GetStormGameString(GameStringConstants.AbilTooltipCooldownText);
                        else
                            cooldownTooltip = GetStormGameString(GameStringConstants.AbilTooltipCooldownPluralText);

                        if (string.IsNullOrEmpty(cooldownTooltip))
                            _logger.LogWarning("{CooldownTooltip} was not found", cooldownTooltip);

                        string? cooldownTooltipFinal = cooldownTooltip?
                               .Replace(GetStormGameString(GameStringConstants.StringCooldownColon) ?? string.Empty, replaceText, StringComparison.OrdinalIgnoreCase)
                               .Replace(GameStringConstants.ReplacementCharacter, timeUseValue, StringComparison.OrdinalIgnoreCase);

                        if (string.IsNullOrEmpty(cooldownTooltipFinal))
                            _logger.LogWarning("No cooldown tooltip was set");
                        else
                            ability.Tooltip.CooldownText = GetTooltipDescriptionFromGameString(cooldownTooltipFinal);
                    }
                }
            }

            if (costInnerData.TryGetElementDataAt("Cooldown", out StormElementData? cooldownData))
            {
                if (cooldownData.TryGetElementDataAt("0", out StormElementData? cooldownInnerData) && cooldownInnerData.TryGetElementDataAt("TimeUse", out StormElementData? timeUseData) && timeUseData.HasValue)
                {
                    string cooldownString = timeUseData.Value.GetString();
                    double cooldown = timeUseData.Value.GetDouble();

                    if (ability.Tooltip.Charges is not null && ability.Tooltip.Charges.HasCharges)
                    {
                        ability.Tooltip.Charges ??= new TooltipCharges();
                        ability.Tooltip.Charges.RecastCooldown = cooldown;
                    }
                    else
                    {
                        if (cooldown == 1)
                        {
                            StormGameString? abilTooltipCooldown = _heroesData.GetStormGameString(GameStringConstants.AbilTooltipCooldownText);
                            if (abilTooltipCooldown is not null)
                                ability.Tooltip.CooldownText = GetTooltipDescriptionFromGameString(abilTooltipCooldown.Value.Replace(GameStringConstants.ReplacementCharacter, cooldownString, StringComparison.OrdinalIgnoreCase));
                        }
                        else if (cooldown > 1)
                        {
                            StormGameString? abilTooltipCooldownPlural = _heroesData.GetStormGameString(GameStringConstants.AbilTooltipCooldownPluralText);
                            if (abilTooltipCooldownPlural is not null)
                                ability.Tooltip.CooldownText = GetTooltipDescriptionFromGameString(abilTooltipCooldownPlural.Value.Replace(GameStringConstants.ReplacementCharacter, cooldownString, StringComparison.OrdinalIgnoreCase));
                        }
                        else
                        {
                            ability.ToggleCooldown = cooldown;
                        }
                    }
                }
            }

            if (costInnerData.TryGetElementDataAt("Vital", out StormElementData? vitalData))
            {
                if (vitalData.TryGetElementDataAt("Energy", out StormElementData? energyData))
                {
                    ability.Tooltip.EnergyCost = energyData.Value.GetString();
                }
            }
        }
    }

    private void SetEffectData(Ability ability, StormElementData effectElementData)
    {
        // TODO: looking up create units
    }

    private void SetCmdButtonArrayData(Ability ability, StormElementData cmdButtonArrayElementData)
    {
        if (cmdButtonArrayElementData.TryGetElementDataAt("DefaultButtonFace", out StormElementData? defaultButtonFaceData))
        {
            string defaultButtonFaceValue = defaultButtonFaceData.Value.GetString();

            if (string.IsNullOrEmpty(ability.ButtonId))
                ability.ButtonId = defaultButtonFaceValue;
        }

        SetButtonData(ability);

        if (cmdButtonArrayElementData.TryGetElementDataAt("Requirements", out StormElementData? requirementsData))
        {
            // TODO: requirements
        }
    }
}
