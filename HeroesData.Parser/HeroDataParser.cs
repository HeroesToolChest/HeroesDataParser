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

        private readonly WeaponData WeaponData;
        private readonly ArmorData ArmorData;
        private readonly AbilityData AbilityData;
        private readonly TalentData TalentData;
        private readonly BehaviorData BehaviorData;

        private HeroDataOverride HeroDataOverride;

        public HeroDataParser(IXmlDataService xmlDataService, HeroOverrideLoader heroOverrideLoader)
            : base(xmlDataService)
        {
            HeroOverrideLoader = heroOverrideLoader;
            WeaponData = xmlDataService.WeaponData;
            ArmorData = xmlDataService.ArmorData;
            AbilityData = xmlDataService.AbilityData;
            TalentData = xmlDataService.TalentData;
            BehaviorData = xmlDataService.BehaviorData;
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

            AbilityData.Localization = Localization;
            AbilityData.HeroDataOverride = HeroDataOverride;

            SetDefaultValues(hero);
            CActorData(hero);

            SetUnitData(hero);
            SetHeroData(heroElement, hero);


            //AddSubHeroCUnits(hero);

            //FinalizeDataChecks(hero);

            //ApplyOverrides(hero, HeroDataOverride);
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
            StormHeroBase = new Hero
            {
                Id = StormHero.CHeroId,
                HyperlinkId = StormHero.CHeroId,
                CHeroId = StormHero.CHeroId,
                CUnitId = StormHero.CUnitId,
                Ratings = null,
            };

            HeroDataOverride = HeroOverrideLoader.GetOverride(StormHeroBase.CHeroId) ?? new HeroDataOverride();
            AbilityData.HeroDataOverride = HeroDataOverride;

            SetBaseHeroData(StormHeroBase);

            ApplyOverrides(StormHeroBase, HeroDataOverride);

            IList<Ability> hearthAbilties = null; //StormHeroBase.PrimaryAbilities(AbilityTier.Hearth);
            IList<Ability> mountAbilties = null; //StormHeroBase.PrimaryAbilities(AbilityTier.Mount);

            // based on the _stormhero data in hero-overrides.xml
            DefaultData.HeroData.DefaultHearthAbilityId = hearthAbilties[0].ReferenceNameId;
            DefaultData.HeroData.DefaultHearthNoManaAbilityId = hearthAbilties[1].ReferenceNameId;
            DefaultData.HeroData.DefaultSummonMountAbilityId = mountAbilties[0].ReferenceNameId;

            return StormHeroBase;
        }

        public HeroDataParser GetInstance()
        {
            return new HeroDataParser(XmlDataService, HeroOverrideLoader);
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
                // TODO: possibly re-add
                //foreach (KeyValuePair<string, Ability> ability in hero.Abilities)
                //{
                //    if (heroDataOverride.PropertyAbilityOverrideMethodByAbilityId.TryGetValue(ability.Key, out Dictionary<string, Action<Ability>> valueOverrideMethods))
                //    {
                //        foreach (var propertyOverride in valueOverrideMethods)
                //        {
                //            // execute each property override
                //            propertyOverride.Value(ability.Value);
                //        }
                //    }
                //}
            }

            // talents
            //if (hero.Talents != null)
            //{
            //    foreach (KeyValuePair<string, Talent> talents in hero.Talents)
            //    {
            //        if (heroDataOverride.PropertyTalentOverrideMethodByTalentId.TryGetValue(talents.Key, out Dictionary<string, Action<Talent>> valueOverrideMethods))
            //        {
            //            foreach (var propertyOverride in valueOverrideMethods)
            //            {
            //                // execute each property override
            //                propertyOverride.Value(talents.Value);
            //            }
            //        }
            //    }
            //}

            //// weapons
            //if (hero.Weapons != null)
            //{
            //    foreach (UnitWeapon weapon in hero.Weapons)
            //    {
            //        if (heroDataOverride.PropertyWeaponOverrideMethodByWeaponId.TryGetValue(weapon.WeaponNameId, out Dictionary<string, Action<UnitWeapon>> valueOverrideMethods))
            //        {
            //            foreach (var propertyOverride in valueOverrideMethods)
            //            {
            //                // execute each property override
            //                propertyOverride.Value(weapon);
            //            }
            //        }
            //    }
            //}

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
                        hero.AddTalent(talent);
                }
            }

            if (hero.ReleaseDate == DefaultData.HeroData.HeroReleaseDate)
                hero.ReleaseDate = DefaultData.HeroData.HeroAlphaReleaseDate;

            //// abilities must be gotten before talents
            //foreach (XElement abilArrayElement in heroElement.Elements("HeroAbilArray"))
            //{
            //    AbilityData.AddHeroAbility(hero, abilArrayElement);
            //}

            ////AbilityData.CreateOverrideButtonAbility(hero);
            //TalentData.SetButtonTooltipAppenderData(StormHeroBase, hero);

            //foreach (XElement talentArrayElement in heroElement.Elements("TalentTreeArray"))
            //{
            //    TalentData.AddTalent(hero, talentArrayElement);
            //}
        }

        private void SetUnitData(Hero hero, XElement unitElement = null)
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
                    SetUnitData(hero, parentElement);
            }

            // loop through all elements and set found elements
            foreach (XElement element in unitElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                //if (elementName == "ABILARRAY")
                //{
                //    string link = element.Attribute("Link")?.Value;

                //    if (link == DefaultData.AbilMountLinkId)
                //        hero.MountLinkId = DefaultData.HeroData.DefaultSummonMountAbilityId;
                //}
                if (elementName == "LIFEMAX")
                {
                    hero.Life.LifeMax = XmlParse.GetDoubleValue(hero.CUnitId, element, GameData);

                    double? scaleValue = GameData.GetScaleValue(("Unit", hero.CUnitId, "LifeMax"));
                    if (scaleValue.HasValue)
                        hero.Life.LifeScaling = scaleValue.Value;
                }
                else if (elementName == "LIFEREGENRATE")
                {
                    hero.Life.LifeRegenerationRate = XmlParse.GetDoubleValue(hero.CUnitId, element, GameData);

                    double? scaleValue = GameData.GetScaleValue(("Unit", hero.CUnitId, "LifeRegenRate"));
                    if (scaleValue.HasValue)
                        hero.Life.LifeRegenerationRateScaling = scaleValue.Value;
                }
                else if (elementName == "RADIUS")
                {
                    hero.Radius = XmlParse.GetDoubleValue(hero.CUnitId, element, GameData);
                }
                else if (elementName == "INNERRADIUS")
                {
                    hero.InnerRadius = XmlParse.GetDoubleValue(hero.CUnitId, element, GameData);
                }
                else if (elementName == "ENERGYMAX")
                {
                    hero.Energy.EnergyMax = XmlParse.GetDoubleValue(hero.CUnitId, element, GameData);
                }
                else if (elementName == "ENERGYREGENRATE")
                {
                    hero.Energy.EnergyRegenerationRate = XmlParse.GetDoubleValue(hero.CUnitId, element, GameData);
                }
                else if (elementName == "SPEED")
                {
                    hero.Speed = XmlParse.GetDoubleValue(hero.CUnitId, element, GameData);
                }
                else if (elementName == "SIGHT")
                {
                    hero.Sight = XmlParse.GetDoubleValue(hero.CUnitId, element, GameData);
                }
                else if (elementName == "ATTRIBUTES")
                {
                    string enabled = element.Attribute("value")?.Value;
                    string attribute = element.Attribute("index").Value;

                    if (enabled == "0" && hero.Attributes.Contains(attribute))
                        hero.RemoveAttribute(attribute);
                    else if (enabled == "1")
                        hero.AddAttribute(attribute);
                }
                else if (elementName == "UNITDAMAGETYPE")
                {
                    hero.DamageType = element.Attribute("value").Value;
                }
                else if (elementName == "NAME")
                {
                    hero.Name = GameData.GetGameString(element.Attribute("value").Value);
                }
                else if (elementName == "DESCRIPTION")
                {
                    hero.Description = new TooltipDescription(GameData.GetGameString(element.Attribute("value").Value));
                }
                else if (elementName == "GENDER")
                {
                    if (Enum.TryParse(element.Attribute("value").Value, out HeroGender heroGender))
                        hero.Gender = heroGender;
                    else
                        hero.Gender = HeroGender.Neutral;
                }
                else if (elementName == "ABILARRAY")
                {
                    Ability ability = AbilityData.CreateAbility(hero.CUnitId, element);
                    if (ability != null)
                    {
                        hero.AddAbility(ability);

                        foreach (string unit in ability.CreatedUnits)
                        {
                            hero.AddUnit(unit);
                        }
                    }
                }
                else if (elementName == "WEAPONARRAY")
                {
                    UnitWeapon weapon = WeaponData.CreateWeapon(element);
                    if (weapon != null)
                        hero.AddUnitWeapon(weapon);
                }
                else if (elementName == "ARMORLINK")
                {
                    IEnumerable<UnitArmor> armorList = ArmorData.CreateArmorCollection(element);
                    if (armorList != null)
                    {
                        foreach (UnitArmor armor in armorList)
                        {
                            hero.AddUnitArmor(armor);
                        }
                    }
                }
                else if (elementName == "BEHAVIORARRAY")
                {
                    string link = BehaviorData.GetScalingBehaviorLink(element);
                    if (!string.IsNullOrEmpty(link))
                        hero.ScalingBehaviorLink = link;
                }
                else if (elementName == "HEROPLAYSTYLEFLAGS")
                {
                    string descriptor = element.Attribute("index").Value;

                    if (element.Attribute("value")?.Value == "1")
                        hero.AddHeroDescriptor(descriptor);
                }
                //else if (elementName == "CARDLAYOUTS")
                //{
                //    foreach (XElement cardLayoutElement in element.Elements())
                //    {
                //        string cardLayoutElementName = cardLayoutElement.Name.LocalName.ToUpper();

                //        if (cardLayoutElementName == "LAYOUTBUTTONS")
                //        {
                //            string cardLayoutFace = cardLayoutElement.Attribute("Face")?.Value;
                //            string cardLayoutAbilCmd = cardLayoutElement.Attribute("AbilCmd")?.Value;

                //            if (!string.IsNullOrEmpty(cardLayoutFace) && !string.IsNullOrEmpty(cardLayoutAbilCmd))
                //            {
                //                if (cardLayoutFace.StartsWith(DefaultData.HeroData.DefaultHearthAbilityId) && cardLayoutAbilCmd.StartsWith(DefaultData.HeroData.DefaultHearthAbilityId))
                //                    hero.HearthLinkId = cardLayoutFace;
                //            }
                //        }
                //    }
                //}
            }

            if (hero.Energy.EnergyMax < 1)
                hero.Energy.EnergyType = string.Empty;

            // set mount link if hero has a custom mount ability
            //IList<Ability> mountAbilities = hero.PrimaryAbilities(AbilityTier.Mount);
            //string parentAttribute = unitElement.Attribute("parent")?.Value;

            //if (parentAttribute == "StormHeroMountedCustom" && mountAbilities.Count > 0)
            //    hero.MountLinkId = mountAbilities.FirstOrDefault()?.ReferenceNameId;
        }

        private void AddSubHeroCUnits(Hero hero)
        {
            //foreach (string cUnitId in HeroDataOverride.HeroUnits)
            //{
            //    if (!GameData.TryGetGameString(DefaultData.HeroData.UnitName.Replace(DefaultData.IdPlaceHolder, cUnitId), out string name))
            //    {
            //        if (!GameData.TryGetGameString(DefaultData.ButtonData.ButtonName.Replace(DefaultData.IdPlaceHolder, cUnitId), out name))
            //            name = cUnitId;
            //    }

            //    Hero heroUnit = new Hero
            //    {
            //        Id = cUnitId,
            //        Name = name,
            //        HyperlinkId = cUnitId.StartsWith("Hero") ? cUnitId.Remove(0, 4) : cUnitId,
            //        CHeroId = null,
            //        CUnitId = cUnitId,
            //        HeroPortrait = null,
            //    };

            //    XElement cUnitElement = GameData.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == cUnitId);
            //    if (cUnitElement != null)
            //    {
            //        SetDefaultValues(heroUnit);
            //        CActorData(heroUnit);
            //        CUnitData(heroUnit);

            //        FinalizeDataChecks(heroUnit);
            //    }

            //    HeroDataOverride heroDataOverride = HeroOverrideLoader.GetOverride(cUnitId);
            //    if (heroDataOverride != null)
            //        ApplyOverrides(heroUnit, heroDataOverride);

            //    hero.HeroUnits.Add(heroUnit);
            //}
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
