using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.UnitData.Overrides;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Data
{
    public class AbilityTalentData
    {
        public AbilityTalentData(GameData gameData, DefaultData defaultData, HeroOverride heroOverride, Localization localization)
        {
            GameData = gameData;
            DefaultData = defaultData;
            HeroOverride = heroOverride;
            Localization = localization;
        }

        protected GameData GameData { get; }
        protected HeroOverride HeroOverride { get; }
        protected Localization Localization { get; }
        protected DefaultData DefaultData { get; }

        /// <summary>
        /// Set the tooltips cost data.
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="elementId"></param>
        /// <param name="abilityTalentBase"></param>
        protected void SetTooltipCostData(Hero hero, string elementId, AbilityTalentBase abilityTalentBase)
        {
            if (string.IsNullOrEmpty(elementId))
                return;

            IEnumerable<XElement> foundElements = GameData.XmlGameData.Root.Elements().Where(x => (x.Name.LocalName != "CButton" && x.Name.LocalName != "CWeaponLegacy" && x.Name.LocalName != "CTalent") && x.Attribute("id")?.Value == elementId);

            // look through all elements to find the tooltip info
            foreach (XElement element in foundElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "COST")
                {
                    // charge
                    XElement chargeElement = element.Element("Charge");
                    if (chargeElement != null)
                    {
                        XElement countMaxElement = chargeElement.Element("CountMax");
                        XElement countStartElement = chargeElement.Element("CountStart");
                        XElement countUseElement = chargeElement.Element("CountUse");
                        XElement hideCountElement = chargeElement.Element("HideCount");
                        XElement timeUseElement = chargeElement.Element("TimeUse");

                        if (countMaxElement != null || countStartElement != null || countUseElement != null || hideCountElement != null || timeUseElement != null)
                        {
                            if (countMaxElement != null)
                                abilityTalentBase.Tooltip.Charges.CountMax = int.Parse(countMaxElement.Attribute("value").Value);

                            if (countStartElement != null)
                                abilityTalentBase.Tooltip.Charges.CountStart = int.Parse(countStartElement.Attribute("value").Value);

                            if (countUseElement != null)
                                abilityTalentBase.Tooltip.Charges.CountUse = int.Parse(countUseElement.Attribute("value").Value);

                            if (hideCountElement != null)
                                abilityTalentBase.Tooltip.Charges.IsHideCount = int.Parse(hideCountElement.Attribute("value").Value) == 1 ? true : false;

                            if (timeUseElement != null)
                            {
                                string cooldownValue = timeUseElement.Attribute("value").Value;

                                string replaceText = string.Empty;
                                if (abilityTalentBase.Tooltip.Charges.CountMax.HasValue && abilityTalentBase.Tooltip.Charges.CountMax.Value > 1)
                                    replaceText = GameData.GetGameString(DefaultData.StringChargeCooldownColon); // Charge Cooldown:<space>
                                else
                                    replaceText = GameData.GetGameString(DefaultData.StringCooldownColon); // Cooldown:<space>

                                if (cooldownValue == "1")
                                {
                                    abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.AbilTooltipCooldownText)
                                        .Replace(GameData.GetGameString(DefaultData.StringCooldownColon), replaceText)
                                        .Replace(DefaultData.ReplacementCharacter, cooldownValue));
                                }
                                else
                                {
                                    abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.AbilTooltipCooldownPluralText)
                                        .Replace(GameData.GetGameString(DefaultData.StringCooldownColon), replaceText)
                                        .Replace(DefaultData.ReplacementCharacter, cooldownValue));
                                }
                            }
                        }
                        else
                        {
                            XAttribute countMaxAttribute = chargeElement.Attribute("CountMax");
                            XAttribute countStartAttribute = chargeElement.Attribute("CountStart");
                            XAttribute countUseAttribute = chargeElement.Attribute("CountUse");
                            XAttribute hideCountAttribute = chargeElement.Attribute("HideCount");
                            XAttribute timeUseAttribute = chargeElement.Attribute("TimeUse");
                            if (countMaxAttribute != null)
                                abilityTalentBase.Tooltip.Charges.CountMax = int.Parse(countMaxAttribute.Value);

                            if (countStartAttribute != null)
                                abilityTalentBase.Tooltip.Charges.CountStart = int.Parse(countStartAttribute.Value);

                            if (countUseAttribute != null)
                                abilityTalentBase.Tooltip.Charges.CountUse = int.Parse(countUseAttribute.Value);

                            if (hideCountAttribute != null)
                                abilityTalentBase.Tooltip.Charges.IsHideCount = int.Parse(hideCountAttribute.Value) == 1 ? true : false;

                            if (timeUseAttribute != null)
                            {
                                string cooldownValue = timeUseAttribute.Value;

                                string replaceText = string.Empty;
                                if (abilityTalentBase.Tooltip.Charges.CountMax.HasValue && abilityTalentBase.Tooltip.Charges.CountMax.Value > 1)
                                    replaceText = GameData.GetGameString(DefaultData.StringChargeCooldownColon); // Charge Cooldown:<space>
                                else
                                    replaceText = GameData.GetGameString(DefaultData.StringCooldownColon); // Cooldown:<space>

                                if (!string.IsNullOrEmpty(cooldownValue))
                                {
                                    if (cooldownValue == "1")
                                    {
                                        abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.AbilTooltipCooldownText)
                                            .Replace(GameData.GetGameString(DefaultData.StringCooldownColon), replaceText)
                                            .Replace(DefaultData.ReplacementCharacter, cooldownValue));
                                    }
                                    else
                                    {
                                        abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.AbilTooltipCooldownPluralText)
                                            .Replace(GameData.GetGameString(DefaultData.StringCooldownColon), replaceText)
                                            .Replace(DefaultData.ReplacementCharacter, cooldownValue));
                                    }
                                }
                            }
                        }
                    }

                    // cooldown
                    XElement cooldownElement = element.Element("Cooldown");
                    if (cooldownElement != null)
                    {
                        string cooldownValue = cooldownElement.Attribute("TimeUse")?.Value;
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
                                    abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.AbilTooltipCooldownText).Replace(DefaultData.ReplacementCharacter, cooldownValue));
                                else if (cooldown >= 1)
                                    abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(GameData.GetGameString(DefaultData.AbilTooltipCooldownPluralText).Replace(DefaultData.ReplacementCharacter, cooldownValue));

                                if (cooldown < 1)
                                    abilityTalentBase.Tooltip.Cooldown.ToggleCooldown = cooldown;
                            }
                        }
                    }

                    // vitals
                    XElement vitalElement = element.Element("Vital");
                    if (vitalElement != null)
                    {
                        string vitalIndex = vitalElement.Attribute("index").Value;
                        string vitalValue = vitalElement.Attribute("value").Value;

                        if (vitalIndex == "Energy")
                        {
                            abilityTalentBase.Tooltip.Energy.EnergyValue = double.Parse(vitalValue);
                            abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(DescriptionValidator.Validate(GameData.GetGameString(DefaultData.ButtonTooltipEnergyVitalName).Replace(DefaultData.ReplacementCharacter, vitalValue)));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set the tooltip descriptions.
        /// </summary>
        /// <param name="abilityTalentBase"></param>
        protected void SetTooltipDescriptions(AbilityTalentBase abilityTalentBase)
        {
            abilityTalentBase.Name = GameData.GetParsedGameString(DefaultData.ButtonName.Replace(DefaultData.IdReplacer, abilityTalentBase.FullTooltipNameId));
            abilityTalentBase.ShortTooltipNameId = abilityTalentBase.FullTooltipNameId; // default

            // full
            if (GameData.TryGetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, abilityTalentBase.FullTooltipNameId), out string fullDescription))
            {
                abilityTalentBase.Tooltip.FullTooltip = new TooltipDescription(fullDescription, Localization);
            }

            // short
            if (GameData.TryGetParsedGameString(DefaultData.ButtonSimpleDisplayText.Replace(DefaultData.IdReplacer, abilityTalentBase.FullTooltipNameId), out string shortDescription))
            {
                abilityTalentBase.Tooltip.ShortTooltip = new TooltipDescription(shortDescription, Localization);
            }
        }

        /// <summary>
        /// Set all overrides found in the button element.
        /// </summary>
        /// <param name="cButtonElement"></param>
        /// <param name="abilityTalentBase"></param>
        protected void SetTooltipOverrideData(XElement cButtonElement, AbilityTalentBase abilityTalentBase)
        {
            string defaultEnergyValue = GameData.GetGameString(DefaultData.HeroEnergyTypeManaText);

            // "UI/Tooltip/Abil/<Type>
            string vitalEnergyValueTextTemp = string.Empty;
            string vitalLifeValueTextTemp = string.Empty;

            string overrideTextTemp = string.Empty;

            // parent lookup
            StormButtonParentLookup(cButtonElement, abilityTalentBase, SetTooltipOverrideData);

            // look through each element to set overrides
            foreach (XElement element in cButtonElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    abilityTalentBase.Name = GameData.GetParsedGameString(element.Attribute("value").Value);
                }
                else if (elementName == "ICON")
                {
                    abilityTalentBase.IconFileName = Path.GetFileName(PathExtensions.GetFilePath(element.Attribute("value").Value));
                }
                else if (elementName == "TOOLTIP")
                {
                    string fullTooltipValue = element.Attribute("value").Value;

                    if (GameData.TryGetParsedGameString(fullTooltipValue, out string fullDescription))
                    {
                        abilityTalentBase.Tooltip.FullTooltip = new TooltipDescription(fullDescription, Localization);
                        abilityTalentBase.FullTooltipNameId = Path.GetFileName(PathExtensions.GetFilePath(fullTooltipValue));
                    }
                }
                else if (elementName == "SIMPLEDISPLAYTEXT")
                {
                    string shortTooltipValue = element.Attribute("value").Value;

                    if (GameData.TryGetParsedGameString(shortTooltipValue, out string shortDescription))
                    {
                        abilityTalentBase.Tooltip.ShortTooltip = new TooltipDescription(shortDescription, Localization);
                        abilityTalentBase.ShortTooltipNameId = Path.GetFileName(PathExtensions.GetFilePath(shortTooltipValue));
                    }
                }
                else if (elementName == "TOOLTIPVITALNAME")
                {
                    string index = element.Attribute("index")?.Value;

                    if (index == "Energy")
                    {
                        vitalEnergyValueTextTemp = element.Attribute("value").Value;

                        if (string.IsNullOrEmpty(vitalEnergyValueTextTemp))
                        {
                            abilityTalentBase.Tooltip.Energy.EnergyTooltip = null;
                        }
                        else if (GameData.TryGetParsedGameString(vitalEnergyValueTextTemp, out string overrideVitalName))
                        {
                            if (string.IsNullOrEmpty(abilityTalentBase.Tooltip.Energy.EnergyTooltip?.RawDescription))
                            {
                                abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName));
                            }
                            else if (overrideVitalName.Contains(DefaultData.ReplacementCharacter) && abilityTalentBase.Tooltip.Energy.EnergyValue.HasValue)
                            {
                                abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace(DefaultData.ReplacementCharacter, abilityTalentBase.Tooltip.Energy.EnergyValue.ToString())));
                            }
                            else if (overrideVitalName.Contains(DefaultData.ReplacementCharacter) && !string.IsNullOrEmpty(abilityTalentBase.Tooltip.Energy.EnergyTooltip.RawDescription) && !overrideTextTemp.StartsWith(defaultEnergyValue))
                            {
                                abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace(DefaultData.ReplacementCharacter, overrideTextTemp)));
                            }
                        }
                    }
                    else if (index == "Life")
                    {
                        vitalLifeValueTextTemp = element.Attribute("value").Value;

                        if (string.IsNullOrEmpty(vitalLifeValueTextTemp))
                        {
                            abilityTalentBase.Tooltip.Life.LifeCostTooltip = null;
                        }
                        else if (GameData.TryGetParsedGameString(vitalLifeValueTextTemp, out string overrideVitalName))
                        {
                            if (string.IsNullOrEmpty(abilityTalentBase.Tooltip.Life.LifeCostTooltip?.RawDescription))
                                abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName));
                            else if (overrideVitalName.Contains(DefaultData.ReplacementCharacter))
                                abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace(DefaultData.ReplacementCharacter, abilityTalentBase.Tooltip.Life.LifeValue.ToString())));
                            else
                                abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace(DefaultData.ReplacementCharacter, abilityTalentBase.Tooltip.Life.LifeCostTooltip.RawDescription)));
                        }
                    }
                }
                else if (elementName == "TOOLTIPVITALOVERRIDETEXT")
                {
                    if (GameData.TryGetParsedGameString(element.Attribute("value").Value, out string text))
                    {
                        if (element.Attribute("index")?.Value == "Energy")
                        {
                            // check if overriding text starts with the energy text
                            if (!new TooltipDescription(DescriptionValidator.Validate(text)).PlainText.StartsWith(defaultEnergyValue))
                            {
                                if (GameData.TryGetGameString(DefaultData.ButtonTooltipEnergyVitalName, out string energyText)) // default
                                {
                                    overrideTextTemp = text;
                                    text = DescriptionValidator.Validate(energyText.Replace(DefaultData.ReplacementCharacter, text));
                                }
                            }

                            abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(text);
                        }
                        else if (element.Attribute("index")?.Value == "Life")
                        {
                            abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(DescriptionValidator.Validate(abilityTalentBase.Tooltip.Life.LifeCostTooltip.RawDescription.Replace(DefaultData.ReplacementCharacter, text)));
                        }
                    }
                }
                else if (elementName == "TOOLTIPCOOLDOWNOVERRIDETEXT")
                {
                    string overrideValueText = element.Attribute("value").Value;
                    if (GameData.TryGetParsedGameString(overrideValueText, out string text))
                    {
                        if (!text.StartsWith(GameData.GetGameString(DefaultData.StringCooldownColon)))
                            text = $"{GameData.GetGameString(DefaultData.StringCooldownColon)}{text}";

                        abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(text);
                    }
                }
                else if (elementName == "TOOLTIPFLAGS")
                {
                    string index = element.Attribute("index").Value;

                    if (index == "ShowName" && element.Attribute("value").Value == "0")
                    {
                        abilityTalentBase.Name = string.Empty;
                    }
                    else if (index == "ShowHotkey")
                    {
                    }
                    else if (index == "ShowUsage" && element.Attribute("value").Value == "0")
                    {
                        abilityTalentBase.Tooltip.Life.LifeCostTooltip = null;
                        abilityTalentBase.Tooltip.Energy.EnergyTooltip = null;
                    }
                    else if (index == "ShowTime")
                    {
                    }
                    else if (index == "ShowCooldown" && element.Attribute("value").Value == "0")
                    {
                        abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = null;
                    }
                    else if (index == "ShowRequirements")
                    {
                    }
                    else if (index == "ShowAutocast")
                    {
                    }
                }
            }

            // check if the life and energy string contain the replacement character
            if (abilityTalentBase.Tooltip.Life?.LifeCostTooltip != null && abilityTalentBase.Tooltip.Life.LifeCostTooltip.RawDescription.Contains(DefaultData.ReplacementCharacter))
                abilityTalentBase.Tooltip.Life.LifeCostTooltip = null;
            if (abilityTalentBase.Tooltip.Energy?.EnergyTooltip != null && abilityTalentBase.Tooltip.Energy.EnergyTooltip.RawDescription.Contains(DefaultData.ReplacementCharacter))
                abilityTalentBase.Tooltip.Energy.EnergyTooltip = null;
        }

        private void StormButtonParentLookup(XElement buttonElement, AbilityTalentBase abilityTalentBase, Action<XElement, AbilityTalentBase> methodToExecute)
        {
            string parentValue = buttonElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue) && parentValue != DefaultData.CButtonDefaultBaseId)
            {
                XElement parentElement = GameData.XmlGameData.Root.Elements("CButton").FirstOrDefault(x => x.Attribute("id")?.Value == parentValue);
                if (parentElement != null)
                    methodToExecute(parentElement, abilityTalentBase);
            }
        }
    }
}
