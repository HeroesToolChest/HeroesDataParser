using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataAnnouncer
    {
        private readonly GameData _gameData;

        public DefaultDataAnnouncer(GameData gameData)
        {
            _gameData = gameData;

            LoadCAnnouncerPackDefault();
        }

        /// <summary>
        /// Gets the default announcer name. Contains ##id##.
        /// </summary>
        public string? AnnouncerName { get; private set; }

        /// <summary>
        /// Gets the default announcer name used for sorting. Contains ##id##.
        /// </summary>
        public string? AnnouncerSortName { get; private set; }

        /// <summary>
        /// Gets the default announcer description. Contains ##id##.
        /// </summary>
        public string? AnnouncerDescription { get; private set; }

        /// <summary>
        /// Gets the default announcer release date.
        /// </summary>
        public DateTime AnnouncerReleaseDate { get; private set; }

        // <CAnnouncerPack default="1">
        private void LoadCAnnouncerPackDefault()
        {
            CAnnouncerPackElement(_gameData.Elements("CAnnouncerPack").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CAnnouncerPackElement(IEnumerable<XElement> cAnnouncerPackElements)
        {
            foreach (XElement element in cAnnouncerPackElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

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
