using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultBundleData
    {
        private readonly GameData _gameData;

        public DefaultBundleData(GameData gameData)
        {
            _gameData = gameData;

            LoadCBundleDefault();
        }

        /// <summary>
        /// Gets the default bundle name. Contains ##id##.
        /// </summary>
        public string? BundleName { get; private set; }

        /// <summary>
        /// Gets the default bundle name used for sorting. Contains ##id##.
        /// </summary>
        public string? BundleSortName { get; private set; }

        /// <summary>
        /// Gets the default bundle hyperlinkId. Contains ##id##.
        /// </summary>
        public string? BundleHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default bundle release date.
        /// </summary>
        public DateTime BundleReleaseDate { get; private set; }

        // <CBundle default="1">
        private void LoadCBundleDefault()
        {
            CBundleElement(_gameData.Elements("CBundle").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CBundleElement(IEnumerable<XElement> cBannerElements)
        {
            foreach (XElement element in cBannerElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "NAME")
                {
                    BundleName = element.Attribute("value")?.Value;
                }
                else if (elementName == "SORTNAME")
                {
                    BundleSortName = element.Attribute("value")?.Value;
                }
                else if (elementName == "HyperlinkId")
                {
                    BundleHyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year")?.Attribute("value")?.Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month")?.Attribute("value")?.Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day")?.Attribute("value")?.Value, out int day))
                        day = 1;

                    BundleReleaseDate = new DateTime(year, month, day);
                }
            }
        }
    }
}
