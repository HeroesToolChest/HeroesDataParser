using System.IO;

namespace HeroesData.Parser.GameStrings
{
    public class FileGameStringData : GameStringData
    {
        protected override void ParseGameStringFiles()
        {
            ParseFiles(Path.Combine(OldDescriptionsPath, GameStringFile));
            ParseNewHeroes();
        }

        protected override void ParseNewHeroes()
        {
            foreach (string heroDirectory in Directory.GetDirectories(HeroModsPath))
            {
                ParseFiles(Path.Combine(heroDirectory, GameStringLocalization, "LocalizedData", "GameStrings.txt"));
            }
        }

        private void ParseFiles(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                ReadFile(reader);
            }
        }
    }
}
