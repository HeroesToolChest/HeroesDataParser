using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace HeroesData.Parser.GameStrings
{
    internal class GameStringValues
    {
        private readonly int? HotsBuild;

        private GameStringValues()
        {
            Initialize();
        }

        private GameStringValues(int? hotsBuild)
        {
            HotsBuild = hotsBuild;
            Initialize();
        }

        /// <summary>
        /// Gets the file name of the GameStringValues file.
        /// </summary>
        public string GameStringValuesXmlFile { get; private set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "GameStringValues.xml");

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

        public static GameStringValues Load(int? hotsBuild)
        {
            return new GameStringValues(hotsBuild);
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
            if (HotsBuild.HasValue)
            {
                string file = $"{Path.GetFileNameWithoutExtension(GameStringValuesXmlFile)}_{HotsBuild}.xml";

                if (File.Exists(file))
                {
                    GameStringValuesXmlFile = file;
                    return XDocument.Load(file);
                }
            }

            // default load
            if (File.Exists(GameStringValuesXmlFile))
            {
                return XDocument.Load(GameStringValuesXmlFile);
            }
            else
            {
                if (HotsBuild.HasValue)
                    throw new FileNotFoundException($"File not found: {GameStringValuesXmlFile} or {Path.GetFileNameWithoutExtension(GameStringValuesXmlFile)}_{HotsBuild}.xml");
                else
                    throw new FileNotFoundException($"File not found: {GameStringValuesXmlFile}");
            }
        }
    }
}
