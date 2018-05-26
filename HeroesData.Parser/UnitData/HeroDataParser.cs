using HeroesData.Parser.Exceptions;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.Models;
using HeroesData.Parser.Models.AbilityTalents;
using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData
{
    public class HeroDataParser
    {
        private readonly double DefaultWeaponPeriod = 1.2; // for hero weapons

        private readonly GameData GameData;
        private readonly GameStringData GameStringData;
        private readonly GameStringParser GameStringParser;
        private readonly OverrideData OverrideData;

        private HeroOverride HeroOverride = new HeroOverride();

        public HeroDataParser(GameData gameData, GameStringData gameStringData, GameStringParser gameStringParser, OverrideData overrideData)
        {
            GameData = gameData;
            GameStringData = gameStringData;
            GameStringParser = gameStringParser;
            OverrideData = overrideData;
        }

        public Hero Parse(string cHeroId, string cUnitId)
        {
            Hero hero = new Hero
            {
                Name = GameStringData.HeroNamesByShortName[cHeroId],
                Description = new TooltipDescription(GameStringParser.HeroParsedDescriptionsByShortName[cHeroId]),
                CHeroId = cHeroId,
                CUnitId = cUnitId,
            };

            HeroOverride heroOverride = OverrideData.HeroOverride(cHeroId);
            if (heroOverride != null)
                HeroOverride = heroOverride;

            SetDefaultValues(hero);
            CHeroData(hero, cHeroId);
            CUnitData(hero, cUnitId);
            AddSubHeroCUnits(hero);

            ApplyOverrides(hero, HeroOverride);
            MoveParentLinkedAbilities(hero);

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

            hero.ReleaseDate = new DateTime(2014, 3, 13);
            hero.Gender = HeroGender.Male;
        }

        private void CHeroData(Hero hero, string cHeroId)
        {
            XElement heroData = GameData.XmlGameData.Root.Elements("CHero").Where(x => x.Attribute("id")?.Value == cHeroId).FirstOrDefault();

            // get short name of hero
            string hyperLinkElement = heroData.Element("HyperlinkId")?.Attribute("value")?.Value;
            if (!string.IsNullOrEmpty(hyperLinkElement))
                hero.ShortName = hyperLinkElement;
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
                else if (elementName == "GENDER")
                {
                    if (Enum.TryParse(element.Attribute("value").Value, out HeroGender heroGender))
                        hero.Gender = heroGender;
                    else
                        hero.Gender = HeroGender.Neutral;
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value").Value, out HeroRarity heroRarity))
                        hero.Rarity = heroRarity;
                    else
                        hero.Rarity = HeroRarity.None;
                }
                else if (elementName == "RATINGS")
                {
                    string damage = string.Empty;
                    string utility = string.Empty;
                    string survivability = string.Empty;
                    string complexity = string.Empty;

                    if (element.HasElements)
                    {
                        damage = element.Element("Damage").Attribute("value")?.Value;
                        utility = element.Element("Utility").Attribute("value")?.Value;
                        survivability = element.Element("Survivability").Attribute("value")?.Value;
                        complexity = element.Element("Complexity").Attribute("value")?.Value;
                    }
                    else
                    {
                        damage = element.Attribute("Damage")?.Value;
                        utility = element.Attribute("Utility")?.Value;
                        survivability = element.Attribute("Survivability")?.Value;
                        complexity = element.Attribute("Complexity")?.Value;
                    }

                    hero.Ratings.Damage = !string.IsNullOrEmpty(damage) ? double.Parse(damage) : 1;
                    hero.Ratings.Utility = !string.IsNullOrEmpty(utility) ? double.Parse(utility) : 1;
                    hero.Ratings.Survivability = !string.IsNullOrEmpty(survivability) ? double.Parse(survivability) : 1;
                    hero.Ratings.Complexity = !string.IsNullOrEmpty(complexity) ? double.Parse(complexity) : 1;
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
                    hero.Life.LifeMax = double.Parse(element.Attribute("value").Value);

                    double? scaleValue = GameData.ScaleValue(("Unit", hero.CUnitId, "LifeMax"));
                    if (scaleValue.HasValue)
                        hero.Life.LifeScaling = scaleValue.Value;
                }
                else if (elementName == "LIFEREGENRATE")
                {
                    hero.Life.LifeRegenerationRate = double.Parse(element.Attribute("value").Value);

                    double? scaleValue = GameData.ScaleValue(("Unit", hero.CUnitId, "LifeRegenRate"));
                    if (scaleValue.HasValue)
                        hero.Life.LifeRegenerationRateScaling = scaleValue.Value;
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
                    hero.Energy.EnergyMax = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ENERGYREGENRATE")
                {
                    hero.Energy.EnergyRegenerationRate = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "GENDER")
                {
                    if (Enum.TryParse(element.Attribute("value").Value, out HeroGender heroGender))
                        hero.Gender = heroGender;
                    else
                        hero.Gender = HeroGender.Neutral;
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
            XElement activableElement = abilityElement.Elements("Flags").Where(x => x.Attribute("index").Value == "ShowInQuickCastSetting" && x.Attribute("value").Value == "1").FirstOrDefault();

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

            XElement cTalentElement = GameData.XmlGameData.Root.Elements("CTalent").Where(x => x.Attribute("id")?.Value == referenceName).FirstOrDefault();

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
            XElement cButtonElement = GameData.XmlGameData.Root.Elements("CButton").Where(x => x.Attribute("id")?.Value == abilityTalentBase.FullTooltipNameId).FirstOrDefault();
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
            XElement cButtonElement = GameData.XmlGameData.Root.Elements("CButton").Where(x => x.Attribute("id")?.Value == faceValue).FirstOrDefault();
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
                    UnitWeapon weapon = AddHeroWeapon(weaponNameId, weaponsIds);
                    if (weapon != null)
                        hero.Weapons.Add(weapon);
                }
            }
        }

        private UnitWeapon AddHeroWeapon(string weaponNameId, List<string> allWeaponIds)
        {
            UnitWeapon weapon = null;

            if (!string.IsNullOrEmpty(weaponNameId))
            {
                XElement weaponLegacy = GameData.XmlGameData.Root.Elements("CWeaponLegacy").Where(x => x.Attribute("id")?.Value == weaponNameId).FirstOrDefault();

                if (weaponLegacy != null)
                {
                    weapon = new UnitWeapon
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

        private void HeroWeaponAddRange(XElement weaponLegacy, UnitWeapon weapon, string weaponNameId)
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

        private void HeroWeaponAddPeriod(XElement weaponLegacy, UnitWeapon weapon, string weaponNameId)
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

        private void HeroWeaponAddDamage(XElement weaponLegacy, UnitWeapon weapon, string weaponNameId)
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

                double? scaleValue = GameData.ScaleValue(("Effect", displayEffectValue, "Amount"));
                if (scaleValue.HasValue)
                    weapon.DamageScaling = scaleValue.Value;
            }
            else if (!string.IsNullOrEmpty(parentWeaponId))
            {
                XElement parentWeaponLegacy = GameData.XmlGameData.Root.Elements("CWeaponLegacy").Where(x => x.Attribute("id")?.Value == parentWeaponId).FirstOrDefault();
                if (parentWeaponLegacy != null)
                    HeroWeaponAddDamage(parentWeaponLegacy, weapon, parentWeaponId);
            }
        }

        private void AddSubHeroCUnits(Hero hero)
        {
            foreach (string unit in HeroOverride.HeroUnits)
            {
                XElement cUnit = GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("id")?.Value == unit).FirstOrDefault();

                string name = string.Empty;
                if (!GameStringData.UnitNamesByShortName.TryGetValue(unit, out name))
                {
                    if (!GameStringData.AbilityTalentNamesByReferenceNameId.TryGetValue(unit, out name))
                        name = unit;
                }

                Hero heroUnit = new Hero
                {
                    Name = name,
                    ShortName = unit.StartsWith("Hero") ? unit.Remove(0, 4) : unit,
                    CHeroId = null,
                    CUnitId = unit,
                };

                if (cUnit != null)
                {
                    if (GameStringParser.HeroParsedDescriptionsByShortName.TryGetValue(unit, out string desc))
                        heroUnit.Description = new TooltipDescription(desc);

                    SetDefaultValues(heroUnit);
                    CUnitData(heroUnit, unit);

                    HeroOverride heroOverride = OverrideData.HeroOverride(unit);
                    if (heroOverride != null)
                        ApplyOverrides(heroUnit, heroOverride);
                }

                hero.HeroUnits.Add(heroUnit);
            }
        }

        private void ApplyOverrides(Hero hero, HeroOverride heroOverride)
        {
            if (heroOverride.NameOverride.Enabled)
                hero.Name = heroOverride.NameOverride.Name;

            if (heroOverride.ShortNameOverride.Enabled)
                hero.ShortName = heroOverride.ShortNameOverride.ShortName;

            if (heroOverride.EnergyTypeOverride.Enabled)
                hero.Energy.EnergyType = heroOverride.EnergyTypeOverride.EnergyType;

            if (heroOverride.EnergyOverride.Enabled)
                hero.Energy.EnergyMax = heroOverride.EnergyOverride.Energy;

            if (heroOverride.ParentLinkOverride.Enabled)
                hero.ParentLink = heroOverride.ParentLinkOverride.ParentLink;

            // abilities
            if (hero.Abilities != null)
            {
                foreach (KeyValuePair<string, Ability> ability in hero.Abilities)
                {
                    if (heroOverride.PropertyOverrideMethodByAbilityId.TryGetValue(ability.Key, out Dictionary<string, Action<Ability>> valueOverrideMethods))
                    {
                        foreach (var propertyOverride in valueOverrideMethods)
                        {
                            // execute each property override
                            propertyOverride.Value(ability.Value);
                        }
                    }
                }
            }

            // weapons
            if (hero.Weapons != null)
            {
                foreach (UnitWeapon weapon in hero.Weapons)
                {
                    if (heroOverride.PropertyOverrideMethodByWeaponId.TryGetValue(weapon.WeaponNameId, out Dictionary<string, Action<UnitWeapon>> valueOverrideMethods))
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

        // moves the parent linked abilites to the respective herounit
        private void MoveParentLinkedAbilities(Hero hero)
        {
            // move sub abilities to hero unit abilites
            ILookup<string, Ability> linkedAbilities = hero.ParentLinkedAbilities();
            if (linkedAbilities.Count > 0)
            {
                foreach (Hero heroUnit in hero.HeroUnits)
                {
                    heroUnit.Abilities = linkedAbilities[heroUnit.CUnitId].ToDictionary(x => x.ReferenceNameId, x => x);

                    foreach (Ability linkedAbility in linkedAbilities[heroUnit.CUnitId].ToList())
                        hero.Abilities.Remove(linkedAbility.ReferenceNameId);
                }
            }
        }
    }
}
