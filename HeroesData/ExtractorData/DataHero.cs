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
    public class DataHero : DataExtractorBase<Hero?, HeroDataParser>, IData
    {
        public DataHero(HeroDataParser parser)
            : base(parser)
        {
        }

        public override string Name => "heroes";

        public override IEnumerable<Hero?> Parse(Localization localization)
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
                foreach (var (cHeroId, exception) in failedParsedHeroes)
                {
                    App.WriteExceptionLog($"FailedHeroParsed_{cHeroId}", exception);
                }
            }

            Console.WriteLine($"{ParsedData.Count,6} successfully parsed {Name}");

            if (failedParsedHeroes.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var (cHeroId, exception) in failedParsedHeroes)
                {
                    Console.WriteLine($"{cHeroId} - {exception.Message}");
                }

                Console.WriteLine($"{failedParsedHeroes.Count} failed to parse [Check logs for details]");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }

            Console.ResetColor();
            Console.WriteLine($"Finished in {time.Elapsed.TotalSeconds:0.####} seconds");
            Console.WriteLine();

            return ParsedData.Values.OrderBy(x => x!.Id, StringComparer.OrdinalIgnoreCase);
        }

        protected override void Validation(Hero? hero)
        {
            if (hero is null)
            {
                throw new ArgumentNullException(nameof(hero));
            }

            if (string.IsNullOrEmpty(hero.AttributeId))
                AddWarning($"{nameof(hero.AttributeId)} is empty");

            if (string.IsNullOrEmpty(hero.Description?.RawDescription))
                AddWarning($"{nameof(hero.Description)} is empty");
            else if (hero.Description.HasErrorTag)
                AddWarning($"{nameof(hero.Description.RawDescription)} contains an error tag");

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

            if (hero.PrimaryAbilities(AbilityTiers.Basic).Count() < 3)
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

                if (!string.IsNullOrEmpty(talent.Tooltip.ShortTooltip?.RawDescription))
                {
                    if (talent.Tooltip.ShortTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.ShortTooltip)} failed to parse correctly");
                    else if (talent.Tooltip.ShortTooltip.RawDescription.Contains("<d ref=\"") || talent.Tooltip.ShortTooltip.RawDescription.Contains("<d const=\""))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.ShortTooltip)} could not be parsed");
                    else if (talent.Tooltip.ShortTooltip.HasErrorTag)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.ShortTooltip)} contains an error tag");
                }

                if (!string.IsNullOrEmpty(talent.Tooltip.FullTooltip?.RawDescription))
                {
                    if (talent.Tooltip.FullTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)} failed to parse correctly");
                    else if (talent.Tooltip.FullTooltip.RawDescription.Contains("<d ref=\"") || talent.Tooltip.FullTooltip.RawDescription.Contains("<d const=\""))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)} could not be parsed");
                    else if (talent.Tooltip.FullTooltip.HasErrorTag)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)} contains an error tag");

                    if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip.PlainText))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)}.{nameof(talent.Tooltip.FullTooltip.PlainText)} is empty");

                    if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip.PlainTextWithNewlines))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)}.{nameof(talent.Tooltip.FullTooltip.PlainTextWithScaling)} is empty");

                    if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip.PlainTextWithScaling))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)}.{nameof(talent.Tooltip.FullTooltip.PlainTextWithScaling)} is empty");

                    if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip.PlainTextWithScalingWithNewlines))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)}.{nameof(talent.Tooltip.FullTooltip.PlainTextWithScalingWithNewlines)} is empty");

                    if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip.ColoredText))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)}.{nameof(talent.Tooltip.FullTooltip.ColoredText)} is empty");

                    if (string.IsNullOrEmpty(talent.Tooltip.FullTooltip.ColoredTextWithScaling))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.FullTooltip)}.{nameof(talent.Tooltip.FullTooltip.ColoredTextWithScaling)} is empty");
                }

                if (!string.IsNullOrEmpty(talent.Tooltip.Cooldown?.CooldownTooltip?.RawDescription))
                {
                    if (talent.Tooltip.Cooldown.CooldownTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Cooldown.CooldownTooltip)} failed to parse correctly");
                    else if (char.IsDigit(talent.Tooltip.Cooldown.CooldownTooltip.PlainText[0]))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Cooldown.CooldownTooltip)} does not have a prefix");
                    else if (talent.Tooltip.Cooldown.CooldownTooltip.HasErrorTag)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Cooldown.CooldownTooltip)} contains an error tag");
                }

                if (!string.IsNullOrEmpty(talent.Tooltip.Energy?.EnergyTooltip?.RawDescription))
                {
                    if (talent.Tooltip.Energy.EnergyTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Energy.EnergyTooltip)} failed to parse correctly");
                    else if (char.IsDigit(talent.Tooltip.Energy.EnergyTooltip.PlainText[0]))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Energy.EnergyTooltip)} does not have a prefix");
                    else if (talent.Tooltip.Energy.EnergyTooltip.HasErrorTag)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Energy.EnergyTooltip)} contains an error tag");
                }

                if (!string.IsNullOrEmpty(talent.Tooltip.Life?.LifeCostTooltip?.RawDescription))
                {
                    if (talent.Tooltip.Life.LifeCostTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Life.LifeCostTooltip)} failed to parse correctly");
                    else if (char.IsDigit(talent.Tooltip.Life.LifeCostTooltip.PlainText[0]))
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Life.LifeCostTooltip)} does not have a prefix");
                    else if (talent.Tooltip.Life.LifeCostTooltip.HasErrorTag)
                        AddWarning($"[{talent}] {nameof(talent.Tooltip.Life.LifeCostTooltip)} contains an error tag");
                }

                if (talent.AbilityTalentId.AbilityType == AbilityTypes.Unknown)
                    AddWarning($"[{talent.AbilityTalentId.Id}] is of type Unknown");
                else if (talent.AbilityTalentId.AbilityType == AbilityTypes.Hidden)
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
                if (ability.Tier == AbilityTiers.Hidden)
                    continue;

                if (string.IsNullOrEmpty(ability.IconFileName))
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.IconFileName)} is empty");

                if (string.IsNullOrEmpty(ability.Name))
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Name)} is empty");

                if (string.IsNullOrEmpty(ability.AbilityTalentId.ButtonId))
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.AbilityTalentId.ButtonId)} is empty");

                if (!string.IsNullOrEmpty(ability.Tooltip.ShortTooltip?.RawDescription))
                {
                    if (ability.Tooltip.ShortTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.ShortTooltip)} failed to parse correctly");
                    else if (ability.Tooltip.ShortTooltip.HasErrorTag)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.ShortTooltip)} contains an error tag");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.FullTooltip?.RawDescription))
                {
                    if (ability.Tooltip.FullTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.FullTooltip)} failed to parse correctly");
                    else if (ability.Tooltip.FullTooltip.HasErrorTag)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.FullTooltip)} contains an error tag");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription))
                {
                    if (ability.Tooltip.Cooldown.CooldownTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Cooldown.CooldownTooltip)} failed to parse correctly");
                    else if (char.IsDigit(ability.Tooltip.Cooldown.CooldownTooltip.PlainText[0]))
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Cooldown.CooldownTooltip)} does not have a prefix");
                    else if (ability.Tooltip.Cooldown.CooldownTooltip.HasErrorTag)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Cooldown.CooldownTooltip)} contains an error tag");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.Energy?.EnergyTooltip?.RawDescription))
                {
                    if (ability.Tooltip.Energy.EnergyTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Energy.EnergyTooltip)} failed to parse correctly");
                    else if (char.IsDigit(ability.Tooltip.Energy.EnergyTooltip.PlainText[0]))
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Energy.EnergyTooltip)} does not have a prefix");
                    else if (ability.Tooltip.Energy.EnergyTooltip.HasErrorTag)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Energy.EnergyTooltip)} contains an error tag");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.Life?.LifeCostTooltip?.RawDescription))
                {
                    if (ability.Tooltip.Life.LifeCostTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Life.LifeCostTooltip)} failed to parse correctly");
                    else if (char.IsDigit(ability.Tooltip.Life.LifeCostTooltip.PlainText[0]))
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Life.LifeCostTooltip)} does not have a prefix");
                    else if (ability.Tooltip.Life.LifeCostTooltip.HasErrorTag)
                        AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Life.LifeCostTooltip)} contains an error tag");
                }

                if (ability.AbilityTalentId.AbilityType == AbilityTypes.Unknown)
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] is of type Unknown");
                else if (ability.AbilityTalentId.AbilityType == AbilityTypes.Hidden)
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] is of type Hidden");

                if (ability.AbilityTalentId.IsPassive && ability.IsActive)
                    AddWarning(unit.Id, $"[{ability.AbilityTalentId.Id}] is both passive and active");
            }
        }

        private void VerifyAbilitiesCount(string unitId, List<Ability> abilitiesList, bool isHeroUnit = false)
        {
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Q && x.Tier != AbilityTiers.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityTypes.Q} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.W && x.Tier != AbilityTiers.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityTypes.W} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.E && x.Tier != AbilityTiers.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityTypes.E} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Heroic && x.Tier != AbilityTiers.Hidden).Count() > 2)
                AddWarning(unitId, $"has more than 2 {AbilityTypes.Heroic} abilities");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Z && x.Tier != AbilityTiers.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityTypes.Z} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.B && x.Tier != AbilityTiers.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityTypes.B} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Trait && x.Tier != AbilityTiers.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityTypes.Trait} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Taunt && x.Tier != AbilityTiers.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityTypes.Taunt} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Spray && x.Tier != AbilityTiers.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityTypes.Spray} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Dance && x.Tier != AbilityTiers.Hidden).Count() > 1)
                AddWarning(unitId, $"has more than 1 {AbilityTypes.Dance} ability");

            if (!isHeroUnit)
            {
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Q && x.Tier != AbilityTiers.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityTypes.Q} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.W && x.Tier != AbilityTiers.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityTypes.W} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.E && x.Tier != AbilityTiers.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityTypes.E} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Heroic && x.Tier != AbilityTiers.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityTypes.Heroic} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Z && x.Tier != AbilityTiers.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityTypes.Z} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.B && x.Tier != AbilityTiers.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityTypes.B} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Trait && x.Tier != AbilityTiers.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityTypes.Trait} ability");
                if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Spray && x.Tier != AbilityTiers.Hidden).Count() < 1)
                    AddWarning(unitId, $"does not have a {AbilityTypes.Spray} ability");
            }
        }

        private void VerifySubAbilitiesCount(string unitId, List<Ability> abilitiesList)
        {
            ILookup<AbilityTalentId?, Ability> abilities = abilitiesList.ToLookup(x => x.ParentLink, x => x);

            foreach (IGrouping<AbilityTalentId?, Ability> parentLinkGroup in abilities)
            {
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Q && x.Tier != AbilityTiers.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityTypes.Q} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.W && x.Tier != AbilityTiers.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityTypes.W} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.E && x.Tier != AbilityTiers.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityTypes.E} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Heroic && x.Tier != AbilityTiers.Hidden).Count() > 2)
                    AddWarning(unitId, $"has more than 2 {AbilityTypes.Heroic} subabilities for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Z && x.Tier != AbilityTiers.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityTypes.Z} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.B && x.Tier != AbilityTiers.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityTypes.B} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Trait && x.Tier != AbilityTiers.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityTypes.Trait} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Taunt && x.Tier != AbilityTiers.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityTypes.Taunt} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Spray && x.Tier != AbilityTiers.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityTypes.Spray} subability for {parentLinkGroup.Key}");
                if (parentLinkGroup.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Dance && x.Tier != AbilityTiers.Hidden).Count() > 1)
                    AddWarning(unitId, $"has more than 1 {AbilityTypes.Dance} subability for {parentLinkGroup.Key}");
            }
        }
    }
}
