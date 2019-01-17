using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using System.Linq;

namespace HeroesData.FileWriter.Writer.HeroData
{
    internal abstract class HeroDataWriter<T, TU> : WriterBase<Hero, T>
        where T : class
        where TU : class
    {
        protected HeroDataWriter(FileOutputType fileOutputType)
            : base(nameof(HeroData).ToLowerInvariant(), fileOutputType)
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
            LocalizedGameString.AddUnitName(unit.ShortName, unit.Name);
            LocalizedGameString.AddUnitType(unit.ShortName, unit.Type);

            string unitDescription = GetTooltip(unit.Description, FileSettings.DescriptionType);
            if (!string.IsNullOrEmpty(unitDescription))
                LocalizedGameString.AddUnitDescription(unit.ShortName, unitDescription);
        }

        protected void AddLocalizedGameString(Hero hero)
        {
            LocalizedGameString.AddUnitName(hero.ShortName, hero.Name);
            LocalizedGameString.AddUnitDifficulty(hero.ShortName, hero.Difficulty);
            LocalizedGameString.AddUnitType(hero.ShortName, hero.Type);
            LocalizedGameString.AddUnitDescription(hero.ShortName, GetTooltip(hero.Description, FileSettings.DescriptionType));
            LocalizedGameString.AddHeroTitle(hero.ShortName, hero.Title);
            LocalizedGameString.AddHeroSearchText(hero.ShortName, hero.Title);

            if (hero.Roles != null && hero.Roles.Count > 0)
                LocalizedGameString.AddUnitRole(hero.ShortName, string.Join(",", hero.Roles));
        }

        protected void AddLocalizedGameString(AbilityTalentBase abilityTalentBase)
        {
            LocalizedGameString.AddAbilityTalentName(abilityTalentBase.ReferenceNameId, abilityTalentBase.Name);

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.Life?.LifeCostTooltip?.RawDescription))
                LocalizedGameString.AddAbilityTalentLifeTooltip(abilityTalentBase.ReferenceNameId, GetTooltip(abilityTalentBase.Tooltip.Life.LifeCostTooltip, FileSettings.DescriptionType));

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.Energy?.EnergyTooltip?.RawDescription))
                LocalizedGameString.AddAbilityTalentEnergyTooltip(abilityTalentBase.ReferenceNameId, GetTooltip(abilityTalentBase.Tooltip.Energy.EnergyTooltip, FileSettings.DescriptionType));

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.Cooldown?.CooldownTooltip?.RawDescription))
                LocalizedGameString.AddAbilityTalentCooldownTooltip(abilityTalentBase.ReferenceNameId, GetTooltip(abilityTalentBase.Tooltip.Cooldown.CooldownTooltip, FileSettings.DescriptionType));

            LocalizedGameString.AddAbilityTalentShortTooltip(abilityTalentBase.ShortTooltipNameId, GetTooltip(abilityTalentBase.Tooltip.ShortTooltip, FileSettings.DescriptionType));
            LocalizedGameString.AddAbilityTalentFullTooltip(abilityTalentBase.FullTooltipNameId, GetTooltip(abilityTalentBase.Tooltip.FullTooltip, FileSettings.DescriptionType));
        }

        protected T HeroPortraits(Hero hero)
        {
            if ((FileSettings.HeroSelectPortrait || FileSettings.LeaderboardPortrait ||
                FileSettings.LoadingPortraitPortrait || FileSettings.PartyPanelPortrait ||
                FileSettings.TargetPortrait) &&
                (!string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName) || !string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName) ||
                !string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName) || !string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName) ||
                !string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName)) && hero.HeroPortrait != null)
            {
                return GetPortraitObject(hero);
            }

            return null;
        }

        protected T UnitArmor(Unit unit)
        {
            if (unit.Armor != null)
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
            if (FileSettings.IncludeWeapons && unit.Weapons?.Count > 0)
            {
                return GetWeaponsObject(unit);
            }

            return null;
        }

        protected T UnitAbilities(Unit unit, bool isSubAbilities)
        {
            if (FileSettings.IncludeAbilities && unit.Abilities?.Count > 0)
            {
                return GetAbilitiesObject(unit, isSubAbilities);
            }

            return null;
        }

        protected T UnitSubAbilities(Unit unit)
        {
            if (FileSettings.IncludeSubAbilities && unit.Abilities?.Count > 0)
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
            if (!string.IsNullOrEmpty(tooltipLife?.LifeCostTooltip?.RawDescription) && !IsLocalizedText)
            {
                return GetAbilityTalentLifeCostObject(tooltipLife);
            }

            return null;
        }

        protected T UnitAbilityTalentEnergyCost(TooltipEnergy tooltipEnergy)
        {
            if (!string.IsNullOrEmpty(tooltipEnergy?.EnergyTooltip?.RawDescription) && !IsLocalizedText)
            {
                return GetAbilityTalentEnergyCostObject(tooltipEnergy);
            }

            return null;
        }

        protected T UnitAbilityTalentCooldown(TooltipCooldown tooltipCooldown)
        {
            if (!string.IsNullOrEmpty(tooltipCooldown?.CooldownTooltip?.RawDescription) && !IsLocalizedText)
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
            if (FileSettings.IncludeTalents && hero.Talents?.Count > 0)
            {
                return GetTalentsObject(hero);
            }

            return null;
        }

        protected T Units(Hero hero)
        {
            if (FileSettings.IncludeHeroUnits && hero.HeroUnits?.Count > 0)
            {
                return GetUnitsObject(hero);
            }

            return null;
        }
    }
}
