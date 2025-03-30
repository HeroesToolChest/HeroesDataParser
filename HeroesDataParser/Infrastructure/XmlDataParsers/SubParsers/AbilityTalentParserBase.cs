using Heroes.Element.Models.AbilityTalents;
using Heroes.XmlData;
using Heroes.XmlData.StormData;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.SubParsers;

public class AbilityTalentParserBase : ParserBase
{
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
        StormElement? buttonElement = _heroesData.GetCompleteStormElement("Button", abilityTalent.ButtonId);

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

        if (buttonDataValues.TryGetElementDataAt("TooltipAppender", out StormElementData? tooltipAppenderArray))
        {
            foreach (string index in tooltipAppenderArray.GetElementDataIndexes())
            {
                StormElementData tooltipAppenderData = tooltipAppenderArray.GetElementDataAt(index);

                if (tooltipAppenderData.TryGetElementDataAt("Face", out StormElementData? faceData) &&
                    tooltipAppenderData.TryGetElementDataAt("Validator", out StormElementData? validatorData))
                {
                    string faceValue = faceData.Value.GetString();
                    string validatorValue = validatorData.Value.GetString();

                    // find the validator
                    StormElement? validatorElement = _heroesData.GetCompleteStormElement("Validator", validatorValue);

                    if (validatorElement is null)
                    {
                        _logger.LogWarning("Validator element {ValidatorValue} not found for button {ButtonId}", validatorValue, abilityTalent.ButtonId);
                        continue;
                    }

                    if (validatorElement.DataValues.TryGetElementDataAt("Value", out StormElementData? valueData))
                    {
                        // this is the CTalent id
                        string valueValue = valueData.Value.GetString();

                        // check if it exists
                        if (_heroesData.StormElementExists("Talent", valueValue))
                            abilityTalent.TooltipAppenderTalentIds.Add(new TalentId(valueValue, faceValue));
                        else
                            _logger.LogWarning("Talent element {ValueValue} not found for button {ButtonId}", valueValue, abilityTalent.ButtonId);
                    }
                }
                else
                {
                    _logger.LogWarning("TooltipAppender element {Index} is missing Face or Validator for button {ButtonId}", index, abilityTalent.ButtonId);
                }
            }
        }
    }
}
