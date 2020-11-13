using HeroesData.Parser.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class Configuration
    {
        private readonly string _appPath;

        private readonly Dictionary<string, List<(string, string)>> _partValuesByElementName = new Dictionary<string, List<(string Part, string Value)>>();
        private readonly Dictionary<string, HashSet<string>> _xmlElementNameByType = new Dictionary<string, HashSet<string>>();
        private readonly HashSet<string> _unitDataAbilities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _deadImageFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private ILookup<string, string?>? _addIdByElementName;
        private ILookup<string, string?>? _removeIdByElementName;

        public Configuration()
        {
            _appPath = string.Empty;
        }

        public Configuration(string appPath)
        {
            _appPath = appPath;
        }

        public string ConfigFileName => Path.Combine(_appPath, "config.xml");

        /// <summary>
        /// Gets a collection of extra unit data abilities that should be ignore when parsing unit data.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> UnitDataExtraAbilities => _unitDataAbilities;

        /// <summary>
        /// Gets a collection of image file names that do not exist.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> NonExistingImageFileNames => _deadImageFileNames;

        /// <summary>
        /// Loads the configuration file.
        /// </summary>
        public void Load()
        {
            try
            {
                LoadConfigurationFile();
            }
            catch (NullReferenceException ex)
            {
                throw new ConfigurationException("Error while loading the configuration file.", ex);
            }
        }

        public bool ConfigFileExists()
        {
            return File.Exists(ConfigFileName);
        }

        /// <summary>
        /// Returns a collection of gamestring default values consisting of a part and value.
        /// </summary>
        /// <param name="element">The element to look for.</param>
        /// <returns></returns>
        public IEnumerable<(string Part, string Value)> GamestringDefaultValues(string element)
        {
            if (_partValuesByElementName.TryGetValue(element, out List<(string Part, string Value)>? values))
                return values;
            else
                return new List<(string Part, string Value)>();
        }

        /// <summary>
        /// Returns a collection of elements if the element is found. Returns null if none found. Used for xml gamestring parsing.
        /// </summary>
        /// <param name="element">The element to look for.</param>
        /// <returns></returns>
        public IEnumerable<string> GamestringXmlElements(string element)
        {
            if (_xmlElementNameByType.TryGetValue(element, out HashSet<string>? elements))
                return elements;
            else
                return new List<string>();
        }

        /// <summary>
        /// Returns a collection of id values from the element name. Used for xml parsing when retrieving items.
        /// </summary>
        /// <param name="element">The element to look for.</param>
        /// <returns></returns>
        public IEnumerable<string> AddDataXmlElementIds(string element)
        {
            return _addIdByElementName![element]!;
        }

        /// <summary>
        /// Returns a collection of id values from the element name. Used for xml parsing when retrieving items.
        /// </summary>
        /// <param name="element">The element to look for.</param>
        /// <returns></returns>
        public IEnumerable<string> RemoveDataXmlElementIds(string element)
        {
            return _removeIdByElementName![element]!;
        }

        /// <summary>
        /// Returns true if the ability id was found.
        /// </summary>
        /// <param name="value">The ability id to look for (case insensitive).</param>
        /// <returns></returns>
        public bool ContainsIgnorableExtraAbility(string value)
        {
            return _unitDataAbilities.Contains(value);
        }

        /// <summary>
        /// Returns true if the iamge file name was found.
        /// </summary>
        /// <param name="value">The image file name to look for (case insensitive).</param>
        /// <returns></returns>
        public bool ContainsDeadImageFileName(string value)
        {
            return _deadImageFileNames.Contains(value);
        }

        private void LoadConfigurationFile()
        {
            XDocument doc = XDocument.Load(ConfigFileName);

            if (doc.Root == null)
                throw new InvalidOperationException();

            // parser helper
            foreach (XElement idElement in doc.Root.Element("ParserHelper")?.Elements("Id") ?? Enumerable.Empty<XElement>())
            {
                string? name = idElement.Attribute("name")?.Value;
                string? part = idElement.Attribute("part")?.Value;
                string? value = idElement.Attribute("value")?.Value;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(part) || string.IsNullOrEmpty(value))
                    continue;

                if (_partValuesByElementName.TryGetValue(name, out List<(string Part, string Value)>? values))
                    values.Add((part, value));
                else
                    _partValuesByElementName.Add(name, new List<(string Part, string Value)>() { (part, value) });
            }

            // xml element lookup
            foreach (XElement typeElement in doc.Root.Element("XmlElementLookup")?.Elements("Type") ?? Enumerable.Empty<XElement>())
            {
                string? name = typeElement.Attribute("name")?.Value;
                if (string.IsNullOrEmpty(name))
                    continue;

                HashSet<string> elements = new HashSet<string>();

                foreach (XElement element in typeElement.Elements("Element"))
                {
                    if (string.IsNullOrEmpty(element.Value))
                        continue;

                    elements.Add(element.Value);
                }

                _xmlElementNameByType.Add(name, elements);
            }

            // extra abilities
            foreach (XElement abilityIdElement in doc.Root.Element("UnitDataExtraAbilities")?.Elements() ?? Enumerable.Empty<XElement>())
            {
                string value = abilityIdElement.Value;
                if (!string.IsNullOrEmpty(value))
                    _unitDataAbilities.Add(value);
            }

            // dead image file names
            foreach (XElement fileNameXelement in doc.Root.Element("NonExistingImageFileNames")?.Elements() ?? Enumerable.Empty<XElement>())
            {
                string value = fileNameXelement.Value;
                if (!string.IsNullOrEmpty(value))
                    _deadImageFileNames.Add(value);
            }

            // additional valid elements for xml parsing
            _addIdByElementName = doc.Root.Element("DataParser")?.Elements().Where(x => string.IsNullOrEmpty(x.Attribute("value")?.Value) || x.Attribute("value")?.Value == "true").ToLookup(x => x.Name.LocalName, x => x.Attribute("id")?.Value);
            _removeIdByElementName = doc.Root.Element("DataParser")?.Elements().Where(x => x.Attribute("value")?.Value == "false").ToLookup(x => x.Name.LocalName, x => x.Attribute("id")?.Value);
        }
    }
}
