using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Exceptions;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class AbilityData : AbilityTalentData
    {
        public AbilityData(GameData gameData, DefaultData defaultData, HeroDataOverride heroDataOverride, Localization localization)
            : base(gameData, defaultData, heroDataOverride, localization)
        {
        }

        public AbilityData(GameData gameData, DefaultData defaultData, UnitDataOverride unitDataOverride, Localization localization)
            : base(gameData, defaultData, unitDataOverride, localization)
        {
        }

        /// <summary>
        /// Adds hero's ability data from the ability xml element.
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="abilityElement">The HeroAbilArray xml element.</param>
        public void AddHeroAbility(Hero hero, XElement abilityElement)
        {
            hero.Abilities = hero.Abilities ?? new Dictionary<string, Ability>();

            string referenceName = abilityElement.Attribute("Abil")?.Value;
            string tooltipName = abilityElement.Attribute("Button")?.Value;
            string parentLink = abilityElement.Attribute("Unit")?.Value;

            XElement usableAbility = abilityElement.Elements("Flags").FirstOrDefault(x => x.Attribute("index").Value == "ShowInHeroSelect" && x.Attribute("value").Value == "1");
            XElement mountAbility = abilityElement.Elements("Flags").FirstOrDefault(x => x.Attribute("index").Value == "MountReplacement" && x.Attribute("value").Value == "1");

            Ability ability = new Ability();

            if (!string.IsNullOrEmpty(referenceName) && HeroDataOverride.IsValidAbilityByAbilityId.TryGetValue(referenceName, out bool validAbility))
            {
                if (!validAbility)
                    return;
            }
            else
            {
                validAbility = false;
            }

            if (usableAbility == null && mountAbility == null && parentLink == null && !validAbility)
                return;

            // check for the HeroAbilArray button value, we may need to override it
            if (!string.IsNullOrEmpty(referenceName) && HeroDataOverride.ButtonNameOverrideByAbilityButtonId.TryGetValue((referenceName, tooltipName), out string setButtonValue))
                tooltipName = setButtonValue;

            // set the ability properties
            if (!string.IsNullOrEmpty(referenceName) && !string.IsNullOrEmpty(tooltipName))
            {
                ability.ReferenceNameId = referenceName;
                ability.FullTooltipNameId = tooltipName;
                ability.ButtonName = tooltipName;
            }
            else if (!string.IsNullOrEmpty(referenceName) && string.IsNullOrEmpty(tooltipName)) // is a secondary ability
            {
                ability.ReferenceNameId = referenceName;
                ability.ParentLink = parentLink;
                ability.FullTooltipNameId = referenceName;
                ability.ButtonName = referenceName;
            }
            else
            {
                ability.ReferenceNameId = tooltipName;
                ability.FullTooltipNameId = tooltipName;
                ability.ButtonName = tooltipName;
            }

            XElement heroicElement = abilityElement.Elements("Flags").FirstOrDefault(x => x.Attribute("index").Value == "Heroic" && x.Attribute("value").Value == "1");
            XElement traitElement = abilityElement.Elements("Flags").FirstOrDefault(x => x.Attribute("index").Value == "Trait" && x.Attribute("value").Value == "1");
            XElement mountElement = abilityElement.Elements("Flags").FirstOrDefault(x => x.Attribute("index").Value == "MountReplacement" && x.Attribute("value").Value == "1");
            XElement activableElement = GameData.Elements("CItemAbil").FirstOrDefault(x => x.Attribute("id")?.Value == ability.ReferenceNameId);

            if (heroicElement != null)
                ability.Tier = AbilityTier.Heroic;
            else if (traitElement != null)
                ability.Tier = AbilityTier.Trait;
            else if (mountElement != null)
                ability.Tier = AbilityTier.Mount;
            else if (activableElement != null)
                ability.Tier = AbilityTier.Activable;
            else
                ability.Tier = AbilityTier.Basic;

            // set button related data
            XElement cButtonElement = GetButtonElement(ability.FullTooltipNameId);

            if (cButtonElement != null)
            {
                SetTooltipCostData(ability.ReferenceNameId, ability);
                SetTooltipDescriptions(ability);
                SetTooltipOverrideData(cButtonElement, ability);
                SetTalentIdUpgrades(cButtonElement, ability);
            }

            SetAbilityType(hero, ability);

            if (AbilityType.Misc.HasFlag(ability.AbilityType))
                return;

            // add ability
            if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
            {
                if (hero.Abilities.Count >= 3 && ability.AbilityType == AbilityType.Active)
                    ability.Tier = AbilityTier.Activable;

                hero.Abilities.Add(ability.ReferenceNameId, ability);
            }
            else if (HeroDataOverride.AddedAbilityByAbilityId.TryGetValue(ability.ReferenceNameId, out var addedAbility))
            {
                // overridden add ability
                if (addedAbility.IsAdded)
                {
                    ability.ReferenceNameId = addedAbility.ButtonName;

                    // attempt to re-add
                    if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
                        hero.Abilities.Add(ability.ReferenceNameId, ability);
                }
            }
        }

        /// <summary>
        /// Adds a unit's ability data from the ability xml element.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="abilityElement">The AbilArray xml element.</param>
        public void AddUnitAbility(Unit unit, XElement abilityElement)
        {
            string abilLink = abilityElement.Attribute("Link")?.Value;
            if (string.IsNullOrEmpty(abilLink))
                return;

            XElement buttonElement = GetButtonElement(abilLink);
            if (buttonElement == null)
                return;

            string buttonId = buttonElement.Attribute("id").Value;

            Ability ability = new Ability()
            {
                FullTooltipNameId = string.IsNullOrEmpty(buttonId) ? abilLink : buttonId,
                ButtonName = string.IsNullOrEmpty(buttonId) ? abilLink : buttonId,
                ReferenceNameId = abilLink,
            };

            // default
            ability.Tier = AbilityTier.Activable;

            SetAbilityType(unit, ability);

            if (AbilityType.Misc.HasFlag(ability.AbilityType))
                return;

            SetAbilityTierFromAbilityType(ability);
            SetTooltipCostData(ability.ReferenceNameId, ability);
            SetTooltipDescriptions(ability);
            SetTooltipOverrideData(buttonElement, ability);

            // add ability
            if (!unit.Abilities.ContainsKey(ability.ReferenceNameId))
                unit.Abilities.Add(ability.ReferenceNameId, ability);
        }

        /// <summary>
        /// For adding additional abilities that are from the AbilArray element.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="id"></param>
        /// <param name="elementName"></param>
        public void AddLinkedAbility(Unit unit, string id, string elementName)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(elementName))
                return;

            Ability ability = new Ability();
            unit.Abilities = unit.Abilities ?? new Dictionary<string, Ability>();

            XElement abilityElement = GameData.MergeXmlElements(GameData.Elements(elementName).Where(x => x.Attribute("id")?.Value == id));

            if (abilityElement == null)
                throw new XmlGameDataParseException($"{nameof(AddLinkedAbility)}: Additional link ability element not found - <{elementName} id=\"{id}\">");

            ability.ReferenceNameId = id;
            ability.FullTooltipNameId = id;

            if (GameData.TryGetGameString(DefaultData.ButtonData.ButtonName.Replace(DefaultData.IdPlaceHolder, id), out string abilityName))
                ability.Name = abilityName;

            XElement cButtonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == ability.FullTooltipNameId));

            if (cButtonElement != null)
            {
                SetTooltipCostData(id, ability);
                SetTooltipDescriptions(ability);
                SetTooltipOverrideData(cButtonElement, ability);
            }

            // add to abilities
            if (!unit.Abilities.ContainsKey(ability.ReferenceNameId))
                unit.Abilities.Add(ability.ReferenceNameId, ability);
        }

        /// <summary>
        /// Adds additional abilities from the overrides file.
        /// </summary>
        /// <param name="unit"></param>
        public void AddOverrideButtonAbilities(Unit unit)
        {
            unit.Abilities = unit.Abilities ?? new Dictionary<string, Ability>();

            foreach (AddedButtonAbility abilityButton in UnitDataOverride.AddedAbilityByButtonId)
            {
                Ability ability = new Ability()
                {
                    FullTooltipNameId = abilityButton.ButtonId,
                    ButtonName = abilityButton.ButtonId,
                    ReferenceNameId = abilityButton.ButtonId,
                };

                if (!string.IsNullOrEmpty(abilityButton.ReferenceNameId))
                    ability.ReferenceNameId = abilityButton.ReferenceNameId;

                // default
                ability.Tier = AbilityTier.Activable;

                XElement cButtonElement = null;
                if (!string.IsNullOrEmpty(abilityButton.ReferenceNameId))
                    cButtonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == ability.ReferenceNameId && x.Attribute("parent")?.Value == abilityButton.ParentValue));

                cButtonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == ability.FullTooltipNameId && x.Attribute("parent")?.Value == abilityButton.ParentValue)) ?? cButtonElement;

                if (cButtonElement == null)
                    throw new XmlGameDataParseException($"Could not find the following element <CButton id=\"{ability.FullTooltipNameId}\" parent=\"{abilityButton.ParentValue}\">");

                SetAbilityType(unit, ability);

                SetAbilityTierFromAbilityType(ability);
                SetTooltipCostData(ability.ReferenceNameId, ability);
                SetTooltipDescriptions(ability);
                SetTooltipOverrideData(cButtonElement, ability);

                unit.Abilities[ability.ReferenceNameId] = ability;
            }
        }

        private void SetAbilityType(Unit unit, Ability ability)
        {
            if (ability.Tier == AbilityTier.Heroic)
            {
                ability.AbilityType = AbilityType.Heroic;
                return;
            }
            else if (ability.Tier == AbilityTier.Mount)
            {
                ability.AbilityType = AbilityType.Z;
                return;
            }
            else if (ability.Tier == AbilityTier.Trait)
            {
                ability.AbilityType = AbilityType.Trait;
                return;
            }
            else if (ability.Tier == AbilityTier.Hearth)
            {
                ability.AbilityType = AbilityType.B;
                return;
            }

            string layoutId = !string.IsNullOrEmpty(ability.ParentLink) ? ability.ParentLink : unit.CUnitId;

            if (!SetAbilityTypeFromLayout(unit, ability, GameData.GetLayoutButtonElements(layoutId)) && !SetAbilityTypeFromLayout(unit, ability, GameData.GetLayoutButtonElements()))
                ability.AbilityType = AbilityType.Active;
        }

        private void SetAbilityTypeFromSlot(string slot, Unit unit, Ability ability)
        {
            if (slot.AsSpan().StartsWith("ABILITY1", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.Q;
            else if (slot.AsSpan().StartsWith("ABILITY2", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.W;
            else if (slot.AsSpan().StartsWith("ABILITY3", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.E;
            else if (slot.AsSpan().StartsWith("MOUNT", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.Z;
            else if (slot.AsSpan().StartsWith("HEROIC", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.Heroic;
            else if (slot.AsSpan().StartsWith("HEARTH", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.B;
            else if (slot.AsSpan().StartsWith("TRAIT", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.Trait;
            else if (Enum.TryParse(slot, true, out AbilityType abilityType))
                ability.AbilityType = abilityType;
            else
                throw new XmlGameDataParseException($"Unknown slot type ({slot}) - CUnit: {unit.CUnitId} - Ability: {ability.ReferenceNameId}");
        }

        private void SetTalentIdUpgrades(XElement buttonElement, Ability ability)
        {
            foreach (XElement tooltipAppenderElement in buttonElement.Elements("TooltipAppender"))
            {
                string talentId = tooltipAppenderElement.Attribute("Face")?.Value;

                if (!string.IsNullOrEmpty(talentId))
                {
                    ability.TalentIdUpgrades.Add(talentId);
                }
            }
        }

        private void SetAbilityTierFromAbilityType(Ability ability)
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
        }

        private XElement GetButtonElement(string buttonId)
        {
            // search for CButton
            XElement buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == buttonId));
            if (buttonElement != null)
                return buttonElement;

            // if not found search for CAbilEffectTarget
            XElement abilEffectTargetElement = GetCAbilEffectTargetElement(GameData.MergeXmlElements(GameData.Elements("CAbilEffectTarget").Where(x => x.Attribute("id")?.Value == buttonId)));
            if (abilEffectTargetElement != null)
            {
                string buttonFaceValue = abilEffectTargetElement.Element("CmdButtonArray")?.Attribute("DefaultButtonFace")?.Value;
                if (!string.IsNullOrEmpty(buttonFaceValue))
                {
                    buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == buttonFaceValue));
                    if (buttonElement != null)
                        return buttonElement;
                }
            }

            return null;
        }

        private XElement GetCAbilEffectTargetElement(XElement cAbilEffectTargetElement)
        {
            if (cAbilEffectTargetElement == null)
                return null;

            XElement currentElement = new XElement(cAbilEffectTargetElement);

            List<XElement> elementsList = new List<XElement>
            {
                cAbilEffectTargetElement,
            };

            // add all the parent elements to the list
            string parentValue = currentElement.Attribute("parent")?.Value;

            while (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements("CAbilEffectTarget").Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                {
                    elementsList.Add(parentElement);
                    currentElement = parentElement;

                    parentValue = currentElement.Attribute("parent")?.Value;
                }
            }

            // merge them
            return GameData.MergeXmlElementsNoAttributes(elementsList);
        }

        // searches the
        private bool SetAbilityTypeFromLayout(Unit unit, Ability ability, ICollection<XElement> layoutButtonElements)
        {
            // check the face attribute
            XElement layoutButton = layoutButtonElements?.FirstOrDefault(x => (x.Attribute("Face")?.Value == ability.ButtonName || x.Attribute("Face")?.Value == ability.ReferenceNameId) &&
                x.Attribute("Slot")?.Value != "Cancel" && x.Attribute("Slot")?.Value != "Hearth");

            // check the abilcmd attribute
            if (layoutButton == null)
            {
                layoutButton = layoutButtonElements?.FirstOrDefault(x => !string.IsNullOrEmpty(x.Attribute("AbilCmd")?.Value) && (x.Attribute("AbilCmd").Value.StartsWith(ability.ButtonName) ||
                    x.Attribute("AbilCmd").Value.StartsWith(ability.ReferenceNameId)) && x.Attribute("Slot")?.Value != "Cancel" && x.Attribute("Slot")?.Value != "Hearth");
            }

            if (layoutButton != null)
            {
                SetAbilityTypeFromSlot(layoutButton.Attribute("Slot").Value, unit, ability);
                return true;
            }
            else // was not found for attributes check as elements
            {
                // check the face element value
                layoutButton = layoutButtonElements?.Where(x => x.HasElements).FirstOrDefault(x => (x.Element("Face")?.Attribute("value")?.Value == ability.ButtonName || x.Element("Face")?.Attribute("value")?.Value == ability.ReferenceNameId) &&
                    x.Element("Slot")?.Attribute("value")?.Value != "Cancel" && x.Element("Slot")?.Attribute("value")?.Value != "Hearth");

                // check the abilcmd element value
                if (layoutButton == null)
                {
                    layoutButton = layoutButtonElements?.Where(x => x.HasElements).FirstOrDefault(x => (!string.IsNullOrEmpty(x.Element("AbilCmd")?.Attribute("value")?.Value) && (x.Element("AbilCmd").Attribute("value").Value.StartsWith(ability.ButtonName) || x.Element("AbilCmd").Attribute("value").Value.StartsWith(ability.ReferenceNameId)) &&
                        x.Element("Slot")?.Attribute("value")?.Value != "Cancel" && x.Element("Slot")?.Attribute("value")?.Value != "Hearth"));
                }

                if (layoutButton != null)
                {
                    SetAbilityTypeFromSlot(layoutButton.Element("Slot").Attribute("value").Value, unit, ability);

                    return true;
                }
            }

            return false;
        }
    }
}
