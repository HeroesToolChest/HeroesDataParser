using CASCLib;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.Parser.GameStrings
{
    public abstract class GameStringData
    {
        private readonly string SimpleDisplayPrefix = "Button/SimpleDisplayText/";
        private readonly string SimplePrefix = "Button/Simple/";
        private readonly string DescriptionPrefix = "Hero/Description/";
        private readonly string FullPrefix = "Button/Tooltip/";
        private readonly string HeroNamePrefix = "Hero/Name/"; // real name of hero
        private readonly string DescriptionNamePrefix = "Button/Name/"; // real name of ability/talent
        private readonly string UnitPrefix = "Unit/Name/";

        protected GameStringData(string modsFolderPath)
        {
            ModsFolderPath = modsFolderPath;
        }

        protected GameStringData(string modsFolderPath, int? hotsBuild)
        {
            ModsFolderPath = modsFolderPath;
            HotsBuild = hotsBuild;
        }

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

        protected int? HotsBuild { get; }
        protected string ModsFolderPath { get; }
        protected string OldDescriptionsPath { get; private set; }
        protected string HeroModsPath { get; private set; }
        protected string GameStringFile => "GameStrings.txt";

        /// <summary>
        /// Loads all the required games strings.
        /// </summary>
        /// <param name="modsFolderPath">The file path of the mods folder.</param>
        /// <returns></returns>
        public static GameStringData Load(string modsFolderPath)
        {
            return new FileGameStringData(modsFolderPath);
        }

        /// <summary>
        /// Loads all the required games strings.
        /// </summary>
        /// <param name="modsFolderPath">The file path of the mods folder.</param>
        /// <param name="hotsBuild">The hots build number.</param>
        /// <returns></returns>
        public static GameStringData Load(string modsFolderPath, int? hotsBuild)
        {
            return new FileGameStringData(modsFolderPath, hotsBuild);
        }

        /// <summary>
        /// Loads all the required games strings.
        /// </summary>
        /// <param name="cascHandler"></param>
        /// <param name="cascFolder"></param>
        /// <param name="modsFolderPath">The root folder of the heroes data.</param>
        /// <returns></returns>
        public static GameStringData Load(CASCHandler cascHandler, CASCFolder cascFolder, string modsFolderPath = "mods")
        {
            return new CASCGameStringData(cascHandler, cascFolder, modsFolderPath);
        }

        /// <summary>
        /// Loads all the required games strings.
        /// </summary>
        /// <param name="cascHandler"></param>
        /// <param name="cascFolder"></param>
        /// <param name="hotsBuild">The hots build number.</param>
        /// <param name="modsFolderPath">The root folder of the heroes data.</param>
        /// <returns></returns>
        public static GameStringData Load(CASCHandler cascHandler, CASCFolder cascFolder, int? hotsBuild, string modsFolderPath = "mods")
        {
            return new CASCGameStringData(cascHandler, cascFolder, modsFolderPath, hotsBuild);
        }

        protected void Initialize()
        {
            OldDescriptionsPath = Path.Combine(ModsFolderPath, "heroesdata.stormmod", "enus.stormdata", "LocalizedData");
            HeroModsPath = Path.Combine(ModsFolderPath, "heromods");

            ParseGameStringFiles();
        }

        protected abstract void ParseGameStringFiles();
        protected abstract void ParseNewHeroes();

        protected void ReadFile(StreamReader reader)
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
}
