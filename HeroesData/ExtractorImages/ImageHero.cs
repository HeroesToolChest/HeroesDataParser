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
        private readonly HashSet<string> _portraits = new HashSet<string>();
        private readonly HashSet<string> _talents = new HashSet<string>();
        private readonly HashSet<string> _abilities = new HashSet<string>();
        private readonly HashSet<string> _abilityTalents = new HashSet<string>();

        private readonly string _portraitsDirectory = "heroportraits";
        private readonly string _abilitiesDirectory = "abilities";
        private readonly string _talentsDirectory = "talents";
        private readonly string _abilityTalentsDirectory = "abilitytalents";

        public ImageHero(CASCHandler? cascHandler, string modsFolderPath)
            : base(cascHandler, modsFolderPath)
        {
        }

        protected override void LoadFileData(Hero hero)
        {
            if (hero is null)
                throw new ArgumentNullException(nameof(hero));

            if (!string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName))
                _portraits.Add(hero.HeroPortrait.HeroSelectPortraitFileName);
            if (!string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName))
                _portraits.Add(hero.HeroPortrait.LeaderboardPortraitFileName);
            if (!string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName))
                _portraits.Add(hero.HeroPortrait.LoadingScreenPortraitFileName);
            if (!string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName))
                _portraits.Add(hero.HeroPortrait.PartyPanelPortraitFileName);
            if (!string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName))
                _portraits.Add(hero.HeroPortrait.TargetPortraitFileName);
            if (!string.IsNullOrEmpty(hero.HeroPortrait.DraftScreenFileName))
                _portraits.Add(hero.HeroPortrait.DraftScreenFileName);
            if (!string.IsNullOrEmpty(hero.UnitPortrait.MiniMapIconFileName))
                _portraits.Add(hero.UnitPortrait.MiniMapIconFileName);
            if (!string.IsNullOrEmpty(hero.UnitPortrait.TargetInfoPanelFileName))
                _portraits.Add(hero.UnitPortrait.TargetInfoPanelFileName);
            if (hero.HeroPortrait.PartyFrameFileName.Count > 0)
            {
                foreach (string partyFrame in hero.HeroPortrait.PartyFrameFileName)
                {
                    _portraits.Add(partyFrame);
                }
            }

            LoadAbilityTalentFiles(hero);

            foreach (Hero heroUnit in hero.HeroUnits)
            {
                LoadAbilityTalentFiles(heroUnit);
            }
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOptions.HeroPortrait))
                ExtractPortraits();

            if (App.ExtractFileOption.HasFlag(ExtractImageOptions.AbilityTalent))
            {
                ExtractAbilityTalentIcons();
            }
            else
            {
                if (App.ExtractFileOption.HasFlag(ExtractImageOptions.Talent))
                    ExtractTalentIcons();
                if (App.ExtractFileOption.HasFlag(ExtractImageOptions.Ability))
                    ExtractAbilityIcons();
            }
        }

        private void LoadAbilityTalentFiles(Hero hero)
        {
            foreach (string? abilityIconFileName in hero.Abilities.Select(x => x.IconFileName))
            {
                if (!string.IsNullOrEmpty(abilityIconFileName))
                {
                    _abilities.Add(abilityIconFileName);
                    _abilityTalents.Add(abilityIconFileName);
                }
            }

            foreach (string? talentIconFileName in hero.Talents.Select(x => x.IconFileName))
            {
                if (!string.IsNullOrEmpty(talentIconFileName))
                {
                    _talents.Add(talentIconFileName);
                    _abilityTalents.Add(talentIconFileName);
                }
            }
        }

        /// <summary>
        /// Extracts all portrait images.
        /// </summary>
        private void ExtractPortraits()
        {
            if (_portraits == null)
                return;

            int count = 0;
            Console.Write($"Extracting portrait files...{count}/{_portraits.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, _portraitsDirectory);

            foreach (string portrait in _portraits)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, portrait)))
                    count++;

                Console.Write($"\rExtracting portrait files...{count}/{_portraits.Count}");
            }

            Console.WriteLine(" Done.");
        }

        /// <summary>
        /// Extract ability icons.
        /// </summary>
        private void ExtractAbilityIcons()
        {
            if (_abilities == null)
                return;

            int count = 0;
            Console.Write($"Extracting ability icon files...{count}/{_abilities.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, _abilitiesDirectory);

            foreach (string ability in _abilities)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, ability)))
                    count++;

                Console.Write($"\rExtracting ability icon files...{count}/{_abilities.Count}");
            }

            Console.WriteLine(" Done.");
        }

        /// <summary>
        /// Extract talent icons.
        /// </summary>
        private void ExtractTalentIcons()
        {
            if (_talents == null)
                return;

            int count = 0;
            Console.Write($"Extracting talent icon files...{count}/{_talents.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, _talentsDirectory);

            foreach (string talent in _talents)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, talent)))
                    count++;

                Console.Write($"\rExtracting talent icon files...{count}/{_talents.Count}");
            }

            Console.WriteLine(" Done.");
        }

        /// <summary>
        /// Extract abilities and talent icons into the same output directory.
        /// </summary>
        private void ExtractAbilityTalentIcons()
        {
            if (_abilityTalents == null)
                return;

            int count = 0;
            Console.Write($"Extracting abilityTalent icon files...{count}/{_abilityTalents.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, _abilityTalentsDirectory);

            foreach (string abilityTalent in _abilityTalents)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, abilityTalent)))
                    count++;

                Console.Write($"\rExtracting abilityTalent icon files...{count}/{_abilityTalents.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
