using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace HeroesData.Loader.XmlGameData
{
    public abstract class GameData
    {
        private readonly Dictionary<(string Catalog, string Entry, string Field), double> ScaleValueByLookupId = new Dictionary<(string Catalog, string Entry, string Field), double>();
        private readonly Dictionary<string, string> GameStringById = new Dictionary<string, string>();
        private ILookup<string, XElement> ElementsByElementName;

        protected GameData(string modsFolderPath)
        {
            ModsFolderPath = modsFolderPath;
        }

        protected GameData(string modsFolderPath, int? hotsBuild)
        {
            ModsFolderPath = modsFolderPath;
            HotsBuild = hotsBuild;
        }

        /// <summary>
        /// Gets the number of gamestrings.
        /// </summary>
        public int GameStringCount => GameStringById.Count;

        /// <summary>
        /// Gets the number of xml files that were added.
        /// </summary>
        public int XmlFileCount { get; protected set; } = 0;

        /// <summary>
        /// Gets the number of text files that were added.
        /// </summary>
        public int TextFileCount { get; protected set; } = 0;

        /// <summary>
        /// Gets a XDocument of all the combined xml game data. Recommended to use <see cref="Elements(string)"/> for quicker access.
        /// </summary>
        public XDocument XmlGameData { get; protected set; } = new XDocument();

        /// <summary>
        /// Gets or sets the game localization. Must be in the stormdata format.
        /// </summary>
        public string GameStringLocalization { get; set; } = "enus.stormdata";

        /// <summary>
        /// Gets the hots build number.
        /// </summary>
        public int? HotsBuild { get; }

        /// <summary>
        /// Gets all the LayoutButton elements.
        /// </summary>
        public IEnumerable<XElement> LayoutButtonElements { get; private set; }

        protected string ModsFolderPath { get; }

        protected string CoreStormModDirectoryName { get; } = "core.stormmod";
        protected string HeroesDataStormModDirectoryName { get; } = "heroesdata.stormmod";
        protected string HeroesMapModsDirectoryName { get; } = "heroesmapmods";
        protected string HeroesModsDiretoryName { get; } = "heromods";
        protected string BaseStormDataDirectoryName { get; } = "base.stormdata";
        protected string BattlegroundMapModsDirectoryName { get; } = "battlegroundmapmods";
        protected string LocalizedDataName { get; set; } = "localizeddata";

        protected string GameDataStringName { get; } = "gamedata";
        protected string HeroInteractionsStringName { get; } = "herointeractions";
        protected string ConveyorBeltsStringName { get; } = "conveyorbelts";
        protected string GameDataXmlFile { get; } = "gamedata.xml";
        protected string IncludesXmlFile { get; } = "includes.xml";
        protected string GameStringFile { get; } = "gamestrings.txt";

        protected string CoreBaseDataDirectoryPath { get; set; }
        protected string HeroesDataBaseDataDirectoryPath { get; set; }
        protected string HeroesMapModsDirectoryPath { get; set; }

        protected string CoreLocalizedDataPath { get; set; }
        protected string HeroesDataLocalizedDataPath { get; set; }

        protected bool LoadXmlFilesEnabled { get; private set; }
        protected bool LoadTextFilesOnlyEnabled { get; private set; }

        /// <summary>
        /// Load only the xml files.
        /// </summary>
        public void LoadXmlFiles()
        {
            LoadXmlFilesEnabled = true;
            LoadTextFilesOnlyEnabled = false;
            Load();
        }

        /// <summary>
        /// Load only the gamestring files.
        /// </summary>
        public void LoadGamestringFiles()
        {
            LoadTextFilesOnlyEnabled = true;
            LoadXmlFilesEnabled = false;
            Load();
        }

        /// <summary>
        /// Load both xml and gamestring files.
        /// </summary>
        public void LoadAllData()
        {
            LoadXmlFilesEnabled = true;
            LoadTextFilesOnlyEnabled = true;
            Load();
        }

        /// <summary>
        /// Gets the scale value of the given lookup id.
        /// </summary>
        /// <param name="lookupId">The lookup id.</param>
        /// <returns></returns>
        public double? GetScaleValue((string Catalog, string Entry, string Field) lookupId)
        {
            if (ScaleValueByLookupId.TryGetValue(lookupId, out double value))
                return value;
            else
                return null;
        }

        /// <summary>
        /// Gets the gamestring by the gamestring id. If not found returns null.
        /// </summary>
        /// <param name="id">The string id to look up.</param>
        /// <returns></returns>
        public string GetGameString(string id)
        {
            if (GameStringById.TryGetValue(id, out string value))
                return value;
            else
                return null;
        }

        /// <summary>
        /// Try to get the value of the gamestring id.
        /// </summary>
        /// <param name="id">The string id to look up.</param>
        /// <param name="value">The value returned.</param>
        /// <returns></returns>
        public bool TryGetGameString(string id, out string value)
        {
            if (GameStringById.TryGetValue(id, out value))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns a collection of all game string ids.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetGameStringIds()
        {
            return GameStringById.Keys.ToList();
        }

        /// <summary>
        /// Adds a gamestring. If a gamestring exists, it will be overridden.
        /// </summary>
        /// <param name="id">The id of the string.</param>
        /// <param name="value">The value of the string.</param>
        public void AddGameString(string id, string value)
        {
            GameStringById[id] = value;
        }

        /// <summary>
        /// Merges the elements in the collection into a single XElement. The elements get added as the first children to the first element.
        /// All the attributes of the elements get added to the first element (overriding existing values).
        /// </summary>
        /// <param name="elements">The collection of elements.</param>
        /// <returns></returns>
        public XElement MergeXmlElements(IEnumerable<XElement> elements)
        {
            if (elements == null)
                return null;

            XElement mergedXElement = elements.FirstOrDefault();

            foreach (XElement element in elements.Skip(1))
            {
                if (element.HasElements)
                {
                    mergedXElement.Add(element.Elements());
                }

                foreach (XAttribute attribute in element.Attributes())
                {
                    mergedXElement.SetAttributeValue(attribute.Name, attribute.Value);
                }
            }

            return mergedXElement;
        }

        /// <summary>
        /// Returns a collection of elements by the element name.
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public IEnumerable<XElement> Elements(string elementName)
        {
            return ElementsByElementName[elementName];
        }

        /// <summary>
        /// Returns a collection of elements by the element name.
        /// </summary>
        /// <param name="excludedElements">Element name to include from the collection.</param>
        /// <param name="attributeId">Value of the attribute id.</param>
        /// <returns></returns>
        public IEnumerable<XElement> ElementsIncluded(string[] elements, string attributeId = null)
        {
            List<XElement> elementList = new List<XElement>();
            foreach (IGrouping<string, XElement> item in ElementsByElementName)
            {
                if (elements.Contains(item.Key))
                {
                    if (!string.IsNullOrEmpty(attributeId))
                        elementList.AddRange(item.Where(x => x.Attribute("id")?.Value == attributeId));
                    else
                        elementList.AddRange(item);
                }
            }

            return elementList;
        }

        protected abstract void LoadCoreStormMod();
        protected abstract void LoadHeroesDataStormMod();
        protected abstract void LoadHeroesMapMods();
        protected abstract void LoadGameDataXmlContents(string gameDataXmlFilePath);

        protected void LoadTextFile(string filePath, bool isMapFile = false)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                if (isMapFile)
                    ReadMapFile(reader);
                else
                    ReadTextFile(reader);
            }
        }

        protected void LoadTextFile(Stream fileStream, bool isMapFile = false)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                if (isMapFile)
                    ReadMapFile(reader);
                else
                    ReadTextFile(reader);
            }

            TextFileCount++;
        }

        protected void LoadXmlFile(string filePath)
        {
            if (Path.GetExtension(filePath) == ".xml")
            {
                XmlGameData.Root.Add(XDocument.Load(filePath).Root.Elements());
                XmlFileCount++;
            }
        }

        protected void LoadXmlFile(Stream stream)
        {
            XmlGameData.Root.Add(XDocument.Load(stream).Root.Elements());
            XmlFileCount++;
        }

        private void Load()
        {
            CoreBaseDataDirectoryPath = Path.Combine(ModsFolderPath, CoreStormModDirectoryName, BaseStormDataDirectoryName);
            HeroesDataBaseDataDirectoryPath = Path.Combine(ModsFolderPath, HeroesDataStormModDirectoryName, BaseStormDataDirectoryName);
            HeroesMapModsDirectoryPath = Path.Combine(ModsFolderPath, HeroesMapModsDirectoryName, BattlegroundMapModsDirectoryName);

            CoreLocalizedDataPath = Path.Combine(ModsFolderPath, CoreStormModDirectoryName, GameStringLocalization, LocalizedDataName);
            HeroesDataLocalizedDataPath = Path.Combine(ModsFolderPath, HeroesDataStormModDirectoryName, GameStringLocalization, LocalizedDataName);

            LoadFiles();
            SetLevelScalingData();
            SetPredefinedElements();
        }

        private void LoadFiles()
        {
            LoadCoreStormMod(); // must come first
            LoadHeroesDataStormMod();

            LoadHeroesMapMods();
        }

        private void ReadTextFile(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                ReadOnlySpan<char> lineSpan = reader.ReadLine().AsSpan();
                int indexOfSplit = lineSpan.IndexOf('=');

                if (indexOfSplit > -1)
                    AddGameString(lineSpan.Slice(0, indexOfSplit).ToString(), lineSpan.Slice(indexOfSplit + 1).ToString());
            }
        }

        private void ReadMapFile(StreamReader reader)
        {
            Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>> mapGamestrings = new Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>>();
            ReadOnlyMemory<char> gamelink = null;

            // load it all up
            while (!reader.EndOfStream)
            {
                ReadOnlyMemory<char> line = reader.ReadLine().AsMemory();
                int indexOfSplit = line.Span.IndexOf('=');

                if (indexOfSplit > -1)
                {
                    ReadOnlyMemory<char> id = line.Slice(0, indexOfSplit);
                    if (id.Span.StartsWith("ScoreValue/Name/EndOfMatchAward"))
                        gamelink = line.Slice(id.Span.LastIndexOf('/') + 1);

                    mapGamestrings.Add(id, line.Slice(indexOfSplit + 1));
                }
            }

            if (!gamelink.IsEmpty && mapGamestrings.TryGetValue($"{MapGameStringPrefixes.MatchAwardMapSpecificInstanceNamePrefix}[Override]Generic Instance_Award Name".AsMemory(), out ReadOnlyMemory<char> instanceAwardName))
                AddGameString($"{MapGameStringPrefixes.MatchAwardMapSpecificInstanceNamePrefix}{gamelink.ToString()}", instanceAwardName.ToString());
        }

        private void SetLevelScalingData()
        {
            if (!LoadXmlFilesEnabled)
                return;

            IEnumerable<XElement> levelScalingArrays = XmlGameData.Root.Descendants("LevelScalingArray");

            foreach (XElement scalingArray in levelScalingArrays)
            {
                foreach (XElement modification in scalingArray.Elements("Modifications"))
                {
                    string catalog = modification.Element("Catalog")?.Attribute("value")?.Value;
                    string entry = modification.Element("Entry")?.Attribute("value")?.Value;
                    string field = modification.Element("Field")?.Attribute("value")?.Value;
                    string value = modification.Element("Value")?.Attribute("value")?.Value;

                    if (string.IsNullOrEmpty(value))
                        continue;

                    // add data without index
                    if (field.Contains("]"))
                        ScaleValueByLookupId[(catalog, entry, Regex.Replace(field, @"\[.*?\]", string.Empty))] = double.Parse(value);

                    ScaleValueByLookupId[(catalog, entry, field)] = double.Parse(value);
                }
            }
        }

        private void SetPredefinedElements()
        {
            LayoutButtonElements = XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("id")?.Value != "TargetHeroDummy").Elements("CardLayouts").Elements("LayoutButtons");

            ElementsByElementName = XmlGameData.Root.Elements().ToLookup(x => x.Name.LocalName, x => x);
        }
    }
}
