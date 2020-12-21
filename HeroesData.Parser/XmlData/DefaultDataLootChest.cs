using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataLootChest
    {
        private readonly GameData _gameData;

        public DefaultDataLootChest(GameData gameData)
        {
            _gameData = gameData;

            LoadCLootChestDefault();
        }

        /// <summary>
        /// Gets the default description text. Contains ##id##.
        /// </summary>
        public string ToolChestDescription { get; private set; } = string.Empty;

        // <CLootChest default="1">
        private void LoadCLootChestDefault()
        {
            CLootChestElement(_gameData.Elements("CLootChest").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CLootChestElement(IEnumerable<XElement> cLootChestElements)
        {
            foreach (XElement element in cLootChestElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "DESCRIPTION")
                {
                    ToolChestDescription = element.Attribute("value")?.Value ?? string.Empty;
                }
            }
        }
    }
}
