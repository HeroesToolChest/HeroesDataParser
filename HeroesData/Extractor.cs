﻿using CASCLib;
using DDSReader;
using Heroes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HeroesData
{
    internal class Extractor
    {
        private readonly string CASCTexturesPath = Path.Combine("mods", "heroes.stormmod", "base.stormassets", "Assets", "Textures");

        private readonly IEnumerable<Hero> Heroes;
        private readonly CASCHandler CASCHandler;
        private SortedSet<string> Portraits = new SortedSet<string>();
        private SortedSet<string> Talents = new SortedSet<string>();
        private SortedSet<string> Abilities = new SortedSet<string>();
        private SortedSet<string> AbilityTalents = new SortedSet<string>();

        public Extractor(IEnumerable<Hero> heroes, CASCHandler cascHandler)
        {
            Heroes = heroes;
            CASCHandler = cascHandler;
            Initialize();
        }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        public string OutputDirectory { get; set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "output");

        /// <summary>
        /// Extracts a file.
        /// </summary>
        /// <param name="path">The path to extract the file to.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        public void ExtractFile(string path, string fileName)
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
                Console.WriteLine($"Error extracting file: {fileName}");
                Console.WriteLine($"--> {ex.Message}");
            }
        }

        /// <summary>
        /// Extracts all portrait images.
        /// </summary>
        public void ExtractPortraits()
        {
            Console.Write("Extracting portrait files...");

            string extractFilePath = Path.Combine(OutputDirectory, "portraits");

            foreach (string portrait in Portraits)
            {
                ExtractFile(extractFilePath, portrait);
            }

            Console.WriteLine("Done.");
        }

        /// <summary>
        /// Extract ability icons.
        /// </summary>
        public void ExtractAbilityIcons()
        {
            Console.Write("Extracting ability icon files...");

            string extractFilePath = Path.Combine(OutputDirectory, "abilities");

            foreach (string ability in Abilities)
            {
                ExtractFile(extractFilePath, ability);
            }

            Console.WriteLine("Done.");
        }

        /// <summary>
        /// Extract talent icons.
        /// </summary>
        public void ExtractTalentIcons()
        {
            Console.Write("Extracting talent icon files...");

            string extractFilePath = Path.Combine(OutputDirectory, "talents");

            foreach (string talent in Talents)
            {
                ExtractFile(extractFilePath, talent);
            }

            Console.WriteLine("Done.");
        }

        /// <summary>
        /// Extract abilities and talent icons into the same output directory.
        /// </summary>
        public void ExtractAbilityTalentIcons()
        {
            Console.Write("Extracting abilityTalent icon files...");

            string extractFilePath = Path.Combine(OutputDirectory, "abilityTalents");

            foreach (string abilityTalent in AbilityTalents)
            {
                ExtractFile(extractFilePath, abilityTalent);
            }

            Console.WriteLine("Done.");
        }

        private void Initialize()
        {
            foreach (Hero hero in Heroes)
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
        }
    }
}
