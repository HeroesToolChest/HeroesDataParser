using CASCLib;
using Heroes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.ExtractorImage
{
    public class ImageHero : ImageExtractorBase<Hero>, IImage
    {
        private readonly HashSet<string> Portraits = new HashSet<string>();
        private readonly HashSet<string> Talents = new HashSet<string>();
        private readonly HashSet<string> Abilities = new HashSet<string>();
        private readonly HashSet<string> AbilityTalents = new HashSet<string>();

        private readonly string PortraitsDirectory = "heroportraits";
        private readonly string AbilitiesDirectory = "abilities";
        private readonly string TalentsDirectory = "talents";
        private readonly string AbilityTalentsDirectory = "abilitytalents";

        public ImageHero(CASCHandler cascHandler, string modsFolderPath)
            : base(cascHandler, modsFolderPath)
        {
        }

        protected override void LoadFileData(Hero hero)
        {
            if (!string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName))
                Portraits.Add(hero.HeroPortrait.HeroSelectPortraitFileName);
            if (!string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName))
                Portraits.Add(hero.HeroPortrait.LeaderboardPortraitFileName);
            if (!string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName))
                Portraits.Add(hero.HeroPortrait.LoadingScreenPortraitFileName);
            if (!string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName))
                Portraits.Add(hero.HeroPortrait.PartyPanelPortraitFileName);
            if (!string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName))
                Portraits.Add(hero.HeroPortrait.TargetPortraitFileName);

            LoadAbilityTalentFiles(hero);
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOption.HeroPortrait))
                ExtractPortraits();

            if (App.ExtractFileOption.HasFlag(ExtractImageOption.AbilityTalent))
            {
                ExtractAbilityTalentIcons();
            }
            else
            {
                if (App.ExtractFileOption.HasFlag(ExtractImageOption.Talent))
                    ExtractTalentIcons();
                if (App.ExtractFileOption.HasFlag(ExtractImageOption.Ability))
                    ExtractAbilityIcons();
            }
        }

        private void LoadAbilityTalentFiles(Hero hero)
        {
            foreach (string abilityIconFileName in hero.Abilities.Select(x => x.IconFileName))
            {
                if (!string.IsNullOrEmpty(abilityIconFileName))
                {
                    Abilities.Add(abilityIconFileName);
                    AbilityTalents.Add(abilityIconFileName);
                }
            }

            foreach (string talentIconFileName in hero.Talents.Select(x => x.IconFileName))
            {
                if (!string.IsNullOrEmpty(talentIconFileName))
                {
                    Talents.Add(talentIconFileName);
                    AbilityTalents.Add(talentIconFileName);
                }
            }
        }

        /// <summary>
        /// Extracts all portrait images.
        /// </summary>
        private void ExtractPortraits()
        {
            if (Portraits == null)
                return;

            int count = 0;
            Console.Write($"Extracting portrait files...{count}/{Portraits.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, PortraitsDirectory);

            foreach (string portrait in Portraits)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, portrait)))
                    count++;

                Console.Write($"\rExtracting portrait files...{count}/{Portraits.Count}");
            }

            Console.WriteLine(" Done.");
        }

        /// <summary>
        /// Extract ability icons.
        /// </summary>
        private void ExtractAbilityIcons()
        {
            if (Abilities == null)
                return;

            int count = 0;
            Console.Write($"Extracting ability icon files...{count}/{Abilities.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, AbilitiesDirectory);

            foreach (string ability in Abilities)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, ability)))
                    count++;

                Console.Write($"\rExtracting ability icon files...{count}/{Abilities.Count}");
            }

            Console.WriteLine(" Done.");
        }

        /// <summary>
        /// Extract talent icons.
        /// </summary>
        private void ExtractTalentIcons()
        {
            if (Talents == null)
                return;

            int count = 0;
            Console.Write($"Extracting talent icon files...{count}/{Talents.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, TalentsDirectory);

            foreach (string talent in Talents)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, talent)))
                    count++;

                Console.Write($"\rExtracting talent icon files...{count}/{Talents.Count}");
            }

            Console.WriteLine(" Done.");
        }

        /// <summary>
        /// Extract abilities and talent icons into the same output directory.
        /// </summary>
        private void ExtractAbilityTalentIcons()
        {
            if (AbilityTalents == null)
                return;

            int count = 0;
            Console.Write($"Extracting abilityTalent icon files...{count}/{AbilityTalents.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, AbilityTalentsDirectory);

            foreach (string abilityTalent in AbilityTalents)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, abilityTalent)))
                    count++;

                Console.Write($"\rExtracting abilityTalent icon files...{count}/{AbilityTalents.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
