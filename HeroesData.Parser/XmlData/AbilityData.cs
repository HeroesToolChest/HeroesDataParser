using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Exceptions;
using HeroesData.Parser.Overrides.DataOverrides;
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

        public void SetAbilityData(Hero hero, XElement abilityElement)
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
            XElement cButtonElement = GameData.MergeXmlElements(GameData.CButtonElements.Where(x => x.Attribute("id")?.Value == ability.FullTooltipNameId));

            if (cButtonElement != null)
            {
                SetTooltipCostData(hero, ability.ReferenceNameId, ability);
                SetTooltipDescriptions(ability);
                SetTooltipOverrideData(cButtonElement, ability);
                SetTalentIdUpgrades(cButtonElement, ability);
            }

            SetAbilityType(hero, ability);

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
        /// For adding additional abilities that are from the AbilArray element.
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="id"></param>
        /// <param name="elementName"></param>
        public void SetLinkedAbility(Hero hero, string id, string elementName)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(elementName))
                return;

            Ability ability = new Ability();
            hero.Abilities = hero.Abilities ?? new Dictionary<string, Ability>();

            XElement abilityElement = GameData.MergeXmlElements(GameData.XmlGameData.Root.Elements(elementName).Where(x => x.Attribute("id")?.Value == id));

            if (abilityElement == null)
                throw new ParseException($"{nameof(SetLinkedAbility)}: Additional link ability element not found - <{elementName} id=\"{id}\">");

            ability.ReferenceNameId = id;
            ability.FullTooltipNameId = id;

            if (GameData.TryGetGameString(DefaultData.ButtonName.Replace(DefaultData.IdPlaceHolder, id), out string abilityName))
                ability.Name = abilityName;

            XElement cButtonElement = GameData.MergeXmlElements(GameData.CButtonElements.Where(x => x.Attribute("id")?.Value == ability.FullTooltipNameId));

            if (cButtonElement != null)
            {
                SetTooltipCostData(hero, id, ability);
                SetTooltipDescriptions(ability);
                SetTooltipOverrideData(cButtonElement, ability);
            }

            // add to abilities
            if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
                hero.Abilities.Add(ability.ReferenceNameId, ability);
        }

        public void AddAdditionalButtonAbilities(Hero hero)
        {
            hero.Abilities = hero.Abilities ?? new Dictionary<string, Ability>();

            foreach ((string buttonId, string parent) in HeroDataOverride.AddedAbilityByButtonId)
            {
                Ability ability = new Ability()
                {
                    FullTooltipNameId = buttonId,
                    ButtonName = buttonId,
                    ReferenceNameId = buttonId,
                };

                // default
                ability.Tier = AbilityTier.Activable;

                // LastOrDefault, since overrides can happen in later xml files
                XElement cButtonElement = GameData.MergeXmlElements(GameData.CButtonElements.Where(x => x.Attribute("id")?.Value == ability.FullTooltipNameId && x.Attribute("parent")?.Value == parent));

                if (cButtonElement == null)
                    throw new ParseException($"Could not find the following element <CButton id=\"{ability.FullTooltipNameId}\" parent=\"{parent}\">");

                SetAbilityType(hero, ability);
                SetAbilityTierFromAbilityType(hero, ability);
                SetTooltipCostData(hero, ability.ReferenceNameId, ability);
                SetTooltipDescriptions(ability);
                SetTooltipOverrideData(cButtonElement, ability);

                // add ability
                if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
                {
                    hero.Abilities.Add(ability.ReferenceNameId, ability);
                }
            }
        }

        private void SetAbilityType(Hero hero, Ability ability)
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

            // as attributes
            XElement layoutButton = GameData.LayoutButtonElements.FirstOrDefault(x => (x.Attribute("Face")?.Value == ability.ButtonName || x.Attribute("Face")?.Value == ability.ReferenceNameId) && x.Attribute("Slot")?.Value != "Cancel" && x.Attribute("Slot")?.Value != "Hearth");
            if (layoutButton != null)
            {
                string slot = layoutButton.Attribute("Slot").Value;
                SetAbilityTypeFromSlot(slot, hero, ability);

                return;
            }
            else // as elements
            {
                layoutButton = GameData.LayoutButtonElements.Where(x => x.HasElements).FirstOrDefault(x => (x.Element("Face")?.Attribute("value")?.Value == ability.ButtonName || x.Element("Face")?.Attribute("value")?.Value == ability.ReferenceNameId) &&
                                                        x.Element("Slot")?.Attribute("value")?.Value != "Cancel" && x.Element("Slot")?.Attribute("value")?.Value != "Hearth");

                if (layoutButton != null)
                {
                    string slot = layoutButton.Element("Slot").Attribute("value").Value;
                    SetAbilityTypeFromSlot(slot, hero, ability);

                    return;
                }
            }

            ability.AbilityType = AbilityType.Active;
        }

        private void SetAbilityTypeFromSlot(string slot, Hero hero, Ability ability)
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
            else if (slot.ToUpper().StartsWith("HEARTH"))
                ability.AbilityType = AbilityType.B;
            else if (slot.ToUpper().StartsWith("TRAIT"))
                ability.AbilityType = AbilityType.Trait;
            else
                throw new ParseException($"Unknown slot type ({slot}) - Hero(CUnit): {hero.CUnitId} - Ability: {ability.ReferenceNameId}");
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

        private void SetAbilityTierFromAbilityType(Hero hero, Ability ability)
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
    }
}
