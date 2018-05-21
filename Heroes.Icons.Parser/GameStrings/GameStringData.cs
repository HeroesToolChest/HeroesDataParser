using System.Collections.Generic;
using System.IO;

namespace Heroes.Icons.Parser.GameStrings
{
    public class GameStringData
    {
        private readonly string SimpleDisplayPrefix = "Button/SimpleDisplayText/";
        private readonly string SimplePrefix = "Button/Simple/";
        private readonly string DescriptionPrefix = "Hero/Description/";
        private readonly string FullPrefix = "Button/Tooltip/";
        private readonly string HeroNamePrefix = "Hero/Name/"; // real name of hero
        private readonly string DescriptionNamePrefix = "Button/Name/"; // real name of ability/talent
        private readonly string UnitPrefix = "Unit/Name/";

        private readonly string ModsFolderPath;
        private readonly string OldDescriptionsPath;
        private readonly string HeroModsPath;

        public GameStringData(string modsFolderPath)
        {
            ModsFolderPath = modsFolderPath;
            OldDescriptionsPath = Path.Combine(modsFolderPath, @"heroesdata.stormmod\enus.stormdata\LocalizedData\GameStrings.txt");
            HeroModsPath = Path.Combine(modsFolderPath, "heromods");
        }

        /// <summary>
        /// Gets or sets the short tooltip descriptions of all ability/talent.
        /// </summary>
        public SortedDictionary<string, string> ShortTooltipsByShortTooltipNameId { get; set; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Gets or sets the full tooltip descriptions of all ability/talent.
        /// </summary>
        public SortedDictionary<string, string> FullTooltipsByFullTooltipNameId { get; set; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Gets or sets the hero descriptions of all heroes.
        /// </summary>
        public SortedDictionary<string, string> HeroDescriptionsByShortName { get; set; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Gets or sets the real names of all heroes.
        /// </summary>
        public SortedDictionary<string, string> HeroNamesByShortName { get; set; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Gets or sets the real names of the all ability/talents.
        /// </summary>
        public SortedDictionary<string, string> AbilityTalentNamesByReferenceNameId { get; set; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Gets or sets the real names of all units.
        /// </summary>
        public SortedDictionary<string, string> UnitNamesByShortName { get; set; } = new SortedDictionary<string, string>();

        public void Load()
        {
            ParseFiles(OldDescriptionsPath);
            ParseNewHeroes();
        }

        private void ParseFiles(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line.StartsWith(SimpleDisplayPrefix))
                    {
                        line = line.Remove(0, SimpleDisplayPrefix.Length);
                        string[] splitLine = line.Split(new char[] { '=' }, 2);
                        ShortTooltipsByShortTooltipNameId.Add(splitLine[0], splitLine[1]);
                    }
                    else if (line.StartsWith(SimplePrefix))
                    {
                        line = line.Remove(0, SimplePrefix.Length);
                        string[] splitLine = line.Split(new char[] { '=' }, 2);
                        ShortTooltipsByShortTooltipNameId.Add(splitLine[0], splitLine[1]);
                    }
                    else if (line.StartsWith(DescriptionPrefix))
                    {
                        line = line.Remove(0, DescriptionPrefix.Length);
                        string[] splitLine = line.Split(new char[] { '=' }, 2);
                        HeroDescriptionsByShortName.Add(splitLine[0], splitLine[1]);
                    }
                    else if (line.StartsWith(FullPrefix))
                    {
                        line = line.Remove(0, FullPrefix.Length);
                        string[] splitLine = line.Split(new char[] { '=' }, 2);
                        FullTooltipsByFullTooltipNameId.Add(splitLine[0], splitLine[1]);
                    }
                    else if (line.StartsWith(HeroNamePrefix))
                    {
                        line = line.Remove(0, HeroNamePrefix.Length);
                        string[] splitLine = line.Split(new char[] { '=' }, 2);

                        if (!HeroNamesByShortName.ContainsKey(splitLine[0]))
                            HeroNamesByShortName.Add(splitLine[0], splitLine[1]);
                    }
                    else if (line.StartsWith(DescriptionNamePrefix))
                    {
                        line = line.Remove(0, DescriptionNamePrefix.Length);
                        string[] splitLine = line.Split(new char[] { '=' }, 2);

                        if (!AbilityTalentNamesByReferenceNameId.ContainsKey(splitLine[0]))
                            AbilityTalentNamesByReferenceNameId.Add(splitLine[0], splitLine[1]);
                    }
                    else if (line.StartsWith(UnitPrefix))
                    {
                        line = line.Remove(0, UnitPrefix.Length);
                        string[] splitLine = line.Split(new char[] { '=' }, 2);

                        if (!UnitNamesByShortName.ContainsKey(splitLine[0]))
                            UnitNamesByShortName.Add(splitLine[0], splitLine[1]);
                    }
                }
            }
        }

        private void ParseNewHeroes()
        {
            foreach (var heroDirectory in Directory.GetDirectories(HeroModsPath))
            {
                ParseFiles(Path.Combine(heroDirectory, @"enus.stormdata\LocalizedData\GameStrings.txt"));
            }
        }
    }
}
