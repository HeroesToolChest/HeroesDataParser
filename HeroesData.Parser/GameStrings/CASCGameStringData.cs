﻿using CASCLib;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.Parser.GameStrings
{
    public class CASCGameStringData : GameStringData
    {
        private readonly CASCHandler CASCHandlerData;
        private readonly CASCFolder CASCFolderData;

        public CASCGameStringData(CASCHandler cascHandler, CASCFolder cascFolder)
        {
            CASCHandlerData = cascHandler;
            CASCFolderData = cascFolder;
        }

        protected override void ParseGameStringFiles()
        {
            CASCFolder currentFolder = CASCExtensions.GetDirectory(CASCFolderData, CoreStormmodDescriptionsPath);
            ParseFiles(CASCHandlerData.OpenFile(((CASCFile)currentFolder.GetEntry(GameStringFile)).FullName));

            currentFolder = CASCExtensions.GetDirectory(CASCFolderData, OldDescriptionsPath);
            ParseFiles(CASCHandlerData.OpenFile(((CASCFile)currentFolder.GetEntry(GameStringFile)).FullName));

            ParseNewHeroes();
            ParseMapMods();
        }

        protected override void ParseMapMods()
        {
            CASCFolder currentFolder = CASCExtensions.GetDirectory(CASCFolderData, MapModsPath);

            foreach (KeyValuePair<string, ICASCEntry> mapFolder in currentFolder.Entries)
            {
                ICASCEntry localizationStormdata = ((CASCFolder)mapFolder.Value).GetEntry(GameStringLocalization);
                ICASCEntry localizedData = ((CASCFolder)localizationStormdata).GetEntry(LocalizedName);

                ICASCEntry gameStringFile = ((CASCFolder)localizedData).GetEntry(GameStringFile);
                Stream data = CASCHandlerData.OpenFile(((CASCFile)gameStringFile).FullName);

                ParseFiles(data, true);
            }
        }

        protected override void ParseNewHeroes()
        {
            CASCFolder currentFolder = CASCExtensions.GetDirectory(CASCFolderData, HeroModsPath);

            foreach (KeyValuePair<string, ICASCEntry> heroFolder in currentFolder.Entries)
            {
                if (heroFolder.Key != "herointeractions.stormmod")
                {
                    ICASCEntry localizationStormdata = ((CASCFolder)heroFolder.Value).GetEntry(GameStringLocalization);
                    ICASCEntry localizedData = ((CASCFolder)localizationStormdata).GetEntry(LocalizedName);

                    ICASCEntry gameStringFile = ((CASCFolder)localizedData).GetEntry(GameStringFile);
                    Stream data = CASCHandlerData.OpenFile(((CASCFile)gameStringFile).FullName);

                    ParseFiles(data);
                }
            }
        }

        private void ParseFiles(Stream fileStream, bool isMapMod = false)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                ReadFile(reader, isMapMod);
            }
        }
    }
}
