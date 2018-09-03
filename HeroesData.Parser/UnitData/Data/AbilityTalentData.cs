using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Parser.Exceptions;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Data
{
    public class AbilityTalentData
    {
        public AbilityTalentData(GameData gameData, HeroOverride heroOverride, ParsedGameStrings parsedGameStrings, TextValueData textValueData)
        {
            GameData = gameData;
            HeroOverride = heroOverride;
            ParsedGameStrings = parsedGameStrings;
            TextValueData = textValueData;
        }

        protected GameData GameData { get; }
        protected HeroOverride HeroOverride { get; }
        protected ParsedGameStrings ParsedGameStrings { get; }
        protected TextValueData TextValueData { get; }

        protected void SetAbilityTalentName(XElement cButtonElement, AbilityTalentBase abilityTalentBase)
        {
            string name = abilityTalentBase.FullTooltipNameId;

            // check for name override
            XElement buttonNameElement = cButtonElement.Element("Name");
            if (buttonNameElement != null)
                name = Path.GetFileName(PathExtensions.GetFilePath(buttonNameElement.Attribute("value").Value)); // override

            if (ParsedGameStrings.TryGetAbilityTalentParsedNames(name, out string abilityTalentName))
                abilityTalentBase.Name = abilityTalentName;
            else
                throw new ParseException($"{nameof(SetAbilityTalentName)}: {name} not found in description names");
        }

        protected void SetAbilityTalentIcon(XElement cButtonElement, AbilityTalentBase abilityTalentBase)
        {
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

            var foundElements = GameData.XmlGameData.Root.Elements().Where(x => x.Attribute("id")?.Value == elementId).ToList();

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
                                    replaceText = TextValueData.StringChargeCooldownColon; // Charge Cooldown:<space>
                                else
                                    replaceText = TextValueData.StringCooldownColon; // Cooldown:<space>

                                if (cooldownValue == "1")
                                    abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(TextValueData.AbilTooltipCooldownText.Replace(TextValueData.StringCooldownColon, replaceText).Replace(TextValueData.ReplacementCharacter, cooldownValue));
                                else
                                    abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(TextValueData.AbilTooltipCooldownPluralText.Replace(TextValueData.StringCooldownColon, replaceText).Replace(TextValueData.ReplacementCharacter, cooldownValue));
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
                                    replaceText = TextValueData.StringChargeCooldownColon; // Charge Cooldown:<space>
                                else
                                    replaceText = TextValueData.StringCooldownColon; // Cooldown:<space>

                                if (!string.IsNullOrEmpty(cooldownValue))
                                {
                                    if (cooldownValue == "1")
                                        abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(TextValueData.AbilTooltipCooldownText.Replace(TextValueData.StringCooldownColon, TextValueData.StringChargeCooldownColon).Replace(TextValueData.ReplacementCharacter, cooldownValue));
                                    else
                                        abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(TextValueData.AbilTooltipCooldownPluralText.Replace(TextValueData.StringCooldownColon, TextValueData.StringChargeCooldownColon).Replace(TextValueData.ReplacementCharacter, cooldownValue));
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
                                abilityTalentBase.Tooltip.Charges.RecastCoodown = double.Parse(cooldownValue);
                            }
                            else
                            {
                                double cooldown = double.Parse(cooldownValue);

                                if (cooldown == 1)
                                    abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(TextValueData.AbilTooltipCooldownText.Replace(TextValueData.ReplacementCharacter, cooldownValue));
                                else if (cooldown >= 1)
                                    abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(TextValueData.AbilTooltipCooldownPluralText.Replace(TextValueData.ReplacementCharacter, cooldownValue));

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

                            if (ParsedGameStrings.TooltipsByKeyString.TryGetValue($"{TextValueData.UITooltipAbilLookupPrefix}{TextValueData.HeroEnergyTypeEnglish}", out string energyText))
                                abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(DescriptionValidator.Validate(energyText.Replace(TextValueData.ReplacementCharacter, vitalValue)));
                            else
                                abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(DescriptionValidator.Validate(TextValueData.DefaultAbilityTalentEnergyText.Replace(TextValueData.ReplacementCharacter, vitalValue)));
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

            // full tooltip
            XElement cButtonTooltipElement = cButtonElement.Element("Tooltip");
            if (cButtonTooltipElement != null)
            {
                fullTooltipValue = Path.GetFileName(PathExtensions.GetFilePath(cButtonTooltipElement.Attribute("value").Value));
            }

            // short tooltip
            XElement cButtonSimpleDisplayTextElement = cButtonElement.Element("SimpleDisplayText");
            if (cButtonSimpleDisplayTextElement != null)
            {
                shortTooltipValue = Path.GetFileName(PathExtensions.GetFilePath(cButtonSimpleDisplayTextElement.Attribute("value").Value));
            }

            // Vital Name override
            XElement cButtonTooltipVitalNameElement = cButtonElement.Element("TooltipVitalName");
            if (cButtonTooltipVitalNameElement != null)
            {
                string index = cButtonTooltipVitalNameElement.Attribute("index")?.Value;
                if (index == "Life")
                {
                    string value = cButtonTooltipVitalNameElement.Attribute("value").Value;

                    if (string.IsNullOrEmpty(value))
                    {
                        abilityTalentBase.Tooltip.Life.LifeCostTooltip = null;
                    }
                    else if (ParsedGameStrings.TooltipsByKeyString.TryGetValue(cButtonTooltipVitalNameElement.Attribute("value").Value, out string overrideVitalName))
                    {
                        if (string.IsNullOrEmpty(abilityTalentBase.Tooltip.Life.LifeCostTooltip?.RawDescription))
                            abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName));
                        else if (overrideVitalName.Contains(TextValueData.ReplacementCharacter))
                            abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace(TextValueData.ReplacementCharacter, abilityTalentBase.Tooltip.Life.LifeValue.ToString())));
                        else
                            abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace(TextValueData.ReplacementCharacter, abilityTalentBase.Tooltip.Life.LifeCostTooltip.RawDescription)));
                    }
                }
                else if (index == "Energy")
                {
                    string value = cButtonTooltipVitalNameElement.Attribute("value").Value;

                    if (string.IsNullOrEmpty(value))
                    {
                        abilityTalentBase.Tooltip.Energy.EnergyTooltip = null;
                    }
                    else if (ParsedGameStrings.TooltipsByKeyString.TryGetValue(value, out string overrideVitalName))
                    {
                        if (string.IsNullOrEmpty(abilityTalentBase.Tooltip.Energy.EnergyTooltip?.RawDescription))
                            abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName));
                        else if (overrideVitalName.Contains(TextValueData.ReplacementCharacter))
                            abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace(TextValueData.ReplacementCharacter, abilityTalentBase.Tooltip.Energy.EnergyValue.ToString())));
                        else
                            abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace(TextValueData.ReplacementCharacter, abilityTalentBase.Tooltip.Energy.EnergyTooltip.RawDescription)));
                    }
                }
            }

            // check for vital override text
            XElement cButtonTooltipVitalElement = cButtonElement.Element("TooltipVitalOverrideText");
            if (cButtonTooltipVitalElement != null)
            {
                if (ParsedGameStrings.TooltipsByKeyString.TryGetValue(cButtonTooltipVitalElement.Attribute("value").Value, out string text))
                {
                    if (cButtonTooltipVitalElement.Attribute("index")?.Value == "Energy")
                    {
                        // check if overriding text starts with the energy text
                        if (!new TooltipDescription(DescriptionValidator.Validate(text)).PlainText.StartsWith(hero.Energy.EnergyType))
                        {
                            if (ParsedGameStrings.TooltipsByKeyString.TryGetValue($"{TextValueData.UITooltipAbilLookupPrefix}{TextValueData.HeroEnergyTypeEnglish}", out string energyText))
                                text = DescriptionValidator.Validate(energyText.Replace(TextValueData.ReplacementCharacter, text)); // add it as the default
                        }

                        abilityTalentBase.Tooltip.Energy.EnergyTooltip = new TooltipDescription(text);
                    }
                    else if (cButtonTooltipVitalElement.Attribute("index")?.Value == "Life")
                    {
                        abilityTalentBase.Tooltip.Life.LifeCostTooltip = new TooltipDescription(DescriptionValidator.Validate(abilityTalentBase.Tooltip.Life.LifeCostTooltip.RawDescription.Replace(TextValueData.ReplacementCharacter, text)));
                    }
                }
            }

            // check for cooldown override text
            XElement cButtonTooltipCooldownElement = cButtonElement.Element("TooltipCooldownOverrideText");
            if (cButtonTooltipCooldownElement != null)
            {
                string overrideValueText = cButtonTooltipCooldownElement.Attribute("value").Value;
                if (ParsedGameStrings.TooltipsByKeyString.TryGetValue(overrideValueText, out string text) ||
                    ParsedGameStrings.TryGetFullParsedTooltips(overrideValueText, out text))
                {
                    if (!text.StartsWith(TextValueData.StringCooldownColon))
                        text = $"{TextValueData.StringCooldownColon}{text}";

                    abilityTalentBase.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(text);
                }
            }

            // full
            if (ParsedGameStrings.TryGetFullParsedTooltips(fullTooltipValue, out string fullDescription))
            {
                abilityTalentBase.Tooltip.FullTooltip = new TooltipDescription(fullDescription);
                abilityTalentBase.FullTooltipNameId = fullTooltipValue;
            }
            else if (ParsedGameStrings.TryGetFullParsedTooltips(faceValue, out fullDescription))
            {
                abilityTalentBase.Tooltip.FullTooltip = new TooltipDescription(fullDescription);
            }

            // short
            if (ParsedGameStrings.TryGetShortParsedTooltips(shortTooltipValue, out string shortDescription))
            {
                abilityTalentBase.Tooltip.ShortTooltip = new TooltipDescription(shortDescription);
                abilityTalentBase.ShortTooltipNameId = shortTooltipValue;
            }
            else if (ParsedGameStrings.TryGetShortParsedTooltips(faceValue, out shortDescription))
            {
                abilityTalentBase.Tooltip.ShortTooltip = new TooltipDescription(shortDescription);
            }

            // check if the life and energy string contain the replacement character
            if (abilityTalentBase.Tooltip.Life.LifeCostTooltip != null && abilityTalentBase.Tooltip.Life.LifeCostTooltip.RawDescription.Contains(TextValueData.ReplacementCharacter))
                abilityTalentBase.Tooltip.Life = null;
            if (abilityTalentBase.Tooltip.Energy.EnergyTooltip != null && abilityTalentBase.Tooltip.Energy.EnergyTooltip.RawDescription.Contains(TextValueData.ReplacementCharacter))
                abilityTalentBase.Tooltip.Energy = null;
        }
    }
}
