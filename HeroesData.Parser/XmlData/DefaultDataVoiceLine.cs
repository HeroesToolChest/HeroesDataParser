using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataVoiceLine
    {
        private readonly GameData GameData;

        public DefaultDataVoiceLine(GameData gameData)
        {
            GameData = gameData;

            LoadCVoiceLineDefault();
        }

        /// <summary>
        /// Gets the default voice line name. Contains ##id##.
        /// </summary>
        public string VoiceLineName { get; private set; }

        /// <summary>
        /// Gets the default voice line name used for sorting. Contains ##id##.
        /// </summary>
        public string VoiceLineSortName { get; private set; }

        /// <summary>
        /// Gets the default voice line description. Contains ##id##.
        /// </summary>
        public string VoiceLineDescription { get; private set; }

        /// <summary>
        /// Gets the default voice line attribute id.
        /// </summary>
        public string VoiceLineAttributeId { get; private set; }

        /// <summary>
        /// Gets the default voice line release date.
        /// </summary>
        public DateTime VoiceLineReleaseDate { get; private set; }

        // <CVoiceLine default="1">
        private void LoadCVoiceLineDefault()
        {
            CVoiceLineElement(GameData.Elements("CVoiceLine").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CVoiceLineElement(IEnumerable<XElement> cVoiceLineElements)
        {
            foreach (XElement element in cVoiceLineElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    VoiceLineName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    VoiceLineSortName = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    VoiceLineDescription = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    VoiceLineReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "ATTRIBUTEID")
                {
                    VoiceLineAttributeId = element.Attribute("value").Value;
                }
            }
        }
    }
}
