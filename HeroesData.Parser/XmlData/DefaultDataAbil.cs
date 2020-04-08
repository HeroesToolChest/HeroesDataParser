using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataAbil
    {
        private readonly GameData _gameData;

        public DefaultDataAbil(GameData gameData)
        {
            _gameData = gameData;

            LoadCAbilDefault();
        }

        /// <summary>
        /// Gets the default abil name text. Contains ##id##.
        /// </summary>
        public string AbilName { get; private set; } = string.Empty;

        // <CAbil default="1">
        protected void LoadCAbilDefault()
        {
            CAbilElement(_gameData.Elements("CAbil").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        protected void CAbilElement(IEnumerable<XElement> elements)
        {
            foreach (XElement element in elements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "NAME")
                {
                    AbilName = element.Attribute("value").Value;
                }
            }
        }
    }
}
