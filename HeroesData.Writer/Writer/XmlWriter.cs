using HeroesData.FileWriter.Settings;
using HeroesData.Parser.Models;
using HeroesData.Parser.Models.AbilityTalents;
using HeroesData.Parser.Models.AbilityTalents.Tooltip;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writer
{
    internal class XmlWriter : Writer<XElement, XElement>
    {
        private readonly string SingleFileName = "heroesdata.xml";
        private XmlWriter(XmlFileSettings fileSettings, List<Hero> heroes)
        {
            FileSettings = fileSettings;

            if (FileSettings.WriterEnabled)
            {
                if (FileSettings.FileSplit)
                    CreateMultipleFiles(heroes);
                else
                    CreateSingleFile(heroes);
            }
        }

        public static void CreateOutput(XmlFileSettings fileSettings, List<Hero> heroes)
        {
            new XmlWriter(fileSettings, heroes);
        }

        protected override void CreateSingleFile(List<Hero> heroes)
        {
            XDocument xmlDoc = new XDocument(new XElement(RootNode, heroes.Select(hero => HeroElement(hero))));
            xmlDoc.Save(Path.Combine(XmlOutputFolder, SingleFileName));
        }

        protected override void CreateMultipleFiles(List<Hero> heroes)
        {
            foreach (Hero hero in heroes)
            {
                XDocument xmlDoc = new XDocument(new XElement(RootNode, HeroElement(hero)));

                xmlDoc.Save(Path.Combine(XmlOutputFolder, $"{hero.ShortName}.xml"));
            }
        }

        protected override XElement HeroElement(Hero hero)
        {
            return new XElement(
                hero.ShortName,
                new XAttribute("name", hero.Name),
                string.IsNullOrEmpty(hero.CHeroId) ? null : new XAttribute("cHeroId", hero.CHeroId),
                string.IsNullOrEmpty(hero.CUnitId) ? null : new XAttribute("cUnitId", hero.CUnitId),
                string.IsNullOrEmpty(hero.AttributeId) ? null : new XAttribute("attributeId", hero.AttributeId),
                new XAttribute("difficulty", hero.Difficulty),
                new XAttribute("franchise", hero.Franchise),
                hero.Gender.HasValue ? new XAttribute("gender", hero.Gender.Value) : null,
                hero.InnerRadius > 0 ? new XAttribute("innerRadius", hero.InnerRadius) : null,
                hero.Radius > 0 ? new XAttribute("radius", hero.Radius) : null,
                hero.ReleaseDate.HasValue ? new XAttribute("releaseDate", hero.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null,
                hero.Sight > 0 ? new XAttribute("sight", hero.Sight) : null,
                hero.Speed > 0 ? new XAttribute("speed", hero.Speed) : null,
                hero.Type.HasValue ? new XAttribute("type", hero.Type.Value) : null,
                hero.Rarity.HasValue ? new XAttribute("rarity", hero.Rarity.Value) : null,
                string.IsNullOrEmpty(hero.Description?.RawDescription) ? null : new XElement("Description", GetTooltip(hero.Description, FileSettings.Description)),
                UnitLife(hero),
                UnitEnergy(hero),
                hero.Roles?.Count > 0 ? new XElement("Roles", hero.Roles.Select(r => new XElement("Role", r))) : null,
                HeroRatings(hero),
                UnitWeapons(hero),
                UnitAbilities(hero, false),
                UnitSubAbilities(hero),
                HeroTalents(hero),
                Units(hero));
        }

        protected override XElement UnitElement(Unit unit)
        {
            return new XElement(
                unit.ShortName,
                new XAttribute("name", unit.Name),
                string.IsNullOrEmpty(unit.CUnitId) ? null : new XAttribute("cUnitId", unit.CUnitId),
                unit.InnerRadius > 0 ? new XAttribute("innerRadius", unit.InnerRadius) : null,
                unit.Radius > 0 ? new XAttribute("radius", unit.Radius) : null,
                unit.Sight > 0 ? new XAttribute("sight", unit.Sight) : null,
                unit.Speed > 0 ? new XAttribute("speed", unit.Speed) : null,
                unit.Type.HasValue ? new XAttribute("type", unit.Type.Value) : null,
                string.IsNullOrEmpty(unit.Description?.RawDescription) ? null : new XElement("Description", GetTooltip(unit.Description, FileSettings.Description)),
                UnitLife(unit),
                UnitEnergy(unit),
                UnitWeapons(unit),
                UnitAbilities(unit, true));
        }

        protected override XElement GetLifeObject(Unit unit)
        {
            return new XElement(
                "Life",
                new XElement("LifeAmount", unit.Life.LifeMax, new XAttribute("scale", unit.Life.LifeScaling)),
                new XElement("LifeRegenRate", unit.Life.LifeRegenerationRate, new XAttribute("scale", unit.Life.LifeRegenerationRateScaling)));
        }

        protected override XElement GetEnergyObject(Unit unit)
        {
            return new XElement(
                "Energy",
                new XElement("EnergyAmount", unit.Energy.EnergyMax, new XAttribute("type", unit.Energy.EnergyType)),
                new XElement("EnergyRegenRate", unit.Energy.EnergyRegenerationRate));
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
                    unit.SubAbilities(AbilityTier.Activable)?.Count > 0 ? new XElement("Activable", unit.SubAbilities(AbilityTier.Activable).Select(activable => AbilityTalentInfoElement(activable))) : null);
            }
            else
            {
                return new XElement(
                    "Abilities",
                    unit.PrimaryAbilities(AbilityTier.Basic)?.Count > 0 ? new XElement("Basic", unit.PrimaryAbilities(AbilityTier.Basic).Select(basic => AbilityTalentInfoElement(basic))) : null,
                    unit.PrimaryAbilities(AbilityTier.Heroic)?.Count > 0 ? new XElement("Heroic", unit.PrimaryAbilities(AbilityTier.Heroic).Select(heroic => AbilityTalentInfoElement(heroic))) : null,
                    unit.PrimaryAbilities(AbilityTier.Trait)?.Count > 0 ? new XElement("Trait", unit.PrimaryAbilities(AbilityTier.Trait).Select(trait => AbilityTalentInfoElement(trait))) : null,
                    unit.PrimaryAbilities(AbilityTier.Mount)?.Count > 0 ? new XElement("Mount", unit.PrimaryAbilities(AbilityTier.Mount).Select(mount => AbilityTalentInfoElement(mount))) : null,
                    unit.PrimaryAbilities(AbilityTier.Activable)?.Count > 0 ? new XElement("Activable", unit.PrimaryAbilities(AbilityTier.Activable).Select(activable => AbilityTalentInfoElement(activable))) : null);
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
                    parent.Where(x => x.Tier == AbilityTier.Activable).Count() > 0 ? new XElement("Activable", parent.Where(x => x.Tier == AbilityTier.Activable).Select(ability => AbilityTalentInfoElement(ability))) : null)));
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
                HeroUnits,
                hero.HeroUnits.Select(heroUnit => new XElement(heroUnit.CUnitId, UnitElement(heroUnit))));
        }

        protected override XElement AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase)
        {
            return new XElement(
                XmlConvert.EncodeName(abilityTalentBase.ReferenceNameId),
                new XAttribute("name", abilityTalentBase.Name),
                string.IsNullOrEmpty(abilityTalentBase.ShortTooltipNameId) ? null : new XAttribute("shortTooltipId", abilityTalentBase.ShortTooltipNameId),
                string.IsNullOrEmpty(abilityTalentBase.FullTooltipNameId) ? null : new XAttribute("fullTooltipId", abilityTalentBase.FullTooltipNameId),
                new XElement("Icon", Path.ChangeExtension(abilityTalentBase.IconFileName, FileSettings.ImageExtension)),
                UnitAbilityLifeCost(abilityTalentBase.Tooltip.Life),
                UnitAbilityEnergyCost(abilityTalentBase.Tooltip.Energy),
                UnitAbilityCooldown(abilityTalentBase.Tooltip.Cooldown),
                UnitAbilityCharges(abilityTalentBase.Tooltip.Charges),
                string.IsNullOrEmpty(abilityTalentBase.Tooltip.Custom) ? null : new XElement("Custom", abilityTalentBase.Tooltip.Custom),
                string.IsNullOrEmpty(abilityTalentBase.Tooltip.ShortTooltip?.RawDescription) ? null : new XElement("ShortTooltip", GetTooltip(abilityTalentBase.Tooltip.ShortTooltip, FileSettings.ShortTooltip)),
                string.IsNullOrEmpty(abilityTalentBase.Tooltip.FullTooltip?.RawDescription) ? null : new XElement("FullTooltip", GetTooltip(abilityTalentBase.Tooltip.FullTooltip, FileSettings.FullTooltip)));
        }

        protected override XElement TalentInfoElement(Talent talent)
        {
            XElement element = AbilityTalentInfoElement(talent);
            element.Add(new XAttribute("sort", talent.Column));

            return element;
        }

        protected override XElement GetAbilityLifeCostObject(TooltipLife tooltipLife)
        {
            return new XElement(
                "Life",
                tooltipLife.LifeCost,
                tooltipLife.IsLifePercentage == true ? new XAttribute("isPercentage", tooltipLife.IsLifePercentage) : null);
        }

        protected override XElement GetAbilityEnergyCostObject(TooltipEnergy tooltipEnergy)
        {
            return new XElement(
                "Energy",
                tooltipEnergy.EnergyCost,
                tooltipEnergy.IsPerCost == true ? new XAttribute("isPerCost", tooltipEnergy.IsPerCost) : null);
        }

        protected override XElement GetAbilityCooldownObject(TooltipCooldown tooltipCooldown)
        {
            return new XElement(
                "Cooldown",
                tooltipCooldown.CooldownValue,
                tooltipCooldown.RecastCooldown.HasValue == true ? new XAttribute("recast", tooltipCooldown.RecastCooldown.Value) : null);
        }

        protected override XElement GetAbilityChargesObject(TooltipCharges tooltipCharges)
        {
            return new XElement(
                "Charges",
                tooltipCharges.CountMax,
                tooltipCharges.CountUse.HasValue == true ? new XAttribute("consume", tooltipCharges.CountUse.Value) : null,
                tooltipCharges.CountStart.HasValue == true ? new XAttribute("initial", tooltipCharges.CountStart.Value) : null,
                tooltipCharges.IsHideCount.HasValue == true ? new XAttribute("isHidden", tooltipCharges.IsHideCount.Value) : null);
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
