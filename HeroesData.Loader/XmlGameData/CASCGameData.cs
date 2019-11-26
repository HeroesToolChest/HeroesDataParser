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
        private readonly CASCHandler CASCHandlerData;
        private readonly CASCFolder CASCFolderData;

        /// <summary>
        /// Contruct the CASCGameData object.
        /// </summary>
        /// <param name="cascHandler"></param>
        /// <param name="cascFolder"></param>
        /// <param name="modsFolderPath">The root folder of the heroes data path.</param>
        public CASCGameData(CASCHandler cascHandler, CASCFolder cascFolder, string modsFolderPath = "mods")
            : base(modsFolderPath)
        {
            CASCHandlerData = cascHandler;
            CASCFolderData = cascFolder;
        }

        /// <summary>
        /// Contruct the CASCGameData object.
        /// </summary>
        /// <param name="cascHandler"></param>
        /// <param name="cascFolder"></param>
        /// <param name="hotsBuild">The hots build number.</param>
        /// <param name="modsFolderPath">The root folder of the heroes data path.</param>
        public CASCGameData(CASCHandler cascHandler, CASCFolder cascFolder, int? hotsBuild, string modsFolderPath = "mods")
            : base(modsFolderPath, hotsBuild)
        {
            CASCHandlerData = cascHandler;
            CASCFolderData = cascFolder;
        }

        protected override void LoadCoreStormMod()
        {
            SetCorrectFileCasing();

            if (LoadXmlFilesEnabled)
            {
                // core.stormmod xml files
                CASCFolder currentFolder = CASCFolderData.GetDirectory(Path.Combine(CoreBaseDataDirectoryPath, GameDataStringName));

                foreach (KeyValuePair<string, ICASCEntry> file in currentFolder.Entries)
                {
                    if (Path.GetExtension(file.Key) != ".xml")
                        continue;

                    string filePath = ((CASCFile)file.Value).FullName;

                    if (!CASCHandlerData.FileExists(filePath))
                        throw new FileNotFoundException(filePath);

                    using Stream stream = CASCHandlerData.OpenFile(filePath);

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
                LoadTextFile(CASCHandlerData.OpenFile(filePath), filePath);
            }

            string fontStylesFilePath = Path.Combine(CoreBaseDataDirectoryPath, UIDirectoryStringName, FontStyleFile);
            LoadStormStyleFile(CASCHandlerData.OpenFile(fontStylesFilePath));

            if (IsCacheEnabled)
                AddStormStyleCachedFilePath(fontStylesFilePath);
        }

        protected override void LoadHeroesDataStormMod()
        {
            if (LoadXmlFilesEnabled)
            {
                // load up xml files in gamedata folder
                CASCFolder currentFolder = CASCFolderData.GetDirectory(Path.Combine(HeroesDataBaseDataDirectoryPath, GameDataStringName));

                foreach (KeyValuePair<string, ICASCEntry> file in currentFolder.Entries)
                {
                    if (Path.GetExtension(file.Key) != ".xml")
                        continue;

                    string filePath = ((CASCFile)file.Value).FullName;

                    if (!CASCHandlerData.FileExists(filePath))
                        throw new FileNotFoundException(filePath);

                    using Stream data = CASCHandlerData.OpenFile(filePath);

                    LoadXmlFile(data, filePath);
                }

                // load up files in gamedata.xml file
                LoadGameDataXmlContents(Path.Combine(HeroesDataBaseDataDirectoryPath, GameDataXmlFile));
            }

            if (LoadTextFilesOnlyEnabled)
            {
                // load gamestring file
                string filePath = Path.Combine(HeroesDataLocalizedDataPath, GameStringFile);
                LoadTextFile(CASCHandlerData.OpenFile(filePath), filePath);
            }

            // load up files in includes.xml file - which are the heroes in the heromods folder
            using (Stream cascXmlStream = CASCHandlerData.OpenFile(Path.Combine(HeroesDataBaseDataDirectoryPath, IncludesXmlFile)))
            {
                if (IsCacheEnabled)
                {
                    AddXmlCachedFilePath(Path.Combine(HeroesDataBaseDataDirectoryPath, IncludesXmlFile));
                }

                XDocument gameDataXml = XDocument.Load(cascXmlStream);
                IEnumerable<XElement> pathElements = gameDataXml.Root.Elements("Path");

                foreach (XElement pathElement in pathElements)
                {
                    string? valuePath = pathElement?.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(valuePath))
                    {
                        valuePath = PathHelper.GetFilePath(valuePath);
                        valuePath = valuePath.Remove(0, 5); // remove 'mods/'

                        if (valuePath.StartsWith(HeroesModsDirectoryName, StringComparison.OrdinalIgnoreCase))
                        {
                            string gameDataPath = Path.Combine(ModsFolderPath, valuePath, BaseStormDataDirectoryName, GameDataXmlFile);

                            LoadGameDataXmlContents(gameDataPath);

                            string heroModsFontStylesFilePath = Path.Combine(ModsFolderPath, valuePath, BaseStormDataDirectoryName, UIDirectoryStringName, FontStyleFile);
                            if (CASCHandlerData.FileExists(heroModsFontStylesFilePath))
                            {
                                LoadStormStyleFile(CASCHandlerData.OpenFile(heroModsFontStylesFilePath));

                                if (IsCacheEnabled)
                                    AddStormStyleCachedFilePath(heroModsFontStylesFilePath);
                            }

                            if (LoadTextFilesOnlyEnabled)
                            {
                                string filePath = Path.Combine(ModsFolderPath, valuePath, GameStringLocalization, LocalizedDataName, GameStringFile);

                                if (CASCHandlerData.FileExists(filePath))
                                    LoadTextFile(CASCHandlerData.OpenFile(filePath), filePath);
                            }
                        }
                    }
                }
            }

            string fontStylesFilePath = Path.Combine(HeroesDataBaseDataDirectoryPath, UIDirectoryStringName, FontStyleFile);
            LoadStormStyleFile(CASCHandlerData.OpenFile(fontStylesFilePath));

            if (IsCacheEnabled)
                AddStormStyleCachedFilePath(fontStylesFilePath);
        }

        protected override void LoadHeroesMapMods()
        {
            CASCFolder currentFolder = CASCFolderData.GetDirectory(HeroesMapModsDirectoryPath);

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

                            if (!CASCHandlerData.FileExists(filePath))
                                throw new FileNotFoundException(filePath);

                            using Stream data = CASCHandlerData.OpenFile(filePath);

                            LoadXmlFile(mapFolder.Value.Name, data, filePath);
                        }
                    }
                }

                if (LoadTextFilesOnlyEnabled)
                {
                    string filePath = Path.Combine(HeroesMapModsDirectoryPath, mapFolder.Value.Name, GameStringLocalization, LocalizedDataName, GameStringFile);

                    if (CASCHandlerData.FileExists(filePath))
                        LoadTextFile(mapFolder.Value.Name, CASCHandlerData.OpenFile(filePath), filePath);
                }
            }
        }

        protected override void LoadGameDataXmlContents(string gameDataXmlFilePath)
        {
            if ((!CASCHandlerData.FileExists(gameDataXmlFilePath) && gameDataXmlFilePath.Contains($"{HeroInteractionsStringName}.stormmod", StringComparison.OrdinalIgnoreCase)) || !LoadXmlFilesEnabled)
                return;

            if (IsCacheEnabled)
            {
                AddXmlCachedFilePath(gameDataXmlFilePath);
            }

            // load up files in gamedata.xml file
            using Stream cascXmlStream = CASCHandlerData.OpenFile(gameDataXmlFilePath);

            XDocument gameDataXml = XDocument.Load(cascXmlStream);
            IEnumerable<XElement> catalogElements = gameDataXml.Root.Elements("Catalog");

            foreach (XElement catalogElement in catalogElements)
            {
                string? pathValue = catalogElement?.Attribute("path")?.Value;
                if (!string.IsNullOrEmpty(pathValue))
                {
                    pathValue = PathHelper.GetFilePath(pathValue);

                    if (pathValue.Contains($"{GameDataStringName}/", StringComparison.OrdinalIgnoreCase))
                    {
                        if (gameDataXmlFilePath.Contains(HeroesDataStormModDirectoryName, StringComparison.OrdinalIgnoreCase))
                        {
                            string xmlFilePath = Path.Combine(HeroesDataBaseDataDirectoryPath, pathValue);
                            if (Path.GetExtension(xmlFilePath) == ".xml")
                            {
                                if (CASCHandlerData.FileExists(xmlFilePath))
                                {
                                    using Stream stream = CASCHandlerData.OpenFile(xmlFilePath);

                                    LoadXmlFile(stream, xmlFilePath);
                                }
                            }
                        }
                        else
                        {
                            string xmlFilePath = Path.Combine(gameDataXmlFilePath.Replace(GameDataXmlFile, string.Empty), pathValue);

                            if (Path.GetExtension(xmlFilePath) == ".xml")
                            {
                                using Stream stream = CASCHandlerData.OpenFile(xmlFilePath);

                                LoadXmlFile(stream, xmlFilePath);
                            }
                        }
                    }
                }
            }
        }

        private void SetCorrectFileCasing()
        {
            if (!CASCFolderData.DirectoryExists(CoreLocalizedDataPath))
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
