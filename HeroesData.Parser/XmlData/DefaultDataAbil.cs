using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataAbil
    {
        private readonly GameData GameData;

        public DefaultDataAbil(GameData gameData)
        {
            GameData = gameData;

            LoadCAbilDefault();
        }

        /// <summary>
        /// Gets the default button name text. Contains ##id##.
        /// </summary>
        public string ButtonName { get; private set; }

        // <CAbil default="1">
        protected void LoadCAbilDefault()
        {
            CAbilElement(GameData.Elements("CAbil").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        protected void CAbilElement(IEnumerable<XElement> elements)
        {
            foreach (XElement element in elements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    ButtonName = element.Attribute("value").Value;
                }
            }
        }
    }
}
