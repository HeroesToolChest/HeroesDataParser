using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataBanner
    {
        private readonly GameData GameData;

        public DefaultDataBanner(GameData gameData)
        {
            GameData = gameData;

            LoadCBannerDefault();
        }

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

        // <CBanner default="1">
        private void LoadCBannerDefault()
        {
            CBannerElement(GameData.Elements("CBanner").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
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
    }
}
