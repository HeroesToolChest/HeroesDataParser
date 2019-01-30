using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using HeroesData.Parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.HeroData
{
    internal class HeroDataXmlWriter : HeroDataWriter<XElement, XElement>
    {
        public HeroDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Heroes";
        }

        protected override XElement MainElement(Hero hero)
        {
            if (IsLocalizedText)
                AddLocalizedGameString(hero);

            return new XElement(
                hero.ShortName,
                string.IsNullOrEmpty(hero.Name) || IsLocalizedText ? null : new XAttribute("name", hero.Name),
                string.IsNullOrEmpty(hero.CHeroId) || hero.CHeroId == StormHero.CHeroId ? null : new XAttribute("cHeroId", hero.CHeroId),
                string.IsNullOrEmpty(hero.CUnitId) || hero.CHeroId == StormHero.CHeroId ? null : new XAttribute("cUnitId", hero.CUnitId),
                string.IsNullOrEmpty(hero.AttributeId) ? null : new XAttribute("attributeId", hero.AttributeId),
                string.IsNullOrEmpty(hero.Difficulty) || IsLocalizedText ? null : new XAttribute("difficulty", hero.Difficulty),
                hero.CHeroId != StormHero.CHeroId ? new XAttribute("franchise", hero.Franchise) : null,
                hero.Gender.HasValue ? new XAttribute("gender", hero.Gender.Value) : null,
                string.IsNullOrEmpty(hero.Title) || IsLocalizedText ? null : new XAttribute("title", hero.Title),
                hero.InnerRadius > 0 ? new XAttribute("innerRadius", hero.InnerRadius) : null,
                hero.Radius > 0 ? new XAttribute("radius", hero.Radius) : null,
                hero.ReleaseDate.HasValue ? new XAttribute("releaseDate", hero.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null,
                hero.Sight > 0 ? new XAttribute("sight", hero.Sight) : null,
                hero.Speed > 0 ? new XAttribute("speed", hero.Speed) : null,
                string.IsNullOrEmpty(hero.Type) || IsLocalizedText ? null : new XAttribute("type", hero.Type),
                hero.Rarity.HasValue ? new XAttribute("rarity", hero.Rarity.Value) : null,
                string.IsNullOrEmpty(hero.MountLinkId) ? null : new XElement("MountLinkId", hero.MountLinkId),
                string.IsNullOrEmpty(hero.HearthLinkId) ? null : new XElement("HearthLinkId", hero.HearthLinkId),
                string.IsNullOrEmpty(hero.SearchText) || IsLocalizedText ? null : new XElement("SearchText", hero.SearchText),
                string.IsNullOrEmpty(hero.Description?.RawDescription) || IsLocalizedText ? null : new XElement("Description", GetTooltip(hero.Description, FileSettings.DescriptionType)),
                hero.HeroDescriptors.Count > 0 ? new XElement("Descriptors", hero.HeroDescriptors.Select(d => new XElement("Descriptor", d))) : null,
                HeroPortraits(hero),
                UnitLife(hero),
                UnitEnergy(hero),
                UnitArmor(hero),
                hero.Roles?.Count > 0 && !IsLocalizedText ? new XElement("Roles", hero.Roles.Select(r => new XElement("Role", r))) : null,
                HeroRatings(hero),
                UnitWeapons(hero),
                UnitAbilities(hero, false),
                UnitSubAbilities(hero),
                HeroTalents(hero),
                Units(hero));
        }

        protected override XElement UnitElement(Unit unit)
        {
            if (IsLocalizedText)
                AddLocalizedGameString(unit);

            return new XElement(
                unit.ShortName,
                string.IsNullOrEmpty(unit.Name) || IsLocalizedText ? null : new XAttribute("name", unit.Name),
                string.IsNullOrEmpty(unit.CUnitId) ? null : new XAttribute("cUnitId", unit.CUnitId),
                unit.InnerRadius > 0 ? new XAttribute("innerRadius", unit.InnerRadius) : null,
                unit.Radius > 0 ? new XAttribute("radius", unit.Radius) : null,
                unit.Sight > 0 ? new XAttribute("sight", unit.Sight) : null,
                unit.Speed > 0 ? new XAttribute("speed", unit.Speed) : null,
                string.IsNullOrEmpty(unit.Type) || IsLocalizedText ? null : new XAttribute("type", unit.Type),
                string.IsNullOrEmpty(unit.Description?.RawDescription) || IsLocalizedText ? null : new XElement("Description", GetTooltip(unit.Description, FileSettings.DescriptionType)),
                unit.HeroDescriptors.Count > 0 ? new XElement("Descriptors", unit.HeroDescriptors.Select(d => new XElement("Descriptor", d))) : null,
                UnitLife(unit),
                UnitArmor(unit),
                UnitEnergy(unit),
                UnitWeapons(unit),
                UnitAbilities(unit, true));
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

        protected override XElement GetRatingsObject(Hero hero)
        {
            return new XElement(
                "Ratings",
                new XAttribute("complexity", hero.Ratings.Complexity),
                new XAttribute("damage", hero.Ratings.Damage),
                new XAttribute("survivability", hero.Ratings.Survivability),
                new XAttribute("utility", hero.Ratings.Utility));
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

        protected override XElement GetSubAbilitiesObject(ILookup<string, Ability> linkedAbilities)
        {
            return new XElement(
                "SubAbilities",
                linkedAbilities.Select(parent => new XElement(
                    parent.Key,
                    parent.Where(x => x.Tier == AbilityTier.Basic).Count() > 0 ? new XElement("Basic", parent.Where(x => x.Tier == AbilityTier.Basic).Select(ability => AbilityTalentInfoElement(ability))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Heroic).Count() > 0 ? new XElement("Heroic", parent.Where(x => x.Tier == AbilityTier.Heroic).Select(ability => AbilityTalentInfoElement(ability))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Trait).Count() > 0 ? new XElement("Trait", parent.Where(x => x.Tier == AbilityTier.Trait).Select(ability => AbilityTalentInfoElement(ability))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Mount).Count() > 0 ? new XElement("Mount", parent.Where(x => x.Tier == AbilityTier.Mount).Select(ability => AbilityTalentInfoElement(ability))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Activable).Count() > 0 ? new XElement("Activable", parent.Where(x => x.Tier == AbilityTier.Activable).Select(ability => AbilityTalentInfoElement(ability))) : null,
                    parent.Where(x => x.Tier == AbilityTier.Hearth).Count() > 0 ? new XElement("Hearth", parent.Where(x => x.Tier == AbilityTier.Hearth).Select(hearth => AbilityTalentInfoElement(hearth))) : null)));
        }

        protected override XElement GetTalentsObject(Hero hero)
        {
            return new XElement(
                "Talents",
                new XElement("Level1", hero.TierTalents(TalentTier.Level1).Select(level1 => TalentInfoElement(level1))),
                new XElement("Level4", hero.TierTalents(TalentTier.Level4).Select(level4 => TalentInfoElement(level4))),
                new XElement("Level7", hero.TierTalents(TalentTier.Level7).Select(level7 => TalentInfoElement(level7))),
                new XElement("Level10", hero.TierTalents(TalentTier.Level10).Select(level10 => TalentInfoElement(level10))),
                new XElement("Level13", hero.TierTalents(TalentTier.Level13).Select(level13 => TalentInfoElement(level13))),
                new XElement("Level16", hero.TierTalents(TalentTier.Level16).Select(level16 => TalentInfoElement(level16))),
                new XElement("Level20", hero.TierTalents(TalentTier.Level20).Select(level20 => TalentInfoElement(level20))));
        }

        protected override XElement GetUnitsObject(Hero hero)
        {
            return new XElement(
                "HeroUnits",
                hero.HeroUnits.Select(heroUnit => new XElement(heroUnit.CUnitId, UnitElement(heroUnit))));
        }

        protected override XElement AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase)
        {
            if (IsLocalizedText)
                AddLocalizedGameString(abilityTalentBase);

            return new XElement(
                XmlConvert.EncodeName(abilityTalentBase.ReferenceNameId),
                string.IsNullOrEmpty(abilityTalentBase.Name) ? null : new XAttribute("name", abilityTalentBase.Name),
                string.IsNullOrEmpty(abilityTalentBase.ShortTooltipNameId) ? null : new XAttribute("shortTooltipId", abilityTalentBase.ShortTooltipNameId),
                string.IsNullOrEmpty(abilityTalentBase.FullTooltipNameId) ? null : new XAttribute("fullTooltipId", abilityTalentBase.FullTooltipNameId),
                new XAttribute("abilityType", abilityTalentBase.AbilityType.ToString()),
                abilityTalentBase.IsActive ? new XAttribute("isActive", abilityTalentBase.IsActive) : null,
                abilityTalentBase.IsQuest ? new XAttribute("isQuest", abilityTalentBase.IsQuest) : null,
                new XElement("Icon", Path.ChangeExtension(abilityTalentBase.IconFileName, FileSettings.ImageExtension)),
                abilityTalentBase.Tooltip.Cooldown.ToggleCooldown.HasValue ? new XElement("ToggleCooldown", abilityTalentBase.Tooltip.Cooldown.ToggleCooldown.Value) : null,
                UnitAbilityTalentLifeCost(abilityTalentBase.Tooltip.Life),
                UnitAbilityTalentEnergyCost(abilityTalentBase.Tooltip.Energy),
                UnitAbilityTalentCharges(abilityTalentBase.Tooltip.Charges),
                UnitAbilityTalentCooldown(abilityTalentBase.Tooltip.Cooldown),
                string.IsNullOrEmpty(abilityTalentBase.Tooltip.ShortTooltip?.RawDescription) || IsLocalizedText ? null : new XElement("ShortTooltip", GetTooltip(abilityTalentBase.Tooltip.ShortTooltip, FileSettings.DescriptionType)),
                string.IsNullOrEmpty(abilityTalentBase.Tooltip.FullTooltip?.RawDescription) || IsLocalizedText ? null : new XElement("FullTooltip", GetTooltip(abilityTalentBase.Tooltip.FullTooltip, FileSettings.DescriptionType)));
        }

        protected override XElement TalentInfoElement(Talent talent)
        {
            XElement element = AbilityTalentInfoElement(talent);
            element.Add(new XAttribute("sort", talent.Column));

            List<string> abilityTalentLinkIds = talent.AbilityTalentLinkIds?.ToList();
            if (abilityTalentLinkIds?.Count > 0)
                element.Add(new XElement("AbilityTalentLinkIds", talent.AbilityTalentLinkIds.Select(link => new XElement("AbilityTalentLinkId", link))));

            return element;
        }

        protected override XElement GetAbilityTalentLifeCostObject(TooltipLife tooltipLife)
        {
            return new XElement(
                "LifeTooltip",
                GetTooltip(tooltipLife.LifeCostTooltip, FileSettings.DescriptionType));
        }

        protected override XElement GetAbilityTalentEnergyCostObject(TooltipEnergy tooltipEnergy)
        {
            return new XElement(
                "EnergyTooltip",
                GetTooltip(tooltipEnergy.EnergyTooltip, FileSettings.DescriptionType));
        }

        protected override XElement GetAbilityTalentCooldownObject(TooltipCooldown tooltipCooldown)
        {
            return new XElement(
                "CooldownTooltip",
                GetTooltip(tooltipCooldown.CooldownTooltip, FileSettings.DescriptionType));
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

        protected override XElement GetPortraitObject(Hero hero)
        {
            return new XElement(
                "Portraits",
                FileSettings.HeroSelectPortrait ? new XElement("HeroSelect", Path.ChangeExtension(hero.HeroPortrait.HeroSelectPortraitFileName, FileSettings.ImageExtension)) : null,
                FileSettings.LeaderboardPortrait ? new XElement("Leaderboard", Path.ChangeExtension(hero.HeroPortrait.LeaderboardPortraitFileName, FileSettings.ImageExtension)) : null,
                FileSettings.LoadingPortraitPortrait ? new XElement("Loading", Path.ChangeExtension(hero.HeroPortrait.LoadingScreenPortraitFileName, FileSettings.ImageExtension)) : null,
                FileSettings.PartyPanelPortrait ? new XElement("PartyFrame", Path.ChangeExtension(hero.HeroPortrait.PartyPanelPortraitFileName, FileSettings.ImageExtension)) : null,
                FileSettings.TargetPortrait ? new XElement("Target", Path.ChangeExtension(hero.HeroPortrait.TargetPortraitFileName, FileSettings.ImageExtension)) : null);
        }
    }
}
