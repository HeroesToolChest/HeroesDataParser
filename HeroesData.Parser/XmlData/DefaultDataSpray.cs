using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataSpray
    {
        private readonly GameData _gameData;

        public DefaultDataSpray(GameData gameData)
        {
            _gameData = gameData;

            LoadCSprayDefault();
        }

        /// <summary>
        /// Gets the default spray name. Contains ##id##.
        /// </summary>
        public string? SprayName { get; private set; }

        /// <summary>
        /// Gets the default spray name used for sorting. Contains ##id##.
        /// </summary>
        public string? SpraySortName { get; private set; }

        /// <summary>
        /// Gets the default spray description. Contains ##id##.
        /// </summary>
        public string? SprayDescription { get; private set; }

        /// <summary>
        /// Gets the default spray additional search text. Contains ##id##.
        /// </summary>
        public string? SprayAdditionalSearchText { get; private set; }

        /// <summary>
        /// Gets the default spray hyperlinkId. Contains ##id##.
        /// </summary>
        public string? SprayHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default spray release date.
        /// </summary>
        public DateTime SprayReleaseDate { get; private set; }

        /// <summary>
        /// Gets the default spray animation count.
        /// </summary>
        public int SprayAnimationCount { get; private set; }

        /// <summary>
        /// Gets the default spray animation duration.
        /// </summary>
        public int SprayAnimationDuration { get; private set; }

        // <CSpray default="1">
        private void LoadCSprayDefault()
        {
            CSprayElement(_gameData.Elements("CSpray").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CSprayElement(IEnumerable<XElement> cSprayElements)
        {
            foreach (XElement element in cSprayElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

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
                else if (elementName == "ANIMCOUNT")
                {
                    SprayAnimationCount = int.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ANIMDURATION")
                {
                    SprayAnimationDuration = int.Parse(element.Attribute("value").Value);
                }
            }
        }
    }
}
