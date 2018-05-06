﻿using Heroes.Icons.Parser.Descriptions;
using Heroes.Icons.Parser.HeroData;
using Heroes.Icons.Parser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.Heroes
{
    public abstract class DefaultHeroData
    {
        public DefaultHeroData(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, DescriptionParser descriptionParser, HeroOverrideLoader heroOverrideLoader)
        {
            HeroDataLoader = heroDataLoader;
            DescriptionLoader = descriptionLoader;
            DescriptionParser = descriptionParser;
            HeroOverrideLoader = heroOverrideLoader;
        }

        protected double DefaultWeaponPeriod => 1.2; // for hero weapons

        protected HeroDataLoader HeroDataLoader { get; }
        protected DescriptionLoader DescriptionLoader { get; }
        protected DescriptionParser DescriptionParser { get; }
        protected HeroOverrideLoader HeroOverrideLoader { get; }

        public Hero ParseHeroData(string cHeroId, string cUnitId)
        {
            var data = HeroDataLoader.XmlData;

            Hero hero = new Hero
            {
                Name = DescriptionLoader.HeroNames[cHeroId],
                Description = DescriptionParser.HeroParsedDescriptions[cHeroId],
                CHeroId = cHeroId,
                UnitName = cUnitId,
            };

            SetDefaultValues(hero, data);
            CHeroData(hero, cHeroId, data);
            CUnitData(hero, cUnitId, data);

            ApplyOverrides(hero);

            // set all default abilities energy types to the hero's energy type
            foreach (var ability in hero.Abilities)
            {
                if (ability.Value.Tooltip.EnergyType == EnergyType.None && ability.Value.Tooltip.Energy > 0)
                    ability.Value.Tooltip.EnergyType = hero.EnergyType;
            }

            return hero;
        }

        /// <summary>
        /// Sets all the tooltip info: vital costs, cooldowns, charges, range, arc, etc...
        /// </summary>
        /// <param name="attributeId">The id name of the ability or talent</param>
        /// <param name="abilityTalentBase"><see cref="AbilityTalentBase"/> object thats being updated</param>
        /// <param name="xmlData">Blizzard xml data</param>
        protected virtual void SetTooltipSubInfo(string attributeId, AbilityTalentBase abilityTalentBase, XDocument xmlData, bool allowOverrides = true)
        {
            if (string.IsNullOrEmpty(attributeId))
                return;

            var foundElements = xmlData.Descendants().Where(x => x.Attribute("id")?.Value == attributeId);

            try
            {
                // look through all elements to find the tooltip info
                foreach (XElement element in foundElements)
                {
                    // cost
                    XElement costElement = element.Descendants("Cost").FirstOrDefault();
                    if (costElement != null)
                    {
                        // charge
                        XElement chargeElement = costElement.Descendants("Charge").FirstOrDefault();
                        if (chargeElement != null)
                        {
                            XElement countMaxElement = chargeElement.Descendants("CountMax").FirstOrDefault();
                            XElement hideCountElement = chargeElement.Descendants("HideCount").FirstOrDefault();
                            if (countMaxElement != null)
                            {
                                // some abilities have one charge (a talent later adds at least one charge)
                                int numOfCharges = int.Parse(countMaxElement.Attribute("value").Value);
                                int hideCount = 0;

                                if (hideCountElement != null)
                                    hideCount = int.Parse(hideCountElement.Attribute("value").Value);

                                abilityTalentBase.Tooltip.NumberOfCharges = numOfCharges;
                                abilityTalentBase.Tooltip.IsChargeCooldown = true;
                            }

                            XElement timeUseElement = chargeElement.Descendants("TimeUse").FirstOrDefault();
                            if (timeUseElement != null)
                            {
                                abilityTalentBase.Tooltip.Cooldown = double.Parse(timeUseElement.Attribute("value").Value);
                            }
                        }

                        SetCooldown(costElement, chargeElement, abilityTalentBase);
                        SetVital(costElement, abilityTalentBase);
                    }

                    // range
                    XElement rangeElement = element.Descendants("Range").FirstOrDefault();
                    if (rangeElement != null)
                        abilityTalentBase.Tooltip.Range = double.Parse(rangeElement.Attribute("value").Value);

                    // arc
                    XElement arcElement = element.Descendants("Arc").FirstOrDefault();
                    if (arcElement != null)
                        abilityTalentBase.Tooltip.Arc = double.Parse(arcElement.Attribute("value").Value);
                }

                if (allowOverrides)
                {
                    if (HeroOverrideLoader.IdRedirectByAbilityId.TryGetValue(attributeId, out Dictionary<string, RedirectElement> idRedirects))
                    {
                        foreach (var redirectElement in idRedirects)
                        {
                            if (string.IsNullOrEmpty(redirectElement.Value.Id))
                                continue;

                            // find element in data file by looking up the id
                            XElement specialElement = xmlData.Descendants().Where(x => x.Attribute("id")?.Value == redirectElement.Value.Id).FirstOrDefault();
                            if (specialElement != null)
                            {
                                // get the first one
                                XElement element = specialElement.Descendants(redirectElement.Key).FirstOrDefault();
                                ApplyUniquePropertyLookups(redirectElement, element, abilityTalentBase);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParseException($"Error {nameof(SetTooltipSubInfo)}() - Id={attributeId}", ex);
            }
        }

        /// <summary>
        /// Gets the values of unqiue property elements
        /// </summary>
        /// <param name="elementName">The element lookup</param>
        /// <param name="dataElement">The xml data element that's being looked into</param>
        /// <param name="objectToBeUpdated">object that's being updated</param>
        protected virtual void ApplyUniquePropertyLookups(KeyValuePair<string, RedirectElement> element, XElement dataElement, AbilityTalentBase abilityTalentBase)
        {
            return;
        }

        /// <summary>
        /// Sets the cooldown of the ability or talent. If the ability or talent has charges, then this will set the RecastCooldown.
        /// </summary>
        /// <param name="costElement">The Cost element</param>
        /// <param name="chargeElement">The Charge element</param>
        /// <param name="abilityTalentBase"><see cref="AbilityTalentBase"/> object thats being updated</param>
        protected virtual void SetCooldown(XElement costElement, XElement chargeElement, AbilityTalentBase abilityTalentBase)
        {
            XElement cooldownElement = costElement.Descendants("Cooldown").FirstOrDefault();
            if (cooldownElement != null)
            {
                if (chargeElement != null && abilityTalentBase.Tooltip.IsChargeCooldown)
                {
                    string time = cooldownElement.Attribute("TimeUse")?.Value;
                    if (!string.IsNullOrEmpty(time))
                        abilityTalentBase.Tooltip.RecastCooldown = double.Parse(time);
                }
                else
                {
                    if (double.TryParse(cooldownElement.Attribute("TimeUse")?.Value, out double cooldownValue))
                        abilityTalentBase.Tooltip.Cooldown = cooldownValue;
                }
            }
        }

        /// <summary>
        /// Sets the vitals of the ability or talent
        /// </summary>
        /// <param name="costElement">The Cost element</param>
        /// <param name="abilityTalentBase"><see cref="AbilityTalentBase"/> object thats being updated</param>
        protected virtual void SetVital(XElement costElement, AbilityTalentBase abilityTalentBase)
        {
            XElement vitalElement = costElement.Descendants("Vital").FirstOrDefault();
            if (vitalElement != null)
            {
                var vitalType = vitalElement.Attribute("index").Value;
                var vitalValue = int.Parse(vitalElement.Attribute("value").Value);

                if (vitalType == "Energy")
                    abilityTalentBase.Tooltip.Energy = vitalValue;
                else if (vitalType == "Life")
                    abilityTalentBase.Tooltip.Life = vitalValue;
            }
        }

        protected virtual void HeroWeaponAddRange(XElement weaponLegacy, HeroWeapon weapon, string weaponNameId)
        {
            var rangeElement = weaponLegacy.Descendants("Range").FirstOrDefault();
            if (rangeElement != null)
                weapon.Range = double.Parse(rangeElement.Attribute("value").Value);
        }

        protected virtual void HeroWeaponAddPeriod(XElement weaponLegacy, HeroWeapon weapon, string weaponNameId)
        {
            XElement periodElement = weaponLegacy.Descendants("Period").FirstOrDefault();
            if (periodElement != null)
                weapon.Period = double.Parse(periodElement.Attribute("value").Value);
            else
                weapon.Period = DefaultWeaponPeriod;
        }

        protected virtual void HeroWeaponAddDamage(XElement weaponLegacy, HeroWeapon weapon, XDocument xmlData, string weaponNameId)
        {
            XElement displayEffectElement = weaponLegacy.Descendants("DisplayEffect").FirstOrDefault();
            if (displayEffectElement != null)
            {
                string displayEffectValue = displayEffectElement.Attribute("value").Value;
                var effectDamageElement = xmlData.Descendants("CEffectDamage").Where(x => x.Attribute("id")?.Value == displayEffectValue);
                if (effectDamageElement != null)
                {
                    XElement amountElement = effectDamageElement.Descendants("Amount").FirstOrDefault();
                    if (amountElement != null)
                    {
                        weapon.Damage = double.Parse(amountElement.Attribute("value").Value);
                    }
                }
            }
        }

        private void ApplyOverrides(Hero hero)
        {
            if (HeroOverrideLoader.EnergyTypeOverrideByCHero.TryGetValue(hero.CHeroId, out EnergyType energyType))
                hero.EnergyType = energyType;

            if (HeroOverrideLoader.EnergyOverrideByCHero.TryGetValue(hero.CHeroId, out int energyOverride))
                hero.Energy = energyOverride;

            // check each ability for overrides
            foreach (var ability in hero.Abilities)
            {
                if (HeroOverrideLoader.ValueOverrideMethodByAbilityId.TryGetValue(ability.Key, out Dictionary<string, Action<Ability>> valueOverrideMethods))
                {
                    foreach (var propertyOverride in valueOverrideMethods)
                    {
                        // execute each property override
                        propertyOverride.Value(ability.Value);
                    }
                }
            }

            // check each weapon for overrides
            foreach (var weapon in hero.Weapons)
            {
                if (HeroOverrideLoader.ValueOverrideMethodByWeaponId.TryGetValue(weapon.WeaponNameId, out Dictionary<string, Action<HeroWeapon>> valueOverrideMethods))
                {
                    foreach (var propertyOverride in valueOverrideMethods)
                    {
                        // execute each property override
                        propertyOverride.Value(weapon);
                    }
                }
            }
        }

        private void CHeroData(Hero hero, string cHeroId, XDocument xmlData)
        {
            XElement heroData = xmlData.Descendants("CHero").Where(x => x.Attribute("id")?.Value == cHeroId).FirstOrDefault();

            // get short name of hero
            XElement hyperLinkElement = heroData.Descendants("HyperlinkId").Where(x => x.Attribute("value") != null).FirstOrDefault();
            if (hyperLinkElement != null)
                hero.ShortName = hyperLinkElement.Value;
            else
                hero.ShortName = cHeroId;

            // loop through all descendants and set found elements
            foreach (var element in heroData.Descendants())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "ATTRIBUTEID")
                {
                    hero.AttributeId = element.Attribute("value").Value;
                }
                else if (elementName == "MELEE")
                {
                    if (element.Attribute("value").Value == "1")
                        hero.Type = HeroType.Melee;
                    else if (element.Attribute("value").Value == "0")
                        hero.Type = HeroType.Ranged;
                    else
                        hero.Type = HeroType.Ranged;
                }
                else if (elementName == "DIFFICULTY")
                {
                    string difficulty = element.Attribute("value").Value.ToUpper();

                    if (difficulty == "EASY")
                        hero.Difficulty = HeroDifficulty.Easy;
                    else if (difficulty == "MEDIUM")
                        hero.Difficulty = HeroDifficulty.Medium;
                    else if (difficulty == "HARD")
                        hero.Difficulty = HeroDifficulty.Hard;
                    else if (difficulty == "VERYHARD")
                        hero.Difficulty = HeroDifficulty.VeryHard;
                    else
                        hero.Difficulty = HeroDifficulty.Unknown;
                }
                else if (elementName == "ROLE" || elementName == "ROLESMULTICLASS")
                {
                    string role = element.Attribute("value").Value.ToUpper();

                    if (hero.Roles == null)
                        hero.Roles = new List<HeroRole>();

                    if (role == "WARRIOR")
                    {
                        if (!hero.Roles.Contains(HeroRole.Warrior))
                            hero.Roles.Add(HeroRole.Warrior);
                    }
                    else if (role == "DAMAGE")
                    {
                        if (!hero.Roles.Contains(HeroRole.Assassin))
                            hero.Roles.Add(HeroRole.Assassin);
                    }
                    else if (role == "SUPPORT")
                    {
                        if (!hero.Roles.Contains(HeroRole.Support))
                            hero.Roles.Add(HeroRole.Support);
                    }
                    else if (role == "SPECIALIST")
                    {
                        if (!hero.Roles.Contains(HeroRole.Specialist))
                            hero.Roles.Add(HeroRole.Specialist);
                    }
                    else if (role == "MULTICLASS")
                    {
                        if (!hero.Roles.Contains(HeroRole.Multiclass))
                            hero.Roles.Add(HeroRole.Multiclass);
                    }
                    else
                    {
                        if (!hero.Roles.Contains(HeroRole.Unknown))
                            hero.Roles.Add(HeroRole.Unknown);
                    }
                }
                else if (elementName == "SELECTSCREENBUTTONIMAGE")
                {
                    hero.HeroPortrait = Path.GetFileName(element.Attribute("value").Value);
                }
                else if (elementName == "SCORESCREENIMAGE")
                {
                    hero.LeaderboardPortrait = Path.GetFileName(element.Attribute("value").Value);
                }
                else if (elementName == "LOADINGSCREENIMAGE")
                {
                    hero.LoadingPortrait = Path.GetFileName(element.Attribute("value").Value);
                }
                else if (elementName == "UNIVERSEICON")
                {
                    string iconImage = Path.GetFileName(element.Attribute("value").Value).ToUpper();

                    if (iconImage == "UI_GLUES_STORE_GAMEICON_SC2.DDS")
                        hero.Franchise = HeroFranchise.Starcraft;
                    else if (iconImage == "UI_GLUES_STORE_GAMEICON_WOW.DDS")
                        hero.Franchise = HeroFranchise.Warcraft;
                    else if (iconImage == "UI_GLUES_STORE_GAMEICON_D3.DDS")
                        hero.Franchise = HeroFranchise.Diablo;
                    else if (iconImage == "UI_GLUES_STORE_GAMEICON_OW.DDS")
                        hero.Franchise = HeroFranchise.Overwatch;
                    else if (iconImage == "UI_GLUES_STORE_GAMEICON_RETRO.DDS")
                        hero.Franchise = HeroFranchise.Classic;
                    else
                        hero.Franchise = HeroFranchise.Unknown;
                }
                else if (elementName == "UNIVERSE")
                {
                    string universe = element.Attribute("value").Value.ToUpper();

                    if (universe == "STARCRAFT")
                        hero.Franchise = HeroFranchise.Starcraft;
                    else if (universe == "WARCRAFT")
                        hero.Franchise = HeroFranchise.Warcraft;
                    else if (universe == "DIABLO")
                        hero.Franchise = HeroFranchise.Diablo;
                    else if (universe == "OVERWATCH")
                        hero.Franchise = HeroFranchise.Overwatch;
                    else if (universe == "RETRO")
                        hero.Franchise = HeroFranchise.Classic;
                    else
                        hero.Franchise = HeroFranchise.Unknown;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = 1;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = 2014;

                    hero.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "HEROABILARRAY")
                {
                    GetAbility(hero, element, xmlData);
                }
                else if (elementName == "TALENTTREEARRAY")
                {
                    GetTalent(hero, element, xmlData);
                }
                else if (elementName == "LEVELSCALINGARRAY")
                {
                    HeroScalingData(hero, element, xmlData);
                }
            }

            if (hero.Type == HeroType.Unknown)
                hero.Type = HeroType.Ranged;

            // add additional linked abilities
            if (HeroOverrideLoader.LinkedAbilityByCHero.TryGetValue(cHeroId, out Dictionary<string, string> linkHeroAbilities))
            {
                foreach (var linkedAbility in linkHeroAbilities)
                {
                    XElement abilityElement = xmlData.Descendants(linkedAbility.Value).Where(x => x.Attribute("id")?.Value == linkedAbility.Key).FirstOrDefault();

                    if (abilityElement == null)
                        throw new ParseException($"{nameof(CHeroData)}: Additional link ability element not found - <{linkedAbility.Value} id=\"{linkedAbility.Key}\">");

                    AddLinkedAbility(hero, abilityElement, xmlData);
                }
            }
        }

        private void CUnitData(Hero hero, string cUnitId, XDocument xmlData)
        {
            XElement heroData = xmlData.Descendants("CUnit").Where(x => x.Attribute("id")?.Value == cUnitId).FirstOrDefault();

            // loop through all descendants and set found elements
            foreach (XElement element in heroData.Descendants())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "LIFEMAX")
                {
                    hero.Life = int.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "LIFEREGENRATE")
                {
                    hero.LifeRegenerationRate = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "RADIUS")
                {
                    hero.Radius = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "INNERRADIUS")
                {
                    hero.InnerRadius = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ENERGYMAX")
                {
                    hero.Energy = int.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ENERGYREGENRATE")
                {
                    hero.EnergyRegenerationRate = int.Parse(element.Attribute("value").Value);
                }
            }

            // get weapons
            CWeaponLegacyData(hero, heroData.Descendants("WeaponArray").Where(x => x.Attribute("Link") != null), xmlData);

            if (hero.Energy < 1)
                hero.EnergyType = EnergyType.None;
            else
                hero.EnergyType = EnergyType.Mana;

            hero.Difficulty = HeroDifficulty.Easy;
        }

        // basic attack data
        private void CWeaponLegacyData(Hero hero, IEnumerable<XElement> weaponElements, XDocument xmlData)
        {
            foreach (XElement weaponElement in weaponElements)
            {
                string weaponNameId = weaponElement.Attribute("Link")?.Value;

                if (string.IsNullOrEmpty(weaponNameId))
                    continue;

                if (HeroOverrideLoader.ValidWeapons.TryGetValue(weaponNameId, out bool validWeapon))
                {
                    if (!validWeapon)
                        continue;
                }
                else
                {
                    validWeapon = false;
                }

                if (validWeapon || weaponElements.Count() == 1 || weaponNameId.Contains("HeroWeapon") || weaponNameId == hero.UnitName)
                {
                    HeroWeapon weapon = AddHeroWeapon(hero, weaponNameId, xmlData);
                    if (weapon != null)
                        hero.Weapons.Add(weapon);
                }
            }
        }

        private void GetAbility(Hero hero, XElement abilityElement, XDocument xmlData)
        {
            hero.Abilities = hero.Abilities ?? new Dictionary<string, Ability>();

            string referenceName = abilityElement.Attribute("Abil")?.Value;
            string descriptionName = abilityElement.Attribute("Button")?.Value;
            string parentLink = abilityElement.Attribute("Unit")?.Value;

            XElement usableAbility = abilityElement.Descendants("Flags").Where(x => x.Attribute("index").Value == "ShowInHeroSelect" && x.Attribute("value").Value == "1").FirstOrDefault();

            Ability ability = new Ability();

            if (!string.IsNullOrEmpty(referenceName) && HeroOverrideLoader.ValidAbilities.TryGetValue(referenceName, out bool validAbility))
            {
                if (!validAbility)
                    return;
            }
            else
            {
                validAbility = false;
            }

            if (usableAbility == null && parentLink == null && !validAbility)
                return;

            if (!string.IsNullOrEmpty(referenceName) && !string.IsNullOrEmpty(descriptionName))
            {
                ability.ReferenceNameId = referenceName;
                ability.FullDescriptionNameId = descriptionName;

                if (!DescriptionLoader.DescriptionNames.ContainsKey(descriptionName))
                    throw new ParseException($"{nameof(GetAbility)}: {descriptionName} not found in description names");

                ability.Name = DescriptionLoader.DescriptionNames[descriptionName];
            }
            else if (!string.IsNullOrEmpty(referenceName) && string.IsNullOrEmpty(descriptionName)) // is a secondary ability
            {
                ability.ReferenceNameId = referenceName;
                ability.ParentLink = parentLink;
                ability.FullDescriptionNameId = referenceName;

                if (!DescriptionLoader.DescriptionNames.ContainsKey(referenceName))
                    throw new ParseException($"{nameof(GetAbility)}: {referenceName} not found in description names");

                ability.Name = DescriptionLoader.DescriptionNames[referenceName];
            }
            else
            {
                ability.ReferenceNameId = descriptionName;
                ability.FullDescriptionNameId = descriptionName;

                if (!DescriptionLoader.DescriptionNames.ContainsKey(descriptionName))
                    throw new ParseException($"{nameof(GetAbility)}: {descriptionName} not found in description names");

                ability.Name = DescriptionLoader.DescriptionNames[descriptionName];
            }

            XElement heroicElement = abilityElement.Descendants("Flags").Where(x => x.Attribute("index").Value == "Heroic" && x.Attribute("value").Value == "1").FirstOrDefault();
            XElement traitElement = abilityElement.Descendants("Flags").Where(x => x.Attribute("index").Value == "Trait" && x.Attribute("value").Value == "1").FirstOrDefault();
            XElement mountElement = abilityElement.Descendants("Flags").Where(x => x.Attribute("index").Value == "MountReplacement" && x.Attribute("value").Value == "1").FirstOrDefault();

            if (heroicElement != null)
                ability.Tier = AbilityTier.Heroic;
            else if (traitElement != null)
                ability.Tier = AbilityTier.Trait;
            else if (mountElement != null)
                ability.Tier = AbilityTier.Mount;
            else
                ability.Tier = AbilityTier.Basic;

            SetAbilityTalentIcon(ability, xmlData);
            SetTooltipSubInfo(referenceName, ability, xmlData);
            SetTooltipDescriptions(ability, xmlData);

            // add to abilities
            if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
                hero.Abilities.Add(ability.ReferenceNameId, ability);
        }

        private void GetTalent(Hero hero, XElement talentElement, XDocument xmlData)
        {
            hero.Talents = hero.Talents ?? new Dictionary<string, Talent>();

            string referenceName = talentElement.Attribute("Talent").Value;
            string tier = talentElement.Attribute("Tier").Value;
            string column = talentElement.Attribute("Column").Value;

            Talent talent = new Talent
            {
                ReferenceNameId = referenceName,
                Column = int.Parse(column),
            };

            if (tier == "1")
                talent.Tier = TalentTier.Level1;
            else if (tier == "2")
                talent.Tier = TalentTier.Level4;
            else if (tier == "3")
                talent.Tier = TalentTier.Level7;
            else if (tier == "4")
                talent.Tier = TalentTier.Level10;
            else if (tier == "5")
                talent.Tier = TalentTier.Level13;
            else if (tier == "6")
                talent.Tier = TalentTier.Level16;
            else if (tier == "7")
                talent.Tier = TalentTier.Level20;
            else
                talent.Tier = TalentTier.Old;

            XElement cTalentElement = xmlData.Descendants("CTalent").Where(x => x.Attribute("id").Value == referenceName).FirstOrDefault();

            // desc name
            XElement talentFaceElement = cTalentElement.Descendants("Face").FirstOrDefault();
            if (talentFaceElement != null)
            {
                talent.FullDescriptionNameId = talentFaceElement.Attribute("value").Value;
                talent.Name = DescriptionLoader.DescriptionNames[talent.FullDescriptionNameId];
            }

            SetAbilityTalentIcon(talent, xmlData);

            XElement talentAbilElement = cTalentElement.Descendants("Abil").FirstOrDefault();
            XElement talentActiveElement = cTalentElement.Descendants("Active").FirstOrDefault();
            if (talentAbilElement != null && talentActiveElement != null)
            {
                string effectId = talentAbilElement.Attribute("value").Value;

                if (talentActiveElement.Attribute("value").Value == "1")
                    SetTooltipSubInfo(effectId, talent, xmlData);
            }

            SetTooltipDescriptions(talent, xmlData);

            hero.Talents.Add(referenceName, talent);
        }

        private void SetAbilityTalentIcon(AbilityTalentBase abilityTalentBase, XDocument xmlData)
        {
            XElement cButtonElement = xmlData.Descendants("CButton").Where(x => x.Attribute("id").Value == abilityTalentBase.FullDescriptionNameId).FirstOrDefault();
            if (cButtonElement != null)
            {
                XElement buttonIconElement = cButtonElement.Descendants("Icon").FirstOrDefault();
                if (buttonIconElement != null)
                    abilityTalentBase.IconFileName = Path.GetFileName(buttonIconElement.Attribute("value").Value);
            }
        }

        private void SetTooltipDescriptions(AbilityTalentBase abilityTalentBase, XDocument xmlData)
        {
            string faceValue = abilityTalentBase.FullDescriptionNameId;
            abilityTalentBase.ShortDescriptionNameId = faceValue; // set to default

            string fullTooltipValue = string.Empty; // Tooltip
            string shortTooltipValue = string.Empty; // SimpleDisplayText

            // check cbutton for tooltip overrides
            XElement cButtonElement = xmlData.Descendants("CButton").Where(x => x.Attribute("id").Value == faceValue).FirstOrDefault();
            if (cButtonElement != null)
            {
                // full tooltip
                XElement cButtonTooltipElement = cButtonElement.Descendants("Tooltip").FirstOrDefault();
                if (cButtonTooltipElement != null)
                {
                    fullTooltipValue = Path.GetFileName(cButtonTooltipElement.Attribute("value").Value);
                }

                // short tooltip
                XElement cButtonSimpleDisplayTextElement = cButtonElement.Descendants("SimpleDisplayText").FirstOrDefault();
                if (cButtonSimpleDisplayTextElement != null)
                {
                    shortTooltipValue = Path.GetFileName(cButtonSimpleDisplayTextElement.Attribute("value").Value);
                }
            }

            // full
            if (DescriptionParser.FullParsedDescriptions.TryGetValue(fullTooltipValue, out string fullDescription))
                abilityTalentBase.Tooltip.FullTooltipDescription = new TooltipDescription(fullDescription);
            else if (DescriptionParser.FullParsedDescriptions.TryGetValue(faceValue, out fullDescription))
                abilityTalentBase.Tooltip.FullTooltipDescription = new TooltipDescription(fullDescription);

            // short
            if (DescriptionParser.ShortParsedDescriptions.TryGetValue(shortTooltipValue, out string shortDescription))
            {
                abilityTalentBase.Tooltip.ShortTooltipDescription = new TooltipDescription(shortDescription);
                abilityTalentBase.ShortDescriptionNameId = shortTooltipValue;
            }
            else if (DescriptionParser.ShortParsedDescriptions.TryGetValue(faceValue, out shortDescription))
            {
                abilityTalentBase.Tooltip.ShortTooltipDescription = new TooltipDescription(shortDescription);
            }
        }

        // Additional ability that is not in the hero's CHero xml element
        private void AddLinkedAbility(Hero hero, XElement abilityElement, XDocument xmlData)
        {
            hero.Abilities = hero.Abilities ?? new Dictionary<string, Ability>();

            string linkName = abilityElement.Attribute("id")?.Value;

            Ability ability = new Ability();

            if (linkName == null)
                return;

            ability.ReferenceNameId = linkName;
            ability.FullDescriptionNameId = linkName;

            if (DescriptionLoader.DescriptionNames.TryGetValue(linkName, out string abilityName))
                ability.Name = abilityName;

            SetAbilityTalentIcon(ability, xmlData);
            SetTooltipSubInfo(linkName, ability, xmlData);
            SetTooltipDescriptions(ability, xmlData);

            // add to abilities
            if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
                hero.Abilities.Add(ability.ReferenceNameId, ability);
        }

        private HeroWeapon AddHeroWeapon(Hero hero, string weaponNameId, XDocument xmlData)
        {
            HeroWeapon weapon = null;

            if (!string.IsNullOrEmpty(weaponNameId))
            {
                XElement weaponLegacy = xmlData.Descendants("CWeaponLegacy").Where(x => x.Attribute("id")?.Value == weaponNameId).FirstOrDefault();
                if (weaponLegacy != null)
                {
                    weapon = new HeroWeapon
                    {
                        WeaponNameId = weaponNameId,
                    };

                    HeroWeaponAddRange(weaponLegacy, weapon, weaponNameId);
                    HeroWeaponAddPeriod(weaponLegacy, weapon, weaponNameId);
                    HeroWeaponAddDamage(weaponLegacy, weapon, xmlData, weaponNameId);
                }
            }

            return weapon;
        }

        private void HeroScalingData(Hero hero, XElement scalingElement, XDocument xmlData)
        {
            if (scalingElement.HasAttributes)
                return;

            IEnumerable<XElement> modifications = scalingElement.Descendants("Modifications");

            foreach (XElement modification in modifications)
            {
                if (modification.Descendants("Entry").Where(x => x.Attribute("value")?.Value == hero.UnitName).FirstOrDefault() != null)
                {
                    if (modification.Descendants("Field").Where(x => x.Attribute("value")?.Value == "LifeMax").FirstOrDefault() != null)
                        hero.LifeScaling = double.Parse(modification.Descendants("Value").FirstOrDefault().Attribute("value").Value);
                    else if (modification.Descendants("Field").Where(x => x.Attribute("value")?.Value == "LifeRegenRate").FirstOrDefault() != null)
                        hero.LifeScalingRegenerationRate = double.Parse(modification.Descendants("Value").FirstOrDefault().Attribute("value").Value);
                }
            }
        }

        private void SetDefaultValues(Hero hero, XDocument xmlData)
        {
            XElement stormHeroData = xmlData.Descendants("CUnit").Where(x => x.Attribute("id")?.Value == "StormHero").FirstOrDefault();
            if (stormHeroData != null)
            {
                string parentDataValue = stormHeroData.Attribute("parent").Value;
                XElement stormBasicHeroicUnitData = xmlData.Descendants("CUnit").Where(x => x.Attribute("id")?.Value == parentDataValue).FirstOrDefault();

                if (stormBasicHeroicUnitData != null)
                {
                    // loop through all descendants and set found elements
                    foreach (XElement element in stormBasicHeroicUnitData.Descendants())
                    {
                        string elementName = element.Name.LocalName.ToUpper();

                        if (elementName == "SPEED")
                        {
                            hero.Speed = double.Parse(element.Attribute("value").Value);
                        }
                        else if (elementName == "SIGHT")
                        {
                            hero.Sight = double.Parse(element.Attribute("value").Value);
                        }
                    }
                }

                // loop through all StormHero descendants and set found elements
                foreach (XElement element in stormHeroData.Descendants())
                {
                    string elementName = element.Name.LocalName.ToUpper();

                    if (elementName == "LIFEMAX")
                    {
                        hero.Life = int.Parse(element.Attribute("value").Value);
                    }
                    else if (elementName == "LIFEREGENRATE")
                    {
                        hero.LifeRegenerationRate = double.Parse(element.Attribute("value").Value);
                    }
                    else if (elementName == "RADIUS")
                    {
                        hero.Radius = double.Parse(element.Attribute("value").Value);
                    }
                    else if (elementName == "INNERRADIUS")
                    {
                        hero.InnerRadius = double.Parse(element.Attribute("value").Value);
                    }
                    else if (elementName == "ENERGYMAX")
                    {
                        hero.Energy = int.Parse(element.Attribute("value").Value);
                    }
                    else if (elementName == "ENERGYREGENRATE")
                    {
                        hero.EnergyRegenerationRate = int.Parse(element.Attribute("value").Value);
                    }
                }
            }
        }
    }
}