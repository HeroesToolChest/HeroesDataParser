using System.Collections.Generic;
using System.IO;

namespace HeroesData.Parser.GameStrings
{
    public abstract class GameStringData
    {
        /// <summary>
        /// Gets the short tooltip descriptions of all ability/talent.
        /// </summary>
        public SortedDictionary<string, string> ShortTooltipsByShortTooltipNameId { get; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Gets the full tooltip descriptions of all ability/talent.
        /// </summary>
        public SortedDictionary<string, string> FullTooltipsByFullTooltipNameId { get; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Gets the hero descriptions of all heroes.
        /// </summary>
        public SortedDictionary<string, string> HeroDescriptionsByShortName { get; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Gets the real names of all heroes.
        /// </summary>
        public SortedDictionary<string, string> HeroNamesByShortName { get; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Gets the real names of the all ability/talents.
        /// </summary>
        public SortedDictionary<string, string> AbilityTalentNamesByReferenceNameId { get; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Gets the real names of all units.
        /// </summary>
        public SortedDictionary<string, string> UnitNamesByShortName { get; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Gets all other strings.
        /// </summary>
        public SortedDictionary<string, string> ValueStringByKeyString { get; } = new SortedDictionary<string, string>();

        public int? HotsBuild { get; set; } = null;
        public string ModsFolderPath { get; set; } = "mods";
        public string GameStringLocalization { get; set; } = "enus.stormdata";
        protected string CoreStormmodDescriptionsPath { get; private set; }
        protected string OldDescriptionsPath { get; private set; }
        protected string MapModsPath { get; private set; }
        protected string HeroModsPath { get; private set; }
        protected string GameStringFile { get; set; }
        protected string LocalizedName { get; set; }

        /// <summary>
        /// Loads all the required games strings.
        /// </summary>
        public void Load()
        {
            Initialize();
        }

        protected void Initialize()
        {
            GameStringFile = "gamestrings.txt";
            LocalizedName = "localizeddata";

            // default check
            OldDescriptionsPath = Path.Combine(ModsFolderPath, "heroesdata.stormmod", GameStringLocalization, LocalizedName);

            // if doesn't exist, try capitilized directory
            if (!Directory.Exists(OldDescriptionsPath))
            {
                GameStringFile = "GameStrings.txt";
                LocalizedName = "LocalizedData";
            }

            CoreStormmodDescriptionsPath = Path.Combine(ModsFolderPath, "core.stormmod", GameStringLocalization, LocalizedName);
            HeroModsPath = Path.Combine(ModsFolderPath, "heromods");
            MapModsPath = Path.Combine(ModsFolderPath, "heroesmapmods", "battlegroundmapmods");

            ParseGameStringFiles();
        }

        protected abstract void ParseGameStringFiles();
        protected abstract void ParseNewHeroes();
        protected abstract void ParseMapMods();

        protected void ReadFile(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (line.StartsWith(GameStringPrefixes.SimpleDisplayPrefix))
                {
                    string[] splitLine = line.Split(new char[] { '=' }, 2);
                    ShortTooltipsByShortTooltipNameId.Add(splitLine[0], splitLine[1]);
                }
                else if (line.StartsWith(GameStringPrefixes.SimplePrefix))
                {
                    string[] splitLine = line.Split(new char[] { '=' }, 2);
                    ShortTooltipsByShortTooltipNameId.Add(splitLine[0], splitLine[1]);
                }
                else if (line.StartsWith(GameStringPrefixes.DescriptionPrefix))
                {
                    string[] splitLine = line.Split(new char[] { '=' }, 2);
                    HeroDescriptionsByShortName.Add(splitLine[0], splitLine[1]);
                }
                else if (line.StartsWith(GameStringPrefixes.FullPrefix))
                {
                    string[] splitLine = line.Split(new char[] { '=' }, 2);
                    FullTooltipsByFullTooltipNameId.Add(splitLine[0], splitLine[1]);
                }
                else if (line.StartsWith(GameStringPrefixes.HeroNamePrefix))
                {
                    string[] splitLine = line.Split(new char[] { '=' }, 2);
                    HeroNamesByShortName.Add(splitLine[0], splitLine[1]);
                }
                else if (line.StartsWith(GameStringPrefixes.DescriptionNamePrefix))
                {
                    string[] splitLine = line.Split(new char[] { '=' }, 2);
                    AbilityTalentNamesByReferenceNameId.Add(splitLine[0], splitLine[1]);
                }
                else if (line.StartsWith(GameStringPrefixes.UnitPrefix))
                {
                    string[] splitLine = line.Split(new char[] { '=' }, 2);
                    UnitNamesByShortName.Add(splitLine[0], splitLine[1]);
                }
                else
                {
                    string[] splitLine = line.Split(new char[] { '=' }, 2);
                    if (splitLine.Length < 2)
                        continue;

                    ValueStringByKeyString.Add(splitLine[0], splitLine[1]);
                }
            }
        }
    }
}
