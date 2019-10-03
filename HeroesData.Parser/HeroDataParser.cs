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
    public class HeroDataParser : ParserBase<Hero, HeroDataOverride>, IParser<Hero?, HeroDataParser>
    {
        private readonly HeroOverrideLoader HeroOverrideLoader;
        private readonly TalentData TalentData;

        private XmlArrayElement? TalentsArray;

        private HeroDataOverride? HeroDataOverride;

        public HeroDataParser(IXmlDataService xmlDataService, HeroOverrideLoader heroOverrideLoader)
            : base(xmlDataService)
        {
            HeroOverrideLoader = heroOverrideLoader;
            TalentData = xmlDataService.TalentData;
        }

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
        public Hero? Parse(params string[] ids)
        {
            string heroId = ids.FirstOrDefault();
            if (string.IsNullOrEmpty(heroId))
                return null;

            TalentsArray = new XmlArrayElement();

            UnitData unitData = XmlDataService.UnitData;
            unitData.IsAbilityTierFilterEnabled = true;
            unitData.IsAbilityTypeFilterEnabled = true;
            unitData.IsHeroParsing = true;
            unitData.Localization = Localization;

            Hero hero = new Hero
            {
                Name = GameData.GetGameString(DefaultData.HeroData?.HeroName?.Replace(DefaultData.IdPlaceHolder, heroId)),
                Description = new TooltipDescription(GameData.GetGameString(DefaultData.HeroData?.HeroDescription?.Replace(DefaultData.IdPlaceHolder, heroId)), Localization),
                CHeroId = heroId,
                Id = heroId,
            };

            HeroDataOverride = HeroOverrideLoader.GetOverride(heroId) ?? new HeroDataOverride();

            XElement? heroElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == heroId));
            if (heroElement == null)
                return null;

            SetDefaultValues(hero);

            // must be done first to any find any units.
            FindUnits(heroElement, hero);

            // adds from overrides, parses for abilities
            AddHeroUnits(hero, unitData);

            // parses the hero's unit data; abilities
            // this must come after AddHeroUnits to correctly set the abilityTalentLinks for the talents
            unitData.SetUnitData(hero, HeroDataOverride, true);

            // parses the hero's hero data; talents
            SetData(heroElement, hero);
            SetTalents(hero);

            ClearHeroUnitsFromUnitIds(hero);

            // execute all overrides
            ApplyOverrides(hero, HeroDataOverride);
            foreach (Hero heroUnit in hero.HeroUnits)
                ApplyOverrides(heroUnit, HeroOverrideLoader.GetOverride(heroUnit.CHeroId) ?? new HeroDataOverride());

            // validation
            ValidateAbilityTalentLinkIds(hero);
            ValidateSubAbilities(hero);
            foreach (Hero heroUnit in hero.HeroUnits)
                ValidateSubAbilities(heroUnit);

            return hero;
        }

        public HeroDataParser GetInstance()
        {
            return new HeroDataParser(XmlDataService, HeroOverrideLoader);
        }

        protected override void ApplyAdditionalOverrides(Hero hero, HeroDataOverride dataOverride)
        {
            if (dataOverride.EnergyTypeOverride.enabled)
                hero.Energy.EnergyType = dataOverride.EnergyTypeOverride.energyType;

            if (dataOverride.EnergyOverride.enabled)
                hero.Energy.EnergyMax = dataOverride.EnergyOverride.energy;

            if (dataOverride.ParentLinkOverride.enabled)
                hero.ParentLink = dataOverride.ParentLinkOverride.parentLink;

            if (hero.Abilities != null)
            {
                foreach (AbilityTalentId addedAbility in dataOverride.AddedAbilities)
                {
                    Ability? ability = XmlDataService.AbilityData.CreateAbility(hero.CUnitId, addedAbility);

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
                    UnitWeapon? weapon = XmlDataService.WeaponData.CreateWeapon(addedWeapon);

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
            hero.Radius = DefaultData.HeroData!.UnitRadius;
            hero.Speed = DefaultData.HeroData.UnitSpeed;
            hero.Sight = DefaultData.HeroData.UnitSight;
            hero.ReleaseDate = DefaultData.HeroData.HeroReleaseDate;
            hero.Gender = UnitGender.Male;
            hero.Franchise = HeroFranchise.Unknown;
            hero.Life.LifeMax = DefaultData.HeroData.UnitLifeMax;
            hero.Life.LifeRegenerationRate = 0;
            hero.Energy.EnergyMax = DefaultData.HeroData.UnitEnergyMax;
            hero.Energy.EnergyRegenerationRate = DefaultData.HeroData.UnitEnergyRegenRate;
            hero.Shield.ShieldMax = DefaultData.HeroData.UnitShieldMax;
            hero.Shield.ShieldRegenerationRate = DefaultData.HeroData.UnitShieldRegenRate;
            hero.Shield.ShieldRegenerationDelay = DefaultData.HeroData.UnitShieldRegenDelay;
            hero.Difficulty = GameData.GetGameString(DefaultData.Difficulty.Replace(DefaultData.IdPlaceHolder, DefaultData.DefaultHeroDifficulty)).Trim();

            if (hero.CHeroId != null)
            {
                hero.HyperlinkId = DefaultData.HeroData.HeroHyperlinkId!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId);
                hero.CUnitId = DefaultData.HeroData.HeroUnit!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId);

                hero.HeroPortrait.HeroSelectPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroSelectScreenButtonImage!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.LeaderboardPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroLeaderboardImage!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.LoadingScreenPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroLoadingScreenImage!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.PartyPanelPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroPartyPanelButtonImage!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.TargetPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroPortrait!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();
                hero.HeroPortrait.PartyFrameFileName.Add(Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroPartyFrameImage!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower());
                hero.HeroPortrait.DraftScreenFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroDraftScreenImage!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId))).ToLower();

                hero.InfoText = GameData.GetGameString(DefaultData.HeroData.HeroInfoText!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId));
                hero.Title = GameData.GetGameString(DefaultData.HeroData.HeroTitle!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId));

                hero.SearchText = GameData.GetGameString(DefaultData.HeroData.HeroAlternateNameSearchText?.Replace(DefaultData.IdPlaceHolder, hero.CHeroId));
                if (!string.IsNullOrEmpty(hero.SearchText) && hero.SearchText.Last() != ' ')
                    hero.SearchText += " ";
                hero.SearchText += GameData.GetGameString(DefaultData.HeroData.HeroAdditionalSearchText?.Replace(DefaultData.IdPlaceHolder, hero.CHeroId));
                hero.SearchText = hero.SearchText.Trim();

                if (HeroDataOverride != null && HeroDataOverride.CUnitOverride.enabled)
                    hero.CUnitId = HeroDataOverride.CUnitOverride.cUnit;
            }
        }

        private void SetData(XElement heroElement, Hero hero)
        {
            if (heroElement == null || hero == null)
                return;

            TalentData.SetButtonTooltipAppenderData(hero);

            // parent lookup
            string? parentValue = heroElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetData(parentElement, hero);
            }

            // loop through all elements and set found elements
            foreach (XElement element in heroElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "HYPERLINKID")
                {
                    hero.HyperlinkId = element.Attribute("value")?.Value ?? string.Empty;
                }
                else if (elementName == "ATTRIBUTEID")
                {
                    hero.AttributeId = element.Attribute("value")?.Value ?? string.Empty;
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
                    string difficultyValue = element.Attribute("value")?.Value ?? string.Empty;

                    if (!string.IsNullOrEmpty(difficultyValue))
                    {
                        hero.Difficulty = GameData.GetGameString(DefaultData.Difficulty.Replace(DefaultData.IdPlaceHolder, difficultyValue)).Trim();
                    }
                }
                else if (elementName == "ROLE" || elementName == "ROLESMULTICLASS")
                {
                    string roleValue = element.Attribute("value")?.Value ?? string.Empty;
                    if (!string.IsNullOrEmpty(roleValue))
                    {
                        string role = GameData.GetGameString(DefaultData.HeroData?.HeroRoleName?.Replace(DefaultData.IdPlaceHolder, roleValue)).Trim();
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
                        day = DefaultData.HeroData!.HeroReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.HeroData!.HeroReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.HeroData!.HeroReleaseDate.Year;

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
                    string? damage = null;
                    string? utility = null;
                    string? survivability = null;
                    string? complexity = null;

                    if (element.HasElements)
                    {
                        damage = element.Element("Damage")?.Attribute("value")?.Value;
                        utility = element.Element("Utility")?.Attribute("value")?.Value;
                        survivability = element.Element("Survivability")?.Attribute("value")?.Value;
                        complexity = element.Element("Complexity")?.Attribute("value")?.Value;
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
                    hero.HeroPortrait.HeroSelectPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value ?? string.Empty)).ToLower();
                }
                else if (elementName == "SCORESCREENIMAGE")
                {
                    hero.HeroPortrait.LeaderboardPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value ?? string.Empty)).ToLower();
                }
                else if (elementName == "LOADINGSCREENIMAGE")
                {
                    hero.HeroPortrait.LoadingScreenPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value ?? string.Empty)).ToLower();
                }
                else if (elementName == "PARTYPANELBUTTONIMAGE")
                {
                    hero.HeroPortrait.PartyPanelPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value ?? string.Empty)).ToLower();
                }
                else if (elementName == "PORTRAIT")
                {
                    hero.HeroPortrait.TargetPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value ?? string.Empty)).ToLower();
                }
                else if (elementName == "PARTYFRAMEIMAGE")
                {
                    hero.HeroPortrait.PartyFrameFileName.Add(Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value ?? string.Empty)).ToLower());
                }
                else if (elementName == "DRAFTSCREENPORTRAIT")
                {
                    hero.HeroPortrait.DraftScreenFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value ?? string.Empty)).ToLower();
                }
                else if (elementName == "ADDITIONALSEARCHTEXT" || elementName == "ALTERNATENAMESEARCHTEXT")
                {
                    if (!string.IsNullOrEmpty(hero.SearchText) && hero.SearchText.Last() != ' ')
                        hero.SearchText += " ";

                    hero.SearchText += element.Attribute("value")?.Value.Trim();
                }
                else if (elementName == "EXPANDEDROLE")
                {
                    string? role = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(role))
                        hero.ExpandedRole = GameData.GetGameString(DefaultData.HeroData?.HeroRoleName?.Replace(DefaultData.IdPlaceHolder, role)).Trim();
                }
                else if (elementName == "TALENTTREEARRAY")
                {
                    TalentsArray?.AddElement(element);
                }
            }

            if (hero.ReleaseDate == DefaultData.HeroData?.HeroReleaseDate)
                hero.ReleaseDate = DefaultData.HeroData?.HeroAlphaReleaseDate;
        }

        private void SetTalents(Hero hero)
        {
            if (TalentsArray == null)
                throw new ArgumentNullException("Call SetData() first to set up the talents");

            foreach (XElement element in TalentsArray.Elements)
            {
                Talent? talent = TalentData.CreateTalent(hero, element);
                if (talent != null)
                {
                    XmlArrayElement prerequisiteTalentArray = GetTalentPrerequisites(element);

                    foreach (XElement prerequisiteTalentElement in prerequisiteTalentArray.Elements)
                    {
                        string? talentPrerequisite = prerequisiteTalentElement.Attribute("value")?.Value;

                        if (!string.IsNullOrEmpty(talentPrerequisite))
                            talent.AddPrerequisiteTalentId(talentPrerequisite);
                    }

                    hero.AddTalent(talent);

                    // makes the abilities that are granted from talents subabilities to that talent
                    if (talent.AbilityTalentId.AbilityType != AbilityType.Heroic || talent.Tier == TalentTier.Level20)
                    {
                        IEnumerable<Ability> abilities = hero.GetAbilities(talent.AbilityTalentId.ReferenceId, StringComparison.OrdinalIgnoreCase);

                        foreach (Ability ability in abilities)
                        {
                            ability.ParentLink = talent.AbilityTalentId;
                        }
                    }
                }
            }
        }

        private XmlArrayElement GetTalentPrerequisites(XElement talentElement)
        {
            XmlArrayElement prerequisiteTalentArray = new XmlArrayElement();

            foreach (XElement prerequisiteElement in talentElement.Elements("PrerequisiteTalentArray"))
            {
                prerequisiteTalentArray.AddElement(prerequisiteElement);
            }

            return prerequisiteTalentArray;
        }

        private void FindUnits(XElement heroElement, Hero hero)
        {
            if (heroElement == null)
                return;

            // parent lookup
            string? parentValue = heroElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    FindUnits(parentElement, hero);
            }

            string? unit = heroElement.Element("Unit")?.Attribute("value")?.Value;

            if (!string.IsNullOrEmpty(unit) && HeroDataOverride != null && !HeroDataOverride.ContainsRemovedHeroUnit(unit))
                HeroDataOverride.AddHeroUnit(unit);
        }

        private void ValidateAbilityTalentLinkIds(Hero hero)
        {
            foreach (Talent talent in hero.Talents)
            {
                // validate all abilityTalentLinkIds
                HashSet<string> validatedIds = new HashSet<string>();

                foreach (string abilityTalentLinkId in talent.AbilityTalentLinkIds)
                {
                    IEnumerable<Ability> abilities = hero.GetAbilities(abilityTalentLinkId, StringComparison.OrdinalIgnoreCase);
                    IEnumerable<Hero> heroes = hero.HeroUnits.Where(x => x.GetAbilities(abilityTalentLinkId, StringComparison.OrdinalIgnoreCase).Where(x => x.Tier != AbilityTier.Hidden).Any());

                    if (abilities.Where(x => x.Tier != AbilityTier.Hidden).Any() || hero.ContainsTalent(abilityTalentLinkId) || heroes.Any())
                    {
                        validatedIds.Add(abilityTalentLinkId);
                    }
                }

                talent.ClearAbilityTalentLinkIds();

                foreach (string validatedId in validatedIds)
                {
                    talent.AddAbilityTalentLinkId(validatedId);
                }
            }
        }

        private void ValidateSubAbilities(Hero hero)
        {
            List<Ability> removableSubAbilities = new List<Ability>();

            foreach (Ability subAbility in hero.SubAbilities())
            {
                AbilityTalentId? parentLinkId = subAbility.ParentLink!;

                // check the abilityType to see if it has one set and then check if it should be an ability or talent
                if (parentLinkId.AbilityType == AbilityType.Unknown)
                {
                    Ability? ability = hero.GetAbilities(parentLinkId.ReferenceId, StringComparison.OrdinalIgnoreCase).FirstOrDefault(x => x.ParentLink == null); // get the first
                    if (ability != null)
                    {
                        parentLinkId.AbilityType = ability.AbilityTalentId.AbilityType;
                        parentLinkId.IsPassive = ability.AbilityTalentId.IsPassive;
                    }
                    else // is a talent
                    {
                        parentLinkId.AbilityType = AbilityType.Unknown;
                        parentLinkId.IsPassive = false;
                    }
                }
                else // verify it is an ability
                {
                    if (!hero.ContainsAbility(parentLinkId))
                    {
                        if (hero.ContainsTalent(parentLinkId.ReferenceId))
                            parentLinkId.AbilityType = AbilityType.Unknown;
                    }
                    else
                    {
                        if (hero.GetAbility(parentLinkId).ParentLink != null && hero.ContainsTalent(parentLinkId.ReferenceId))
                            parentLinkId.AbilityType = AbilityType.Unknown;
                    }
                }
            }
        }

        private void AddHeroUnits(Hero hero, UnitData unitData)
        {
            if (HeroDataOverride != null)
            {
                foreach (string heroUnit in HeroDataOverride.HeroUnits)
                {
                    if (string.IsNullOrEmpty(heroUnit))
                        continue;

                    Hero newHeroUnit = new Hero
                    {
                        Id = heroUnit,
                        CUnitId = heroUnit,
                        CHeroId = heroUnit,
                    };

                    unitData.SetUnitData(newHeroUnit, HeroOverrideLoader.GetOverride(newHeroUnit.CHeroId) ?? new HeroDataOverride());

                    // set the hyperlinkId to id if it doesn't have one
                    if (string.IsNullOrEmpty(newHeroUnit.HyperlinkId))
                        newHeroUnit.HyperlinkId = newHeroUnit.Id;

                    hero.AddHeroUnit(newHeroUnit);
                }
            }
        }

        private void ClearHeroUnitsFromUnitIds(Hero hero)
        {
            foreach (Hero heroUnit in hero.HeroUnits)
            {
                hero.RemoveUnitId(heroUnit.CUnitId);
            }
        }
    }
}
