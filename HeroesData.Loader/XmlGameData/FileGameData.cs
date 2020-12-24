using HeroesData.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace HeroesData.Loader.XmlGameData
{
    public class FileGameData : GameData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileGameData"/> class.
        /// </summary>
        /// <param name="modsFolderPath">The file path of the mods folder.</param>
        public FileGameData(string modsFolderPath)
            : base(modsFolderPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileGameData"/> class.
        /// </summary>
        /// <param name="modsFolderPath">The file path of the mods folder.</param>
        /// <param name="hotsBuild">The hots build number.</param>
        public FileGameData(string modsFolderPath, int? hotsBuild)
            : base(modsFolderPath, hotsBuild)
        {
        }

        protected override void LoadCoreStormMod()
        {
            if (LoadXmlFilesEnabled)
            {
                foreach (string file in Directory.GetFiles(Path.Combine(CoreBaseDataDirectoryPath, GameDataStringName)))
                {
                    if (Path.GetExtension(file) != ".xml")
                        continue;

                    if (XmlGameData.LastNode == null)
                    {
                        XmlGameData = XDocument.Load(file);
                        XmlFileCount++;
                    }
                    else
                    {
                        LoadXmlFile(file);
                    }
                }
            }

            if (LoadTextFilesOnlyEnabled)
                LoadTextFile(Path.Combine(CoreLocalizedDataPath, GameStringFile));

            if (LoadStormStyleEnabled)
                LoadStormStyleFile(Path.Combine(CoreBaseDataDirectoryPath, UIDirectoryStringName, FontStyleFile));
        }

        protected override void LoadHeroesDataStormMod()
        {
            LoadDefaultData(HeroesDataBaseDataDirectoryPath, HeroesDataLocalizedDataPath, true);

            // load up files in includes.xml file - which are the heroes in the heromods folder
            XDocument includesXml = XDocument.Load(Path.Combine(HeroesDataBaseDataDirectoryPath, IncludesXmlFile));

            if (includesXml.Root == null)
                throw new InvalidOperationException();

            IEnumerable pathElements = includesXml.Root.Elements("Path");

            foreach (XElement? pathElement in pathElements)
            {
                string? valuePath = pathElement?.Attribute("value")?.Value?.ToLowerInvariant();
                if (!string.IsNullOrEmpty(valuePath))
                {
                    valuePath = PathHelper.GetFilePath(valuePath);

                    if (!string.IsNullOrEmpty(valuePath))
                    {
                        valuePath = valuePath.Remove(0, 5); // remove 'mods/'

                        if (valuePath.StartsWith(HeroesModsDirectoryName, StringComparison.OrdinalIgnoreCase))
                        {
                            string gameDataPath = Path.Combine(ModsFolderPath, valuePath, BaseStormDataDirectoryName, GameDataXmlFile);

                            LoadGameDataXmlContents(gameDataPath);

                            if (LoadStormStyleEnabled)
                                LoadStormStyleFile(Path.Combine(ModsFolderPath, valuePath, BaseStormDataDirectoryName, UIDirectoryStringName, FontStyleFile));

                            if (LoadTextFilesOnlyEnabled)
                            {
                                try
                                {
                                    LoadTextFile(Path.Combine(ModsFolderPath, valuePath, GameStringLocalization, LocalizedDataName, GameStringFile));
                                }
                                catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                                {
                                    if (!valuePath.Contains(HeroInteractionsStringName, StringComparison.OrdinalIgnoreCase))
                                        throw;
                                }
                            }
                        }
                    }
                }
            }

            if (LoadStormStyleEnabled)
                LoadStormStyleFile(Path.Combine(CoreBaseDataDirectoryPath, UIDirectoryStringName, FontStyleFile));
        }

        protected override void LoadHeroesMapMods()
        {
            foreach (string mapModsFolderPath in Directory.GetDirectories(HeroesMapModsDirectoryPath))
            {
                string mapFolderName = Path.GetFileName(mapModsFolderPath);
                string xmlMapGameDataPath = Path.Combine(HeroesMapModsDirectoryPath, mapFolderName, BaseStormDataDirectoryName, GameDataStringName);

                if (LoadXmlFilesEnabled)
                {
                    foreach (string xmlFilePath in Directory.GetFiles(xmlMapGameDataPath))
                    {
                        LoadXmlFile(mapFolderName, xmlFilePath);
                    }
                }

                if (LoadTextFilesOnlyEnabled)
                {
                    try
                    {
                        LoadTextFile(mapFolderName, Path.Combine(HeroesMapModsDirectoryPath, mapFolderName, GameStringLocalization, LocalizedDataName, GameStringFile));
                    }
                    catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                    {
                        if (!mapFolderName.Contains(ConveyorBeltsStringName, StringComparison.OrdinalIgnoreCase))
                            throw;
                    }
                }
            }
        }

        protected override void LoadGameDataXmlContents(string gameDataXmlFilePath)
        {
            if (gameDataXmlFilePath is null)
                throw new ArgumentNullException(nameof(gameDataXmlFilePath));

            if ((!File.Exists(gameDataXmlFilePath) && gameDataXmlFilePath.Contains("herointeractions.stormmod", StringComparison.OrdinalIgnoreCase)) || !LoadXmlFilesEnabled)
                return;

            // load up files in gamedata.xml file
            XDocument gameDataXml = XDocument.Load(gameDataXmlFilePath);

            if (gameDataXml.Root == null)
                throw new InvalidOperationException();

            IEnumerable<XElement> catalogElements = gameDataXml.Root.Elements("Catalog");

            foreach (XElement catalogElement in catalogElements)
            {
                string? pathValue = catalogElement?.Attribute("path")?.Value?.ToLowerInvariant();
                if (!string.IsNullOrEmpty(pathValue))
                {
                    pathValue = PathHelper.GetFilePath(pathValue);

                    if (pathValue!.Contains($"{GameDataStringName}/", StringComparison.OrdinalIgnoreCase))
                    {
                        if (gameDataXmlFilePath.Contains(HeroesDataStormModDirectoryName, StringComparison.OrdinalIgnoreCase))
                        {
                            string xmlFilePath = Path.Combine(HeroesDataBaseDataDirectoryPath, pathValue);

                            if (Path.GetExtension(xmlFilePath) == ".xml")
                            {
                                if (File.Exists(xmlFilePath))
                                {
                                    LoadXmlFile(xmlFilePath);
                                }
                            }
                        }
                        else
                        {
                            string xmlFilePath = Path.Combine(gameDataXmlFilePath.Replace(GameDataXmlFile, string.Empty, StringComparison.OrdinalIgnoreCase), pathValue);
                            LoadXmlFile(xmlFilePath);
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
                // added loot box storm mod parsing for hdp in 83036
                if (HotsBuild >= 83086 || !stormModPath.Contains(LootBoxStormModsDirectoryName, StringComparison.OrdinalIgnoreCase) || Directory.Exists(stormModPath))
                {
                    // load up xml files in gamedata folder
                    foreach (string file in Directory.GetFiles(Path.Combine(stormModPath, GameDataStringName)))
                    {
                        LoadXmlFile(file);
                    }
                }

                if (loadGameDataFile)
                {
                    // load up files in gamedata.xml file
                    LoadGameDataXmlContents(Path.Combine(HeroesDataBaseDataDirectoryPath, GameDataXmlFile));
                }
            }

            if (LoadTextFilesOnlyEnabled)
            {
                // load gamestring file
                LoadTextFile(Path.Combine(localizedPath, GameStringFile));
            }
        }
    }
}
