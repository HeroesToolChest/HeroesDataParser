using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
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
                string.IsNullOrEmpty(unit.DamageType) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("damageType", unit.DamageType),
                string.IsNullOrEmpty(unit.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("Description", GetTooltip(unit.Description, FileOutputOptions.DescriptionType)),
                unit.HeroDescriptors.Count > 0 ? new XElement("Descriptors", unit.HeroDescriptors.Select(d => new XElement("Descriptor", d))) : null,
                unit.Attributes.Count > 0 ? new XElement("Attributes", unit.Attributes.Select(x => new XElement("Attribute", x))) : null,
                unit.TargetInfoPanelImageFileNames.Count > 0 ? new XElement("Images", unit.TargetInfoPanelImageFileNames.Select(x => new XElement("Image", Path.ChangeExtension(x, StaticImageExtension)))) : null,
                UnitLife(unit),
                UnitEnergy(unit),
                UnitArmor(unit),
                UnitWeapons(unit),
                UnitAbilities(unit, false));
        }

        protected override XElement GetArmorObject(Unit unit)
        {
            return new XElement(
                "Armor",
                new XElement("Physical", unit.Armor.PhysicalArmor),
                new XElement("Spell", unit.Armor.SpellArmor));
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
                new XElement("Amount", unit.Energy.EnergyMax, new XAttribute("type", unit.Energy.EnergyType)),
                new XElement("RegenRate", unit.Energy.EnergyRegenerationRate));
        }

        protected override XElement GetAbilitiesObject(Unit unit, bool isSubAbilities)
        {
            if (isSubAbilities)
            {
                return new XElement(
                    "Abilities",
                    unit.SubAbilities(AbilityTier.Basic)?.Count > 0 ? new XElement("Basic", unit.SubAbilities(AbilityTier.Basic).Select(basic => AbilityTalentInfoElement(basic))) : null,
                    unit.SubAbilities(AbilityTier.Heroic)?.Count > 0 ? new XElement("Heroic", unit.SubAbilities(AbilityTier.Heroic).Select(heroic => AbilityTalentInfoElement(heroic))) : null,
                    unit.SubAbilities(AbilityTier.Trait)?.Count > 0 ? new XElement("Trait", unit.SubAbilities(AbilityTier.Trait).Select(trait => AbilityTalentInfoElement(trait))) : null,
                    unit.SubAbilities(AbilityTier.Mount)?.Count > 0 ? new XElement("Mount", unit.SubAbilities(AbilityTier.Mount).Select(mount => AbilityTalentInfoElement(mount))) : null,
                    unit.SubAbilities(AbilityTier.Activable)?.Count > 0 ? new XElement("Activable", unit.SubAbilities(AbilityTier.Activable).Select(activable => AbilityTalentInfoElement(activable))) : null,
                    unit.SubAbilities(AbilityTier.Hearth)?.Count > 0 ? new XElement("Hearth", unit.SubAbilities(AbilityTier.Hearth).Select(hearth => AbilityTalentInfoElement(hearth))) : null);
            }
            else
            {
                return new XElement(
                    "Abilities",
                    unit.PrimaryAbilities(AbilityTier.Basic)?.Count > 0 ? new XElement("Basic", unit.PrimaryAbilities(AbilityTier.Basic).Select(basic => AbilityTalentInfoElement(basic))) : null,
                    unit.PrimaryAbilities(AbilityTier.Heroic)?.Count > 0 ? new XElement("Heroic", unit.PrimaryAbilities(AbilityTier.Heroic).Select(heroic => AbilityTalentInfoElement(heroic))) : null,
                    unit.PrimaryAbilities(AbilityTier.Trait)?.Count > 0 ? new XElement("Trait", unit.PrimaryAbilities(AbilityTier.Trait).Select(trait => AbilityTalentInfoElement(trait))) : null,
                    unit.PrimaryAbilities(AbilityTier.Mount)?.Count > 0 ? new XElement("Mount", unit.PrimaryAbilities(AbilityTier.Mount).Select(mount => AbilityTalentInfoElement(mount))) : null,
                    unit.PrimaryAbilities(AbilityTier.Activable)?.Count > 0 ? new XElement("Activable", unit.PrimaryAbilities(AbilityTier.Activable).Select(activable => AbilityTalentInfoElement(activable))) : null,
                    unit.PrimaryAbilities(AbilityTier.Hearth)?.Count > 0 ? new XElement("Hearth", unit.PrimaryAbilities(AbilityTier.Hearth).Select(hearth => AbilityTalentInfoElement(hearth))) : null);
            }
        }

        protected override XElement AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(abilityTalentBase);

            return new XElement(
                XmlConvert.EncodeName(abilityTalentBase.ReferenceNameId),
                string.IsNullOrEmpty(abilityTalentBase.Name) ? null : new XAttribute("name", abilityTalentBase.Name),
                string.IsNullOrEmpty(abilityTalentBase.ShortTooltipNameId) ? null : new XAttribute("shortTooltipId", abilityTalentBase.ShortTooltipNameId),
                string.IsNullOrEmpty(abilityTalentBase.FullTooltipNameId) ? null : new XAttribute("fullTooltipId", abilityTalentBase.FullTooltipNameId),
                new XAttribute("abilityType", abilityTalentBase.AbilityType.ToString()),
                abilityTalentBase.IsActive ? new XAttribute("isActive", abilityTalentBase.IsActive) : null,
                abilityTalentBase.IsQuest ? new XAttribute("isQuest", abilityTalentBase.IsQuest) : null,
                new XElement("Icon", Path.ChangeExtension(abilityTalentBase.IconFileName, StaticImageExtension)),
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
                    new XElement("Damage", w.Damage, new XAttribute("scale", w.DamageScaling)))));
        }
    }
}
