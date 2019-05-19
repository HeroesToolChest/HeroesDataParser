using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using System.Linq;

namespace HeroesData.FileWriter.Writers.HeroData
{
    internal abstract class HeroDataWriter<T, TU> : WriterBase<Hero, T>
        where T : class
        where TU : class
    {
        protected HeroDataWriter(FileOutputType fileOutputType)
            : base(nameof(HeroData), fileOutputType)
        {
        }

        protected abstract T UnitElement(Unit unit);
        protected abstract T GetPortraitObject(Hero hero);
        protected abstract T GetArmorObject(Unit unit);
        protected abstract T GetLifeObject(Unit unit);
        protected abstract T GetEnergyObject(Unit unit);
        protected abstract T GetRatingsObject(Hero hero);
        protected abstract T GetWeaponsObject(Unit unit);
        protected abstract T GetAbilitiesObject(Unit unit, bool isUnitAbilities);
        protected abstract T GetSubAbilitiesObject(ILookup<string, Ability> linkedAbilities);
        protected abstract T GetTalentsObject(Hero hero);
        protected abstract T GetUnitsObject(Hero hero);
        protected abstract TU AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase);
        protected abstract TU TalentInfoElement(Talent talent);
        protected abstract T GetAbilityTalentLifeCostObject(TooltipLife tooltipLife);
        protected abstract T GetAbilityTalentEnergyCostObject(TooltipEnergy tooltipEnergy);
        protected abstract T GetAbilityTalentCooldownObject(TooltipCooldown tooltipCooldown);
        protected abstract T GetAbilityTalentChargesObject(TooltipCharges tooltipCharges);

        protected void AddLocalizedGameString(Unit unit)
        {
            GameStringWriter.AddUnitName(unit.Id, unit.Name);

            string unitDescription = GetTooltip(unit.Description, FileOutputOptions.DescriptionType);
            if (!string.IsNullOrEmpty(unitDescription))
                GameStringWriter.AddUnitDescription(unit.Id, unitDescription);
        }

        protected void AddLocalizedGameString(Hero hero)
        {
            GameStringWriter.AddUnitName(hero.Id, hero.Name);
            GameStringWriter.AddUnitDifficulty(hero.Id, hero.Difficulty);
            GameStringWriter.AddUnitType(hero.Id, hero.Type);
            GameStringWriter.AddUnitDescription(hero.Id, GetTooltip(hero.Description, FileOutputOptions.DescriptionType));
            GameStringWriter.AddHeroTitle(hero.Id, hero.Title);
            GameStringWriter.AddHeroSearchText(hero.Id, hero.SearchText);

            if (hero.Roles != null && hero.Roles.Count > 0)
                GameStringWriter.AddUnitRole(hero.Id, string.Join(",", hero.Roles));

            GameStringWriter.AddUnitExpandedRole(hero.Id, hero.ExpandedRole);
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

        protected T HeroPortraits(Hero hero)
        {
            if ((!string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName) || !string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName) ||
                !string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName) || !string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName) ||
                !string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName)) && hero.HeroPortrait != null)
            {
                return GetPortraitObject(hero);
            }

            return null;
        }

        protected T UnitArmor(Unit unit)
        {
            if (unit.Armor != null && unit.Armor.Any())
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

        protected T HeroRatings(Hero hero)
        {
            if (hero.Ratings != null)
            {
                return GetRatingsObject(hero);
            }

            return null;
        }

        protected T UnitWeapons(Unit unit)
        {
            if (unit.Weapons.Any())
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

        protected T UnitSubAbilities(Unit unit)
        {
            if (unit.Abilities?.Count > 0)
            {
                ILookup<string, Ability> linkedAbilities = unit.ParentLinkedAbilities();
                if (linkedAbilities.Count > 0)
                {
                    return GetSubAbilitiesObject(linkedAbilities);
                }
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

        protected T HeroTalents(Hero hero)
        {
            if (hero.Talents?.Count > 0)
            {
                return GetTalentsObject(hero);
            }

            return null;
        }

        protected T Units(Hero hero)
        {
            if (hero.HeroUnits?.Count > 0)
            {
                return GetUnitsObject(hero);
            }

            return null;
        }
    }
}
