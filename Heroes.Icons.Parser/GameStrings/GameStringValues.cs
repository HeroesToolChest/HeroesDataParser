using System.Collections.Generic;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.GameStrings
{
    public class GameStringValues
    {
        private readonly string GameStringValuesXmlFile = "GameStringValues.xml";

        public GameStringValues()
        {
            Load();
        }

        public List<(string Name, string PartIndex, string Value)> PartValueByPartName { get; set; } = new List<(string Name, string PartIndex, string Value)>();

        public static GameStringValues GetGameStringValues()
        {
            return new GameStringValues();
        }

        private void Load()
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
