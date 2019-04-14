using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;

namespace HeroesData.FileWriter.Writers.UnitData
{
    internal abstract class UnitDataWriter<T, TU> : WriterBase<Unit, T>
        where T : class
        where TU : class
    {
        public UnitDataWriter(FileOutputType fileOutputType)
            : base(nameof(UnitData), fileOutputType)
        {
        }

        protected abstract T GetArmorObject(Unit unit);
        protected abstract T GetLifeObject(Unit unit);
        protected abstract T GetEnergyObject(Unit unit);
        protected abstract T GetWeaponsObject(Unit unit);
        protected abstract T GetAbilitiesObject(Unit unit, bool isUnitAbilities);
        protected abstract TU AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase);
        protected abstract T GetAbilityTalentLifeCostObject(TooltipLife tooltipLife);
        protected abstract T GetAbilityTalentEnergyCostObject(TooltipEnergy tooltipEnergy);
        protected abstract T GetAbilityTalentCooldownObject(TooltipCooldown tooltipCooldown);
        protected abstract T GetAbilityTalentChargesObject(TooltipCharges tooltipCharges);

        protected void AddLocalizedGameString(Unit unit)
        {
            GameStringWriter.AddUnitName(unit.Id, unit.Name);
            GameStringWriter.AddUnitDescription(unit.Id, GetTooltip(unit.Description, FileOutputOptions.DescriptionType));
            GameStringWriter.AddUnitDamageType(unit.Id, unit.DamageType);
        }

        protected void AddLocalizedGameString(AbilityTalentBase abilityTalentBase)
        {
            GameStringWriter.AddAbilityTalentName(abilityTalentBase.ReferenceNameId, abilityTalentBase.Name);

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.Life?.LifeCostTooltip?.RawDescription))
                GameStringWriter.AddAbilityTalentLifeTooltip(abilityTalentBase.ReferenceNameId, GetTooltip(abilityTalentBase.Tooltip.Life.LifeCostTooltip, FileOutputOptions.DescriptionType));

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.Energy?.EnergyTooltip?.RawDescription))
                GameStringWriter.AddAbilityTalentEnergyTooltip(abilityTalentBase.ReferenceNameId, GetTooltip(abilityTalentBase.Tooltip.Energy.EnergyTooltip, FileOutputOptions.DescriptionType));

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.Cooldown?.CooldownTooltip?.RawDescription))
                GameStringWriter.AddAbilityTalentCooldownTooltip(abilityTalentBase.ReferenceNameId, GetTooltip(abilityTalentBase.Tooltip.Cooldown.CooldownTooltip, FileOutputOptions.DescriptionType));

            GameStringWriter.AddAbilityTalentShortTooltip(abilityTalentBase.ShortTooltipNameId, GetTooltip(abilityTalentBase.Tooltip.ShortTooltip, FileOutputOptions.DescriptionType));
            GameStringWriter.AddAbilityTalentFullTooltip(abilityTalentBase.FullTooltipNameId, GetTooltip(abilityTalentBase.Tooltip.FullTooltip, FileOutputOptions.DescriptionType));
        }

        protected T UnitArmor(Unit unit)
        {
            if (unit.Armor != null && unit.Armor.PhysicalArmor > 0 && unit.Armor.SpellArmor > 0)
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

        protected T UnitWeapons(Unit unit)
        {
            if (unit.Weapons?.Count > 0)
            {
                return GetWeaponsObject(unit);
            }

            return null;
        }

        protected T UnitAbilities(Unit unit, bool isSubAbilities)
        {
            if (unit.Abilities?.Count > 0)
            {
                return GetAbilitiesObject(unit, isSubAbilities);
            }

            return null;
        }

        protected T UnitAbilityTalentLifeCost(TooltipLife tooltipLife)
        {
            if (!string.IsNullOrEmpty(tooltipLife?.LifeCostTooltip?.RawDescription) && !FileOutputOptions.IsLocalizedText)
            {
                return GetAbilityTalentLifeCostObject(tooltipLife);
            }

            return null;
        }

        protected T UnitAbilityTalentEnergyCost(TooltipEnergy tooltipEnergy)
        {
            if (!string.IsNullOrEmpty(tooltipEnergy?.EnergyTooltip?.RawDescription) && !FileOutputOptions.IsLocalizedText)
            {
                return GetAbilityTalentEnergyCostObject(tooltipEnergy);
            }

            return null;
        }

        protected T UnitAbilityTalentCooldown(TooltipCooldown tooltipCooldown)
        {
            if (!string.IsNullOrEmpty(tooltipCooldown?.CooldownTooltip?.RawDescription) && !FileOutputOptions.IsLocalizedText)
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
    }
}
