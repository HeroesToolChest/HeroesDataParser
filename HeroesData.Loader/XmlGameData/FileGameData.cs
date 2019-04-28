using HeroesData.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace HeroesData.Loader.XmlGameData
{
    public class FileGameData : GameData
    {
        /// <summary>
        /// Contruct the FileGameData object.
        /// </summary>
        /// <param name="modsFolderPath">The file path of the mods folder.</param>
        public FileGameData(string modsFolderPath)
            : base(modsFolderPath)
        {
        }

        /// <summary>
        /// Contruct the FileGameData object.
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
            {
                LoadTextFile(Path.Combine(CoreLocalizedDataPath, GameStringFile));
            }
        }

        protected override void LoadHeroesDataStormMod()
        {
            if (LoadXmlFilesEnabled)
            {
                // load up xml files in gamedata folder
                foreach (string file in Directory.GetFiles(Path.Combine(HeroesDataBaseDataDirectoryPath, GameDataStringName)))
                {
                    LoadXmlFile(file);
                }

                // load up files in gamedata.xml file
                LoadGameDataXmlContents(Path.Combine(HeroesDataBaseDataDirectoryPath, GameDataXmlFile));
            }

            if (LoadTextFilesOnlyEnabled)
            {
                // load gamestring file
                LoadTextFile(Path.Combine(HeroesDataLocalizedDataPath, GameStringFile));
            }

            // load up files in includes.xml file - which are the heroes in the heromods folder
            XDocument includesXml = XDocument.Load(Path.Combine(HeroesDataBaseDataDirectoryPath, IncludesXmlFile));
            IEnumerable pathElements = includesXml.Root.Elements("Path");

            foreach (XElement pathElement in pathElements)
            {
                string valuePath = pathElement.Attribute("value")?.Value?.ToLower();
                if (!string.IsNullOrEmpty(valuePath))
                {
                    valuePath = PathHelpers.GetFilePath(valuePath);
                    valuePath = valuePath.Remove(0, 5); // remove 'mods/'

                    if (valuePath.StartsWith(HeroesModsDiretoryName))
                    {
                        string gameDataPath = Path.Combine(ModsFolderPath, valuePath, BaseStormDataDirectoryName, GameDataXmlFile);

                        LoadGameDataXmlContents(gameDataPath);

                        if (LoadTextFilesOnlyEnabled)
                        {
                            try
                            {
                                LoadTextFile(Path.Combine(ModsFolderPath, valuePath, GameStringLocalization, LocalizedDataName, GameStringFile));
                            }
                            catch (FileNotFoundException)
                            {
                                if (!valuePath.Contains(HeroInteractionsStringName))
                                    throw;
                            }
                        }
                    }
                }
            }
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
                    catch (FileNotFoundException)
                    {
                        if (!mapFolderName.Contains(ConveyorBeltsStringName))
                            throw;
                    }
                }
            }
        }

        protected override void LoadGameDataXmlContents(string gameDataXmlFilePath)
        {
            if ((!File.Exists(gameDataXmlFilePath) && gameDataXmlFilePath.Contains("herointeractions.stormmod")) || !LoadXmlFilesEnabled)
                return;

            // load up files in gamedata.xml file
            XDocument gameDataXml = XDocument.Load(gameDataXmlFilePath);
            IEnumerable<XElement> catalogElements = gameDataXml.Root.Elements("Catalog");

            foreach (XElement catalogElement in catalogElements)
            {
                string pathValue = catalogElement.Attribute("path")?.Value?.ToLower();
                if (!string.IsNullOrEmpty(pathValue))
                {
                    pathValue = PathHelpers.GetFilePath(pathValue);

                    if (pathValue.Contains("gamedata/"))
                    {
                        if (gameDataXmlFilePath.Contains(HeroesDataStormModDirectoryName))
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
                            string xmlFilePath = Path.Combine(gameDataXmlFilePath.Replace(GameDataXmlFile, string.Empty), pathValue);
                            LoadXmlFile(xmlFilePath);
                        }
                    }
                }
            }
        }
    }
}
