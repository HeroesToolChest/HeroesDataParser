﻿using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using HeroesData.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.FileWriter.Writers.HeroData
{
    internal class HeroDataJsonWriter : HeroDataWriter<JProperty, JObject>
    {
        public HeroDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(Hero hero)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(hero);

            JObject heroObject = new JObject();

            if (!string.IsNullOrEmpty(hero.Name) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("name", hero.Name);
            if (!string.IsNullOrEmpty(hero.CUnitId) && hero.CHeroId != StormHero.CHeroId)
                heroObject.Add("unitId", hero.CUnitId);
            if (!string.IsNullOrEmpty(hero.HyperlinkId) && hero.CHeroId != StormHero.CHeroId)
                heroObject.Add("hyperlinkId", hero.HyperlinkId);
            if (!string.IsNullOrEmpty(hero.AttributeId))
                heroObject.Add("attributeId", hero.AttributeId);

            if (!FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(hero.Difficulty))
                heroObject.Add("difficulty", hero.Difficulty);

            if (hero.CHeroId != StormHero.CHeroId)
                heroObject.Add("franchise", hero.Franchise.ToString());

            if (hero.Gender.HasValue)
                heroObject.Add("gender", hero.Gender.Value.ToString());
            if (!FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(hero.Title))
                heroObject.Add("title", hero.Title);
            if (hero.InnerRadius > 0)
                heroObject.Add("innerRadius", hero.InnerRadius);
            if (hero.Radius > 0)
                heroObject.Add("radius", hero.Radius);
            if (hero.ReleaseDate.HasValue)
                heroObject.Add("releaseDate", hero.ReleaseDate.Value.ToString("yyyy-MM-dd"));
            if (hero.Sight > 0)
                heroObject.Add("sight", hero.Sight);
            if (hero.Speed > 0)
                heroObject.Add("speed", hero.Speed);
            if (!string.IsNullOrEmpty(hero.Type) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("type", hero.Type);
            if (hero.Rarity.HasValue)
                heroObject.Add("rarity", hero.Rarity.Value.ToString());
            if (!string.IsNullOrEmpty(hero.MountLinkId))
                heroObject.Add("mountLinkId", hero.MountLinkId);
            if (!string.IsNullOrEmpty(hero.MountLinkId))
                heroObject.Add("hearthLinkId", hero.HearthLinkId);
            if (!FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(hero.SearchText))
                heroObject.Add("searchText", hero.SearchText);
            if (!string.IsNullOrEmpty(hero.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("description", GetTooltip(hero.Description, FileOutputOptions.DescriptionType));
            if (hero.HeroDescriptors.Count > 0)
                heroObject.Add(new JProperty("descriptors", hero.HeroDescriptors));

            JProperty portraits = HeroPortraits(hero);
            if (portraits != null)
                heroObject.Add(portraits);

            JProperty life = UnitLife(hero);
            if (life != null)
                heroObject.Add(life);

            JProperty energy = UnitEnergy(hero);
            if (energy != null)
                heroObject.Add(energy);

            JProperty armor = UnitArmor(hero);
            if (armor != null)
                heroObject.Add(armor);

            if (hero.Roles?.Count > 0 && !FileOutputOptions.IsLocalizedText)
                heroObject.Add(new JProperty("roles", hero.Roles));

            JProperty ratings = HeroRatings(hero);
            if (ratings != null)
                heroObject.Add(ratings);

            JProperty weapons = UnitWeapons(hero);
            if (weapons != null)
                heroObject.Add(weapons);

            JProperty abilities = UnitAbilities(hero, false);
            if (abilities != null)
                heroObject.Add(abilities);

            JProperty subAbilities = UnitSubAbilities(hero);
            if (subAbilities != null)
                heroObject.Add(subAbilities);

            JProperty talents = HeroTalents(hero);
            if (talents != null)
                heroObject.Add(talents);

            JProperty units = Units(hero);
            if (units != null)
                heroObject.Add(units);

            return new JProperty(hero.Id, heroObject);
        }

        protected override JProperty UnitElement(Unit unit)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(unit);

            JObject heroObject = new JObject();

            if (!string.IsNullOrEmpty(unit.Name) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("name", unit.Name);
            if (!string.IsNullOrEmpty(unit.CUnitId))
                heroObject.Add("unitId", unit.CUnitId);
            if (unit.InnerRadius > 0)
                heroObject.Add("innerRadius", unit.InnerRadius);
            if (unit.Radius > 0)
                heroObject.Add("radius", unit.Radius);
            if (unit.Sight > 0)
                heroObject.Add("sight", unit.Sight);
            if (unit.Speed > 0)
                heroObject.Add("speed", unit.Speed);
            if (!string.IsNullOrEmpty(unit.Type) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("type", unit.Type);
            if (!string.IsNullOrEmpty(unit.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("description", GetTooltip(unit.Description, FileOutputOptions.DescriptionType));
            if (unit.HeroDescriptors.Count > 0)
                heroObject.Add(new JProperty("descriptors", unit.HeroDescriptors));

            JProperty life = UnitLife(unit);
            if (life != null)
                heroObject.Add(life);

            JProperty armor = UnitArmor(unit);
            if (armor != null)
                heroObject.Add(armor);

            JProperty energy = UnitEnergy(unit);
            if (energy != null)
                heroObject.Add(energy);

            JProperty weapons = UnitWeapons(unit);
            if (weapons != null)
                heroObject.Add(weapons);

            JProperty abilities = UnitAbilities(unit, true);
            if (abilities != null)
                heroObject.Add(abilities);

            return new JProperty(unit.HyperlinkId, heroObject);
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

        protected override JProperty GetSubAbilitiesObject(ILookup<string, Ability> linkedAbilities)
        {
            JObject parentLink = null;

            IEnumerable<string> parentLinks = linkedAbilities.Select(x => x.Key);
            foreach (string parent in parentLinks)
            {
                JObject abilities = new JObject();

                IEnumerable<Ability> basicAbilities = linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Basic);
                if (basicAbilities.Count() > 0)
                {
                    abilities.Add(new JProperty(
                        "basic",
                        new JArray(
                            from abil in basicAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> heroicAbilities = linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Heroic);
                if (heroicAbilities.Count() > 0)
                {
                    abilities.Add(new JProperty(
                        "heroic",
                        new JArray(
                            from abil in heroicAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> traitAbilities = linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Trait);
                if (traitAbilities.Count() > 0)
                {
                    abilities.Add(new JProperty(
                        "trait",
                        new JArray(
                            from abil in traitAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> mountAbilities = linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Mount);
                if (mountAbilities.Count() > 0)
                {
                    abilities.Add(new JProperty(
                        "mount",
                        new JArray(
                            from abil in mountAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                IEnumerable<Ability> activableAbilities = linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Activable);
                if (activableAbilities.Count() > 0)
                {
                    abilities.Add(new JProperty(
                        "activable",
                        new JArray(
                            from abil in activableAbilities
                            select new JObject(AbilityTalentInfoElement(abil)))));
                }

                if (parentLink == null)
                    parentLink = new JObject();
                parentLink.Add(new JProperty(parent, abilities));
            }

            return new JProperty("subAbilities", new JArray(new JObject(parentLink)));
        }

        protected override JProperty GetUnitsObject(Hero hero)
        {
            return new JProperty(
                "heroUnits",
                new JArray(
                    from heroUnit in hero.HeroUnits
                    select new JObject(UnitElement(heroUnit))));
        }

        protected override JProperty GetRatingsObject(Hero hero)
        {
            return new JProperty(
                "ratings",
                new JObject(
                    new JProperty("complexity", hero.Ratings.Complexity),
                    new JProperty("damage", hero.Ratings.Damage),
                    new JProperty("survivability", hero.Ratings.Survivability),
                    new JProperty("utility", hero.Ratings.Utility)));
        }

        protected override JObject TalentInfoElement(Talent talent)
        {
            JObject jObject = AbilityTalentInfoElement(talent);
            jObject.Add(new JProperty("sort", talent.Column));

            List<string> abilityTalentLinkIds = talent.AbilityTalentLinkIds?.ToList();
            if (abilityTalentLinkIds?.Count > 0)
                jObject.Add(new JProperty("abilityTalentLinkIds", talent.AbilityTalentLinkIds));

            return jObject;
        }

        protected override JProperty GetTalentsObject(Hero hero)
        {
            JObject talantObject = new JObject();

            ICollection<Talent> level1Talents = hero.TierTalents(TalentTier.Level1);
            if (level1Talents?.Count > 0)
            {
                talantObject.Add(new JProperty(
                    "level1",
                    new JArray(
                        from talent in level1Talents
                        select new JObject(TalentInfoElement(talent)))));
            }

            ICollection<Talent> level4Talents = hero.TierTalents(TalentTier.Level4);
            if (level4Talents?.Count > 0)
            {
                talantObject.Add(new JProperty(
                    "level4",
                    new JArray(
                        from talent in level4Talents
                        select new JObject(TalentInfoElement(talent)))));
            }

            ICollection<Talent> level7Talents = hero.TierTalents(TalentTier.Level7);
            if (level7Talents?.Count > 0)
            {
                talantObject.Add(new JProperty(
                    "level7",
                    new JArray(
                        from talent in level7Talents
                        select new JObject(TalentInfoElement(talent)))));
            }

            ICollection<Talent> level10Talents = hero.TierTalents(TalentTier.Level10);
            if (level10Talents?.Count > 0)
            {
                talantObject.Add(new JProperty(
                    "level10",
                    new JArray(
                        from talent in level10Talents
                        select new JObject(TalentInfoElement(talent)))));
            }

            ICollection<Talent> level13Talents = hero.TierTalents(TalentTier.Level13);
            if (level13Talents?.Count > 0)
            {
                talantObject.Add(new JProperty(
                    "level13",
                    new JArray(
                        from talent in level13Talents
                        select new JObject(TalentInfoElement(talent)))));
            }

            ICollection<Talent> level16Talents = hero.TierTalents(TalentTier.Level16);
            if (level13Talents?.Count > 0)
            {
                talantObject.Add(new JProperty(
                    "level16",
                    new JArray(
                        from talent in level16Talents
                        select new JObject(TalentInfoElement(talent)))));
            }

            ICollection<Talent> level20Talents = hero.TierTalents(TalentTier.Level20);
            if (level20Talents?.Count > 0)
            {
                talantObject.Add(new JProperty(
                    "level20",
                    new JArray(
                        from talent in level20Talents
                        select new JObject(TalentInfoElement(talent)))));
            }

            return new JProperty("talents", talantObject);
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

        protected override JProperty GetPortraitObject(Hero hero)
        {
            JObject portrait = new JObject();

            if (!string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName))
                portrait.Add("heroSelect", Path.ChangeExtension(hero.HeroPortrait.HeroSelectPortraitFileName, ImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName))
                portrait.Add("leaderboard", Path.ChangeExtension(hero.HeroPortrait.LeaderboardPortraitFileName, ImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName))
                portrait.Add("loading", Path.ChangeExtension(hero.HeroPortrait.LoadingScreenPortraitFileName, ImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName))
                portrait.Add("partyPanel", Path.ChangeExtension(hero.HeroPortrait.PartyPanelPortraitFileName, ImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName))
                portrait.Add("target", Path.ChangeExtension(hero.HeroPortrait.TargetPortraitFileName, ImageExtension));

            return new JProperty("portraits", portrait);
        }
    }
}
