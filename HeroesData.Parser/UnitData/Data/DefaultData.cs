using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Data
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

        public const string StormButtonParentName = "StormButtonParent";
        public const string DefaultHeroDifficulty = "Easy";

        private readonly GameData GameData;

        public DefaultData(GameData gameData)
        {
            GameData = gameData;
        }

        /// <summary>
        /// Get the default hero name text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string HeroName { get; private set; }

        /// <summary>
        /// Get the default unit name text. Contains ##id##. Use with CUnit id.
        /// </summary>
        public string UnitName { get; private set; }

        /// <summary>
        /// Gets the default description text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the default life amount.
        /// </summary>
        public double LifeMax { get; private set; }

        /// <summary>
        /// Gets the default radius value.
        /// </summary>
        public double Radius { get; private set; }

        /// <summary>
        /// Gets the default speed value.
        /// </summary>
        public double Speed { get; private set; }

        /// <summary>
        /// Gets the default sight value.
        /// </summary>
        public double Sight { get; private set; }

        /// <summary>
        /// Gets the default energy value.
        /// </summary>
        public double EnergyMax { get; private set; }

        /// <summary>
        /// Gets the default energy regeneration rate.
        /// </summary>
        public double EnergyRegenRate { get; private set; }

        /// <summary>
        /// Gets the default portrait text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string Portrait { get; private set; }

        /// <summary>
        /// Gets the default select screen button text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string SelectScreenButtonImage { get; private set; }

        /// <summary>
        /// Gets the default party panel button image text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string PartyPanelButtonImage { get; private set; }

        /// <summary>
        /// Gets the default leaderboard image text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string LeaderboardImage { get; private set; }

        /// <summary>
        /// Gets the default loading screen text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string LoadingScreenImage { get; private set; }

        /// <summary>
        /// Gets the default hyperlink id text. Used for shortname. Contains ##id##. Use with CHero id.
        /// </summary>
        public string HyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default release date value.
        /// </summary>
        public DateTime ReleaseDate { get; private set; }

        /// <summary>
        /// Gets the alpha release date of 2014/3/13.
        /// </summary>
        public DateTime AlphaReleaseDate { get; private set; } = new DateTime(2014, 3, 13);

        /// <summary>
        /// Gets the unit text. Contains ##id##. Used to get the cUnitId.
        /// </summary>
        public string Unit { get; private set; }

        /// <summary>
        /// Gets the default hero role text. Contains ##id##.
        /// </summary>
        public string HeroRoleName { get; private set; }

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
        // there are two, but second one doesn't have anything useful so get first one only
        private void LoadCUnitDefault()
        {
            XElement cUnitDefault = GameData.XmlGameData.Root.Elements("CUnit").FirstOrDefault(x => x.Attribute("default")?.Value == "1");

            UnitName = cUnitDefault.Element("Name").Attribute("value").Value;
            LifeMax = double.Parse(cUnitDefault.Element("LifeMax").Attribute("value").Value);
            Radius = double.Parse(cUnitDefault.Element("Radius").Attribute("value").Value);
        }

        // <CUnit default="1" id="StormBasicHeroicUnit">
        private void LoadCUnitDefaultStormBasicHeroicUnit()
        {
            XElement cUnitDefaultStormBasicHeroicUnit = GameData.XmlGameData.Root.Elements("CUnit").FirstOrDefault(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == "StormBasicHeroicUnit");

            Speed = double.Parse(cUnitDefaultStormBasicHeroicUnit.Element("Speed").Attribute("value").Value);
            Sight = double.Parse(cUnitDefaultStormBasicHeroicUnit.Element("Sight").Attribute("value").Value);
        }

        // <CUnit default="1" id="StormHero" parent="StormBasicHeroicUnit">
        private void LoadCUnitDefaultStormHero()
        {
            XElement cUnitDefaultStormHero = GameData.XmlGameData.Root.Elements("CUnit").FirstOrDefault(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == "StormHero");

            EnergyMax = double.Parse(cUnitDefaultStormHero.Element("EnergyMax").Attribute("value").Value);
            EnergyRegenRate = double.Parse(cUnitDefaultStormHero.Element("EnergyRegenRate").Attribute("value").Value);
        }

        // <CHero default="1">
        private void LoadCHeroDefault()
        {
            IEnumerable<XElement> cHeroDefaults = GameData.XmlGameData.Root.Elements("CHero").Where(x => x.Attribute("default")?.Value == "1");

            foreach (XElement cHeroDefault in cHeroDefaults)
            {
                foreach (XElement element in cHeroDefault.Elements())
                {
                    string elementName = element.Name.LocalName.ToUpper();

                    if (elementName == "NAME")
                    {
                        HeroName = element.Attribute("value").Value;
                    }
                    else if (elementName == "DESCRIPTION")
                    {
                        Description = element.Attribute("value").Value;
                    }
                    else if (elementName == "PORTRAIT")
                    {
                        Portrait = element.Attribute("value").Value;
                    }
                    else if (elementName == "SELECTSCREENBUTTONIMAGE")
                    {
                        SelectScreenButtonImage = element.Attribute("value").Value;
                    }
                    else if (elementName == "SCORESCREENIMAGE")
                    {
                        LeaderboardImage = element.Attribute("value").Value;
                    }
                    else if (elementName == "LOADINGSCREENIMAGE")
                    {
                        LoadingScreenImage = element.Attribute("value").Value;
                    }
                    else if (elementName == "PARTYPANELBUTTONIMAGE")
                    {
                        PartyPanelButtonImage = element.Attribute("value").Value;
                    }
                    else if (elementName == "RELEASEDATE")
                    {
                        if (!int.TryParse(element.Element("Year").Value, out int year))
                            year = 2014;

                        if (!int.TryParse(element.Element("Month").Value, out int month))
                            month = 1;

                        if (!int.TryParse(element.Element("Day").Value, out int day))
                            day = 1;

                        ReleaseDate = new DateTime(year, month, day);
                    }
                    else if (elementName == "UNIT")
                    {
                        Unit = element.Attribute("value").Value;
                    }
                    else if (elementName == "HYPERLINKID")
                    {
                        HyperlinkId = element.Attribute("value").Value;
                    }
                }
            }
        }

        // <CHeroRole default="1">
        private void LoadCHeroRoleDefault()
        {
            XElement cHeroRoleDefault = GameData.XmlGameData.Root.Elements("CHeroRole").FirstOrDefault(x => x.Attribute("default")?.Value == "1");

            HeroRoleName = cHeroRoleDefault.Element("Name").Attribute("value").Value;
        }

        // <CButton default="1">
        private void LoadCButtonDefault()
        {
            XElement cButtonDefault = GameData.XmlGameData.Root.Elements("CButton").FirstOrDefault(x => x.Attribute("default")?.Value == "1");

            ButtonName = cButtonDefault.Element("Name").Attribute("value").Value;
            ButtonTooltip = cButtonDefault.Element("Tooltip").Attribute("value").Value;
        }

        // <CButton default="1" id="StormButtonParent">
        private void LoadCButtonDefaultStormButtonParent()
        {
            XElement cButtonDefaultStormButtonParent = GameData.XmlGameData.Root.Elements("CButton").FirstOrDefault(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == StormButtonParentName);

            ButtonSimpleDisplayText = cButtonDefaultStormButtonParent.Element("SimpleDisplayText").Attribute("value").Value;

            XElement tooltipVitalNameElement = cButtonDefaultStormButtonParent.Element("TooltipVitalName");
            if (tooltipVitalNameElement.Attribute("index")?.Value == "Energy")
                ButtonTooltipEnergyVitalName = tooltipVitalNameElement.Attribute("value").Value;
        }

        // <CWeapon default="1">
        private void LoadCWeaponDefault()
        {
            XElement cWeaponDefault = GameData.XmlGameData.Root.Elements("CWeapon").FirstOrDefault(x => x.Attribute("default")?.Value == "1");

            WeaponName = cWeaponDefault.Element("Name").Attribute("value").Value;
            WeaponDisplayEffect = cWeaponDefault.Element("DisplayEffect").Attribute("value").Value;
            WeaponRange = double.Parse(cWeaponDefault.Element("Range").Attribute("value").Value);
            WeaponPeriod = double.Parse(cWeaponDefault.Element("Period").Attribute("value").Value);
        }
    }
}
