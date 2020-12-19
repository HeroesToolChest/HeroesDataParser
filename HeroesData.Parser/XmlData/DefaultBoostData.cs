using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultBoostData
    {
        private readonly GameData _gameData;

        public DefaultBoostData(GameData gameData)
        {
            _gameData = gameData ?? throw new ArgumentNullException(nameof(gameData));

            LoadCBoostDefault();
        }

        /// <summary>
        /// Gets the default boost name. Contains ##id##.
        /// </summary>
        public string? BoostName { get; private set; }

        /// <summary>
        /// Gets the default boost name used for sorting. Contains ##id##.
        /// </summary>
        public string? BoostSortName { get; private set; }

        /// <summary>
        /// Gets the default boost hyperlinkId. Contains ##id##.
        /// </summary>
        public string? BoostHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default boost release date.
        /// </summary>
        public DateTime BoostReleaseDate { get; private set; }

        // <CBoost default="1">
        private void LoadCBoostDefault()
        {
            CBoostElement(_gameData.Elements("CBoost").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CBoostElement(IEnumerable<XElement> cBannerElements)
        {
            foreach (XElement element in cBannerElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "NAME")
                {
                    BoostName = element.Attribute("value")?.Value;
                }
                else if (elementName == "SORTNAME")
                {
                    BoostSortName = element.Attribute("value")?.Value;
                }
                else if (elementName == "HyperlinkId")
                {
                    BoostHyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year")?.Attribute("value")?.Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month")?.Attribute("value")?.Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day")?.Attribute("value")?.Value, out int day))
                        day = 1;

                    BoostReleaseDate = new DateTime(year, month, day);
                }
            }
        }
    }
}
