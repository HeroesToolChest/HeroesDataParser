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
        public AbilityData(GameData gameData, DefaultData defaultData, Configuration configuration)
        : base(gameData, defaultData, configuration)
        {
        }

        public bool IsAbilityTypeFilterEnabled { get; set; } = false;
        public bool IsAbilityTierFilterEnabled { get; set; } = false;

        /// <summary>
        /// Adds hero's ability data from the ability xml element.
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="abilityElement">The HeroAbilArray xml element.</param>
        public void AddHeroAbility(Hero hero, XElement abilityElement)
        {
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
            //XElement cButtonElement = GetButtonElement(ability.FullTooltipNameId);

            //if (cButtonElement != null)
            //{
            //    SetTooltipCostData(ability.ReferenceNameId, ability);
            //    SetTooltipDescriptions(ability);
            //    SetTooltipOverrideData(cButtonElement, ability);
            //    SetTalentIdUpgrades(cButtonElement, ability);
            //}

           // SetAbilityType(hero, ability);

            if (AbilityType.Misc.HasFlag(ability.AbilityType))
                return;

            // add ability
            //if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
            //{
            //    if (hero.Abilities.Count >= 3 && ability.AbilityType == AbilityType.Active)
            //        ability.Tier = AbilityTier.Activable;

            //    hero.Abilities.Add(ability.ReferenceNameId, ability);
            //}
            //else if (HeroDataOverride.AddedAbilityByAbilityId.TryGetValue(ability.ReferenceNameId, out var addedAbility))
            //{
            //    // overridden add ability
            //    if (addedAbility.IsAdded)
            //    {
            //        ability.ReferenceNameId = addedAbility.ButtonName;

            //        // attempt to re-add
            //        if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
            //            hero.Abilities.Add(ability.ReferenceNameId, ability);
            //    }
            //}
        }

        /// <summary>
        /// Creates an ability from the layout button element.
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="layoutButtonElement"></param>
        /// <returns></returns>
        public Ability CreateAbility(string unitId, XElement layoutButtonElement)
        {
            if (string.IsNullOrEmpty(unitId) || layoutButtonElement == null)
                throw new ArgumentNullException();

            Ability ability = new Ability();

            string faceValue = layoutButtonElement.Attribute("Face")?.Value; // button id
            string typeValue = layoutButtonElement.Attribute("Type")?.Value;
            string abilCmdValue = layoutButtonElement.Attribute("AbilCmd")?.Value; // the ability command
            string requirementsValue = layoutButtonElement.Attribute("Requirements")?.Value;
            string slotValue = layoutButtonElement.Attribute("Slot")?.Value;

            if (string.IsNullOrEmpty(faceValue) || slotValue == "Hidden2")
                return null;

            // default button id values
            ability.FullTooltipNameId = faceValue;
            ability.ShortTooltipNameId = faceValue;

            // default tier
            ability.Tier = AbilityTier.Unknown;

            if (typeValue.AsSpan().Equals("Passive", StringComparison.OrdinalIgnoreCase)) // passive "ability"
            {
                ability.ReferenceNameId = faceValue;
                ability.IsPassive = true;

                XElement buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == faceValue));
                if (buttonElement != null)
                    SetButtonData(buttonElement, ability);
            }
            else if (typeValue.AsSpan().Equals("CancelTargetMode", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            else if (!string.IsNullOrEmpty(abilCmdValue)) // non-passive
            {
                int index = abilCmdValue.IndexOf(',');
                string abilityId;
                if (index > 0)
                    abilityId = abilCmdValue.Substring(0, index);
                else
                    abilityId = abilCmdValue;

                ability.ReferenceNameId = abilityId;

                // find all abilities and loop through them
                foreach (XElement element in GetAbilityElements(abilityId))
                {
                    SetAbilityTalentData(element, ability);
                }
            }
            else if (string.IsNullOrEmpty(abilCmdValue))
            {
                // doesn't have an abilcmd value set, so this is just a dummy button that doesn't do anything
                // most likely still has an ability set in the ability array but wasn't set for the abilcmd value
                return null;
            }
            else
            {
                throw new XmlGameDataParseException($"Could not create ability for {unitId}, no abilCmdValue: faceValue: {faceValue} - typeValue: {typeValue}");
            }

            // set the ability type
            SetAbilityTypeFromSlot(slotValue, unitId, ability);

            // if type is not a type we want, return
            if (IsAbilityTypeFilterEnabled && AbilityType.Misc.HasFlag(ability.AbilityType) && ability.AbilityType != AbilityType.Unknown)
                return null;

            // set the ability tier
            SetAbilityTierFromAbilityType(ability);

            // filter ability tiers
            if ((IsAbilityTierFilterEnabled && AbilityTier.Misc.HasFlag(ability.Tier)) || (IsAbilityTierFilterEnabled && ability.Tier == AbilityTier.MapMechanic))
                return null;

            // if no reference id, return null
            if (string.IsNullOrEmpty(ability.ReferenceNameId))
                return null;

            // if no name was set, then use the ability name
            if (string.IsNullOrEmpty(ability.Name) && GameData.TryGetGameString(DefaultData.AbilData.AbilName.Replace(DefaultData.IdPlaceHolder, ability.ReferenceNameId), out string value))
                ability.Name = value;

            return ability;
        }

        /// <summary>
        /// Creates an ability from the abil link value. This ability does not have a command card layout button.
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="abilityId"></param>
        /// <returns></returns>
        public Ability CreateAbility(string unitId, string abilityId)
        {
            if (string.IsNullOrEmpty(unitId) || string.IsNullOrEmpty(abilityId))
                throw new ArgumentNullException();

            Ability ability = new Ability()
            {
                ReferenceNameId = abilityId,
            };

            // find all abilities and loop through them
            foreach (XElement element in GetAbilityElements(abilityId))
            {
                SetAbilityTalentData(element, ability);
            }

            // default
            ability.Tier = AbilityTier.Hidden;
            ability.AbilityType = AbilityType.Hidden;

            // if no reference id, return null
            if (string.IsNullOrEmpty(ability.ReferenceNameId))
                return null;

            // if no name was set, then use the ability name
            if (string.IsNullOrEmpty(ability.Name) && GameData.TryGetGameString(DefaultData.AbilData.AbilName.Replace(DefaultData.IdPlaceHolder, ability.ReferenceNameId), out string value))
                ability.Name = value;

            return ability;
        }

        ///// <summary>
        ///// Create an ability.
        ///// </summary>
        ///// <param name="unitId">The id of the unit.</param>
        ///// <param name="abilityId">The ability id.</param>
        //public Ability CreateAbility(string unitId, string abilityId)
        //{
        //    if (string.IsNullOrEmpty(abilityId))
        //        return null;

        //    // abilities that we don't want
        //    if (abilityId == "move" || abilityId == "stop" || abilityId == "attack" || abilityId == "Queue5Storm" || abilityId == "Que1Passive")
        //        return null;

        //    // check if there's a passive ability associated with the current ability
        //    if (HasPassiveAbility(unitId, abilityId))
        //        return null;

        //    IEnumerable<XElement> abilityElements = GetAbilityElements(abilityId);

        //    Ability ability = new Ability
        //    {
        //        ReferenceNameId = abilityId,
        //    };

        //    if (abilityElements.Any())
        //    {
        //        foreach (XElement element in abilityElements)
        //        {
        //            SetAbilityTalentData(element, ability);
        //        }
        //    }
        //    else
        //    {
        //        XElement buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == abilityId));

        //        if (buttonElement != null)
        //            SetButtonData(buttonElement, ability);
        //    }

        //    // default
        //    ability.Tier = AbilityTier.Unknown;

        //    // set the ability type
        //    SetAbilityType(unitId, ability);

        //    // if type is not a type we want, return
        //    if (IsAbilityTypeFilterEnabled && AbilityType.Misc.HasFlag(ability.AbilityType) && ability.AbilityType != AbilityType.Unknown)
        //        return null;

        //    // set the ability tier
        //    SetAbilityTierFromAbilityType(ability);

        //    // filter ability tiers
        //    if ((IsAbilityTierFilterEnabled && AbilityTier.Misc.HasFlag(ability.Tier)) || (IsAbilityTierFilterEnabled && ability.Tier == AbilityTier.MapMechanic))
        //        return null;

        //    // if no reference id, return null
        //    if (string.IsNullOrEmpty(ability.ReferenceNameId))
        //        return null;

        //    // if no name was set, then use the ability name
        //    if (string.IsNullOrEmpty(ability.Name) && GameData.TryGetGameString(DefaultData.AbilData.AbilName.Replace(DefaultData.IdPlaceHolder, ability.ReferenceNameId), out string value))
        //        ability.Name = value;

        //    return ability;
        //}

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
            //unit.Abilities = unit.Abilities ?? new Dictionary<string, Ability>();

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

            //// add to abilities
            //if (!unit.Abilities.ContainsKey(ability.ReferenceNameId))
            //    unit.Abilities.Add(ability.ReferenceNameId, ability);
        }

        /// <summary>
        /// Adds additional abilities from the overrides file.
        /// </summary>
        /// <param name="unit"></param>
        public Ability CreateOverrideButtonAbility(AddedButtonAbility addedButtonAbility)
        {
            Ability ability = new Ability()
            {
                FullTooltipNameId = addedButtonAbility.ButtonId,
                ButtonName = addedButtonAbility.ButtonId,
                ReferenceNameId = addedButtonAbility.ButtonId,
            };

            if (!string.IsNullOrEmpty(addedButtonAbility.ReferenceNameId))
                ability.ReferenceNameId = addedButtonAbility.ReferenceNameId;

            // default
            ability.Tier = AbilityTier.Activable;

            XElement cButtonElement = null;
            if (!string.IsNullOrEmpty(addedButtonAbility.ReferenceNameId))
                cButtonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == ability.ReferenceNameId && x.Attribute("parent")?.Value == addedButtonAbility.ParentValue));

            cButtonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == ability.FullTooltipNameId && x.Attribute("parent")?.Value == addedButtonAbility.ParentValue)) ?? cButtonElement;

            if (cButtonElement == null)
                throw new XmlGameDataParseException($"Could not find the following element <CButton id=\"{ability.FullTooltipNameId}\" parent=\"{addedButtonAbility.ParentValue}\">");

            //SetAbilityType(unit, ability);

            SetAbilityTierFromAbilityType(ability);
            SetTooltipCostData(ability.ReferenceNameId, ability);
            SetTooltipDescriptions(ability);
            SetTooltipOverrideData(cButtonElement, ability);

            //unit.Abilities[ability.ReferenceNameId] = ability;

            return null;
        }

        private void SetAbilityType(string slotValue, Ability ability)
        {
            //if (Enum.TryParse(slotValue, true, out AbilityType  )



            //string layoutId = !string.IsNullOrEmpty(ability.ParentLink) ? ability.ParentLink : unitId;

            //if (!SetAbilityTypeFromLayout(unitId, ability, GameData.GetLayoutButtonElements(layoutId)) && !SetAbilityTypeFromLayout(unitId, ability, GameData.GetLayoutButtonElements()))
            //    ability.AbilityType = AbilityType.Unknown;
        }

        private void SetAbilityTypeFromSlot(string slot, string unitId, Ability ability)
        {
            ReadOnlySpan<char> slotSpan = slot.AsSpan();

            if (slotSpan.IsEmpty)
                ability.AbilityType = AbilityType.Attack;
            else if (slotSpan.StartsWith("ABILITY1", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.Q;
            else if (slotSpan.StartsWith("ABILITY2", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.W;
            else if (slotSpan.StartsWith("ABILITY3", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.E;
            else if (slotSpan.StartsWith("MOUNT", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.Z;
            else if (slotSpan.StartsWith("HEROIC", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.Heroic;
            else if (slotSpan.StartsWith("HEARTH", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.B;
            else if (slotSpan.StartsWith("TRAIT", StringComparison.OrdinalIgnoreCase))
                ability.AbilityType = AbilityType.Trait;
            else if (Enum.TryParse(slot, true, out AbilityType abilityType))
                ability.AbilityType = abilityType;
            else
                throw new XmlGameDataParseException($"Unknown slot type ({slot}) - CUnit: {unitId} - Ability: {ability.ReferenceNameId}");
        }

        private void SetTalentIdUpgrades(XElement buttonElement, Ability ability)
        {
            foreach (XElement tooltipAppenderElement in buttonElement.Elements("TooltipAppender"))
            {
                string talentId = tooltipAppenderElement.Attribute("Face")?.Value;

                if (!string.IsNullOrEmpty(talentId))
                {
                    ability.AddTalentIdUpgrade(talentId);
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
            else if (ability.AbilityType == AbilityType.Taunt)
                ability.Tier = AbilityTier.Taunt;
            else if (ability.AbilityType == AbilityType.Dance)
                ability.Tier = AbilityTier.Dance;
            else if (ability.AbilityType == AbilityType.Spray)
                ability.Tier = AbilityTier.Spray;
            else if (ability.AbilityType == AbilityType.Voice)
                ability.Tier = AbilityTier.Voice;
            else if (ability.AbilityType == AbilityType.MapMechanic)
                ability.Tier = AbilityTier.MapMechanic;
            else if (ability.AbilityType == AbilityType.Interact)
                ability.Tier = AbilityTier.Interact;
            else if (ability.AbilityType == AbilityType.Attack || ability.AbilityType == AbilityType.Stop || ability.AbilityType == AbilityType.Hold || ability.AbilityType == AbilityType.Cancel | ability.AbilityType == AbilityType.ForceMove)
                ability.Tier = AbilityTier.Action;
            else
                ability.Tier = AbilityTier.Unknown;
        }

        private XElement GetButtonElement(string buttonId)
        {
            // search for CButton
            XElement buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == buttonId));
            if (buttonElement != null)
                return buttonElement;

            // if not found search for ability elements
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
                // TODO: dont specify ability cabileffecttarget, check all, possibly from parameter
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

        private bool SetAbilityTypeFromLayout(string unitId, Ability ability, ICollection<XElement> layoutButtonElements)
        {
            // check the face attribute
            XElement layoutButton = layoutButtonElements?.FirstOrDefault(x => (x.Attribute("Face")?.Value == ability.ReferenceNameId));

            // check the abilcmd attribute
            if (layoutButton == null)
            {
                layoutButton = layoutButtonElements?.FirstOrDefault(x =>
                {
                    return IsValidAbilityCommand(x.Attribute("AbilCmd")?.Value, ability.ReferenceNameId);
                });
            }

            if (layoutButton != null)
            {
                SetAbilityTypeFromSlot(layoutButton.Attribute("Slot")?.Value, unitId, ability);
                return true;
            }
            else // was not found for attributes check as elements
            {
                // check the face element value
                layoutButton = layoutButtonElements?.Where(x => x.HasElements).FirstOrDefault(x => (x.Element("Face")?.Attribute("value")?.Value == ability.ReferenceNameId));

                // check the abilcmd element value
                if (layoutButton == null)
                {
                    layoutButton = layoutButtonElements?.Where(x => x.HasElements).FirstOrDefault(x =>
                    {
                        return IsValidAbilityCommand(x.Element("AbilCmd")?.Attribute("value")?.Value, ability.ReferenceNameId);
                    });
                }

                if (layoutButton != null)
                {
                    SetAbilityTypeFromSlot(layoutButton.Element("Slot").Attribute("value")?.Value, unitId, ability);

                    return true;
                }
            }

            return false;
        }

        // check to see if there is a passive ability layout button associated with the current ability.
        // if there is, then we can ignore the current ability
        private bool HasPassiveAbility(string unitId, string abilId)
        {
            if (GameData.TryGetLayoutButtonElements(unitId, out List<XElement> layoutButtons))
            {
                // find current
                XElement abilLayoutButton = layoutButtons.FirstOrDefault(x =>
                {
                    return IsValidAbilityCommand(x.Attribute("AbilCmd")?.Value, abilId);
                });

                if (abilLayoutButton != null)
                {
                    // get the face value and slot
                    string faceValue = abilLayoutButton.Attribute("Face")?.Value;
                    string slotValue = abilLayoutButton.Attribute("Slot")?.Value;

                    if (!string.IsNullOrEmpty(faceValue) && !string.IsNullOrEmpty(slotValue))
                    {
                        // try to find a passive layout button
                        XElement passiveButtonElement = layoutButtons.FirstOrDefault(x => x.Attribute("Face")?.Value == faceValue && x.Attribute("Slot")?.Value == slotValue && x.Attribute("Type")?.Value == "Passive");
                        if (passiveButtonElement != null)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // detect if the ability is one we want to keep
        private bool IsValidAbilityCommand(string abilCmd, string abilityId)
        {
            ReadOnlySpan<char> abilCmdSpan = abilCmd.AsSpan();
            ReadOnlySpan<char> firstPartAbilCmdSpan;
            int index = abilCmdSpan.IndexOf(',');

            if (index > 0)
                firstPartAbilCmdSpan = abilCmdSpan.Slice(0, index);
            else
                firstPartAbilCmdSpan = abilCmdSpan;

            return firstPartAbilCmdSpan.Equals(abilityId, StringComparison.OrdinalIgnoreCase);
        }
    }
}
