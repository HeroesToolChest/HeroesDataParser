using Heroes.Models;
using HeroesData.Parser;
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
            if (unit.Life.LifeMax <= 0)
                AddWarning($"{nameof(unit.Life)} is 0");

            if (unit.Sight <= 0)
                AddWarning($"{nameof(unit.Sight)} is 0");

            if (unit.Speed <= 0 && unit.DamageType != "Structure" && !unit.Attributes.Contains("Structure"))
                AddWarning($"{nameof(unit.Speed)} is 0");

            if (string.IsNullOrEmpty(unit.DamageType))
                AddWarning($"{nameof(unit.DamageType)} is null or emtpy");

            if (unit.Attributes.Count < 1)
                AddWarning($"{nameof(unit.Attributes)} is 0");

            VerifyWeapons(unit);

            if (!unit.Weapons.Any() && unit.DamageType != "Structure" && !unit.Attributes.Contains("Structure"))
                AddWarning("has no weapons");

            VerifyAbilities(unit);
        }

        private void VerifyWeapons(Unit unit)
        {
            foreach (UnitWeapon weapon in unit.Weapons)
            {
                // TODO: add back
                //if (unit.DamageType == "Structure" || unit.Attributes?.Contains("Structure"))
                if (unit.DamageType == "Structure")
                    continue;

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
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.IconFileName)} is empty");

                if (string.IsNullOrEmpty(ability.Value.Name))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.Name)} is empty");

                if (string.IsNullOrEmpty(ability.Value.ReferenceNameId))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.ReferenceNameId)} is empty");

                if (string.IsNullOrEmpty(ability.Value.FullTooltipNameId))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.FullTooltipNameId)} is empty");

                if (string.IsNullOrEmpty(ability.Value.ShortTooltipNameId))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.ShortTooltipNameId)} is empty");

                if (string.IsNullOrEmpty(ability.Value.Tooltip.ShortTooltip?.RawDescription))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.ShortTooltip)} is empty");

                if (string.IsNullOrEmpty(ability.Value.Tooltip.FullTooltip?.RawDescription))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.FullTooltip)} is empty");

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
