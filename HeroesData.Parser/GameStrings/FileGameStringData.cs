using System.IO;

namespace HeroesData.Parser.GameStrings
{
    public class FileGameStringData : GameStringData
    {
        protected override void ParseGameStringFiles()
        {
            ParseFiles(Path.Combine(CoreStormmodDescriptionsPath, GameStringFile));
            ParseFiles(Path.Combine(OldDescriptionsPath, GameStringFile));

            ParseNewHeroes();
            ParseMapMods();
        }

        protected override void ParseMapMods()
        {
            foreach (string mapDirectory in Directory.GetDirectories(MapModsPath))
            {
                ParseFiles(Path.Combine(mapDirectory, GameStringLocalization, LocalizedName, GameStringFile), true);
            }
        }

        protected override void ParseNewHeroes()
        {
            foreach (string heroDirectory in Directory.GetDirectories(HeroModsPath))
            {
                ParseFiles(Path.Combine(heroDirectory, GameStringLocalization, LocalizedName, GameStringFile));
            }
        }

        private void ParseFiles(string filePath, bool isMapMod = false)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                ReadFile(reader, isMapMod);
            }
        }
    }
}
