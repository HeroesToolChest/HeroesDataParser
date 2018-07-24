using CASCLib;
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
            CASCFolder currentFolder = CASCExtensions.GetDirectory(CASCFolderData, OldDescriptionsPath);

            ParseFiles(CASCHandlerData.OpenFile(((CASCFile)currentFolder.GetEntry(GameStringFile)).FullName));
            ParseNewHeroes();
        }

        protected override void ParseNewHeroes()
        {
            CASCFolder currentFolder = CASCExtensions.GetDirectory(CASCFolderData, HeroModsPath);

            foreach (KeyValuePair<string, ICASCEntry> heroFolder in currentFolder.Entries)
            {
                if (heroFolder.Key != "herointeractions.stormmod")
                {
                    ICASCEntry localizationStormdata = ((CASCFolder)heroFolder.Value).GetEntry(GameStringLocalization);
                    ICASCEntry localizedData = ((CASCFolder)localizationStormdata).GetEntry("LocalizedData");

                    ICASCEntry gameStringFile = ((CASCFolder)localizedData).GetEntry(GameStringFile);
                    Stream data = CASCHandlerData.OpenFile(((CASCFile)gameStringFile).FullName);

                    ParseFiles(data);
                }
            }
        }

        private void ParseFiles(Stream fileStream)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                ReadFile(reader);
            }
        }
    }
}
