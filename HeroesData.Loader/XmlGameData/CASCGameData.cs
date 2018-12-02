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

        public CASCGameData(CASCHandler cascHandler, CASCFolder cascFolder, string modsFolderPath)
            : base(modsFolderPath)
        {
            CASCHandlerData = cascHandler;
            CASCFolderData = cascFolder;

            Initialize();
        }

        public CASCGameData(CASCHandler cascHandler, CASCFolder cascFolder, string modsFolderPath, int? hotsBuild)
            : base(modsFolderPath, hotsBuild)
        {
            CASCHandlerData = cascHandler;
            CASCFolderData = cascFolder;

            Initialize();
        }

        protected override void LoadCoreStormMod()
        {
            // core.stormmod xml files
            CASCFolder currentFolder = CASCFolderData.GetDirectory(Path.Combine(CoreBaseDataDirectoryPath, GameDataStringName));

            foreach (KeyValuePair<string, ICASCEntry> file in currentFolder.Entries)
            {
                if (Path.GetExtension(file.Key) != ".xml")
                    continue;

                Stream data = CASCHandlerData.OpenFile(((CASCFile)file.Value).FullName);

                if (XmlGameData.LastNode == null)
                {
                    XmlGameData = XDocument.Load(data);
                    XmlFileCount++;
                }
                else
                {
                    XmlGameData.Root.Add(XDocument.Load(data).Root.Elements());
                    XmlFileCount++;
                }
            }
        }

        protected override void LoadHeroesDataStormMod()
        {
            // load up xml files in gamedata folder
            CASCFolder currentFolder = CASCFolderData.GetDirectory(Path.Combine(HeroesDataBaseDataDirectoryPath, GameDataStringName));

            foreach (KeyValuePair<string, ICASCEntry> file in currentFolder.Entries)
            {
                if (Path.GetExtension(file.Key) != ".xml")
                    continue;

                Stream data = CASCHandlerData.OpenFile(((CASCFile)file.Value).FullName);

                XmlGameData.Root.Add(XDocument.Load(data).Root.Elements());
                XmlFileCount++;
            }

            // load up files in gamedata.xml file
            LoadGameDataXmlContents(Path.Combine(HeroesDataBaseDataDirectoryPath, GameDataXmlFile));

            // load up files in includes.xml file - which is the heroes in the heromods folder
            Stream cascXml = CASCHandlerData.OpenFile(Path.Combine(HeroesDataBaseDataDirectoryPath, IncludesXmlFile));
            XDocument gameDataXml = XDocument.Load(cascXml);
            IEnumerable<XElement> pathElements = gameDataXml.Root.Elements("Path");

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
            CASCFolder currentFolder = CASCFolderData.GetDirectory(HeroesMapModsDirectoryPath);

            // loop through each mapmods folder
            foreach (KeyValuePair<string, ICASCEntry> mapFolder in currentFolder.Entries)
            {
                ICASCEntry baseStormDataFolder = ((CASCFolder)mapFolder.Value).GetEntry(BaseStormDataDirectoryName);
                ICASCEntry gameDataFolder = ((CASCFolder)baseStormDataFolder).GetEntry(GameDataStringName);

                foreach (KeyValuePair<string, ICASCEntry> dataFiles in ((CASCFolder)gameDataFolder).Entries)
                {
                    if (Path.GetExtension(dataFiles.Key) == ".xml")
                    {
                        Stream data = CASCHandlerData.OpenFile(((CASCFile)dataFiles.Value).FullName);

                        XmlGameData.Root.Add(XDocument.Load(data).Root.Elements());
                        XmlFileCount++;
                    }
                }
            }
        }

        protected override void LoadGameDataXmlContents(string gameDataXmlFilePath)
        {
            // load up files in gamedata.xml file
            Stream cascXml = CASCHandlerData.OpenFile(gameDataXmlFilePath);

            XDocument gameDataXml = XDocument.Load(cascXml);
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
                            cascXml = CASCHandlerData.OpenFile(xmlFilePath);

                            XmlGameData.Root.Add(XDocument.Load(cascXml).Root.Elements());
                            XmlFileCount++;
                        }
                        else if (gameDataXmlFilePath.Contains(HeroesModsDiretoryName) && pathValue.StartsWith("gamedata/"))
                        {
                            string xmlFilePath = Path.Combine(gameDataXmlFilePath.Replace(GameDataXmlFile, string.Empty), pathValue);
                            cascXml = CASCHandlerData.OpenFile(xmlFilePath);

                            XmlGameData.Root.Add(XDocument.Load(cascXml).Root.Elements());
                            XmlFileCount++;
                        }
                    }
                }
            }
        }
    }
}
