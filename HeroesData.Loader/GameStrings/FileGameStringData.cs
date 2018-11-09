using System.IO;

namespace HeroesData.Loader.GameStrings
{
    public class FileGameStringData : GameStringData
    {
        protected override void ParseGameStringFiles()
        {
            ParseFile(Path.Combine(CoreStormmodDescriptionsPath, GameStringFile));
            ParseFile(Path.Combine(OldDescriptionsPath, GameStringFile));

            ParseNewHeroes();
            ParseMapMods();
        }

        protected override void ParseMapMods()
        {
            foreach (string mapDirectory in Directory.GetDirectories(MapModsPath))
            {
                ParseFile(Path.Combine(mapDirectory, GameStringLocalization, LocalizedName, GameStringFile), true);
            }
        }

        protected override void ParseNewHeroes()
        {
            foreach (string heroDirectory in Directory.GetDirectories(HeroModsPath))
            {
                ParseFile(Path.Combine(heroDirectory, GameStringLocalization, LocalizedName, GameStringFile));
            }
        }
    }
}
