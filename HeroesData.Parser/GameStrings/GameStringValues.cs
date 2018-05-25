using System.Collections.Generic;
using System.Xml.Linq;

namespace HeroesData.Parser.GameStrings
{
    internal class GameStringValues
    {
        private readonly string GameStringValuesXmlFile = "GameStringValues.xml";

        private GameStringValues()
        {
            Initialize();
        }

        /// <summary>
        /// Gets a lists of values for selected parts of a path.
        /// </summary>
        public List<(string Name, string PartIndex, string Value)> PartValueByPartName { get; } = new List<(string Name, string PartIndex, string Value)>();

        /// <summary>
        /// Loads the GameStringValues xml file data.
        /// </summary>
        /// <returns></returns>
        public static GameStringValues Load()
        {
            return new GameStringValues();
        }

        private void Initialize()
        {
            XDocument xDoc = XDocument.Load(GameStringValuesXmlFile);

            foreach (XElement element in xDoc.Root.Elements("Id"))
            {
                string name = element.Attribute("name")?.Value;
                string part = element.Attribute("part")?.Value;
                string value = element.Attribute("value")?.Value;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(part) || string.IsNullOrEmpty(value))
                    continue;

                PartValueByPartName.Add((name, part, value));
            }
        }
    }
}
