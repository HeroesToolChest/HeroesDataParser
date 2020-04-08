using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataHero : DefaultDataUnit
    {
        public const string CUnitDefaultBaseId = "StormBasicHeroicUnit";

        public DefaultDataHero(GameData gameData)
            : base(gameData)
        {
            LoadCHeroDefault();
            LoadCHeroRoleDefault();
            LoadCUnitDefault();
            LoadCUnitDefaultStormBasicHeroicUnit();
            LoadCUnitDefaultStormHero();
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
        public string? HeroName { get; private set; }

        /// <summary>
        /// Gets the default description text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string? HeroDescription { get; private set; }

        /// <summary>
        /// Gets the default portrait text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string? HeroPortrait { get; private set; }

        /// <summary>
        /// Gets the default select screen button text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string? HeroSelectScreenButtonImage { get; private set; }

        /// <summary>
        /// Gets the default party panel button image text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string? HeroPartyPanelButtonImage { get; private set; }

        /// <summary>
        /// Gets the default leaderboard image text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string? HeroLeaderboardImage { get; private set; }

        /// <summary>
        /// Gets the default loading screen text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string? HeroLoadingScreenImage { get; private set; }

        /// <summary>
        /// Gets the default party frame text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string? HeroPartyFrameImage { get; private set; }

        /// <summary>
        /// Gets the default draft screen text. Contains ##id##. Use with CHero id.
        /// </summary>
        public string? HeroDraftScreenImage { get; private set; }

        /// <summary>
        /// Gets the default hyperlink id text. Used for shortname. Contains ##id##. Use with CHero id.
        /// </summary>
        public string? HeroHyperlinkId { get; private set; }

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
        public string? HeroUnit { get; private set; }

        /// <summary>
        /// Gets the default hero role text. Contains ##id##.
        /// </summary>
        public string? HeroRoleName { get; private set; }

        /// <summary>
        /// Gets the default hero information text. Contains ##id##.
        /// </summary>
        public string? HeroInfoText { get; private set; }

        /// <summary>
        /// Gets the default hero title. Contains ##id##.
        /// </summary>
        public string? HeroTitle { get; private set; }

        /// <summary>
        /// Gets the default additional search text. Contains ##id##.
        /// </summary>
        public string? HeroAdditionalSearchText { get; private set; }

        /// <summary>
        /// Gets the default alternamte name search text. Contains ##id##.
        /// </summary>
        public string? HeroAlternateNameSearchText { get; private set; }

        // <CHero default="1">
        private void LoadCHeroDefault()
        {
            CHeroElement(GameData.Elements("CHero").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CHeroRole default="1">
        private void LoadCHeroRoleDefault()
        {
            CHeroRoleElement(GameData.Elements("CHeroRole").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CUnit default="1" id="StormBasicHeroicUnit">
        private void LoadCUnitDefaultStormBasicHeroicUnit()
        {
            CUnitElement(GameData.Elements("CUnit").Where(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == "StormBasicHeroicUnit"));
        }

        // <CUnit default="1" id="StormHero" parent="StormBasicHeroicUnit">
        private void LoadCUnitDefaultStormHero()
        {
            CUnitElement(GameData.Elements("CUnit").Where(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == CUnitDefaultBaseId));
        }

        private void CHeroElement(IEnumerable<XElement> elements)
        {
            foreach (XElement element in elements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

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
                else if (elementName == "PARTYPANELBUTTONIMAGE")
                {
                    HeroPartyPanelButtonImage = element.Attribute("value").Value;
                }
                else if (elementName == "PARTYFRAMEIMAGE")
                {
                    HeroPartyFrameImage = element.Attribute("value").Value;
                }
                else if (elementName == "LOADINGSCREENIMAGE")
                {
                    HeroLoadingScreenImage = element.Attribute("value").Value;
                }
                else if (elementName == "SCORESCREENIMAGE")
                {
                    HeroLeaderboardImage = element.Attribute("value").Value;
                }
                else if (elementName == "DRAFTSCREENPORTRAIT")
                {
                    HeroDraftScreenImage = element.Attribute("value").Value;
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

        private void CHeroRoleElement(IEnumerable<XElement> elements)
        {
            foreach (XElement element in elements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "NAME")
                {
                    HeroRoleName = element.Attribute("value").Value;
                }
            }
        }
    }
}
