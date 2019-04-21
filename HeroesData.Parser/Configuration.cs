using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class Configuration
    {
        private readonly Dictionary<string, List<(string, string)>> PartValuesByElementName = new Dictionary<string, List<(string Part, string Value)>>();
        private readonly Dictionary<string, HashSet<string>> XmlElementNameByType = new Dictionary<string, HashSet<string>>();

        private ILookup<string, string> IdByElementName;

        public string ConfigFileName => "config.xml";

        /// <summary>
        /// Loads the configuration file.
        /// </summary>
        public void Load()
        {
            LoadConfigurationFile();
        }

        public bool ConfigFileExists()
        {
            return File.Exists(ConfigFileName);
        }

        /// <summary>
        /// Gets a collection of gamestring default values consisting of a part and value.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<(string Part, string Value)> GamestringDefaultValues(string element)
        {
            if (PartValuesByElementName.TryGetValue(element, out List<(string Part, string Value)> values))
                return values;
            else
                return new List<(string Part, string Value)>();
        }

        /// <summary>
        /// Gets a collection of elements if the element if found. Returns null if none found. Used for xml gamestring parsing.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<string> GamestringXmlElements(string element)
        {
            if (XmlElementNameByType.TryGetValue(element, out HashSet<string> elements))
                return elements;
            else
                return new List<string>();
        }

        /// <summary>
        /// Gets a collection of id values from the element name. Used for xml parsing when retrieving items.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<string> DataXmlElementIds(string element)
        {
            return IdByElementName[element];
        }

        private void LoadConfigurationFile()
        {
            XDocument doc = XDocument.Load(ConfigFileName);

            // parser helper
            foreach (XElement idElement in doc.Root.Element("ParserHelper").Elements("Id"))
            {
                string name = idElement.Attribute("name")?.Value;
                string part = idElement.Attribute("part")?.Value;
                string value = idElement.Attribute("value")?.Value;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(part) || string.IsNullOrEmpty(value))
                    continue;

                if (PartValuesByElementName.TryGetValue(name, out List<(string Part, string Value)> values))
                    values.Add((part, value));
                else
                    PartValuesByElementName.Add(name, new List<(string Part, string Value)>() { (part, value) });
            }

            // xml element lookup
            foreach (XElement typeElement in doc.Root.Element("XmlElementLookup").Elements("Type"))
            {
                string name = typeElement.Attribute("name")?.Value;
                if (string.IsNullOrEmpty(name))
                    continue;

                HashSet<string> elements = new HashSet<string>();

                foreach (XElement element in typeElement.Elements("Element"))
                {
                    if (string.IsNullOrEmpty(element.Value))
                        continue;

                    elements.Add(element.Value);
                }

                XmlElementNameByType.Add(name, elements);
            }

            // additional valid elements for xml parsing
            IdByElementName = doc.Root.Element("DataParser").Elements().ToLookup(x => x.Name.LocalName, x => x.Attribute("id")?.Value);
        }
    }
}
