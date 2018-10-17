using CASCLib;
using System.Collections.Generic;

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
            CASCFolder currentFolder = CASCFolderData.GetDirectory(CoreStormmodDescriptionsPath);
            ParseFile(CASCHandlerData.OpenFile(((CASCFile)currentFolder.GetEntry(GameStringFile)).FullName));

            currentFolder = CASCFolderData.GetDirectory(OldDescriptionsPath);
            ParseFile(CASCHandlerData.OpenFile(((CASCFile)currentFolder.GetEntry(GameStringFile)).FullName));

            ParseNewHeroes();
            ParseMapMods();
        }

        protected override void ParseMapMods()
        {
            CASCFolder currentFolder = CASCFolderData.GetDirectory(MapModsPath);

            foreach (KeyValuePair<string, ICASCEntry> mapFolder in currentFolder.Entries)
            {
                // check if localization folder exists
                if (!((CASCFolder)mapFolder.Value).Entries.ContainsKey(GameStringLocalization))
                    continue;

                ICASCEntry localizationStormdata = ((CASCFolder)mapFolder.Value).GetEntry(GameStringLocalization);
                ICASCEntry localizedData = ((CASCFolder)localizationStormdata).GetEntry(LocalizedName);

                ICASCEntry gameStringFile = ((CASCFolder)localizedData).GetEntry(GameStringFile);
                ParseFile(CASCHandlerData.OpenFile(((CASCFile)gameStringFile).FullName), true);
            }
        }

        protected override void ParseNewHeroes()
        {
            CASCFolder currentFolder = CASCFolderData.GetDirectory(HeroModsPath);

            foreach (KeyValuePair<string, ICASCEntry> heroFolder in currentFolder.Entries)
            {
                if (heroFolder.Key != "herointeractions.stormmod")
                {
                    ICASCEntry localizationStormdata = ((CASCFolder)heroFolder.Value).GetEntry(GameStringLocalization);
                    ICASCEntry localizedData = ((CASCFolder)localizationStormdata).GetEntry(LocalizedName);

                    ICASCEntry gameStringFile = ((CASCFolder)localizedData).GetEntry(GameStringFile);
                    ParseFile(CASCHandlerData.OpenFile(((CASCFile)gameStringFile).FullName));
                }
            }
        }
    }
}
