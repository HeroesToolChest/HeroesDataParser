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
        private readonly HeroOverrideLoader _heroOverrideLoader;
        private readonly TalentData _talentData;

        private XmlArrayElement? _talentsArray;

        private HeroDataOverride? _heroDataOverride;

        public HeroDataParser(IXmlDataService xmlDataService, HeroOverrideLoader heroOverrideLoader)
            : base(xmlDataService)
        {
            if (xmlDataService is null)
                throw new ArgumentNullException(nameof(xmlDataService));

            _heroOverrideLoader = heroOverrideLoader ?? throw new ArgumentNullException(nameof(heroOverrideLoader));
            _talentData = xmlDataService.TalentData;
        }

        /// <summary>
        /// Gets a collection of all the parsable ids. Allows multiple ids.
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
                    string? id = hero.Attribute("id")?.Value;
                    if (string.IsNullOrEmpty(id))
                        continue;

                    XElement? attributIdValue = hero.Elements("AttributeId")?.FirstOrDefault(x => x.Attribute("value") != null);

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
        /// <param name="ids">The ids of the item to parse.</param>
        /// <returns></returns>
        public Hero? Parse(params string[] ids)
        {
            string? heroId = ids.FirstOrDefault();
            if (string.IsNullOrEmpty(heroId))
                return null;

            _talentsArray = new XmlArrayElement();

            UnitData unitData = XmlDataService.UnitData;
            unitData.IsAbilityTierFilterEnabled = true;
            unitData.IsAbilityTypeFilterEnabled = true;
            unitData.IsHeroParsing = true;
            unitData.Localization = Localization;

            Hero hero = new Hero
            {
                Name = GameData.GetGameString(DefaultData.HeroData?.HeroName?.Replace(DefaultData.IdPlaceHolder, heroId, StringComparison.OrdinalIgnoreCase)),
                Description = new TooltipDescription(GameData.GetGameString(DefaultData.HeroData?.HeroDescription?.Replace(DefaultData.IdPlaceHolder, heroId, StringComparison.OrdinalIgnoreCase)), Localization),
                InfoText = new TooltipDescription(GameData.GetGameString(DefaultData.HeroData?.HeroInfoText?.Replace(DefaultData.IdPlaceHolder, heroId, StringComparison.OrdinalIgnoreCase)), Localization),
                CHeroId = heroId,
                Id = heroId,
            };

            _heroDataOverride = _heroOverrideLoader.GetOverride(heroId) ?? new HeroDataOverride();

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
            unitData.SetUnitData(hero, _heroDataOverride, true);

            // parses the hero's hero data; talents
            SetData(heroElement, hero);
            SetTalents(hero);

            ClearHeroUnitsFromUnitIds(hero);

            // execute all overrides
            ApplyOverrides(hero, _heroDataOverride);
            foreach (Hero heroUnit in hero.HeroUnits)
                ApplyOverrides(heroUnit, _heroOverrideLoader.GetOverride(heroUnit.CHeroId) ?? new HeroDataOverride());

            // validation
            ValidateAbilityTalentLinkIds(hero);
            ValidateSubAbilities(hero);
            foreach (Hero heroUnit in hero.HeroUnits)
                ValidateSubAbilities(heroUnit);

            return hero;
        }

        public HeroDataParser GetInstance()
        {
            return new HeroDataParser(XmlDataService, _heroOverrideLoader);
        }

        protected override void ApplyAdditionalOverrides(Hero t, HeroDataOverride dataOverride)
        {
            if (t is null)
                throw new ArgumentNullException(nameof(t));
            if (dataOverride is null)
                throw new ArgumentNullException(nameof(dataOverride));

            if (dataOverride.EnergyTypeOverride.Enabled)
                t.Energy.EnergyType = dataOverride.EnergyTypeOverride.EnergyType;

            if (dataOverride.EnergyOverride.Enabled)
                t.Energy.EnergyMax = dataOverride.EnergyOverride.Energy;

            if (dataOverride.ParentLinkOverride.Enabled)
                t.ParentLink = dataOverride.ParentLinkOverride.ParentLink;

            if (t.Abilities != null)
            {
                foreach (AbilityTalentId addedAbility in dataOverride.AddedAbilities)
                {
                    Ability? ability = XmlDataService.AbilityData.CreateAbility(t.CUnitId, addedAbility);

                    if (ability != null)
                    {
                        if (dataOverride.IsAddedAbility(addedAbility))
                            t.AddAbility(ability);
                        else
                            t.RemoveAbility(ability);
                    }
                }

                dataOverride.ExecuteAbilityOverrides(t.Abilities);
            }

            if (t.HeroPortrait != null)
                dataOverride.ExecutePortraitOverrides(t.CHeroId, t.HeroPortrait);

            if (t.Talents != null)
            {
                dataOverride.ExecuteTalentOverrides(t.Talents);
            }

            if (t.Weapons != null)
            {
                foreach (string addedWeapon in dataOverride.AddedWeapons)
                {
                    UnitWeapon? weapon = XmlDataService.WeaponData.CreateWeapon(addedWeapon);

                    if (weapon != null)
                    {
                        if (dataOverride.IsAddedWeapon(addedWeapon))
                            t.Weapons.Add(weapon);
                        else
                            t.Weapons.Remove(weapon);
                    }
                }

                dataOverride.ExecuteWeaponOverrides(t.Weapons);
            }
        }

        private static XmlArrayElement GetTalentPrerequisites(XElement talentElement)
        {
            XmlArrayElement prerequisiteTalentArray = new XmlArrayElement();

            foreach (XElement prerequisiteElement in talentElement.Elements("PrerequisiteTalentArray"))
            {
                prerequisiteTalentArray.AddElement(prerequisiteElement);
            }

            return prerequisiteTalentArray;
        }

        private static void ValidateAbilityTalentLinkIds(Hero hero)
        {
            foreach (Talent talent in hero.Talents)
            {
                // validate all abilityTalentLinkIds
                HashSet<string> validatedIds = new HashSet<string>();

                foreach (string abilityTalentLinkId in talent.AbilityTalentLinkIds)
                {
                    IEnumerable<Ability> abilities = hero.GetAbilitiesFromReferenceId(abilityTalentLinkId, StringComparison.Ordinal);
                    IEnumerable<Hero> heroes = hero.HeroUnits.Where(x => x.GetAbilitiesFromReferenceId(abilityTalentLinkId, StringComparison.Ordinal).Where(x => x.Tier != AbilityTiers.Hidden).Any());

                    if (abilities.Where(x => x.Tier != AbilityTiers.Hidden).Any() || (hero.TryGetTalent(abilityTalentLinkId, out Talent? foundTalent) && talent != foundTalent) || heroes.Any())
                    {
                        validatedIds.Add(abilityTalentLinkId);
                    }
                }

                talent.AbilityTalentLinkIds.Clear();

                foreach (string validatedId in validatedIds)
                {
                    talent.AbilityTalentLinkIds.Add(validatedId);
                }
            }
        }

        private static void ValidateSubAbilities(Hero hero)
        {
            List<Ability> removableSubAbilities = new List<Ability>();

            foreach (Ability subAbility in hero.SubAbilities())
            {
                AbilityTalentId? parentLinkId = subAbility.ParentLink!;

                // check the abilityType to see if it has one set and then check if it should be an ability or talent
                if (parentLinkId.AbilityType == AbilityTypes.Unknown)
                {
                    Ability? ability = hero.GetAbilitiesFromReferenceId(parentLinkId.ReferenceId, StringComparison.OrdinalIgnoreCase).FirstOrDefault(x => x.ParentLink == null); // get the first
                    if (ability != null)
                    {
                        parentLinkId.AbilityType = ability.AbilityTalentId.AbilityType;
                        parentLinkId.IsPassive = ability.AbilityTalentId.IsPassive;
                    }
                    else // is a talent
                    {
                        parentLinkId.AbilityType = AbilityTypes.Unknown;
                        parentLinkId.IsPassive = false;
                    }
                }
                else // verify it is an ability
                {
                    if (!hero.ContainsAbility(parentLinkId))
                    {
                        if (hero.ContainsTalent(parentLinkId.ReferenceId))
                            parentLinkId.AbilityType = AbilityTypes.Unknown;
                    }
                    else
                    {
                        if (hero.GetAbility(parentLinkId).ParentLink != null && hero.ContainsTalent(parentLinkId.ReferenceId))
                            parentLinkId.AbilityType = AbilityTypes.Unknown;
                    }
                }
            }
        }

        private static void ClearHeroUnitsFromUnitIds(Hero hero)
        {
            foreach (Hero heroUnit in hero.HeroUnits)
            {
                hero.UnitIds.Remove(heroUnit.CUnitId);
            }
        }

        private void SetDefaultValues(Hero hero)
        {
            hero.Type = new TooltipDescription(GameData.GetGameString(DefaultData.StringRanged).Trim());
            hero.Radius = DefaultData.HeroData!.UnitRadius;
            hero.Speed = DefaultData.HeroData.UnitSpeed;
            hero.Sight = DefaultData.HeroData.UnitSight;
            hero.ReleaseDate = DefaultData.HeroData.HeroReleaseDate;
            hero.Gender = UnitGender.Male;
            hero.Franchise = Franchise.Unknown;
            hero.Life.LifeMax = DefaultData.HeroData.UnitLifeMax;
            hero.Life.LifeRegenerationRate = 0;
            hero.Energy.EnergyMax = DefaultData.HeroData.UnitEnergyMax;
            hero.Energy.EnergyRegenerationRate = DefaultData.HeroData.UnitEnergyRegenRate;
            hero.Shield.ShieldMax = DefaultData.HeroData.UnitShieldMax;
            hero.Shield.ShieldRegenerationRate = DefaultData.HeroData.UnitShieldRegenRate;
            hero.Shield.ShieldRegenerationDelay = DefaultData.HeroData.UnitShieldRegenDelay;
            hero.Difficulty = GameData.GetGameString(DefaultData.Difficulty.Replace(DefaultData.IdPlaceHolder, DefaultData.DefaultHeroDifficulty, StringComparison.OrdinalIgnoreCase)).Trim();

            if (hero.CHeroId != null)
            {
                hero.HyperlinkId = DefaultData.HeroData.HeroHyperlinkId!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId, StringComparison.OrdinalIgnoreCase);
                hero.CUnitId = DefaultData.HeroData.HeroUnit!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId, StringComparison.OrdinalIgnoreCase);

                hero.HeroPortrait.HeroSelectPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroSelectScreenButtonImage!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId, StringComparison.OrdinalIgnoreCase)))?.ToLowerInvariant() ?? string.Empty;
                hero.HeroPortrait.LeaderboardPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroLeaderboardImage!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId, StringComparison.OrdinalIgnoreCase)))?.ToLowerInvariant() ?? string.Empty;
                hero.HeroPortrait.LoadingScreenPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroLoadingScreenImage!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId, StringComparison.OrdinalIgnoreCase)))?.ToLowerInvariant() ?? string.Empty;
                hero.HeroPortrait.PartyPanelPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroPartyPanelButtonImage!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId, StringComparison.OrdinalIgnoreCase)))?.ToLowerInvariant() ?? string.Empty;
                hero.HeroPortrait.TargetPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroPortrait!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId, StringComparison.OrdinalIgnoreCase)))?.ToLowerInvariant() ?? string.Empty;
                hero.HeroPortrait.PartyFrameFileName.Add(Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroPartyFrameImage!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId, StringComparison.OrdinalIgnoreCase)))?.ToLowerInvariant() ?? string.Empty);
                hero.HeroPortrait.DraftScreenFileName = Path.GetFileName(PathHelper.GetFilePath(DefaultData.HeroData.HeroDraftScreenImage!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId, StringComparison.OrdinalIgnoreCase)))?.ToLowerInvariant() ?? string.Empty;

                hero.Title = GameData.GetGameString(DefaultData.HeroData.HeroTitle!.Replace(DefaultData.IdPlaceHolder, hero.CHeroId, StringComparison.OrdinalIgnoreCase));

                hero.SearchText = GameData.GetGameString(DefaultData.HeroData.HeroAlternateNameSearchText?.Replace(DefaultData.IdPlaceHolder, hero.CHeroId, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(hero.SearchText) && hero.SearchText.Last() != ' ')
                    hero.SearchText += " ";
                hero.SearchText += GameData.GetGameString(DefaultData.HeroData.HeroAdditionalSearchText?.Replace(DefaultData.IdPlaceHolder, hero.CHeroId, StringComparison.OrdinalIgnoreCase));
                hero.SearchText = hero.SearchText.Trim();

                if (_heroDataOverride != null && _heroDataOverride.CUnitOverride.Enabled)
                    hero.CUnitId = _heroDataOverride.CUnitOverride.CUnit;
            }
        }

        private void SetData(XElement heroElement, Hero hero)
        {
            if (heroElement == null || hero == null)
                return;

            _talentData.SetButtonTooltipAppenderData(hero);

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
                string elementName = element.Name.LocalName.ToUpperInvariant();

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
                    if (element.Attribute("value")?.Value == "1")
                        hero.Type = new TooltipDescription(GameData.GetGameString(DefaultData.StringMelee).Trim());
                    else if (element.Attribute("value")?.Value == "0")
                        hero.Type = new TooltipDescription(GameData.GetGameString(DefaultData.StringRanged).Trim());
                    else
                        hero.Type = new TooltipDescription(GameData.GetGameString(DefaultData.StringRanged).Trim());
                }
                else if (elementName == "DIFFICULTY")
                {
                    string? difficultyValue = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(difficultyValue))
                    {
                        hero.Difficulty = GameData.GetGameString(DefaultData.Difficulty.Replace(DefaultData.IdPlaceHolder, difficultyValue, StringComparison.OrdinalIgnoreCase)).Trim();
                    }
                }
                else if (elementName == "ROLE" || elementName == "ROLESMULTICLASS")
                {
                    string? roleValue = element.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(roleValue))
                    {
                        string role = GameData.GetGameString(DefaultData.HeroData?.HeroRoleName?.Replace(DefaultData.IdPlaceHolder, roleValue, StringComparison.OrdinalIgnoreCase)).Trim();
                        if (!string.IsNullOrEmpty(role))
                        {
                            hero.Roles.Add(role);
                        }
                    }
                }
                else if (elementName == "UNIVERSEICON")
                {
                    string? iconImage = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value))?.ToUpperInvariant();

                    if (iconImage == "UI_GLUES_STORE_GAMEICON_SC2.DDS")
                        hero.Franchise = Franchise.Starcraft;
                    else if (iconImage == "UI_GLUES_STORE_GAMEICON_WOW.DDS")
                        hero.Franchise = Franchise.Warcraft;
                    else if (iconImage == "UI_GLUES_STORE_GAMEICON_D3.DDS")
                        hero.Franchise = Franchise.Diablo;
                    else if (iconImage == "UI_GLUES_STORE_GAMEICON_OW.DDS")
                        hero.Franchise = Franchise.Overwatch;
                    else if (iconImage == "UI_GLUES_STORE_GAMEICON_RETRO.DDS")
                        hero.Franchise = Franchise.Classic;
                    else if (iconImage == "UI_GLUES_STORE_GAMEICON_NEXUS.DDS")
                        hero.Franchise = Franchise.Nexus;
                }
                else if (elementName == "UNIVERSE")
                {
                    string? universe = element.Attribute("value")?.Value.ToUpperInvariant();

                    if (universe == "STARCRAFT")
                        hero.Franchise = Franchise.Starcraft;
                    else if (universe == "WARCRAFT")
                        hero.Franchise = Franchise.Warcraft;
                    else if (universe == "DIABLO")
                        hero.Franchise = Franchise.Diablo;
                    else if (universe == "OVERWATCH")
                        hero.Franchise = Franchise.Overwatch;
                    else if (universe == "NEXUS")
                        hero.Franchise = Franchise.Nexus;
                    else if (universe == "RETRO" && hero.Franchise == Franchise.Unknown)
                        hero.Franchise = Franchise.Classic;
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
                    if (Enum.TryParse(element.Attribute("value")?.Value, out UnitGender unitGender))
                        hero.Gender = unitGender;
                    else
                        hero.Gender = UnitGender.Neutral;
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value")?.Value, out Rarity heroRarity))
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
                    hero.HeroPortrait.HeroSelectPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value))?.ToLowerInvariant();
                }
                else if (elementName == "SCORESCREENIMAGE")
                {
                    hero.HeroPortrait.LeaderboardPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value))?.ToLowerInvariant();
                }
                else if (elementName == "LOADINGSCREENIMAGE")
                {
                    hero.HeroPortrait.LoadingScreenPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value))?.ToLowerInvariant();
                }
                else if (elementName == "PARTYPANELBUTTONIMAGE")
                {
                    hero.HeroPortrait.PartyPanelPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value))?.ToLowerInvariant();
                }
                else if (elementName == "PORTRAIT")
                {
                    hero.HeroPortrait.TargetPortraitFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value))?.ToLowerInvariant();
                }
                else if (elementName == "PARTYFRAMEIMAGE")
                {
                    string? partyFrameImage = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value))?.ToLowerInvariant();
                    if (partyFrameImage is not null)
                        hero.HeroPortrait.PartyFrameFileName.Add(partyFrameImage);
                }
                else if (elementName == "DRAFTSCREENPORTRAIT")
                {
                    hero.HeroPortrait.DraftScreenFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value))?.ToLowerInvariant();
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
                        hero.ExpandedRole = GameData.GetGameString(DefaultData.HeroData?.HeroRoleName?.Replace(DefaultData.IdPlaceHolder, role, StringComparison.OrdinalIgnoreCase)).Trim();
                }
                else if (elementName == "TALENTTREEARRAY")
                {
                    _talentsArray?.AddElement(element);
                }
                else if (elementName == "SKINARRAY")
                {
                    string? skin = element.Attribute("value")?.Value;

                    if (skin is not null)
                    {
                        hero.SkinIds.Add(skin);
                    }
                }
                else if (elementName == "VARIATIONARRAY")
                {
                    string? variation = element.Attribute("value")?.Value;

                    if (variation is not null)
                    {
                        hero.VariationSkinIds.Add(variation);
                    }
                }
                else if (elementName == "VOICELINEARRAY")
                {
                    string? voiceLine = element.Attribute("value")?.Value;

                    if (voiceLine is not null)
                    {
                        hero.VoiceLineIds.Add(voiceLine);
                    }
                }
                else if (elementName == "ALLOWEDMOUNTCATEGORYARRAY")
                {
                    string? allowedMountCategory = element.Attribute("value")?.Value;

                    if (allowedMountCategory is not null)
                    {
                        hero.AllowedMountCategoryIds.Add(allowedMountCategory);
                    }
                }
                else if (elementName == "DEFAULTMOUNT")
                {
                    string? defaultMount = element.Attribute("value")?.Value;

                    if (defaultMount is not null)
                    {
                        hero.DefaultMountId = defaultMount;
                    }
                }
            }

            if (hero.ReleaseDate == DefaultData.HeroData?.HeroReleaseDate)
                hero.ReleaseDate = DefaultData.HeroData?.HeroAlphaReleaseDate;
        }

        private void SetTalents(Hero hero)
        {
            if (_talentsArray == null)
                throw new InvalidOperationException("Call SetData() first to set up the talents");

            foreach (XElement element in _talentsArray.Elements)
            {
                Talent? talent = _talentData.CreateTalent(hero, element);
                if (talent != null)
                {
                    XmlArrayElement prerequisiteTalentArray = GetTalentPrerequisites(element);

                    foreach (XElement prerequisiteTalentElement in prerequisiteTalentArray.Elements)
                    {
                        string? talentPrerequisite = prerequisiteTalentElement.Attribute("value")?.Value;

                        if (!string.IsNullOrEmpty(talentPrerequisite))
                            talent.PrerequisiteTalentIds.Add(talentPrerequisite);
                    }

                    hero.AddTalent(talent);

                    // makes the abilities that are granted from talents subabilities to that talent
                    if (talent.AbilityTalentId.AbilityType != AbilityTypes.Heroic || talent.Tier == TalentTiers.Level20)
                    {
                        IEnumerable<Ability> abilities = hero.GetAbilitiesFromReferenceId(talent.AbilityTalentId.ReferenceId, StringComparison.OrdinalIgnoreCase);

                        foreach (Ability ability in abilities)
                        {
                            ability.ParentLink = new AbilityTalentId(talent.AbilityTalentId.ReferenceId, talent.AbilityTalentId.ButtonId)
                            {
                                AbilityType = talent.AbilityTalentId.AbilityType,
                                IsPassive = talent.AbilityTalentId.IsPassive,
                            };
                        }
                    }
                }
            }
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

            if (!string.IsNullOrEmpty(unit) && _heroDataOverride != null && !_heroDataOverride.ContainsRemovedHeroUnit(unit))
                _heroDataOverride.AddHeroUnit(unit);
        }

        private void AddHeroUnits(Hero hero, UnitData unitData)
        {
            if (_heroDataOverride != null)
            {
                foreach (string heroUnit in _heroDataOverride.HeroUnits)
                {
                    if (string.IsNullOrEmpty(heroUnit))
                        continue;

                    Hero newHeroUnit = new Hero
                    {
                        Id = heroUnit,
                        CUnitId = heroUnit,
                        CHeroId = heroUnit,
                    };

                    unitData.SetUnitData(newHeroUnit, _heroOverrideLoader.GetOverride(newHeroUnit.CHeroId) ?? new HeroDataOverride());

                    // set the hyperlinkId to id if it doesn't have one
                    if (string.IsNullOrEmpty(newHeroUnit.HyperlinkId))
                        newHeroUnit.HyperlinkId = newHeroUnit.Id;

                    hero.HeroUnits.Add(newHeroUnit);
                }
            }
        }
    }
}
