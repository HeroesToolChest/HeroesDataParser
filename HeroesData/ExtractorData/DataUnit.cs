using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using System;
using System.Linq;

namespace HeroesData.ExtractorData
{
    public class DataUnit : DataExtractorBase<Unit, UnitParser>, IData
    {
        public DataUnit(UnitParser parser)
            : base(parser)
        {
        }

        public override string Name => "units";

        protected override void Validation(Unit unit)
        {
            if (unit.Id.EndsWith("dummy", StringComparison.OrdinalIgnoreCase))
                return;

            if (unit.ContainsAttribute("Heroic"))
            {
                if (!unit.ContainsAttribute("ImmuneToAOE"))
                {
                    if (unit.Life.LifeMax <= 0)
                        AddWarning($"{nameof(unit.Life)} is 0");

                    if (unit.Sight <= 0)
                        AddWarning($"{nameof(unit.Sight)} is 0");

                    if (unit.Speed <= 0 && unit.DamageType != "Structure" && !unit.Attributes.Contains("Structure"))
                        AddWarning($"{nameof(unit.Speed)} is 0");

                    if (!unit.Attributes.Any())
                        AddWarning($"{nameof(unit.Attributes)} is 0");
                }

                VerifyAbilities(unit);
            }
            else
            {
                VerifyTooltipDescription(unit);
            }

            VerifyWeapons(unit);
        }

        private void VerifyWeapons(Unit unit)
        {
            foreach (UnitWeapon weapon in unit.Weapons)
            {
                if (weapon.Damage > 0)
                {
                    if (weapon.Period <= 0)
                        AddWarning($"{weapon.WeaponNameId} {nameof(weapon.Period)} is 0");

                    if (weapon.Range <= 0)
                        AddWarning($"{weapon.WeaponNameId} {nameof(weapon.Range)} is 0");
                }
            }
        }

        private void VerifyAbilities(Unit unit)
        {
            foreach (Ability ability in unit.Abilities)
            {
                if (string.IsNullOrEmpty(ability.ReferenceNameId))
                    AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.ReferenceNameId)} is empty");

                if (ability.Tooltip.ShortTooltip?.RawDescription == GameStringParser.FailedParsed)
                    AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.Tooltip.ShortTooltip)} failed to parse correctly");

                if (ability.Tooltip.FullTooltip?.RawDescription == GameStringParser.FailedParsed)
                    AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.Tooltip.FullTooltip)} failed to parse correctly");

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

        private void VerifyTooltipDescription(Unit unit)
        {
            foreach (Ability ability in unit.Abilities)
            {
                if (ability.Tooltip.ShortTooltip?.RawDescription == GameStringParser.FailedParsed)
                    AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.Tooltip.ShortTooltip)} failed to parse correctly");

                if (ability.Tooltip.FullTooltip?.RawDescription == GameStringParser.FailedParsed)
                    AddWarning($"[{ability.ReferenceNameId}] {nameof(ability.Tooltip.FullTooltip)} failed to parse correctly");
            }
        }
    }
}
