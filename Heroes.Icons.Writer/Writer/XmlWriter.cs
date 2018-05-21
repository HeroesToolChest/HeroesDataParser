using Heroes.Icons.FileWriter.Settings;
using Heroes.Icons.Parser.Models;
using Heroes.Icons.Parser.Models.AbilityTalents;
using Heroes.Icons.Parser.Models.AbilityTalents.Tooltip;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Heroes.Icons.FileWriter.Writer
{
    internal class XmlWriter : Writer
    {
        private readonly XmlFileSettings FileSettings;

        private XmlWriter(XmlFileSettings fileSettings, List<Hero> heroes)
        {
            FileSettings = fileSettings;

            if (FileSettings.WriterEnabled)
            {
                if (FileSettings.FileSplit)
                    CreateMultipleFilesXml(heroes);
                else
                    CreateSingleFileXml(heroes);
            }
        }

        public static void CreateOutput(XmlFileSettings fileSettings, List<Hero> heroes)
        {
            new XmlWriter(fileSettings, heroes);
        }

        private void CreateSingleFileXml(List<Hero> heroes)
        {
            XDocument xmlDoc = new XDocument(
                new XElement(
                    "Heroes",
                    heroes.Select(
                        hero => HeroElement(hero))));

            xmlDoc.Save(Path.Combine(OutputFolder, "heroesdata.xml"));
        }

        private void CreateMultipleFilesXml(List<Hero> heroes)
        {
            foreach (Hero hero in heroes)
            {
                XDocument xmlDoc = new XDocument(new XElement("Heroes", HeroElement(hero)));

                xmlDoc.Save(Path.Combine(OutputFolder, $"{hero.ShortName}.xml"));
            }
        }

        private XElement HeroElement(Hero hero)
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
                new XAttribute("innerRadius", hero.InnerRadius),
                new XAttribute("radius", hero.Radius),
                hero.ReleaseDate.HasValue ? new XAttribute("releaseDate", hero.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null,
                new XAttribute("sight", hero.Sight),
                new XAttribute("speed", hero.Speed),
                hero.Type.HasValue ? new XAttribute("type", hero.Type.Value) : null,
                hero.Rarity.HasValue ? new XAttribute("rarity", hero.Rarity.Value) : null,
                string.IsNullOrEmpty(hero.Description?.RawDescription) ? null : new XElement("Description", GetTooltip(hero.Description, FileSettings.Description)),
                new XElement(
                    "Life",
                    new XElement("LifeAmount", hero.Life.LifeMax, new XAttribute("scale", hero.Life.LifeScaling)),
                    new XElement("LifeRegenRate", hero.Life.LifeRegenerationRate, new XAttribute("scale", hero.Life.LifeRegenerationRateScaling))),
                new XElement(
                    "Energy",
                    new XElement("EnergyAmount", hero.Energy.EnergyMax, new XAttribute("type", hero.Energy.EnergyType)),
                    new XElement("EnergyRegenRate", hero.Energy.EnergyRegenerationRate)),
                hero.Roles != null ? new XElement("Roles", hero.Roles.Select(r => new XElement("Role", r))) : null,
                RatingsElement(hero),
                WeaponsElement(hero),
                AbilitiesElement(hero),
                ExtraAbilitiesElement(hero),
                TalentsElement(hero),
                HeroUnitsElement(hero));
        }

        private XElement RatingsElement(Hero hero)
        {
            if (hero.Ratings != null)
            {
                return new XElement(
                    "Ratings",
                    new XAttribute("complexity", hero.Ratings.Complexity),
                    new XAttribute("damage", hero.Ratings.Damage),
                    new XAttribute("survivability", hero.Ratings.Survivability),
                    new XAttribute("utility", hero.Ratings.Utility));
            }
            else
            {
                return null;
            }
        }

        private XElement WeaponsElement(Hero hero)
        {
            if (FileSettings.IncludeWeapons && hero.Weapons?.Count > 0)
            {
                return new XElement(
                    "Weapons",
                    hero.Weapons.Select(w => new XElement(
                        w.WeaponNameId,
                        new XAttribute("range", w.Range),
                        new XAttribute("period", w.Period),
                        new XElement("Damage", w.Damage, new XAttribute("scale", w.DamageScaling)))));
            }
            else
            {
                return null;
            }
        }

        private XElement AbilitiesElement(Hero hero)
        {
            if (FileSettings.IncludeAbilities && hero.Abilities?.Count > 0)
            {
                return new XElement(
                    "Abilities",
                    hero.TierAbilities(AbilityTier.Basic, false).Count > 0 ? new XElement("Basic", hero.TierAbilities(AbilityTier.Basic, false).Select(basic => AbilityTalentInfoElement(basic))) : null,
                    hero.TierAbilities(AbilityTier.Heroic, false).Count > 0 ? new XElement("Heroic", hero.TierAbilities(AbilityTier.Heroic, false).Select(heroic => AbilityTalentInfoElement(heroic))) : null,
                    hero.TierAbilities(AbilityTier.Trait, false).Count > 0 ? new XElement("Trait", hero.TierAbilities(AbilityTier.Trait, false).Select(trait => AbilityTalentInfoElement(trait))) : null,
                    hero.TierAbilities(AbilityTier.Mount, false).Count > 0 ? new XElement("Mount", hero.TierAbilities(AbilityTier.Mount, false).Select(mount => AbilityTalentInfoElement(mount))) : null,
                    hero.TierAbilities(AbilityTier.Activable, false).Count > 0 ? new XElement("Activable", hero.TierAbilities(AbilityTier.Activable, false).Select(activable => AbilityTalentInfoElement(activable))) : null);
            }
            else
            {
                return null;
            }
        }

        private XElement ExtraAbilitiesElement(Hero hero)
        {
            if (FileSettings.IncludeExtraAbilities && hero.Abilities?.Count > 0)
            {
                ILookup<string, Ability> linkedAbilities = hero.ParentLinkedAbilities();
                if (linkedAbilities.Count > 0)
                {
                    return new XElement(
                        "ExtraAbilities",
                        linkedAbilities.Select(parent => new XElement(
                            parent.Key,
                            parent.Where(x => x.Tier == AbilityTier.Basic).Count() > 0 ? new XElement("Basic", parent.Where(x => x.Tier == AbilityTier.Basic).Select(ability => AbilityTalentInfoElement(ability))) : null,
                            parent.Where(x => x.Tier == AbilityTier.Heroic).Count() > 0 ? new XElement("Heroic", parent.Where(x => x.Tier == AbilityTier.Heroic).Select(ability => AbilityTalentInfoElement(ability))) : null,
                            parent.Where(x => x.Tier == AbilityTier.Trait).Count() > 0 ? new XElement("Trait", parent.Where(x => x.Tier == AbilityTier.Trait).Select(ability => AbilityTalentInfoElement(ability))) : null,
                            parent.Where(x => x.Tier == AbilityTier.Mount).Count() > 0 ? new XElement("Mount", parent.Where(x => x.Tier == AbilityTier.Mount).Select(ability => AbilityTalentInfoElement(ability))) : null,
                            parent.Where(x => x.Tier == AbilityTier.Activable).Count() > 0 ? new XElement("Activable", parent.Where(x => x.Tier == AbilityTier.Activable).Select(ability => AbilityTalentInfoElement(ability))) : null)));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private XElement TalentsElement(Hero hero)
        {
            if (FileSettings.IncludeTalents && hero.Talents?.Count > 0)
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
            else
            {
                return null;
            }
        }

        private XElement HeroUnitsElement(Hero hero)
        {
            if (FileSettings.IncludeHeroUnits && hero.HeroUnits?.Count > 0)
            {
                return new XElement(
                    "HeroUnits",
                    hero.HeroUnits?.Select(heroUnit => new XElement(heroUnit.ParentLink, HeroElement(heroUnit))));
            }
            else
            {
                return null;
            }
        }

        private XElement AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase)
        {
            return new XElement(
                XmlConvert.EncodeName(abilityTalentBase.ReferenceNameId),
                new XAttribute("name", abilityTalentBase.Name),
                string.IsNullOrEmpty(abilityTalentBase.ShortTooltipNameId) ? null : new XAttribute("shortTooltipId", abilityTalentBase.ShortTooltipNameId),
                string.IsNullOrEmpty(abilityTalentBase.FullTooltipNameId) ? null : new XAttribute("fullTooltipId", abilityTalentBase.FullTooltipNameId),
                new XElement("Icon", Path.ChangeExtension(abilityTalentBase.IconFileName, FileSettings.ImageExtension)),
                AbilityLifeElement(abilityTalentBase.Tooltip.Life),
                AbilityEnergyElement(abilityTalentBase.Tooltip.Energy),
                AbilityCooldownElement(abilityTalentBase.Tooltip.Cooldown),
                AbilityChargesElement(abilityTalentBase.Tooltip.Charges),
                string.IsNullOrEmpty(abilityTalentBase.Tooltip.Custom) ? null : new XElement("Custom", abilityTalentBase.Tooltip.Custom),
                string.IsNullOrEmpty(abilityTalentBase.Tooltip.ShortTooltip?.RawDescription) ? null : new XElement("ShortTooltip", GetTooltip(abilityTalentBase.Tooltip.ShortTooltip, FileSettings.ShortTooltip)),
                string.IsNullOrEmpty(abilityTalentBase.Tooltip.FullTooltip?.RawDescription) ? null : new XElement("FullTooltip", GetTooltip(abilityTalentBase.Tooltip.FullTooltip, FileSettings.FullTooltip)));
        }

        private XElement TalentInfoElement(Talent talent)
        {
            XElement element = AbilityTalentInfoElement(talent);
            element.Add(new XAttribute("sort", talent.Column));

            return element;
        }

        private XElement AbilityLifeElement(TooltipLife tooltipLife)
        {
            if (tooltipLife.LifeCost.HasValue)
            {
                XElement lifeElement = new XElement(
                    "Life",
                    tooltipLife.LifeCost,
                    tooltipLife.IsLifePercentage == true ? new XAttribute("isPercentage", tooltipLife.IsLifePercentage) : null);

                return lifeElement;
            }

            return null;
        }

        private XElement AbilityEnergyElement(TooltipEnergy tooltipEnergy)
        {
            if (tooltipEnergy.EnergyCost.HasValue)
            {
                XElement energyElement = new XElement(
                    "Energy",
                    tooltipEnergy.EnergyCost,
                    new XAttribute("type", tooltipEnergy.EnergyType),
                    tooltipEnergy.IsPerCost == true ? new XAttribute("isPerCost", tooltipEnergy.IsPerCost) : null);

                return energyElement;
            }

            return null;
        }

        private XElement AbilityCooldownElement(TooltipCooldown tooltipCooldown)
        {
            if (tooltipCooldown.CooldownValue.HasValue)
            {
                XElement cooldownElement = new XElement(
                    "Cooldown",
                    tooltipCooldown.CooldownValue,
                    tooltipCooldown.RecastCooldown.HasValue == true ? new XAttribute("recast", tooltipCooldown.RecastCooldown.Value) : null);

                return cooldownElement;
            }

            return null;
        }

        private XElement AbilityChargesElement(TooltipCharges tooltipCharges)
        {
            if (tooltipCharges.HasCharges)
            {
                XElement chargesElement = new XElement(
                    "Charges",
                    tooltipCharges.CountMax,
                    tooltipCharges.CountUse.HasValue == true ? new XAttribute("consume", tooltipCharges.CountUse.Value) : null,
                    tooltipCharges.CountStart.HasValue == true ? new XAttribute("initial", tooltipCharges.CountStart.Value) : null,
                    tooltipCharges.IsHideCount.HasValue == true ? new XAttribute("isHidden", tooltipCharges.IsHideCount.Value) : null);

                return chargesElement;
            }

            return null;
        }

        private string GetTooltip(TooltipDescription tooltipDescription, int setting)
        {
            if (setting == 0)
                return tooltipDescription.RawDescription;
            else if (setting == 1)
                return tooltipDescription.PlainText;
            else if (setting == 2)
                return tooltipDescription.PlainTextWithNewlines;
            else if (setting == 3)
                return tooltipDescription.PlainTextWithScaling;
            else if (setting == 4)
                return tooltipDescription.PlainTextWithScalingWithNewlines;
            else if (setting == 6)
                return tooltipDescription.ColoredTextWithScaling;
            else
                return tooltipDescription.ColoredText;
        }
    }
}
