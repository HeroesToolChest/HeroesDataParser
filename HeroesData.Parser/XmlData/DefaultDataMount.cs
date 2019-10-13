using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataMount
    {
        private readonly GameData GameData;

        public DefaultDataMount(GameData gameData)
        {
            GameData = gameData;

            LoadCMountDefault();
        }

        /// <summary>
        /// Gets the default mount name. Contains ##id##.
        /// </summary>
        public string? MountName { get; private set; }

        /// <summary>
        /// Gets the default mount name used for sorting. Contains ##id##.
        /// </summary>
        public string? MountSortName { get; private set; }

        /// <summary>
        /// Gets the default mount info text. Contains ##id##.
        /// </summary>
        public string? MountInfoText { get; private set; }

        /// <summary>
        /// Gets the default mount additional search text. Contains ##id##.
        /// </summary>
        public string? MountAdditionalSearchText { get; private set; }

        /// <summary>
        /// Gets the default mount hyperlinkId. Contains ##id##.
        /// </summary>
        public string? MountHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default mount release date.
        /// </summary>
        public DateTime MountReleaseDate { get; private set; }

        // <CMount default="1">
        private void LoadCMountDefault()
        {
            CMountElement(GameData.Elements("CMount").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
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
    }
}
