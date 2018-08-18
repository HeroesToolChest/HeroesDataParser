using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Parser.Exceptions;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Data
{
    public class AbilityData : AbilityTalentData
    {
        public AbilityData(GameData gameData, HeroOverride heroOverride, ParsedGameStrings parsedGameStrings, TextValueData textValueData)
            : base(gameData, heroOverride, parsedGameStrings, textValueData)
        {
        }

        public void SetAbilityData(Hero hero, XElement abilityElement, IEnumerable<XElement> layoutButtons)
        {
            hero.Abilities = hero.Abilities ?? new Dictionary<string, Ability>();

            string referenceName = abilityElement.Attribute("Abil")?.Value;
            string tooltipName = abilityElement.Attribute("Button")?.Value;
            string parentLink = abilityElement.Attribute("Unit")?.Value;

            XElement usableAbility = abilityElement.Elements("Flags").Where(x => x.Attribute("index").Value == "ShowInHeroSelect" && x.Attribute("value").Value == "1").FirstOrDefault();
            XElement mountAbility = abilityElement.Elements("Flags").Where(x => x.Attribute("index").Value == "MountReplacement" && x.Attribute("value").Value == "1").FirstOrDefault();

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

            XElement heroicElement = abilityElement.Elements("Flags").Where(x => x.Attribute("index").Value == "Heroic" && x.Attribute("value").Value == "1").FirstOrDefault();
            XElement traitElement = abilityElement.Elements("Flags").Where(x => x.Attribute("index").Value == "Trait" && x.Attribute("value").Value == "1").FirstOrDefault();
            XElement mountElement = abilityElement.Elements("Flags").Where(x => x.Attribute("index").Value == "MountReplacement" && x.Attribute("value").Value == "1").FirstOrDefault();
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
            XElement cButtonElement = GameData.XmlGameData.Root.Elements("CButton").Where(x => x.Attribute("id")?.Value == ability.FullTooltipNameId).FirstOrDefault();

            if (cButtonElement != null)
            {
                SetAbilityTalentName(cButtonElement, ability);
                SetAbilityTalentIcon(cButtonElement, ability);
                SetTooltipSubInfo(hero, ability.ReferenceNameId, ability);
                SetTooltipDescriptions(cButtonElement, hero, ability);
            }

            SetAbilityType(hero, ability, layoutButtons);

            // add ability
            if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
            {
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

            XElement abilityElement = GameData.XmlGameData.Root.Elements(elementName).Where(x => x.Attribute("id")?.Value == idValue).FirstOrDefault();

            if (abilityElement == null)
                throw new ParseException($"{nameof(SetLinkedAbility)}: Additional link ability element not found - <{elementName} id=\"{idValue}\">");

            string linkName = abilityElement.Attribute("id")?.Value;
            if (linkName == null)
                return;

            ability.ReferenceNameId = linkName;
            ability.FullTooltipNameId = linkName;

            if (ParsedGameStrings.TryGetAbilityTalentParsedNames(linkName, out string abilityName))
                ability.Name = abilityName;

            XElement cButtonElement = GameData.XmlGameData.Root.Elements("CButton").Where(x => x.Attribute("id")?.Value == ability.FullTooltipNameId).FirstOrDefault();

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
            XElement layoutButton = layoutButtons.Where(x => x.Attribute("Face")?.Value == ability.ButtonName && x.Attribute("Slot")?.Value != "Cancel" && x.Attribute("Slot")?.Value != "Hearth").FirstOrDefault();
            if (layoutButton != null)
            {
                string slot = layoutButton.Attribute("Slot").Value;
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
                    throw new ParseException($"Unknown slot type (attribute) for ability type: {slot} - Hero(CUnit): {hero.CUnitId} - Ability: {ability.ReferenceNameId}");

                return;
            }

            // as elements
            if (layoutButton == null)
            {
                layoutButton = layoutButtons.Where(x => x.HasElements)
                     .Where(x => x.Element("Face")?.Attribute("value")?.Value == ability.ButtonName && x.Element("Slot")?.Attribute("value")?.Value != "Cancel" && x.Element("Slot")?.Attribute("value")?.Value != "Hearth").FirstOrDefault();
            }

            if (layoutButton != null)
            {
                string slot = layoutButton.Element("Slot").Attribute("value").Value;
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
                    throw new ParseException($"Unknown slot type (element) for ability type: {slot} - Hero(CUnit): {hero.CUnitId} - Ability: {ability.ReferenceNameId}");

                return;
            }

            ability.AbilityType = AbilityType.Active;
        }
    }
}
