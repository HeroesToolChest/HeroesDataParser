using HeroesData.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace HeroesData.Loader.XmlGameData
{
    public class FileGameData : GameData
    {
        public FileGameData(string modsFolderPath)
            : base(modsFolderPath)
        {
            Initialize();
        }

        public FileGameData(string modsFolderPath, int? hotsBuild)
            : base(modsFolderPath, hotsBuild)
        {
            Initialize();
        }

        protected override void LoadCoreStormMod()
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
                    XmlGameData.Root.Add(XDocument.Load(file).Root.Elements());
                    XmlFileCount++;
                }
            }
        }

        protected override void LoadHeroesDataStormMod()
        {
            // load up xml files in gamedata folder
            foreach (string file in Directory.GetFiles(Path.Combine(HeroesDataBaseDataDirectoryPath, GameDataStringName)))
            {
                if (Path.GetExtension(file) != ".xml")
                    continue;

                XmlGameData.Root.Add(XDocument.Load(file).Root.Elements());
                XmlFileCount++;
            }

            LoadGameDataXmlContents(Path.Combine(HeroesDataBaseDataDirectoryPath, GameDataXmlFile));

            // load up files in includes.xml file - which is the heroes in the heromods folder
            XDocument includesXml = XDocument.Load(Path.Combine(HeroesDataBaseDataDirectoryPath, IncludesXmlFile));
            IEnumerable pathElements = includesXml.Root.Elements("Path");

            foreach (XElement pathElement in pathElements)
            {
                string valuePath = pathElement.Attribute("value")?.Value?.ToLower();
                if (!string.IsNullOrEmpty(valuePath))
                {
                    valuePath = PathExtensions.GetFilePath(valuePath);
                    valuePath = valuePath.Remove(0, 5); // remove 'mods/'

                    if (valuePath.StartsWith(HeroesModsDiretoryName) && !valuePath.EndsWith("herointeractions.stormmod"))
                    {
                        string gameDataPath = Path.Combine(ModsFolderPath, valuePath, BaseStormDataDirectoryName, GameDataXmlFile);

                        LoadGameDataXmlContents(gameDataPath);
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

                foreach (string xmlFilePath in Directory.GetFiles(xmlMapGameDataPath))
                {
                    if (Path.GetExtension(xmlFilePath) == ".xml")
                    {
                        XmlGameData.Root.Add(XDocument.Load(xmlFilePath).Root.Elements());
                        XmlFileCount++;
                    }
                }
            }
        }

        protected override void LoadGameDataXmlContents(string gameDataXmlFilePath)
        {
            // load up files in gamedata.xml file
            XDocument gameDataXml = XDocument.Load(gameDataXmlFilePath);
            IEnumerable<XElement> catalogElements = gameDataXml.Root.Elements("Catalog");

            foreach (XElement catalogElement in catalogElements)
            {
                string pathValue = catalogElement.Attribute("path")?.Value?.ToLower();
                if (!string.IsNullOrEmpty(pathValue))
                {
                    pathValue = PathExtensions.GetFilePath(pathValue);

                    if (!pathValue.Contains("skindata") && !pathValue.Contains("sounddata"))
                    {
                        if (gameDataXmlFilePath.Contains(HeroesDataStormModDirectoryName) && pathValue.StartsWith("gamedata/heroes"))
                        {
                            string xmlFilePath = Path.Combine(HeroesDataBaseDataDirectoryPath, pathValue);

                            XmlGameData.Root.Add(XDocument.Load(xmlFilePath).Root.Elements());
                            XmlFileCount++;
                        }
                        else if (gameDataXmlFilePath.Contains(HeroesModsDiretoryName) && pathValue.StartsWith("gamedata/"))
                        {
                            string xmlFilePath = Path.Combine(gameDataXmlFilePath.Replace(GameDataXmlFile, string.Empty), pathValue);

                            XmlGameData.Root.Add(XDocument.Load(xmlFilePath).Root.Elements());
                            XmlFileCount++;
                        }
                    }
                }
            }
        }
    }
}
