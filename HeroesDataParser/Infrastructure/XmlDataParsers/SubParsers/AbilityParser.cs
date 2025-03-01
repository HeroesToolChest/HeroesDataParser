using Heroes.Element.Models.AbilityTalents;
using Heroes.Element.Types;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers;

public class AbilityParser : ParserBase, IAbilityParser
{
    private readonly ILogger<AbilityParser> _logger;
    private readonly HeroesData _heroesData;

    public AbilityParser(ILogger<AbilityParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
    }

    public Ability? GetAbility(string unitId, StormElementData layoutButtonData)
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

        // passive "ability", actually just a dummy button
        if (typeValue.AsSpan().Equals("Passive", StringComparison.OrdinalIgnoreCase))
        {
            ability.ButtonId = faceValue;
            ability.IsPassive = true;
            ability.IsActive = false;

            SetButtonData(ability);
        }
        else if (!string.IsNullOrEmpty(abilCmdValue)) // non-passive
        {
            (string abilityId, string index) = GetAbilCmdSplit(abilCmdValue);

            ability.NameId = abilityId;

            SetAbilityTalentData(ability, index);
        }
        else
        {
            // doesn't have an abilcmd value set, so this is just a dummy button that doesn't do anything
            // most likely still has an ability set in the ability array but wasn't set for the abilcmd value
            return null;
        }

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

        // if no NameId and it is not a passive ability, return null
        if (string.IsNullOrEmpty(ability.NameId) && ability.IsPassive is not true)
            return null;

        // if no NameId and it is a passive ability, set the NameId to the ButtonId
        if (string.IsNullOrEmpty(ability.NameId) && ability.IsPassive is true)
            ability.NameId = ability.ButtonId;

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

    private static (string AbilityId, string Index) GetAbilCmdSplit(string abilCmdValue)
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

    private static void SetAbilityTypeFromSlot(AbilityTalentBase abilityTalent, string slot, bool isBehaviorAbility)
    {
        if (string.IsNullOrEmpty(slot))
        {
            if (isBehaviorAbility)
                abilityTalent.AbilityType = AbilityType.Active;
            else
                abilityTalent.AbilityType = AbilityType.Attack;

            return;
        }

        if (slot.StartsWith("Ability1", StringComparison.OrdinalIgnoreCase))
            abilityTalent.AbilityType = AbilityType.Q;
        else if (slot.StartsWith("Ability2", StringComparison.OrdinalIgnoreCase))
            abilityTalent.AbilityType = AbilityType.W;
        else if (slot.StartsWith("Ability3", StringComparison.OrdinalIgnoreCase))
            abilityTalent.AbilityType = AbilityType.E;
        else if (slot.StartsWith("Mount", StringComparison.OrdinalIgnoreCase))
            abilityTalent.AbilityType = AbilityType.Z;
        else if (slot.StartsWith("Heroic", StringComparison.OrdinalIgnoreCase))
            abilityTalent.AbilityType = AbilityType.Heroic;
        else if (slot.StartsWith("Hearth", StringComparison.OrdinalIgnoreCase))
            abilityTalent.AbilityType = AbilityType.B;
        else if (slot.StartsWith("Trait", StringComparison.OrdinalIgnoreCase))
            abilityTalent.AbilityType = AbilityType.Trait;
        else if (Enum.TryParse(slot, true, out AbilityType abilityType))
            abilityTalent.AbilityType = abilityType;
        else
            abilityTalent.AbilityType = AbilityType.Unknown;
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

    private void SetButtonData(AbilityTalentBase abilityTalent)
    {
        StormElement? buttonElement = _heroesData.GetCompleteStormElement("Button", abilityTalent.ButtonId);

        if (buttonElement is null)
            return;

        StormElementData buttonDataValues = buttonElement.DataValues;
        //SetTooltipDescriptions(abilityTalent, buttonElement);
        //SetTooltipData(abilityTalent, buttonElement);

        if (buttonDataValues.TryGetElementDataAt("Name", out StormElementData? nameData))
            abilityTalent.Name = GetTooltipDescriptionFromId(nameData.Value.GetString());

        if (buttonDataValues.TryGetElementDataAt("SimpleDisplayText", out StormElementData? simpleDisplayTextData))
            abilityTalent.Tooltip.ShortTooltip = GetTooltipDescriptionFromId(simpleDisplayTextData.Value.GetString());

        if (buttonDataValues.TryGetElementDataAt("Tooltip", out StormElementData? tooltipData))
            abilityTalent.Tooltip.FullTooltip = GetTooltipDescriptionFromId(tooltipData.Value.GetString());

        if (buttonDataValues.TryGetElementDataAt("Icon", out StormElementData? iconData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(iconData);
            if (imageFilePath is not null)
            {
                abilityTalent.Icon = imageFilePath.Image;
                abilityTalent.IconPath = imageFilePath.FilePath;
            }
        }

        // TODO: is needed?
        //if (buttonDataValues.TryGetElementDataAt("TooltipVitalName", out StormElementData? tooltipVitalNameData))
        //{
        //    string value = tooltipVitalNameData.Value.GetString();

        //    if (tooltipVitalNameData.TryGetElementDataAt("Energy", out _))
        //    {
        //        //abilityTalent.Tooltip.EnergyTooltip = GetTooltipDescriptionFromId(value);
        //        //value.Replace(GameStringConstants.ReplacementCharacter, cooldownString)
        //    }
        //    else if (tooltipVitalNameData.TryGetElementDataAt("Life", out _))
        //    {

        //    }
        //    else
        //    {
        //        _logger.LogWarning("Unknown vital name: {VitalName}", value);
        //    }
        //}

        if (buttonDataValues.TryGetElementDataAt("TooltipVitalOverrideText", out StormElementData? tooltipVitalOverrideTextData))
        {
            if (tooltipVitalOverrideTextData.TryGetElementDataAt("Energy", out StormElementData? energyData))
            {
                string? energyText = GetStormGameString(energyData.Value.GetString());
                if (!string.IsNullOrEmpty(energyText))
                {
                    // TODO: check if the override text starts with the default energy text
                    //new TooltipDescription(energyData.Value.GetString(), StormLocale.ENUS).PlainText.StartsWith("", StringComparison.OrdinalIgnoreCase);

                    if (buttonDataValues.TryGetElementDataAt("TooltipVitalName", out StormElementData? tooltipVitalNameData) && tooltipVitalNameData.TryGetElementDataAt("Energy", out StormElementData? vitalNameEnergyData))
                    {
                        string? defaultEnergyText = GetStormGameString(vitalNameEnergyData.Value.GetString());

                        if (!string.IsNullOrEmpty(defaultEnergyText))
                            abilityTalent.Tooltip.EnergyTooltip = GetTooltipDescriptionFromGameString(defaultEnergyText.Replace(GameStringConstants.ReplacementCharacter, energyText, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }

            if (tooltipVitalOverrideTextData.TryGetElementDataAt("Life", out StormElementData? lifeData))
            {
                string? lifeText = GetStormGameString(lifeData.Value.GetString());
                if (!string.IsNullOrEmpty(lifeText))
                {
                    // TODO: check if the override text starts with the default life text
                    //new TooltipDescription(energyData.Value.GetString(), StormLocale.ENUS).PlainText.StartsWith("", StringComparison.OrdinalIgnoreCase);

                    if (buttonDataValues.TryGetElementDataAt("TooltipVitalName", out StormElementData? tooltipVitalNameData) && tooltipVitalNameData.TryGetElementDataAt("Life", out StormElementData? vitalNameLifeData))
                    {
                        string? defaultLifeText = GetStormGameString(vitalNameLifeData.Value.GetString());

                        if (!string.IsNullOrEmpty(defaultLifeText))
                            abilityTalent.Tooltip.LifeTooltip = GetTooltipDescriptionFromGameString(defaultLifeText.Replace(GameStringConstants.ReplacementCharacter, lifeText, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
        }

        if (buttonDataValues.TryGetElementDataAt("TooltipCooldownOverrideText", out StormElementData? tooltipCooldownOverrideTextData))
        {
            string? cooldownText = GetStormGameString(tooltipCooldownOverrideTextData.Value.GetString());
            if (!string.IsNullOrEmpty(cooldownText))
            {
                string? defaultCooldownText = GetStormGameString(GameStringConstants.StringCooldownColon);

                if (!string.IsNullOrEmpty(defaultCooldownText))
                {
                    TooltipDescription cooldownTooltip = new(cooldownText, StormLocale.ENUS);

                    if (!cooldownTooltip.PlainText.StartsWith(defaultCooldownText, StringComparison.OrdinalIgnoreCase))
                        abilityTalent.Tooltip.CooldownTooltip = new TooltipDescription($"{defaultCooldownText}{cooldownText}");
                    else
                        abilityTalent.Tooltip.CooldownTooltip = cooldownTooltip;
                }
            }
        }

        if (buttonDataValues.TryGetElementDataAt("TooltipFlags", out StormElementData? tooltipFlagsData))
        {
            if (tooltipFlagsData.TryGetElementDataAt("ShowName", out StormElementData? showNameData) && showNameData.Value.GetInt() == 0)
            {
                abilityTalent.Name = null;
            }

            if (tooltipFlagsData.TryGetElementDataAt("ShowHotkey", out StormElementData? showHotkeyData) && showHotkeyData.Value.GetInt() == 0)
            {
            }

            if (tooltipFlagsData.TryGetElementDataAt("ShowUsage", out StormElementData? showUsageData) && showUsageData.Value.GetInt() == 0)
            {
                abilityTalent.Tooltip.EnergyTooltip = null;
                abilityTalent.Tooltip.LifeTooltip = null;
            }

            if (tooltipFlagsData.TryGetElementDataAt("ShowTime", out StormElementData? showTimeData) && showTimeData.Value.GetInt() == 0)
            {
            }

            if (tooltipFlagsData.TryGetElementDataAt("ShowCooldown", out StormElementData? showCooldownData) && showCooldownData.Value.GetInt() == 0)
            {
                // ignore, always show the cooldown
            }

            if (tooltipFlagsData.TryGetElementDataAt("ShowRequirements", out StormElementData? showRequirementsData) && showRequirementsData.Value.GetInt() == 0)
            {
            }

            if (tooltipFlagsData.TryGetElementDataAt("ShowAutocast", out StormElementData? showAutocastData) && showAutocastData.Value.GetInt() == 0)
            {
            }
        }

        if (buttonDataValues.TryGetElementDataAt("UseHotkeyLabel", out StormElementData? useHotkeyLabelData))
        {
            if (useHotkeyLabelData.Value.GetInt() == 0)
                abilityTalent.IsActive = false; // ability has no hotkey, not activable
        }

        // validate, check if the tooltip for energy and life still contain the replacement character, if so then then set them to null
        //if (abilityTalent.Tooltip.EnergyTooltip is not null && abilityTalent.Tooltip.EnergyTooltip.RawDescription.Contains(GameStringConstants.ReplacementCharacter, StringComparison.OrdinalIgnoreCase))
        //    abilityTalent.Tooltip.EnergyTooltip = null;
        //if (abilityTalent.Tooltip.LifeTooltip is not null && abilityTalent.Tooltip.LifeTooltip.RawDescription.Contains(GameStringConstants.ReplacementCharacter, StringComparison.OrdinalIgnoreCase))
        //    abilityTalent.Tooltip.LifeTooltip = null;
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

    private void SetAbilityTalentData(AbilityTalentBase abilityTalent, string abilCmdIndex)
    {
        StormElement? abilityElement = _heroesData.GetCompleteStormElement("Abil", abilityTalent.NameId);
        if (abilityElement is null)
            return;

        StormElementData abilityDataValues = abilityElement.DataValues;

        if (abilityDataValues.TryGetElementDataAt("Cost", out StormElementData? costData))
            SetCostData(abilityTalent, costData);

        if (abilityDataValues.TryGetElementDataAt("Effect", out StormElementData? effectData))
            SetEffectData(abilityTalent, effectData);

        if (abilityDataValues.TryGetElementDataAt("Name", out StormElementData? nameData))
        {
            // TODO: name
        }

        if (abilityDataValues.TryGetElementDataAt("ProducedUnitArray", out StormElementData? producedUnitArrayData))
        {
            foreach (string item in producedUnitArrayData.GetElementDataIndexes())
            {
                abilityTalent.CreateUnits.Add(producedUnitArrayData.GetElementDataAt(item).Value.GetString());
            }
        }

        if (abilityDataValues.TryGetElementDataAt("ParentAbil", out StormElementData? parentAbilData))
        {
            // TODO: parent abil
        }

        if (abilityDataValues.TryGetElementDataAt("Flags", out StormElementData? flagsData))
        {
            // TODO: flags
        }

        // must be done last
        if (abilityDataValues.TryGetElementDataAt("CmdButtonArray", out StormElementData? cmdButtonArrayData))
        {
            if (cmdButtonArrayData.TryGetElementDataAt(abilCmdIndex, out StormElementData? abilCmdData))
            {
                SetCmdButtonArrayData(abilityTalent, abilCmdData);
            }
            else if (cmdButtonArrayData.TryGetElementDataAt("Execute", out StormElementData? executeData))
            {
                SetCmdButtonArrayData(abilityTalent, executeData);
            }

                //foreach (string index in cmdButtonArrayData.GetElementDataIndexes())
                //{
                //    // check if the index is the same as the abilCmdIndex or it's "Execute"
                //    // cancel is also an available index, but it doesn't seem to be used in HOTS
                //    if (index.Equals(abilCmdIndex, StringComparison.OrdinalIgnoreCase) || index.Equals("Execute", StringComparison.OrdinalIgnoreCase))
                //    {
                //        StormElementData arrayElement = cmdButtonArrayData.GetElementDataAt(index);

                //        SetCmdButtonArrayData(abilityTalent, arrayElement);
                //    }
                //}
        }
    }

    private void SetCostData(AbilityTalentBase abilityTalent, StormElementData costElementData)
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
                    abilityTalent.Tooltip.Charges ??= new TooltipCharges();

                    if (chargeInnerData.TryGetElementDataAt("CountMax", out StormElementData? countMaxData))
                        abilityTalent.Tooltip.Charges.CountMax = countMaxData.Value.GetInt();

                    if (chargeInnerData.TryGetElementDataAt("CountStart", out StormElementData? countStartData))
                        abilityTalent.Tooltip.Charges.CountStart = countStartData.Value.GetInt();

                    if (chargeInnerData.TryGetElementDataAt("CountUse", out StormElementData? countUseData))
                        abilityTalent.Tooltip.Charges.CountUse = countUseData.Value.GetInt();

                    if (chargeInnerData.TryGetElementDataAt("HideCount", out StormElementData? hideCountData))
                        abilityTalent.Tooltip.Charges.IsHideCount = hideCountData.Value.GetInt() == 1;

                    if (chargeInnerData.TryGetElementDataAt("TimeUse", out StormElementData? timeUseData))
                    {
                        string? timeUseValue = timeUseData.Value.GetString();

                        string? replaceText;
                        if (abilityTalent.Tooltip.Charges.CountMax.HasValue && abilityTalent.Tooltip.Charges.CountMax.Value > 1)
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
                            abilityTalent.Tooltip.CooldownTooltip = GetTooltipDescriptionFromGameString(cooldownTooltipFinal);
                    }
                }
            }

            if (costInnerData.TryGetElementDataAt("Cooldown", out StormElementData? cooldownData))
            {
                if (cooldownData.TryGetElementDataAt("0", out StormElementData? cooldownInnerData) && cooldownInnerData.TryGetElementDataAt("TimeUse", out StormElementData? timeUseData) && timeUseData.HasValue)
                {
                    string cooldownString = timeUseData.Value.GetString();
                    double cooldown = timeUseData.Value.GetDouble();

                    if (abilityTalent.Tooltip.Charges is not null && abilityTalent.Tooltip.Charges.HasCharges)
                    {
                        abilityTalent.Tooltip.Charges ??= new TooltipCharges();
                        abilityTalent.Tooltip.Charges.RecastCooldown = cooldown;
                    }
                    else
                    {
                        if (cooldown == 1)
                        {
                            StormGameString? abilTooltipCooldown = _heroesData.GetStormGameString(GameStringConstants.AbilTooltipCooldownText);
                            if (abilTooltipCooldown is not null)
                                abilityTalent.Tooltip.CooldownTooltip = GetTooltipDescriptionFromGameString(abilTooltipCooldown.Value.Replace(GameStringConstants.ReplacementCharacter, cooldownString, StringComparison.OrdinalIgnoreCase));
                        }
                        else if (cooldown > 1)
                        {
                            StormGameString? abilTooltipCooldownPlural = _heroesData.GetStormGameString(GameStringConstants.AbilTooltipCooldownPluralText);
                            if (abilTooltipCooldownPlural is not null)
                                abilityTalent.Tooltip.CooldownTooltip = GetTooltipDescriptionFromGameString(abilTooltipCooldownPlural.Value.Replace(GameStringConstants.ReplacementCharacter, cooldownString, StringComparison.OrdinalIgnoreCase));
                        }
                        else
                        {
                            abilityTalent.ToggleCooldown = cooldown;
                        }
                    }
                }
            }
        }

        // TODO: vitals
    }

    private void SetEffectData(AbilityTalentBase abilityTalent, StormElementData effectElementData)
    {
        // TODO: looking up create units
    }

    private void SetCmdButtonArrayData(AbilityTalentBase abilityTalent, StormElementData cmdButtonArrayElementData)
    {
        if (cmdButtonArrayElementData.TryGetElementDataAt("DefaultButtonFace", out StormElementData? defaultButtonFaceData))
        {
            string defaultButtonFaceValue = defaultButtonFaceData.Value.GetString();

            if (string.IsNullOrEmpty(abilityTalent.ButtonId))
                abilityTalent.ButtonId = defaultButtonFaceValue;
        }

        SetButtonData(abilityTalent);

        if (cmdButtonArrayElementData.TryGetElementDataAt("Requirements", out StormElementData? requirementsData))
        {
            // TODO: requirements
        }
    }
}
