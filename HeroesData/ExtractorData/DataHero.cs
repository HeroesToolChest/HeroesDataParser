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
            // TODO: possibly re-add base hero
            //Parser.ParseBaseHero();
            //ParsedData.GetOrAdd(Parser.StormHeroBase.CHeroId, Parser.StormHeroBase);

            HashSet<string[]> heroes = Parser.Items;

            // parse all the heroes
            Console.Write($"\r{currentCount,6} / {heroes.Count} total {Name}");
            // TODO: re-add max parallelism
            Parallel.ForEach(heroes, new ParallelOptions { MaxDegreeOfParallelism = 1 }, hero =>
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
            Console.WriteLine($"Finished in {time.Elapsed.TotalSeconds} seconds");
            Console.WriteLine();

            return ParsedData.Values.OrderBy(x => x.Id);
        }

        protected override void Validation(Hero hero)
        {
            if (hero.CHeroId == StormHero.CHeroId)
                return;

            if (string.IsNullOrEmpty(hero.AttributeId))
                AddWarning($"{nameof(hero.AttributeId)} is empty");

            if (string.IsNullOrEmpty(hero.Description?.RawDescription))
                AddWarning($"{nameof(hero.Description)} is empty");

            if (string.IsNullOrEmpty(hero.Difficulty))
                AddWarning($"{nameof(hero.Difficulty)} is Unknown");

            if (hero.Franchise == HeroFranchise.Unknown)
                AddWarning($"{nameof(hero.Franchise)} is Unknown");

            if (hero.Roles.Contains("Unknown"))
                AddWarning($"{nameof(hero.Roles)} is Unknown");

            if (Parser.HotsBuild.GetValueOrDefault(0) >= 72880 && string.IsNullOrEmpty(hero.ExpandedRole))
                AddWarning($"{nameof(hero.ExpandedRole)} is empty");

            if (!hero.Abilities.Any())
                AddWarning("Hero has no abilities");

            if (!hero.Talents.Any())
                AddWarning("Hero has no talents");

            if (hero.Life.LifeMax <= 0)
                AddWarning($"{nameof(hero.Life)} is 0");

            if (hero.Life.LifeScaling <= 0)
                AddWarning($"{nameof(hero.Life.LifeScaling)} is 0");

            if (hero.Life.LifeRegenerationRateScaling <= 0)
                AddWarning($"{nameof(hero.Life.LifeRegenerationRateScaling)} is 0");

            if (hero.Energy.EnergyMax > 0 && string.IsNullOrEmpty(hero.Energy.EnergyType))
                AddWarning($"{nameof(hero.Energy)} > 0 and {nameof(hero.Energy.EnergyType)} is NONE");

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

            if (!hero.Weapons.Any())
                AddWarning("has no weapons");

            if (hero.PrimaryAbilities(AbilityTier.Basic).Count() < 3)
                AddWarning($"has less than 3 basic abilities");

            if (hero.PrimaryAbilities(AbilityTier.Basic).Count() > 3)
                AddWarning($"has more than 3 basic abilities");

            VerifyAbilities(hero);

            // hero portraits
            if (string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName))
                AddWarning($"[{nameof(hero.HeroPortrait.HeroSelectPortraitFileName)}]  is empty");

            if (string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName))
                AddWarning($"[{nameof(hero.HeroPortrait.LeaderboardPortraitFileName)}]  is empty");

            if (string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName))
                AddWarning($"[{nameof(hero.HeroPortrait.LoadingScreenPortraitFileName)}]  is empty");

            if (string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName))
                AddWarning($"[{nameof(hero.HeroPortrait.PartyPanelPortraitFileName)}]  is empty");

            if (string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName))
                AddWarning($"[{nameof(hero.HeroPortrait.TargetPortraitFileName)}]  is empty");

            foreach (Talent talent in hero.Talents)
            {
                if (string.IsNullOrEmpty(talent.IconFileName))
                    AddWarning($"[{talent}] {nameof(talent.IconFileName)} is empty");

                if (string.IsNullOrEmpty(talent.Name))
                    AddWarning($"[{talent}] {nameof(talent.Name)} is empty");

                if (string.IsNullOrEmpty(talent.ReferenceNameId))
                    AddWarning($"[{talent}] {nameof(talent.ReferenceNameId)} is empty");

                if (string.IsNullOrEmpty(talent.FullTooltipNameId))
                    AddWarning($"[{talent}] {nameof(talent.FullTooltipNameId)} is empty");

                if (string.IsNullOrEmpty(talent.ShortTooltipNameId))
                    AddWarning($"[{talent}] {nameof(talent.ShortTooltipNameId)} is empty");

                if (string.IsNullOrEmpty(talent.Tooltip.ShortTooltip?.RawDescription))
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.ShortTooltip)} is empty");

                if (!string.IsNullOrEmpty(talent.Tooltip.ShortTooltip?.RawDescription) && (talent.Tooltip.ShortTooltip.RawDescription.Contains("<d ref=\"") || talent.Tooltip.ShortTooltip.RawDescription.Contains("<d const=\"")))
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.ShortTooltip)} could not be parsed");

                if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip?.RawDescription))
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)} is empty");

                if (!string.IsNullOrEmpty(talent.Tooltip.FullTooltip?.RawDescription) && (talent.Tooltip.FullTooltip.RawDescription.Contains("<d ref=\"") || talent.Tooltip.FullTooltip.RawDescription.Contains("<d const=\"")))
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)} could not be parsed");

                if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip?.PlainText))
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)}.{nameof(talent.Tooltip.FullTooltip.PlainText)} is empty");

                if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip?.PlainTextWithNewlines))
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)}.{nameof(talent.Tooltip.FullTooltip.PlainTextWithScaling)} is empty");

                if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip?.PlainTextWithScaling))
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)}.{nameof(talent.Tooltip.FullTooltip.PlainTextWithScaling)} is empty");

                if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip?.PlainTextWithScalingWithNewlines))
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)}.{nameof(talent.Tooltip.FullTooltip.PlainTextWithScalingWithNewlines)} is empty");

                if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip?.ColoredText))
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)}.{nameof(talent.Tooltip.FullTooltip.ColoredText)} is empty");

                if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip?.ColoredTextWithScaling))
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)}.{nameof(talent.Tooltip.FullTooltip.ColoredTextWithScaling)} is empty");

                if (talent.Tooltip.Cooldown?.CooldownTooltip != null)
                {
                    if (!string.IsNullOrEmpty(talent.Tooltip.Cooldown.CooldownTooltip.PlainText) && char.IsDigit(talent.Tooltip.Cooldown.CooldownTooltip.PlainText[0]))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Cooldown.CooldownTooltip)} does not have a prefix");
                }

                if (talent.Tooltip.Energy?.EnergyTooltip != null)
                {
                    if (!string.IsNullOrEmpty(talent.Tooltip.Energy.EnergyTooltip.PlainText) && char.IsDigit(talent.Tooltip.Energy.EnergyTooltip.PlainText[0]))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Energy.EnergyTooltip)} does not have a prefix");
                }

                if (talent.Tooltip.Life?.LifeCostTooltip != null)
                {
                    if (!string.IsNullOrEmpty(talent.Tooltip.Life.LifeCostTooltip.PlainText) && char.IsDigit(talent.Tooltip.Life.LifeCostTooltip.PlainText[0]))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Life.LifeCostTooltip)} does not have a prefix");
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
            foreach (Ability ability in unit.Abilities)
            {
                if (string.IsNullOrEmpty(ability.IconFileName))
                    AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.IconFileName)} is empty");

                if (string.IsNullOrEmpty(ability.Name))
                    AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.Name)} is empty");

                if (string.IsNullOrEmpty(ability.ReferenceNameId))
                    AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.ReferenceNameId)} is empty");

                if (string.IsNullOrEmpty(ability.FullTooltipNameId))
                    AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.FullTooltipNameId)} is empty");

                if (string.IsNullOrEmpty(ability.ShortTooltipNameId))
                    AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.ShortTooltipNameId)} is empty");

                if (string.IsNullOrEmpty(ability.Tooltip.ShortTooltip?.RawDescription))
                    AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.Tooltip.ShortTooltip)} is empty");

                if (string.IsNullOrEmpty(ability.Tooltip.FullTooltip?.RawDescription))
                    AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.Tooltip.FullTooltip)} is empty");

                if (!string.IsNullOrEmpty(ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription))
                {
                    if (char.IsDigit(ability.Tooltip.Cooldown.CooldownTooltip.PlainText[0]))
                        AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.Tooltip.Cooldown.CooldownTooltip)} does not have a prefix");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.Energy?.EnergyTooltip?.RawDescription))
                {
                    if (char.IsDigit(ability.Tooltip.Energy.EnergyTooltip.PlainText[0]))
                        AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.Tooltip.Energy.EnergyTooltip)} does not have a prefix");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.Life?.LifeCostTooltip?.RawDescription))
                {
                    if (char.IsDigit(ability.Tooltip.Life.LifeCostTooltip.PlainText[0]))
                        AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.Tooltip.Life.LifeCostTooltip)} does not have a prefix");
                }
            }
        }
    }
}
