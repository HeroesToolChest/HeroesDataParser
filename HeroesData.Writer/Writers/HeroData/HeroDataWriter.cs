using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using System.Linq;

namespace HeroesData.FileWriter.Writers.HeroData
{
    internal abstract class HeroDataWriter<T, TU, TExtractable> : WriterBase<TExtractable, T>
        where T : class
        where TU : class
        where TExtractable : IExtractable
    {
        protected HeroDataWriter(FileOutputType fileOutputType)
            : base(nameof(HeroData), fileOutputType)
        {
        }

        protected HeroDataWriter(string type, FileOutputType fileOutputType)
            : base(type, fileOutputType)
        {
        }

        protected abstract T UnitElement(Unit unit);
        protected abstract T GetPortraitObject(Hero hero);
        protected abstract T GetUnitPortraitObject(Unit unit);
        protected abstract T GetArmorObject(Unit unit);
        protected abstract T GetLifeObject(Unit unit);
        protected abstract T GetEnergyObject(Unit unit);
        protected abstract T GetShieldObject(Unit unit);
        protected abstract T GetRatingsObject(Hero hero);
        protected abstract T GetWeaponsObject(Unit unit);
        protected abstract T GetAbilitiesObject(Unit unit);
        protected abstract T GetSubAbilitiesObject(ILookup<AbilityTalentId, Ability> linkedAbilities);
        protected abstract T GetTalentsObject(Hero hero);
        protected abstract T GetUnitsObject(Hero hero);
        protected abstract TU AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase);
        protected abstract TU AbilityInfoElement(Ability ability);
        protected abstract TU TalentInfoElement(Talent talent);
        protected abstract T GetAbilityTalentLifeCostObject(TooltipLife tooltipLife);
        protected abstract T GetAbilityTalentEnergyCostObject(TooltipEnergy tooltipEnergy);
        protected abstract T GetAbilityTalentCooldownObject(TooltipCooldown tooltipCooldown);
        protected abstract T GetAbilityTalentChargesObject(TooltipCharges tooltipCharges);

        protected virtual void AddLocalizedGameString(Unit unit)
        {
            GameStringWriter.AddUnitName(unit.Id, unit.Name);
            GameStringWriter.AddUnitDescription(unit.Id, GetTooltip(unit.Description, FileOutputOptions.DescriptionType));
            GameStringWriter.AddUnitLifeType(unit.Id, unit.Life.LifeType);
            GameStringWriter.AddUnitEnergyType(unit.Id, unit.Energy.EnergyType);
            GameStringWriter.AddUnitShieldType(unit.Id, unit.Shield.ShieldType);
        }

        protected void AddLocalizedGameString(Hero hero)
        {
            GameStringWriter.AddUnitName(hero.Id, hero.Name);
            GameStringWriter.AddUnitDifficulty(hero.Id, hero.Difficulty);
            GameStringWriter.AddUnitType(hero.Id, hero.Type);
            GameStringWriter.AddUnitDescription(hero.Id, GetTooltip(hero.Description, FileOutputOptions.DescriptionType));
            GameStringWriter.AddUnitLifeType(hero.Id, hero.Life.LifeType);
            GameStringWriter.AddUnitEnergyType(hero.Id, hero.Energy.EnergyType);
            GameStringWriter.AddUnitShieldType(hero.Id, hero.Shield.ShieldType);
            GameStringWriter.AddHeroTitle(hero.Id, hero.Title);
            GameStringWriter.AddHeroSearchText(hero.Id, hero.SearchText);

            if (hero.RolesCount > 0)
                GameStringWriter.AddUnitRole(hero.Id, string.Join(",", hero.Roles));

            GameStringWriter.AddUnitExpandedRole(hero.Id, hero.ExpandedRole);
        }

        protected virtual void AddLocalizedGameString(AbilityTalentBase abilityTalentBase)
        {
            GameStringWriter.AddAbilityTalentName(abilityTalentBase.AbilityTalentId.Id, abilityTalentBase.Name);

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.Life?.LifeCostTooltip?.RawDescription))
                GameStringWriter.AddAbilityTalentLifeTooltip(abilityTalentBase.AbilityTalentId.Id, GetTooltip(abilityTalentBase.Tooltip.Life.LifeCostTooltip, FileOutputOptions.DescriptionType));

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.Energy?.EnergyTooltip?.RawDescription))
                GameStringWriter.AddAbilityTalentEnergyTooltip(abilityTalentBase.AbilityTalentId.Id, GetTooltip(abilityTalentBase.Tooltip.Energy.EnergyTooltip, FileOutputOptions.DescriptionType));

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.Cooldown?.CooldownTooltip?.RawDescription))
                GameStringWriter.AddAbilityTalentCooldownTooltip(abilityTalentBase.AbilityTalentId.Id, GetTooltip(abilityTalentBase.Tooltip.Cooldown.CooldownTooltip, FileOutputOptions.DescriptionType));

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.ShortTooltip?.RawDescription))
                GameStringWriter.AddAbilityTalentShortTooltip(abilityTalentBase.AbilityTalentId.Id, GetTooltip(abilityTalentBase.Tooltip.ShortTooltip, FileOutputOptions.DescriptionType));

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip?.FullTooltip?.RawDescription))
                GameStringWriter.AddAbilityTalentFullTooltip(abilityTalentBase.AbilityTalentId.Id, GetTooltip(abilityTalentBase.Tooltip.FullTooltip, FileOutputOptions.DescriptionType));
        }

        protected T HeroPortraits(Hero hero)
        {
            if ((!string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName) || !string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName) ||
                !string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName) || !string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName) ||
                !string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName) || !string.IsNullOrEmpty(hero.HeroPortrait.DraftScreenFileName) ||
                hero.HeroPortrait.PartyFrameFileName.Count > 0 || !string.IsNullOrEmpty(hero.UnitPortrait.MiniMapIconFileName) ||
                !string.IsNullOrEmpty(hero.UnitPortrait.TargetInfoPanelFileName)) && hero.HeroPortrait != null)
            {
                return GetPortraitObject(hero);
            }

            return null;
        }

        protected T UnitPortraits(Unit unit)
        {
            if ((!string.IsNullOrEmpty(unit.UnitPortrait.MiniMapIconFileName) || !string.IsNullOrEmpty(unit.UnitPortrait.TargetInfoPanelFileName)) && unit.UnitPortrait != null)
            {
                return GetUnitPortraitObject(unit);
            }

            return null;
        }

        protected virtual T UnitArmor(Unit unit)
        {
            if (unit.ArmorCount > 0)
            {
                return GetArmorObject(unit);
            }

            return null;
        }

        protected virtual T UnitLife(Unit unit)
        {
            if (unit.Life.LifeMax > 0)
            {
                return GetLifeObject(unit);
            }

            return null;
        }

        protected virtual T UnitEnergy(Unit unit)
        {
            if (unit.Energy.EnergyMax > 0)
            {
                return GetEnergyObject(unit);
            }

            return null;
        }

        protected virtual T UnitShield(Unit unit)
        {
            if (unit.Shield.ShieldMax > 0)
            {
                return GetShieldObject(unit);
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

        protected virtual T UnitWeapons(Unit unit)
        {
            if (unit.WeaponsCount > 0)
            {
                return GetWeaponsObject(unit);
            }

            return null;
        }

        protected virtual T UnitAbilities(Unit unit)
        {
            if (unit.PrimaryAbilities().Any())
            {
                return GetAbilitiesObject(unit);
            }

            return null;
        }

        protected T UnitSubAbilities(Unit unit)
        {
            if (unit.SubAbilities().Any())
            {
                ILookup<AbilityTalentId, Ability> linkedAbilities = unit.ParentLinkedAbilities();
                if (linkedAbilities.Count > 0)
                {
                    return GetSubAbilitiesObject(linkedAbilities);
                }
            }

            return null;
        }

        protected virtual T UnitAbilityTalentLifeCost(TooltipLife tooltipLife)
        {
            if (!string.IsNullOrEmpty(tooltipLife?.LifeCostTooltip?.RawDescription) && !FileOutputOptions.IsLocalizedText)
            {
                return GetAbilityTalentLifeCostObject(tooltipLife);
            }

            return null;
        }

        protected virtual T UnitAbilityTalentEnergyCost(TooltipEnergy tooltipEnergy)
        {
            if (!string.IsNullOrEmpty(tooltipEnergy?.EnergyTooltip?.RawDescription) && !FileOutputOptions.IsLocalizedText)
            {
                return GetAbilityTalentEnergyCostObject(tooltipEnergy);
            }

            return null;
        }

        protected virtual T UnitAbilityTalentCooldown(TooltipCooldown tooltipCooldown)
        {
            if (!string.IsNullOrEmpty(tooltipCooldown?.CooldownTooltip?.RawDescription) && !FileOutputOptions.IsLocalizedText)
            {
                return GetAbilityTalentCooldownObject(tooltipCooldown);
            }

            return null;
        }

        protected virtual T UnitAbilityTalentCharges(TooltipCharges tooltipCharges)
        {
            if (tooltipCharges.HasCharges)
            {
                return GetAbilityTalentChargesObject(tooltipCharges);
            }

            return null;
        }

        protected T HeroTalents(Hero hero)
        {
            if (hero.TalentsCount > 0)
            {
                return GetTalentsObject(hero);
            }

            return null;
        }

        protected T Units(Hero hero)
        {
            if (hero.HeroUnitCount > 0)
            {
                return GetUnitsObject(hero);
            }

            return null;
        }
    }
}
