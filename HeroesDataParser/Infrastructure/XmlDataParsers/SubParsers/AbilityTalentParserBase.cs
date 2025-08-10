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

    /// <summary>
    /// Gets the name of the custom hdp parentAbil attribute or element name.
    /// </summary>
    public const string HdpParentAbilName = "hdp-ParentAbil";

    /// <summary>
    /// Gets the name of the custom hdp parentTalent attribute or element name.
    /// </summary>
    public const string HdpParentTalentName = "hdp-ParentTalent";

    /// <summary>
    /// Gets the name of the custom hdp ability parentLink element name.
    /// </summary>
    public const string HdpParentAbilLinkName = "hdp-ParentAbilLink";

    /// <summary>
    /// Gets the name of the custom hdp talent parentLink element name.
    /// </summary>
    public const string HdpParentTalentLinkName = "hdp-ParentTalentLink";

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
            string? value = TooltipDescriptionService.GetStormGameString(tooltipVitalNameData2.Value.GetString());

            if (!string.IsNullOrEmpty(value))
            {
                if (tooltipVitalNameData2.TryGetElementDataAt("Energy", out _) && abilityTalent.EnergyCost is not null)
                    abilityTalent.EnergyText = TooltipDescriptionService.GetTooltipDescriptionFromGameString(value.Replace(GameStringConstants.ReplacementCharacter, abilityTalent.EnergyCost));
                else if (tooltipVitalNameData2.TryGetElementDataAt("Life", out _) && abilityTalent.LifeCost is not null)
                    abilityTalent.LifeText = TooltipDescriptionService.GetTooltipDescriptionFromGameString(value.Replace(GameStringConstants.ReplacementCharacter, abilityTalent.LifeCost));
            }
        }

        if (buttonDataValues.TryGetElementDataAt("TooltipVitalOverrideText", out StormElementData? tooltipVitalOverrideTextData))
        {
            if (tooltipVitalOverrideTextData.TryGetElementDataAt("Energy", out StormElementData? energyData))
            {
                string? energyText = TooltipDescriptionService.GetStormGameString(energyData.Value.GetString());
                if (!string.IsNullOrEmpty(energyText))
                {
                    // TODO: check if the override text starts with the default energy text
                    //new TooltipDescription(energyData.Value.GetString(), StormLocale.ENUS).PlainText.StartsWith("", StringComparison.OrdinalIgnoreCase);

                    if (buttonDataValues.TryGetElementDataAt("TooltipVitalName", out StormElementData? tooltipVitalNameData) && tooltipVitalNameData.TryGetElementDataAt("Energy", out StormElementData? vitalNameEnergyData))
                    {
                        string? defaultEnergyText = TooltipDescriptionService.GetStormGameString(vitalNameEnergyData.Value.GetString());

                        if (!string.IsNullOrEmpty(defaultEnergyText))
                            abilityTalent.EnergyText = TooltipDescriptionService.GetTooltipDescriptionFromGameString(defaultEnergyText.Replace(GameStringConstants.ReplacementCharacter, energyText, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }

            if (tooltipVitalOverrideTextData.TryGetElementDataAt("Life", out StormElementData? lifeData))
            {
                string? lifeText = TooltipDescriptionService.GetStormGameString(lifeData.Value.GetString());
                if (!string.IsNullOrEmpty(lifeText))
                {
                    // TODO: check if the override text starts with the default life text
                    //new TooltipDescription(energyData.Value.GetString(), StormLocale.ENUS).PlainText.StartsWith("", StringComparison.OrdinalIgnoreCase);

                    if (buttonDataValues.TryGetElementDataAt("TooltipVitalName", out StormElementData? tooltipVitalNameData) && tooltipVitalNameData.TryGetElementDataAt("Life", out StormElementData? vitalNameLifeData))
                    {
                        string? defaultLifeText = TooltipDescriptionService.GetStormGameString(vitalNameLifeData.Value.GetString());

                        if (!string.IsNullOrEmpty(defaultLifeText))
                            abilityTalent.LifeText = TooltipDescriptionService.GetTooltipDescriptionFromGameString(defaultLifeText.Replace(GameStringConstants.ReplacementCharacter, lifeText, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
        }

        if (buttonDataValues.TryGetElementDataAt("TooltipCooldownOverrideText", out StormElementData? tooltipCooldownOverrideTextData))
            SetTooltipCooldownOverrideText(abilityTalent, tooltipCooldownOverrideTextData);

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

                    ProcessValidatorData(abilityTalent, validatorValue);
                }
            }
        }

        if (buttonDataValues.TryGetElementDataAt(HdpParentAbilName, out StormElementData? hdpParentAbilData))
            abilityTalent.ParentAbilityElementId = hdpParentAbilData.Value.GetString();

        if (buttonDataValues.TryGetElementDataAt(HdpParentTalentName, out StormElementData? hdpParentTalentData))
            abilityTalent.ParentTalentElementId = hdpParentTalentData.Value.GetString();

        if (buttonDataValues.TryGetElementDataAt("hdp-IsActive", out StormElementData? hdpIsActiveData))
        {
            if (hdpIsActiveData.Value.GetInt() == 1)
                abilityTalent.IsActive = true;
            else if (hdpIsActiveData.Value.GetInt() == 0)
                abilityTalent.IsActive = false;
        }

        if (buttonDataValues.TryGetElementDataAt(HdpParentAbilLinkName, out StormElementData? hdpParentLinkNameData))
            SetHdpParentAbilLink(abilityTalent, hdpParentLinkNameData);

        if (buttonDataValues.TryGetElementDataAt(HdpParentTalentLinkName, out StormElementData? hdpTalentParentLinkNameData))
            SetHdpParentTalentLink(abilityTalent, hdpTalentParentLinkNameData);
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
        // only when there is no abilCmdIndex set, or if it's On
        if ((string.IsNullOrEmpty(abilCmdIndex) || !abilCmdIndex.Equals("Off", StringComparison.OrdinalIgnoreCase)) && abilityDataValues.TryGetElementDataAt("Cost", out StormElementData? costData))
            SetCostData(abilityTalent, costData);

        // OffCost is the when is ability is manually cancelled
        if (!string.IsNullOrWhiteSpace(abilCmdIndex) && abilCmdIndex.Equals("Off", StringComparison.OrdinalIgnoreCase) && abilityDataValues.TryGetElementDataAt("OffCost", out StormElementData? offCostData))
            SetCostData(abilityTalent, offCostData);

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
            abilityTalent.ParentAbilityElementId = parentAbilData.Value.GetString();

        if (abilityDataValues.TryGetElementDataAt(HdpParentTalentName, out StormElementData? parentTalentData))
             abilityTalent.ParentTalentElementId = parentTalentData.Value.GetString();

        if (abilityDataValues.TryGetElementDataAt(HdpParentAbilLinkName, out StormElementData? hdpParentLinkNameData))
            SetHdpParentAbilLink(abilityTalent, hdpParentLinkNameData);

        if (abilityDataValues.TryGetElementDataAt(HdpParentTalentLinkName, out StormElementData? hdpParentTalentLinkNameData))
            SetHdpParentTalentLink(abilityTalent, hdpParentTalentLinkNameData);

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

    protected void SetHdpParentAbilLink(AbilityTalentBase abilityTalent, StormElementData hdpParentAbilLinkNameData)
    {
        if (hdpParentAbilLinkNameData.TryGetElementDataAt("abilityId", out StormElementData? abilityIdData) &&
            hdpParentAbilLinkNameData.TryGetElementDataAt("buttonId", out StormElementData? buttonIdData) &&
            hdpParentAbilLinkNameData.TryGetElementDataAt("abilityType", out StormElementData? abilityTypeData) && Enum.TryParse(abilityTypeData.Value.GetString(), true, out AbilityType abilityType))
        {
            abilityTalent.ParentAbilityLinkId = new AbilityLinkId(abilityIdData.Value.GetString(), buttonIdData.Value.GetString(), abilityType);
        }
    }

    protected void SetHdpParentTalentLink(AbilityTalentBase abilityTalent, StormElementData hdpParentTalentLinkNameData)
    {
        if (hdpParentTalentLinkNameData.TryGetElementDataAt("talentId", out StormElementData? talentIdData) &&
            hdpParentTalentLinkNameData.TryGetElementDataAt("buttonId", out StormElementData? buttonIdData) &&
            hdpParentTalentLinkNameData.TryGetElementDataAt("abilityType", out StormElementData? abilityTypeData) && Enum.TryParse(abilityTypeData.Value.GetString(), true, out AbilityType abilityType) &&
            hdpParentTalentLinkNameData.TryGetElementDataAt("talentTier", out StormElementData? talentTierData) && Enum.TryParse(talentTierData.Value.GetString(), true, out TalentTier talentTier))
        {
            abilityTalent.ParentTalentLinkId = new TalentLinkId(talentIdData.Value.GetString(), buttonIdData.Value.GetString(), abilityType, talentTier);
        }
    }

    private void SetCostData(AbilityTalentBase abilityTalent, StormElementData costElementData)
    {
        if (costElementData.TryGetElementDataAt("Charge", out StormElementData? chargeData))
            SetCostChargeData(abilityTalent, chargeData);

        if (costElementData.TryGetElementDataAt("Cooldown", out StormElementData? cooldownData))
        {
            if (cooldownData.TryGetElementDataAt("TimeUse", out StormElementData? timeUseData) && timeUseData.HasValue)
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
                            abilityTalent.CooldownText = TooltipDescriptionService.GetTooltipDescriptionFromGameString(abilTooltipCooldown.Value.Replace(GameStringConstants.ReplacementCharacter, cooldownString, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (cooldown > 1)
                    {
                        StormGameString? abilTooltipCooldownPlural = HeroesData.GetStormGameString(GameStringConstants.AbilTooltipCooldownPluralText);
                        if (abilTooltipCooldownPlural is not null)
                            abilityTalent.CooldownText = TooltipDescriptionService.GetTooltipDescriptionFromGameString(abilTooltipCooldownPlural.Value.Replace(GameStringConstants.ReplacementCharacter, cooldownString, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (abilityTalent.Charges is null || (abilityTalent.Charges is not null && !abilityTalent.Charges.HasCharges))
                    {
                        abilityTalent.ToggleCooldown = cooldown;
                    }
                }
            }
        }

        if (costElementData.TryGetElementDataAt("Vital", out StormElementData? vitalData))
        {
            if (vitalData.TryGetElementDataAt("Energy", out StormElementData? energyData))
            {
                abilityTalent.EnergyCost = energyData.Value.GetString();
            }
        }
    }

    private void SetCostChargeData(AbilityTalentBase abilityTalent, StormElementData chargeData)
    {
        abilityTalent.Charges ??= new TooltipCharges();

        // this needs to be done first
        if (chargeData.TryGetElementDataAt("Link", out StormElementData? linkData))
            SetChargeLinkData(abilityTalent, linkData);

        if (chargeData.TryGetElementDataAt("CountMax", out StormElementData? countMaxData))
            abilityTalent.Charges.CountMax = countMaxData.Value.GetInt();

        if (chargeData.TryGetElementDataAt("CountStart", out StormElementData? countStartData))
            abilityTalent.Charges.CountStart = countStartData.Value.GetInt();

        if (chargeData.TryGetElementDataAt("CountUse", out StormElementData? countUseData))
            abilityTalent.Charges.CountUse = countUseData.Value.GetInt();

        if (chargeData.TryGetElementDataAt("HideCount", out StormElementData? hideCountData))
            abilityTalent.Charges.IsCountHidden = hideCountData.Value.GetInt() == 1;

        if (chargeData.TryGetElementDataAt("TimeUse", out StormElementData? timeUseData))
        {
            string? timeUseValue = timeUseData.Value.GetString();

            string? replaceText;
            if (abilityTalent.Charges.CountMax.HasValue && abilityTalent.Charges.CountMax.Value > 1)
                replaceText = TooltipDescriptionService.GetStormGameString(GameStringConstants.StringChargeCooldownColon); // Charge Cooldown:<space>
            else
                replaceText = TooltipDescriptionService.GetStormGameString(GameStringConstants.StringCooldownColon); // Cooldown:<space>

            if (string.IsNullOrEmpty(replaceText))
                Logger.LogWarning("{ReplaceText} was not found", replaceText);

            string? cooldownTooltip;
            if (timeUseValue == "1")
                cooldownTooltip = TooltipDescriptionService.GetStormGameString(GameStringConstants.AbilTooltipCooldownText);
            else
                cooldownTooltip = TooltipDescriptionService.GetStormGameString(GameStringConstants.AbilTooltipCooldownPluralText);

            if (string.IsNullOrEmpty(cooldownTooltip))
                Logger.LogWarning("{CooldownTooltip} was not found", cooldownTooltip);

            string? cooldownTooltipFinal = cooldownTooltip?
                    .Replace(TooltipDescriptionService.GetStormGameString(GameStringConstants.StringCooldownColon) ?? string.Empty, replaceText, StringComparison.OrdinalIgnoreCase)
                    .Replace(GameStringConstants.ReplacementCharacter, timeUseValue, StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(cooldownTooltipFinal))
                Logger.LogWarning("No cooldown tooltip was set");
            else
                abilityTalent.CooldownText = TooltipDescriptionService.GetTooltipDescriptionFromGameString(cooldownTooltipFinal);
        }

        if (abilityTalent.Charges.HasCharges is false)
            abilityTalent.Charges = null;
    }

    private void SetChargeLinkData(AbilityTalentBase abilityTalent, StormElementData linkData)
    {
        string linkValue = linkData.Value.GetString();

        if (!string.IsNullOrEmpty(linkValue))
        {
            Span<Range> linkSplitSpan = stackalloc Range[2];
            linkValue.AsSpan().Split(linkSplitSpan, '/');

            // we need to check to see if the link is not pointing to the current (same) abilityTalent.
            if (linkValue[linkSplitSpan[1]].Equals(abilityTalent.AbilityElementId) is false)
            {
                StormElement? linkAbilElement = HeroesData.GetCompleteStormElement(linkValue[linkSplitSpan[0]], linkValue[linkSplitSpan[1]]);

                if (linkAbilElement is not null &&
                    linkAbilElement.DataValues.TryGetElementDataAt("Cost", out StormElementData? costData) &&
                    costData.TryGetElementDataAt("Charge", out StormElementData? linkChargeData))
                {
                    SetCostChargeData(abilityTalent, linkChargeData);
                }
            }
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
        if (stormElement.DataValues.TryGetElementDataAt("ProducedUnitArray", out StormElementData? producedUnitArrayData))
            ProcessEffectForSpawnUnit(abilityTalent, producedUnitArrayData.Value.GetString());
    }

    private void SetCmdButtonArrayData(AbilityTalentBase abilityTalent, StormElementData cmdButtonArrayElementData)
    {
        if (cmdButtonArrayElementData.TryGetElementDataAt("DefaultButtonFace", out StormElementData? defaultButtonFaceData))
        {
            string defaultButtonFaceValue = defaultButtonFaceData.Value.GetString();

            if (string.IsNullOrEmpty(abilityTalent.ButtonElementId))
                abilityTalent.ButtonElementId = defaultButtonFaceValue;
        }

        if (cmdButtonArrayElementData.TryGetElementDataAt(HdpParentAbilName, out StormElementData? hdpParentAbilNameData))
            abilityTalent.ParentAbilityElementId = hdpParentAbilNameData.Value.GetString();

        if (cmdButtonArrayElementData.TryGetElementDataAt(HdpParentTalentName, out StormElementData? hdpParentTalentNameData))
            abilityTalent.ParentTalentElementId = hdpParentTalentNameData.Value.GetString();

        if (cmdButtonArrayElementData.TryGetElementDataAt(HdpParentAbilLinkName, out StormElementData? hdpParentAbilLinkNameIndexData))
            SetHdpParentAbilLink(abilityTalent, hdpParentAbilLinkNameIndexData);

        if (cmdButtonArrayElementData.TryGetElementDataAt(HdpParentTalentLinkName, out StormElementData? hdpParentTalentLinkNameIndexData))
            SetHdpParentTalentLink(abilityTalent, hdpParentTalentLinkNameIndexData);

        SetButtonData(abilityTalent);

        if (cmdButtonArrayElementData.TryGetElementDataAt("Requirements", out StormElementData? requirementsData))
        {
            StormElement? requirementElement = HeroesData.GetCompleteStormElement("Requirement", requirementsData.Value.GetString());

            if (requirementElement is not null)
            {
                if (requirementElement.DataValues.TryGetElementDataAt("NodeArray", out StormElementData? nodeArrayData))
                {
                    IEnumerable<string> nodeArrayDataIndexes = nodeArrayData.GetElementDataIndexes();

                    foreach (string index in nodeArrayDataIndexes)
                    {
                        if (index.Equals("use", StringComparison.OrdinalIgnoreCase))
                        {
                            StormElementData nodeArray = nodeArrayData.GetElementDataAt(index);
                            if (nodeArray.TryGetElementDataAt("link", out StormElementData? linkData) && linkData.Value.GetString() == "0")
                            {
                                abilityTalent.IsValid = false;
                            }
                        }
                    }
                }
            }
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

    private void ProcessValidatorData(AbilityTalentBase abilityTalent, string validatorId)
    {
        // find the validator
        StormElement? validatorElement = HeroesData.GetCompleteStormElement("Validator", validatorId);

        if (validatorElement is null)
        {
            Logger.LogWarning("Validator element {ValidatorValue} not found for button {ButtonId}", validatorId, abilityTalent.ButtonElementId);
            return;
        }

        if (validatorElement.DataValues.TryGetElementDataAt("Value", out StormElementData? valueData))
        {
            // this is the CTalent id
            string valueValue = valueData.Value.GetString();

            // check if it exists
            if (HeroesData.StormElementExists("Talent", valueValue))
                abilityTalent.TooltipAppendersTalentElementIds.Add(valueValue);
        }
        else if (validatorElement.DataValues.TryGetElementDataAt("CombineArray", out StormElementData? combineArrayData))
        {
            IEnumerable<string> combineArray = combineArrayData.GetElementDataIndexes();

            foreach (string combineIndex in combineArray)
            {
                // another validator id
                string valueValue = combineArrayData.GetElementDataAt(combineIndex).Value.GetString();

                ProcessValidatorData(abilityTalent, valueValue);
            }
        }
    }

    private void SetTooltipCooldownOverrideText(AbilityTalentBase abilityTalent, StormElementData tooltipCooldownOverrideTextData)
    {
        if (abilityTalent is Talent && (string.IsNullOrEmpty(abilityTalent.AbilityElementId) || abilityTalent.AbilityElementId == PassiveAbilityElementId))
            return; // if a talent has no ability element id or is passive, then we don't set the cooldown text

        string? cooldownText = TooltipDescriptionService.GetStormGameString(tooltipCooldownOverrideTextData.Value.GetString());
        if (!string.IsNullOrEmpty(cooldownText))
        {
            string? defaultCooldownText = TooltipDescriptionService.GetStormGameString(GameStringConstants.StringCooldownColon);

            if (!string.IsNullOrEmpty(defaultCooldownText))
            {
                TooltipDescription cooldownTooltip = TooltipDescriptionService.GetTooltipDescription(cooldownText);

                if (!cooldownTooltip.PlainText.StartsWith(defaultCooldownText, StringComparison.OrdinalIgnoreCase))
                    abilityTalent.CooldownText = TooltipDescriptionService.GetTooltipDescriptionFromGameString($"{defaultCooldownText}{cooldownText}");
                else
                    abilityTalent.CooldownText = cooldownTooltip;
            }
        }
    }
}
