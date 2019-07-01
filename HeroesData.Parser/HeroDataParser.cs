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

        //private readonly WeaponData WeaponData;
        //private readonly ArmorData ArmorData;
        //private readonly AbilityData AbilityData;
        private readonly TalentData TalentData;
        //private readonly BehaviorData BehaviorData;

        private HeroDataOverride HeroDataOverride;

        public HeroDataParser(IXmlDataService xmlDataService, HeroOverrideLoader heroOverrideLoader)
            : base(xmlDataService)
        {
            HeroOverrideLoader = heroOverrideLoader;

            //WeaponData = xmlDataService.WeaponData;
            //ArmorData = xmlDataService.ArmorData;
            //AbilityData = xmlDataService.AbilityData;
            TalentData = xmlDataService.TalentData;
            //BehaviorData = xmlDataService.BehaviorData;

            //WeaponData.IsHeroParsing = true;
            //AbilityData.IsAbilityTypeFilterEnabled = true;
            //AbilityData.IsAbilityTierFilterEnabled = true;
        }

        /// <summary>
        /// Gets or sets the base hero data.
        /// </summary>
        public Hero StormHeroBase { get; set; } = new Hero();

        /// <summary>
        /// Returns a collection of all the parsable ids. Allows multiple ids.
        /// </summary>
        /// <returns></returns>
        public override HashSet<string[]> Items
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

        protected override string ElementType => "CHero";

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

            UnitData unitData = XmlDataService.UnitData;
            unitData.IsAbilityTierFilterEnabled = true;
            unitData.IsAbilityTypeFilterEnabled = true;
            unitData.IsHeroParsing = true;
            unitData.Localization = Localization;

            Hero hero = new Hero
            {
                Name = GameData.GetGameString(DefaultData.HeroData.HeroName.Replace(DefaultData.IdPlaceHolder, heroId)),
                Description = new TooltipDescription(GameData.GetGameString(DefaultData.HeroData.HeroDescription.Replace(DefaultData.IdPlaceHolder, heroId)), Localization),
                CHeroId = heroId,
                Id = heroId,
            };

            XElement heroElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == heroId));
            if (heroElement == null)
                return null;

            HeroDataOverride = HeroOverrideLoader.GetOverride(heroId) ?? new HeroDataOverride();

            SetDefaultValues(hero);
            CActorData(hero);

            // must be done first to any find any units.
            FindUnits(heroElement, hero);

            // adds from overrides, parses for abilities
            AddHeroUnits(hero, unitData);

            // parses the hero's unit data; abilities
            // this must come after AddHeroUnits to correctly set the abilityTalentLinks for the talents
            unitData.SetUnitData(hero, true);

            // parses the hero's hero data; talents
            SetHeroData(heroElement, hero);

            ClearHeroUnitsFromUnitIds(hero);

            // execute all overrides
            ApplyOverrides(hero, HeroDataOverride);
            foreach (Hero heroUnit in hero.HeroUnits)
            {
                ApplyOverrides(heroUnit, HeroOverrideLoader.GetOverride(heroUnit.CHeroId) ?? new HeroDataOverride());
            }

            ValidateAbilityTalentLinkIds(hero);

            //MoveParentLinkedAbilities(hero);
            //MoveParentLinkedWeapons(hero);
            //RemoveAbilityTalents(hero, HeroDataOverride);

            return hero;
        }

        /// <summary>
        /// Parses the base hero data.
        /// </summary>
        /// <returns></returns>
        public Hero ParseBaseHero()
        {
            //StormHeroBase = new Hero
            //{
            //    Id = StormHero.CHeroId,
            //    HyperlinkId = StormHero.CHeroId,
            //    CHeroId = StormHero.CHeroId,
            //    CUnitId = StormHero.CUnitId,
            //    Ratings = null,
            //};

            //HeroDataOverride = HeroOverrideLoader.GetOverride(StormHeroBase.CHeroId) ?? new HeroDataOverride();
            //AbilityData.HeroDataOverride = HeroDataOverride;

            //SetBaseHeroData(StormHeroBase);

            //ApplyOverrides(StormHeroBase, HeroDataOverride);

            //IList<Ability> hearthAbilties = null; //StormHeroBase.PrimaryAbilities(AbilityTier.Hearth);
            //IList<Ability> mountAbilties = null; //StormHeroBase.PrimaryAbilities(AbilityTier.Mount);

            //// based on the _stormhero data in hero-overrides.xml
            //DefaultData.HeroData.DefaultHearthAbilityId = hearthAbilties[0].ReferenceNameId;
            //DefaultData.HeroData.DefaultHearthNoManaAbilityId = hearthAbilties[1].ReferenceNameId;
            //DefaultData.HeroData.DefaultSummonMountAbilityId = mountAbilties[0].ReferenceNameId;

            //return StormHeroBase;
            return null;
        }

        public HeroDataParser GetInstance()
        {
            return new HeroDataParser(XmlDataService, HeroOverrideLoader);
        }

        protected override void ApplyAdditionalOverrides(Hero hero, HeroDataOverride dataOverride)
        {
            if (dataOverride.EnergyTypeOverride.Enabled)
                hero.Energy.EnergyType = dataOverride.EnergyTypeOverride.EnergyType;

            if (dataOverride.EnergyOverride.Enabled)
                hero.Energy.EnergyMax = dataOverride.EnergyOverride.Energy;

            if (dataOverride.ParentLinkOverride.Enabled)
                hero.ParentLink = dataOverride.ParentLinkOverride.ParentLink;

            if (hero.Abilities != null)
            {
                foreach (AbilityTalentId addedAbility in dataOverride.AddedAbilities)
                {
                    Ability ability = XmlDataService.AbilityData.CreateAbility(hero.CUnitId, addedAbility);

                    if (ability != null)
                    {
                        if (dataOverride.IsAddedAbility(addedAbility))
                            hero.AddAbility(ability);
                        else
                            hero.RemoveAbility(ability);
                    }
                }

                dataOverride.ExecuteAbilityOverrides(hero.Abilities);
            }

            if (hero.HeroPortrait != null)
                dataOverride.ExecutePortraitOverrides(hero.CHeroId, hero.HeroPortrait);

            if (hero.Talents != null)
            {
                dataOverride.ExecuteTalentOverrides(hero.Talents);
            }

            if (hero.Weapons != null)
            {
                foreach (string addedWeapon in dataOverride.AddedWeapons)
                {
                    UnitWeapon weapon = XmlDataService.WeaponData.CreateWeapon(addedWeapon);

                    if (weapon != null)
                    {
                        if (dataOverride.IsAddedWeapon(addedWeapon))
                            hero.AddUnitWeapon(weapon);
                        else
                            hero.RemoveUnitWeapon(weapon);
                    }
                }

                dataOverride.ExecuteWeaponOverrides(hero.Weapons);
            }
        }

        private void SetDefaultValues(Hero hero)
        {
            hero.Type = GameData.GetGameString(DefaultData.StringRanged).Trim();
            hero.Radius = DefaultData.HeroData.UnitRadius;
            hero.Speed = DefaultData.HeroData.UnitSpeed;
            hero.Sight = DefaultData.HeroData.UnitSight;
            hero.ReleaseDate = DefaultData.HeroData.HeroReleaseDate;
            hero.Gender = UnitGender.Male;
            hero.Franchise = HeroFranchise.Unknown;
            hero.Life.LifeMax = DefaultData.HeroData.UnitLifeMax;
            hero.Life.LifeRegenerationRate = 0;
            hero.Energy.EnergyType = GameData.GetGameString(DefaultData.HeroEnergyTypeManaText);
            hero.Energy.EnergyMax = DefaultData.HeroData.UnitEnergyMax;
            hero.Energy.EnergyRegenerationRate = DefaultData.HeroData.UnitEnergyRegenRate;
            hero.Difficulty = GameData.GetGameString(DefaultData.Difficulty.Replace(DefaultData.IdPlaceHolder, DefaultData.DefaultHeroDifficulty)).Trim();

            if (hero.CHeroId != null)
            {
                hero.HyperlinkId = DefaultData.HeroData.HeroHyperlinkId.Replace(DefaultData.IdPlaceHolder, hero.CHeroId);
                hero.CUnitId = DefaultData.HeroData.HeroUnit.Replace(DefaultData.IdPlaceHolder, hero.CHeroId);

                hero.HeroPortrait.HeroSelectPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroSelectScreenButtonImage.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.LeaderboardPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroLeaderboardImage.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.LoadingScreenPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroLoadingScreenImage.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.PartyPanelPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroPartyPanelButtonImage.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.TargetPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroPortrait.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();

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
            IEnumerable<XElement> actorUnitElements = GameData.Elements("CActorUnit").Where(x => x.Attribute("id")?.Value == hero.CUnitId);

            if (actorUnitElements == null || !actorUnitElements.Any())
                return;

            // find special energy type
            foreach (XElement element in actorUnitElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "VITALNAMES")
                {
                    string indexValue = element.Attribute("index")?.Value;
                    string valueValue = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(indexValue) && !string.IsNullOrEmpty(valueValue) && indexValue == "Energy")
                    {
                        if (GameData.TryGetGameString(valueValue, out string energyType))
                        {
                            hero.Energy.EnergyType = energyType;
                        }
                    }
                }
                else if (elementName == "UNITBUTTON" || elementName == "UNITBUTTONMULTIPLE")
                {
                    string value = element.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        HeroDataOverride.AddAddedAbility(new AbilityTalentId(value, value), true);
                    }
                }
            }
        }

        private void SetHeroData(XElement heroElement, Hero hero)
        {
            if (heroElement == null)
                return;

            TalentData.SetButtonTooltipAppenderData(hero);

            // parent lookup
            string parentValue = heroElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetHeroData(parentElement, hero);
            }

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
                    if (!string.IsNullOrEmpty(roleValue))
                    {
                        string role = GameData.GetGameString(DefaultData.HeroData.HeroRoleName.Replace(DefaultData.IdPlaceHolder, roleValue)).Trim();
                        if (!string.IsNullOrEmpty(roleValue))
                        {
                            hero.AddRole(role);
                        }
                    }
                }
                else if (elementName == "UNIVERSEICON")
                {
                    string iconImage = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value").Value)).ToUpper();

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
                    if (Enum.TryParse(element.Attribute("value").Value, out UnitGender unitGender))
                        hero.Gender = unitGender;
                    else
                        hero.Gender = UnitGender.Neutral;
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
                    hero.HeroPortrait.HeroSelectPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "SCORESCREENIMAGE")
                {
                    hero.HeroPortrait.LeaderboardPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "LOADINGSCREENIMAGE")
                {
                    hero.HeroPortrait.LoadingScreenPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "PARTYPANELBUTTONIMAGE")
                {
                    hero.HeroPortrait.PartyPanelPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
                else if (elementName == "PORTRAIT")
                {
                    hero.HeroPortrait.TargetPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value)).ToLower();
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
                else if (elementName == "TALENTTREEARRAY")
                {
                    Talent talent = TalentData.CreateTalent(hero, element);
                    if (talent != null)
                    {
                        hero.AddTalent(talent);

                        // makes the abilities that are granted from talents subabilities to that talent
                        if ((talent.AbilityType != AbilityType.Heroic || talent.Tier == TalentTier.Level20) && hero.TryGetAbility(talent.AbilityTalentId.ReferenceId, out Ability ability))
                        {
                            ability.ParentLink = talent.AbilityTalentId;
                        }
                    }
                }
            }

            if (hero.ReleaseDate == DefaultData.HeroData.HeroReleaseDate)
                hero.ReleaseDate = DefaultData.HeroData.HeroAlphaReleaseDate;
        }

        private void FindUnits(XElement heroElement, Hero hero)
        {
            if (heroElement == null)
                return;

            // parent lookup
            string parentValue = heroElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetHeroData(parentElement, hero);
            }

            string unit = heroElement.Element("Unit")?.Attribute("value")?.Value;

            if (!string.IsNullOrEmpty(unit) && !HeroDataOverride.ContainsRemovedHeroUnit(unit))
                HeroDataOverride.AddHeroUnit(unit);
        }

        private void ValidateAbilityTalentLinkIds (Hero hero)
        {
            foreach (Talent talent in hero.Talents)
            {
                // validate all abilityTalentLinkIds
                HashSet<string> validatedIds = new HashSet<string>();
                foreach (string abilityTalentLinkId in talent.AbilityTalentLinkIds)
                {
                    if (hero.ContainsAbility(abilityTalentLinkId) || hero.ContainsTalent(abilityTalentLinkId) || hero.HeroUnits.Select(x => x.ContainsAbility(abilityTalentLinkId)).Any())
                        validatedIds.Add(abilityTalentLinkId);
                }

                talent.ClearAbilityTalentLinkIds();

                foreach (string validatedId in validatedIds)
                {
                    talent.AddAbilityTalentLinkId(validatedId);
                }
            }
        }

        private void AddHeroUnits(Hero hero, UnitData unitData)
        {
            foreach (string heroUnit in HeroDataOverride.HeroUnits)
            {
                if (string.IsNullOrEmpty(heroUnit))
                    continue;

                //if (!GameData.TryGetGameString(DefaultData.HeroData.UnitName.Replace(DefaultData.IdPlaceHolder, heroUnit), out string name))
                //{
                //    if (!GameData.TryGetGameString(DefaultData.ButtonData.ButtonName.Replace(DefaultData.IdPlaceHolder, heroUnit), out name))
                //        name = heroUnit;
                //}

                Hero newHeroUnit = new Hero
                {
                    Id = heroUnit,
                    CUnitId = heroUnit,
                    CHeroId = heroUnit,
                };

                unitData.SetUnitData(newHeroUnit);

                // set the hyperlinkId to id if it doesn't have one
                if (string.IsNullOrEmpty(newHeroUnit.HyperlinkId))
                    newHeroUnit.HyperlinkId = newHeroUnit.Id;

                hero.AddHeroUnit(newHeroUnit);

                //XElement unitElement = GameData.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == heroUnit);
                //if (unitElement == null)
                //    continue;

                //SetDefaultValues(heroUnit);
                //CActorData(heroUnit);
                //CUnitData(heroUnit);

                //FinalizeDataChecks(heroUnit);


                //HeroDataOverride heroDataOverride = HeroOverrideLoader.GetOverride(heroUnit);
                //if (heroDataOverride != null)
                //    ApplyOverrides(heroUnit, heroDataOverride);

                //unit.HeroUnits.Add(heroUnit);

                //Hero subHeroUnit = new Hero
                //{
                //    Id = heroUnit,
                //    Name = name,
                //    HyperlinkId = heroUnit.StartsWith("Hero") ? heroUnit.Remove(0, 4) : heroUnit,
                //    CHeroId = null,
                //    CUnitId = heroUnit,
                //    HeroPortrait = null,
                //};

                //XElement cUnitElement = GameData.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == heroUnit);
                //if (cUnitElement != null)
                //{
                //    SetDefaultValues(heroUnit);
                //    CActorData(heroUnit);
                //    CUnitData(heroUnit);

                //    FinalizeDataChecks(heroUnit);
                //}

                //HeroDataOverride heroDataOverride = HeroOverrideLoader.GetOverride(heroUnit);
                //if (heroDataOverride != null)
                //    ApplyOverrides(heroUnit, heroDataOverride);

                //subHeroUnit.HeroUnits.Add(heroUnit);
            }
        }

        private void ClearHeroUnitsFromUnitIds(Hero hero)
        {
            foreach (Hero heroUnit in hero.HeroUnits)
            {
                hero.RemoveUnitId(heroUnit.CUnitId);
            }
        }

        private void SetBaseHeroData(Hero hero)
        {
            foreach (AddedButtonAbility abilityButton in HeroDataOverride.AddedAbilityByButtonId)
            {

            }
            //AbilityData.CreateOverrideButtonAbility(hero);
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
                //if (removedAbility.Value)
                //    hero.Abilities.Remove(removedAbility.Key);
            }
        }
    }
}
