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
            if (unit.KillXP > 0)
                unitObject.Add("killXP", unit.KillXP);
            if (!string.IsNullOrEmpty(unit.DamageType) && !FileOutputOptions.IsLocalizedText)
                unitObject.Add("damageType", unit.DamageType);
            if (!string.IsNullOrEmpty(unit.ScalingBehaviorLink))
                unitObject.Add(new JProperty("scalingLinkId", unit.ScalingBehaviorLink));
            if (!string.IsNullOrEmpty(unit.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                unitObject.Add("description", GetTooltip(unit.Description, FileOutputOptions.DescriptionType));
            if (unit.HeroDescriptorsCount > 0)
                unitObject.Add(new JProperty("descriptors", unit.HeroDescriptors.OrderBy(x => x)));
            if (unit.AttributesCount > 0)
                unitObject.Add(new JProperty("attributes", unit.Attributes.OrderBy(x => x)));
            if (unit.UnitIdsCount > 0)
                unitObject.Add(new JProperty("units", unit.UnitIds.OrderBy(x => x)));

            JProperty portraits = UnitPortraits(unit);
            if (portraits != null)
                unitObject.Add(portraits);

            JProperty life = UnitLife(unit);
            if (life != null)
                unitObject.Add(life);

            JProperty shield = UnitShield(unit);
            if (shield != null)
                unitObject.Add(shield);

            JProperty energy = UnitEnergy(unit);
            if (energy != null)
                unitObject.Add(energy);

            JProperty armor = UnitArmor(unit);
            if (armor != null)
                unitObject.Add(armor);

            JProperty weapons = UnitWeapons(unit);
            if (weapons != null)
                unitObject.Add(weapons);

            JProperty abilities = UnitAbilities(unit);
            if (abilities != null)
                unitObject.Add(abilities);

            JProperty subAbilities = UnitSubAbilities(unit);
            if (subAbilities != null)
                unitObject.Add(subAbilities);

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
            JObject lifeObject = new JObject
            {
                new JProperty("amount", unit.Life.LifeMax),
                new JProperty("scale", unit.Life.LifeScaling),
            };

            if (!FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(unit.Life.LifeType))
                lifeObject.Add(new JProperty("type", unit.Life.LifeType));

            lifeObject.Add(new JProperty("regenRate", unit.Life.LifeRegenerationRate));
            lifeObject.Add(new JProperty("regenScale", unit.Life.LifeRegenerationRateScaling));

            return new JProperty("life", lifeObject);
        }

        protected override JProperty GetEnergyObject(Unit unit)
        {
            JObject energyObject = new JObject
            {
                new JProperty("amount", unit.Energy.EnergyMax),
            };

            if (!FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(unit.Energy.EnergyType))
                energyObject.Add(new JProperty("type", unit.Energy.EnergyType));

            energyObject.Add(new JProperty("regenRate", unit.Energy.EnergyRegenerationRate));

            return new JProperty("energy", energyObject);
        }

        protected override JProperty GetShieldObject(Unit unit)
        {
            JObject shieldObject = new JObject
            {
                new JProperty("amount", unit.Shield.ShieldMax),
                new JProperty("scale", unit.Shield.ShieldScaling),
            };

            if (!FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(unit.Shield.ShieldType))
                shieldObject.Add(new JProperty("type", unit.Shield.ShieldType));

            shieldObject.Add(new JProperty("regenDelay", unit.Shield.ShieldRegenerationDelay));
            shieldObject.Add(new JProperty("regenRate", unit.Shield.ShieldRegenerationRate));
            shieldObject.Add(new JProperty("regenScale", unit.Shield.ShieldRegenerationRateScaling));

            return new JProperty("shield", shieldObject);
        }

        protected override JProperty GetAbilitiesObject(Unit unit)
        {
            JObject abilityObject = new JObject();

            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Basic), "basic");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Heroic), "heroic");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Trait), "trait");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Mount), "mount");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Activable), "activable");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Hearth), "hearth");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Taunt), "taunt");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Dance), "dance");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Spray), "spray");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Voice), "voice");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.MapMechanic), "mapMechanic");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Interact), "interact");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Action), "action");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Hidden), "hidden");
            SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Unknown), "unknown");

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
                { "nameId", abilityTalentBase.AbilityTalentId.ReferenceId },
            };

            if (!string.IsNullOrEmpty(abilityTalentBase.AbilityTalentId.ButtonId))
                info.Add("buttonId", abilityTalentBase.AbilityTalentId.ButtonId);

            if (!string.IsNullOrEmpty(abilityTalentBase.Name) && !FileOutputOptions.IsLocalizedText)
                info.Add("name", abilityTalentBase.Name);

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

            info.Add("abilityType", abilityTalentBase.AbilityTalentId.AbilityType.ToString());

            if (abilityTalentBase.IsActive)
                info.Add("isActive", abilityTalentBase.IsActive);

            if (abilityTalentBase.AbilityTalentId.IsPassive)
                info.Add("isPassive", abilityTalentBase.AbilityTalentId.IsPassive);

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

        protected override JProperty GetSubAbilitiesObject(ILookup<AbilityTalentId, Ability> linkedAbilities)
        {
            JObject parentLinkObject = new JObject();

            IEnumerable<AbilityTalentId> parentLinks = linkedAbilities.Select(x => x.Key);
            foreach (AbilityTalentId parent in parentLinks)
            {
                JObject abilities = new JObject();

                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Basic), "basic");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Heroic), "heroic");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Trait), "trait");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Mount), "mount");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Activable), "activable");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Hearth), "hearth");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Taunt), "taunt");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Dance), "dance");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Spray), "spray");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Voice), "voice");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.MapMechanic), "mapMechanic");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Interact), "interact");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Action), "action");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Hidden), "hidden");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Unknown), "unknown");

                if (abilities.Count > 0)
                {
                    if (parent.AbilityType != AbilityType.Unknown)
                    {
                        if (parent.IsPassive)
                            parentLinkObject.Add(new JProperty(parent.Id, abilities));
                        else
                            parentLinkObject.Add(new JProperty($"{parent.ReferenceId}|{parent.ButtonId}|{parent.AbilityType}", abilities));
                    }
                    else
                    {
                        parentLinkObject.Add(new JProperty($"{parent.ReferenceId}|{parent.ButtonId}", abilities));
                    }
                }
            }

            if (parentLinkObject.Count > 0)
                return new JProperty("subAbilities", new JArray(new JObject(parentLinkObject)));

            return null;
        }

        protected void SetAbilities(JObject abilityObject, IEnumerable<Ability> abilities, string propertyName)
        {
            if (abilities.Any())
            {
                abilityObject.Add(new JProperty(
                    propertyName,
                    new JArray(
                        from abil in abilities
                        orderby abil.AbilityTalentId.AbilityType ascending
                        select new JObject(AbilityInfoElement(abil)))));
            }
        }

        protected override JObject AbilityInfoElement(Ability ability)
        {
            JObject jObject = AbilityTalentInfoElement(ability);

            return jObject;
        }

        protected override JProperty GetUnitPortraitObject(Unit unit)
        {
            JObject portrait = new JObject();

            if (!string.IsNullOrEmpty(unit.UnitPortrait.TargetInfoPanelFileName))
                portrait.Add("targetInfo", Path.ChangeExtension(unit.UnitPortrait.TargetInfoPanelFileName?.ToLower(), StaticImageExtension));
            if (!string.IsNullOrEmpty(unit.UnitPortrait.MiniMapIconFileName))
                portrait.Add("minimap", Path.ChangeExtension(unit.UnitPortrait.MiniMapIconFileName?.ToLower(), StaticImageExtension));

            return new JProperty("portraits", portrait);
        }
    }
}
