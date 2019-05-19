using Heroes.Models;
using Heroes.Models.AbilityTalents;
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

            if (!unit.Attributes.Any())
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
