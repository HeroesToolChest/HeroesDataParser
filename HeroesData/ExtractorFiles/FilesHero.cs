using CASCLib;
using Heroes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.ExtractorFiles
{
    public class FilesHero : FilesExtractorBase<Hero>, IFile
    {
        private readonly SortedSet<string> Portraits = new SortedSet<string>();
        private readonly SortedSet<string> Talents = new SortedSet<string>();
        private readonly SortedSet<string> Abilities = new SortedSet<string>();
        private readonly SortedSet<string> AbilityTalents = new SortedSet<string>();

        private readonly string PortraitsDirectory = "portraits";
        private readonly string AbilitiesDirectory = "abilities";
        private readonly string TalentsDirectory = "talents";
        private readonly string AbilityTalentsDirectory = "abilityTalents";

        public FilesHero(CASCHandler cascHandler, StorageMode storageMode)
            : base(cascHandler, storageMode)
        {
        }

        protected override void LoadFileData(Hero hero)
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

            LoadAbilityTalentFiles(hero);

            foreach (Hero heroUnit in hero.HeroUnits)
            {
                LoadAbilityTalentFiles(heroUnit);
            }
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractImagePortraits)
                ExtractPortraits();

            if (App.ExtractImageAbilityTalents)
            {
                ExtractAbilityTalentIcons();
            }
            else
            {
                if (App.ExtractImageTalents)
                    ExtractTalentIcons();
                if (App.ExtractImageAbilities)
                    ExtractAbilityIcons();
            }
        }

        private void LoadAbilityTalentFiles(Hero hero)
        {
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

        /// <summary>
        /// Extracts all portrait images.
        /// </summary>
        private void ExtractPortraits()
        {
            if (Portraits == null)
                return;

            Console.Write("Extracting portrait files...");

            string extractFilePath = Path.Combine(ExtractDirectory, PortraitsDirectory);

            foreach (string portrait in Portraits)
            {
                ExtractImageFile(extractFilePath, portrait);
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

            string extractFilePath = Path.Combine(ExtractDirectory, AbilitiesDirectory);

            foreach (string ability in Abilities)
            {
                ExtractImageFile(extractFilePath, ability);
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

            string extractFilePath = Path.Combine(ExtractDirectory, TalentsDirectory);

            foreach (string talent in Talents)
            {
                ExtractImageFile(extractFilePath, talent);
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

            string extractFilePath = Path.Combine(ExtractDirectory, AbilityTalentsDirectory);

            foreach (string abilityTalent in AbilityTalents)
            {
                ExtractImageFile(extractFilePath, abilityTalent);
            }

            Console.WriteLine("Done.");
        }
    }
}
