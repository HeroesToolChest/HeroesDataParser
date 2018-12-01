using Heroes.Models;
using Heroes.Models.AbilityTalents;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HeroesData.Parser
{
    public class VerifyData
    {
        private readonly string VerifyIgnoreFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "VerifyIgnore.txt");

        private string WarningId;

        private HashSet<string> IgnoreLines = new HashSet<string>();

        public VerifyData()
        {
            ReadIgnoreFile();
        }

        /// <summary>
        /// Gets or sets the parsed hero data.
        /// </summary>
        public IEnumerable<Hero> ParsedHeroData { get; set; } = new List<Hero>();

        /// <summary>
        /// Gets or sets the parsed match award data.
        /// </summary>
        public IEnumerable<MatchAward> ParsedMatchAwardData { get; set; } = new List<MatchAward>();

        public HashSet<string> Warnings { get; private set; } = new HashSet<string>();
        public int WarningsIgnored { get; private set; } = 0;

        public void PerformVerification()
        {
            VerifyHeroData();
            VerifyMatchAwardData();
        }

        private void VerifyHeroData()
        {
            foreach (Hero hero in ParsedHeroData)
            {
                WarningId = hero.Name;

                if (string.IsNullOrEmpty(hero.AttributeId))
                    AddWarning($"{nameof(hero.AttributeId)} is null or empty");

                if (string.IsNullOrEmpty(hero.Description?.RawDescription))
                    AddWarning($"{nameof(hero.Description)} is null or empty");

                if (string.IsNullOrEmpty(hero.Difficulty))
                    AddWarning($"{nameof(hero.Difficulty)} is Unknown");

                if (hero.Franchise == HeroFranchise.Unknown)
                    AddWarning($"{nameof(hero.Franchise)} is Unknown");

                if (hero.Roles.Contains("Unknown"))
                    AddWarning($"{nameof(hero.Roles)} is Unknown");

                if (hero.Abilities.Count < 1)
                    AddWarning("Hero has no abilities");

                if (hero.Talents.Count < 1)
                    AddWarning("Hero has no talents");

                if (hero.Life.LifeMax <= 0)
                    AddWarning($"{nameof(hero.Life)} is 0");

                if (hero.Life.LifeScaling <= 0)
                    AddWarning($"{nameof(hero.Life.LifeScaling)} is 0");

                if (hero.Life.LifeRegenerationRateScaling <= 0)
                    AddWarning($"{nameof(hero.Life.LifeRegenerationRateScaling)} is 0");

                if (hero.Energy.EnergyMax > 0 && string.IsNullOrEmpty(hero.Energy.EnergyType))
                    AddWarning($"{nameof(hero.Energy)} > 0 and {nameof(hero.Energy.EnergyType)} is NONE");

                if (hero.Armor != null && hero.Armor.PhysicalArmor < 1 && hero.Armor.SpellArmor < 1)
                    AddWarning($"{nameof(hero.Armor.PhysicalArmor)} and {nameof(hero.Armor.SpellArmor)} are both 0");

                if (hero.Sight <= 0)
                    AddWarning($"{nameof(hero.Sight)} is 0");

                if (hero.Speed <= 0)
                    AddWarning($"{nameof(hero.Speed)} is 0");

                if (hero.Rarity == HeroRarity.None)
                    AddWarning($"{nameof(hero.Rarity)} is None");

                if (!hero.ReleaseDate.HasValue)
                    AddWarning($"{nameof(hero.ReleaseDate)} is null");

                if (string.IsNullOrEmpty(hero.Type))
                    AddWarning($"{nameof(hero.Type)} is null or emtpy");

                if (string.IsNullOrEmpty(hero.MountLinkId))
                    AddWarning($"{nameof(hero.MountLinkId)} is null or emtpy");

                VerifyWeapons(hero);

                if (hero.Weapons.Count < 1)
                    AddWarning("has no weapons");
                else if (hero.Weapons.Count > 1)
                    AddWarning("has more than 1 weapon");

                if (hero.PrimaryAbilities(AbilityTier.Basic).Count < 3)
                    AddWarning($"has less than 3 basic abilities");

                if (hero.PrimaryAbilities(AbilityTier.Basic).Count > 3)
                    AddWarning($"has more than 3 basic abilities");

                VerifyAbilities(hero);

                // hero portraits
                if (string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName))
                    AddWarning($"[{nameof(hero.HeroPortrait.HeroSelectPortraitFileName)}]  is null or empty");

                if (string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName))
                    AddWarning($"[{nameof(hero.HeroPortrait.LeaderboardPortraitFileName)}]  is null or empty");

                if (string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName))
                    AddWarning($"[{nameof(hero.HeroPortrait.LoadingScreenPortraitFileName)}]  is null or empty");

                if (string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName))
                    AddWarning($"[{nameof(hero.HeroPortrait.PartyPanelPortraitFileName)}]  is null or empty");

                if (string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName))
                    AddWarning($"[{nameof(hero.HeroPortrait.TargetPortraitFileName)}]  is null or empty");

                foreach (var talent in hero.Talents)
                {
                    if (string.IsNullOrEmpty(talent.Value.IconFileName))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.IconFileName)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Name))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Name)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.ReferenceNameId))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.ReferenceNameId)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.FullTooltipNameId))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.FullTooltipNameId)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.ShortTooltipNameId))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.ShortTooltipNameId)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.ShortTooltip?.RawDescription))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.ShortTooltip)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.RawDescription))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.PlainText))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)}.{nameof(talent.Value.Tooltip.FullTooltip.PlainText)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.PlainTextWithNewlines))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)}.{nameof(talent.Value.Tooltip.FullTooltip.PlainTextWithScaling)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.PlainTextWithScaling))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)}.{nameof(talent.Value.Tooltip.FullTooltip.PlainTextWithScaling)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.PlainTextWithScalingWithNewlines))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)}.{nameof(talent.Value.Tooltip.FullTooltip.PlainTextWithScalingWithNewlines)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.ColoredText))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)}.{nameof(talent.Value.Tooltip.FullTooltip.ColoredText)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.ColoredTextWithScaling))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)}.{nameof(talent.Value.Tooltip.FullTooltip.ColoredTextWithScaling)} is null or empty");

                    if (talent.Value.Tooltip.Cooldown?.CooldownTooltip != null)
                    {
                        if (char.IsDigit(talent.Value.Tooltip.Cooldown.CooldownTooltip.PlainText[0]))
                            AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.Cooldown.CooldownTooltip)} does not have a prefix");
                    }

                    if (talent.Value.Tooltip.Energy?.EnergyTooltip != null)
                    {
                        if (char.IsDigit(talent.Value.Tooltip.Energy.EnergyTooltip.PlainText[0]))
                            AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.Energy.EnergyTooltip)} does not have a prefix");
                    }

                    if (talent.Value.Tooltip.Life?.LifeCostTooltip != null)
                    {
                        if (char.IsDigit(talent.Value.Tooltip.Life.LifeCostTooltip.PlainText[0]))
                            AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.Life.LifeCostTooltip)} does not have a prefix");
                    }
                }

                if (hero.HeroUnits.Count > 0)
                {
                    foreach (Unit additionalUnit in hero.HeroUnits)
                    {
                        if (additionalUnit.Life.LifeMax <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(additionalUnit.Life)} is 0");

                        if (additionalUnit.Life.LifeScaling <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(additionalUnit.Life.LifeScaling)} is 0");

                        if (additionalUnit.Life.LifeRegenerationRateScaling <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(additionalUnit.Life.LifeRegenerationRateScaling)} is 0");

                        if (additionalUnit.Energy.EnergyMax > 0 && string.IsNullOrEmpty(hero.Energy.EnergyType))
                            AddWarning($"[{additionalUnit.Name}] {nameof(additionalUnit.Energy)} > 0 and {nameof(additionalUnit.Energy.EnergyType)} is NONE");

                        if (additionalUnit.Sight <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(additionalUnit.Sight)} is 0");

                        if (additionalUnit.Speed <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(additionalUnit.Speed)} is 0");

                        VerifyWeapons(hero);
                        VerifyAbilities(hero);
                    }
                }
            }
        }

        private void VerifyWeapons(Unit unit)
        {
            foreach (var weapon in unit.Weapons)
            {
                if (weapon.Damage <= 0)
                    AddWarning($"{weapon.WeaponNameId} {nameof(weapon.Damage)} is 0");

                if (weapon.Period <= 0)
                    AddWarning($"{weapon.WeaponNameId} {nameof(weapon.Period)} is 0");

                if (weapon.Range <= 0)
                    AddWarning($"{weapon.WeaponNameId} {nameof(weapon.Range)} is 0");

                if (weapon.DamageScaling <= 0)
                    AddWarning($"{weapon.WeaponNameId} {nameof(weapon.DamageScaling)} is 0");
            }
        }

        private void VerifyAbilities(Unit unit)
        {
            foreach (var ability in unit.Abilities)
            {
                if (string.IsNullOrEmpty(ability.Value.IconFileName))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.IconFileName)} is null or empty");

                if (string.IsNullOrEmpty(ability.Value.Name))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.Name)} is null or empty");

                if (string.IsNullOrEmpty(ability.Value.ReferenceNameId))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.ReferenceNameId)} is null or empty");

                if (string.IsNullOrEmpty(ability.Value.FullTooltipNameId))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.FullTooltipNameId)} is null or empty");

                if (string.IsNullOrEmpty(ability.Value.ShortTooltipNameId))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.ShortTooltipNameId)} is null or empty");

                if (string.IsNullOrEmpty(ability.Value.Tooltip.ShortTooltip?.RawDescription))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.ShortTooltip)} is null or empty");

                if (string.IsNullOrEmpty(ability.Value.Tooltip.FullTooltip?.RawDescription))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.FullTooltip)} is null or empty");

                if (!string.IsNullOrEmpty(ability.Value.Tooltip.Cooldown?.CooldownTooltip?.RawDescription))
                {
                    if (char.IsDigit(ability.Value.Tooltip.Cooldown.CooldownTooltip.PlainText[0]))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.Cooldown.CooldownTooltip)} does not have a prefix");
                }

                if (!string.IsNullOrEmpty(ability.Value.Tooltip.Energy?.EnergyTooltip?.RawDescription))
                {
                    if (char.IsDigit(ability.Value.Tooltip.Energy.EnergyTooltip.PlainText[0]))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.Energy.EnergyTooltip)} does not have a prefix");
                }

                if (!string.IsNullOrEmpty(ability.Value.Tooltip.Life?.LifeCostTooltip?.RawDescription))
                {
                    if (char.IsDigit(ability.Value.Tooltip.Life.LifeCostTooltip.PlainText[0]))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.Life.LifeCostTooltip)} does not have a prefix");
                }
            }
        }

        private void VerifyMatchAwardData()
        {
            if (ParsedMatchAwardData == null)
                return;

            foreach (MatchAward award in ParsedMatchAwardData)
            {
                WarningId = award.ShortName;

                if (string.IsNullOrEmpty(award.Name))
                    AddWarning($"{nameof(award.Name)} is null or empty");

                if (award.Name.Contains("_"))
                    AddWarning($"{nameof(award.Name)} contains an underscore, may have a duplicate name");

                if (string.IsNullOrEmpty(award.ShortName))
                    AddWarning($"{nameof(award.ShortName)} is null or empty");

                if (award.ShortName.Contains(","))
                    AddWarning($"{nameof(award.ShortName)} contains a comma, may have a duplicate short name");

                if (string.IsNullOrEmpty(award.Tag))
                    AddWarning($"{nameof(award.Tag)} is null or empty");

                if (string.IsNullOrEmpty(award.MVPScreenImageFileName))
                    AddWarning($"{nameof(award.MVPScreenImageFileName)} is null or empty");

                if (string.IsNullOrEmpty(award.ScoreScreenImageFileName))
                    AddWarning($"{nameof(award.ScoreScreenImageFileName)} is null or empty");

                if (string.IsNullOrEmpty(award.ScoreScreenImageFileNameOriginal))
                    AddWarning($"{nameof(award.ScoreScreenImageFileNameOriginal)} is null or empty");
            }
        }

        private void AddWarning(string message)
        {
            message = $"[{WarningId}] {message}".Trim();

            if (!IgnoreLines.Contains(message))
                Warnings.Add(message);
            else
                WarningsIgnored++;
        }

        private void ReadIgnoreFile()
        {
            if (File.Exists(VerifyIgnoreFileName))
            {
                using (StreamReader reader = new StreamReader(VerifyIgnoreFileName))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                        {
                            IgnoreLines.Add(line);
                        }
                    }
                }
            }
        }
    }
}
