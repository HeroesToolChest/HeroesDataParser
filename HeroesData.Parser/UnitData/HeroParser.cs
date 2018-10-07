using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.UnitData.Data;
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
        private readonly GameData GameData;
        private readonly GameStringData GameStringData;
        private readonly ParsedGameStrings ParsedGameStrings;
        private readonly OverrideData OverrideData;
        private readonly TextValueData TextValueData;

        private HeroOverride HeroOverride;

        private WeaponData WeaponData;
        private ArmorData ArmorData;
        private AbilityData AbilityData;
        private TalentData TalentData;

        public HeroParser(GameData gameData, GameStringData gameStringData, ParsedGameStrings parsedGameStrings, OverrideData overrideData)
        {
            GameData = gameData;
            GameStringData = gameStringData;
            ParsedGameStrings = parsedGameStrings;
            OverrideData = overrideData;
            TextValueData = new TextValueData(parsedGameStrings);
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

            HeroOverride = OverrideData.HeroOverride(cHeroId) ?? new HeroOverride();

            SetDefaultValues(hero);
            CActorData(hero);

            WeaponData = new WeaponData(GameData, HeroOverride);
            ArmorData = new ArmorData(GameData);
            AbilityData = new AbilityData(GameData, HeroOverride, ParsedGameStrings, TextValueData);
            TalentData = new TalentData(GameData, HeroOverride, ParsedGameStrings, TextValueData);

            CHeroData(hero);
            CUnitData(hero);
            SetHeroPortraits(hero);
            AddSubHeroCUnits(hero);

            ApplyOverrides(hero, HeroOverride);
            MoveParentLinkedAbilities(hero);
            MoveParentLinkedWeapons(hero);

            return hero;
        }

        private void SetDefaultValues(Hero hero)
        {
            XElement stormHeroData = GameData.XmlGameData.Root.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == "StormHero");
            if (stormHeroData != null)
            {
                string parentDataValue = stormHeroData.Attribute("parent").Value;
                XElement stormBasicHeroicUnitData = GameData.XmlGameData.Root.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == parentDataValue);

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

            hero.Energy.EnergyType = TextValueData.DefaultHeroEnergyText;
            TextValueData.HeroEnergyTypeEnglish = TextValueData.UIHeroEnergyTypeMana.Split('/').Last();
            hero.ReleaseDate = new DateTime(2014, 3, 13);
            hero.Gender = HeroGender.Male;
            hero.Type = TextValueData.StringRanged;
            hero.Franchise = HeroFranchise.Unknown;
        }

        private void CHeroData(Hero hero)
        {
            XElement heroData = GameData.XmlGameData.Root.Elements("CHero").FirstOrDefault(x => x.Attribute("id")?.Value == hero.CHeroId);
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
                        hero.Type = TextValueData.StringMelee;
                    else if (element.Attribute("value").Value == "0")
                        hero.Type = TextValueData.StringRanged;
                    else
                        hero.Type = TextValueData.StringRanged;
                }
                else if (elementName == "DIFFICULTY")
                {
                    string difficultyValue = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(difficultyValue))
                    {
                        if (ParsedGameStrings.TooltipsByKeyString.TryGetValue($"{TextValueData.UIHeroUtilDifficultyPrefix}{difficultyValue}", out string difficulty))
                            hero.Difficulty = difficulty;
                    }
                }
                else if (elementName == "ROLE" || elementName == "ROLESMULTICLASS")
                {
                    string roleValue = element.Attribute("value")?.Value;

                    if (hero.Roles == null)
                        hero.Roles = new List<string>();

                    if (ParsedGameStrings.TooltipsByKeyString.TryGetValue($"{TextValueData.UIHeroUtilRolePrefix}{roleValue}", out string role))
                    {
                        if (!hero.Roles.Contains(role))
                            hero.Roles.Add(role);
                    }
                    else
                    {
                        hero.Roles.Add("Unknown");
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
                AbilityData.SetAbilityData(hero, abilArrayElement, layoutButtons);
            }

            foreach (XElement talentArrayElement in heroData.Elements("TalentTreeArray"))
            {
                TalentData.SetTalentData(hero, talentArrayElement);
            }

            if (string.IsNullOrEmpty(hero.Type))
                hero.Type = TextValueData.StringRanged;

            SetAdditionalLinkedAbilities(hero);
        }

        private void CUnitData(Hero hero)
        {
            XElement heroData = GameData.XmlGameData.Root.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == hero.CUnitId);

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
                else if (elementName == "SPEED")
                {
                    hero.Speed = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "SIGHT")
                {
                    hero.Sight = double.Parse(element.Attribute("value").Value);
                }
            }

            // set weapons
            WeaponData.SetHeroWeaponData(hero, heroData.Elements("WeaponArray").Where(x => x.Attribute("Link") != null));
            ArmorData.SetUnitArmorData(hero, heroData.Element("ArmorLink"));

            if (hero.Energy.EnergyMax < 1)
                hero.Energy.EnergyType = string.Empty;

            if (string.IsNullOrEmpty(hero.Difficulty))
                hero.Difficulty = TextValueData.DefaultHeroDifficulty;
        }

        private void CActorData(Hero hero)
        {
            XElement heroData = GameData.XmlGameData.Root.Elements("CActorUnit").FirstOrDefault(x => x.Attribute("id")?.Value == hero.CUnitId);

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
                    {
                        hero.Energy.EnergyType = energyType;
                        TextValueData.HeroEnergyTypeEnglish = valueValue.Split('/').Last();
                    }
                }
            }
        }

        private void SetAdditionalLinkedAbilities(Hero hero)
        {
            foreach (KeyValuePair<string, string> linkedAbility in HeroOverride.LinkedElementNamesByAbilityId)
            {
                AbilityData.SetLinkedAbility(hero, linkedAbility.Key, linkedAbility.Value);
            }
        }

        private void AddSubHeroCUnits(Hero hero)
        {
            foreach (string unit in HeroOverride.HeroUnits)
            {
                XElement cUnit = GameData.XmlGameData.Root.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == unit);

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
            if (linkedAbilities?.Count > 0)
            {
                foreach (Hero heroUnit in hero.HeroUnits)
                {
                    heroUnit.Abilities = linkedAbilities[heroUnit.CUnitId].ToDictionary(x => x.ReferenceNameId, x => x);

                    foreach (Ability linkedAbility in linkedAbilities[heroUnit.CUnitId])
                        hero.Abilities.Remove(linkedAbility.ReferenceNameId);
                }
            }
        }

        private void MoveParentLinkedWeapons(Hero hero)
        {
            ILookup<string, UnitWeapon> linkedWeapons = hero.ParentLinkedWeapons();
            if (linkedWeapons?.Count > 0)
            {
                foreach (Hero heroUnit in hero.HeroUnits)
                {
                    heroUnit.Weapons = linkedWeapons[heroUnit.CUnitId].ToList();

                    foreach (UnitWeapon linkedWeapon in linkedWeapons[heroUnit.CUnitId])
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
