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

        protected void SetAbilityTalentName(XElement cButtonElement, AbilityTalentBase abilityTalentBase)
        {
            string name = GameData.GetParsedGameString(DefaultData.ButtonName.Replace(DefaultData.IdReplacer, abilityTalentBase.FullTooltipNameId));

            StormButtonParentLookup(cButtonElement, abilityTalentBase, SetAbilityTalentName);

            // check for name override
            XElement buttonNameElement = cButtonElement.Element("Name");
            if (buttonNameElement != null)
                name = GameData.GetParsedGameString(buttonNameElement.Attribute("value").Value); // override

            abilityTalentBase.Name = name;
        }

        protected void SetAbilityTalentIcon(XElement cButtonElement, AbilityTalentBase abilityTalentBase)
        {
            StormButtonParentLookup(cButtonElement, abilityTalentBase, SetAbilityTalentIcon);

            XElement buttonIconElement = cButtonElement.Element("Icon");
            if (buttonIconElement != null)
                abilityTalentBase.IconFileName = Path.GetFileName(PathExtensions.GetFilePath(buttonIconElement.Attribute("value").Value));
        }

        /// <summary>
        /// Initally set tooltip info.
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="elementId"></param>
        /// <param name="abilityTalentBase"></param>
        protected void SetTooltipSubInfo(Hero hero, string elementId, AbilityTalentBase abilityTalentBase)
        {
            if (string.IsNullOrEmpty(elementId))
                return;

            IEnumerable<XElement> foundElements = GameData.XmlGameData.Root.Elements().Where(x => x.Attribute("id")?.Value == elementId);

            // look through all elements to find the tooltip info
            foreach (XElement element in foundElements)
            {
                if (element.Name.LocalName == "CButton" || element.Name.LocalName == "CWeaponLegacy" || element.Name.LocalName == "CTalent")
                    continue;

                // cost
                XElement costElement = element.Element("Cost");
                if (costElement != null)
                {
                    // charge
                    XElement chargeElement = costElement.Element("Charge");
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
                    XElement cooldownElement = costElement.Element("Cooldown");
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
                    XElement vitalElement = costElement.Element("Vital");
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
        /// Set short and full descriptions and find tooltip overrides.
        /// </summary>
        /// <param name="cButtonElement"></param>
        /// <param name="hero"></param>
        /// <param name="abilityTalentBase"></param>
        protected void SetTooltipDescriptions(XElement cButtonElement, Hero hero, AbilityTalentBase abilityTalentBase)
        {
            string faceValue = abilityTalentBase.FullTooltipNameId;
            abilityTalentBase.ShortTooltipNameId = faceValue; // set to default

            string fullTooltipValue = string.Empty; // Tooltip
            string shortTooltipValue = string.Empty; // SimpleDisplayText

            // "UI/Tooltip/Abil/<Type>
            string vitalEnergyValueText = string.Empty;
            string vitalLifeValueText = string.Empty;

            StormButtonParentLookup(cButtonElement, hero, abilityTalentBase, SetTooltipDescriptions);

            // full tooltip override
            XElement cButtonTooltipElement = cButtonElement.Element("Tooltip");
            if (cButtonTooltipElement != null)
            {
                fullTooltipValue = cButtonTooltipElement.Attribute("value").Value;
            }

            // short tooltip override
            XElement cButtonSimpleDisplayTextElement = cButtonElement.Element("SimpleDisplayText");
            if (cButtonSimpleDisplayTextElement != null)
            {
                shortTooltipValue = cButtonSimpleDisplayTextElement.Attribute("value").Value;
            }

            // Vital Name override
            XElement cButtonTooltipVitalNameElement = cButtonElement.Element("TooltipVitalName");
            if (cButtonTooltipVitalNameElement != null)
            {
                string index = cButtonTooltipVitalNameElement.Attribute("index")?.Value;

                if (index == "Energy")
                {
                    vitalEnergyValueText = cButtonTooltipVitalNameElement.Attribute("value").Value;

                    if (string.IsNullOrEmpty(vitalEnergyValueText))
                    {
                        abilityTalentBase.Tooltip.Energy.EnergyTooltip = null;
                    }
                    else if (GameData.TryGetParsedGameString(vitalEnergyValueText, out string overrideVitalName))
                    {
                        if (string.IsNullOrEmpty(abilityTalentBase.Tooltip.Energy.EnergyTooltip?.RawDescription))
                            abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName));
                        else if (overrideVitalName.Contains(DefaultData.ReplacementCharacter))
                            abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace(DefaultData.ReplacementCharacter, abilityTalentBase.Tooltip.Energy.EnergyValue.ToString())));
                        else
                            abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace(DefaultData.ReplacementCharacter, abilityTalentBase.Tooltip.Energy.EnergyTooltip.RawDescription)));
                    }
                }
                else if (index == "Life")
                {
                    vitalLifeValueText = cButtonTooltipVitalNameElement.Attribute("value").Value;

                    if (string.IsNullOrEmpty(vitalLifeValueText))
                    {
                        abilityTalentBase.Tooltip.Life.LifeCostTooltip = null;
                    }
                    else if (GameData.TryGetParsedGameString(vitalLifeValueText, out string overrideVitalName))
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

            // check for vital override text
            XElement cButtonTooltipVitalElement = cButtonElement.Element("TooltipVitalOverrideText");
            if (cButtonTooltipVitalElement != null)
            {
                if (GameData.TryGetParsedGameString(cButtonTooltipVitalElement.Attribute("value").Value, out string text))
                {
                    if (cButtonTooltipVitalElement.Attribute("index")?.Value == "Energy")
                    {
                        // check if overriding text starts with the energy text
                        if (!new TooltipDescription(DescriptionValidator.Validate(text)).PlainText.StartsWith(hero.Energy.EnergyType))
                        {
                            if (GameData.TryGetGameString(vitalEnergyValueText, out string energyText)) // vital name override check
                                text = DescriptionValidator.Validate(energyText.Replace(DefaultData.ReplacementCharacter, text));
                            else if (GameData.TryGetGameString(DefaultData.ButtonTooltipEnergyVitalName, out energyText)) // default
                                text = DescriptionValidator.Validate(energyText.Replace(DefaultData.ReplacementCharacter, text));
                        }

                        abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(text);
                    }
                    else if (cButtonTooltipVitalElement.Attribute("index")?.Value == "Life")
                    {
                        abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(DescriptionValidator.Validate(abilityTalentBase.Tooltip.Life.LifeCostTooltip.RawDescription.Replace(DefaultData.ReplacementCharacter, text)));
                    }
                }
            }

            // check for cooldown override text
            XElement cButtonTooltipCooldownElement = cButtonElement.Element("TooltipCooldownOverrideText");
            if (cButtonTooltipCooldownElement != null)
            {
                string overrideValueText = cButtonTooltipCooldownElement.Attribute("value").Value;
                if (GameData.TryGetParsedGameString(overrideValueText, out string text))
                {
                    if (!text.StartsWith(GameData.GetGameString(DefaultData.StringCooldownColon)))
                        text = $"{GameData.GetGameString(DefaultData.StringCooldownColon)}{text}";

                    abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(text);
                }
            }

            // full
            if (GameData.TryGetParsedGameString(fullTooltipValue, out string fullDescription))
            {
                abilityTalentBase.Tooltip.FullTooltip = new TooltipDescription(fullDescription, Localization);
                abilityTalentBase.FullTooltipNameId = Path.GetFileName(PathExtensions.GetFilePath(fullTooltipValue));
            }
            else if (GameData.TryGetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, faceValue), out fullDescription))
            {
                abilityTalentBase.Tooltip.FullTooltip = new TooltipDescription(fullDescription, Localization);
            }

            // short
            if (GameData.TryGetParsedGameString(shortTooltipValue, out string shortDescription))
            {
                abilityTalentBase.Tooltip.ShortTooltip = new TooltipDescription(shortDescription, Localization);
                abilityTalentBase.ShortTooltipNameId = Path.GetFileName(PathExtensions.GetFilePath(shortTooltipValue));
            }
            else if (GameData.TryGetParsedGameString(DefaultData.ButtonSimpleDisplayText.Replace(DefaultData.IdReplacer, faceValue), out shortDescription))
            {
                abilityTalentBase.Tooltip.ShortTooltip = new TooltipDescription(shortDescription, Localization);
            }

            // check if the life and energy string contain the replacement character
            if (abilityTalentBase.Tooltip.Life?.LifeCostTooltip != null && abilityTalentBase.Tooltip.Life.LifeCostTooltip.RawDescription.Contains(DefaultData.ReplacementCharacter))
                abilityTalentBase.Tooltip.Life = null;
            if (abilityTalentBase.Tooltip.Energy?.EnergyTooltip != null && abilityTalentBase.Tooltip.Energy.EnergyTooltip.RawDescription.Contains(DefaultData.ReplacementCharacter))
                abilityTalentBase.Tooltip.Energy = null;
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

        private void StormButtonParentLookup(XElement buttonElement, Hero hero, AbilityTalentBase abilityTalentBase, Action<XElement, Hero, AbilityTalentBase> methodToExecute)
        {
            string parentValue = buttonElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue) && parentValue != DefaultData.CButtonDefaultBaseId)
            {
                XElement parentElement = GameData.XmlGameData.Root.Elements("CButton").FirstOrDefault(x => x.Attribute("id")?.Value == parentValue);
                if (parentElement != null)
                    methodToExecute(parentElement, hero, abilityTalentBase);
            }
        }
    }
}
