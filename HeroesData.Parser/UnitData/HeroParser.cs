using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.UnitData.Data;
using HeroesData.Parser.UnitData.Overrides;
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
        private readonly DefaultData DefaultData;
        private readonly OverrideData OverrideData;

        private HeroOverride HeroOverride;
        private WeaponData WeaponData;
        private ArmorData ArmorData;
        private AbilityData AbilityData;
        private TalentData TalentData;

        public HeroParser(GameData gameData, DefaultData defaultData, OverrideData overrideData)
        {
            GameData = gameData;
            DefaultData = defaultData;
            OverrideData = overrideData;
        }

        /// <summary>
        /// Gets or sets the hots build number.
        /// </summary>
        public int? HotsBuild { get; set; }

        /// <summary>
        /// Gets or sets the localization.
        /// </summary>
        public Localization Localization { get; set; }

        /// <summary>
        /// Gets or sets the base hero data.
        /// </summary>
        public Hero StormHeroBase { get; set; } = new Hero();

        /// <summary>
        /// Parses the hero's game data.
        /// </summary>
        /// <param name="cHeroId">The id value of the CHero element.</param>
        /// <returns></returns>
        public Hero Parse(string cHeroId)
        {
            Hero hero = new Hero
            {
                Name = GameData.GetGameString(DefaultData.HeroName.Replace(DefaultData.IdReplacer, cHeroId)),
                Description = new TooltipDescription(GameData.GetGameString(DefaultData.HeroDescription.Replace(DefaultData.IdReplacer, cHeroId)), Localization),
                CHeroId = cHeroId,
            };

            HeroOverride = OverrideData.HeroOverride(cHeroId) ?? new HeroOverride();

            WeaponData = new WeaponData(GameData, DefaultData, HeroOverride);
            ArmorData = new ArmorData(GameData);
            AbilityData = new AbilityData(GameData, DefaultData, HeroOverride, Localization);
            TalentData = new TalentData(GameData, DefaultData, HeroOverride, Localization);

            SetDefaultValues(hero);
            CActorData(hero);

            CHeroData(hero);
            CUnitData(hero);

            AddSubHeroCUnits(hero);

            FinalizeDataChecks(hero);

            ApplyOverrides(hero, HeroOverride);
            MoveParentLinkedAbilities(hero);
            MoveParentLinkedWeapons(hero);

            return hero;
        }

        /// <summary>
        /// Parses the base hero data.
        /// </summary>
        /// <returns></returns>
        public Hero ParseBaseHero()
        {
            StormHeroBase = new Hero
            {
                ShortName = StormHero.CHeroId,
                CHeroId = StormHero.CHeroId,
                CUnitId = StormHero.CUnitId,
                Ratings = null,
            };

            HeroOverride = OverrideData.HeroOverride(StormHeroBase.CHeroId) ?? new HeroOverride();
            AbilityData = new AbilityData(GameData, DefaultData, HeroOverride, Localization);

            SetBaseHeroData(StormHeroBase);

            ApplyOverrides(StormHeroBase, HeroOverride);
            MoveParentLinkedAbilities(StormHeroBase);

            IList<Ability> hearthAbilties = StormHeroBase.PrimaryAbilities(AbilityTier.Hearth);
            IList<Ability> mountAbilties = StormHeroBase.PrimaryAbilities(AbilityTier.Mount);

            // based on the _stormhero data in hero-overrides.xml
            DefaultData.DefaultHearthAbilityId = hearthAbilties[0].ReferenceNameId;
            DefaultData.DefaultHearthNoManaAbilityId = hearthAbilties[1].ReferenceNameId;
            DefaultData.DefaultSummonMountAbilityId = mountAbilties[0].ReferenceNameId;

            return StormHeroBase;
        }

        private void SetDefaultValues(Hero hero)
        {
            hero.Type = GameData.GetGameString(DefaultData.StringRanged).Trim();
            hero.Radius = DefaultData.UnitRadius;
            hero.Speed = DefaultData.UnitSpeed;
            hero.Sight = DefaultData.UnitSight;
            hero.ReleaseDate = DefaultData.ReleaseDate;
            hero.Gender = HeroGender.Male;
            hero.Franchise = HeroFranchise.Unknown;
            hero.Life.LifeMax = DefaultData.UnitLifeMax;
            hero.Life.LifeRegenerationRate = 0;
            hero.Energy.EnergyType = GameData.GetGameString(DefaultData.HeroEnergyTypeManaText);
            hero.Energy.EnergyMax = DefaultData.UnitEnergyMax;
            hero.Energy.EnergyRegenerationRate = DefaultData.UnitEnergyRegenRate;
            hero.Difficulty = GameData.GetGameString(DefaultData.Difficulty.Replace(DefaultData.IdReplacer, DefaultData.DefaultHeroDifficulty)).Trim();
            hero.HearthLinkId = DefaultData.DefaultHearthAbilityId;

            if (hero.CHeroId != null)
            {
                hero.ShortName = DefaultData.HeroHyperlinkId.Replace(DefaultData.IdReplacer, hero.CHeroId);
                hero.CUnitId = DefaultData.HeroUnit.Replace(DefaultData.IdReplacer, hero.CHeroId);

                hero.HeroPortrait.HeroSelectPortraitFileName = Path.GetFileName(PathExtensions.GetFilePath(DefaultData.HeroSelectScreenButtonImage.Replace(DefaultData.IdReplacer, hero.CHeroId))).ToLower();
                hero.HeroPortrait.LeaderboardPortraitFileName = Path.GetFileName(PathExtensions.GetFilePath(DefaultData.HeroLeaderboardImage.Replace(DefaultData.IdReplacer, hero.CHeroId))).ToLower();
                hero.HeroPortrait.LoadingScreenPortraitFileName = Path.GetFileName(PathExtensions.GetFilePath(DefaultData.HeroLoadingScreenImage.Replace(DefaultData.IdReplacer, hero.CHeroId))).ToLower();
                hero.HeroPortrait.PartyPanelPortraitFileName = Path.GetFileName(PathExtensions.GetFilePath(DefaultData.HeroPartyPanelButtonImage.Replace(DefaultData.IdReplacer, hero.CHeroId))).ToLower();
                hero.HeroPortrait.TargetPortraitFileName = Path.GetFileName(PathExtensions.GetFilePath(DefaultData.HeroPortrait.Replace(DefaultData.IdReplacer, hero.CHeroId))).ToLower();

                if (HeroOverride.CUnitOverride.Enabled)
                    hero.CUnitId = HeroOverride.CUnitOverride.CUnit;
            }
        }

        // used to acquire the hero's unique energy type
        private void CActorData(Hero hero)
        {
            XElement actorUnitElement = GameData.XmlGameData.Root.Elements("CActorUnit").FirstOrDefault(x => x.Attribute("id")?.Value == hero.CUnitId);

            if (actorUnitElement == null)
                return;

            // find special energy type
            foreach (XElement vitalName in actorUnitElement.Elements("VitalNames"))
            {
                string indexValue = vitalName.Attribute("index")?.Value;
                string valueValue = vitalName.Attribute("value")?.Value;

                if (indexValue == "Energy")
                {
                    if (GameData.TryGetGameString(valueValue, out string energyType))
                    {
                        hero.Energy.EnergyType = energyType;
                    }
                }
            }
        }

        private void CHeroData(Hero hero)
        {
            XElement heroElement = GameData.XmlGameData.Root.Elements("CHero").FirstOrDefault(x => x.Attribute("id")?.Value == hero.CHeroId);

            if (heroElement == null)
                return;

            // loop through all elements and set found elements
            foreach (XElement element in heroElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "HYPERLINKID")
                {
                    hero.ShortName = element.Attribute("value")?.Value;
                }
                else if (elementName == "ATTRIBUTEID")
                {
                    hero.AttributeId = element.Attribute("value")?.Value;
                }
                else if (elementName == "MELEE")
                {
                    if (element.Attribute("value").Value == "1")
                        hero.Type = GameData.GetGameString(DefaultData.StringMelee).Trim();
                    else if (element.Attribute("value").Value == "0")
                        hero.Type = GameData.GetGameString(DefaultData.StringRanged).Trim();
                    else
                        hero.Type = GameData.GetGameString(DefaultData.StringRanged).Trim();
                }
                else if (elementName == "DIFFICULTY")
                {
                    string difficultyValue = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(difficultyValue))
                    {
                        hero.Difficulty = GameData.GetGameString(DefaultData.Difficulty.Replace(DefaultData.IdReplacer, difficultyValue)).Trim();
                    }
                }
                else if (elementName == "ROLE" || elementName == "ROLESMULTICLASS")
                {
                    string roleValue = element.Attribute("value")?.Value;

                    if (hero.Roles == null)
                        hero.Roles = new List<string>();

                    string role = GameData.GetGameString(DefaultData.HeroRoleName.Replace(DefaultData.IdReplacer, roleValue)).Trim();

                    if (!hero.Roles.Contains(role))
                        hero.Roles.Add(role);
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
                    else if (iconImage == "UI_GLUES_STORE_GAMEICON_NEXUS.DDS")
                        hero.Franchise = HeroFranchise.Nexus;
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
                    else if (universe == "RETRO" && hero.Franchise == HeroFranchise.Unknown)
                        hero.Franchise = HeroFranchise.Classic;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = DefaultData.ReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.ReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.ReleaseDate.Year;

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
                else if (elementName == "SELECTSCREENBUTTONIMAGE")
                {
                    hero.HeroPortrait.HeroSelectPortraitFileName = Path.GetFileName(PathExtensions.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "SCORESCREENIMAGE")
                {
                    hero.HeroPortrait.LeaderboardPortraitFileName = Path.GetFileName(PathExtensions.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "LOADINGSCREENIMAGE")
                {
                    hero.HeroPortrait.LoadingScreenPortraitFileName = Path.GetFileName(PathExtensions.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "PARTYPANELBUTTONIMAGE")
                {
                    hero.HeroPortrait.PartyPanelPortraitFileName = Path.GetFileName(PathExtensions.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "PORTRAIT")
                {
                    hero.HeroPortrait.TargetPortraitFileName = Path.GetFileName(PathExtensions.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
            }

            if (hero.ReleaseDate == DefaultData.ReleaseDate)
                hero.ReleaseDate = DefaultData.AlphaReleaseDate;

            // abilities must be gotten before talents
            foreach (XElement abilArrayElement in heroElement.Elements("HeroAbilArray"))
            {
                AbilityData.SetAbilityData(hero, abilArrayElement);
            }

            AbilityData.AddAdditionalButtonAbilities(hero);
            TalentData.SetButtonTooltipAppenderData(StormHeroBase, hero);

            foreach (XElement talentArrayElement in heroElement.Elements("TalentTreeArray"))
            {
                TalentData.SetTalentData(hero, talentArrayElement);
            }
        }

        private void CUnitData(Hero hero, XElement unitElement = null)
        {
            unitElement = unitElement ?? GameData.XmlGameData.Root.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == hero.CUnitId);

            if (unitElement == null)
                return;

            // parent lookup
            string parentValue = unitElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue) && parentValue != DefaultData.CUnitDefaultBaseId)
            {
                XElement parentElement = GameData.XmlGameData.Root.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == parentValue);
                if (parentElement != null)
                    CUnitData(hero, parentElement);
            }

            // loop through all elements and set found elements
            foreach (XElement element in unitElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "ABILARRAY")
                {
                    string link = element.Attribute("Link")?.Value;

                    if (link == DefaultData.AbilMountLinkId)
                        hero.MountLinkId = DefaultData.DefaultSummonMountAbilityId;
                }
                else if (elementName == "LIFEMAX")
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
                else if (elementName == "CARDLAYOUTS")
                {
                    foreach (XElement cardLayoutElement in element.Elements())
                    {
                        string cardLayoutElementName = cardLayoutElement.Name.LocalName.ToUpper();

                        if (cardLayoutElementName == "LAYOUTBUTTONS")
                        {
                            string cardLayoutFace = cardLayoutElement.Attribute("Face")?.Value;
                            string cardLayoutAbilCmd = cardLayoutElement.Attribute("AbilCmd")?.Value;

                            if (!string.IsNullOrEmpty(cardLayoutFace) && !string.IsNullOrEmpty(cardLayoutAbilCmd))
                            {
                                if (cardLayoutFace.StartsWith(DefaultData.DefaultHearthAbilityId) && cardLayoutAbilCmd.StartsWith(DefaultData.DefaultHearthAbilityId))
                                    hero.HearthLinkId = cardLayoutFace;
                            }
                        }
                    }
                }
            }

            // set weapons
            WeaponData.SetHeroWeapons(hero, unitElement.Elements("WeaponArray").Where(x => x.Attribute("Link") != null));

            // set armor
            ArmorData.SetUnitArmorData(hero, unitElement.Element("ArmorLink"));

            if (hero.Energy.EnergyMax < 1)
                hero.Energy.EnergyType = string.Empty;

            // set mount link if hero has a custom mount ability
            IList<Ability> mountAbilities = hero.PrimaryAbilities(AbilityTier.Mount);
            string parentAttribute = unitElement.Attribute("parent")?.Value;

            if (parentAttribute == "StormHeroMountedCustom" && mountAbilities.Count > 0)
                hero.MountLinkId = mountAbilities.FirstOrDefault()?.ReferenceNameId;
        }

        private void AddSubHeroCUnits(Hero hero)
        {
            foreach (string cUnitId in HeroOverride.HeroUnits)
            {
                if (!GameData.TryGetGameString(DefaultData.UnitName.Replace(DefaultData.IdReplacer, cUnitId), out string name))
                {
                    if (!GameData.TryGetGameString(DefaultData.ButtonName.Replace(DefaultData.IdReplacer, cUnitId), out name))
                        name = cUnitId;
                }

                Hero heroUnit = new Hero
                {
                    Name = name,
                    ShortName = cUnitId.StartsWith("Hero") ? cUnitId.Remove(0, 4) : cUnitId,
                    CHeroId = null,
                    CUnitId = cUnitId,
                    HeroPortrait = null,
                };

                XElement cUnitElement = GameData.XmlGameData.Root.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == cUnitId);
                if (cUnitElement != null)
                {
                    SetDefaultValues(heroUnit);
                    CActorData(heroUnit);
                    CUnitData(heroUnit);

                    FinalizeDataChecks(heroUnit);
                }

                HeroOverride heroOverride = OverrideData.HeroOverride(cUnitId);
                if (heroOverride != null)
                    ApplyOverrides(heroUnit, heroOverride);

                hero.HeroUnits.Add(heroUnit);
            }
        }

        private void SetBaseHeroData(Hero hero)
        {
            AbilityData.AddAdditionalButtonAbilities(hero);
        }

        private void FinalizeDataChecks(Hero hero)
        {
            if (hero.Life.LifeMax == 1 && hero.Life.LifeRegenerationRate == 0 && hero.Life.LifeRegenerationRateScaling == 0 && hero.Life.LifeScaling == 0)
                hero.Life.LifeMax = 0;
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
    }
}
