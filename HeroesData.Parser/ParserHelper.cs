using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace HeroesData.Parser.GameStrings
{
    /// <summary>
    /// Used to parse xml elements that don't exist. Gives default values for missing elements and values.
    /// </summary>
    internal class ParserHelper
    {
        private readonly int? HotsBuild;

        private ParserHelper()
        {
            Initialize();
        }

        private ParserHelper(int? hotsBuild)
        {
            HotsBuild = hotsBuild;
            Initialize();
        }

        /// <summary>
        /// Gets the file name of the helper file.
        /// </summary>
        public string HelperXmlFile { get; private set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "parserhelper.xml");

        /// <summary>
        /// Gets a lists of values for selected parts of a path.
        /// </summary>
        public List<(string Name, string PartIndex, string Value)> PartValueByPartName { get; } = new List<(string Name, string PartIndex, string Value)>();

        /// <summary>
        /// Loads the helper xml file data.
        /// </summary>
        /// <returns></returns>
        public static ParserHelper Load()
        {
            return new ParserHelper();
        }

        public static ParserHelper Load(int? hotsBuild)
        {
            return new ParserHelper(hotsBuild);
        }

        private void Initialize()
        {
            XDocument xDoc = LoadGameStringFile();

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

        private XDocument LoadGameStringFile()
        {
            if (File.Exists(HelperXmlFile))
            {
                return XDocument.Load(HelperXmlFile);
            }
            else
            {
                if (HotsBuild.HasValue)
                    throw new FileNotFoundException($"File not found: {HelperXmlFile} or {Path.GetFileNameWithoutExtension(HelperXmlFile)}_{HotsBuild}.xml");
                else
                    throw new FileNotFoundException($"File not found: {HelperXmlFile}");
            }
        }
    }
}
