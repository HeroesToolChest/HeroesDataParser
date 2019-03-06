using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultData
    {
        /// <summary>
        /// Id to be replaced in some strings.
        /// </summary>
        public const string IdPlaceHolder = "##id##";

        public const string ReplacementCharacter = "%1";

        public const string StringRanged = "e_gameUIStringRanged";
        public const string StringMelee = "e_gameUIStringMelee";
        public const string StringChargeCooldownColon = "e_gameUIStringChargeCooldownColon";
        public const string StringCooldownColon = "e_gameUIStringCooldownColon";
        public const string AbilTooltipCooldownText = "UI/AbilTooltipCooldown";
        public const string AbilTooltipCooldownPluralText = "UI/AbilTooltipCooldownPlural";
        public const string MatchAwardMapSpecificInstanceNamePrefix = "UserData/EndOfMatchMapSpecificAward/";
        public const string HeroEnergyTypeManaText = "UI/HeroEnergyType/Mana";

        public const string CButtonDefaultBaseId = "StormButtonParent";
        public const string CUnitDefaultBaseId = "StormHero";

        public const string DefaultHeroDifficulty = "Easy";

        public const string AbilMountLinkId = "Mount";

        private readonly GameData GameData;

        public DefaultData(GameData gameData)
        {
            GameData = gameData;
        }

        /// <summary>
        /// Gets or sets the default summon mount ability id name.
        /// </summary>
        public string DefaultSummonMountAbilityId { get; set; } = "SummonMount";

        /// <summary>
        /// Gets or sets the default hearth ability id name.
        /// </summary>
        public string DefaultHearthAbilityId { get; set; } = "PortBackToBase";

        /// <summary>
        /// Gets or sets the default hearth no mana ability id name.
        /// </summary>
        public string DefaultHearthNoManaAbilityId { get; set; } = "PortBackToBaseNoMana";

        /// <summary>
        /// Gets the default hero name text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string HeroName { get; private set; }

        /// <summary>
        /// Gets the default unit name text. Contains ##id##. Use with CUnit id.
        /// </summary>
        public string UnitName { get; private set; }

        /// <summary>
        /// Gets the default description text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string HeroDescription { get; private set; }

        /// <summary>
        /// Gets the default life amount.
        /// </summary>
        public double UnitLifeMax { get; private set; }

        /// <summary>
        /// Gets the default radius value.
        /// </summary>
        public double UnitRadius { get; private set; }

        /// <summary>
        /// Gets the default speed value.
        /// </summary>
        public double UnitSpeed { get; private set; }

        /// <summary>
        /// Gets the default sight value.
        /// </summary>
        public double UnitSight { get; private set; }

        /// <summary>
        /// Gets the default energy value.
        /// </summary>
        public double UnitEnergyMax { get; private set; }

        /// <summary>
        /// Gets the default energy regeneration rate.
        /// </summary>
        public double UnitEnergyRegenRate { get; private set; }

        /// <summary>
        /// Gets the default portrait text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string HeroPortrait { get; private set; }

        /// <summary>
        /// Gets the default select screen button text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string HeroSelectScreenButtonImage { get; private set; }

        /// <summary>
        /// Gets the default party panel button image text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string HeroPartyPanelButtonImage { get; private set; }

        /// <summary>
        /// Gets the default leaderboard image text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string HeroLeaderboardImage { get; private set; }

        /// <summary>
        /// Gets the default loading screen text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string HeroLoadingScreenImage { get; private set; }

        /// <summary>
        /// Gets the default hyperlink id text. Used for shortname. Contains ##id##. Use with CHero id.
        /// </summary>
        public string HeroHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default release date value.
        /// </summary>
        public DateTime HeroReleaseDate { get; private set; }

        /// <summary>
        /// Gets the alpha release date of 2014/3/13.
        /// </summary>
        public DateTime HeroAlphaReleaseDate { get; private set; } = new DateTime(2014, 3, 13);

        /// <summary>
        /// Gets the unit text. Contains ##id##. Used to get the cUnitId.
        /// </summary>
        public string HeroUnit { get; private set; }

        /// <summary>
        /// Gets the default hero role text. Contains ##id##.
        /// </summary>
        public string HeroRoleName { get; private set; }

        /// <summary>
        /// Gets the default hero information text. Contains ##id##.
        /// </summary>
        public string HeroInfoText { get; private set; }

        /// <summary>
        /// Gets the default hero title. Contains ##id##.
        /// </summary>
        public string HeroTitle { get; private set; }

        /// <summary>
        /// Gets the default additional search text. Contains ##id##.
        /// </summary>
        public string HeroAdditionalSearchText { get; private set; }

        /// <summary>
        /// Gets the default alternamte name search text. Contains ##id##.
        /// </summary>
        public string HeroAlternateNameSearchText { get; private set; }

        /// <summary>
        /// Gets the default button name text. Contains ##id##.
        /// </summary>
        public string ButtonName { get; private set; }

        /// <summary>
        /// Gets the default button tooltip text. Contains ##id##. Full text.
        /// </summary>
        public string ButtonTooltip { get; private set; }

        /// <summary>
        /// Gets the default button simple display text. Contains ##id##. Short text.
        /// </summary>
        public string ButtonSimpleDisplayText { get; private set; }

        /// <summary>
        /// Gets the default button tooltip vital text.
        /// </summary>
        public string ButtonTooltipEnergyVitalName { get; private set; }

        /// <summary>
        /// Gets the default button hotkey text. Contains ##id##.
        /// </summary>
        public string ButtonHotkey { get; private set; }

        /// <summary>
        /// Gets the default button hotkey alias text. Contains ##id##.
        /// </summary>
        public string ButtonHotkeyAlias { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show name.
        /// </summary>
        public bool ButtonTooltipFlagShowName { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show hotkey.
        /// </summary>
        public bool ButtonTooltipFlagShowHotkey { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show usage.
        /// </summary>
        public bool ButtonTooltipFlagShowUsage { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show time.
        /// </summary>
        public bool ButtonTooltipFlagShowTime { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show cooldown.
        /// </summary>
        public bool ButtonTooltipFlagShowCooldown { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show requirements.
        /// </summary>
        public bool ButtonTooltipFlagShowRequirements { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show autocast.
        /// </summary>
        public bool ButtonTooltipFlagShowAutocast { get; private set; }

        /// <summary>
        /// Gets the defualt weapon name text. Contains ##id##.
        /// </summary>
        public string WeaponName { get; private set; }

        /// <summary>
        /// Gets the default weapon range value.
        /// </summary>
        public double WeaponRange { get; private set; }

        /// <summary>
        /// Gets the default weapon period value.
        /// </summary>
        public double WeaponPeriod { get; private set; }

        /// <summary>
        /// Gets the default weapon display effect name. Contains ##id##.
        /// </summary>
        public string WeaponDisplayEffect { get; private set; }

        /// <summary>
        /// Gets the default hero skin name. Contains ##id##.
        /// </summary>
        public string HeroSkinName { get; private set; }

        /// <summary>
        /// Gets the default hero skin name used for sorting. Contains ##id##.
        /// </summary>
        public string HeroSkinSortName { get; private set; }

        /// <summary>
        /// Gets the default hero skin info text. Contains ##id##.
        /// </summary>
        public string HeroSkinInfoText { get; private set; }

        /// <summary>
        /// Gets the default hero skin additional search text. Contains ##id##.
        /// </summary>
        public string HeroSkinAdditionalSearchText { get; private set; }

        /// <summary>
        /// Gets the default hero skin hyperlinkId. Contains ##id##.
        /// </summary>
        public string HeroSkinHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default hero skins release date.
        /// </summary>
        public DateTime HeroSkinReleaseDate { get; private set; }

        /// <summary>
        /// Gets the default mount name. Contains ##id##.
        /// </summary>
        public string MountName { get; private set; }

        /// <summary>
        /// Gets the default mount name used for sorting. Contains ##id##.
        /// </summary>
        public string MountSortName { get; private set; }

        /// <summary>
        /// Gets the default mount info text. Contains ##id##.
        /// </summary>
        public string MountInfoText { get; private set; }

        /// <summary>
        /// Gets the default mount additional search text. Contains ##id##.
        /// </summary>
        public string MountAdditionalSearchText { get; private set; }

        /// <summary>
        /// Gets the default mount hyperlinkId. Contains ##id##.
        /// </summary>
        public string MountHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default mount release date.
        /// </summary>
        public DateTime MountReleaseDate { get; private set; }

        /// <summary>
        /// Gets the default banner name. Contains ##id##.
        /// </summary>
        public string BannerName { get; private set; }

        /// <summary>
        /// Gets the default banner name used for sorting. Contains ##id##.
        /// </summary>
        public string BannerSortName { get; private set; }

        /// <summary>
        /// Gets the default banner description. Contains ##id##.
        /// </summary>
        public string BannerDescription { get; private set; }

        /// <summary>
        /// Gets the default banner release date.
        /// </summary>
        public DateTime BannerReleaseDate { get; private set; }

        /// <summary>
        /// Gets the default spray name. Contains ##id##.
        /// </summary>
        public string SprayName { get; private set; }

        /// <summary>
        /// Gets the default spray name used for sorting. Contains ##id##.
        /// </summary>
        public string SpraySortName { get; private set; }

        /// <summary>
        /// Gets the default spray description. Contains ##id##.
        /// </summary>
        public string SprayDescription { get; private set; }

        /// <summary>
        /// Gets the default spray additional search text. Contains ##id##.
        /// </summary>
        public string SprayAdditionalSearchText { get; private set; }

        /// <summary>
        /// Gets the default spray hyperlinkId. Contains ##id##.
        /// </summary>
        public string SprayHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default spray release date.
        /// </summary>
        public DateTime SprayReleaseDate { get; private set; }

        /// <summary>
        /// Gets the default announcer name. Contains ##id##.
        /// </summary>
        public string AnnouncerName { get; private set; }

        /// <summary>
        /// Gets the default announcer name used for sorting. Contains ##id##.
        /// </summary>
        public string AnnouncerSortName { get; private set; }

        /// <summary>
        /// Gets the default announcer description. Contains ##id##.
        /// </summary>
        public string AnnouncerDescription { get; private set; }

        /// <summary>
        /// Gets the default announcer release date.
        /// </summary>
        public DateTime AnnouncerReleaseDate { get; private set; }

        /// <summary>
        /// Gets the default difficulty text. Contains ##id##.
        /// </summary>
        public string Difficulty { get; } = $"UI/HeroUtil/Difficulty/{IdPlaceHolder}";

        /// <summary>
        /// Load all default data.
        /// </summary>
        /// <remarks>Order is important.</remarks>
        public void Load()
        {
            if (GameData == null)
                return;

            LoadCHeroDefault();

            LoadCUnitDefault();
            LoadCUnitDefaultStormBasicHeroicUnit();
            LoadCUnitDefaultStormHero();

            LoadCHeroRoleDefault();

            LoadCButtonDefault();
            LoadCButtonDefaultStormButtonParent();

            LoadCWeaponDefault();

            LoadCSkinDefault();
            LoadCMountDefault();
            LoadCBannerDefault();
            LoadCSprayDefault();
            LoadCAnnouncerPackDefault();
        }

        // <CUnit default="1">
        private void LoadCUnitDefault()
        {
            CUnitElement(GameData.CUnitElements.Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CUnit default="1" id="StormBasicHeroicUnit">
        private void LoadCUnitDefaultStormBasicHeroicUnit()
        {
            CUnitElement(GameData.CUnitElements.Where(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == "StormBasicHeroicUnit"));
        }

        // <CUnit default="1" id="StormHero" parent="StormBasicHeroicUnit">
        private void LoadCUnitDefaultStormHero()
        {
            CUnitElement(GameData.CUnitElements.Where(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == CUnitDefaultBaseId));
        }

        // <CHero default="1">
        private void LoadCHeroDefault()
        {
            CHeroElement(GameData.CHeroElements.Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CHeroRole default="1">
        private void LoadCHeroRoleDefault()
        {
            CHeroRoleElement(GameData.XmlGameData.Root.Elements("CHeroRole").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CButton default="1">
        private void LoadCButtonDefault()
        {
            CButtonElement(GameData.CButtonElements.Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CButton default="1" id="StormButtonParent">
        private void LoadCButtonDefaultStormButtonParent()
        {
            CButtonElement(GameData.CButtonElements.Where(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == CButtonDefaultBaseId));
        }

        // <CWeapon default="1">
        private void LoadCWeaponDefault()
        {
            CWeaponElement(GameData.XmlGameData.Root.Elements("CWeapon").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CSkin default="1">
        private void LoadCSkinDefault()
        {
            CSkinElement(GameData.CSkinElements.Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CMount default="1">
        private void LoadCMountDefault()
        {
            CMountElement(GameData.CMountElements.Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CBanner default="1">
        private void LoadCBannerDefault()
        {
            CBannerElement(GameData.CBannerElements.Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CSpray default="1">
        private void LoadCSprayDefault()
        {
            CSprayElement(GameData.CSprayElements.Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CAnnouncerPack default="1">
        private void LoadCAnnouncerPackDefault()
        {
            CAnnouncerPackElement(GameData.CAnnouncerPackElements.Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CUnitElement(IEnumerable<XElement> cUnitElements)
        {
            foreach (XElement element in cUnitElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    UnitName = element.Attribute("value").Value;
                }
                else if (elementName == "LIFEMAX")
                {
                    UnitLifeMax = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "RADIUS")
                {
                    UnitRadius = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "SPEED")
                {
                    UnitSpeed = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "SIGHT")
                {
                    UnitSight = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ENERGYMAX")
                {
                    UnitEnergyMax = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ENERGYREGENRATE")
                {
                    UnitEnergyRegenRate = double.Parse(element.Attribute("value").Value);
                }
            }
        }

        private void CHeroElement(IEnumerable<XElement> cHeroElements)
        {
            foreach (XElement element in cHeroElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    HeroName = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    HeroDescription = element.Attribute("value").Value;
                }
                else if (elementName == "PORTRAIT")
                {
                    HeroPortrait = element.Attribute("value").Value;
                }
                else if (elementName == "SELECTSCREENBUTTONIMAGE")
                {
                    HeroSelectScreenButtonImage = element.Attribute("value").Value;
                }
                else if (elementName == "SCORESCREENIMAGE")
                {
                    HeroLeaderboardImage = element.Attribute("value").Value;
                }
                else if (elementName == "LOADINGSCREENIMAGE")
                {
                    HeroLoadingScreenImage = element.Attribute("value").Value;
                }
                else if (elementName == "PARTYPANELBUTTONIMAGE")
                {
                    HeroPartyPanelButtonImage = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    HeroReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "UNIT")
                {
                    HeroUnit = element.Attribute("value").Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    HeroHyperlinkId = element.Attribute("value").Value;
                }
                else if (elementName == "INFOTEXT")
                {
                    HeroInfoText = element.Attribute("value").Value;
                }
                else if (elementName == "TITLE")
                {
                    HeroTitle = element.Attribute("value").Value;
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    HeroAdditionalSearchText = element.Attribute("value").Value;
                }
                else if (elementName == "ALTERNATENAMESEARCHTEXT")
                {
                    HeroAlternateNameSearchText = element.Attribute("value").Value;
                }
            }
        }

        private void CHeroRoleElement(IEnumerable<XElement> cHeroRoleElements)
        {
            foreach (XElement element in cHeroRoleElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    HeroRoleName = element.Attribute("value").Value;
                }
            }
        }

        private void CButtonElement(IEnumerable<XElement> cButtonElements)
        {
            foreach (XElement element in cButtonElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    ButtonName = element.Attribute("value").Value;
                }
                else if (elementName == "TOOLTIP")
                {
                    ButtonTooltip = element.Attribute("value").Value;
                }
                else if (elementName == "HOTKEY")
                {
                    ButtonHotkey = element.Attribute("value").Value;
                }
                else if (elementName == "HOTKEYALIAS")
                {
                    ButtonHotkeyAlias = element.Attribute("value").Value;
                }
                else if (elementName == "TOOLTIPFLAGS")
                {
                    string index = element.Attribute("index").Value;

                    if (index == "ShowName")
                        ButtonTooltipFlagShowName = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowHotkey")
                        ButtonTooltipFlagShowHotkey = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowUsage")
                        ButtonTooltipFlagShowUsage = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowTime")
                        ButtonTooltipFlagShowTime = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowCooldown")
                        ButtonTooltipFlagShowCooldown = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowRequirements")
                        ButtonTooltipFlagShowRequirements = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowAutocast")
                        ButtonTooltipFlagShowAutocast = element.Attribute("value").Value == "1" ? true : false;
                }
                else if (elementName == "SIMPLEDISPLAYTEXT")
                {
                    ButtonSimpleDisplayText = element.Attribute("value").Value;
                }
                else if (elementName == "TOOLTIPVITALNAME")
                {
                    string index = element.Attribute("index").Value;

                    if (index == "Energy")
                        ButtonTooltipEnergyVitalName = element.Attribute("value").Value;
                }
            }
        }

        private void CWeaponElement(IEnumerable<XElement> cWeaponElements)
        {
            foreach (XElement element in cWeaponElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    WeaponName = element.Attribute("value").Value;
                }
                else if (elementName == "DISPLAYEFFECT")
                {
                    WeaponDisplayEffect = element.Attribute("value").Value;
                }
                else if (elementName == "RANGE")
                {
                    WeaponRange = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "PERIOD")
                {
                    WeaponPeriod = double.Parse(element.Attribute("value").Value);
                }
            }
        }

        private void CSkinElement(IEnumerable<XElement> cSkinElements)
        {
            foreach (XElement element in cSkinElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    HeroSkinName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    HeroSkinSortName = element.Attribute("value").Value;
                }
                else if (elementName == "INFOTEXT")
                {
                    HeroSkinInfoText = element.Attribute("value").Value;
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    HeroSkinAdditionalSearchText = element.Attribute("value").Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    HeroSkinHyperlinkId = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 3;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    HeroSkinReleaseDate = new DateTime(year, month, day);
                }
            }
        }

        private void CMountElement(IEnumerable<XElement> cMountElements)
        {
            foreach (XElement element in cMountElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    MountName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    MountSortName = element.Attribute("value").Value;
                }
                else if (elementName == "INFOTEXT")
                {
                    MountInfoText = element.Attribute("value").Value;
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    MountAdditionalSearchText = element.Attribute("value").Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    MountHyperlinkId = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    MountReleaseDate = new DateTime(year, month, day);
                }
            }
        }

        private void CBannerElement(IEnumerable<XElement> cBannerElements)
        {
            foreach (XElement element in cBannerElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    BannerName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    BannerSortName = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    BannerDescription = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    BannerReleaseDate = new DateTime(year, month, day);
                }
            }
        }

        private void CSprayElement(IEnumerable<XElement> cSprayElements)
        {
            foreach (XElement element in cSprayElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    SprayName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    SpraySortName = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    SprayDescription = element.Attribute("value").Value;
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    SprayAdditionalSearchText = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    SprayReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "HYPERLINKID")
                {
                    SprayHyperlinkId = element.Attribute("value").Value;
                }
            }
        }

        private void CAnnouncerPackElement(IEnumerable<XElement> cAnnouncerPackElements)
        {
            foreach (XElement element in cAnnouncerPackElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    AnnouncerName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    AnnouncerSortName = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    AnnouncerDescription = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    AnnouncerReleaseDate = new DateTime(year, month, day);
                }
            }
        }
    }
}
