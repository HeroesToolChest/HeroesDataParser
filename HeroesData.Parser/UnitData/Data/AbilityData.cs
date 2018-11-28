using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Exceptions;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.UnitData.Overrides;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Data
{
    public class AbilityData : AbilityTalentData
    {
        public AbilityData(GameData gameData, HeroOverride heroOverride, ParsedGameStrings parsedGameStrings, TextValueData textValueData, Localization localization)
            : base(gameData, heroOverride, parsedGameStrings, textValueData, localization)
        {
        }

        public void SetAbilityData(Hero hero, XElement abilityElement, IEnumerable<XElement> layoutButtons)
        {
            hero.Abilities = hero.Abilities ?? new Dictionary<string, Ability>();

            string referenceName = abilityElement.Attribute("Abil")?.Value;
            string tooltipName = abilityElement.Attribute("Button")?.Value;
            string parentLink = abilityElement.Attribute("Unit")?.Value;

            XElement usableAbility = abilityElement.Elements("Flags").FirstOrDefault(x => x.Attribute("index").Value == "ShowInHeroSelect" && x.Attribute("value").Value == "1");
            XElement mountAbility = abilityElement.Elements("Flags").FirstOrDefault(x => x.Attribute("index").Value == "MountReplacement" && x.Attribute("value").Value == "1");

            Ability ability = new Ability();

            if (!string.IsNullOrEmpty(referenceName) && HeroOverride.IsValidAbilityByAbilityId.TryGetValue(referenceName, out bool validAbility))
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
            if (!string.IsNullOrEmpty(tooltipName) && HeroOverride.NewButtonValueByHeroAbilArrayButton.TryGetValue(tooltipName, out string setButtonValue))
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
            XElement activableElement = GameData.XmlGameData.Root.Elements("CItemAbil").FirstOrDefault(x => x.Attribute("id")?.Value == ability.ReferenceNameId);

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
            XElement cButtonElement = GameData.XmlGameData.Root.Elements("CButton").FirstOrDefault(x => x.Attribute("id")?.Value == ability.FullTooltipNameId);

            if (cButtonElement != null)
            {
                SetAbilityTalentName(cButtonElement, ability);
                SetAbilityTalentIcon(cButtonElement, ability);
                SetTooltipSubInfo(hero, ability.ReferenceNameId, ability);
                SetTooltipDescriptions(cButtonElement, hero, ability);
                SetTalentIdUpgrades(cButtonElement, ability);
            }

            SetAbilityType(hero, ability, layoutButtons);

            // add ability
            if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
            {
                if (hero.Abilities.Count >= 3 && ability.AbilityType == AbilityType.Active)
                    ability.Tier = AbilityTier.Activable;

                hero.Abilities.Add(ability.ReferenceNameId, ability);
            }
            else if (HeroOverride.AddedAbilitiesByAbilityId.TryGetValue(ability.ReferenceNameId, out var addedAbility))
            {
                // overridden add ability
                if (addedAbility.Add)
                {
                    ability.ReferenceNameId = addedAbility.Button;

                    // attempt to re-add
                    if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
                        hero.Abilities.Add(ability.ReferenceNameId, ability);
                }
            }
        }

        public void SetLinkedAbility(Hero hero, string idValue, string elementName)
        {
            Ability ability = new Ability();
            hero.Abilities = hero.Abilities ?? new Dictionary<string, Ability>();

            XElement abilityElement = GameData.XmlGameData.Root.Elements(elementName).FirstOrDefault(x => x.Attribute("id")?.Value == idValue);

            if (abilityElement == null)
                throw new ParseException($"{nameof(SetLinkedAbility)}: Additional link ability element not found - <{elementName} id=\"{idValue}\">");

            string linkName = abilityElement.Attribute("id")?.Value;
            if (linkName == null)
                return;

            ability.ReferenceNameId = linkName;
            ability.FullTooltipNameId = linkName;

            if (ParsedGameStrings.TryGetAbilityTalentParsedNames(linkName, out string abilityName))
                ability.Name = abilityName;

            XElement cButtonElement = GameData.XmlGameData.Root.Elements("CButton").FirstOrDefault(x => x.Attribute("id")?.Value == ability.FullTooltipNameId);

            if (cButtonElement != null)
            {
                SetAbilityTalentIcon(cButtonElement, ability);
                SetTooltipSubInfo(hero, linkName, ability);
                SetTooltipDescriptions(cButtonElement, hero, ability);
            }

            // add to abilities
            if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
                hero.Abilities.Add(ability.ReferenceNameId, ability);
        }

        private void SetAbilityType(Hero hero, Ability ability, IEnumerable<XElement> layoutButtons)
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

            // as attributes
            XElement layoutButton = layoutButtons.FirstOrDefault(x => (x.Attribute("Face")?.Value == ability.ButtonName || x.Attribute("Face")?.Value == ability.ReferenceNameId) &&
                                                                      x.Attribute("Slot")?.Value != "Cancel" && x.Attribute("Slot")?.Value != "Hearth");
            if (layoutButton != null)
            {
                string slot = layoutButton.Attribute("Slot").Value;
                SetAbilityTypeFromSlot(slot, hero, ability, "attribute");

                return;
            }
            else // as elements
            {
                layoutButton = layoutButtons.Where(x => x.HasElements)
                     .FirstOrDefault(x => (x.Element("Face")?.Attribute("value")?.Value == ability.ButtonName || x.Element("Face")?.Attribute("value")?.Value == ability.ReferenceNameId) &&
                                     x.Element("Slot")?.Attribute("value")?.Value != "Cancel" && x.Element("Slot")?.Attribute("value")?.Value != "Hearth");

                if (layoutButton != null)
                {
                    string slot = layoutButton.Element("Slot").Attribute("value").Value;
                    SetAbilityTypeFromSlot(slot, hero, ability, "element");

                    return;
                }
            }

            ability.AbilityType = AbilityType.Active;
        }

        private void SetAbilityTypeFromSlot(string slot, Hero hero, Ability ability, string type)
        {
            if (slot.ToUpper().StartsWith("ABILITY1"))
                ability.AbilityType = AbilityType.Q;
            else if (slot.ToUpper().StartsWith("ABILITY2"))
                ability.AbilityType = AbilityType.W;
            else if (slot.ToUpper().StartsWith("ABILITY3"))
                ability.AbilityType = AbilityType.E;
            else if (slot.ToUpper().StartsWith("MOUNT"))
                ability.AbilityType = AbilityType.Z;
            else if (slot.ToUpper().StartsWith("HEROIC"))
                ability.AbilityType = AbilityType.Heroic;
            else
                throw new ParseException($"Unknown slot type ({type}) for ability type: {slot} - Hero(CUnit): {hero.CUnitId} - Ability: {ability.ReferenceNameId}");
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
    }
}
