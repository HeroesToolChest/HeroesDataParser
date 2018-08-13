using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Parser.Exceptions;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData
{
    public class HeroParser
    {
        private readonly double DefaultWeaponPeriod = 1.2; // for hero weapons
        private readonly string UITooltipAbilLookupPrefix = "UI/Tooltip/Abil/";
        private readonly string DefaultAbilityTalentEnergyText;
        private readonly string DefaultHeroEnergyText;
        private readonly string AbilTooltipCooldownText;
        private readonly string AbilTooltipCooldownPluralText;
        private readonly string StringChargeCooldownColon;
        private readonly string StringCooldownColon;

        private readonly GameData GameData;
        private readonly GameStringData GameStringData;
        private readonly ParsedGameStrings ParsedGameStrings;
        private readonly OverrideData OverrideData;

        private HeroOverride HeroOverride = new HeroOverride();

        public HeroParser(GameData gameData, GameStringData gameStringData, ParsedGameStrings parsedGameStrings, OverrideData overrideData)
        {
            GameData = gameData;
            GameStringData = gameStringData;
            ParsedGameStrings = parsedGameStrings;
            OverrideData = overrideData;

            AbilTooltipCooldownText = ParsedGameStrings.TooltipsByKeyString["UI/AbilTooltipCooldown"];
            AbilTooltipCooldownPluralText = ParsedGameStrings.TooltipsByKeyString["UI/AbilTooltipCooldownPlural"];
            StringChargeCooldownColon = ParsedGameStrings.TooltipsByKeyString["e_gameUIStringChargeCooldownColon"];
            StringCooldownColon = ParsedGameStrings.TooltipsByKeyString["e_gameUIStringCooldownColon"];
            DefaultAbilityTalentEnergyText = ParsedGameStrings.TooltipsByKeyString["UI/Tooltip/Abil/Mana"];
            DefaultHeroEnergyText = ParsedGameStrings.TooltipsByKeyString["UI/HeroEnergyType/Mana"];
        }

        /// <summary>
        /// Gets or sets the hots build number.
        /// </summary>
        public int? HotsBuild { get; set; }

        /// <summary>
        /// Parses the hero's game data.
        /// </summary>
        /// <param name="cHeroId">The id value of the CHero element.</param>
        /// <param name="cUnitId">The id value of the CUnit element.</param>
        /// <returns></returns>
        public Hero Parse(string cHeroId, string cUnitId)
        {
            Hero hero = new Hero
            {
                Name = GameStringData.HeroNamesByShortName[$"{GameStringPrefixes.HeroNamePrefix}{cHeroId}"],
                Description = new TooltipDescription(ParsedGameStrings.HeroParsedDescriptionsByShortName[$"{GameStringPrefixes.DescriptionPrefix}{cHeroId}"]),
                CHeroId = cHeroId,
                CUnitId = cUnitId,
            };

            HeroOverride heroOverride = OverrideData.HeroOverride(cHeroId);
            if (heroOverride != null)
                HeroOverride = heroOverride;

            SetDefaultValues(hero);
            CActorData(hero);
            CHeroData(hero);
            CUnitData(hero);
            SetHeroPortraits(hero);
            SetUnitArmor(hero);
            AddSubHeroCUnits(hero);

            ApplyOverrides(hero, HeroOverride);
            MoveParentLinkedAbilities(hero);
            MoveParentLinkedWeapons(hero);

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

            hero.Energy.EnergyType = DefaultHeroEnergyText;
            hero.ReleaseDate = new DateTime(2014, 3, 13);
            hero.Gender = HeroGender.Male;
            hero.Type = UnitType.Ranged;
            hero.Franchise = HeroFranchise.Unknown;
        }

        private void CHeroData(Hero hero)
        {
            XElement heroData = GameData.XmlGameData.Root.Elements("CHero").Where(x => x.Attribute("id")?.Value == hero.CHeroId).FirstOrDefault();
            IEnumerable<XElement> layoutButtons = GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("id")?.Value != "TargetHeroDummy").Elements("CardLayouts").Elements("LayoutButtons");

            if (heroData == null)
                return;

            // get short name of hero
            string hyperLinkElement = heroData.Element("HyperlinkId")?.Attribute("value")?.Value;
            if (!string.IsNullOrEmpty(hyperLinkElement))
                hero.ShortName = hyperLinkElement;
            else
                hero.ShortName = hero.CHeroId;

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
                else if (elementName == "UNIVERSEICON")
                {
                    string iconImage = Path.GetFileName(PathExtensions.GetFilePath(element.Attribute("value").Value)).ToUpper();

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
            }

            // abilities must be gotten before talents
            foreach (XElement abilArrayElement in heroData.Elements("HeroAbilArray"))
            {
                GetAbility(hero, abilArrayElement, layoutButtons);
            }

            foreach (XElement talentArrayElement in heroData.Elements("TalentTreeArray"))
            {
                GetTalent(hero, talentArrayElement);
            }

            if (hero.Type == UnitType.Unknown)
                hero.Type = UnitType.Ranged;

            SetAdditionalLinkedAbilities(hero);
        }

        private void CUnitData(Hero hero)
        {
            XElement heroData = GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("id")?.Value == hero.CUnitId).FirstOrDefault();

            if (heroData == null)
                return;

            // loop through all elements and set found elements
            foreach (XElement element in heroData.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "LIFEMAX")
                {
                    hero.Life.LifeMax = double.Parse(element.Attribute("value").Value);

                    double? scaleValue = GameData.GetScaleValue(("Unit", hero.CUnitId, "LifeMax"));
                    if (scaleValue.HasValue)
                        hero.Life.LifeScaling = scaleValue.Value;
                }
                else if (elementName == "LIFEREGENRATE")
                {
                    hero.Life.LifeRegenerationRate = double.Parse(element.Attribute("value").Value);

                    double? scaleValue = GameData.GetScaleValue(("Unit", hero.CUnitId, "LifeRegenRate"));
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
                hero.Energy.EnergyType = string.Empty;

            if (hero.Difficulty == HeroDifficulty.Unknown)
                hero.Difficulty = HeroDifficulty.Easy;
        }

        private void CActorData(Hero hero)
        {
            XElement heroData = GameData.XmlGameData.Root.Elements("CActorUnit").Where(x => x.Attribute("id")?.Value == hero.CUnitId).FirstOrDefault();

            if (heroData == null)
                return;

            // find special energy type
            foreach (XElement vitalName in heroData.Elements("VitalNames"))
            {
                string indexValue = vitalName.Attribute("index")?.Value;
                string valueValue = vitalName.Attribute("value")?.Value;
                if (indexValue == "Energy")
                {
                    if (ParsedGameStrings.TooltipsByKeyString.TryGetValue(valueValue, out string energyType))
                        hero.Energy.EnergyType = energyType;
                }
            }
        }

        private void GetAbility(Hero hero, XElement abilityElement, IEnumerable<XElement> layoutButtons)
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

            // check for the HeroAbilArray button value
            if (!string.IsNullOrEmpty(tooltipName) && HeroOverride.NewButtonValueByHeroAbilArrayButton.TryGetValue(tooltipName, out string setButtonValue))
                tooltipName = setButtonValue;

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

            SetAbilityTalentName(ability, ability.ButtonName);

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

            SetAbilityTalentIcon(ability);
            SetTooltipSubInfo(hero, referenceName, ability);
            SetTooltipDescriptions(hero, ability);
            SetAbilityTypeForAbility(hero.CUnitId, ability, layoutButtons);

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

                SetAbilityTalentName(talent, talent.FullTooltipNameId);
            }

            SetAbilityTalentIcon(talent);

            XElement talentAbilElement = cTalentElement.Elements("Abil").FirstOrDefault();
            XElement talentActiveElement = cTalentElement.Elements("Active").FirstOrDefault();
            if (talentAbilElement != null && talentActiveElement != null)
            {
                string effectId = talentAbilElement.Attribute("value").Value;

                if (talentActiveElement.Attribute("value").Value == "1")
                    SetTooltipSubInfo(hero, effectId, talent);
            }

            SetTooltipDescriptions(hero, talent);
            SetAbilityTypeForTalent(hero, talent, cTalentElement);

            hero.Talents.Add(referenceName, talent);
        }

        private void SetAbilityTalentName(AbilityTalentBase abilityTalentBase, string tooltipName)
        {
            // check for name override
            XElement cButtonElement = GameData.XmlGameData.Root.Elements("CButton").Where(x => x.Attribute("id")?.Value == abilityTalentBase.FullTooltipNameId).FirstOrDefault();
            if (cButtonElement != null)
            {
                XElement buttonNameElement = cButtonElement.Element("Name");
                if (buttonNameElement != null)
                    tooltipName = Path.GetFileName(PathExtensions.GetFilePath(buttonNameElement.Attribute("value").Value)); // override
            }

            if (ParsedGameStrings.TryGetAbilityTalentParsedNames(tooltipName, out string abilityTalentName))
                abilityTalentBase.Name = abilityTalentName;
            else
                throw new ParseException($"{nameof(SetAbilityTalentName)}: {tooltipName} not found in description names");
        }

        private void SetAbilityTalentIcon(AbilityTalentBase abilityTalentBase)
        {
            XElement cButtonElement = GameData.XmlGameData.Root.Elements("CButton").Where(x => x.Attribute("id")?.Value == abilityTalentBase.FullTooltipNameId).FirstOrDefault();
            if (cButtonElement != null)
            {
                XElement buttonIconElement = cButtonElement.Element("Icon");
                if (buttonIconElement != null)
                    abilityTalentBase.IconFileName = Path.GetFileName(PathExtensions.GetFilePath(buttonIconElement.Attribute("value").Value));
            }
        }

        /// <summary>
        /// Sets all the tooltip info: vital costs, cooldowns, charges.
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="elementId"></param>
        /// <param name="abilityTalentBase"></param>
        private void SetTooltipSubInfo(Hero hero, string elementId, AbilityTalentBase abilityTalentBase)
        {
            if (string.IsNullOrEmpty(elementId))
                return;

            var foundElements = GameData.XmlGameData.Root.Elements().Where(x => x.Attribute("id")?.Value == elementId);

            // look through all elements to find the tooltip info
            foreach (XElement element in foundElements)
            {
                if (element.Name.LocalName == "CButton" || element.Name.LocalName == "CWeaponLegacy")
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
                                    replaceText = StringChargeCooldownColon; // Charge Cooldown:<space>
                                else
                                    replaceText = StringCooldownColon; // Cooldown:<space>

                                if (cooldownValue == "1")
                                    abilityTalentBase.Tooltip.Cooldown.CooldownText = new TooltipDescription(AbilTooltipCooldownText.Replace(StringCooldownColon, replaceText).Replace("%1", cooldownValue));
                                else
                                    abilityTalentBase.Tooltip.Cooldown.CooldownText = new TooltipDescription(AbilTooltipCooldownPluralText.Replace(StringCooldownColon, replaceText).Replace("%1", cooldownValue));
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
                                    replaceText = StringChargeCooldownColon; // Charge Cooldown:<space>
                                else
                                    replaceText = StringCooldownColon; // Cooldown:<space>

                                if (!string.IsNullOrEmpty(cooldownValue))
                                {
                                    if (cooldownValue == "1")
                                        abilityTalentBase.Tooltip.Cooldown.CooldownText = new TooltipDescription(AbilTooltipCooldownText.Replace(StringCooldownColon, StringChargeCooldownColon).Replace("%1", cooldownValue));
                                    else
                                        abilityTalentBase.Tooltip.Cooldown.CooldownText = new TooltipDescription(AbilTooltipCooldownPluralText.Replace(StringCooldownColon, StringChargeCooldownColon).Replace("%1", cooldownValue));
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
                                    abilityTalentBase.Tooltip.Cooldown.CooldownText = new TooltipDescription(AbilTooltipCooldownText.Replace("%1", cooldownValue));
                                else if (cooldown >= 1)
                                    abilityTalentBase.Tooltip.Cooldown.CooldownText = new TooltipDescription(AbilTooltipCooldownPluralText.Replace("%1", cooldownValue));

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

                            if (ParsedGameStrings.TooltipsByKeyString.TryGetValue($"{UITooltipAbilLookupPrefix}{hero.Energy.EnergyType}", out string energyText))
                                abilityTalentBase.Tooltip.Energy.EnergyText = new TooltipDescription(DescriptionValidator.Validate(energyText.Replace("%1", vitalValue)));
                            else
                                abilityTalentBase.Tooltip.Energy.EnergyText = new TooltipDescription(DescriptionValidator.Validate(DefaultAbilityTalentEnergyText.Replace("%1", vitalValue)));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets tooltip descriptions and override texts for vitals and cooldowns.
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="abilityTalentBase"></param>
        private void SetTooltipDescriptions(Hero hero, AbilityTalentBase abilityTalentBase)
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
                        if (ParsedGameStrings.TooltipsByKeyString.TryGetValue(cButtonTooltipVitalNameElement.Attribute("value").Value, out string overrideVitalName))
                        {
                            if (string.IsNullOrEmpty(abilityTalentBase.Tooltip.Life.LifeCostText?.RawDescription))
                                abilityTalentBase.Tooltip.Life.LifeCostText = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName));
                            else if (overrideVitalName.Contains("%1"))
                                abilityTalentBase.Tooltip.Life.LifeCostText = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace("%1", abilityTalentBase.Tooltip.Life.LifeValue.ToString())));
                            else
                                abilityTalentBase.Tooltip.Life.LifeCostText = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace("%1", abilityTalentBase.Tooltip.Life.LifeCostText.RawDescription)));
                        }
                    }
                    else if (index == "Energy")
                    {
                        if (ParsedGameStrings.TooltipsByKeyString.TryGetValue(cButtonTooltipVitalNameElement.Attribute("value").Value, out string overrideVitalName))
                        {
                            if (string.IsNullOrEmpty(abilityTalentBase.Tooltip.Energy.EnergyText?.RawDescription))
                                abilityTalentBase.Tooltip.Energy.EnergyText = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName));
                            else if (overrideVitalName.Contains("%1"))
                                abilityTalentBase.Tooltip.Energy.EnergyText = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace("%1", abilityTalentBase.Tooltip.Energy.EnergyValue.ToString())));
                            else
                                abilityTalentBase.Tooltip.Energy.EnergyText = new TooltipDescription(DescriptionValidator.Validate(overrideVitalName.Replace("%1", abilityTalentBase.Tooltip.Energy.EnergyText.RawDescription)));
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
                            // check if overriding text has starts with the energy text
                            if (!new TooltipDescription(DescriptionValidator.Validate(text)).PlainText.StartsWith(hero.Energy.EnergyType))
                            {
                                if (ParsedGameStrings.TooltipsByKeyString.TryGetValue($"{UITooltipAbilLookupPrefix}{hero.Energy.EnergyType}", out string energyText))
                                    text = DescriptionValidator.Validate(energyText.Replace("%1", text)); // add it as the default
                            }

                            abilityTalentBase.Tooltip.Energy.EnergyText = new TooltipDescription(text);
                        }
                        else if (cButtonTooltipVitalElement.Attribute("index")?.Value == "Life")
                        {
                            abilityTalentBase.Tooltip.Life.LifeCostText = new TooltipDescription(DescriptionValidator.Validate(abilityTalentBase.Tooltip.Life.LifeCostText.RawDescription.Replace("%1", text)));
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
                        if (!text.StartsWith(StringCooldownColon))
                            text = $"{StringCooldownColon}{text}";

                        abilityTalentBase.Tooltip.Cooldown.CooldownText = new TooltipDescription(text);
                    }
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

                if (ParsedGameStrings.TryGetAbilityTalentParsedNames(linkName, out string abilityName))
                    ability.Name = abilityName;

                SetAbilityTalentIcon(ability);
                SetTooltipSubInfo(hero, linkName, ability);
                SetTooltipDescriptions(hero, ability);

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

                double? scaleValue = GameData.GetScaleValue(("Effect", displayEffectValue, "Amount"));
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

        private void SetUnitArmor(Unit unit)
        {
            string name = unit.ShortName;
            if (name.EndsWith("Form"))
                name = name.Remove(name.LastIndexOf("Form"), 4);

            XElement armorElement = GameData.XmlGameData.Root.Elements("CArmor").Where(x => x.Attribute("id")?.Value == $"{name}Armor").FirstOrDefault();
            XElement physicalArmorElement = GameData.XmlGameData.Root.Elements("CArmor").Where(x => x.Attribute("id")?.Value == $"{name}PhysicalArmor").FirstOrDefault();
            XElement spellArmorElement = GameData.XmlGameData.Root.Elements("CArmor").Where(x => x.Attribute("id")?.Value == $"{name}SpellArmor").FirstOrDefault();

            if (armorElement != null)
            {
                UnitArmorAddValue(armorElement, unit);
            }

            if (physicalArmorElement != null)
            {
                UnitArmorAddValue(physicalArmorElement, unit);
            }

            if (spellArmorElement != null)
            {
                UnitArmorAddValue(spellArmorElement, unit);
            }
        }

        private void UnitArmorAddValue(XElement armorElement, Unit unit)
        {
            unit.Armor = unit.Armor ?? new UnitArmor();

            XElement basicElement = armorElement.Element("ArmorSet").Elements("ArmorMitigationTable").Where(x => x.Attribute("index")?.Value == "Basic").FirstOrDefault();
            XElement abilityElement = armorElement.Element("ArmorSet").Elements("ArmorMitigationTable").Where(x => x.Attribute("index")?.Value == "Ability").FirstOrDefault();

            if (basicElement != null && int.TryParse(basicElement.Attribute("value").Value, out int armorValue))
            {
                unit.Armor.PhysicalArmor = armorValue;
            }

            if (abilityElement != null && int.TryParse(abilityElement.Attribute("value").Value, out armorValue))
            {
                unit.Armor.SpellArmor = armorValue;
            }
        }

        private void SetAbilityTypeForAbility(string cUnit, Ability ability, IEnumerable<XElement> layoutButtons)
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
                    throw new ParseException($"Unknown slot type (attribute) for ability type: {slot} - Hero(CUnit): {cUnit} - Ability: {ability.ReferenceNameId}");

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
                    throw new ParseException($"Unknown slot type (element) for ability type: {slot} - Hero(CUnit): {cUnit} - Ability: {ability.ReferenceNameId}");

                return;
            }

            ability.AbilityType = AbilityType.Active;
        }

        private void SetAbilityTypeForTalent(Hero hero, Talent talent, XElement talentElement)
        {
            XElement talentTraitElement = talentElement.Element("Trait");
            XElement talentAbilElement = talentElement.Element("Abil");
            XElement talentActiveElement = talentElement.Element("Active");
            XElement talentQuestElement = talentElement.Element("QuestData");

            if (talentTraitElement != null && talentTraitElement.Attribute("value")?.Value == "1")
            {
                talent.AbilityType = AbilityType.Trait;
            }
            else if (talentAbilElement != null)
            {
                string abilValue = talentAbilElement.Attribute("value").Value;
                if (hero.Abilities.TryGetValue(abilValue, out Ability ability))
                    talent.AbilityType = ability.AbilityType;
                else
                    talent.AbilityType = AbilityType.Active;
            }
            else
            {
                talent.AbilityType = AbilityType.Passive;
            }

            if (talentActiveElement != null && talentActiveElement.Attribute("value")?.Value == "1")
                talent.IsActive = true;

            if (talentQuestElement != null && !string.IsNullOrEmpty(talentQuestElement.Attribute("StackBehavior")?.Value))
                talent.IsQuest = true;
        }

        private void AddSubHeroCUnits(Hero hero)
        {
            foreach (string unit in HeroOverride.HeroUnits)
            {
                XElement cUnit = GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("id")?.Value == unit).FirstOrDefault();

                string name = string.Empty;
                if (!GameStringData.UnitNamesByShortName.TryGetValue($"{GameStringPrefixes.UnitPrefix}{unit}", out name))
                {
                    if (!GameStringData.AbilityTalentNamesByReferenceNameId.TryGetValue($"{GameStringPrefixes.DescriptionNamePrefix}{unit}", out name))
                        name = unit;
                }

                Hero heroUnit = new Hero
                {
                    Name = name,
                    ShortName = unit.StartsWith("Hero") ? unit.Remove(0, 4) : unit,
                    CHeroId = null,
                    CUnitId = unit,
                    HeroPortrait = null,
                };

                if (cUnit != null)
                {
                    if (ParsedGameStrings.TryGetHeroParsedDescriptions(unit, out string desc))
                        heroUnit.Description = new TooltipDescription(desc);

                    SetDefaultValues(heroUnit);
                    CUnitData(heroUnit);
                }

                SetUnitArmor(heroUnit);

                HeroOverride heroOverride = OverrideData.HeroOverride(unit);
                if (heroOverride != null)
                    ApplyOverrides(heroUnit, heroOverride);

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
                    if (heroOverride.PropertyAbilityOverrideMethodByAbilityId.TryGetValue(ability.Key, out Dictionary<string, Action<Ability>> valueOverrideMethods))
                    {
                        foreach (var propertyOverride in valueOverrideMethods)
                        {
                            // execute each property override
                            propertyOverride.Value(ability.Value);
                        }
                    }
                }
            }

            // talents
            if (hero.Talents != null)
            {
                foreach (KeyValuePair<string, Talent> talents in hero.Talents)
                {
                    if (heroOverride.PropertyTalentOverrideMethodByTalentId.TryGetValue(talents.Key, out Dictionary<string, Action<Talent>> valueOverrideMethods))
                    {
                        foreach (var propertyOverride in valueOverrideMethods)
                        {
                            // execute each property override
                            propertyOverride.Value(talents.Value);
                        }
                    }
                }
            }

            // weapons
            if (hero.Weapons != null)
            {
                foreach (UnitWeapon weapon in hero.Weapons)
                {
                    if (heroOverride.PropertyWeaponOverrideMethodByWeaponId.TryGetValue(weapon.WeaponNameId, out Dictionary<string, Action<UnitWeapon>> valueOverrideMethods))
                    {
                        foreach (var propertyOverride in valueOverrideMethods)
                        {
                            // execute each property override
                            propertyOverride.Value(weapon);
                        }
                    }
                }
            }

            if (hero.HeroPortrait != null)
            {
                if (heroOverride.PropertyPortraitOverrideMethodByCHeroId.TryGetValue(hero.CHeroId, out Dictionary<string, Action<HeroPortrait>> valueOverrideMethods))
                {
                    foreach (var propertyOverride in valueOverrideMethods)
                    {
                        propertyOverride.Value(hero.HeroPortrait);
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

        private void MoveParentLinkedWeapons(Hero hero)
        {
            ILookup<string, UnitWeapon> linkedWeapons = hero.ParentLinkedWeapons();
            if (linkedWeapons.Count > 0)
            {
                foreach (Hero heroUnit in hero.HeroUnits)
                {
                    heroUnit.Weapons = linkedWeapons[heroUnit.CUnitId].ToList();

                    foreach (UnitWeapon linkedWeapon in linkedWeapons[heroUnit.CUnitId].ToList())
                        hero.Weapons.Remove(linkedWeapon);
                }
            }
        }

        private void SetHeroPortraits(Hero hero)
        {
            hero.HeroPortrait.HeroSelectPortraitFileName = $"{PortraitFileNames.HeroSelectPortraitPrefix}{hero.CHeroId.ToLower()}.dds";
            hero.HeroPortrait.LeaderboardPortraitFileName = $"{PortraitFileNames.LeaderboardPortraitPrefix}{hero.CHeroId.ToLower()}.dds";
            hero.HeroPortrait.LoadingScreenPortraitFileName = $"{PortraitFileNames.LoadingPortraitPrefix}{hero.CHeroId.ToLower()}.dds";
            hero.HeroPortrait.PartyPanelPortraitFileName = $"{PortraitFileNames.PartyPanelPortraitPrefix}{hero.CHeroId.ToLower()}.dds";
            hero.HeroPortrait.TargetPortraitFileName = $"{PortraitFileNames.TargetPortraitPrefix}{hero.CHeroId.ToLower()}.dds";
        }
    }
}
