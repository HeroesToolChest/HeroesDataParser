using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.HeroData
{
    internal class HeroDataXmlWriter : HeroDataWriter<XElement, XElement, Hero>
    {
        public HeroDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Heroes";
        }

        protected override XElement MainElement(Hero hero)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(hero);

            return new XElement(
                hero.Id,
                string.IsNullOrEmpty(hero.Name) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("name", hero.Name),
                string.IsNullOrEmpty(hero.CUnitId) ? null : new XAttribute("unitId", hero.CUnitId),
                string.IsNullOrEmpty(hero.HyperlinkId) ? null : new XAttribute("hyperlinkId", hero.HyperlinkId),
                string.IsNullOrEmpty(hero.AttributeId) ? null : new XAttribute("attributeId", hero.AttributeId),
                string.IsNullOrEmpty(hero.Difficulty) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("difficulty", hero.Difficulty),
                new XAttribute("franchise", hero.Franchise),
                hero.Gender.HasValue ? new XAttribute("gender", hero.Gender.Value) : null,
                string.IsNullOrEmpty(hero.Title) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("title", hero.Title),
                hero.InnerRadius > 0 ? new XAttribute("innerRadius", hero.InnerRadius) : null,
                hero.Radius > 0 ? new XAttribute("radius", hero.Radius) : null,
                hero.ReleaseDate.HasValue ? new XAttribute("releaseDate", hero.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null,
                hero.Sight > 0 ? new XAttribute("sight", hero.Sight) : null,
                hero.Speed > 0 ? new XAttribute("speed", hero.Speed) : null,
                string.IsNullOrEmpty(hero.Type?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("type", GetTooltip(hero.Type, FileOutputOptions.DescriptionType)),
                new XAttribute("rarity", hero.Rarity),
                string.IsNullOrEmpty(hero.SearchText) || FileOutputOptions.IsLocalizedText ? null : new XElement("SearchText", hero.SearchText),
                string.IsNullOrEmpty(hero.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("Description", GetTooltip(hero.Description, FileOutputOptions.DescriptionType)),
                string.IsNullOrEmpty(hero.InfoText?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("InfoText", GetTooltip(hero.InfoText, FileOutputOptions.DescriptionType)),
                hero.HeroDescriptors.Count > 0 ? new XElement("Descriptors", hero.HeroDescriptors.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).Select(d => new XElement("Descriptor", d))) : null,
                hero.UnitIds.Count > 0 ? new XElement("Units", hero.UnitIds.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).Select(x => new XElement("Unit", x))) : null,
                HeroPortraits(hero),
                UnitLife(hero),
                UnitShield(hero),
                UnitEnergy(hero),
                UnitArmor(hero),
                hero.Roles.Count > 0 && !FileOutputOptions.IsLocalizedText ? new XElement("Roles", hero.Roles.Select(r => new XElement("Role", r))) : null,
                string.IsNullOrEmpty(hero.ExpandedRole) || FileOutputOptions.IsLocalizedText ? null : new XElement("ExpandedRole", hero.ExpandedRole),
                HeroRatings(hero),
                UnitWeapons(hero),
                UnitAbilities(hero),
                UnitSubAbilities(hero),
                HeroTalents(hero),
                Units(hero));
        }

        protected override XElement UnitElement(Unit unit)
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
                unit.HeroDescriptors.Count > 0 ? new XElement("Descriptors", unit.HeroDescriptors.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).Select(d => new XElement("Descriptor", d))) : null,
                unit.Attributes.Count > 0 ? new XElement("Attributes", unit.Attributes.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).Select(x => new XElement("Attribute", x))) : null,
                unit.UnitIds.Count > 0 ? new XElement("Units", unit.UnitIds.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).Select(x => new XElement("Unit", x))) : null,
                UnitPortraits(unit),
                UnitLife(unit),
                UnitEnergy(unit),
                UnitArmor(unit),
                UnitWeapons(unit),
                UnitAbilities(unit),
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
                new XElement("Amount", unit.Life.LifeMax, new XAttribute("scale", unit.Life.LifeScaling), !FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(unit.Life.LifeType) ? new XAttribute("type", unit.Life.LifeType) : null),
                new XElement("RegenRate", unit.Life.LifeRegenerationRate, new XAttribute("scale", unit.Life.LifeRegenerationRateScaling)));
        }

        protected override XElement GetEnergyObject(Unit unit)
        {
            return new XElement(
                "Energy",
                new XElement("Amount", unit.Energy.EnergyMax, !FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(unit.Energy.EnergyType) ? new XAttribute("type", unit.Energy.EnergyType) : null),
                new XElement("RegenRate", unit.Energy.EnergyRegenerationRate));
        }

        protected override XElement GetShieldObject(Unit unit)
        {
            return new XElement(
                "Shield",
                new XElement("Amount", unit.Shield.ShieldMax, new XAttribute("scale", unit.Shield.ShieldScaling), !FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(unit.Shield.ShieldType) ? new XAttribute("type", unit.Shield.ShieldType) : null, new XAttribute("regenDelay", unit.Shield.ShieldRegenerationDelay)),
                new XElement("RegenRate", unit.Shield.ShieldRegenerationRate, new XAttribute("scale", unit.Shield.ShieldRegenerationRateScaling)));
        }

        protected override XElement GetRatingsObject(Hero hero)
        {
            return new XElement(
                "Ratings",
                new XAttribute("complexity", hero.Ratings.Complexity),
                new XAttribute("damage", hero.Ratings.Damage),
                new XAttribute("survivability", hero.Ratings.Survivability),
                new XAttribute("utility", hero.Ratings.Utility));
        }

        protected override XElement GetAbilitiesObject(Unit unit)
        {
            return new XElement(
                "Abilities",
                unit.PrimaryAbilities(AbilityTiers.Basic).Any() ? new XElement("Basic", unit.PrimaryAbilities(AbilityTiers.Basic).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.Heroic).Any() ? new XElement("Heroic", unit.PrimaryAbilities(AbilityTiers.Heroic).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.Trait).Any() ? new XElement("Trait", unit.PrimaryAbilities(AbilityTiers.Trait).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.Mount).Any() ? new XElement("Mount", unit.PrimaryAbilities(AbilityTiers.Mount).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.Activable).Any() ? new XElement("Activable", unit.PrimaryAbilities(AbilityTiers.Activable).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.Hearth).Any() ? new XElement("Hearth", unit.PrimaryAbilities(AbilityTiers.Hearth).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.Taunt).Any() ? new XElement("Taunt", unit.PrimaryAbilities(AbilityTiers.Taunt).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.Dance).Any() ? new XElement("Dance", unit.PrimaryAbilities(AbilityTiers.Dance).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.Spray).Any() ? new XElement("Spray", unit.PrimaryAbilities(AbilityTiers.Spray).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.Voice).Any() ? new XElement("Voice", unit.PrimaryAbilities(AbilityTiers.Voice).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.MapMechanic).Any() ? new XElement("MapMechanic", unit.PrimaryAbilities(AbilityTiers.MapMechanic).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.Interact).Any() ? new XElement("Interact", unit.PrimaryAbilities(AbilityTiers.Interact).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.Action).Any() ? new XElement("Action", unit.PrimaryAbilities(AbilityTiers.Action).Select(x => AbilityInfoElement(x))) : null,
                unit.PrimaryAbilities(AbilityTiers.Unknown).Any() ? new XElement("Unknown", unit.PrimaryAbilities(AbilityTiers.Unknown).Select(x => AbilityInfoElement(x))) : null);
        }

        protected override XElement? GetSubAbilitiesObject(ILookup<AbilityTalentId, Ability> linkedAbilities)
        {
            XElement parentLinkElement = new XElement("SubAbilities");

            IEnumerable<AbilityTalentId> parentLinks = linkedAbilities.Select(x => x.Key);
            foreach (AbilityTalentId parent in parentLinks)
            {
                XElement? abilities = null;

                if (parent.AbilityType != AbilityTypes.Unknown)
                {
                    if (parent.IsPassive)
                        abilities = new XElement(XmlConvert.EncodeName(parent.Id));
                    else
                        abilities = new XElement(XmlConvert.EncodeName($"{parent.ReferenceId}|{parent.ButtonId}|{parent.AbilityType}"));
                }
                else
                {
                    abilities = new XElement(XmlConvert.EncodeName($"{parent.ReferenceId}|{parent.ButtonId}"));
                }

                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Basic).Any() ? new XElement("Basic", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Basic).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Heroic).Any() ? new XElement("Heroic", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Heroic).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Trait).Any() ? new XElement("Trait", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Trait).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Mount).Any() ? new XElement("Mount", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Mount).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Activable).Any() ? new XElement("Activable", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Activable).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Hearth).Any() ? new XElement("Hearth", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Hearth).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Taunt).Any() ? new XElement("Taunt", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Taunt).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Dance).Any() ? new XElement("Dance", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Dance).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Spray).Any() ? new XElement("Spray", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Spray).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Voice).Any() ? new XElement("Voice", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Voice).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.MapMechanic).Any() ? new XElement("MapMechanic", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.MapMechanic).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Interact).Any() ? new XElement("Interact", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Interact).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Action).Any() ? new XElement("Action", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Action).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);
                abilities.Add(linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Unknown).Any() ? new XElement("Unknown", linkedAbilities[parent].Where(x => x.Tier == AbilityTiers.Unknown).OrderBy(x => x.AbilityTalentId.AbilityType).Select(x => AbilityInfoElement(x))) : null);

                if (abilities.HasElements)
                    parentLinkElement.Add(abilities);
            }

            if (parentLinkElement.HasElements)
                return parentLinkElement;
            else
                return null;
        }

        protected override XElement GetUnitsObject(Hero hero)
        {
            return new XElement(
                "HeroUnits",
                hero.HeroUnits.Select(heroUnit => new XElement(heroUnit.CUnitId, UnitElement(heroUnit))));
        }

        protected override XElement GetTalentsObject(Hero hero)
        {
            return new XElement(
                "Talents",
                new XElement("Level1", hero.TierTalents(TalentTiers.Level1).Select(level1 => TalentInfoElement(level1))),
                new XElement("Level4", hero.TierTalents(TalentTiers.Level4).Select(level4 => TalentInfoElement(level4))),
                new XElement("Level7", hero.TierTalents(TalentTiers.Level7).Select(level7 => TalentInfoElement(level7))),
                new XElement("Level10", hero.TierTalents(TalentTiers.Level10).Select(level10 => TalentInfoElement(level10))),
                new XElement("Level13", hero.TierTalents(TalentTiers.Level13).Select(level13 => TalentInfoElement(level13))),
                new XElement("Level16", hero.TierTalents(TalentTiers.Level16).Select(level16 => TalentInfoElement(level16))),
                new XElement("Level20", hero.TierTalents(TalentTiers.Level20).Select(level20 => TalentInfoElement(level20))));
        }

        protected override XElement AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(abilityTalentBase);

            return new XElement(
                XmlConvert.EncodeName(abilityTalentBase.AbilityTalentId.ReferenceId),
                string.IsNullOrEmpty(abilityTalentBase.AbilityTalentId.ButtonId) ? null : new XAttribute("buttonId", abilityTalentBase.AbilityTalentId.ButtonId),
                string.IsNullOrEmpty(abilityTalentBase.Name) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("name", abilityTalentBase.Name),
                new XAttribute("abilityType", abilityTalentBase.AbilityTalentId.AbilityType.ToString()),
                abilityTalentBase.IsActive && abilityTalentBase is Talent ? new XAttribute("isActive", abilityTalentBase.IsActive) : !abilityTalentBase.IsActive && abilityTalentBase is Ability ? new XAttribute("isActive", abilityTalentBase.IsActive) : null,
                abilityTalentBase.AbilityTalentId.IsPassive ? new XAttribute("isPassive", abilityTalentBase.AbilityTalentId.IsPassive) : null,
                abilityTalentBase.IsQuest ? new XAttribute("isQuest", abilityTalentBase.IsQuest) : null,
                string.IsNullOrEmpty(abilityTalentBase.IconFileName) ? null : new XElement("Icon", Path.ChangeExtension(abilityTalentBase.IconFileName?.ToLowerInvariant(), StaticImageExtension)),
                abilityTalentBase.Tooltip.Cooldown.ToggleCooldown.HasValue ? new XElement("ToggleCooldown", abilityTalentBase.Tooltip.Cooldown.ToggleCooldown.Value) : null,
                UnitAbilityTalentLifeCost(abilityTalentBase.Tooltip.Life),
                UnitAbilityTalentEnergyCost(abilityTalentBase.Tooltip.Energy),
                UnitAbilityTalentCharges(abilityTalentBase.Tooltip.Charges),
                UnitAbilityTalentCooldown(abilityTalentBase.Tooltip.Cooldown),
                string.IsNullOrEmpty(abilityTalentBase.Tooltip.ShortTooltip?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("ShortTooltip", GetTooltip(abilityTalentBase.Tooltip.ShortTooltip, FileOutputOptions.DescriptionType)),
                string.IsNullOrEmpty(abilityTalentBase.Tooltip.FullTooltip?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("FullTooltip", GetTooltip(abilityTalentBase.Tooltip.FullTooltip, FileOutputOptions.DescriptionType)));
        }

        protected override XElement AbilityInfoElement(Ability ability)
        {
            XElement element = AbilityTalentInfoElement(ability);

            return element;
        }

        protected override XElement TalentInfoElement(Talent talent)
        {
            XElement element = AbilityTalentInfoElement(talent);
            element.Add(new XAttribute("sort", talent.Column));

            if (talent.AbilityTalentLinkIds.Count > 0)
                element.Add(new XElement("AbilityTalentLinkIds", talent.AbilityTalentLinkIds.Select(link => new XElement("AbilityTalentLinkId", link))));
            if (talent.PrerequisiteTalentIds.Count > 0)
                element.Add(new XElement("PrerequisiteTalentIds", talent.PrerequisiteTalentIds.Select(id => new XElement("PrerequisiteTalentId", id))));
            return element;
        }

        protected override XElement GetAbilityTalentLifeCostObject(TooltipLife tooltipLife)
        {
            return new XElement(
                "LifeTooltip",
                GetTooltip(tooltipLife.LifeCostTooltip!, FileOutputOptions.DescriptionType));
        }

        protected override XElement GetAbilityTalentEnergyCostObject(TooltipEnergy tooltipEnergy)
        {
            return new XElement(
                "EnergyTooltip",
                GetTooltip(tooltipEnergy.EnergyTooltip!, FileOutputOptions.DescriptionType));
        }

        protected override XElement GetAbilityTalentCooldownObject(TooltipCooldown tooltipCooldown)
        {
            return new XElement(
                "CooldownTooltip",
                GetTooltip(tooltipCooldown.CooldownTooltip!, FileOutputOptions.DescriptionType));
        }

        protected override XElement GetAbilityTalentChargesObject(TooltipCharges tooltipCharges)
        {
            return new XElement(
                "Charges",
                tooltipCharges.CountMax,
                tooltipCharges.CountUse.HasValue == true ? new XAttribute("consume", tooltipCharges.CountUse!.Value) : null,
                tooltipCharges.CountStart.HasValue == true ? new XAttribute("initial", tooltipCharges.CountStart!.Value) : null,
                tooltipCharges.IsHideCount.HasValue == true ? new XAttribute("isHidden", tooltipCharges.IsHideCount!.Value) : null,
                tooltipCharges.RecastCooldown.HasValue == true ? new XAttribute("recastCooldown", tooltipCharges.RecastCooldown!.Value) : null);
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

        protected override XElement GetPortraitObject(Hero hero)
        {
            return new XElement(
                "Portraits",
                new XElement("HeroSelect", Path.ChangeExtension(hero.HeroPortrait.HeroSelectPortraitFileName?.ToLowerInvariant(), StaticImageExtension)),
                new XElement("Leaderboard", Path.ChangeExtension(hero.HeroPortrait.LeaderboardPortraitFileName?.ToLowerInvariant(), StaticImageExtension)),
                new XElement("Loading", Path.ChangeExtension(hero.HeroPortrait.LoadingScreenPortraitFileName?.ToLowerInvariant(), StaticImageExtension)),
                new XElement("PartyFrame", Path.ChangeExtension(hero.HeroPortrait.PartyPanelPortraitFileName?.ToLowerInvariant(), StaticImageExtension)),
                new XElement("Target", Path.ChangeExtension(hero.HeroPortrait.TargetPortraitFileName?.ToLowerInvariant(), StaticImageExtension)),
                new XElement("DraftScreen", Path.ChangeExtension(hero.HeroPortrait.DraftScreenFileName?.ToLowerInvariant(), StaticImageExtension)),
                new XElement("PartyFrames", hero.HeroPortrait.PartyFrameFileName.Select(x => new XElement("PartyFrame", Path.ChangeExtension(x.ToLowerInvariant(), StaticImageExtension)))),
                new XElement("Minimap", Path.ChangeExtension(hero.UnitPortrait.MiniMapIconFileName?.ToLowerInvariant(), StaticImageExtension)),
                new XElement("TargetInfo", Path.ChangeExtension(hero.UnitPortrait.TargetInfoPanelFileName?.ToLowerInvariant(), StaticImageExtension)));
        }

        protected override XElement GetUnitPortraitObject(Unit unit)
        {
            return new XElement(
                "Portraits",
                string.IsNullOrEmpty(unit.UnitPortrait.TargetInfoPanelFileName) ? null : new XElement("TargetInfo", Path.ChangeExtension(unit.UnitPortrait.TargetInfoPanelFileName?.ToLowerInvariant(), StaticImageExtension)),
                string.IsNullOrEmpty(unit.UnitPortrait.MiniMapIconFileName) ? null : new XElement("Minimap", Path.ChangeExtension(unit.UnitPortrait.MiniMapIconFileName?.ToLowerInvariant(), StaticImageExtension)));
        }
    }
}
