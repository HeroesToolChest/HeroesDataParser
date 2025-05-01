using Heroes.Element.Models.AbilityTalents;
using Heroes.XmlData;
using Heroes.XmlData.StormData;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers;

public class AbilityTalentParserBase : ParserBase
{
    /// <summary>
    /// Gets the value for an ability that has no ability element id.
    /// </summary>
    public const string PassiveAbilityElementId = ":PASSIVE:";

    /// <summary>
    /// Gets the value for a button that has no button element id.
    /// </summary>
    public const string NoButtonElementId = ":NONE:";

    private readonly ILogger<AbilityTalentParserBase> _logger;
    private readonly HeroesData _heroesData;

    public AbilityTalentParserBase(ILogger<AbilityTalentParserBase> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
    }

    protected void SetButtonData(AbilityTalentBase abilityTalent)
    {
        StormElement? buttonElement = _heroesData.GetCompleteStormElement("Button", abilityTalent.ButtonElementId);

        if (buttonElement is null)
            return;

        StormElementData buttonDataValues = buttonElement.DataValues;
        //SetTooltipDescriptions(abilityTalent, buttonElement);
        //SetTooltipData(abilityTalent, buttonElement);

        if (buttonDataValues.TryGetElementDataAt("Name", out StormElementData? nameData))
            abilityTalent.Name = GetTooltipDescriptionFromId(nameData.Value.GetString());

        if (buttonDataValues.TryGetElementDataAt("SimpleDisplayText", out StormElementData? simpleDisplayTextData))
            abilityTalent.Tooltip.ShortText = GetTooltipDescriptionFromId(simpleDisplayTextData.Value.GetString());

        if (buttonDataValues.TryGetElementDataAt("Tooltip", out StormElementData? tooltipData))
            abilityTalent.Tooltip.FullText = GetTooltipDescriptionFromId(tooltipData.Value.GetString());

        if (buttonDataValues.TryGetElementDataAt("Icon", out StormElementData? iconData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(iconData);
            if (imageFilePath is not null)
            {
                abilityTalent.Icon = imageFilePath.Image;
                abilityTalent.IconPath = imageFilePath.FilePath;
            }
        }

        // must be done before the TooltipVital overrides
        if (buttonDataValues.TryGetElementDataAt("TooltipVitalName", out StormElementData? tooltipVitalNameData2))
        {
            string? value = GetStormGameString(tooltipVitalNameData2.Value.GetString());

            if (!string.IsNullOrEmpty(value))
            {
                if (tooltipVitalNameData2.TryGetElementDataAt("Energy", out _) && abilityTalent.Tooltip.EnergyCost is not null)
                    abilityTalent.Tooltip.EnergyText = GetTooltipDescriptionFromGameString(value.Replace(GameStringConstants.ReplacementCharacter, abilityTalent.Tooltip.EnergyCost));
                else if (tooltipVitalNameData2.TryGetElementDataAt("Life", out _) && abilityTalent.Tooltip.LifeCost is not null)
                    abilityTalent.Tooltip.LifeText = GetTooltipDescriptionFromGameString(value.Replace(GameStringConstants.ReplacementCharacter, abilityTalent.Tooltip.LifeCost));
            }
        }

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
                            abilityTalent.Tooltip.EnergyText = GetTooltipDescriptionFromGameString(defaultEnergyText.Replace(GameStringConstants.ReplacementCharacter, energyText, StringComparison.OrdinalIgnoreCase));
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
                            abilityTalent.Tooltip.LifeText = GetTooltipDescriptionFromGameString(defaultLifeText.Replace(GameStringConstants.ReplacementCharacter, lifeText, StringComparison.OrdinalIgnoreCase));
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
                        abilityTalent.Tooltip.CooldownText = new TooltipDescription($"{defaultCooldownText}{cooldownText}");
                    else
                        abilityTalent.Tooltip.CooldownText = cooldownTooltip;
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
                abilityTalent.Tooltip.EnergyText = null;
                abilityTalent.Tooltip.LifeText = null;
            }

            if (tooltipFlagsData.TryGetElementDataAt("ShowTime", out StormElementData? showTimeData) && showTimeData.Value.GetInt() == 0)
            {
            }

            if (tooltipFlagsData.TryGetElementDataAt("ShowCooldown", out StormElementData? showCooldownData) && showCooldownData.Value.GetInt() == 0)
            {
                abilityTalent.Tooltip.CooldownText = null;
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

        if (buttonDataValues.TryGetElementDataAt("TooltipAppender", out StormElementData? tooltipAppenderArray))
        {
            foreach (string index in tooltipAppenderArray.GetElementDataIndexes())
            {
                StormElementData tooltipAppenderData = tooltipAppenderArray.GetElementDataAt(index);

                if (tooltipAppenderData.TryGetElementDataAt("Validator", out StormElementData? validatorData))
                {
                    string validatorValue = validatorData.Value.GetString();

                    // find the validator
                    StormElement? validatorElement = _heroesData.GetCompleteStormElement("Validator", validatorValue);

                    if (validatorElement is null)
                    {
                        _logger.LogWarning("Validator element {ValidatorValue} not found for button {ButtonId}", validatorValue, abilityTalent.ButtonElementId);
                        continue;
                    }

                    if (validatorElement.DataValues.TryGetElementDataAt("Value", out StormElementData? valueData))
                    {
                        // this is the CTalent id
                        string valueValue = valueData.Value.GetString();

                        // check if it exists
                        if (_heroesData.StormElementExists("Talent", valueValue))
                            abilityTalent.TooltipAppendersTalentElementIds.Add(valueValue);
                        else
                            _logger.LogWarning("Talent element {ValueValue} not found for button {ButtonId}", valueValue, abilityTalent.ButtonElementId);
                    }
                }
            }
        }
    }

    //protected void SetAbilityData(AbilityTalentBase abilityTalent, string? abilCmdIndex = null)
    //{
    //    StormElement? abilityElement = _heroesData.GetCompleteStormElement("Abil", abilityTalent.NameId);
    //    if (abilityElement is null)
    //        return;

    //    SetAbilityData(abilityElement, abilityTalent, abilCmdIndex);
    //}

    //protected void SetAbilityData(string abilityId, AbilityTalentBase abilityTalent, string? abilCmdIndex = null)
    //{
    //    StormElement? abilityElement = _heroesData.GetCompleteStormElement("Abil", abilityId);
    //    if (abilityElement is null)
    //        return;

    //    SetAbilityData(abilityElement, abilityTalent, abilCmdIndex);
    //}

    protected void SetAbilityData(StormElement abilityElement, AbilityTalentBase abilityTalent, string? abilCmdIndex)
    {
        StormElementData abilityDataValues = abilityElement.DataValues;

        // it's important to have the cost data set first (before the button data) because the tooltips need the cost data
        if (abilityDataValues.TryGetElementDataAt("Cost", out StormElementData? costData))
            SetCostData(abilityTalent, costData);

        if (abilityDataValues.TryGetElementDataAt("Effect", out StormElementData? effectData))
            SetEffectData(abilityTalent, effectData);

        if (abilityDataValues.TryGetElementDataAt("Name", out StormElementData? nameData))
            abilityTalent.Name = GetTooltipDescriptionFromId(nameData.Value.GetString());

        if (abilityDataValues.TryGetElementDataAt("ProducedUnitArray", out StormElementData? producedUnitArrayData))
        {
            foreach (string item in producedUnitArrayData.GetElementDataIndexes())
            {
                string value = producedUnitArrayData.GetElementDataAt(item).Value.GetString();

                if (_heroesData.StormElementExists("Unit", value))
                    abilityTalent.CreatedUnits.Add(value);
            }
        }

        if (abilityDataValues.TryGetElementDataAt("ParentAbil", out StormElementData? parentAbilData))
        {
            abilityTalent.ParentAbilityElementId = parentAbilData.Value.GetString();
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
                SetCmdButtonArrayData(abilityTalent, abilCmdData);
            }
            else if (cmdButtonArrayData.TryGetElementDataAt("Execute", out StormElementData? executeData))
            {
                SetCmdButtonArrayData(abilityTalent, executeData);
            }
        }
    }

    private void SetCostData(AbilityTalentBase abilityTalent, StormElementData costElementData)
    {
        if (costElementData.TryGetElementDataAt("0", out StormElementData? costInnerData))
        {
            if (costInnerData.TryGetElementDataAt("Charge", out StormElementData? chargeData))
                SetCostChargeData(abilityTalent, chargeData);

            if (costInnerData.TryGetElementDataAt("Cooldown", out StormElementData? cooldownData))
            {
                if (cooldownData.TryGetElementDataAt("0", out StormElementData? cooldownInnerData) && cooldownInnerData.TryGetElementDataAt("TimeUse", out StormElementData? timeUseData) && timeUseData.HasValue)
                {
                    string cooldownString = timeUseData.Value.GetString();
                    double cooldown = timeUseData.Value.GetDouble();

                    if (abilityTalent.Tooltip.Charges is not null && abilityTalent.Tooltip.Charges.CountUse > 0)
                    {
                        abilityTalent.Tooltip.Charges.RecastCooldown = cooldown;
                    }

                    if (abilityTalent.Tooltip.CooldownText is null)
                    {
                        if (cooldown == 1)
                        {
                            StormGameString? abilTooltipCooldown = _heroesData.GetStormGameString(GameStringConstants.AbilTooltipCooldownText);
                            if (abilTooltipCooldown is not null)
                                abilityTalent.Tooltip.CooldownText = GetTooltipDescriptionFromGameString(abilTooltipCooldown.Value.Replace(GameStringConstants.ReplacementCharacter, cooldownString, StringComparison.OrdinalIgnoreCase));
                        }
                        else if (cooldown > 1)
                        {
                            StormGameString? abilTooltipCooldownPlural = _heroesData.GetStormGameString(GameStringConstants.AbilTooltipCooldownPluralText);
                            if (abilTooltipCooldownPlural is not null)
                                abilityTalent.Tooltip.CooldownText = GetTooltipDescriptionFromGameString(abilTooltipCooldownPlural.Value.Replace(GameStringConstants.ReplacementCharacter, cooldownString, StringComparison.OrdinalIgnoreCase));
                        }
                        else if (abilityTalent.Tooltip.Charges is null || (abilityTalent.Tooltip.Charges is not null && !abilityTalent.Tooltip.Charges.HasCharges))
                        {
                            abilityTalent.ToggleCooldown = cooldown;
                        }
                    }
                }
            }

            if (costInnerData.TryGetElementDataAt("Vital", out StormElementData? vitalData))
            {
                if (vitalData.TryGetElementDataAt("Energy", out StormElementData? energyData))
                {
                    abilityTalent.Tooltip.EnergyCost = energyData.Value.GetString();
                }
            }
        }
    }

    private void SetCostChargeData(AbilityTalentBase abilityTalent, StormElementData chargeData)
    {
        if (chargeData.TryGetElementDataAt("0", out StormElementData? chargeInnerData))
        {
            abilityTalent.Tooltip.Charges ??= new TooltipCharges();

            // this needs to be done first
            if (chargeInnerData.TryGetElementDataAt("Link", out StormElementData? linkData))
            {
                string linkValue = linkData.Value.GetString();

                if (!string.IsNullOrEmpty(linkValue))
                {
                    string[] linkSplit = linkValue.Split('/', 2);

                    // we need to check to see if the link is not pointing to the current (same) abilityTalent.
                    if (linkSplit[1].Equals(abilityTalent.AbilityElementId) is false)
                    {
                        StormElement? linkAbilElement = _heroesData.GetCompleteStormElement(linkSplit[0], linkSplit[1]);

                        if (linkAbilElement is not null &&
                            linkAbilElement.DataValues.TryGetElementDataAt("Cost", out StormElementData? costData) &&
                            costData.TryGetElementDataAt("0", out StormElementData? costInnerData) &&
                            costInnerData.TryGetElementDataAt("Charge", out StormElementData? linkChargeData))
                        {
                            SetCostChargeData(abilityTalent, linkChargeData);
                        }
                    }
                }
            }

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
                    abilityTalent.Tooltip.CooldownText = GetTooltipDescriptionFromGameString(cooldownTooltipFinal);
            }
        }
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

            if (string.IsNullOrEmpty(abilityTalent.ButtonElementId))
                abilityTalent.ButtonElementId = defaultButtonFaceValue;
        }

        SetButtonData(abilityTalent);

        if (cmdButtonArrayElementData.TryGetElementDataAt("Requirements", out StormElementData? requirementsData))
        {
            // TODO: requirements
        }
    }
}
