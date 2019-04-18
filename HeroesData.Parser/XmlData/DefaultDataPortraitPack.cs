using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataPortraitPack
    {
        private readonly GameData GameData;

        public DefaultDataPortraitPack(GameData gameData)
        {
            GameData = gameData;

            LoadCPortraitPackDefault();
        }

        /// <summary>
        /// Gets the default portrait name. Contains ##id##.
        /// </summary>
        public string PortraitName { get; private set; }

        /// <summary>
        /// Gets the default portrait name used for sorting. Contains ##id##.
        /// </summary>
        public string PortraitSortName { get; private set; }

        /// <summary>
        /// Gets the default portrait hyperlinkId. Contains ##id##.
        /// </summary>
        public string PortraitHyperlinkId { get; private set; }

        // <CPortraitPack default="1">
        private void LoadCPortraitPackDefault()
        {
            CPortraitPackElement(GameData.Elements("CPortraitPack").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CPortraitPackElement(IEnumerable<XElement> cPortraitPackElements)
        {
            foreach (XElement element in cPortraitPackElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    PortraitName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    PortraitSortName = element.Attribute("value").Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    PortraitHyperlinkId = element.Attribute("value").Value;
                }
            }
        }
    }
}
