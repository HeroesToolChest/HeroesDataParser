using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class AbilityTalentData
    {
        public AbilityTalentData(GameData gameData, DefaultData defaultData, Configuration configuration)
        {
            GameData = gameData;
            DefaultData = defaultData;
            Configuration = configuration;
        }

        public Localization Localization { get; set; }
        public UnitDataOverride? UnitDataOverride { get; set; }
        public HeroDataOverride? HeroDataOverride { get; set; }

        protected GameData GameData { get; }
        protected DefaultData DefaultData { get; }
        protected Configuration Configuration { get; }

        /// <summary>
        /// Returns a collection of all ability elements.
        /// </summary>
        /// <param name="abilElementId">The id of the ability.</param>
        /// <returns></returns>
        protected IEnumerable<XElement> GetAbilityElements(string abilElementId)
        {
            if (string.IsNullOrEmpty(abilElementId))
                return new List<XElement>();

            return GameData.ElementsIncluded(Configuration.GamestringXmlElements("Abil"), abilElementId);
        }

        /// <summary>
        /// Sets the ability data.
        /// </summary>
        /// <param name="abilityElement">The ability element.</param>
        /// <param name="abilityTalentBase"></param>
        /// <param name="abilIndex"></param>
        protected void SetAbilityTalentData(XElement abilityElement, AbilityTalentBase abilityTalentBase, string abilIndex)
        {
            if (abilityElement == null)
                throw new ArgumentNullException(nameof(abilityElement));
            if (abilityTalentBase == null)
                throw new ArgumentNullException(nameof(abilityTalentBase));

            // parent lookup
            string? parentValue = abilityElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(abilityElement.Name.LocalName).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetAbilityTalentData(parentElement, abilityTalentBase, abilIndex);
            }

            // don't want these
            if (parentValue == "attack" || abilityElement.Name.LocalName == "CAbilAttack")
                abilityTalentBase.AbilityTalentId.ReferenceId = string.Empty;

            Action? setCmdButtonArrayDataAction = null;
            bool isAutoCast = false;
            bool isAutoCastOn = false;

            // look through all elements to set all the data
            foreach (XElement element in abilityElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "COST")
                {
                    SetCostData(element, abilityTalentBase);
                }
                else if (elementName == "EFFECT")
                {
                    SetEffectData(element, abilityTalentBase);
                }
                else if (elementName == "CMDBUTTONARRAY")
                {
                    string? indexValue = element.Attribute("index")?.Value;

                    if (!string.IsNullOrEmpty(indexValue) && abilIndex == indexValue)
                    {
                        setCmdButtonArrayDataAction = () => SetCmdButtonArrayData(element, abilityTalentBase);
                    }
                    else if (!string.IsNullOrEmpty(indexValue))
                    {
                        // only set if index is execute
                        // cancel is also an available index, but it doesn't seem to be used in HOTS
                        if (indexValue.Equals("Execute", StringComparison.OrdinalIgnoreCase))
                            setCmdButtonArrayDataAction = () => SetCmdButtonArrayData(element, abilityTalentBase);
                    }
                }
                else if (elementName == "PRODUCEDUNITARRAY")
                {
                    string? elementValue = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(elementValue))
                    {
                        if (GameData.Elements("CUnit").Where(x => x.Attribute("id")?.Value == elementValue).Any())
                        {
                            abilityTalentBase.CreatedUnits.Add(elementValue);
                        }
                    }
                }
                else if (elementName == "NAME") // ability name
                {
                    string? valueAttribute = element.Attribute("value")?.Value;

                    if (string.IsNullOrEmpty(abilityTalentBase.Name) && !string.IsNullOrEmpty(valueAttribute) && GameData.TryGetGameString(valueAttribute, out string? value))
                        abilityTalentBase.Name = value;
                }
                else if (elementName == "PARENTABIL")
                {
                    string? parentAbilityValue = element.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(parentAbilityValue))
                    {
                        XElement? parentAbilityElement = GetAbilityElements(parentAbilityValue).FirstOrDefault();
                        if (parentAbilityElement != null)
                        {
                            string? defaultButtonFace = parentAbilityElement.Element("CmdButtonArray")?.Attribute("DefaultButtonFace")?.Value;

                            abilityTalentBase.ParentLink = new AbilityTalentId(parentAbilityValue, defaultButtonFace ?? parentAbilityValue);
                        }
                    }
                }
                else if (elementName == "FLAGS")
                {
                    string? index = element.Attribute("index")?.Value.ToUpperInvariant();

                    if (index == "AUTOCAST" && element.Attribute("value")?.Value == "1")
                    {
                        isAutoCast = true;
                    }
                    else if (index == "AUTOCASTON" && element.Attribute("value")?.Value == "1")
                    {
                        isAutoCastOn = true;
                    }
                }
            }

            if (isAutoCast && isAutoCastOn)
                abilityTalentBase.IsActive = false;

            // must execute the cmdButtonArrayData last
            setCmdButtonArrayDataAction?.Invoke();
        }

        protected void SetButtonData(XElement buttonElement, AbilityTalentBase abilityTalentBase)
        {
            if (buttonElement == null)
                throw new ArgumentNullException(nameof(buttonElement));
            if (abilityTalentBase == null)
                throw new ArgumentNullException(nameof(abilityTalentBase));

            string? parentValue = buttonElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue) && parentValue != DefaultDataButton.CButtonDefaultBaseId)
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == parentValue), false);
                if (parentElement != null)
                    SetButtonData(parentElement, abilityTalentBase);
            }

            SetTooltipDescriptions(buttonElement, abilityTalentBase);
            SetTooltipData(buttonElement, abilityTalentBase);
        }

        /// <summary>
        /// Set the tooltip descriptions.
        /// </summary>
        /// <param name="buttonElement"></param>
        /// <param name="abilityTalentBase"></param>
        protected void SetTooltipDescriptions(XElement buttonElement, AbilityTalentBase abilityTalentBase)
        {
            if (buttonElement == null)
                throw new ArgumentNullException(nameof(buttonElement));
            if (abilityTalentBase == null)
                throw new ArgumentNullException(nameof(abilityTalentBase));

            string? buttonId = buttonElement.Attribute("id")?.Value;

            string name = GameData.GetGameString(DefaultData.ButtonData?.ButtonName?.Replace(DefaultData.IdPlaceHolder, buttonId, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(name))
                abilityTalentBase.Name = name;

            if (string.IsNullOrEmpty(abilityTalentBase.Name))
                abilityTalentBase.Name = GameData.GetGameString(DefaultData.AbilData?.AbilName.Replace(DefaultData.IdPlaceHolder, buttonId, StringComparison.OrdinalIgnoreCase));

            // full
            if (DefaultData.ButtonData?.ButtonTooltip != null && GameData.TryGetGameString(DefaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, buttonId, StringComparison.OrdinalIgnoreCase), out string? fullDescription))
            {
                abilityTalentBase.Tooltip.FullTooltip = new TooltipDescription(fullDescription ?? string.Empty, Localization);
            }

            // short
            if (DefaultData.ButtonData?.ButtonSimpleDisplayText != null && GameData.TryGetGameString(DefaultData.ButtonData.ButtonSimpleDisplayText.Replace(DefaultData.IdPlaceHolder, buttonId, StringComparison.OrdinalIgnoreCase), out string? shortDescription))
            {
                abilityTalentBase.Tooltip.ShortTooltip = new TooltipDescription(shortDescription ?? string.Empty, Localization);
            }
        }

        /// <summary>
        /// Set all element data found in the button element.
        /// </summary>
        /// <param name="buttonElement"></param>
        /// <param name="abilityTalentBase"></param>
        protected void SetTooltipData(XElement buttonElement, AbilityTalentBase abilityTalentBase)
        {
            if (buttonElement == null)
                throw new ArgumentNullException(nameof(buttonElement));
            if (abilityTalentBase == null)
                throw new ArgumentNullException(nameof(abilityTalentBase));

            string defaultEnergyValue = GameData.GetGameString(DefaultData.HeroEnergyTypeManaText);

            // "UI/Tooltip/Abil/<Type>
            string? vitalEnergyValueTextTemp;
            string? vitalLifeValueTextTemp;

            string overrideTextTemp = string.Empty;

            // look through each element to set overrides
            foreach (XElement element in buttonElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "NAME")
                {
                    abilityTalentBase.Name = GameData.GetGameString(element.Attribute("value")?.Value);
                }
                else if (elementName == "ICON")
                {
                    abilityTalentBase.IconFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value)) ?? string.Empty;
                }
                else if (elementName == "TOOLTIP")
                {
                    string? fullTooltipValue = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(fullTooltipValue) && GameData.TryGetGameString(fullTooltipValue, out string? fullDescription))
                    {
                        abilityTalentBase.Tooltip.FullTooltip = new TooltipDescription(fullDescription ?? string.Empty, Localization);
                    }
                }
                else if (elementName == "SIMPLEDISPLAYTEXT")
                {
                    string? shortTooltipValue = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(shortTooltipValue) && GameData.TryGetGameString(shortTooltipValue, out string? shortDescription))
                    {
                        abilityTalentBase.Tooltip.ShortTooltip = new TooltipDescription(shortDescription ?? string.Empty, Localization);
                    }
                }
                else if (elementName == "TOOLTIPVITALNAME")
                {
                    string? index = element.Attribute("index")?.Value;

                    if (index == "Energy")
                    {
                        vitalEnergyValueTextTemp = element.Attribute("value")?.Value;

                        if (string.IsNullOrEmpty(vitalEnergyValueTextTemp))
                        {
                            abilityTalentBase.Tooltip.Energy.EnergyTooltip = null;
                        }
                        else if (GameData.TryGetGameString(vitalEnergyValueTextTemp, out string? overrideVitalName))
                        {
                            if (string.IsNullOrEmpty(abilityTalentBase.Tooltip.Energy.EnergyTooltip?.RawDescription))
                            {
                                abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(overrideVitalName!);
                            }
                            else if (overrideVitalName!.Contains(DefaultData.ReplacementCharacter, StringComparison.OrdinalIgnoreCase) && abilityTalentBase.Tooltip.Energy.EnergyValue.HasValue)
                            {
                                abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(overrideVitalName.Replace(DefaultData.ReplacementCharacter, abilityTalentBase.Tooltip.Energy.EnergyValue.ToString(), StringComparison.OrdinalIgnoreCase));
                            }
                            else if (overrideVitalName.Contains(DefaultData.ReplacementCharacter, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(abilityTalentBase.Tooltip.Energy.EnergyTooltip.RawDescription) && !overrideTextTemp!.StartsWith(defaultEnergyValue, StringComparison.OrdinalIgnoreCase))
                            {
                                if (string.IsNullOrEmpty(overrideTextTemp))
                                    abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(string.Empty);
                                else
                                    abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(overrideVitalName.Replace(DefaultData.ReplacementCharacter, overrideTextTemp, StringComparison.OrdinalIgnoreCase));
                            }
                        }
                    }
                    else if (index == "Life")
                    {
                        vitalLifeValueTextTemp = element.Attribute("value")?.Value;

                        if (string.IsNullOrEmpty(vitalLifeValueTextTemp))
                        {
                            abilityTalentBase.Tooltip.Life.LifeCostTooltip = null;
                        }
                        else if (GameData.TryGetGameString(vitalLifeValueTextTemp, out string? overrideVitalName))
                        {
                            if (string.IsNullOrEmpty(abilityTalentBase.Tooltip.Life.LifeCostTooltip?.RawDescription))
                                abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(overrideVitalName!);
                            else if (overrideVitalName!.Contains(DefaultData.ReplacementCharacter, StringComparison.OrdinalIgnoreCase))
                                abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(overrideVitalName.Replace(DefaultData.ReplacementCharacter, abilityTalentBase.Tooltip.Life.LifeValue.ToString(), StringComparison.OrdinalIgnoreCase));
                            else
                                abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(overrideVitalName.Replace(DefaultData.ReplacementCharacter, abilityTalentBase.Tooltip.Life.LifeCostTooltip.RawDescription, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                }
                else if (elementName == "TOOLTIPVITALOVERRIDETEXT")
                {
                    string? value = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(value))
                    {
                        if (GameData.TryGetGameString(value, out string? text))
                        {
                            if (element.Attribute("index")?.Value == "Energy")
                            {
                                // check if overriding text starts with the energy text
                                if (!new TooltipDescription(text!).PlainText.StartsWith(defaultEnergyValue, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (DefaultData.ButtonData?.ButtonTooltipEnergyVitalName != null && GameData.TryGetGameString(DefaultData.ButtonData.ButtonTooltipEnergyVitalName, out string? energyText)) // default
                                    {
                                        overrideTextTemp = text;
                                        text = DescriptionValidator.Validate(energyText!.Replace(DefaultData.ReplacementCharacter, text, StringComparison.OrdinalIgnoreCase));
                                    }
                                }

                                abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(text!);
                            }
                            else if (element.Attribute("index")?.Value == "Life" && abilityTalentBase.Tooltip.Life.LifeCostTooltip != null)
                            {
                                abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(abilityTalentBase.Tooltip.Life.LifeCostTooltip.RawDescription.Replace(DefaultData.ReplacementCharacter, text, StringComparison.OrdinalIgnoreCase));
                            }
                        }
                    }
                }
                else if (elementName == "TOOLTIPCOOLDOWNOVERRIDETEXT")
                {
                    string? overrideValueText = element.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(overrideValueText))
                    {
                        if (GameData.TryGetGameString(overrideValueText, out string? text))
                        {
                            if (!text!.StartsWith(GameData.GetGameString(DefaultData.StringCooldownColon), StringComparison.OrdinalIgnoreCase))
                                text = $"{GameData.GetGameString(DefaultData.StringCooldownColon)}{text}";

                            abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(text);
                        }
                    }
                }
                else if (elementName == "TOOLTIPFLAGS")
                {
                    string? index = element.Attribute("index")?.Value;

                    if (!string.IsNullOrEmpty(index))
                    {
                        if (index == "ShowName" && element.Attribute("value")?.Value == "0")
                        {
                            abilityTalentBase.Name = string.Empty;
                        }
                        else if (index == "ShowHotkey")
                        {
                        }
                        else if (index == "ShowUsage" && element.Attribute("value")?.Value == "0")
                        {
                            abilityTalentBase.Tooltip.Life.LifeCostTooltip = null;
                            abilityTalentBase.Tooltip.Energy.EnergyTooltip = null;
                        }
                        else if (index == "ShowTime")
                        {
                        }
                        else if (index == "ShowCooldown" && element.Attribute("value")?.Value == "0")
                        {
                            // ignore, always show the cooldown
                        }
                        else if (index == "ShowRequirements")
                        {
                        }
                        else if (index == "ShowAutocast")
                        {
                        }
                    }
                }
                else if (elementName == "USEHOTKEYLABEL")
                {
                    string? value = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(value) && value == "0")
                        abilityTalentBase.IsActive = false; // ability has no hotkey, not activable
                }
            }

            // check if the life and energy string contain the replacement character
            if (abilityTalentBase.Tooltip.Life?.LifeCostTooltip != null && abilityTalentBase.Tooltip.Life.LifeCostTooltip.RawDescription.Contains(DefaultData.ReplacementCharacter, StringComparison.OrdinalIgnoreCase))
                abilityTalentBase.Tooltip.Life.LifeCostTooltip = null;
            if (abilityTalentBase.Tooltip.Energy?.EnergyTooltip != null && abilityTalentBase.Tooltip.Energy.EnergyTooltip.RawDescription.Contains(DefaultData.ReplacementCharacter, StringComparison.OrdinalIgnoreCase))
                abilityTalentBase.Tooltip.Energy.EnergyTooltip = null;
        }

        private void SetCostData(XElement costElement, AbilityTalentBase abilityTalentBase)
        {
            if (costElement == null)
                throw new ArgumentNullException(nameof(costElement));
            if (abilityTalentBase == null)
                throw new ArgumentNullException(nameof(abilityTalentBase));

            XElement? chargeElement = costElement.Element("Charge");
            if (chargeElement != null)
            {
                XElement? countMaxElement = chargeElement.Element("CountMax");
                XElement? countStartElement = chargeElement.Element("CountStart");
                XElement? countUseElement = chargeElement.Element("CountUse");
                XElement? hideCountElement = chargeElement.Element("HideCount");
                XElement? timeUseElement = chargeElement.Element("TimeUse");

                // as attributes
                if (countMaxElement != null || countStartElement != null || countUseElement != null || hideCountElement != null || timeUseElement != null)
                {
                    if (countMaxElement != null)
                        abilityTalentBase.Tooltip.Charges.CountMax = int.Parse(GameData.GetValueFromAttribute(countMaxElement.Attribute("value")?.Value)!);

                    if (countStartElement != null)
                        abilityTalentBase.Tooltip.Charges.CountStart = int.Parse(GameData.GetValueFromAttribute(countStartElement.Attribute("value")?.Value)!);

                    if (countUseElement != null)
                        abilityTalentBase.Tooltip.Charges.CountUse = int.Parse(GameData.GetValueFromAttribute(countUseElement.Attribute("value")?.Value)!);

                    if (hideCountElement != null)
                        abilityTalentBase.Tooltip.Charges.IsHideCount = int.Parse(GameData.GetValueFromAttribute(hideCountElement.Attribute("value")?.Value)!) == 1;

                    if (timeUseElement != null)
                    {
                        string? cooldownValue = GameData.GetValueFromAttribute(timeUseElement.Attribute("value")?.Value);

                        string replaceText;
                        if (abilityTalentBase.Tooltip.Charges.CountMax.HasValue && abilityTalentBase.Tooltip.Charges.CountMax.Value > 1)
                            replaceText = GameData.GetGameString(DefaultData.StringChargeCooldownColon); // Charge Cooldown:<space>
                        else
                            replaceText = GameData.GetGameString(DefaultData.StringCooldownColon); // Cooldown:<space>

                        if (cooldownValue == "1")
                        {
                            abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.AbilTooltipCooldownText)
                                .Replace(GameData.GetGameString(DefaultData.StringCooldownColon), replaceText, StringComparison.OrdinalIgnoreCase)
                                .Replace(DefaultData.ReplacementCharacter, cooldownValue, StringComparison.OrdinalIgnoreCase));
                        }
                        else
                        {
                            abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.AbilTooltipCooldownPluralText)
                                .Replace(GameData.GetGameString(DefaultData.StringCooldownColon), replaceText, StringComparison.OrdinalIgnoreCase)
                                .Replace(DefaultData.ReplacementCharacter, cooldownValue, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                }
                else // as elements
                {
                    XAttribute? countMaxAttribute = chargeElement.Attribute("CountMax");
                    XAttribute? countStartAttribute = chargeElement.Attribute("CountStart");
                    XAttribute? countUseAttribute = chargeElement.Attribute("CountUse");
                    XAttribute? hideCountAttribute = chargeElement.Attribute("HideCount");
                    XAttribute? timeUseAttribute = chargeElement.Attribute("TimeUse");

                    if (countMaxAttribute != null)
                        abilityTalentBase.Tooltip.Charges.CountMax = int.Parse(GameData.GetValueFromAttribute(countMaxAttribute.Value)!);

                    if (countStartAttribute != null)
                        abilityTalentBase.Tooltip.Charges.CountStart = int.Parse(GameData.GetValueFromAttribute(countStartAttribute.Value)!);

                    if (countUseAttribute != null)
                        abilityTalentBase.Tooltip.Charges.CountUse = int.Parse(GameData.GetValueFromAttribute(countUseAttribute.Value)!);

                    if (hideCountAttribute != null)
                        abilityTalentBase.Tooltip.Charges.IsHideCount = int.Parse(GameData.GetValueFromAttribute(hideCountAttribute.Value)!) == 1;

                    if (timeUseAttribute != null)
                    {
                        string cooldownValue = timeUseAttribute.Value;

                        string replaceText;
                        if (abilityTalentBase.Tooltip.Charges.CountMax.HasValue && abilityTalentBase.Tooltip.Charges.CountMax.Value > 1)
                            replaceText = GameData.GetGameString(DefaultData.StringChargeCooldownColon); // Charge Cooldown:<space>
                        else
                            replaceText = GameData.GetGameString(DefaultData.StringCooldownColon); // Cooldown:<space>

                        if (!string.IsNullOrEmpty(cooldownValue))
                        {
                            if (cooldownValue == "1")
                            {
                                abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.AbilTooltipCooldownText)
                                    .Replace(GameData.GetGameString(DefaultData.StringCooldownColon), replaceText, StringComparison.OrdinalIgnoreCase)
                                    .Replace(DefaultData.ReplacementCharacter, cooldownValue, StringComparison.OrdinalIgnoreCase));
                            }
                            else
                            {
                                abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.AbilTooltipCooldownPluralText)
                                    .Replace(GameData.GetGameString(DefaultData.StringCooldownColon), replaceText, StringComparison.OrdinalIgnoreCase)
                                    .Replace(DefaultData.ReplacementCharacter, cooldownValue, StringComparison.OrdinalIgnoreCase));
                            }
                        }
                    }
                }
            }

            // cooldown
            XElement? cooldownElement = costElement.Element("Cooldown");
            if (cooldownElement != null)
            {
                string? cooldownValue = GameData.GetValueFromAttribute(cooldownElement.Attribute("TimeUse")?.Value ?? string.Empty);
                if (!string.IsNullOrEmpty(cooldownValue))
                {
                    if (abilityTalentBase.Tooltip.Charges.HasCharges)
                    {
                        abilityTalentBase.Tooltip.Charges.RecastCooldown = double.Parse(cooldownValue);
                    }
                    else
                    {
                        double cooldown = double.Parse(cooldownValue);

                        if (cooldown == 1)
                            abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.AbilTooltipCooldownText).Replace(DefaultData.ReplacementCharacter, cooldownValue, StringComparison.OrdinalIgnoreCase));
                        else if (cooldown >= 1)
                            abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.AbilTooltipCooldownPluralText).Replace(DefaultData.ReplacementCharacter, cooldownValue, StringComparison.OrdinalIgnoreCase));

                        if (cooldown < 1)
                            abilityTalentBase.Tooltip.Cooldown.ToggleCooldown = cooldown;
                    }
                }
            }

            // vitals
            XElement? vitalElement = costElement.Element("Vital");
            if (vitalElement != null)
            {
                string? vitalIndex = GameData.GetValueFromAttribute(vitalElement.Attribute("index")?.Value);
                string? vitalValue = GameData.GetValueFromAttribute(vitalElement.Attribute("value")?.Value);

                if (vitalIndex == "Energy" && !string.IsNullOrEmpty(vitalValue))
                {
                    abilityTalentBase.Tooltip.Energy.EnergyValue = double.Parse(vitalValue);
                    abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.ButtonData?.ButtonTooltipEnergyVitalName).Replace(DefaultData.ReplacementCharacter, vitalValue, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        private void SetCmdButtonArrayData(XElement cmdButtonArrayElement, AbilityTalentBase abilityTalentBase)
        {
            if (cmdButtonArrayElement == null)
                throw new ArgumentNullException(nameof(cmdButtonArrayElement));
            if (abilityTalentBase == null)
                throw new ArgumentNullException(nameof(abilityTalentBase));

            string defaultButtonFace = cmdButtonArrayElement.Attribute("DefaultButtonFace")?.Value ?? cmdButtonArrayElement.Element("DefaultButtonFace")?.Attribute("value")?.Value ?? string.Empty;
            string requirement = cmdButtonArrayElement.Attribute("Requirements")?.Value ?? cmdButtonArrayElement.Element("Requirements")?.Attribute("value")?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(abilityTalentBase.AbilityTalentId.ButtonId))
                abilityTalentBase.AbilityTalentId.ButtonId = defaultButtonFace;

            // check only the face value (fullTooltipNameId), we could also check the defaultButtonFace but it was chosen not to
            XElement? buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == abilityTalentBase.AbilityTalentId.ButtonId), false);
            if (buttonElement != null)
            {
                SetButtonData(buttonElement, abilityTalentBase);
            }

            if (!string.IsNullOrEmpty(requirement))
            {
                if (requirement == "HeroHasNoDeadBehaviorAndNotInBase")
                    return;

                if (requirement == "IsMounted")
                {
                    // ability id of the standard Mount ability
                    abilityTalentBase.ParentLink = new AbilityTalentId("Mount", "SummonMount")
                    {
                        AbilityType = AbilityTypes.Z,
                    };
                    return;
                }

                XElement? requirementElement = GameData.MergeXmlElements(GameData.Elements("CRequirement").Where(x => x.Attribute("id")?.Value == requirement));
                if (requirementElement != null)
                {
                    foreach (XElement element in requirementElement.Elements())
                    {
                        string elementName = element.Name.LocalName.ToUpperInvariant();

                        if (elementName == "NODEARRAY")
                        {
                            string? indexValue = element.Attribute("index")?.Value;
                            string? linkValue = element.Attribute("Link")?.Value;

                            if (linkValue == "EqCountBehaviorHeroGenericPregameAbilitySuppressionCompleteOnlyAtUnit0")
                                return;

                            if (linkValue == "0")
                            {
                                abilityTalentBase.AbilityTalentId.ReferenceId = string.Empty;
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void SetEffectData(XElement effectElement, AbilityTalentBase abilityTalentBase)
        {
            if (effectElement == null)
                throw new ArgumentNullException(nameof(effectElement));
            if (abilityTalentBase == null)
                throw new ArgumentNullException(nameof(abilityTalentBase));

            string? effectValue = effectElement.Attribute("value")?.Value;
            if (!string.IsNullOrEmpty(effectValue))
            {
                // see if we can find a create unit
                XElement? effectTypeElement = GameData.MergeXmlElements(GameData.ElementsIncluded(Configuration.GamestringXmlElements("Effect"), effectValue));
                if (effectTypeElement != null)
                {
                    foreach (XElement element in effectTypeElement.Elements())
                    {
                        string elementName = element.Name.LocalName.ToUpperInvariant();

                        if (elementName == "EFFECTARRAY" || elementName == "CASEDEFAULT" || elementName == "PRODUCEDUNITARRAY")
                        {
                            FindCreateUnit(element.Attribute("value")?.Value, abilityTalentBase);
                        }
                        else if (elementName == "CASEARRAY")
                        {
                            FindCreateUnit(element.Attribute("Effect")?.Value, abilityTalentBase);
                        }
                    }
                }
            }
        }

        private void FindCreateUnit(string? effectId, AbilityTalentBase abilityTalentBase)
        {
            if (abilityTalentBase == null)
            {
                throw new ArgumentNullException(nameof(abilityTalentBase));
            }

            if (!string.IsNullOrEmpty(effectId))
            {
                // find CEffectCreateUnit
                XElement? effectCreateUnitElement = GameData.MergeXmlElements(GameData.Elements("CEffectCreateUnit").Where(x => x.Attribute("id")?.Value == effectId), false);
                if (effectCreateUnitElement != null)
                {
                    string? spawnUnitValue = effectCreateUnitElement.Element("SpawnUnit")?.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(spawnUnitValue))
                    {
                        if (GameData.Elements("CUnit").Where(x => x.Attribute("id")?.Value == spawnUnitValue).Any())
                        {
                            abilityTalentBase.CreatedUnits.Add(spawnUnitValue);
                        }
                    }
                }
            }
        }
    }
}
