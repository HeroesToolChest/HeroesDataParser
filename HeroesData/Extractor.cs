using CASCLib;
using DDSReader;
using Heroes.Models;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData
{
    internal class Extractor
    {
        private readonly CASCHandler CASCHandler;
        private readonly StorageMode StorageMode;

        private readonly string CASCTexturesPath = Path.Combine("mods", "heroes.stormmod", "base.stormassets", "assets", "textures");
        private readonly string ImagesDirectory = "images";
        private readonly string PortraitsDirectory = "portraits";
        private readonly string AbilitiesDirectory = "abilities";
        private readonly string TalentsDirectory = "talents";
        private readonly string AbilityTalentsDirectory = "abilityTalents";
        private readonly string MatchAwardsDirectory = "matchAwards";

        private string _outputDirectory;

        private SortedSet<string> Portraits = new SortedSet<string>();
        private SortedSet<string> Talents = new SortedSet<string>();
        private SortedSet<string> Abilities = new SortedSet<string>();
        private SortedSet<string> AbilityTalents = new SortedSet<string>();
        private SortedSet<(string OriginalName, string NewName)> Awards = new SortedSet<(string Original, string NewName)>();

        public Extractor(CASCHandler cascHandler, StorageMode storageMode)
        {
            CASCHandler = cascHandler;
            StorageMode = storageMode;
        }

        /// <summary>
        /// Gets or sets the parsed hero data.
        /// </summary>
        public IEnumerable<Hero> ParsedHeroData { get; set; } = new List<Hero>();

        /// <summary>
        /// Gets or sets the parsed match award data.
        /// </summary>
        public IEnumerable<MatchAward> ParsedMatchAwardData { get; set; } = new List<MatchAward>();

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        public string OutputDirectory
        {
            get => _outputDirectory;
            set => _outputDirectory = Path.Combine(value, ImagesDirectory);
        }

        public bool ExtractImagePortraits { get; set; }
        public bool ExtractImageAbilityTalents { get; set; }
        public bool ExtractImageTalents { get; set; }
        public bool ExtractImageAbilities { get; set; }
        public bool ExtractMatchAwards { get; set; }

        public void ExtractFiles(string outputDirectory)
        {
            if (!ExtractImagePortraits && !ExtractImageAbilityTalents && !ExtractImageTalents && !ExtractImageAbilities && !ExtractMatchAwards)
                return;

            LoadImageFileNames();

            if (StorageMode != StorageMode.CASC || string.IsNullOrEmpty(outputDirectory))
                return;

            if (ExtractImagePortraits)
                ExtractPortraits();
            if (ExtractMatchAwards)
                ExtractMatchAwardIcons();

            if (ExtractImageAbilityTalents)
            {
                ExtractAbilityTalentIcons();
            }
            else
            {
                if (ExtractImageTalents)
                    ExtractTalentIcons();
                if (ExtractImageAbilities)
                    ExtractAbilityIcons();
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Extracts all portrait images.
        /// </summary>
        private void ExtractPortraits()
        {
            if (Portraits == null)
                return;

            Console.Write("Extracting portrait files...");

            string extractFilePath = Path.Combine(OutputDirectory, PortraitsDirectory);

            foreach (string portrait in Portraits)
            {
                ExtractFile(extractFilePath, portrait);
            }

            Console.WriteLine("Done.");
        }

        /// <summary>
        /// Extract ability icons.
        /// </summary>
        private void ExtractAbilityIcons()
        {
            if (Abilities == null)
                return;

            Console.Write("Extracting ability icon files...");

            string extractFilePath = Path.Combine(OutputDirectory, AbilitiesDirectory);

            foreach (string ability in Abilities)
            {
                ExtractFile(extractFilePath, ability);
            }

            Console.WriteLine("Done.");
        }

        /// <summary>
        /// Extract talent icons.
        /// </summary>
        private void ExtractTalentIcons()
        {
            if (Talents == null)
                return;

            Console.Write("Extracting talent icon files...");

            string extractFilePath = Path.Combine(OutputDirectory, TalentsDirectory);

            foreach (string talent in Talents)
            {
                ExtractFile(extractFilePath, talent);
            }

            Console.WriteLine("Done.");
        }

        /// <summary>
        /// Extract abilities and talent icons into the same output directory.
        /// </summary>
        private void ExtractAbilityTalentIcons()
        {
            if (AbilityTalents == null)
                return;

            Console.Write("Extracting abilityTalent icon files...");

            string extractFilePath = Path.Combine(OutputDirectory, AbilityTalentsDirectory);

            foreach (string abilityTalent in AbilityTalents)
            {
                ExtractFile(extractFilePath, abilityTalent);
            }

            Console.WriteLine("Done.");
        }

        private void ExtractMatchAwardIcons()
        {
            if (Awards == null)
                return;

            Console.Write("Extracting match award icon files...");

            string extractFilePath = Path.Combine(OutputDirectory, MatchAwardsDirectory);

            foreach ((string originalName, string newName) in Awards)
            {
                if (originalName.StartsWith("storm_ui_mvp_icons_rewards_"))
                {
                    ExtractMVPAwardFile(extractFilePath, originalName, newName);
                }
                else
                {
                    ExtractScoreAwardFile(extractFilePath, originalName, newName, "red");
                    ExtractScoreAwardFile(extractFilePath, originalName, newName, "blue");
                }
            }

            Console.WriteLine("Done.");
        }

        private void LoadImageFileNames()
        {
            foreach (Hero hero in ParsedHeroData)
            {
                if (!string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName))
                    Portraits.Add(hero.HeroPortrait.HeroSelectPortraitFileName.ToLower());
                if (!string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName))
                    Portraits.Add(hero.HeroPortrait.LeaderboardPortraitFileName.ToLower());
                if (!string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName))
                    Portraits.Add(hero.HeroPortrait.LoadingScreenPortraitFileName.ToLower());
                if (!string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName))
                    Portraits.Add(hero.HeroPortrait.PartyPanelPortraitFileName.ToLower());
                if (!string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName))
                    Portraits.Add(hero.HeroPortrait.TargetPortraitFileName.ToLower());

                foreach (string abilityIconFileName in hero.Abilities.Select(x => x.Value.IconFileName))
                {
                    if (!string.IsNullOrEmpty(abilityIconFileName))
                    {
                        Abilities.Add(abilityIconFileName.ToLower());
                        AbilityTalents.Add(abilityIconFileName.ToLower());
                    }
                }

                foreach (string talentIconFileName in hero.Talents.Select(x => x.Value.IconFileName))
                {
                    if (!string.IsNullOrEmpty(talentIconFileName))
                    {
                        Talents.Add(talentIconFileName.ToLower());
                        AbilityTalents.Add(talentIconFileName.ToLower());
                    }
                }
            }

            foreach (MatchAward matchAward in ParsedMatchAwardData)
            {
                if (!string.IsNullOrEmpty(matchAward.MVPScreenImageFileNameOriginal))
                    Awards.Add((matchAward.MVPScreenImageFileNameOriginal.ToLower(), matchAward.MVPScreenImageFileName.ToLower()));
                if (!string.IsNullOrEmpty(matchAward.ScoreScreenImageFileNameOriginal))
                    Awards.Add((matchAward.ScoreScreenImageFileNameOriginal.ToLower(), matchAward.ScoreScreenImageFileName.ToLower()));
            }
        }

        /// <summary>
        /// Extracts a file.
        /// </summary>
        /// <param name="path">The path to extract the file to.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        private void ExtractFile(string path, string fileName)
        {
            Directory.CreateDirectory(path);

            try
            {
                string cascFilepath = Path.Combine(CASCTexturesPath, fileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));
                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(fileName)}.png"));
                }
                else
                {
                    Console.WriteLine($"CASC file not found: {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"Error extracting file: {fileName}");
                Console.WriteLine($"--> {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Extracts a score screen match award file.
        /// </summary>
        /// <param name="path">The path to extract the file to.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        /// <param name="newFileName">The new file name of the award.</param>
        /// <param name="color">The color of the award.</param>
        private void ExtractScoreAwardFile(string path, string fileName, string newFileName, string color)
        {
            Directory.CreateDirectory(path);

            try
            {
                fileName = fileName.Replace("%team%", color);
                string cascFilepath = Path.Combine(CASCTexturesPath, fileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));

                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%team%", color))}.png"));
                }
                else
                {
                    Console.WriteLine($"CASC file not found: {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"Error extracting file: {fileName}");
                Console.WriteLine($"--> {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Extracts a MVP match award file.
        /// </summary>
        /// <param name="path">The path to extract the file to.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        /// <param name="newFileName">The new file name of the award.</param>
        private void ExtractMVPAwardFile(string path, string fileName, string newFileName)
        {
            Directory.CreateDirectory(path);

            try
            {
                string cascFilepath = Path.Combine(CASCTexturesPath, fileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));

                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%color%", "blue"))}.png"), new Point(0, 0), new Size(148, 148));
                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%color%", "red"))}.png"), new Point(148, 0), new Size(148, 148));
                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%color%", "gold"))}.png"), new Point(296, 0), new Size(148, 148));
                }
                else
                {
                    Console.WriteLine($"CASC file not found: {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"Error extracting file: {fileName}");
                Console.WriteLine($"--> {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
