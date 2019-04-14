using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.FileWriter.Writers.UnitData
{
    internal class UnitDataJsonWriter : UnitDataWriter<JProperty, JObject>
    {
        public UnitDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(Unit unit)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(unit);

            JObject heroObject = new JObject();

            if (!string.IsNullOrEmpty(unit.Name) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("name", unit.Name);
            if (!string.IsNullOrEmpty(unit.HyperlinkId))
                heroObject.Add("hyperlinkId", unit.HyperlinkId);
            if (unit.InnerRadius > 0)
                heroObject.Add("innerRadius", unit.InnerRadius);
            if (unit.Radius > 0)
                heroObject.Add("radius", unit.Radius);
            if (unit.Sight > 0)
                heroObject.Add("sight", unit.Sight);
            if (unit.Speed > 0)
                heroObject.Add("speed", unit.Speed);
            if (!string.IsNullOrEmpty(unit.DamageType) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("damageType", unit.DamageType);
            if (!string.IsNullOrEmpty(unit.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("description", GetTooltip(unit.Description, FileOutputOptions.DescriptionType));
            if (unit.HeroDescriptors.Count > 0)
                heroObject.Add(new JProperty("descriptors", unit.HeroDescriptors));
            if (unit.Attributes.Count > 0)
                heroObject.Add(new JProperty("attributes", unit.Attributes));
            if (unit.TargetInfoPanelImageFileNames.Count > 0)
                heroObject.Add(new JProperty("images", unit.TargetInfoPanelImageFileNames.Select(x => Path.ChangeExtension(x, StaticImageExtension))));

            JProperty life = UnitLife(unit);
            if (life != null)
                heroObject.Add(life);

            JProperty energy = UnitEnergy(unit);
            if (energy != null)
                heroObject.Add(energy);

            JProperty armor = UnitArmor(unit);
            if (armor != null)
                heroObject.Add(armor);

            JProperty weapons = UnitWeapons(unit);
            if (weapons != null)
                heroObject.Add(weapons);

            JProperty abilities = UnitAbilities(unit, false);
            if (abilities != null)
                heroObject.Add(abilities);

            return new JProperty(unit.Id, heroObject);
        }

        protected override JProperty GetArmorObject(Unit unit)
        {
            return new JProperty(
                "armor",
                new JObject(
                    new JProperty("physical", unit.Armor.PhysicalArmor),
                    new JProperty("spell", unit.Armor.SpellArmor)));
        }

        protected override JProperty GetLifeObject(Unit unit)
        {
            return new JProperty(
                "life",
                new JObject(
                    new JProperty("amount", unit.Life.LifeMax),
                    new JProperty("scale", unit.Life.LifeScaling),
                    new JProperty("regenRate", unit.Life.LifeRegenerationRate),
                    new JProperty("regenScale", unit.Life.LifeRegenerationRateScaling)));
        }

        protected override JProperty GetEnergyObject(Unit unit)
        {
            return new JProperty(
                "energy",
                new JObject(
                    new JProperty("amount", unit.Energy.EnergyMax),
                    new JProperty("type", unit.Energy.EnergyType.ToString()),
                    new JProperty("regenRate", unit.Energy.EnergyRegenerationRate)));
        }

        protected override JProperty GetAbilitiesObject(Unit unit, bool isSubAbilities)
        {
            JObject abilityObject = new JObject();

            if (isSubAbilities)
            {
                ICollection<Ability> basicAbilities = unit.SubAbilities(AbilityTier.Basic);
                if (basicAbilities?.Count > 0)
                {
                    abilityObject.Add(new JProperty(
                        "basic",
                        new JArray(
                            from abil in basicAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                ICollection<Ability> heroicAbilities = unit.SubAbilities(AbilityTier.Heroic);
                if (heroicAbilities?.Count > 0)
                {
                    abilityObject.Add(new JProperty(
                        "heroic",
                        new JArray(
                            from abil in heroicAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                ICollection<Ability> traitAbilities = unit.SubAbilities(AbilityTier.Trait);
                if (traitAbilities?.Count > 0)
                {
                    abilityObject.Add(new JProperty(
                        "trait",
                        new JArray(
                            from abil in traitAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                ICollection<Ability> mountAbilities = unit.SubAbilities(AbilityTier.Mount);
                if (mountAbilities?.Count > 0)
                {
                    abilityObject.Add(new JProperty(
                        "mount",
                        new JArray(
                            from abil in mountAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                ICollection<Ability> activableAbilities = unit.SubAbilities(AbilityTier.Activable);
                if (activableAbilities?.Count > 0)
                {
                    abilityObject.Add(new JProperty(
                        "activable",
                        new JArray(
                            from abil in activableAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                ICollection<Ability> hearthAbilities = unit.SubAbilities(AbilityTier.Hearth);
                if (hearthAbilities?.Count > 0)
                {
                    abilityObject.Add(new JProperty(
                        "hearth",
                        new JArray(
                            from abil in hearthAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }
            }
            else
            {
                ICollection<Ability> basicAbilities = unit.PrimaryAbilities(AbilityTier.Basic);
                if (basicAbilities?.Count > 0)
                {
                    abilityObject.Add(new JProperty(
                        "basic",
                        new JArray(
                            from abil in basicAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                ICollection<Ability> heroicAbilities = unit.PrimaryAbilities(AbilityTier.Heroic);
                if (heroicAbilities?.Count > 0)
                {
                    abilityObject.Add(new JProperty(
                        "heroic",
                        new JArray(
                            from abil in heroicAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                ICollection<Ability> traitAbilities = unit.PrimaryAbilities(AbilityTier.Trait);
                if (traitAbilities?.Count > 0)
                {
                    abilityObject.Add(new JProperty(
                        "trait",
                        new JArray(
                            from abil in traitAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                ICollection<Ability> mountAbilities = unit.PrimaryAbilities(AbilityTier.Mount);
                if (mountAbilities?.Count > 0)
                {
                    abilityObject.Add(new JProperty(
                        "mount",
                        new JArray(
                            from abil in mountAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                ICollection<Ability> activableAbilities = unit.PrimaryAbilities(AbilityTier.Activable);
                if (activableAbilities?.Count > 0)
                {
                    abilityObject.Add(new JProperty(
                        "activable",
                        new JArray(
                            from abil in activableAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                ICollection<Ability> hearthAbilities = unit.PrimaryAbilities(AbilityTier.Hearth);
                if (hearthAbilities?.Count > 0)
                {
                    abilityObject.Add(new JProperty(
                        "hearth",
                        new JArray(
                            from abil in hearthAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }
            }

            return new JProperty("abilities", abilityObject);
        }

        protected override JProperty GetAbilityTalentChargesObject(TooltipCharges tooltipCharges)
        {
            JObject charges = new JObject
            {
                { "countMax", tooltipCharges.CountMax },
            };

            if (tooltipCharges.CountUse.HasValue)
                charges.Add("countUse", tooltipCharges.CountUse.Value);

            if (tooltipCharges.CountStart.HasValue)
                charges.Add("countStart", tooltipCharges.CountStart.Value);

            if (tooltipCharges.IsHideCount.HasValue)
                charges.Add("hideCount", tooltipCharges.IsHideCount.Value);

            if (tooltipCharges.RecastCooldown.HasValue)
                charges.Add("recastCooldown", tooltipCharges.RecastCooldown.Value);

            return new JProperty("charges", charges);
        }

        protected override JProperty GetAbilityTalentCooldownObject(TooltipCooldown tooltipCooldown)
        {
            return new JProperty("cooldownTooltip", GetTooltip(tooltipCooldown.CooldownTooltip, FileOutputOptions.DescriptionType));
        }

        protected override JProperty GetAbilityTalentEnergyCostObject(TooltipEnergy tooltipEnergy)
        {
            return new JProperty("energyTooltip", GetTooltip(tooltipEnergy.EnergyTooltip, FileOutputOptions.DescriptionType));
        }

        protected override JProperty GetAbilityTalentLifeCostObject(TooltipLife tooltipLife)
        {
            return new JProperty("lifeTooltip", GetTooltip(tooltipLife.LifeCostTooltip, FileOutputOptions.DescriptionType));
        }

        protected override JObject AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(abilityTalentBase);

            JObject info = new JObject
            {
                { "nameId", abilityTalentBase.ReferenceNameId },
            };

            if (!FileOutputOptions.IsLocalizedText)
                info.Add("name", abilityTalentBase.Name);

            if (!string.IsNullOrEmpty(abilityTalentBase.ShortTooltipNameId))
                info.Add("shortTooltipId", abilityTalentBase.ShortTooltipNameId);

            if (!string.IsNullOrEmpty(abilityTalentBase.FullTooltipNameId))
                info.Add("fullTooltipId", abilityTalentBase.FullTooltipNameId);

            info.Add("icon", Path.ChangeExtension(abilityTalentBase.IconFileName, ".png"));

            if (abilityTalentBase.Tooltip.Cooldown.ToggleCooldown.HasValue)
                info.Add("toggleCooldown", abilityTalentBase.Tooltip.Cooldown.ToggleCooldown.Value);

            JProperty life = UnitAbilityTalentLifeCost(abilityTalentBase.Tooltip.Life);
            if (life != null)
                info.Add(life);

            JProperty energy = UnitAbilityTalentEnergyCost(abilityTalentBase.Tooltip.Energy);
            if (energy != null)
                info.Add(energy);

            JProperty charges = UnitAbilityTalentCharges(abilityTalentBase.Tooltip.Charges);
            if (charges != null)
                info.Add(charges);

            JProperty cooldown = UnitAbilityTalentCooldown(abilityTalentBase.Tooltip.Cooldown);
            if (cooldown != null)
                info.Add(cooldown);

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip.ShortTooltip?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                info.Add("shortTooltip", GetTooltip(abilityTalentBase.Tooltip.ShortTooltip, FileOutputOptions.DescriptionType));

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip.FullTooltip?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                info.Add("fullTooltip", GetTooltip(abilityTalentBase.Tooltip.FullTooltip, FileOutputOptions.DescriptionType));

            info.Add("abilityType", abilityTalentBase.AbilityType.ToString());

            if (abilityTalentBase.IsActive)
                info.Add("isActive", abilityTalentBase.IsActive);

            if (abilityTalentBase.IsQuest)
                info.Add("isQuest", abilityTalentBase.IsQuest);

            return info;
        }

        protected override JProperty GetWeaponsObject(Unit unit)
        {
            return new JProperty(
                "weapons",
                new JArray(
                    from w in unit.Weapons
                    select new JObject(
                        new JProperty("nameId", w.WeaponNameId),
                        new JProperty("range", w.Range),
                        new JProperty("period", w.Period),
                        new JProperty("damage", w.Damage),
                        new JProperty("damageScale", w.DamageScaling))));
        }
    }
}
