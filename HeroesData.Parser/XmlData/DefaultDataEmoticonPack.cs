using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataEmoticonPack
    {
        private readonly GameData GameData;

        public DefaultDataEmoticonPack(GameData gameData)
        {
            GameData = gameData;

            LoadCEmoticonPackDefault();
        }

        /// <summary>
        /// Gets the default emoticon pack name. Contains ##id##.
        /// </summary>
        public string EmoticonPackName { get; private set; }

        /// <summary>
        /// Gets the default emoticon pack name used for sorting. Contains ##id##.
        /// </summary>
        public string EmoticonPackSortName { get; private set; }

        /// <summary>
        /// Gets the default emoticon pack description. Contains ##id##.
        /// </summary>
        public string EmoticonPackDescription { get; private set; }

        /// <summary>
        /// Gets the default emoticon pack hyperlinkId. Contains ##id##.
        /// </summary>
        public string EmoticonPackHyperlinkId { get; private set; }

        // <CEmoticonPack default="1">
        private void LoadCEmoticonPackDefault()
        {
            CEmoticonPackElement(GameData.Elements("CEmoticonPack").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CEmoticonPackElement(IEnumerable<XElement> cEmoticonPackElements)
        {
            foreach (XElement element in cEmoticonPackElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    EmoticonPackName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    EmoticonPackSortName = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    EmoticonPackDescription = element.Attribute("value").Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    EmoticonPackHyperlinkId = element.Attribute("value").Value;
                }
            }
        }
    }
}
