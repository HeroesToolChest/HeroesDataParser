using Heroes.Icons.Parser.Models;
using System.Collections.Generic;

namespace Heroes.Icons.Parser
{
    public static class VerifyHeroData
    {
        private static string HeroName;
        private static List<string> Warnings = new List<string>();

        /// <summary>
        /// Verifies the all the hero data for missing data. Returns a list of warnings.
        /// </summary>
        /// <param name="heroData">A list of all hero data</param>
        /// <returns></returns>
        public static List<string> Verify(List<Hero> heroData)
        {
            foreach (var hero in heroData)
            {
                HeroName = hero.Name;

                if (string.IsNullOrEmpty(hero.AttributeId))
                    AddWarning($"{nameof(hero.AttributeId)} is null or empty");

                if (string.IsNullOrEmpty(hero.Description?.RawDescription))
                    AddWarning($"{nameof(hero.Description)} is null or empty");

                if (hero.Difficulty == HeroDifficulty.Unknown)
                    AddWarning($"{nameof(hero.Difficulty)} is Unknown");

                if (hero.Franchise == HeroFranchise.Unknown)
                    AddWarning($"{nameof(hero.Franchise)} is Unknown");

                if (hero.Roles[0] == HeroRole.Unknown)
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

                if (hero.Energy.EnergyMax > 0 && hero.Energy.EnergyType == UnitEnergyType.None)
                    AddWarning($"{nameof(hero.Energy)} > 0 and {nameof(hero.Energy.EnergyType)} is NONE");

                if (hero.Sight <= 0)
                    AddWarning($"{nameof(hero.Sight)} is 0");

                if (hero.Speed <= 0)
                    AddWarning($"{nameof(hero.Speed)} is 0");

                if (hero.Rarity == HeroRarity.None)
                    AddWarning($"{nameof(hero.Rarity)} is None");

                foreach (var weapon in hero.Weapons)
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

                if (hero.Weapons.Count < 1)
                    AddWarning("has no weapons");
                else if (hero.Weapons.Count > 1)
                    AddWarning("has more than 1 weapon");

                foreach (var ability in hero.Abilities)
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

                    if (ability.Value.Tooltip.Cooldown.CooldownValue < 0)
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.Cooldown.CooldownValue)} is negative.");

                    if (ability.Value.Tooltip.Energy.EnergyCost < 0)
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.Energy.EnergyCost)} is negative.");

                    if (ability.Value.Tooltip.Life.LifeCost < 0)
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.Life.LifeCost)} is negative.");

                    if (string.IsNullOrEmpty(ability.Value.Tooltip.ShortTooltip?.RawDescription))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.ShortTooltip)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.Tooltip.FullTooltip?.RawDescription))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.FullTooltip)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.Tooltip.FullTooltip?.PlainText))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.FullTooltip)}.{nameof(ability.Value.Tooltip.FullTooltip.PlainText)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.Tooltip.FullTooltip?.PlainTextWithNewlines))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.FullTooltip)}.{nameof(ability.Value.Tooltip.FullTooltip.PlainTextWithScaling)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.Tooltip.FullTooltip?.PlainTextWithScaling))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.FullTooltip)}.{nameof(ability.Value.Tooltip.FullTooltip.PlainTextWithScaling)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.Tooltip.FullTooltip?.PlainTextWithScalingWithNewlines))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.FullTooltip)}.{nameof(ability.Value.Tooltip.FullTooltip.PlainTextWithScalingWithNewlines)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.Tooltip.FullTooltip?.ColoredText))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.FullTooltip)}.{nameof(ability.Value.Tooltip.FullTooltip.ColoredText)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.Tooltip.FullTooltip?.ColoredTextWithScaling))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.FullTooltip)}.{nameof(ability.Value.Tooltip.FullTooltip.ColoredTextWithScaling)} is null or empty");
                }

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

                    if (talent.Value.Tooltip.Cooldown.CooldownValue < 0)
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.Cooldown.CooldownValue)} is negative.");

                    if (talent.Value.Tooltip.Energy.EnergyCost < 0)
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.Energy.EnergyCost)} is negative.");

                    if (talent.Value.Tooltip.Life.LifeCost < 0)
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.Life.LifeCost)} is negative.");

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
                }

                if (hero.AdditionalHeroUnits.Count > 0)
                {
                    foreach (Hero additionalUnit in hero.AdditionalHeroUnits)
                    {
                        if (additionalUnit.Life.LifeMax <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(hero.Life)} is 0");

                        if (additionalUnit.Life.LifeScaling <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(hero.Life.LifeScaling)} is 0");

                        if (additionalUnit.Life.LifeRegenerationRateScaling <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(hero.Life.LifeRegenerationRateScaling)} is 0");

                        if (additionalUnit.Energy.EnergyMax > 0 && hero.Energy.EnergyType == UnitEnergyType.None)
                            AddWarning($"[{additionalUnit.Name}] {nameof(hero.Energy)} > 0 and {nameof(hero.Energy.EnergyType)} is NONE");

                        if (additionalUnit.Sight <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(hero.Sight)} is 0");

                        if (additionalUnit.Speed <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(hero.Speed)} is 0");

                        foreach (var weapon in additionalUnit.Weapons)
                        {
                            if (weapon.Damage <= 0)
                                AddWarning($"[{additionalUnit.Name}] {weapon.WeaponNameId} {nameof(weapon.Damage)} is 0");

                            if (weapon.Period <= 0)
                                AddWarning($"[{additionalUnit.Name}] {weapon.WeaponNameId} {nameof(weapon.Period)} is 0");

                            if (weapon.Range <= 0)
                                AddWarning($"[{additionalUnit.Name}] {weapon.WeaponNameId} {nameof(weapon.Range)} is 0");

                            if (weapon.DamageScaling <= 0)
                                AddWarning($"[{additionalUnit.Name}] {weapon.WeaponNameId} {nameof(weapon.DamageScaling)} is 0");
                        }
                    }
                }
            }

            return Warnings;
        }

        private static void AddWarning(string message)
        {
            Warnings.Add($"[{HeroName}] {message}");
        }
    }
}
