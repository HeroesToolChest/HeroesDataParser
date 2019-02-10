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
        public const string IdReplacer = "##id##";

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
        /// Gets the default difficulty text. Contains ##id##.
        /// </summary>
        public string Difficulty { get; } = $"UI/HeroUtil/Difficulty/{IdReplacer}";

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
        }

        // <CUnit default="1">
        private void LoadCUnitDefault()
        {
            CUnitElement(GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CUnit default="1" id="StormBasicHeroicUnit">
        private void LoadCUnitDefaultStormBasicHeroicUnit()
        {
            CUnitElement(GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == "StormBasicHeroicUnit"));
        }

        // <CUnit default="1" id="StormHero" parent="StormBasicHeroicUnit">
        private void LoadCUnitDefaultStormHero()
        {
            CUnitElement(GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == CUnitDefaultBaseId));
        }

        // <CHero default="1">
        private void LoadCHeroDefault()
        {
            CHeroElement(GameData.XmlGameData.Root.Elements("CHero").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CHeroRole default="1">
        private void LoadCHeroRoleDefault()
        {
            CHeroRoleElement(GameData.XmlGameData.Root.Elements("CHeroRole").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CButton default="1">
        private void LoadCButtonDefault()
        {
            CButtonElement(GameData.XmlGameData.Root.Elements("CButton").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CButton default="1" id="StormButtonParent">
        private void LoadCButtonDefaultStormButtonParent()
        {
            CButtonElement(GameData.XmlGameData.Root.Elements("CButton").Where(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == CButtonDefaultBaseId));
        }

        // <CWeapon default="1">
        private void LoadCWeaponDefault()
        {
            CWeaponElement(GameData.XmlGameData.Root.Elements("CWeapon").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
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
                    if (!int.TryParse(element.Element("Year").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day").Value, out int day))
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
    }
}
