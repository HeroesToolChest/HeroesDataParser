using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataBehaviorVeterancy
    {
        private readonly GameData _gameData;

        public DefaultDataBehaviorVeterancy(GameData gameData)
        {
            _gameData = gameData;

            LoadCBehaviorVeterancyDefault();
        }

        /// <summary>
        /// Gets a value indicating whether the veterancy combines modifications.
        /// </summary>
        public bool CombineNumericModifications { get; private set; }

        /// <summary>
        /// Gets a value indicating whether gets the veterancy combines xp.
        /// </summary>
        public bool CombineXP { get; private set; }

        // <CBehaviorVeterancy default="1">
        private void LoadCBehaviorVeterancyDefault()
        {
            CBehaviorVeterancyElement(_gameData.Elements("CBehaviorVeterancy").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CBehaviorVeterancyElement(IEnumerable<XElement> cBehaviorVeterancyElements)
        {
            foreach (XElement element in cBehaviorVeterancyElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "FLAGS")
                {
                    string? indexValue = element.Attribute("index")?.Value?.ToUpperInvariant();
                    string valueValue = element.Attribute("value")?.Value ?? string.Empty;

                    if (!string.IsNullOrEmpty(indexValue))
                    {
                        if (indexValue == "COMBINENUMERICMODIFICATIONS")
                        {
                            if (valueValue == "1")
                                CombineNumericModifications = true;
                            else if (valueValue == "0")
                                CombineNumericModifications = false;
                        }
                        else if (indexValue == "COMBINEXP")
                        {
                            if (valueValue == "1")
                                CombineXP = true;
                            else if (valueValue == "0")
                                CombineXP = false;
                        }
                    }
                }
            }
        }
    }
}
