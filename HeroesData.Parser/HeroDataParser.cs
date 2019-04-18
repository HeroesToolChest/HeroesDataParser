using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class HeroDataParser : ParserBase<Hero, HeroDataOverride>, IParser<Hero, HeroDataParser>
    {
        private readonly HeroOverrideLoader HeroOverrideLoader;

        private HeroDataOverride HeroDataOverride;
        private WeaponData WeaponData;
        private ArmorData ArmorData;
        private AbilityData AbilityData;
        private TalentData TalentData;

        public HeroDataParser(GameData gameData, DefaultData defaultData, HeroOverrideLoader heroOverrideLoader)
            : base(gameData, defaultData)
        {
            HeroOverrideLoader = heroOverrideLoader;
        }

        /// <summary>
        /// Gets or sets the base hero data.
        /// </summary>
        public Hero StormHeroBase { get; set; } = new Hero();

        /// <summary>
        /// Returns a collection of all the parsable ids. Allows multiple ids.
        /// </summary>
        /// <returns></returns>
        public HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());

                IEnumerable<XElement> cHeroElements = GameData.Elements("CHero").Where(x => x.Attribute("id") != null);

                foreach (XElement hero in cHeroElements)
                {
                    string id = hero.Attribute("id").Value;
                    XElement attributIdValue = hero.Elements("AttributeId").FirstOrDefault(x => x.Attribute("value") != null);

                    if (attributIdValue == null || id == "TestHero" || id == "Random")
                        continue;

                    items.Add(new string[] { id });
                }

                return items;
            }
        }

        /// <summary>
        /// Returns the parsed game data from the given ids. Multiple ids may be used to identity one item.
        /// </summary>
        /// <param name="id">The ids of the item to parse.</param>
        /// <returns></returns>
        public Hero Parse(params string[] ids)
        {
            string heroId = ids.FirstOrDefault();
            if (string.IsNullOrEmpty(heroId))
                return null;

            Hero hero = new Hero
            {
                Name = GameData.GetGameString(DefaultData.HeroData.HeroName.Replace(DefaultData.IdPlaceHolder, heroId)),
                Description = new TooltipDescription(GameData.GetGameString(DefaultData.HeroData.HeroDescription.Replace(DefaultData.IdPlaceHolder, heroId)), Localization),
                CHeroId = heroId,
                Id = heroId,
            };

            HeroDataOverride = HeroOverrideLoader.GetOverride(heroId) ?? new HeroDataOverride();

            WeaponData = new WeaponData(GameData, DefaultData, HeroDataOverride);
            ArmorData = new ArmorData(GameData);
            AbilityData = new AbilityData(GameData, DefaultData, HeroDataOverride, Localization);
            TalentData = new TalentData(GameData, DefaultData, HeroDataOverride, Localization);

            SetDefaultValues(hero);
            CActorData(hero);

            CHeroData(hero);
            CUnitData(hero);

            AddSubHeroCUnits(hero);

            FinalizeDataChecks(hero);

            ApplyOverrides(hero, HeroDataOverride);
            MoveParentLinkedAbilities(hero);
            MoveParentLinkedWeapons(hero);
            RemoveAbilityTalents(hero, HeroDataOverride);

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
                Id = StormHero.CHeroId,
                HyperlinkId = StormHero.CHeroId,
                CHeroId = StormHero.CHeroId,
                CUnitId = StormHero.CUnitId,
                Ratings = null,
            };

            HeroDataOverride = HeroOverrideLoader.GetOverride(StormHeroBase.CHeroId) ?? new HeroDataOverride();
            AbilityData = new AbilityData(GameData, DefaultData, HeroDataOverride, Localization);

            SetBaseHeroData(StormHeroBase);

            ApplyOverrides(StormHeroBase, HeroDataOverride);
            MoveParentLinkedAbilities(StormHeroBase);

            IList<Ability> hearthAbilties = StormHeroBase.PrimaryAbilities(AbilityTier.Hearth);
            IList<Ability> mountAbilties = StormHeroBase.PrimaryAbilities(AbilityTier.Mount);

            // based on the _stormhero data in hero-overrides.xml
            DefaultData.HeroData.DefaultHearthAbilityId = hearthAbilties[0].ReferenceNameId;
            DefaultData.HeroData.DefaultHearthNoManaAbilityId = hearthAbilties[1].ReferenceNameId;
            DefaultData.HeroData.DefaultSummonMountAbilityId = mountAbilties[0].ReferenceNameId;

            return StormHeroBase;
        }

        public HeroDataParser GetInstance()
        {
            return new HeroDataParser(GameData, DefaultData, HeroOverrideLoader);
        }

        protected override void ApplyAdditionalOverrides(Hero hero, HeroDataOverride heroDataOverride)
        {
            if (heroDataOverride.NameOverride.Enabled)
                hero.Name = heroDataOverride.NameOverride.Value;

            if (heroDataOverride.HyperlinkIdOverride.Enabled)
                hero.HyperlinkId = heroDataOverride.HyperlinkIdOverride.Value;

            if (heroDataOverride.EnergyTypeOverride.Enabled)
                hero.Energy.EnergyType = heroDataOverride.EnergyTypeOverride.EnergyType;

            if (heroDataOverride.EnergyOverride.Enabled)
                hero.Energy.EnergyMax = heroDataOverride.EnergyOverride.Energy;

            if (heroDataOverride.ParentLinkOverride.Enabled)
                hero.ParentLink = heroDataOverride.ParentLinkOverride.ParentLink;

            // abilities
            if (hero.Abilities != null)
            {
                foreach (KeyValuePair<string, Ability> ability in hero.Abilities)
                {
                    if (heroDataOverride.PropertyAbilityOverrideMethodByAbilityId.TryGetValue(ability.Key, out Dictionary<string, Action<Ability>> valueOverrideMethods))
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
                    if (heroDataOverride.PropertyTalentOverrideMethodByTalentId.TryGetValue(talents.Key, out Dictionary<string, Action<Talent>> valueOverrideMethods))
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
                    if (heroDataOverride.PropertyWeaponOverrideMethodByWeaponId.TryGetValue(weapon.WeaponNameId, out Dictionary<string, Action<UnitWeapon>> valueOverrideMethods))
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
                if (heroDataOverride.PropertyPortraitOverrideMethodByCHeroId.TryGetValue(hero.CHeroId, out Dictionary<string, Action<HeroPortrait>> valueOverrideMethods))
                {
                    foreach (var propertyOverride in valueOverrideMethods)
                    {
                        propertyOverride.Value(hero.HeroPortrait);
                    }
                }
            }
        }

        private void SetDefaultValues(Hero hero)
        {
            hero.Type = GameData.GetGameString(DefaultData.StringRanged).Trim();
            hero.Radius = DefaultData.HeroData.UnitRadius;
            hero.Speed = DefaultData.HeroData.UnitSpeed;
            hero.Sight = DefaultData.HeroData.UnitSight;
            hero.ReleaseDate = DefaultData.HeroData.HeroReleaseDate;
            hero.Gender = HeroGender.Male;
            hero.Franchise = HeroFranchise.Unknown;
            hero.Life.LifeMax = DefaultData.HeroData.UnitLifeMax;
            hero.Life.LifeRegenerationRate = 0;
            hero.Energy.EnergyType = GameData.GetGameString(DefaultData.HeroEnergyTypeManaText);
            hero.Energy.EnergyMax = DefaultData.HeroData.UnitEnergyMax;
            hero.Energy.EnergyRegenerationRate = DefaultData.HeroData.UnitEnergyRegenRate;
            hero.Difficulty = GameData.GetGameString(DefaultData.Difficulty.Replace(DefaultData.IdPlaceHolder, DefaultData.DefaultHeroDifficulty)).Trim();
            hero.HearthLinkId = DefaultData.HeroData.DefaultHearthAbilityId;

            if (hero.CHeroId != null)
            {
                hero.HyperlinkId = DefaultData.HeroData.HeroHyperlinkId.Replace(DefaultData.IdPlaceHolder, hero.CHeroId);
                hero.CUnitId = DefaultData.HeroData.HeroUnit.Replace(DefaultData.IdPlaceHolder, hero.CHeroId);

                hero.HeroPortrait.HeroSelectPortraitFileName = Path.GetFileName(PathHelpers.GetFilePath(DefaultData.HeroData.HeroSelectScreenButtonImage.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.LeaderboardPortraitFileName = Path.GetFileName(PathHelpers.GetFilePath(DefaultData.HeroData.HeroLeaderboardImage.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.LoadingScreenPortraitFileName = Path.GetFileName(PathHelpers.GetFilePath(DefaultData.HeroData.HeroLoadingScreenImage.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.PartyPanelPortraitFileName = Path.GetFileName(PathHelpers.GetFilePath(DefaultData.HeroData.HeroPartyPanelButtonImage.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.TargetPortraitFileName = Path.GetFileName(PathHelpers.GetFilePath(DefaultData.HeroData.HeroPortrait.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();

                hero.InfoText = GameData.GetGameString(DefaultData.HeroData.HeroInfoText.Replace(DefaultData.IdPlaceHolder, hero.CHeroId));
                hero.Title = GameData.GetGameString(DefaultData.HeroData.HeroTitle.Replace(DefaultData.IdPlaceHolder, hero.CHeroId));

                hero.SearchText = GameData.GetGameString(DefaultData.HeroData.HeroAlternateNameSearchText.Replace(DefaultData.IdPlaceHolder, hero.CHeroId));
                if (!string.IsNullOrEmpty(hero.SearchText) && hero.SearchText.Last() != ' ')
                    hero.SearchText += " ";
                hero.SearchText += GameData.GetGameString(DefaultData.HeroData.HeroAdditionalSearchText.Replace(DefaultData.IdPlaceHolder, hero.CHeroId));
                hero.SearchText = hero.SearchText.Trim();

                if (HeroDataOverride.CUnitOverride.Enabled)
                    hero.CUnitId = HeroDataOverride.CUnitOverride.CUnit;
            }
        }

        // used to acquire the hero's unique energy type
        private void CActorData(Hero hero)
        {
            XElement actorUnitElement = GameData.Elements("CActorUnit").FirstOrDefault(x => x.Attribute("id")?.Value == hero.CUnitId);

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
            XElement heroElement = GameData.Elements("CHero").FirstOrDefault(x => x.Attribute("id")?.Value == hero.CHeroId);

            if (heroElement == null)
                return;

            // loop through all elements and set found elements
            foreach (XElement element in heroElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "HYPERLINKID")
                {
                    hero.HyperlinkId = element.Attribute("value")?.Value;
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
                        hero.Difficulty = GameData.GetGameString(DefaultData.Difficulty.Replace(DefaultData.IdPlaceHolder, difficultyValue)).Trim();
                    }
                }
                else if (elementName == "ROLE" || elementName == "ROLESMULTICLASS")
                {
                    string roleValue = element.Attribute("value")?.Value;

                    if (hero.Roles == null)
                        hero.Roles = new List<string>();

                    string role = GameData.GetGameString(DefaultData.HeroData.HeroRoleName.Replace(DefaultData.IdPlaceHolder, roleValue)).Trim();

                    if (!hero.Roles.Contains(role))
                        hero.Roles.Add(role);
                }
                else if (elementName == "UNIVERSEICON")
                {
                    string iconImage = Path.GetFileName(PathHelpers.GetFilePath(element.Attribute("value").Value)).ToUpper();

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
                        day = DefaultData.HeroData.HeroReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.HeroData.HeroReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.HeroData.HeroReleaseDate.Year;

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
                    if (Enum.TryParse(element.Attribute("value").Value, out Rarity heroRarity))
                        hero.Rarity = heroRarity;
                    else
                        hero.Rarity = Rarity.Unknown;
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
                    hero.HeroPortrait.HeroSelectPortraitFileName = Path.GetFileName(PathHelpers.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "SCORESCREENIMAGE")
                {
                    hero.HeroPortrait.LeaderboardPortraitFileName = Path.GetFileName(PathHelpers.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "LOADINGSCREENIMAGE")
                {
                    hero.HeroPortrait.LoadingScreenPortraitFileName = Path.GetFileName(PathHelpers.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "PARTYPANELBUTTONIMAGE")
                {
                    hero.HeroPortrait.PartyPanelPortraitFileName = Path.GetFileName(PathHelpers.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "PORTRAIT")
                {
                    hero.HeroPortrait.TargetPortraitFileName = Path.GetFileName(PathHelpers.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "ADDITIONALSEARCHTEXT" || elementName == "ALTERNATENAMESEARCHTEXT")
                {
                    if (!string.IsNullOrEmpty(hero.SearchText) && hero.SearchText.Last() != ' ')
                        hero.SearchText += " ";

                    hero.SearchText += element.Attribute("value")?.Value.Trim();
                }
                else if (elementName == "EXPANDEDROLE")
                {
                    string role = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(role))
                        hero.ExpandedRole = GameData.GetGameString(DefaultData.HeroData.HeroRoleName.Replace(DefaultData.IdPlaceHolder, role)).Trim();
                }
            }

            if (hero.ReleaseDate == DefaultData.HeroData.HeroReleaseDate)
                hero.ReleaseDate = DefaultData.HeroData.HeroAlphaReleaseDate;

            // abilities must be gotten before talents
            foreach (XElement abilArrayElement in heroElement.Elements("HeroAbilArray"))
            {
                AbilityData.AddHeroAbility(hero, abilArrayElement);
            }

            AbilityData.AddOverrideButtonAbilities(hero);
            TalentData.SetButtonTooltipAppenderData(StormHeroBase, hero);

            foreach (XElement talentArrayElement in heroElement.Elements("TalentTreeArray"))
            {
                TalentData.AddTalent(hero, talentArrayElement);
            }
        }

        private void CUnitData(Hero hero, XElement unitElement = null)
        {
            unitElement = unitElement ?? GameData.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == hero.CUnitId);

            if (unitElement == null)
                return;

            // parent lookup
            string parentValue = unitElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue) && parentValue != DefaultDataHero.CUnitDefaultBaseId)
            {
                XElement parentElement = GameData.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == parentValue);
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
                        hero.MountLinkId = DefaultData.HeroData.DefaultSummonMountAbilityId;
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
                                if (cardLayoutFace.StartsWith(DefaultData.HeroData.DefaultHearthAbilityId) && cardLayoutAbilCmd.StartsWith(DefaultData.HeroData.DefaultHearthAbilityId))
                                    hero.HearthLinkId = cardLayoutFace;
                            }
                        }
                    }
                }
                else if (elementName == "HEROPLAYSTYLEFLAGS")
                {
                    string descriptor = element.Attribute("index").Value;

                    if (element.Attribute("value")?.Value == "1")
                        hero.HeroDescriptors.Add(descriptor);
                }
            }

            // set weapons
            WeaponData.AddHeroWeapons(hero, unitElement.Elements("WeaponArray").Where(x => x.Attribute("Link") != null));

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
            foreach (string cUnitId in HeroDataOverride.HeroUnits)
            {
                if (!GameData.TryGetGameString(DefaultData.HeroData.UnitName.Replace(DefaultData.IdPlaceHolder, cUnitId), out string name))
                {
                    if (!GameData.TryGetGameString(DefaultData.ButtonData.ButtonName.Replace(DefaultData.IdPlaceHolder, cUnitId), out name))
                        name = cUnitId;
                }

                Hero heroUnit = new Hero
                {
                    Id = cUnitId,
                    Name = name,
                    HyperlinkId = cUnitId.StartsWith("Hero") ? cUnitId.Remove(0, 4) : cUnitId,
                    CHeroId = null,
                    CUnitId = cUnitId,
                    HeroPortrait = null,
                };

                XElement cUnitElement = GameData.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == cUnitId);
                if (cUnitElement != null)
                {
                    SetDefaultValues(heroUnit);
                    CActorData(heroUnit);
                    CUnitData(heroUnit);

                    FinalizeDataChecks(heroUnit);
                }

                HeroDataOverride heroDataOverride = HeroOverrideLoader.GetOverride(cUnitId);
                if (heroDataOverride != null)
                    ApplyOverrides(heroUnit, heroDataOverride);

                hero.HeroUnits.Add(heroUnit);
            }
        }

        private void SetBaseHeroData(Hero hero)
        {
            AbilityData.AddOverrideButtonAbilities(hero);
        }

        private void FinalizeDataChecks(Hero hero)
        {
            if (hero.Life.LifeMax == 1 && hero.Life.LifeRegenerationRate == 0 && hero.Life.LifeRegenerationRateScaling == 0 && hero.Life.LifeScaling == 0)
                hero.Life.LifeMax = 0;
        }

        private void RemoveAbilityTalents(Hero hero, HeroDataOverride heroDataOverride)
        {
            foreach (KeyValuePair<string, bool> removedAbility in heroDataOverride.RemovedAbilityByAbilityReferenceNameId)
            {
                if (removedAbility.Value)
                    hero.Abilities.Remove(removedAbility.Key);
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
