using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using HeroesData.FileWriter.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.FileWriter.Writer
{
    internal abstract class Writer<T, TU>
        where T : class
        where TU : class
    {
        public FileSettings FileSettings { get; set; }
        public string OutputDirectory { get; set; }
        public string Localization { get; set; }
        public bool IsLocalizedText { get; set; }
        public bool CreateLocalizedTextFile { get; set; }
        public bool CreateMinifiedFiles { get; set; }
        public int? HotsBuild { get; set; }
        public IEnumerable<Hero> Heroes { get; set; }
        public IEnumerable<MatchAward> MatchAwards { get; set; }
        public LocalizedGameString LocalizedGameString { get; } = new LocalizedGameString();

        protected string HeroDataSingleFileName { get; set; }
        protected string HeroDataSingleFileNameNoIndentation { get; set; }
        protected string MatchAwardSingleFileName { get; set; }
        protected string MatchAwardFileNameNoIndentation { get; set; }
        protected string XmlOutputFolder => Path.Combine(OutputDirectory, "xml");
        protected string JsonOutputFolder => Path.Combine(OutputDirectory, "json");
        protected string XmlOutputSplitFolder { get; set; }
        protected string JsonOutputSplitFolder { get; set; }
        protected string XmlOutputSplitMinFolder { get; set; }
        protected string JsonOutputSplitMinFolder { get; set; }
        protected string GameStringFolder { get; set; }
        protected string HeroDataRootNode => "Heroes";
        protected string HeroDataHeroUnits => "HeroUnits";
        protected string MultiFilesSubDirectoryHeroes => "heroes";
        protected string MultiFilesSubDirectoryAwards => "awards";
        protected string GameStringTextFileName { get; set; }

        public virtual void CreateOutput()
        {
            if (FileSettings.IsWriterEnabled)
            {
                if (FileSettings.IsFileSplit)
                {
                    CreateHeroDataMultipleFiles();
                    CreateMatchAwardMultipleFiles();
                }
                else
                {
                    CreateHeroDataSingleFile();
                    CreateMatchAwardSingleFile();
                }
            }

            if (IsLocalizedText && CreateLocalizedTextFile)
            {
                if (HotsBuild.HasValue)
                {
                    GameStringFolder = Path.Combine(OutputDirectory, $"gamestrings-{HotsBuild.Value}");
                    GameStringTextFileName = $"gamestrings_{HotsBuild.Value}_{Localization}.txt";
                }
                else
                {
                    GameStringFolder = Path.Combine(OutputDirectory, $"gamestrings");
                    GameStringTextFileName = $"gamestrings_{Localization}.txt";
                }

                Directory.CreateDirectory(GameStringFolder);
                DeleteExistingGameStrings();
                CreateGameStringTextFile();
            }
        }

        protected virtual void SetSingleFileName(string fileExtension)
        {
            if (HotsBuild.HasValue)
            {
                HeroDataSingleFileName = $"heroesdata_{HotsBuild.Value}_{Localization}.{fileExtension}";
                HeroDataSingleFileNameNoIndentation = $"heroesdata_{HotsBuild.Value}_{Localization}.min.{fileExtension}";
                MatchAwardSingleFileName = $"awards_{HotsBuild.Value}_{Localization}.{fileExtension}";
                MatchAwardFileNameNoIndentation = $"awards_{HotsBuild.Value}_{Localization}.min.{fileExtension}";
            }
            else
            {
                HeroDataSingleFileName = $"heroesdata_{Localization}.{fileExtension}";
                HeroDataSingleFileNameNoIndentation = $"heroesdata_{Localization}.min.{fileExtension}";
                MatchAwardSingleFileName = $"awards_{Localization}.{fileExtension}";
                MatchAwardFileNameNoIndentation = $"awards_{Localization}.min.{fileExtension}";
            }
        }

        protected virtual void SetMultipleFileFolderNames(string outFolder, ref string splitFolder, ref string minFolder)
        {
            if (HotsBuild.HasValue)
            {
                splitFolder = Path.Combine(outFolder, $"splitfiles-{HotsBuild.Value}-{Localization}");
                minFolder = Path.Combine(outFolder, $"splitfiles-{HotsBuild.Value}-{Localization}.min");
            }
            else
            {
                splitFolder = Path.Combine(outFolder, $"splitfiles-{Localization}");
                minFolder = Path.Combine(outFolder, $"splitfiles-{Localization}");
            }
        }

        protected virtual void CreateGameStringTextFile()
        {
            List<string> gameStrings = LocalizedGameString.GameStrings.ToList();
            gameStrings.Sort();

            using (StreamWriter writer = new StreamWriter(Path.Combine(GameStringFolder, GameStringTextFileName)))
            {
                foreach (string item in gameStrings)
                {
                    writer.WriteLine(item);
                }
            }
        }

        protected abstract void CreateSingleFile<TObject>(IEnumerable<TObject> items, string rootNodeName, string singleFileName, string noIndentationName, Func<TObject, T> dataMethod)
            where TObject : IName;
        protected abstract void CreateMultipleFiles<TObject>(IEnumerable<TObject> items, string rootNodeName, string subDirectory, Func<TObject, T> dataMethod)
            where TObject : IName;

        protected abstract void CreateHeroDataMultipleFiles();
        protected abstract void CreateMatchAwardMultipleFiles();
        protected abstract void CreateHeroDataSingleFile();
        protected abstract void CreateMatchAwardSingleFile();
        protected abstract T HeroElement(Hero hero);
        protected abstract T UnitElement(Unit unit);
        protected abstract T GetPortraitObject(Hero hero);
        protected abstract T GetArmorObject(Unit unit);
        protected abstract T GetLifeObject(Unit unit);
        protected abstract T GetEnergyObject(Unit unit);
        protected abstract T GetRatingsObject(Hero hero);
        protected abstract T GetWeaponsObject(Unit unit);
        protected abstract T GetAbilitiesObject(Unit unit, bool isUnitAbilities);
        protected abstract T GetSubAbilitiesObject(ILookup<string, Ability> linkedAbilities);
        protected abstract T GetTalentsObject(Hero hero);
        protected abstract T GetUnitsObject(Hero hero);
        protected abstract TU AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase);
        protected abstract TU TalentInfoElement(Talent talent);
        protected abstract T GetAbilityTalentLifeCostObject(TooltipLife tooltipLife);
        protected abstract T GetAbilityTalentEnergyCostObject(TooltipEnergy tooltipEnergy);
        protected abstract T GetAbilityTalentCooldownObject(TooltipCooldown tooltipCooldown);
        protected abstract T GetAbilityTalentChargesObject(TooltipCharges tooltipCharges);
        protected abstract T AwardElement(MatchAward matchAward);

        protected T HeroPortraits(Hero hero)
        {
            if ((FileSettings.HeroSelectPortrait || FileSettings.LeaderboardPortrait ||
                FileSettings.LoadingPortraitPortrait || FileSettings.PartyPanelPortrait ||
                FileSettings.TargetPortrait) &&
                (!string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName) || !string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName) ||
                !string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName) || !string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName) ||
                !string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName)) && hero.HeroPortrait != null)
            {
                return GetPortraitObject(hero);
            }

            return null;
        }

        protected T UnitArmor(Unit unit)
        {
            if (unit.Armor != null)
            {
                return GetArmorObject(unit);
            }

            return null;
        }

        protected T UnitLife(Unit unit)
        {
            if (unit.Life.LifeMax > 0)
            {
                return GetLifeObject(unit);
            }

            return null;
        }

        protected T UnitEnergy(Unit unit)
        {
            if (unit.Energy.EnergyMax > 0)
            {
                return GetEnergyObject(unit);
            }

            return null;
        }

        protected T HeroRatings(Hero hero)
        {
            if (hero.Ratings != null)
            {
                return GetRatingsObject(hero);
            }

            return null;
        }

        protected T UnitWeapons(Unit unit)
        {
            if (FileSettings.IncludeWeapons && unit.Weapons?.Count > 0)
            {
                return GetWeaponsObject(unit);
            }

            return null;
        }

        protected T UnitAbilities(Unit unit, bool isSubAbilities)
        {
            if (FileSettings.IncludeAbilities && unit.Abilities?.Count > 0)
            {
                return GetAbilitiesObject(unit, isSubAbilities);
            }

            return null;
        }

        protected T UnitSubAbilities(Unit unit)
        {
            if (FileSettings.IncludeSubAbilities && unit.Abilities?.Count > 0)
            {
                ILookup<string, Ability> linkedAbilities = unit.ParentLinkedAbilities();
                if (linkedAbilities.Count > 0)
                {
                    return GetSubAbilitiesObject(linkedAbilities);
                }
            }

            return null;
        }

        protected T UnitAbilityTalentLifeCost(TooltipLife tooltipLife)
        {
            if (!string.IsNullOrEmpty(tooltipLife?.LifeCostTooltip?.RawDescription) && !IsLocalizedText)
            {
                return GetAbilityTalentLifeCostObject(tooltipLife);
            }

            return null;
        }

        protected T UnitAbilityTalentEnergyCost(TooltipEnergy tooltipEnergy)
        {
            if (!string.IsNullOrEmpty(tooltipEnergy?.EnergyTooltip?.RawDescription) && !IsLocalizedText)
            {
                return GetAbilityTalentEnergyCostObject(tooltipEnergy);
            }

            return null;
        }

        protected T UnitAbilityTalentCooldown(TooltipCooldown tooltipCooldown)
        {
            if (!string.IsNullOrEmpty(tooltipCooldown?.CooldownTooltip?.RawDescription) && !IsLocalizedText)
            {
                return GetAbilityTalentCooldownObject(tooltipCooldown);
            }

            return null;
        }

        protected T UnitAbilityTalentCharges(TooltipCharges tooltipCharges)
        {
            if (tooltipCharges.HasCharges)
            {
                return GetAbilityTalentChargesObject(tooltipCharges);
            }

            return null;
        }

        protected T HeroTalents(Hero hero)
        {
            if (FileSettings.IncludeTalents && hero.Talents?.Count > 0)
            {
                return GetTalentsObject(hero);
            }

            return null;
        }

        protected T Units(Hero hero)
        {
            if (FileSettings.IncludeHeroUnits && hero.HeroUnits?.Count > 0)
            {
                return GetUnitsObject(hero);
            }

            return null;
        }

        protected string GetTooltip(TooltipDescription tooltipDescription, int setting)
        {
            if (tooltipDescription == null)
                return string.Empty;

            if (setting == 0)
                return tooltipDescription.RawDescription;
            else if (setting == 1)
                return tooltipDescription.PlainText;
            else if (setting == 2)
                return tooltipDescription.PlainTextWithNewlines;
            else if (setting == 3)
                return tooltipDescription.PlainTextWithScaling;
            else if (setting == 4)
                return tooltipDescription.PlainTextWithScalingWithNewlines;
            else if (setting == 6)
                return tooltipDescription.ColoredTextWithScaling;
            else
                return tooltipDescription.ColoredText;
        }

        /// <summary>
        /// Adds the gamestrings to to a localized text file.
        /// </summary>
        /// <param name="abilityTalentBase"></param>
        protected void AddAbilityTalentGameStrings(AbilityTalentBase abilityTalentBase)
        {
            LocalizedGameString.AddAbilityTalentName(abilityTalentBase.ReferenceNameId, abilityTalentBase.Name);

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.Life?.LifeCostTooltip?.RawDescription))
                LocalizedGameString.AddAbilityTalentLifeTooltip(abilityTalentBase.ReferenceNameId, GetTooltip(abilityTalentBase.Tooltip.Life.LifeCostTooltip, FileSettings.Description));

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.Energy?.EnergyTooltip?.RawDescription))
                LocalizedGameString.AddAbilityTalentEnergyTooltip(abilityTalentBase.ReferenceNameId, GetTooltip(abilityTalentBase.Tooltip.Energy.EnergyTooltip, FileSettings.Description));

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.Cooldown?.CooldownTooltip?.RawDescription))
                LocalizedGameString.AddAbilityTalentCooldownTooltip(abilityTalentBase.ReferenceNameId, GetTooltip(abilityTalentBase.Tooltip.Cooldown.CooldownTooltip, FileSettings.Description));

            LocalizedGameString.AddAbilityTalentShortTooltip(abilityTalentBase.ShortTooltipNameId, GetTooltip(abilityTalentBase.Tooltip.ShortTooltip, FileSettings.Description));
            LocalizedGameString.AddAbilityTalentFullTooltip(abilityTalentBase.FullTooltipNameId, GetTooltip(abilityTalentBase.Tooltip.FullTooltip, FileSettings.Description));
        }

        protected void AddUnitGameStrings(Unit unit)
        {
            LocalizedGameString.AddUnitName(unit.ShortName, unit.Name);
            LocalizedGameString.AddUnitType(unit.ShortName, unit.Type);

            string unitDescription = GetTooltip(unit.Description, FileSettings.Description);
            if (!string.IsNullOrEmpty(unitDescription))
                LocalizedGameString.AddUnitDescription(unit.ShortName, unitDescription);
        }

        protected void AddHeroGameStrings(Hero hero)
        {
            LocalizedGameString.AddUnitName(hero.ShortName, hero.Name);
            LocalizedGameString.AddUnitDifficulty(hero.ShortName, hero.Difficulty);
            LocalizedGameString.AddUnitType(hero.ShortName, hero.Type);
            LocalizedGameString.AddUnitDescription(hero.ShortName, GetTooltip(hero.Description, FileSettings.Description));
            LocalizedGameString.AddHeroTitle(hero.ShortName, hero.Title);
            LocalizedGameString.AddHeroSearchText(hero.ShortName, hero.Title);

            if (hero.Roles != null && hero.Roles.Count > 0)
                LocalizedGameString.AddUnitRole(hero.ShortName, string.Join(",", hero.Roles));
        }

        protected void AddMatchAwardGameStrings(MatchAward matchAward)
        {
            LocalizedGameString.AddMatchAwardName(matchAward.ShortName, matchAward.Name);
            LocalizedGameString.AddMatchAwardDescription(matchAward.ShortName, GetTooltip(matchAward.Description, FileSettings.Description));
        }

        private void DeleteExistingGameStrings()
        {
            string filePath = Path.Combine(GameStringFolder, GameStringTextFileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
