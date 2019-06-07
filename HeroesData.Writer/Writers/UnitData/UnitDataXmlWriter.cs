using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.UnitData
{
    internal class UnitDataXmlWriter : UnitDataWriter<XElement, XElement>
    {
        public UnitDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Units";
        }

        protected override XElement MainElement(Unit unit)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(unit);

            return new XElement(
                unit.Id,
                string.IsNullOrEmpty(unit.Name) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("name", unit.Name),
                string.IsNullOrEmpty(unit.HyperlinkId) ? null : new XAttribute("hyperlinkId", unit.HyperlinkId),
                unit.InnerRadius > 0 ? new XAttribute("innerRadius", unit.InnerRadius) : null,
                unit.Radius > 0 ? new XAttribute("radius", unit.Radius) : null,
                unit.Sight > 0 ? new XAttribute("sight", unit.Sight) : null,
                unit.Speed > 0 ? new XAttribute("speed", unit.Speed) : null,
                unit.KillXP > 0 ? new XAttribute("killXP", unit.KillXP) : null,
                string.IsNullOrEmpty(unit.DamageType) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("damageType", unit.DamageType),
                string.IsNullOrEmpty(unit.ScalingBehaviorLink) ? null : new XElement("ScalingLinkId", unit.ScalingBehaviorLink),
                string.IsNullOrEmpty(unit.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("Description", GetTooltip(unit.Description, FileOutputOptions.DescriptionType)),
                unit.HeroDescriptorsCount > 0 ? new XElement("Descriptors", unit.HeroDescriptors.OrderBy(x => x).Select(d => new XElement("Descriptor", d))) : null,
                unit.AttributesCount > 0 ? new XElement("Attributes", unit.Attributes.OrderBy(x => x).Select(x => new XElement("Attribute", x))) : null,
                unit.UnitIdsCount > 0 ? new XElement("Units", unit.UnitIds.OrderBy(x => x).Select(x => new XElement("Unit", x))) : null,
                string.IsNullOrEmpty(unit.TargetInfoPanelImageFileName) ? null : new XElement("Image", Path.ChangeExtension(unit.TargetInfoPanelImageFileName?.ToLower(), StaticImageExtension)),
                UnitLife(unit),
                UnitEnergy(unit),
                UnitArmor(unit),
                UnitWeapons(unit),
                UnitAbilities(unit, false),
                UnitSubAbilities(unit));
        }

        protected override XElement GetArmorObject(Unit unit)
        {
            return new XElement(
                "Armor",
                unit.Armor.Select(armor => new XElement(
                    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(armor.Type),
                    new XAttribute("basic", armor.BasicArmor),
                    new XAttribute("ability", armor.AbilityArmor),
                    new XAttribute("splash", armor.SplashArmor))));
        }

        protected override XElement GetLifeObject(Unit unit)
        {
            return new XElement(
                "Life",
                new XElement("Amount", unit.Life.LifeMax, new XAttribute("scale", unit.Life.LifeScaling)),
                new XElement("RegenRate", unit.Life.LifeRegenerationRate, new XAttribute("scale", unit.Life.LifeRegenerationRateScaling)));
        }

        protected override XElement GetEnergyObject(Unit unit)
        {
            return new XElement(
                "Energy",
                new XElement("Amount", unit.Energy.EnergyMax, !string.IsNullOrEmpty(unit.Energy.EnergyType) ? new XAttribute("type", unit.Energy.EnergyType) : null),
                new XElement("RegenRate", unit.Energy.EnergyRegenerationRate));
        }

        protected override XElement GetAbilitiesObject(Unit unit, bool isSubAbilities)
        {
            if (isSubAbilities)
            {
                return new XElement(
                    "Abilities",
                    unit.SubAbilities(AbilityTier.Basic).Any() ? new XElement("Basic", unit.SubAbilities(AbilityTier.Basic).OrderBy(x => x.AbilityType).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Heroic).Any() ? new XElement("Heroic", unit.SubAbilities(AbilityTier.Heroic).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Trait).Any() ? new XElement("Trait", unit.SubAbilities(AbilityTier.Trait).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Mount).Any() ? new XElement("Mount", unit.SubAbilities(AbilityTier.Mount).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Activable).Any() ? new XElement("Activable", unit.SubAbilities(AbilityTier.Activable).OrderBy(x => x.AbilityType).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Hearth).Any() ? new XElement("Hearth", unit.SubAbilities(AbilityTier.Hearth).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Taunt).Any() ? new XElement("Taunt", unit.SubAbilities(AbilityTier.Taunt).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Dance).Any() ? new XElement("Dance", unit.SubAbilities(AbilityTier.Dance).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Spray).Any() ? new XElement("Spray", unit.SubAbilities(AbilityTier.Spray).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Voice).Any() ? new XElement("Voice", unit.SubAbilities(AbilityTier.Voice).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.MapMechanic).Any() ? new XElement("MapMechanic", unit.SubAbilities(AbilityTier.MapMechanic).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Interact).Any() ? new XElement("Interact", unit.SubAbilities(AbilityTier.Interact).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Action).Any() ? new XElement("Action", unit.SubAbilities(AbilityTier.Action).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Hidden).Any() ? new XElement("Hidden", unit.SubAbilities(AbilityTier.Hidden).Select(x => AbilityInfoElement(x))) : null,
                    unit.SubAbilities(AbilityTier.Unknown).Any() ? new XElement("Unknown", unit.SubAbilities(AbilityTier.Unknown).Select(x => AbilityInfoElement(x))) : null);
            }
            else
            {
                return new XElement(
                    "Abilities",
                    unit.PrimaryAbilities(AbilityTier.Basic).Any() ? new XElement("Basic", unit.PrimaryAbilities(AbilityTier.Basic).OrderBy(x => x.AbilityType).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Heroic).Any() ? new XElement("Heroic", unit.PrimaryAbilities(AbilityTier.Heroic).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Trait).Any() ? new XElement("Trait", unit.PrimaryAbilities(AbilityTier.Trait).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Mount).Any() ? new XElement("Mount", unit.PrimaryAbilities(AbilityTier.Mount).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Activable).Any() ? new XElement("Activable", unit.PrimaryAbilities(AbilityTier.Activable).OrderBy(x => x.AbilityType).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Hearth).Any() ? new XElement("Hearth", unit.PrimaryAbilities(AbilityTier.Hearth).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Taunt).Any() ? new XElement("Taunt", unit.SubAbilities(AbilityTier.Taunt).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Dance).Any() ? new XElement("Dance", unit.SubAbilities(AbilityTier.Dance).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Spray).Any() ? new XElement("Spray", unit.SubAbilities(AbilityTier.Spray).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Voice).Any() ? new XElement("Voice", unit.SubAbilities(AbilityTier.Voice).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.MapMechanic).Any() ? new XElement("MapMechanic", unit.SubAbilities(AbilityTier.MapMechanic).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Interact).Any() ? new XElement("Interact", unit.SubAbilities(AbilityTier.Interact).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Action).Any() ? new XElement("Action", unit.SubAbilities(AbilityTier.Action).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Hidden).Any() ? new XElement("Hidden", unit.SubAbilities(AbilityTier.Hidden).Select(x => AbilityInfoElement(x))) : null,
                    unit.PrimaryAbilities(AbilityTier.Unknown).Any() ? new XElement("Unknown", unit.SubAbilities(AbilityTier.Unknown).Select(x => AbilityInfoElement(x))) : null);
            }
        }

        protected override XElement GetSubAbilitiesObject(ILookup<string, Ability> linkedAbilities)
        {
            return new XElement(
                "SubAbilities",
                linkedAbilities.Select(parent => new XElement(
                    parent.Key,
                    parent.Where(x => x.Tier == AbilityTier.Basic).Any() ? new XElement("Basic", parent.Where(x => x.Tier == AbilityTier.Basic).OrderBy(x => x.AbilityType).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Heroic).Any() ? new XElement("Heroic", parent.Where(x => x.Tier == AbilityTier.Heroic).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Trait).Any() ? new XElement("Trait", parent.Where(x => x.Tier == AbilityTier.Trait).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Mount).Any() ? new XElement("Mount", parent.Where(x => x.Tier == AbilityTier.Mount).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Activable).Any() ? new XElement("Activable", parent.Where(x => x.Tier == AbilityTier.Activable).OrderBy(x => x.AbilityType).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Hearth).Any() ? new XElement("Hearth", parent.Where(x => x.Tier == AbilityTier.Hearth).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Taunt).Any() ? new XElement("Taunt", parent.Where(x => x.Tier == AbilityTier.Taunt).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Dance).Any() ? new XElement("Dance", parent.Where(x => x.Tier == AbilityTier.Dance).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Spray).Any() ? new XElement("Spray", parent.Where(x => x.Tier == AbilityTier.Spray).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Voice).Any() ? new XElement("Voice", parent.Where(x => x.Tier == AbilityTier.Voice).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.MapMechanic).Any() ? new XElement("MapMechanic", parent.Where(x => x.Tier == AbilityTier.MapMechanic).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Interact).Any() ? new XElement("Interact", parent.Where(x => x.Tier == AbilityTier.Interact).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Hidden).Any() ? new XElement("Hidden", parent.Where(x => x.Tier == AbilityTier.Hidden).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Action).Any() ? new XElement("Action", parent.Where(x => x.Tier == AbilityTier.Action).Select(x => AbilityInfoElement(x))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Unknown).Any() ? new XElement("Unknown", parent.Where(x => x.Tier == AbilityTier.Unknown).Select(x => AbilityInfoElement(x))) : null)));
        }
        protected override XElement AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(abilityTalentBase);

            return new XElement(
                XmlConvert.EncodeName(abilityTalentBase.ReferenceNameId),
                string.IsNullOrEmpty(abilityTalentBase.Name) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("name", abilityTalentBase.Name),
                string.IsNullOrEmpty(abilityTalentBase.ShortTooltipNameId) ? null : new XAttribute("shortTooltipId", abilityTalentBase.ShortTooltipNameId),
                string.IsNullOrEmpty(abilityTalentBase.FullTooltipNameId) ? null : new XAttribute("fullTooltipId", abilityTalentBase.FullTooltipNameId),
                new XAttribute("abilityType", abilityTalentBase.AbilityType.ToString()),
                abilityTalentBase.IsActive ? new XAttribute("isActive", abilityTalentBase.IsActive) : null,
                abilityTalentBase.IsPassive ? new XAttribute("isPassive", abilityTalentBase.IsPassive) : null,
                abilityTalentBase.IsQuest ? new XAttribute("isQuest", abilityTalentBase.IsQuest) : null,
                string.IsNullOrEmpty(abilityTalentBase.IconFileName) ? null : new XElement("Icon", Path.ChangeExtension(abilityTalentBase.IconFileName?.ToLower(), StaticImageExtension)),
                abilityTalentBase.Tooltip.Cooldown.ToggleCooldown.HasValue ? new XElement("ToggleCooldown", abilityTalentBase.Tooltip.Cooldown.ToggleCooldown.Value) : null,
                UnitAbilityTalentLifeCost(abilityTalentBase.Tooltip.Life),
                UnitAbilityTalentEnergyCost(abilityTalentBase.Tooltip.Energy),
                UnitAbilityTalentCharges(abilityTalentBase.Tooltip.Charges),
                UnitAbilityTalentCooldown(abilityTalentBase.Tooltip.Cooldown),
                string.IsNullOrEmpty(abilityTalentBase.Tooltip.ShortTooltip?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("ShortTooltip", GetTooltip(abilityTalentBase.Tooltip.ShortTooltip, FileOutputOptions.DescriptionType)),
                string.IsNullOrEmpty(abilityTalentBase.Tooltip.FullTooltip?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("FullTooltip", GetTooltip(abilityTalentBase.Tooltip.FullTooltip, FileOutputOptions.DescriptionType)));
        }

        protected override XElement GetAbilityTalentLifeCostObject(TooltipLife tooltipLife)
        {
            return new XElement(
                "LifeTooltip",
                GetTooltip(tooltipLife.LifeCostTooltip, FileOutputOptions.DescriptionType));
        }

        protected override XElement GetAbilityTalentEnergyCostObject(TooltipEnergy tooltipEnergy)
        {
            return new XElement(
                "EnergyTooltip",
                GetTooltip(tooltipEnergy.EnergyTooltip, FileOutputOptions.DescriptionType));
        }

        protected override XElement GetAbilityTalentCooldownObject(TooltipCooldown tooltipCooldown)
        {
            return new XElement(
                "CooldownTooltip",
                GetTooltip(tooltipCooldown.CooldownTooltip, FileOutputOptions.DescriptionType));
        }

        protected override XElement GetAbilityTalentChargesObject(TooltipCharges tooltipCharges)
        {
            return new XElement(
                "Charges",
                tooltipCharges.CountMax,
                tooltipCharges.CountUse.HasValue == true ? new XAttribute("consume", tooltipCharges.CountUse.Value) : null,
                tooltipCharges.CountStart.HasValue == true ? new XAttribute("initial", tooltipCharges.CountStart.Value) : null,
                tooltipCharges.IsHideCount.HasValue == true ? new XAttribute("isHidden", tooltipCharges.IsHideCount.Value) : null,
                tooltipCharges.RecastCooldown.HasValue == true ? new XAttribute("recastCooldown", tooltipCharges.RecastCooldown.Value) : null);
        }

        protected override XElement GetWeaponsObject(Unit unit)
        {
            return new XElement(
                "Weapons",
                unit.Weapons.Select(w => new XElement(
                    w.WeaponNameId,
                    new XAttribute("range", w.Range),
                    new XAttribute("period", w.Period),
                    new XElement("Damage", w.Damage, new XAttribute("scale", w.DamageScaling)),
                    w.AttributeFactors.Any() ? new XElement("DamageFactor", w.AttributeFactors.Select(x => new XElement(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.Type), x.Value))) : null)));
        }

        protected override XElement AbilityInfoElement(Ability ability)
        {
            XElement element = AbilityTalentInfoElement(ability);

            return element;
        }
    }
}
