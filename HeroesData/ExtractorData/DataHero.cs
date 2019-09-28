using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
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
            ParsedData.Clear();

            Console.WriteLine($"Parsing {Name}...");

            time.Start();

            Parser.HotsBuild = App.HotsBuild;
            Parser.Localization = localization;

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

            Console.WriteLine($"{ParsedData.Count,6} successfully parsed {Name}");

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

            if (string.IsNullOrEmpty(hero.ScalingBehaviorLink))
                AddWarning($"{nameof(hero.ScalingBehaviorLink)} is null or empty");

            if (!hero.ReleaseDate.HasValue)
                AddWarning($"{nameof(hero.ReleaseDate)} is null");

            if (string.IsNullOrEmpty(hero.Type))
                AddWarning($"{nameof(hero.Type)} is null or emtpy");

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

            VerifyAbilities(hero);
            VerifyAbilitiesCount(hero.Id, hero.PrimaryAbilities().ToList());
            VerifySubAbilitiesCount(hero.Id, hero.SubAbilities().ToList());

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

            if (string.IsNullOrEmpty(hero.HeroPortrait.DraftScreenFileName))
                AddWarning($"[{nameof(hero.HeroPortrait.DraftScreenFileName)}]  is empty");

            if (string.IsNullOrEmpty(hero.UnitPortrait.MiniMapIconFileName))
                AddWarning($"[{nameof(hero.UnitPortrait.MiniMapIconFileName)}]  is empty");

            if (string.IsNullOrEmpty(hero.UnitPortrait.TargetInfoPanelFileName))
                AddWarning($"[{nameof(hero.UnitPortrait.TargetInfoPanelFileName)}]  is empty");

            if (hero.HeroPortrait.PartyFrameFileName.Count < 1)
                AddWarning($"[{nameof(hero.HeroPortrait.PartyFrameFileName)}]  is empty");

            foreach (Talent talent in hero.Talents)
            {
                if (string.IsNullOrEmpty(talent.IconFileName))
                    AddWarning($"[{talent}, {talent}] {nameof(talent.IconFileName)} is empty");

                if (string.IsNullOrEmpty(talent.Name))
                    AddWarning($"[{talent}] {nameof(talent.Name)} is empty");

                if (string.IsNullOrEmpty(talent.AbilityTalentId.ReferenceId))
                    AddWarning($"[{talent}] {nameof(talent.AbilityTalentId.Id)} is empty");

                if (string.IsNullOrEmpty(talent.AbilityTalentId.ButtonId))
                    AddWarning($"[{talent}] {nameof(talent.AbilityTalentId.Id)} is empty");

                if (talent.Tooltip.ShortTooltip?.RawDescription == GameStringParser.FailedParsed)
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.ShortTooltip)} failed to parse correctly");

                if (!string.IsNullOrEmpty(talent.Tooltip.ShortTooltip?.RawDescription) && (talent.Tooltip.ShortTooltip.RawDescription.Contains("<d ref=\"") || talent.Tooltip.ShortTooltip.RawDescription.Contains("<d const=\"")))
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.ShortTooltip)} could not be parsed");

                if (talent.Tooltip.FullTooltip?.RawDescription == GameStringParser.FailedParsed)
                    AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)} failed to parse correctly");

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

                if (!string.IsNullOrEmpty(talent.Tooltip.Cooldown?.CooldownTooltip?.RawDescription))
                {
                    if (talent.Tooltip.Cooldown.CooldownTooltip?.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Cooldown.CooldownTooltip)} failed to parse correctly");
                    else if (char.IsDigit(talent.Tooltip.Cooldown.CooldownTooltip.PlainText[0]))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Cooldown.CooldownTooltip)} does not have a prefix");
                }

                if (!string.IsNullOrEmpty(talent.Tooltip.Energy?.EnergyTooltip?.RawDescription))
                {
                    if (talent.Tooltip.Energy.EnergyTooltip?.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Energy.EnergyTooltip)} failed to parse correctly");
                    else if (char.IsDigit(talent.Tooltip.Energy.EnergyTooltip.PlainText[0]))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Energy.EnergyTooltip)} does not have a prefix");
                }

                if (!string.IsNullOrEmpty(talent.Tooltip.Life?.LifeCostTooltip?.RawDescription))
                {
                    if (talent.Tooltip.Life.LifeCostTooltip?.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Life.LifeCostTooltip)} failed to parse correctly");
                    else if (char.IsDigit(talent.Tooltip.Life.LifeCostTooltip.PlainText[0]))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Life.LifeCostTooltip)} does not have a prefix");
                }

                if (talent.AbilityTalentId.AbilityType == AbilityType.Unknown)
                    AddWarning($"[{talent.AbilityTalentId.Id}] is of type Unknown");
                else if (talent.AbilityTalentId.AbilityType == AbilityType.Hidden)
                    AddWarning($"[{talent.AbilityTalentId.Id}] is of type Hidden");
            }

            foreach (Hero heroUnit in hero.HeroUnits)
            {
                VerifyAbilities(heroUnit);
                VerifyAbilitiesCount(heroUnit.Id, heroUnit.PrimaryAbilities().ToList(), true);
                VerifySubAbilitiesCount(heroUnit.Id, heroUnit.SubAbilities().ToList());
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
                if (ability.Tier == AbilityTier.Hidden)
                    continue;

                if (string.IsNullOrEmpty(ability.IconFileName))
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.IconFileName)} is empty");

                if (string.IsNullOrEmpty(ability.Name))
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Name)} is empty");

                if (string.IsNullOrEmpty(ability.AbilityTalentId.ButtonId))
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.AbilityTalentId.ButtonId)} is empty");

                if (ability.Tooltip.ShortTooltip?.RawDescription == GameStringParser.FailedParsed)
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.ShortTooltip)} failed to parse correctly");

                if (ability.Tooltip.FullTooltip?.RawDescription == GameStringParser.FailedParsed)
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.FullTooltip)} failed to parse correctly");

                if (!string.IsNullOrEmpty(ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription))
                {
                    if (ability.Tooltip.Cooldown.CooldownTooltip?.RawDescription == GameStringParser.FailedParsed)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Cooldown.CooldownTooltip)} failed to parse correctly");
                    else if (char.IsDigit(ability.Tooltip.Cooldown.CooldownTooltip.PlainText[0]))
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Cooldown.CooldownTooltip)} does not have a prefix");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.Energy?.EnergyTooltip?.RawDescription))
                {
                    if (ability.Tooltip.Energy.EnergyTooltip?.RawDescription == GameStringParser.FailedParsed)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Energy.EnergyTooltip)} failed to parse correctly");
                    else if (char.IsDigit(ability.Tooltip.Energy.EnergyTooltip.PlainText[0]))
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Energy.EnergyTooltip)} does not have a prefix");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.Life?.LifeCostTooltip?.RawDescription))
                {
                    if (ability.Tooltip.Life.LifeCostTooltip?.RawDescription == GameStringParser.FailedParsed)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Life.LifeCostTooltip)} failed to parse correctly");
                    else if (char.IsDigit(ability.Tooltip.Life.LifeCostTooltip.PlainText[0]))
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Life.LifeCostTooltip)} does not have a prefix");
                }

                if (ability.AbilityTalentId.AbilityType == AbilityType.Unknown)
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] is of type Unknown");
                else if (ability.AbilityTalentId.AbilityType == AbilityType.Hidden)
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] is of type Hidden");
            }
        }

        private void VerifyAbilitiesCount(string unitId, List<Ability> abilitiesList, bool isHeroUnit = false)
        {
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Q && x.Tier != AbilityTier.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityType.Q} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.W && x.Tier != AbilityTier.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityType.W} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.E && x.Tier != AbilityTier.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityType.E} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Heroic && x.Tier != AbilityTier.Hidden).Count() > 2)
                AddWarning(unitId, $"has more than 2 {AbilityType.Heroic} abilities");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Z && x.Tier != AbilityTier.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityType.Z} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.B && x.Tier != AbilityTier.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityType.B} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Trait && x.Tier != AbilityTier.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityType.Trait} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Taunt && x.Tier != AbilityTier.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityType.Taunt} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Spray && x.Tier != AbilityTier.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityType.Spray} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Dance && x.Tier != AbilityTier.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityType.Dance} ability");

            if (!isHeroUnit)
            {
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Q && x.Tier != AbilityTier.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityType.Q} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.W && x.Tier != AbilityTier.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityType.W} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.E && x.Tier != AbilityTier.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityType.E} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Heroic && x.Tier != AbilityTier.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityType.Heroic} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Z && x.Tier != AbilityTier.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityType.Z} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.B && x.Tier != AbilityTier.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityType.B} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Trait && x.Tier != AbilityTier.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityType.Trait} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Spray && x.Tier != AbilityTier.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityType.Spray} ability");
            }
        }

        private void VerifySubAbilitiesCount(string unitId, List<Ability> abilitiesList)
        {
            ILookup<AbilityTalentId, Ability> abilities = abilitiesList.ToLookup(x => x.ParentLink, x => x);

            foreach (IGrouping<AbilityTalentId, Ability> parentLinkGroup in abilities)
            {
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Q && x.Tier != AbilityTier.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityType.Q} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityType.W && x.Tier != AbilityTier.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityType.W} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityType.E && x.Tier != AbilityTier.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityType.E} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Heroic && x.Tier != AbilityTier.Hidden).Count() > 2)
                    AddWarning(unitId, $"has more than 2 {AbilityType.Heroic} subabilities for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Z && x.Tier != AbilityTier.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityType.Z} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityType.B && x.Tier != AbilityTier.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityType.B} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Trait && x.Tier != AbilityTier.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityType.Trait} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Taunt && x.Tier != AbilityTier.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityType.Taunt} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Spray && x.Tier != AbilityTier.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityType.Spray} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityType.Dance && x.Tier != AbilityTier.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityType.Dance} subability for {parentLinkGroup.Key}");
            }
        }
    }
}
