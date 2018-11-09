using CASCLib;
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
            CASCFolder currentFolder = CASCFolderData.GetDirectory(CoreStormModFolderPath);

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
            CASCFolder currentFolder = CASCFolderData.GetDirectory(HeroesdataStormModFolderPath);

            foreach (KeyValuePair<string, ICASCEntry> file in currentFolder.Entries)
            {
                if (Path.GetExtension(file.Key) != ".xml")
                    continue;

                Stream data = CASCHandlerData.OpenFile(((CASCFile)file.Value).FullName);

                XmlGameData.Root.Add(XDocument.Load(data).Root.Elements());
                XmlFileCount++;
            }
        }

        protected override void LoadOldHeroes()
        {
            CASCFolder currentFolder = CASCFolderData.GetDirectory(OldHeroesFolderPath);

            // loop through each hero folder
            foreach (KeyValuePair<string, ICASCEntry> heroFolder in currentFolder.Entries)
            {
                if (!heroFolder.Key.ToLower().Contains("data"))
                    continue;

                string pathFileName = ((CASCFile)((CASCFolder)heroFolder.Value).GetEntry($"{heroFolder.Key}.xml")).FullName;
                Stream data = CASCHandlerData.OpenFile(pathFileName);

                XmlGameData.Root.Add(XDocument.Load(data).Root.Elements());
                XmlFileCount++;
            }
        }

        protected override void LoadNewHeroes()
        {
            CASCFolder currentFolder = CASCFolderData.GetDirectory(NewHeroesFolderPath);

            // loop through each hero folder
            foreach (KeyValuePair<string, ICASCEntry> heroFolder in currentFolder.Entries)
            {
                if (!heroFolder.Key.Contains("stormmod") || heroFolder.Key == "herointeractions.stormmod")
                    continue;

                string heroName = heroFolder.Key.Split('.')[0];

                ICASCEntry baseStormData = ((CASCFolder)heroFolder.Value).GetEntry("base.stormdata");
                ICASCEntry gameData = ((CASCFolder)baseStormData).GetEntry(GameDataStringName);

                ICASCEntry xmlHero = ((CASCFolder)gameData).GetEntry($"{heroName}{DataStringName}.xml");
                ICASCEntry xmlHeroName = ((CASCFolder)gameData).GetEntry($"{heroName}.xml");
                ICASCEntry xmlHeroData = ((CASCFolder)gameData).GetEntry($"{HeroDataStringName}.xml");

                if (xmlHero != null && !string.IsNullOrEmpty(xmlHero.Name))
                {
                    Stream data = CASCHandlerData.OpenFile(((CASCFile)xmlHero).FullName);
                    XmlGameData.Root.Add(XDocument.Load(data).Root.Elements());
                    XmlFileCount++;
                }
                else
                {
                    Stream data = CASCHandlerData.OpenFile(((CASCFile)xmlHeroName).FullName);
                    XmlGameData.Root.Add(XDocument.Load(data).Root.Elements());
                    XmlFileCount++;
                }

                if (xmlHeroData != null && !string.IsNullOrEmpty(xmlHeroData.Name))
                {
                    Stream data = CASCHandlerData.OpenFile(((CASCFile)xmlHeroData).FullName);
                    XmlGameData.Root.Add(XDocument.Load(data).Root.Elements());
                    XmlFileCount++;
                }
            }
        }

        protected override void LoadHeroesMapMods()
        {
            CASCFolder currentFolder = CASCFolderData.GetDirectory(HeroesMapModsFolderPath);

            // loop through each mapmods folder
            foreach (KeyValuePair<string, ICASCEntry> mapFolder in currentFolder.Entries)
            {
                ICASCEntry baseStormDataFolder = ((CASCFolder)mapFolder.Value).GetEntry("base.stormdata");
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
    }
}
