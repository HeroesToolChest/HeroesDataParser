using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.ExtractorData
{
    public class DataUnit : DataExtractorBase<Unit?, UnitParser>, IData
    {
        public DataUnit(UnitParser parser)
            : base(parser)
        {
        }

        public override string Name => "units";

        protected override void Validation(Unit? data)
        {
            if (data is null)
                return;

            if (data.Id.EndsWith("dummy", StringComparison.OrdinalIgnoreCase))
                return;

            if (data.Attributes.Contains("Heroic"))
            {
                if (!data.Attributes.Contains("ImmuneToAOE"))
                {
                    if (data.Life.LifeMax <= 0)
                        AddWarning($"{nameof(data.Life)} is 0");

                    if (data.Sight <= 0)
                        AddWarning($"{nameof(data.Sight)} is 0");

                    if (data.Speed <= 0 && data.DamageType != "Structure" && !data.Attributes.Contains("Structure"))
                        AddWarning($"{nameof(data.Speed)} is 0");

                    if (!data.Attributes.Any())
                        AddWarning($"{nameof(data.Attributes)} is 0");
                }

                VerifyAbilities(data);
            }
            else
            {
                VerifyTooltipDescription(data);
            }

            VerifyAbilitiesCount(data.PrimaryAbilities().ToList());
            VerifyAbilitiesCount(data.SubAbilities().ToList());

            VerifyWeapons(data);
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
                if (!string.IsNullOrEmpty(ability.Tooltip.ShortTooltip?.RawDescription))
                {
                    if (ability.Tooltip.ShortTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.ShortTooltip)} failed to parse correctly");
                    else if (ability.Tooltip.ShortTooltip.HasErrorTag)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.ShortTooltip)} contains an error tag");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.FullTooltip?.RawDescription))
                {
                    if (ability.Tooltip.FullTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.FullTooltip)} failed to parse correctly");
                    else if (ability.Tooltip.FullTooltip.HasErrorTag)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.FullTooltip)} contains an error tag");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription))
                {
                    if (ability.Tooltip.Cooldown.CooldownTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Cooldown.CooldownTooltip)} failed to parse correctly");
                    else if (char.IsDigit(ability.Tooltip.Cooldown.CooldownTooltip.PlainText[0]))
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Cooldown.CooldownTooltip)} does not have a prefix");
                    else if (ability.Tooltip.Cooldown.CooldownTooltip.HasErrorTag)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Cooldown.CooldownTooltip)} contains an error tag");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.Energy?.EnergyTooltip?.RawDescription))
                {
                    if (ability.Tooltip.Energy.EnergyTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Energy.EnergyTooltip)} failed to parse correctly");
                    else if (char.IsDigit(ability.Tooltip.Energy.EnergyTooltip.PlainText[0]))
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Energy.EnergyTooltip)} does not have a prefix");
                    else if (ability.Tooltip.Energy.EnergyTooltip.HasErrorTag)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Energy.EnergyTooltip)} contains an error tag");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.Life?.LifeCostTooltip?.RawDescription))
                {
                    if (ability.Tooltip.Life.LifeCostTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Life.LifeCostTooltip)} failed to parse correctly");
                    else if (char.IsDigit(ability.Tooltip.Life.LifeCostTooltip.PlainText[0]))
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Life.LifeCostTooltip)} does not have a prefix");
                    else if (ability.Tooltip.Life.LifeCostTooltip.HasErrorTag)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.Life.LifeCostTooltip)} contains an error tag");
                }
            }
        }

        private void VerifyAbilitiesCount(List<Ability> abilitiesList)
        {
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Q).Count() > 1)
                AddWarning($"has more than 1 {AbilityTypes.Q} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.W).Count() > 1)
                AddWarning($"has more than 1 {AbilityTypes.W} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.E).Count() > 1)
                AddWarning($"has more than 1 {AbilityTypes.E} abilities");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Heroic).Count() > 2)
                AddWarning($"has more than 2 {AbilityTypes.Heroic} abilities");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Z).Count() > 1)
                AddWarning($"has more than 1 {AbilityTypes.Z} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.B).Count() > 1)
                AddWarning($"has more than 1 {AbilityTypes.B} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Trait).Count() > 1)
                AddWarning($"has more than 1 {AbilityTypes.Trait} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Taunt).Count() > 1)
                AddWarning($"has more than 1 {AbilityTypes.Taunt} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Spray).Count() > 1)
                AddWarning($"has more than 1 {AbilityTypes.Spray} ability");
            if (abilitiesList.Where(x => x.AbilityTalentId.AbilityType == AbilityTypes.Dance).Count() > 1)
                AddWarning($"has more than 1 {AbilityTypes.Dance} ability");
        }

        private void VerifyTooltipDescription(Unit unit)
        {
            foreach (Ability ability in unit.Abilities)
            {
                if (!string.IsNullOrEmpty(ability.Tooltip.ShortTooltip?.RawDescription))
                {
                    if (ability.Tooltip.ShortTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.ShortTooltip)} failed to parse correctly");
                    else if (ability.Tooltip.ShortTooltip.HasErrorTag)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.ShortTooltip)} contains an error tag");
                }

                if (!string.IsNullOrEmpty(ability.Tooltip.FullTooltip?.RawDescription))
                {
                    if (ability.Tooltip.FullTooltip.RawDescription == GameStringParser.FailedParsed)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.FullTooltip)} failed to parse correctly");
                    else if (ability.Tooltip.FullTooltip.HasErrorTag)
                        AddWarning($"[{ability.AbilityTalentId.Id}] {nameof(ability.Tooltip.FullTooltip)} contains an error tag");
                }
            }
        }
    }
}
