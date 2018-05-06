using System.Collections.Generic;
using System.IO;

namespace Heroes.Icons.Parser.Descriptions
{
    /// <summary>
    /// Loads all the tooltip descriptions from gamestrings text files
    /// </summary>
    public class DescriptionLoader
    {
        private readonly string SimpleDisplayPrefix = "Button/SimpleDisplayText/";
        private readonly string SimplePrefix = "Button/Simple/";
        private readonly string DescriptionPrefix = "Hero/Description/";
        private readonly string FullPrefix = "Button/Tooltip/";
        private readonly string HeroNamePrefix = "Hero/Name/"; // real name of hero
        private readonly string DescriptionNamePrefix = "Button/Name/"; // real name of ability/talent

        private string ModsFolderPath;
        private string OldDescriptionsPath;
        private string HeroModsPath;

        public DescriptionLoader(string modsFolderPath)
        {
            ModsFolderPath = modsFolderPath;
            OldDescriptionsPath = Path.Combine(modsFolderPath, @"heroesdata.stormmod\enus.stormdata\LocalizedData\GameStrings.txt");
            HeroModsPath = Path.Combine(modsFolderPath, "heromods");
        }

        /// <summary>
        /// Short tooltip descriptions of ability/talent
        /// </summary>
        public SortedDictionary<string, string> ShortDescriptions { get; set; } = new SortedDictionary<string, string>();

        /// <summary>
        /// The full tooltip descritions of ability/talent
        /// </summary>
        public SortedDictionary<string, string> FullDescriptions { get; set; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Hero description found on hero select screen
        /// </summary>
        public SortedDictionary<string, string> HeroDescriptions { get; set; } = new SortedDictionary<string, string>();

        /// <summary>
        /// The real name of heroes
        /// </summary>
        public SortedDictionary<string, string> HeroNames { get; set; } = new SortedDictionary<string, string>();

        /// <summary>
        /// The names of the ability/talents
        /// </summary>
        public SortedDictionary<string, string> DescriptionNames { get; set; } = new SortedDictionary<string, string>();

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
                        ShortDescriptions.Add(splitLine[0], splitLine[1]);
                    }
                    else if (line.StartsWith(SimplePrefix))
                    {
                        line = line.Remove(0, SimplePrefix.Length);
                        string[] splitLine = line.Split(new char[] { '=' }, 2);
                        ShortDescriptions.Add(splitLine[0], splitLine[1]);
                    }
                    else if (line.StartsWith(DescriptionPrefix))
                    {
                        line = line.Remove(0, DescriptionPrefix.Length);
                        string[] splitLine = line.Split(new char[] { '=' }, 2);
                        HeroDescriptions.Add(splitLine[0], splitLine[1]);
                    }
                    else if (line.StartsWith(FullPrefix))
                    {
                        line = line.Remove(0, FullPrefix.Length);
                        string[] splitLine = line.Split(new char[] { '=' }, 2);
                        FullDescriptions.Add(splitLine[0], splitLine[1]);
                    }
                    else if (line.StartsWith(HeroNamePrefix))
                    {
                        line = line.Remove(0, HeroNamePrefix.Length);
                        string[] splitLine = line.Split(new char[] { '=' }, 2);

                        if (!HeroNames.ContainsKey(splitLine[0]))
                            HeroNames.Add(splitLine[0], splitLine[1]);
                    }
                    else if (line.StartsWith(DescriptionNamePrefix))
                    {
                        line = line.Remove(0, DescriptionNamePrefix.Length);
                        string[] splitLine = line.Split(new char[] { '=' }, 2);

                        if (!DescriptionNames.ContainsKey(splitLine[0]))
                            DescriptionNames.Add(splitLine[0], splitLine[1]);
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
