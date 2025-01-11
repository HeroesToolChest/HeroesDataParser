using CASCLib;
using HeroesData.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace HeroesData.Loader.XmlGameData
{
    public class CASCGameData : GameData
    {
        private readonly CASCHandler _cascHandlerData;
        private readonly CASCFolder _cascFolderData;

        /// <summary>
        /// Initializes a new instance of the <see cref="CASCGameData"/> class.
        /// </summary>
        /// <param name="cascHandler"></param>
        /// <param name="cascFolder"></param>
        /// <param name="modsFolderPath">The root folder of the heroes data path.</param>
        public CASCGameData(CASCHandler cascHandler, CASCFolder cascFolder, string modsFolderPath = "mods")
            : base(modsFolderPath)
        {
            _cascHandlerData = cascHandler;
            _cascFolderData = cascFolder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CASCGameData"/> class.
        /// </summary>
        /// <param name="cascHandler"></param>
        /// <param name="cascFolder"></param>
        /// <param name="hotsBuild">The hots build number.</param>
        /// <param name="modsFolderPath">The root folder of the heroes data path.</param>
        public CASCGameData(CASCHandler cascHandler, CASCFolder cascFolder, int? hotsBuild, string modsFolderPath = "mods")
            : base(modsFolderPath, hotsBuild)
        {
            _cascHandlerData = cascHandler;
            _cascFolderData = cascFolder;
        }

        protected override void LoadCoreStormMod()
        {
            SetCorrectFileCasing();

            if (LoadXmlFilesEnabled)
            {
                // core.stormmod xml files
                CASCFolder currentFolder = _cascFolderData.GetDirectory(Path.Combine(CoreBaseDataDirectoryPath, GameDataStringName));

                foreach (KeyValuePair<string, ICASCEntry> file in currentFolder.Entries)
                {
                    if (Path.GetExtension(file.Key) != ".xml")
                        continue;

                    string filePath = ((CASCFile)file.Value).FullName;

                    if (!_cascHandlerData.FileExists(filePath))
                        throw new FileNotFoundException(filePath);

                    using Stream stream = _cascHandlerData.OpenFile(filePath);

                    if (XmlGameData.LastNode == null)
                    {
                        XmlGameData = XDocument.Load(stream);
                        XmlFileCount++;

                        if (IsCacheEnabled)
                        {
                            AddXmlCachedFilePath(filePath);
                        }
                    }
                    else
                    {
                        LoadXmlFile(stream, filePath);
                    }
                }
            }

            if (LoadTextFilesOnlyEnabled)
            {
                string filePath = Path.Combine(CoreLocalizedDataPath, GameStringFile);
                LoadTextFile(_cascHandlerData.OpenFile(filePath), filePath);
            }

            string fontStylesFilePath = Path.Combine(CoreBaseDataDirectoryPath, UIDirectoryStringName, FontStyleFile);
            if (LoadStormStyleEnabled)
                LoadStormStyleFile(_cascHandlerData.OpenFile(fontStylesFilePath));

            if (IsCacheEnabled)
                AddStormStyleCachedFilePath(fontStylesFilePath);
        }

        protected override void LoadHeroesDataStormMod()
        {
            LoadDefaultData(HeroesDataBaseDataDirectoryPath, HeroesDataLocalizedDataPath, true);

            // load up files in includes.xml file - which are the heroes in the heromods folder
            using (Stream cascXmlStream = _cascHandlerData.OpenFile(Path.Combine(HeroesDataBaseDataDirectoryPath, IncludesXmlFile)))
            {
                if (IsCacheEnabled)
                {
                    AddXmlCachedFilePath(Path.Combine(HeroesDataBaseDataDirectoryPath, IncludesXmlFile));
                }

                XDocument gameDataXml = XDocument.Load(cascXmlStream);

                if (gameDataXml.Root == null)
                    throw new InvalidOperationException();

                IEnumerable<XElement> pathElements = gameDataXml.Root.Elements("Path");

                foreach (XElement pathElement in pathElements)
                {
                    string? valuePath = pathElement?.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(valuePath))
                    {
                        valuePath = PathHelper.GetFilePath(valuePath);

                        if (!string.IsNullOrEmpty(valuePath))
                        {
                            valuePath = valuePath.Remove(0, 5); // remove 'mods/'

                            if (valuePath.StartsWith(HeroesModsDirectoryName, StringComparison.OrdinalIgnoreCase))
                            {
                                string gameDataDirectory = Path.Join(ModsFolderPath, valuePath, BaseStormDataDirectoryName, GameDataStringName);
                                CASCFolder currentFolder = _cascFolderData.GetDirectory(gameDataDirectory);

                                foreach (KeyValuePair<string, ICASCEntry> file in currentFolder.Entries)
                                {
                                    if (!file.Key.Equals("AnnouncerPackData.xml", StringComparison.OrdinalIgnoreCase))
                                        continue;

                                    string filePath = ((CASCFile)file.Value).FullName;

                                    if (!_cascHandlerData.FileExists(filePath))
                                        throw new FileNotFoundException(filePath);

                                    using Stream stream = _cascHandlerData.OpenFile(filePath);
                                    LoadXmlFile(stream, filePath);
                                }

                                string gameDataPath = Path.Combine(ModsFolderPath, valuePath, BaseStormDataDirectoryName, GameDataXmlFile);

                                LoadGameDataXmlContents(gameDataPath);

                                string heroModsFontStylesFilePath = Path.Combine(ModsFolderPath, valuePath, BaseStormDataDirectoryName, UIDirectoryStringName, FontStyleFile);
                                if (_cascHandlerData.FileExists(heroModsFontStylesFilePath))
                                {
                                    if (LoadStormStyleEnabled)
                                        LoadStormStyleFile(_cascHandlerData.OpenFile(heroModsFontStylesFilePath));

                                    if (IsCacheEnabled)
                                        AddStormStyleCachedFilePath(heroModsFontStylesFilePath);
                                }

                                if (LoadTextFilesOnlyEnabled)
                                {
                                    string filePath = Path.Combine(ModsFolderPath, valuePath, GameStringLocalization, LocalizedDataName, GameStringFile);

                                    if (_cascHandlerData.FileExists(filePath))
                                        LoadTextFile(_cascHandlerData.OpenFile(filePath), filePath);
                                }
                            }
                        }
                    }
                }
            }

            string fontStylesFilePath = Path.Combine(HeroesDataBaseDataDirectoryPath, UIDirectoryStringName, FontStyleFile);
            if (LoadStormStyleEnabled)
                LoadStormStyleFile(_cascHandlerData.OpenFile(fontStylesFilePath));

            if (IsCacheEnabled)
                AddStormStyleCachedFilePath(fontStylesFilePath);
        }

        protected override void LoadHeroesMapMods()
        {
            CASCFolder currentFolder = _cascFolderData.GetDirectory(HeroesMapModsDirectoryPath);

            // loop through each mapmods folder
            foreach (KeyValuePair<string, ICASCEntry> mapFolder in currentFolder.Entries)
            {
                ICASCEntry baseStormDataFolder = ((CASCFolder)mapFolder.Value).GetEntry(BaseStormDataDirectoryName);
                ICASCEntry gameDataFolder = ((CASCFolder)baseStormDataFolder).GetEntry(GameDataStringName);

                if (LoadXmlFilesEnabled)
                {
                    foreach (KeyValuePair<string, ICASCEntry> dataFiles in ((CASCFolder)gameDataFolder).Entries)
                    {
                        if (Path.GetExtension(dataFiles.Key) == ".xml")
                        {
                            string filePath = ((CASCFile)dataFiles.Value).FullName;

                            if (!_cascHandlerData.FileExists(filePath))
                                throw new FileNotFoundException(filePath);

                            using Stream data = _cascHandlerData.OpenFile(filePath);

                            LoadXmlFile(mapFolder.Value.Name, data, filePath);
                        }
                    }
                }

                if (LoadTextFilesOnlyEnabled)
                {
                    string filePath = Path.Combine(HeroesMapModsDirectoryPath, mapFolder.Value.Name, GameStringLocalization, LocalizedDataName, GameStringFile);

                    if (_cascHandlerData.FileExists(filePath))
                        LoadTextFile(mapFolder.Value.Name, _cascHandlerData.OpenFile(filePath), filePath);
                }
            }
        }

        protected override void LoadGameDataXmlContents(string gameDataXmlFilePath)
        {
            if (gameDataXmlFilePath is null)
                throw new ArgumentNullException(nameof(gameDataXmlFilePath));

            if ((!_cascHandlerData.FileExists(gameDataXmlFilePath) && gameDataXmlFilePath.Contains($"{HeroInteractionsStringName}.stormmod", StringComparison.OrdinalIgnoreCase)) || !LoadXmlFilesEnabled)
                return;

            if (IsCacheEnabled)
            {
                AddXmlCachedFilePath(gameDataXmlFilePath);
            }

            // load up files in gamedata.xml file
            using Stream cascXmlStream = _cascHandlerData.OpenFile(gameDataXmlFilePath);

            XDocument gameDataXml = XDocument.Load(cascXmlStream);

            if (gameDataXml.Root == null)
                throw new InvalidOperationException();

            IEnumerable<XElement> catalogElements = gameDataXml.Root.Elements("Catalog");

            foreach (XElement catalogElement in catalogElements)
            {
                string? pathValue = catalogElement?.Attribute("path")?.Value;
                if (!string.IsNullOrEmpty(pathValue))
                {
                    pathValue = PathHelper.GetFilePath(pathValue);

                    if (pathValue!.Contains($"{GameDataStringName}{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                    {
                        if (gameDataXmlFilePath.Contains(HeroesDataStormModDirectoryName, StringComparison.OrdinalIgnoreCase))
                        {
                            string xmlFilePath = Path.Combine(HeroesDataBaseDataDirectoryPath, pathValue);
                            if (Path.GetExtension(xmlFilePath) == ".xml")
                            {
                                if (_cascHandlerData.FileExists(xmlFilePath))
                                {
                                    using Stream stream = _cascHandlerData.OpenFile(xmlFilePath);

                                    LoadXmlFile(stream, xmlFilePath);
                                }
                            }
                        }
                        else
                        {
                            string xmlFilePath = Path.Combine(gameDataXmlFilePath.Replace(GameDataXmlFile, string.Empty, StringComparison.OrdinalIgnoreCase), pathValue);

                            if (Path.GetExtension(xmlFilePath) == ".xml")
                            {
                                using Stream stream = _cascHandlerData.OpenFile(xmlFilePath);

                                LoadXmlFile(stream, xmlFilePath);
                            }
                        }
                    }
                }
            }
        }

        protected override void LoadGameplayMods()
        {
            LoadDefaultData(GameplayModsLootboxDirectoryPath, GameplayModsLocalizedDataPath, false);
        }

        private void LoadDefaultData(string stormModPath, string localizedPath, bool loadGameDataFile)
        {
            if (LoadXmlFilesEnabled)
            {
                // load up xml files in gamedata folder
                CASCFolder currentFolder = _cascFolderData.GetDirectory(Path.Combine(stormModPath, GameDataStringName));

                foreach (KeyValuePair<string, ICASCEntry> file in currentFolder.Entries)
                {
                    if (Path.GetExtension(file.Key) != ".xml")
                        continue;

                    string filePath = ((CASCFile)file.Value).FullName;

                    if (!_cascHandlerData.FileExists(filePath))
                        throw new FileNotFoundException(filePath);

                    using Stream data = _cascHandlerData.OpenFile(filePath);

                    LoadXmlFile(data, filePath);
                }

                if (loadGameDataFile)
                {
                    // load up files in gamedata.xml file
                    LoadGameDataXmlContents(Path.Combine(stormModPath, GameDataXmlFile));
                }
            }

            if (LoadTextFilesOnlyEnabled)
            {
                // load gamestring file
                string filePath = Path.Combine(localizedPath, GameStringFile);
                LoadTextFile(_cascHandlerData.OpenFile(filePath), filePath);
            }
        }

        private void SetCorrectFileCasing()
        {
            if (!_cascFolderData.DirectoryExists(CoreLocalizedDataPath))
            {
                LocalizedDataName = "LocalizedData";
                GameDataStringName = "GameData";
                UIDirectoryStringName = "UI";
                GameDataXmlFile = "GameData.xml";
                IncludesXmlFile = "Includes.xml";
                GameStringFile = "GameStrings.txt";
                FontStyleFile = "FontStyles.StormStyle";
            }
        }
    }
}
