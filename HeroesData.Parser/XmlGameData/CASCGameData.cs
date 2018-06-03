using CASCLib;
using HeroesData.Parser.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlGameData
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

        protected override void LoadCoreStormMod()
        {
            // core.stormmod xml files
            CASCFolder currentFolder = CASCExtensions.GetDirectory(CASCFolderData, CoreStormModFolderPath);

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
            CASCFolder currentFolder = CASCExtensions.GetDirectory(CASCFolderData, HeroesdataStormModFolderPath);

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
            CASCFolder currentFolder = CASCExtensions.GetDirectory(CASCFolderData, OldHeroesFolderPath);

            // loop through each hero folder
            foreach (KeyValuePair<string, ICASCEntry> heroFolder in currentFolder.Entries)
            {
                if (!heroFolder.Key.Contains("Data"))
                    continue;

                string pathFileName = ((CASCFile)((CASCFolder)heroFolder.Value).GetEntry($"{heroFolder.Key}.xml")).FullName;
                Stream data = CASCHandlerData.OpenFile(pathFileName);

                XmlGameData.Root.Add(XDocument.Load(data).Root.Elements());
                XmlFileCount++;
            }
        }

        protected override void LoadNewHeroes()
        {
            CASCFolder currentFolder = CASCExtensions.GetDirectory(CASCFolderData, NewHeroesFolderPath);

            // loop through each hero folder
            foreach (KeyValuePair<string, ICASCEntry> heroFolder in currentFolder.Entries)
            {
                try
                {
                    if (!heroFolder.Key.Contains("stormmod") || heroFolder.Key == "herointeractions.stormmod")
                        continue;

                    string heroName = heroFolder.Key.Split('.')[0];

                    ICASCEntry baseStormData = ((CASCFolder)heroFolder.Value).GetEntry("base.stormData");
                    ICASCEntry gameData = ((CASCFolder)baseStormData).GetEntry("GameData");

                    ICASCEntry xmlHero = ((CASCFolder)gameData).GetEntry($"{heroName}Data.xml");
                    ICASCEntry xmlHeroName = ((CASCFolder)gameData).GetEntry($"{heroName}.xml");
                    ICASCEntry xmlHeroData = ((CASCFolder)gameData).GetEntry($"HeroData.xml");

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
                catch (Exception ex)
                {
                    throw new ParseException($"LoadNewHeroes error: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }
    }
}
