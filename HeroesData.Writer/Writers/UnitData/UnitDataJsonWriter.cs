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

            JObject unitObject = new JObject();

            if (!string.IsNullOrEmpty(unit.Name) && !FileOutputOptions.IsLocalizedText)
                unitObject.Add("name", unit.Name);
            if (!string.IsNullOrEmpty(unit.HyperlinkId))
                unitObject.Add("hyperlinkId", unit.HyperlinkId);
            if (unit.InnerRadius > 0)
                unitObject.Add("innerRadius", unit.InnerRadius);
            if (unit.Radius > 0)
                unitObject.Add("radius", unit.Radius);
            if (unit.Sight > 0)
                unitObject.Add("sight", unit.Sight);
            if (unit.Speed > 0)
                unitObject.Add("speed", unit.Speed);
            if (!string.IsNullOrEmpty(unit.DamageType) && !FileOutputOptions.IsLocalizedText)
                unitObject.Add("damageType", unit.DamageType);
            if (!string.IsNullOrEmpty(unit.ScalingBehaviorLink))
                unitObject.Add(new JProperty("scalingLinkId", unit.ScalingBehaviorLink));
            if (!string.IsNullOrEmpty(unit.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                unitObject.Add("description", GetTooltip(unit.Description, FileOutputOptions.DescriptionType));
            if (unit.HeroDescriptors.Any())
                unitObject.Add(new JProperty("descriptors", unit.HeroDescriptors));
            if (unit.Attributes.Any())
                unitObject.Add(new JProperty("attributes", unit.Attributes));
            if (!string.IsNullOrEmpty(unit.TargetInfoPanelImageFileName))
                unitObject.Add("image", Path.ChangeExtension(unit.TargetInfoPanelImageFileName?.ToLower(), StaticImageExtension));

            JProperty life = UnitLife(unit);
            if (life != null)
                unitObject.Add(life);

            JProperty energy = UnitEnergy(unit);
            if (energy != null)
                unitObject.Add(energy);

            JProperty armor = UnitArmor(unit);
            if (armor != null)
                unitObject.Add(armor);

            JProperty weapons = UnitWeapons(unit);
            if (weapons != null)
                unitObject.Add(weapons);

            JProperty abilities = UnitAbilities(unit, false);
            if (abilities != null)
                unitObject.Add(abilities);

            return new JProperty(unit.Id, unitObject);
        }

        protected override JProperty GetArmorObject(Unit unit)
        {
            JObject armorObject = new JObject();

            foreach (UnitArmor armor in unit.Armor)
            {
                armorObject.Add(new JProperty(
                    armor.Type.ToLower(),
                    new JObject(
                        new JProperty("basic", armor.BasicArmor),
                        new JProperty("ability", armor.AbilityArmor),
                        new JProperty("splash", armor.SplashArmor))));
            }

            return new JProperty("armor", armorObject);
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
            JObject energyObject = new JObject
            {
                new JProperty("amount", unit.Energy.EnergyMax),
            };

            if (!string.IsNullOrEmpty(unit.Energy.EnergyType))
                energyObject.Add(new JProperty("type", unit.Energy.EnergyType));

            energyObject.Add(new JProperty("regenRate", unit.Energy.EnergyRegenerationRate));

            return new JProperty("energy", energyObject);
        }

        protected override JProperty GetAbilitiesObject(Unit unit, bool isSubAbilities)
        {
            JObject abilityObject = new JObject();

            if (isSubAbilities)
            {
                IEnumerable<Ability> basicAbilities = unit.SubAbilities(AbilityTier.Basic);
                if (basicAbilities.Any())
                {
                    abilityObject.Add(new JProperty(
                        "basic",
                        new JArray(
                            from abil in basicAbilities
                            orderby abil.AbilityType ascending
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> heroicAbilities = unit.SubAbilities(AbilityTier.Heroic);
                if (heroicAbilities.Any())
                {
                    abilityObject.Add(new JProperty(
                        "heroic",
                        new JArray(
                            from abil in heroicAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> traitAbilities = unit.SubAbilities(AbilityTier.Trait);
                if (traitAbilities.Any())
                {
                    abilityObject.Add(new JProperty(
                        "trait",
                        new JArray(
                            from abil in traitAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> mountAbilities = unit.SubAbilities(AbilityTier.Mount);
                if (mountAbilities.Any())
                {
                    abilityObject.Add(new JProperty(
                        "mount",
                        new JArray(
                            from abil in mountAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> activableAbilities = unit.SubAbilities(AbilityTier.Activable);
                if (activableAbilities.Any())
                {
                    abilityObject.Add(new JProperty(
                        "activable",
                        new JArray(
                            from abil in activableAbilities
                            orderby abil.AbilityType ascending
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> hearthAbilities = unit.SubAbilities(AbilityTier.Hearth);
                if (hearthAbilities.Any())
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
                IEnumerable<Ability> basicAbilities = unit.PrimaryAbilities(AbilityTier.Basic);
                if (basicAbilities.Any())
                {
                    abilityObject.Add(new JProperty(
                        "basic",
                        new JArray(
                            from abil in basicAbilities
                            orderby abil.AbilityType ascending
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> heroicAbilities = unit.PrimaryAbilities(AbilityTier.Heroic);
                if (heroicAbilities.Any())
                {
                    abilityObject.Add(new JProperty(
                        "heroic",
                        new JArray(
                            from abil in heroicAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> traitAbilities = unit.PrimaryAbilities(AbilityTier.Trait);
                if (traitAbilities.Any())
                {
                    abilityObject.Add(new JProperty(
                        "trait",
                        new JArray(
                            from abil in traitAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> mountAbilities = unit.PrimaryAbilities(AbilityTier.Mount);
                if (mountAbilities.Any())
                {
                    abilityObject.Add(new JProperty(
                        "mount",
                        new JArray(
                            from abil in mountAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> activableAbilities = unit.PrimaryAbilities(AbilityTier.Activable);
                if (activableAbilities.Any())
                {
                    abilityObject.Add(new JProperty(
                        "activable",
                        new JArray(
                            from abil in activableAbilities
                            orderby abil.AbilityType ascending
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> hearthAbilities = unit.PrimaryAbilities(AbilityTier.Hearth);
                if (hearthAbilities.Any())
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

            if (!string.IsNullOrEmpty(abilityTalentBase.IconFileName))
                info.Add("icon", Path.ChangeExtension(abilityTalentBase.IconFileName?.ToLower(), ".png"));

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
            JArray weaponArray = new JArray();

            foreach (UnitWeapon weapon in unit.Weapons)
            {
                JObject weaponObject = new JObject
                {
                    new JProperty("nameId", weapon.WeaponNameId),
                    new JProperty("range", weapon.Range),
                    new JProperty("period", weapon.Period),
                    new JProperty("damage", weapon.Damage),
                    new JProperty("damageScale", weapon.DamageScaling),
                };

                if (weapon.AttributeFactors.Any())
                {
                    JObject attributeFactorOjbect = new JObject();

                    foreach (WeaponAttributeFactor item in weapon.AttributeFactors)
                    {
                        attributeFactorOjbect.Add(item.Type.ToLower(), item.Value);
                    }

                    weaponObject.Add("damageFactor", attributeFactorOjbect);
                }

                weaponArray.Add(weaponObject);
            }

            return new JProperty("weapons", weaponArray);
        }
    }
}
