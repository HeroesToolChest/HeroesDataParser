using Heroes.Element.Models.AbilityTalents;
using Heroes.XmlData;
using Heroes.XmlData.StormData;
using Microsoft.AspNetCore.Mvc.TagHelpers;

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

    public AbilityTalentParserBase(ILogger<AbilityTalentParserBase> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, ITooltipDescriptionService tooltipDescriptionService)
        : base(logger, options, heroesXmlLoaderService, tooltipDescriptionService)
    {
    }

    protected void SetButtonData(AbilityTalentBase abilityTalent)
    {
        StormElement? buttonElement = HeroesData.GetCompleteStormElement("Button", abilityTalent.ButtonElementId);

        if (buttonElement is null)
            return;

        StormElementData buttonDataValues = buttonElement.DataValues;
        //SetTooltipDescriptions(abilityTalent, buttonElement);
        //SetTooltipData(abilityTalent, buttonElement);

        if (buttonDataValues.TryGetElementDataAt("Name", out StormElementData? nameData))
            abilityTalent.Name = TooltipDescriptionService.GetTooltipDescriptionFromId(nameData.Value.GetString());

        if (buttonDataValues.TryGetElementDataAt("SimpleDisplayText", out StormElementData? simpleDisplayTextData))
            abilityTalent.ShortText = TooltipDescriptionService.GetTooltipDescriptionFromId(simpleDisplayTextData.Value.GetString());

        if (buttonDataValues.TryGetElementDataAt("Tooltip", out StormElementData? tooltipData))
            abilityTalent.FullText = TooltipDescriptionService.GetTooltipDescriptionFromId(tooltipData.Value.GetString());

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
                if (tooltipVitalNameData2.TryGetElementDataAt("Energy", out _) && abilityTalent.EnergyCost is not null)
                    abilityTalent.EnergyText = GetTooltipDescriptionFromGameString(value.Replace(GameStringConstants.ReplacementCharacter, abilityTalent.EnergyCost));
                else if (tooltipVitalNameData2.TryGetElementDataAt("Life", out _) && abilityTalent.LifeCost is not null)
                    abilityTalent.LifeText = GetTooltipDescriptionFromGameString(value.Replace(GameStringConstants.ReplacementCharacter, abilityTalent.LifeCost));
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
                            abilityTalent.EnergyText = GetTooltipDescriptionFromGameString(defaultEnergyText.Replace(GameStringConstants.ReplacementCharacter, energyText, StringComparison.OrdinalIgnoreCase));
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
                            abilityTalent.LifeText = GetTooltipDescriptionFromGameString(defaultLifeText.Replace(GameStringConstants.ReplacementCharacter, lifeText, StringComparison.OrdinalIgnoreCase));
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
                        abilityTalent.CooldownText = new TooltipDescription($"{defaultCooldownText}{cooldownText}");
                    else
                        abilityTalent.CooldownText = cooldownTooltip;
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
                abilityTalent.EnergyText = null;
                abilityTalent.LifeText = null;
            }

            if (tooltipFlagsData.TryGetElementDataAt("ShowTime", out StormElementData? showTimeData) && showTimeData.Value.GetInt() == 0)
            {
            }

            if (tooltipFlagsData.TryGetElementDataAt("ShowCooldown", out StormElementData? showCooldownData) && showCooldownData.Value.GetInt() == 0)
            {
                abilityTalent.CooldownText = null;
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
        //if (abilityTalent.EnergyTooltip is not null && abilityTalent.EnergyTooltip.RawDescription.Contains(GameStringConstants.ReplacementCharacter, StringComparison.OrdinalIgnoreCase))
        //    abilityTalent.EnergyTooltip = null;
        //if (abilityTalent.LifeTooltip is not null && abilityTalent.LifeTooltip.RawDescription.Contains(GameStringConstants.ReplacementCharacter, StringComparison.OrdinalIgnoreCase))
        //    abilityTalent.LifeTooltip = null;

        if (buttonDataValues.TryGetElementDataAt("TooltipAppender", out StormElementData? tooltipAppenderArray))
        {
            IEnumerable<string> tooltipAppenders = tooltipAppenderArray.GetElementDataIndexes();

            foreach (string index in tooltipAppenders)
            {
                StormElementData tooltipAppenderData = tooltipAppenderArray.GetElementDataAt(index);

                if (tooltipAppenderData.TryGetElementDataAt("Validator", out StormElementData? validatorData))
                {
                    string validatorValue = validatorData.Value.GetString();

                    // find the validator
                    StormElement? validatorElement = HeroesData.GetCompleteStormElement("Validator", validatorValue);

                    if (validatorElement is null)
                    {
                        Logger.LogWarning("Validator element {ValidatorValue} not found for button {ButtonId}", validatorValue, abilityTalent.ButtonElementId);
                        continue;
                    }

                    if (validatorElement.DataValues.TryGetElementDataAt("Value", out StormElementData? valueData))
                    {
                        // this is the CTalent id
                        string valueValue = valueData.Value.GetString();

                        // check if it exists
                        if (HeroesData.StormElementExists("Talent", valueValue))
                            abilityTalent.TooltipAppendersTalentElementIds.Add(valueValue);
                        else
                            Logger.LogWarning("Talent element {ValueValue} not found for button {ButtonId}", valueValue, abilityTalent.ButtonElementId);
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
            abilityTalent.Name = TooltipDescriptionService.GetTooltipDescriptionFromId(nameData.Value.GetString());

        if (abilityDataValues.TryGetElementDataAt("ProducedUnitArray", out StormElementData? producedUnitArrayData))
        {
            IEnumerable<string> producedUnits = producedUnitArrayData.GetElementDataIndexes();

            foreach (string item in producedUnits)
            {
                string value = producedUnitArrayData.GetElementDataAt(item).Value.GetString();

                AddSummonedUnit(abilityTalent, value);
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

                    if (abilityTalent.Charges is not null && abilityTalent.Charges.CountUse > 0)
                    {
                        abilityTalent.Charges.RecastCooldown = cooldown;
                    }

                    if (abilityTalent.CooldownText is null)
                    {
                        if (cooldown == 1)
                        {
                            StormGameString? abilTooltipCooldown = HeroesData.GetStormGameString(GameStringConstants.AbilTooltipCooldownText);
                            if (abilTooltipCooldown is not null)
                                abilityTalent.CooldownText = GetTooltipDescriptionFromGameString(abilTooltipCooldown.Value.Replace(GameStringConstants.ReplacementCharacter, cooldownString, StringComparison.OrdinalIgnoreCase));
                        }
                        else if (cooldown > 1)
                        {
                            StormGameString? abilTooltipCooldownPlural = HeroesData.GetStormGameString(GameStringConstants.AbilTooltipCooldownPluralText);
                            if (abilTooltipCooldownPlural is not null)
                                abilityTalent.CooldownText = GetTooltipDescriptionFromGameString(abilTooltipCooldownPlural.Value.Replace(GameStringConstants.ReplacementCharacter, cooldownString, StringComparison.OrdinalIgnoreCase));
                        }
                        else if (abilityTalent.Charges is null || (abilityTalent.Charges is not null && !abilityTalent.Charges.HasCharges))
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
                    abilityTalent.EnergyCost = energyData.Value.GetString();
                }
            }
        }
    }

    private void SetCostChargeData(AbilityTalentBase abilityTalent, StormElementData chargeData)
    {
        if (chargeData.TryGetElementDataAt("0", out StormElementData? chargeInnerData))
        {
            abilityTalent.Charges ??= new TooltipCharges();

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
                        StormElement? linkAbilElement = HeroesData.GetCompleteStormElement(linkSplit[0], linkSplit[1]);

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
                abilityTalent.Charges.CountMax = countMaxData.Value.GetInt();

            if (chargeInnerData.TryGetElementDataAt("CountStart", out StormElementData? countStartData))
                abilityTalent.Charges.CountStart = countStartData.Value.GetInt();

            if (chargeInnerData.TryGetElementDataAt("CountUse", out StormElementData? countUseData))
                abilityTalent.Charges.CountUse = countUseData.Value.GetInt();

            if (chargeInnerData.TryGetElementDataAt("HideCount", out StormElementData? hideCountData))
                abilityTalent.Charges.IsCountHidden = hideCountData.Value.GetInt() == 1;

            if (chargeInnerData.TryGetElementDataAt("TimeUse", out StormElementData? timeUseData))
            {
                string? timeUseValue = timeUseData.Value.GetString();

                string? replaceText;
                if (abilityTalent.Charges.CountMax.HasValue && abilityTalent.Charges.CountMax.Value > 1)
                    replaceText = GetStormGameString(GameStringConstants.StringChargeCooldownColon); // Charge Cooldown:<space>
                else
                    replaceText = GetStormGameString(GameStringConstants.StringCooldownColon); // Cooldown:<space>

                if (string.IsNullOrEmpty(replaceText))
                    Logger.LogWarning("{ReplaceText} was not found", replaceText);

                string? cooldownTooltip;
                if (timeUseValue == "1")
                    cooldownTooltip = GetStormGameString(GameStringConstants.AbilTooltipCooldownText);
                else
                    cooldownTooltip = GetStormGameString(GameStringConstants.AbilTooltipCooldownPluralText);

                if (string.IsNullOrEmpty(cooldownTooltip))
                    Logger.LogWarning("{CooldownTooltip} was not found", cooldownTooltip);

                string? cooldownTooltipFinal = cooldownTooltip?
                        .Replace(GetStormGameString(GameStringConstants.StringCooldownColon) ?? string.Empty, replaceText, StringComparison.OrdinalIgnoreCase)
                        .Replace(GameStringConstants.ReplacementCharacter, timeUseValue, StringComparison.OrdinalIgnoreCase);

                if (string.IsNullOrEmpty(cooldownTooltipFinal))
                    Logger.LogWarning("No cooldown tooltip was set");
                else
                    abilityTalent.CooldownText = GetTooltipDescriptionFromGameString(cooldownTooltipFinal);
            }

            if (abilityTalent.Charges.HasCharges is false)
                abilityTalent.Charges = null;
        }
    }

    private void SetEffectData(AbilityTalentBase abilityTalent, StormElementData effectElementData)
    {
        StormElement? stormElement = HeroesData.GetCompleteStormElement("Effect", effectElementData.Value.GetString());

        if (stormElement is null)
            return;

        if (stormElement.DataValues.TryGetElementDataAt("EffectArray", out StormElementData? effectArrayData))
        {
            IEnumerable<string> effectArray = effectArrayData.GetElementDataIndexes();

            foreach (string effectIndex in effectArray)
            {
                ProcessEffectForSpawnUnit(abilityTalent, effectArrayData.GetElementDataAt(effectIndex).Value.GetString());
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("CaseDefault", out StormElementData? caseDefaultData))
            ProcessEffectForSpawnUnit(abilityTalent, caseDefaultData.Value.GetString());
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

    private void ProcessEffectForSpawnUnit(AbilityTalentBase abilityTalent, string id)
    {
        StormElement? stormElement = HeroesData.GetCompleteStormElement("Effect", id);
        if (stormElement is null)
        {
            Logger.LogInformation("Effect element {Id} not found", id);
            return;
        }

        if (stormElement.DataValues.TryGetElementDataAt("SpawnUnit", out StormElementData? spawnUnitData))
        {
            string unitId = spawnUnitData.Value.GetString();

            AddSummonedUnit(abilityTalent, unitId);
        }
    }

    private void AddSummonedUnit(AbilityTalentBase abilityTalent, string unitId)
    {
        StormElement? unitStormElement = HeroesData.GetCompleteStormElement("Unit", unitId);
        if (unitStormElement is null)
        {
            Logger.LogInformation("Unit element {UnitId} not found", unitId);
            return;
        }

        if (unitStormElement.DataValues.TryGetElementDataAt("Attributes", out StormElementData? attributesData))
        {
            if (attributesData.TryGetElementDataAt("Summoned", out StormElementData? summonedData))
            {
                if (summonedData.Value.GetInt() == 1)
                    abilityTalent.SummonedUnitIds.Add(unitId);
            }
        }
    }
}
