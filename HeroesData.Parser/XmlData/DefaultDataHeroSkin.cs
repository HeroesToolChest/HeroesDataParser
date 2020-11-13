using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataHeroSkin
    {
        private readonly GameData _gameData;

        public DefaultDataHeroSkin(GameData gameData)
        {
            _gameData = gameData;

            LoadCSkinDefault();
        }

        /// <summary>
        /// Gets the default hero skin name. Contains ##id##.
        /// </summary>
        public string? HeroSkinName { get; private set; }

        /// <summary>
        /// Gets the default hero skin name used for sorting. Contains ##id##.
        /// </summary>
        public string? HeroSkinSortName { get; private set; }

        /// <summary>
        /// Gets the default hero skin info text. Contains ##id##.
        /// </summary>
        public string? HeroSkinInfoText { get; private set; }

        /// <summary>
        /// Gets the default hero skin additional search text. Contains ##id##.
        /// </summary>
        public string? HeroSkinAdditionalSearchText { get; private set; }

        /// <summary>
        /// Gets the default hero skin hyperlinkId. Contains ##id##.
        /// </summary>
        public string? HeroSkinHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default hero skins release date.
        /// </summary>
        public DateTime HeroSkinReleaseDate { get; private set; }

        // <CSkin default="1">
        private void LoadCSkinDefault()
        {
            CSkinElement(_gameData.Elements("CSkin").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CSkinElement(IEnumerable<XElement> cSkinElements)
        {
            foreach (XElement element in cSkinElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "NAME")
                {
                    HeroSkinName = element.Attribute("value")?.Value;
                }
                else if (elementName == "SORTNAME")
                {
                    HeroSkinSortName = element.Attribute("value")?.Value;
                }
                else if (elementName == "INFOTEXT")
                {
                    HeroSkinInfoText = element.Attribute("value")?.Value;
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    HeroSkinAdditionalSearchText = element.Attribute("value")?.Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    HeroSkinHyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year")?.Attribute("value")?.Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month")?.Attribute("value")?.Value, out int month))
                        month = 3;

                    if (!int.TryParse(element.Element("Day")?.Attribute("value")?.Value, out int day))
                        day = 1;

                    HeroSkinReleaseDate = new DateTime(year, month, day);
                }
            }
        }
    }
}
