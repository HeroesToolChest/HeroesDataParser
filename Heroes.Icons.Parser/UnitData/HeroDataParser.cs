using Heroes.Icons.Parser.Exceptions;
using Heroes.Icons.Parser.GameStrings;
using Heroes.Icons.Parser.Models;
using Heroes.Icons.Parser.Models.AbilityTalents;
using Heroes.Icons.Parser.UnitData.Overrides;
using Heroes.Icons.Parser.XmlGameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.UnitData
{
    public class HeroDataParser
    {
        private readonly double DefaultWeaponPeriod = 1.2; // for hero weapons

        private readonly GameData GameData;
        private readonly GameStringData GameStringData;
        private readonly GameStringParser GameStringParser;
        private readonly HeroOverrideData HeroOverrideData;

        private HeroOverride HeroOverride = new HeroOverride();

        public HeroDataParser(GameData gameData, GameStringData gameStringData, GameStringParser gameStringParser, HeroOverrideData heroOverrideData)
        {
            GameData = gameData;
            GameStringData = gameStringData;
            GameStringParser = gameStringParser;
            HeroOverrideData = heroOverrideData;
        }

        public Hero Parse(string cHeroId, string cUnitId)
        {
            Hero hero = new Hero
            {
                Name = GameStringData.HeroNamesByShortName[cHeroId],
                Description = GameStringParser.HeroParsedDescriptionsByShortName[cHeroId],
                CHeroId = cHeroId,
                CUnitId = cUnitId,
            };

            if (HeroOverrideData.HeroOverridesByCHero.TryGetValue(cHeroId, out HeroOverride heroOverride))
                HeroOverride = heroOverride;

            SetDefaultValues(hero);
            CHeroData(hero, cHeroId);
            CUnitData(hero, cUnitId);
            AddAdditionalCUnits(hero);

            ApplyOverrides(hero);

            // set all default abilities energy types to the hero's energy type
            foreach (KeyValuePair<string, Ability> ability in hero.Abilities)
            {
                if (ability.Value.Tooltip.Energy.EnergyType == UnitEnergyType.None && ability.Value.Tooltip.Energy.EnergyCost > 0)
                    ability.Value.Tooltip.Energy.EnergyType = hero.Energy.EnergyType;
            }

            return hero;
        }

        private void SetDefaultValues(Hero hero)
        {
            XElement stormHeroData = GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("id")?.Value == "StormHero").FirstOrDefault();
            if (stormHeroData != null)
            {
                string parentDataValue = stormHeroData.Attribute("parent").Value;
                XElement stormBasicHeroicUnitData = GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("id")?.Value == parentDataValue).FirstOrDefault();

                if (stormBasicHeroicUnitData != null)
                {
                    // loop through all elements and set found elements
                    foreach (XElement element in stormBasicHeroicUnitData.Elements())
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

                // loop through all StormHero elements and set found elements
                foreach (XElement element in stormHeroData.Elements())
                {
                    string elementName = element.Name.LocalName.ToUpper();

                    if (elementName == "LIFEMAX")
                    {
                        hero.Life.LifeMax = int.Parse(element.Attribute("value").Value);
                    }
                    else if (elementName == "LIFEREGENRATE")
                    {
                        hero.Life.LifeRegenerationRate = double.Parse(element.Attribute("value").Value);
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
                        hero.Energy.EnergyMax = int.Parse(element.Attribute("value").Value);
                    }
                    else if (elementName == "ENERGYREGENRATE")
                    {
                        hero.Energy.EnergyRegenerationRate = int.Parse(element.Attribute("value").Value);
                    }
                }
            }
        }

        private void CHeroData(Hero hero, string cHeroId)
        {
            XElement heroData = GameData.XmlGameData.Root.Elements("CHero").Where(x => x.Attribute("id")?.Value == cHeroId).FirstOrDefault();

            // get short name of hero
            XElement hyperLinkElement = heroData.Elements("HyperlinkId").Where(x => x.Attribute("value") != null).FirstOrDefault();
            if (hyperLinkElement != null)
                hero.ShortName = hyperLinkElement.Value;
            else
                hero.ShortName = cHeroId;

            // loop through all elements and set found elements
            foreach (var element in heroData.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "ATTRIBUTEID")
                {
                    hero.AttributeId = element.Attribute("value").Value;
                }
                else if (elementName == "MELEE")
                {
                    if (element.Attribute("value").Value == "1")
                        hero.Type = UnitType.Melee;
                    else if (element.Attribute("value").Value == "0")
                        hero.Type = UnitType.Ranged;
                    else
                        hero.Type = UnitType.Ranged;
                }
                else if (elementName == "DIFFICULTY")
                {
                    string difficulty = element.Attribute("value")?.Value.ToUpper();

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
                    string role = element.Attribute("value")?.Value.ToUpper();

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
                    GetAbility(hero, element);
                }
                else if (elementName == "TALENTTREEARRAY")
                {
                    GetTalent(hero, element);
                }
            }

            if (hero.Type == UnitType.Unknown)
                hero.Type = UnitType.Ranged;

            SetAdditionalLinkedAbilities(hero);
        }

        private void CUnitData(Hero hero, string cUnitId)
        {
            XElement heroData = GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("id")?.Value == cUnitId).FirstOrDefault();

            // loop through all elements and set found elements
            foreach (XElement element in heroData.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "LIFEMAX")
                {
                    hero.Life.LifeMax = int.Parse(element.Attribute("value").Value);

                    if (GameData.ScaleValueByLookupId.TryGetValue($"Unit#{hero.CUnitId}#LifeMax", out double scaleValue))
                        hero.Life.LifeScaling = scaleValue;
                }
                else if (elementName == "LIFEREGENRATE")
                {
                    hero.Life.LifeRegenerationRate = double.Parse(element.Attribute("value").Value);

                    if (GameData.ScaleValueByLookupId.TryGetValue($"Unit#{hero.CUnitId}#LifeRegenRate", out double scaleValue))
                        hero.Life.LifeRegenerationRateScaling = scaleValue;
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
                    hero.Energy.EnergyMax = int.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ENERGYREGENRATE")
                {
                    hero.Energy.EnergyRegenerationRate = int.Parse(element.Attribute("value").Value);
                }
            }

            // get weapons
            CWeaponLegacyData(hero, heroData.Elements("WeaponArray").Where(x => x.Attribute("Link") != null));

            if (hero.Energy.EnergyMax < 1)
                hero.Energy.EnergyType = UnitEnergyType.None;
            else
                hero.Energy.EnergyType = UnitEnergyType.Mana;

            if (hero.Difficulty == HeroDifficulty.Unknown)
                hero.Difficulty = HeroDifficulty.Easy;
        }

        private void GetAbility(Hero hero, XElement abilityElement)
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

            if (!string.IsNullOrEmpty(referenceName) && !string.IsNullOrEmpty(tooltipName))
            {
                ability.ReferenceNameId = referenceName;
                ability.FullTooltipNameId = tooltipName;

                if (!GameStringData.AbilityTalentNamesByReferenceNameId.ContainsKey(tooltipName))
                    throw new ParseException($"{nameof(GetAbility)}: {tooltipName} not found in description names");

                ability.Name = GameStringData.AbilityTalentNamesByReferenceNameId[tooltipName];
            }
            else if (!string.IsNullOrEmpty(referenceName) && string.IsNullOrEmpty(tooltipName)) // is a secondary ability
            {
                ability.ReferenceNameId = referenceName;
                ability.ParentLink = parentLink;
                ability.FullTooltipNameId = referenceName;

                if (!GameStringData.AbilityTalentNamesByReferenceNameId.ContainsKey(referenceName))
                    throw new ParseException($"{nameof(GetAbility)}: {referenceName} not found in description names");

                ability.Name = GameStringData.AbilityTalentNamesByReferenceNameId[referenceName];
            }
            else
            {
                ability.ReferenceNameId = tooltipName;
                ability.FullTooltipNameId = tooltipName;

                if (!GameStringData.AbilityTalentNamesByReferenceNameId.ContainsKey(tooltipName))
                    throw new ParseException($"{nameof(GetAbility)}: {tooltipName} not found in description names");

                ability.Name = GameStringData.AbilityTalentNamesByReferenceNameId[tooltipName];
            }

            XElement heroicElement = abilityElement.Elements("Flags").Where(x => x.Attribute("index").Value == "Heroic" && x.Attribute("value").Value == "1").FirstOrDefault();
            XElement traitElement = abilityElement.Elements("Flags").Where(x => x.Attribute("index").Value == "Trait" && x.Attribute("value").Value == "1").FirstOrDefault();
            XElement mountElement = abilityElement.Elements("Flags").Where(x => x.Attribute("index").Value == "MountReplacement" && x.Attribute("value").Value == "1").FirstOrDefault();

            if (heroicElement != null)
                ability.Tier = AbilityTier.Heroic;
            else if (traitElement != null)
                ability.Tier = AbilityTier.Trait;
            else if (mountElement != null)
                ability.Tier = AbilityTier.Mount;
            else
                ability.Tier = AbilityTier.Basic;

            SetAbilityTalentIcon(ability);
            SetTooltipSubInfo(referenceName, ability);
            SetTooltipDescriptions(ability);

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

                    // attempt to readd
                    if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
                        hero.Abilities.Add(ability.ReferenceNameId, ability);
                }
            }
        }

        private void GetTalent(Hero hero, XElement talentElement)
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

            XElement cTalentElement = GameData.XmlGameData.Root.Elements("CTalent").Where(x => x.Attribute("id").Value == referenceName).FirstOrDefault();

            // desc name
            XElement talentFaceElement = cTalentElement.Element("Face");
            if (talentFaceElement != null)
            {
                talent.FullTooltipNameId = talentFaceElement.Attribute("value").Value;
                talent.Name = GameStringData.AbilityTalentNamesByReferenceNameId[talent.FullTooltipNameId];
            }

            SetAbilityTalentIcon(talent);

            XElement talentAbilElement = cTalentElement.Elements("Abil").FirstOrDefault();
            XElement talentActiveElement = cTalentElement.Elements("Active").FirstOrDefault();
            if (talentAbilElement != null && talentActiveElement != null)
            {
                string effectId = talentAbilElement.Attribute("value").Value;

                if (talentActiveElement.Attribute("value").Value == "1")
                    SetTooltipSubInfo(effectId, talent);
            }

            SetTooltipDescriptions(talent);

            hero.Talents.Add(referenceName, talent);
        }

        private void SetAbilityTalentIcon(AbilityTalentBase abilityTalentBase)
        {
            XElement cButtonElement = GameData.XmlGameData.Root.Elements("CButton").Where(x => x.Attribute("id").Value == abilityTalentBase.FullTooltipNameId).FirstOrDefault();
            if (cButtonElement != null)
            {
                XElement buttonIconElement = cButtonElement.Element("Icon");
                if (buttonIconElement != null)
                    abilityTalentBase.IconFileName = Path.GetFileName(buttonIconElement.Attribute("value").Value);
            }
        }

        // Sets all the tooltip info: vital costs, cooldowns, charges, range, arc, etc...
        private void SetTooltipSubInfo(string elementId, AbilityTalentBase abilityTalentBase)
        {
            if (string.IsNullOrEmpty(elementId))
                return;

            var foundElements = GameData.XmlGameData.Root.Elements().Where(x => x.Attribute("id")?.Value == elementId);

            try
            {
                // look through all elements to find the tooltip info
                foreach (XElement element in foundElements)
                {
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
                            if (countMaxElement != null)
                                abilityTalentBase.Tooltip.Charges.CountMax = int.Parse(countMaxElement.Attribute("value").Value);

                            if (countStartElement != null)
                                abilityTalentBase.Tooltip.Charges.CountStart = int.Parse(countStartElement.Attribute("value").Value);

                            if (countUseElement != null)
                                abilityTalentBase.Tooltip.Charges.CountUse = int.Parse(countUseElement.Attribute("value").Value);

                            if (hideCountElement != null)
                                abilityTalentBase.Tooltip.Charges.IsHideCount = int.Parse(hideCountElement.Attribute("value").Value) == 1 ? true : false;

                            if (timeUseElement != null)
                                abilityTalentBase.Tooltip.Cooldown.CooldownValue = double.Parse(timeUseElement.Attribute("value").Value);
                        }

                        // cooldown
                        XElement cooldownElement = costElement.Element("Cooldown");
                        if (cooldownElement != null)
                        {
                            if (chargeElement != null && abilityTalentBase.Tooltip.Charges.HasCharges)
                            {
                                string time = cooldownElement.Attribute("TimeUse")?.Value;
                                if (!string.IsNullOrEmpty(time))
                                    abilityTalentBase.Tooltip.Cooldown.RecastCooldown = double.Parse(time);
                            }
                            else
                            {
                                if (double.TryParse(cooldownElement.Attribute("TimeUse")?.Value, out double cooldownValue))
                                    abilityTalentBase.Tooltip.Cooldown.CooldownValue = cooldownValue;
                            }
                        }

                        // vitals
                        XElement vitalElement = costElement.Element("Vital");
                        if (vitalElement != null)
                        {
                            string vitalType = vitalElement.Attribute("index").Value;
                            int vitalValue = int.Parse(vitalElement.Attribute("value").Value);

                            if (vitalType == "Energy")
                                abilityTalentBase.Tooltip.Energy.EnergyCost = vitalValue;
                            else if (vitalType == "Life")
                                abilityTalentBase.Tooltip.Life.LifeCost = vitalValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParseException($"Error {nameof(SetTooltipSubInfo)}() - Id={elementId}", ex);
            }
        }

        private void SetTooltipDescriptions(AbilityTalentBase abilityTalentBase)
        {
            string faceValue = abilityTalentBase.FullTooltipNameId;
            abilityTalentBase.ShortTooltipNameId = faceValue; // set to default

            string fullTooltipValue = string.Empty; // Tooltip
            string shortTooltipValue = string.Empty; // SimpleDisplayText

            // check cbutton for tooltip overrides
            XElement cButtonElement = GameData.XmlGameData.Root.Elements("CButton").Where(x => x.Attribute("id").Value == faceValue).FirstOrDefault();
            if (cButtonElement != null)
            {
                // full tooltip
                XElement cButtonTooltipElement = cButtonElement.Element("Tooltip");
                if (cButtonTooltipElement != null)
                {
                    fullTooltipValue = Path.GetFileName(cButtonTooltipElement.Attribute("value").Value);
                }

                // short tooltip
                XElement cButtonSimpleDisplayTextElement = cButtonElement.Element("SimpleDisplayText");
                if (cButtonSimpleDisplayTextElement != null)
                {
                    shortTooltipValue = Path.GetFileName(cButtonSimpleDisplayTextElement.Attribute("value").Value);
                }
            }

            // full
            if (GameStringParser.FullParsedTooltipsByFullTooltipNameId.TryGetValue(fullTooltipValue, out string fullDescription))
                abilityTalentBase.Tooltip.FullTooltip = new TooltipDescription(fullDescription);
            else if (GameStringParser.FullParsedTooltipsByFullTooltipNameId.TryGetValue(faceValue, out fullDescription))
                abilityTalentBase.Tooltip.FullTooltip = new TooltipDescription(fullDescription);

            // short
            if (GameStringParser.ShortParsedTooltipsByShortTooltipNameId.TryGetValue(shortTooltipValue, out string shortDescription))
            {
                abilityTalentBase.Tooltip.ShortTooltip = new TooltipDescription(shortDescription);
                abilityTalentBase.ShortTooltipNameId = shortTooltipValue;
            }
            else if (GameStringParser.ShortParsedTooltipsByShortTooltipNameId.TryGetValue(faceValue, out shortDescription))
            {
                abilityTalentBase.Tooltip.ShortTooltip = new TooltipDescription(shortDescription);
            }
        }

        private void SetAdditionalLinkedAbilities(Hero hero)
        {
            foreach (KeyValuePair<string, string> linkedAbility in HeroOverride.LinkedElementNamesByAbilityId)
            {
                Ability ability = new Ability();
                hero.Abilities = hero.Abilities ?? new Dictionary<string, Ability>();

                XElement abilityElement = GameData.XmlGameData.Root.Elements(linkedAbility.Value).Where(x => x.Attribute("id")?.Value == linkedAbility.Key).FirstOrDefault();

                if (abilityElement == null)
                    throw new ParseException($"{nameof(CHeroData)}: Additional link ability element not found - <{linkedAbility.Value} id=\"{linkedAbility.Key}\">");

                string linkName = abilityElement.Attribute("id")?.Value;
                if (linkName == null)
                    return;

                ability.ReferenceNameId = linkName;
                ability.FullTooltipNameId = linkName;

                if (GameStringData.AbilityTalentNamesByReferenceNameId.TryGetValue(linkName, out string abilityName))
                    ability.Name = abilityName;

                SetAbilityTalentIcon(ability);
                SetTooltipSubInfo(linkName, ability);
                SetTooltipDescriptions(ability);

                // add to abilities
                if (!hero.Abilities.ContainsKey(ability.ReferenceNameId))
                    hero.Abilities.Add(ability.ReferenceNameId, ability);
            }
        }

        // basic attack data
        private void CWeaponLegacyData(Hero hero, IEnumerable<XElement> weaponElements)
        {
            List<string> weaponsIds = new List<string>();
            foreach (XElement weaponElement in weaponElements)
            {
                string weaponNameId = weaponElement.Attribute("Link")?.Value;
                if (!string.IsNullOrEmpty(weaponNameId))
                    weaponsIds.Add(weaponNameId);
            }

            foreach (string weaponNameId in weaponsIds)
            {
                if (HeroOverride.IsValidWeaponByWeaponId.TryGetValue(weaponNameId, out bool validWeapon))
                {
                    if (!validWeapon)
                        continue;
                }
                else
                {
                    validWeapon = false;
                }

                if (validWeapon || weaponsIds.Count == 1 || weaponNameId.Contains("HeroWeapon") || weaponNameId == hero.CUnitId)
                {
                    HeroWeapon weapon = AddHeroWeapon(weaponNameId, weaponsIds);
                    if (weapon != null)
                        hero.Weapons.Add(weapon);
                }
            }
        }

        private HeroWeapon AddHeroWeapon(string weaponNameId, List<string> allWeaponIds)
        {
            HeroWeapon weapon = null;

            if (!string.IsNullOrEmpty(weaponNameId))
            {
                XElement weaponLegacy = GameData.XmlGameData.Root.Elements("CWeaponLegacy").Where(x => x.Attribute("id")?.Value == weaponNameId).FirstOrDefault();

                if (weaponLegacy != null)
                {
                    weapon = new HeroWeapon
                    {
                        WeaponNameId = weaponNameId,
                    };

                    HeroWeaponAddRange(weaponLegacy, weapon, weaponNameId);
                    HeroWeaponAddPeriod(weaponLegacy, weapon, weaponNameId);
                    HeroWeaponAddDamage(weaponLegacy, weapon, weaponNameId);
                }
            }

            return weapon;
        }

        private void HeroWeaponAddRange(XElement weaponLegacy, HeroWeapon weapon, string weaponNameId)
        {
            XElement rangeElement = weaponLegacy.Element("Range");
            string parentWeaponId = weaponLegacy.Attribute("parent")?.Value;

            if (rangeElement != null)
            {
                weapon.Range = double.Parse(rangeElement.Attribute("value").Value);
            }
            else if (!string.IsNullOrEmpty(parentWeaponId))
            {
                XElement parentWeaponLegacy = GameData.XmlGameData.Root.Elements("CWeaponLegacy").Where(x => x.Attribute("id")?.Value == parentWeaponId).FirstOrDefault();
                if (parentWeaponLegacy != null)
                    HeroWeaponAddRange(parentWeaponLegacy, weapon, parentWeaponId);
            }
        }

        private void HeroWeaponAddPeriod(XElement weaponLegacy, HeroWeapon weapon, string weaponNameId)
        {
            XElement periodElement = weaponLegacy.Element("Period");
            string parentWeaponId = weaponLegacy.Attribute("parent")?.Value;

            if (periodElement != null)
            {
                weapon.Period = double.Parse(periodElement.Attribute("value").Value);
            }
            else if (!string.IsNullOrEmpty(parentWeaponId))
            {
                XElement parentWeaponLegacy = GameData.XmlGameData.Root.Elements("CWeaponLegacy").Where(x => x.Attribute("id")?.Value == parentWeaponId).FirstOrDefault();
                if (parentWeaponLegacy != null)
                    HeroWeaponAddPeriod(parentWeaponLegacy, weapon, parentWeaponId);
            }
            else
            {
                weapon.Period = DefaultWeaponPeriod;
            }
        }

        private void HeroWeaponAddDamage(XElement weaponLegacy, HeroWeapon weapon, string weaponNameId)
        {
            XElement displayEffectElement = weaponLegacy.Element("DisplayEffect");
            string parentWeaponId = weaponLegacy.Attribute("parent")?.Value;

            if (displayEffectElement != null)
            {
                string displayEffectValue = displayEffectElement.Attribute("value").Value;
                XElement effectDamageElement = GameData.XmlGameData.Root.Elements("CEffectDamage").Where(x => x.Attribute("id")?.Value == displayEffectValue).FirstOrDefault();
                if (effectDamageElement != null)
                {
                    XElement amountElement = effectDamageElement.Element("Amount");
                    if (amountElement != null)
                    {
                        weapon.Damage = double.Parse(amountElement.Attribute("value").Value);
                    }
                }

                if (GameData.ScaleValueByLookupId.TryGetValue($"Effect#{displayEffectValue}#Amount", out double scaleValue))
                    weapon.DamageScaling = scaleValue;
            }
            else if (!string.IsNullOrEmpty(parentWeaponId))
            {
                XElement parentWeaponLegacy = GameData.XmlGameData.Root.Elements("CWeaponLegacy").Where(x => x.Attribute("id")?.Value == parentWeaponId).FirstOrDefault();
                if (parentWeaponLegacy != null)
                    HeroWeaponAddDamage(parentWeaponLegacy, weapon, parentWeaponId);
            }
        }

        private void AddAdditionalCUnits(Hero hero)
        {
            foreach (string unit in HeroOverride.AdditionalHeroUnits)
            {
                XElement cUnit = GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("id")?.Value == unit).FirstOrDefault();
                if (cUnit != null)
                {
                    Hero additionalHero = new Hero
                    {
                        Name = GameStringData.UnitNamesByShortName[unit],
                        Description = null,
                        CHeroId = null,
                        CUnitId = unit,
                    };

                    SetDefaultValues(additionalHero);
                    CUnitData(additionalHero, unit);

                    additionalHero.Difficulty = hero.Difficulty; // set to same as parent

                    hero.AdditionalHeroUnits.Add(additionalHero);
                }
            }
        }

        private void ApplyOverrides(Hero hero)
        {
            if (HeroOverride.EnergyTypeOverride.Enabled)
                hero.Energy.EnergyType = HeroOverride.EnergyTypeOverride.EnergyType;

            if (HeroOverride.EnergyOverride.Enabled)
                hero.Energy.EnergyMax = HeroOverride.EnergyOverride.Energy;

            // abilities
            foreach (KeyValuePair<string, Ability> ability in hero.Abilities)
            {
                if (HeroOverride.PropertyOverrideMethodByAbilityId.TryGetValue(ability.Key, out Dictionary<string, Action<Ability>> valueOverrideMethods))
                {
                    foreach (var propertyOverride in valueOverrideMethods)
                    {
                        // execute each property override
                        propertyOverride.Value(ability.Value);
                    }
                }
            }

            // weapons
            foreach (HeroWeapon weapon in hero.Weapons)
            {
                if (HeroOverride.PropertyOverrideMethodByWeaponId.TryGetValue(weapon.WeaponNameId, out Dictionary<string, Action<HeroWeapon>> valueOverrideMethods))
                {
                    foreach (var propertyOverride in valueOverrideMethods)
                    {
                        // execute each property override
                        propertyOverride.Value(weapon);
                    }
                }
            }
        }
    }
}
