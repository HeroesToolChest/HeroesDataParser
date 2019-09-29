using HeroesData.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace HeroesData.Loader.XmlGameData
{
    public abstract class GameData
    {
        private readonly HashSet<string> XmlCachedFilePathsList = new HashSet<string>();
        private readonly HashSet<string> TextCachedFilePathsList = new HashSet<string>();
        private readonly HashSet<string> StormStyleCachedFilePathList = new HashSet<string>();

        private readonly Dictionary<string, GameData> MapGameDataByMapId = new Dictionary<string, GameData>();
        private readonly Dictionary<string, string> StormStyleHexColorValueByName = new Dictionary<string, string>();

        private Dictionary<(string catalog, string entry, string field), double> ScaleValueByLookupId = new Dictionary<(string catalog, string entry, string field), double>();
        private Dictionary<string, string> GameStringById = new Dictionary<string, string>();
        private Dictionary<string, List<XElement>> XmlGameDataElementsByElementName = new Dictionary<string, List<XElement>>();
        private Dictionary<string, List<XElement>> LayoutButtonElements = new Dictionary<string, List<XElement>>();

        // temp variables used for map game data swapping
        private Dictionary<(string catalog, string entry, string field), double>? OriginalScaleValueByLookupId = new Dictionary<(string catalog, string entry, string field), double>();
        private Dictionary<string, string>? OriginalGameStringById = new Dictionary<string, string>();
        private Dictionary<string, List<XElement>>? OriginalXmlGameDataElementsByElementName = new Dictionary<string, List<XElement>>();
        private Dictionary<string, List<XElement>>? OriginalLayoutButtonElements = new Dictionary<string, List<XElement>>();

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
        /// Gets the number of specific map gamestrings.
        /// </summary>
        public int GameStringMapCount => MapGameDataByMapId.Values.Sum(x => x.GameStringCount);

        /// <summary>
        /// Gets the number of xml files that were added.
        /// </summary>
        public int XmlFileCount { get; protected set; } = 0;

        /// <summary>
        /// Gets the number of text files that were added.
        /// </summary>
        public int TextFileCount { get; protected set; } = 0;

        /// <summary>
        /// Gets the number of storm style files that were added.
        /// </summary>
        public int StormStyleCount { get; protected set; } = 0;

        /// <summary>
        /// Gets a XDocument of the xml game data. Recommended to use <see cref="Elements(string)"/> for quicker access.
        /// </summary>
        public XDocument XmlGameData { get; protected set; } = new XDocument();

        /// <summary>
        /// Gets a XDocument of the xml storm style data.
        /// </summary>
        public XDocument XmlStormStyleData { get; protected set; } = new XDocument();

        /// <summary>
        /// Gets or sets the game localization. Must be in the stormdata format.
        /// </summary>
        public string GameStringLocalization { get; set; } = "enus.stormdata";

        /// <summary>
        /// Gets the hots build number.
        /// </summary>
        public int? HotsBuild { get; }

        /// <summary>
        /// Gets or sets the value of the cache.
        /// </summary>
        public bool IsCacheEnabled { get; set; } = false;

        /// <summary>
        /// Gets the cached xml file paths.
        /// </summary>
        public IEnumerable<string> XmlCachedFilePaths => XmlCachedFilePathsList;

        /// <summary>
        /// Gets the cached text file paths.
        /// </summary>
        public IEnumerable<string> TextCachedFilePaths => TextCachedFilePathsList;

        /// <summary>
        /// Gets the cached storm style file paths.
        /// </summary>
        public IEnumerable<string> StormStyleCachedFilePath => StormStyleCachedFilePathList;

        /// <summary>
        /// Gets the amount of xml cached file paths.
        /// </summary>
        public int XmlCachedFilePathCount => XmlCachedFilePathsList.Count;

        /// <summary>
        /// Gets the amount of text cached file paths.
        /// </summary>
        public int TextCachedFilePathCount => TextCachedFilePathsList.Count;

        /// <summary>
        /// Gets the amount of storm stye file paths.
        /// </summary>
        public int StormStyleCachedFilePathCount => StormStyleCachedFilePathList.Count;

        /// <summary>
        /// Gets a collection of all game string ids.
        /// </summary>
        public IList<string> GameStringIds => GameStringById.Keys.ToList();

        /// <summary>
        /// Gets a collection of all map ids.
        /// </summary>
        public IList<string> MapIds => MapGameDataByMapId.Keys.ToList();

        /// <summary>
        /// Gets a collection of all the storm style names.
        /// </summary>
        public IList<string> StormStyleNames => StormStyleHexColorValueByName.Keys.ToList();

        /// <summary>
        /// Gets the value of the current state of the game data.
        /// </summary>
        public bool IsMapGameData { get; private set; } = false;

        protected string ModsFolderPath { get; }

        protected string CoreStormModDirectoryName { get; } = "core.stormmod";
        protected string HeroesDataStormModDirectoryName { get; } = "heroesdata.stormmod";
        protected string HeroesMapModsDirectoryName { get; } = "heroesmapmods";
        protected string HeroesModsDirectoryName { get; } = "heromods";
        protected string BaseStormDataDirectoryName { get; } = "base.stormdata";
        protected string BattlegroundMapModsDirectoryName { get; } = "battlegroundmapmods";
        protected string LocalizedDataName { get; set; } = "localizeddata";

        protected string GameDataStringName { get; set; } = "gamedata";
        protected string UIDirectoryStringName { get; set; } = "ui";
        protected string HeroInteractionsStringName { get; } = "herointeractions";
        protected string ConveyorBeltsStringName { get; } = "conveyorbelts";
        protected string GameDataXmlFile { get; set; } = "gamedata.xml";
        protected string IncludesXmlFile { get; set; } = "includes.xml";
        protected string GameStringFile { get; set; } = "gamestrings.txt";
        protected string FontStyleFile { get; set; } = "fontstyles.stormstyle";

        protected string CoreBaseDataDirectoryPath { get; set; } = string.Empty;
        protected string HeroesDataBaseDataDirectoryPath { get; set; } = string.Empty;
        protected string HeroesMapModsDirectoryPath { get; set; } = string.Empty;

        protected string CoreLocalizedDataPath { get; set; } = string.Empty;
        protected string HeroesDataLocalizedDataPath { get; set; } = string.Empty;

        protected bool LoadXmlFilesEnabled { get; private set; } = false;
        protected bool LoadTextFilesOnlyEnabled { get; private set; } = false;

        /// <summary>
        /// Merges the elements in the collection into a single XElement.
        /// All the attributes of the elements get added to the first element (overriding existing values).
        /// </summary>
        /// <param name="elements">The collection of elements.</param>
        /// <param name="append">Add elements as the last children to the first element.</param>
        /// <returns></returns>
        /// <remarks>
        /// When append is true, the returned XElement object should be looped through, this allows the later items
        /// in the collection to override the earlier items.
        /// When append is false, the Element() method should be used on the returned XElement object instead. This allows the
        /// newer items to be used while ignore the older items.</remarks>
        public static XElement? MergeXmlElements(IEnumerable<XElement> elements, bool append = true)
        {
            if (elements == null || !elements.Any())
                return null;

            if (!append)
                elements = elements.Reverse();

            XElement mergedXElement = new XElement(elements.FirstOrDefault());

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
        /// Merges the elements in the collection into a single XElement.
        /// Returned element has the attributes of the first element.
        /// </summary>
        /// <param name="elements">The collection of elements.</param>
        /// <param name="append">Add elements as the last children to the first element.</param>
        /// <returns></returns>
        public static XElement? MergeXmlElementsNoAttributes(IEnumerable<XElement> elements, bool append = true)
        {
            if (elements == null || !elements.Any())
                return null;

            if (!append)
                elements = elements.Reverse();

            XElement mergedXElement = new XElement(elements.FirstOrDefault());

            foreach (XElement element in elements.Skip(1))
            {
                if (element.HasElements)
                {
                    mergedXElement.Add(element.Elements());
                }
            }

            return mergedXElement;
        }

        /// <summary>
        /// Appends gamedata to the existing gamedata.
        /// </summary>
        /// <param name="gameData"></param>
        /// <returns></returns>
        public void AppendGameData(GameData gameData)
        {
            if (gameData == null)
            {
                throw new ArgumentNullException(nameof(gameData));
            }

            IsMapGameData = true;

            // make a temp copy so we can restore later
            OriginalGameStringById = new Dictionary<string, string>(GameStringById);
            OriginalScaleValueByLookupId = new Dictionary<(string catalog, string entry, string field), double>(ScaleValueByLookupId);
            OriginalXmlGameDataElementsByElementName = XmlGameDataElementsByElementName.ToDictionary(x => x.Key, x => x.Value.ToList());
            OriginalLayoutButtonElements = LayoutButtonElements.ToDictionary(x => x.Key, x => x.Value.ToList());

            // appending all map data together
            XmlGameData.Root.Add(gameData.XmlGameData.Root.Elements());

            foreach (KeyValuePair<string, string> gamestrings in gameData.GameStringById)
                GameStringById[gamestrings.Key] = gamestrings.Value;

            foreach (KeyValuePair<(string catalog, string entry, string field), double> item in gameData.ScaleValueByLookupId)
                ScaleValueByLookupId[item.Key] = item.Value;

            foreach (KeyValuePair<string, List<XElement>> item in gameData.LayoutButtonElements)
            {
                if (LayoutButtonElements.TryGetValue(item.Key, out List<XElement>? elementList))
                    elementList.AddRange(item.Value);
                else
                    LayoutButtonElements.Add(item.Key, item.Value);
            }

            foreach (KeyValuePair<string, List<XElement>> item in gameData.XmlGameDataElementsByElementName)
            {
                if (XmlGameDataElementsByElementName.TryGetValue(item.Key, out List<XElement>? elementList))
                    elementList.AddRange(item.Value);
                else
                    XmlGameDataElementsByElementName.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Restores the game data back to the original.
        /// </summary>
        public void RestoreGameData()
        {
            if (!IsMapGameData)
                return;

            ScaleValueByLookupId = OriginalScaleValueByLookupId!;
            GameStringById = OriginalGameStringById!;
            XmlGameDataElementsByElementName = OriginalXmlGameDataElementsByElementName!;
            LayoutButtonElements = OriginalLayoutButtonElements!;

            OriginalGameStringById = null;
            OriginalScaleValueByLookupId = null;
            OriginalXmlGameDataElementsByElementName = null;
            OriginalLayoutButtonElements = null;

            IsMapGameData = false;
        }

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
        public double? GetScaleValue((string catalog, string entry, string field) lookupId)
        {
            if (ScaleValueByLookupId.TryGetValue(lookupId, out double value))
                return value;
            else
                return null;
        }

        /// <summary>
        /// Gets the value by the gamestring id. If not found returns an empty string.
        /// </summary>
        /// <param name="id">The string id to look up.</param>
        /// <returns></returns>
        public string GetGameString(string? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (GameStringById.TryGetValue(id, out string? value))
                return value;
            else
                return string.Empty;
        }

        /// <summary>
        /// Try to get the value of the gamestring id.
        /// </summary>
        /// <param name="id">The string id to look up.</param>
        /// <param name="value">The value returned.</param>
        /// <returns></returns>
        public bool TryGetGameString(string id, [NotNullWhen(true)] out string? value)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (GameStringById.TryGetValue(id, out value))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Adds a gamestring. If a gamestring exists, it will be overridden.
        /// </summary>
        /// <param name="id">The id of the string.</param>
        /// <param name="value">The value of the string.</param>
        public void AddGameString(string id, string value)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Argument should not be empty or null", nameof(id));

            GameStringById[id] = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Adds a map gamestring. If a gamestring exists, it will be overridden.
        /// </summary>
        /// <param name="mapId">The map id that pertains to the gamestring.</param>
        /// <param name="id">The id of the string.</param>
        /// <param name="value">The value of the string.</param>
        public void AddMapGameString(string mapId, string id, string value)
        {
            if (string.IsNullOrEmpty(mapId))
            {
                throw new ArgumentException("Argument should not be empty or null", nameof(mapId));
            }

            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (MapGameDataByMapId.TryGetValue(mapId, out GameData? mapGameData))
                mapGameData.GameStringById[id] = value;
            else
                MapGameDataByMapId[mapId].GameStringById.Add(id, value);
        }

        /// <summary>
        /// Returns map game data based on the map id.
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public GameData GetMapGameData(string mapId)
        {
            if (string.IsNullOrEmpty(mapId))
            {
                throw new ArgumentException("Argument should not be empty or null", nameof(mapId));
            }

            if (MapGameDataByMapId.TryGetValue(mapId, out GameData? mapGameData))
                return mapGameData;

            throw new KeyNotFoundException($"MapId not found: {mapId}");
        }

        /// <summary>
        /// Returns a collection of all layout button elements.
        /// </summary>
        /// <returns></returns>
        public ICollection<XElement> GetLayoutButtonElements()
        {
            return LayoutButtonElements.Values.SelectMany(x => x).Distinct().ToList();
        }

        /// <summary>
        /// Gets a collection of card layout buttons from a unit id.
        /// </summary>
        /// <param name="unitId">The id of the unit.</param>
        /// <returns></returns>
        public ICollection<XElement> GetLayoutButtonElements(string unitId)
        {
            if (string.IsNullOrEmpty(unitId))
            {
                throw new ArgumentException("Argument should not be empty or null", nameof(unitId));
            }

            if (LayoutButtonElements.TryGetValue(unitId, out List<XElement>? value))
                return value;
            else
                return new List<XElement>();
        }

        /// <summary>
        /// Trys to get a collection card layout buttons from a unit id.
        /// </summary>
        /// <param name="unitId">The id of the unit.</param>
        /// <param name="value">The returned collection of card layout buttons.</param>
        /// <returns></returns>
        public bool TryGetLayoutButtonElements(string unitId, out List<XElement>? value)
        {
            if (string.IsNullOrEmpty(unitId))
            {
                throw new ArgumentException("Argument should not be empty or null", nameof(unitId));
            }

            if (LayoutButtonElements.TryGetValue(unitId, out value))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns a collection of elements by the element name. If not found, returns an empty collection.
        /// </summary>
        /// <param name="elementName">The element to look up.</param>
        /// <returns></returns>
        public IEnumerable<XElement> Elements(string elementName)
        {
            if (string.IsNullOrEmpty(elementName))
            {
                throw new ArgumentException("Argument should not be empty or null", nameof(elementName));
            }

            if (XmlGameDataElementsByElementName.TryGetValue(elementName, out List<XElement>? values))
                return values;

            return new List<XElement>();
        }

        /// <summary>
        /// Returns a collection of elements by the element name. If not found, returns an empty collection.
        /// </summary>
        /// <param name="elementName">The element to look up.</param>
        /// <param name="mapNameId">The map name id.</param>
        /// <returns></returns>
        public IEnumerable<XElement> Elements(string elementName, string mapNameId)
        {
            if (string.IsNullOrEmpty(elementName))
            {
                throw new ArgumentException("Argument should not be empty or null", nameof(elementName));
            }

            List<XElement> values = new List<XElement>();

            if (!string.IsNullOrEmpty(mapNameId))
            {
                GameData mapGameData = GetMapGameData(mapNameId);
                if (mapGameData.XmlGameDataElementsByElementName.TryGetValue(elementName, out List<XElement>? mapValues))
                    values.AddRange(mapValues);
            }

            if (XmlGameDataElementsByElementName.TryGetValue(elementName, out List<XElement>? genericValues))
                values.AddRange(genericValues);

            return values;
        }

        /// <summary>
        /// Returns a collection of elements by the element name.
        /// </summary>
        /// <param name="elements">A collection of element names to include from the collection.</param>
        /// <param name="attributeIdValue">The value of the attribute id.</param>
        /// <returns></returns>
        public IEnumerable<XElement> ElementsIncluded(IEnumerable<string> elements, string? attributeIdValue = null)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            List<string> elementsList = elements.ToList();
            List<XElement> foundElementList = new List<XElement>();

            foreach (KeyValuePair<string, List<XElement>> item in XmlGameDataElementsByElementName)
            {
                if (elementsList.Contains(item.Key))
                {
                    if (!string.IsNullOrEmpty(attributeIdValue))
                        foundElementList.AddRange(item.Value.Where(x => x.Attribute("id")?.Value == attributeIdValue));
                    else
                        foundElementList.AddRange(item.Value);
                }
            }

            return foundElementList;
        }

        /// <summary>
        /// Does a lookup up for a const value if one exists otherwise returns the passed value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetValueFromAttribute(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.StartsWith("$"))
            {
                XElement constElement = XmlGameData.Root.Elements("const").Where(x => x.Attribute("id")?.Value == value).FirstOrDefault();
                if (constElement != null)
                {
                    string? attributeValue = constElement.Attribute("value")?.Value;
                    string? isExpression = constElement.Attribute("evaluateAsExpression")?.Value;

                    if (!string.IsNullOrEmpty(attributeValue) && !string.IsNullOrEmpty(isExpression) && isExpression == "1")
                    {
                        ReadOnlySpan<char> attributeValueSpan = attributeValue.AsSpan().Trim();
                        char mathOperator = attributeValueSpan[0];

                        ReadOnlySpan<char> variables = attributeValueSpan.Slice(1).TrimStart('(').TrimEnd(')');
                        int indexOfSplit = variables.IndexOf(' ');

                        return HeroesMathEval.CalculatePathEquation($"{GetValueFromAttribute(variables.Slice(0, indexOfSplit).ToString())}{mathOperator}{GetValueFromAttribute(variables.Slice(indexOfSplit + 1).ToString())}").ToString();
                    }

                    return attributeValue ?? string.Empty;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the hex value from the name. Value is NOT prefixed with '#'.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetStormStyleHexValueFromName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            if (StormStyleHexColorValueByName.TryGetValue(name.TrimStart('#'), out string? value))
                return value;
            else
                return string.Empty;
        }

        public bool AddXmlCachedFilePath(string filePath)
        {
            return XmlCachedFilePathsList.Add(filePath);
        }

        public bool AddTextCachedFilePath(string filePath)
        {
            return TextCachedFilePathsList.Add(filePath);
        }

        public bool AddStormStyleCachedFilePath(string filePath)
        {
            return StormStyleCachedFilePathList.Add(filePath);
        }

        protected abstract void LoadCoreStormMod();
        protected abstract void LoadHeroesDataStormMod();
        protected abstract void LoadHeroesMapMods();
        protected abstract void LoadGameDataXmlContents(string gameDataXmlFilePath);

        protected void LoadTextFile(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                ReadTextFile(reader);
            }

            TextFileCount++;
        }

        protected void LoadTextFile(Stream fileStream, string filePath)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                ReadTextFile(reader);

                if (IsCacheEnabled)
                {
                    AddTextCachedFilePath(filePath);
                }
            }

            TextFileCount++;
        }

        protected void LoadTextFile(string mapName, string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                ReadMapTextFile(mapName, reader);
            }

            TextFileCount++;
        }

        protected void LoadTextFile(string mapName, Stream fileStream, string filePath)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                ReadMapTextFile(mapName, reader);

                if (IsCacheEnabled)
                {
                    AddTextCachedFilePath(filePath);
                }
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

        protected void LoadXmlFile(Stream stream, string filePath)
        {
            XDocument document = XDocument.Load(stream);
            XmlGameData.Root.Add(document.Root.Elements());
            XmlFileCount++;

            if (IsCacheEnabled)
            {
                AddXmlCachedFilePath(filePath);
            }
        }

        protected void LoadXmlFile(string id, string filePath)
        {
            if (Path.GetExtension(filePath) == ".xml")
            {
                if (MapGameDataByMapId.TryGetValue(id, out GameData? mapGameData))
                {
                    mapGameData.XmlGameData.Root.Add(XDocument.Load(filePath).Root.Elements());
                }
                else
                {
                    MapGameDataByMapId.Add(id, new FileGameData(ModsFolderPath, HotsBuild)
                    {
                        XmlGameData = XDocument.Load(filePath),
                        LoadXmlFilesEnabled = LoadXmlFilesEnabled,
                        LoadTextFilesOnlyEnabled = LoadTextFilesOnlyEnabled,
                    });
                }

                XmlFileCount++;
            }
        }

        protected void LoadXmlFile(string id, Stream stream, string filePath)
        {
            if (MapGameDataByMapId.TryGetValue(id, out GameData? mapGameData))
            {
                mapGameData.XmlGameData.Root.Add(XDocument.Load(stream).Root.Elements());
            }
            else
            {
                MapGameDataByMapId.Add(id, new FileGameData(ModsFolderPath, HotsBuild)
                {
                    XmlGameData = XDocument.Load(stream),
                    LoadXmlFilesEnabled = LoadXmlFilesEnabled,
                    LoadTextFilesOnlyEnabled = LoadTextFilesOnlyEnabled,
                });
            }

            XmlFileCount++;

            if (IsCacheEnabled)
            {
                AddXmlCachedFilePath(filePath);
            }
        }

        protected void LoadStormStyleFile(string filePath)
        {
            if (Path.GetExtension(filePath).Equals(".stormstyle", StringComparison.OrdinalIgnoreCase))
            {
                if (XmlStormStyleData.LastNode == null)
                    XmlStormStyleData = XDocument.Load(filePath);
                else
                    XmlStormStyleData.Root.Add(XDocument.Load(filePath).Root.Elements());

                StormStyleCount++;

                if (IsCacheEnabled)
                {
                    AddStormStyleCachedFilePath(filePath);
                }
            }
        }

        protected void LoadStormStyleFile(Stream stream)
        {
            XDocument document = XDocument.Load(stream);

            if (XmlStormStyleData.LastNode == null)
                XmlStormStyleData = document;
            else
                XmlStormStyleData.Root.Add(document.Root.Elements());

            StormStyleCount++;
        }

        private void Load()
        {
            CoreBaseDataDirectoryPath = Path.Combine(ModsFolderPath, CoreStormModDirectoryName, BaseStormDataDirectoryName);
            HeroesDataBaseDataDirectoryPath = Path.Combine(ModsFolderPath, HeroesDataStormModDirectoryName, BaseStormDataDirectoryName);
            HeroesMapModsDirectoryPath = Path.Combine(ModsFolderPath, HeroesMapModsDirectoryName, BattlegroundMapModsDirectoryName);

            CoreLocalizedDataPath = Path.Combine(ModsFolderPath, CoreStormModDirectoryName, GameStringLocalization, LocalizedDataName);
            HeroesDataLocalizedDataPath = Path.Combine(ModsFolderPath, HeroesDataStormModDirectoryName, GameStringLocalization, LocalizedDataName);

            LoadFiles();

            if (LoadXmlFilesEnabled)
            {
                SetLevelScalingData();
                SetPredefinedElements();

                foreach (GameData mapGameData in MapGameDataByMapId.Values)
                {
                    mapGameData.SetLevelScalingData();
                    mapGameData.SetPredefinedElements();
                }
            }

            SetFontStyles();
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

        private void ReadMapTextFile(string mapName, StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                ReadOnlySpan<char> lineSpan = reader.ReadLine().AsSpan();
                int indexOfSplit = lineSpan.IndexOf('=');

                if (indexOfSplit > -1)
                    AddMapGameString(mapName, lineSpan.Slice(0, indexOfSplit).ToString(), lineSpan.Slice(indexOfSplit + 1).ToString());
            }
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
                    string? catalog = modification.Element("Catalog")?.Attribute("value")?.Value;
                    string? entry = modification.Element("Entry")?.Attribute("value")?.Value;
                    string? field = modification.Element("Field")?.Attribute("value")?.Value;
                    string? value = modification.Element("Value")?.Attribute("value")?.Value;

                    if (string.IsNullOrEmpty(value) || catalog == null || entry == null || field == null)
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
            foreach (XElement element in XmlGameData.Root.Elements())
            {
                if (XmlGameDataElementsByElementName.TryGetValue(element.Name.LocalName, out List<XElement>? values))
                    values.Add(element);
                else
                    XmlGameDataElementsByElementName.Add(element.Name.LocalName, new List<XElement>() { element });
            }

            IEnumerable<XElement> units = Elements("CUnit").Where(x => !string.IsNullOrEmpty(x.Attribute("id")?.Value) && x.Attribute("id")?.Value != "TargetHeroDummy");
            foreach (XElement unit in units)
            {
                string id = unit.Attribute("id").Value;
                IEnumerable<XElement> cardLayouts = unit.Elements("CardLayouts");
                if (cardLayouts != null)
                {
                    if (LayoutButtonElements.TryGetValue(id, out List<XElement>? values))
                        values.AddRange(cardLayouts.Elements("LayoutButtons").ToList());
                    else
                        LayoutButtonElements.Add(unit.Attribute("id").Value, cardLayouts.Elements("LayoutButtons").ToList());
                }
            }
        }

        private void SetFontStyles()
        {
            foreach (XElement element in XmlStormStyleData.Root.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "CONSTANT")
                {
                    string? name = element.Attribute("name")?.Value;
                    string? val = element.Attribute("val")?.Value;

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(val))
                    {
                        StormStyleHexColorValueByName[name] = val;
                    }
                }
                else if (elementName == "STYLE")
                {
                    string? name = element.Attribute("name")?.Value;
                    string? textColor = element.Attribute("textcolor")?.Value;

                    if (!string.IsNullOrEmpty(textColor) && !string.IsNullOrEmpty(name))
                    {
                        if (textColor[0] == '#') // variable
                        {
                            if (StormStyleHexColorValueByName.TryGetValue(textColor.TrimStart('#'), out string? hexValue))
                            {
                                StormStyleHexColorValueByName.TryAdd(name, hexValue);
                            }
                        }
                        else if (!textColor.Contains(','))
                        {
                            StormStyleHexColorValueByName.TryAdd(name, textColor);
                        }
                    }
                }
            }
        }
    }
}
