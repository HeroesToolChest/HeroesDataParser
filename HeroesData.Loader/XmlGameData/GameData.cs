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
        /// Gets a XDocument of all the combined xml game data.
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

        /// <summary>
        /// Gets all the CHero elements.
        /// </summary>
        public IEnumerable<XElement> CHeroElements { get; private set; }

        /// <summary>
        /// Gets all the CUnit elements.
        /// </summary>
        public IEnumerable<XElement> CUnitElements { get; private set; }

        /// <summary>
        /// Gets all the CButton elements.
        /// </summary>
        public IEnumerable<XElement> CButtonElements { get; private set; }

        /// <summary>
        /// Gets all the CArmor elements.
        /// </summary>
        public IEnumerable<XElement> CArmorElements { get; private set; }

        /// <summary>
        /// Gets all the CTalent elements.
        /// </summary>
        public IEnumerable<XElement> CTalentElements { get; private set; }

        /// <summary>
        /// Gets all the CValidatorCombine elements.
        /// </summary>
        public IEnumerable<XElement> CValidatorCombineElements { get; private set; }

        /// <summary>
        /// Gets all the CValidatorPlayerTalent elements.
        /// </summary>
        public IEnumerable<XElement> CValidatorPlayerTalentElements { get; private set; }

        /// <summary>
        /// Gets all the CWeaponLegacy elements.
        /// </summary>
        public IEnumerable<XElement> CWeaponLegacyElements { get; private set; }

        /// <summary>
        /// Gets all the CAnnouncerPack elements.
        /// </summary>
        public IEnumerable<XElement> CAnnouncerPackElements { get; private set; }

        /// <summary>
        /// Gets all the CBanner elements.
        /// </summary>
        public IEnumerable<XElement> CBannerElements { get; private set; }

        /// <summary>
        /// Gets all the CSkin elements.
        /// </summary>
        public IEnumerable<XElement> CSkinElements { get; private set; }

        /// <summary>
        /// Gets all the CScoreValueCustom elements.
        /// </summary>
        public IEnumerable<XElement> CScoreValueCustomElements { get; private set; }

        /// <summary>
        /// Gets all the CMount elements.
        /// </summary>
        public IEnumerable<XElement> CMountElements { get; private set; }

        /// <summary>
        /// Gets all the CSpray elements.
        /// </summary>
        public IEnumerable<XElement> CSprayElements { get; private set; }


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
        /// Merges the elements in the collection into a single XElement.  The elements get added as the first children to the first element.
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
                if ( element.HasElements)
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

        protected abstract void LoadCoreStormMod();
        protected abstract void LoadHeroesDataStormMod();
        protected abstract void LoadHeroesMapMods();
        protected abstract void LoadGameDataXmlContents(string gameDataXmlFilePath);

        protected void ParseTextFile(string filePath, bool isMapFile = false)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                if (isMapFile)
                    ReadMapFile(reader);
                else
                    ReadTextFile(reader);
            }
        }

        protected void ParseTextFile(Stream fileStream, bool isMapFile = false)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                if (isMapFile)
                    ReadMapFile(reader);
                else
                    ReadTextFile(reader);
            }
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
                string line = reader.ReadLine();

                string[] splitLine = line.Split(new char[] { '=' }, 2);
                if (splitLine.Length == 2)
                    AddGameString(splitLine[0], splitLine[1]);
            }
        }

        private void ReadMapFile(StreamReader reader)
        {
            Dictionary<string, string> mapGamestrings = new Dictionary<string, string>();
            string gamelink = string.Empty;

            // load it all up
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] splitLine = line.Split(new char[] { '=' }, 2);

                if (splitLine.Length == 2)
                {
                    if (splitLine[0].StartsWith("ScoreValue/Name/EndOfMatchAward"))
                        gamelink = splitLine[0].Split('/')[2]; // get the last part

                    mapGamestrings.Add(splitLine[0], splitLine[1]);
                }
            }

            if (!string.IsNullOrEmpty(gamelink) && mapGamestrings.TryGetValue($"{MapGameStringPrefixes.MatchAwardMapSpecificInstanceNamePrefix}[Override]Generic Instance_Award Name", out string instanceAwardName))
                AddGameString($"{MapGameStringPrefixes.MatchAwardMapSpecificInstanceNamePrefix}{gamelink}", instanceAwardName);
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
            CHeroElements = XmlGameData.Root.Elements("CHero").ToList();
            CUnitElements = XmlGameData.Root.Elements("CUnit").ToList();
            CButtonElements = XmlGameData.Root.Elements("CButton").ToList();
            CArmorElements = XmlGameData.Root.Elements("CArmor").ToList();
            CTalentElements = XmlGameData.Root.Elements("CTalent").ToList();
            CValidatorCombineElements = XmlGameData.Root.Elements("CValidatorCombine").ToList();
            CValidatorPlayerTalentElements = XmlGameData.Root.Elements("CValidatorPlayerTalent").ToList();
            CWeaponLegacyElements = XmlGameData.Root.Elements("CWeaponLegacy").ToList();
            CAnnouncerPackElements = XmlGameData.Root.Elements("CAnnouncerPack").ToList();
            CBannerElements = XmlGameData.Root.Elements("CBanner").ToList();
            CSkinElements = XmlGameData.Root.Elements("CSkin").ToList();
            CScoreValueCustomElements = XmlGameData.Root.Elements("CScoreValueCustom").ToList();
            CMountElements = XmlGameData.Root.Elements("CMount").ToList();
            CSprayElements = XmlGameData.Root.Elements("CSpray").ToList();
        }
    }
}
