using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Helpers;
using HeroesData.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesData.ExtractorData
{
    public class DataHero : DataExtractorBase<Hero, HeroDataParser>, IData
    {
        public DataHero(HeroDataParser parser)
            : base(parser)
        {
        }

        public override string Name => "heroes";

        public override IEnumerable<Hero> Parse(Localization localization)
        {
            Stopwatch time = new Stopwatch();
            var failedParsedHeroes = new List<(string CHeroId, Exception Exception)>();
            int currentCount = 0;

            Console.WriteLine($"Parsing {Name}...");

            time.Start();

            Parser.HotsBuild = App.HotsBuild;
            Parser.Localization = localization;

            // parse the base hero and add it to parsedHeroes
            Parser.ParseBaseHero();
            ParsedData.GetOrAdd(Parser.StormHeroBase.CHeroId, Parser.StormHeroBase);

            HashSet<string[]> heroes = Parser.Items;

            // parse all the heroes
            Console.Write($"\r{currentCount,6} / {heroes.Count} total {Name}");
            Parallel.ForEach(heroes, new ParallelOptions { MaxDegreeOfParallelism = App.MaxParallelism }, hero =>
            {
                try
                {
                    ParsedData.GetOrAdd(hero[0], Parser.GetInstance().Parse(hero));
                }
                catch (Exception ex)
                {
                    failedParsedHeroes.Add((hero[0], ex));
                }
                finally
                {
                    Console.Write($"\r{Interlocked.Increment(ref currentCount),6} / {heroes.Count} total {Name}");
                }
            });

            time.Stop();

            Console.WriteLine();

            if (failedParsedHeroes.Count > 0)
            {
                foreach (var hero in failedParsedHeroes)
                {
                    App.WriteExceptionLog($"FailedHeroParsed_{hero.CHeroId}", hero.Exception);
                }
            }

            Console.WriteLine($"{ParsedData.Count - 1,6} successfully parsed {Name}"); // minus 1 to account for base hero

            if (failedParsedHeroes.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var hero in failedParsedHeroes)
                {
                    Console.WriteLine($"{hero.CHeroId} - {hero.Exception.Message}");
                }

                Console.WriteLine($"{failedParsedHeroes.Count} failed to parse [Check logs for details]");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }

            Console.ResetColor();
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();

            return ParsedData.Values.OrderBy(x => x.Id);
        }

        protected override void Validation(Hero hero)
        {
            if (hero.CHeroId == StormHero.CHeroId)
                return;

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

            if (Parser.HotsBuild.GetValueOrDefault(0) >= 72880 && string.IsNullOrEmpty(hero.ExpandedRole))
                AddWarning($"{nameof(hero.ExpandedRole)} is null or empty");

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

            if (hero.Rarity == Rarity.None || hero.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(hero.Rarity)} is none or unknown");

            if (!hero.ReleaseDate.HasValue)
                AddWarning($"{nameof(hero.ReleaseDate)} is null");

            if (string.IsNullOrEmpty(hero.Type))
                AddWarning($"{nameof(hero.Type)} is null or emtpy");

            if (string.IsNullOrEmpty(hero.MountLinkId))
                AddWarning($"{nameof(hero.MountLinkId)} is null or emtpy");

            if (string.IsNullOrEmpty(hero.Title))
                AddWarning($"{nameof(hero.Title)} is null or emtpy");

            if (string.IsNullOrEmpty(hero.SearchText))
                AddWarning($"{nameof(hero.SearchText)} is null or emtpy");

            if (hero.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(hero.Rarity)} is unknown");

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

                if (!string.IsNullOrEmpty(talent.Value.Tooltip.ShortTooltip?.RawDescription) && talent.Value.Tooltip.ShortTooltip.RawDescription.Contains("<d ref=\""))
                    AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.ShortTooltip)} could not be parsed");

                if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.RawDescription))
                    AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)} is null or empty");

                if (!string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.RawDescription) && talent.Value.Tooltip.FullTooltip.RawDescription.Contains("<d ref=\""))
                    AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)} could not be parsed");

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
    }
}
