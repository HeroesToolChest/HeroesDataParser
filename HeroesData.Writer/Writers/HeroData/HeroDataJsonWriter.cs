using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.FileWriter.Writers.HeroData
{
    internal class HeroDataJsonWriter : HeroDataWriter<JProperty, JObject, Hero>
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
            if (!string.IsNullOrEmpty(hero.CUnitId))
                heroObject.Add("unitId", hero.CUnitId);
            if (!string.IsNullOrEmpty(hero.HyperlinkId))
                heroObject.Add("hyperlinkId", hero.HyperlinkId);
            if (!string.IsNullOrEmpty(hero.AttributeId))
                heroObject.Add("attributeId", hero.AttributeId);

            if (!FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(hero.Difficulty))
                heroObject.Add("difficulty", hero.Difficulty);

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
            if (!string.IsNullOrEmpty(hero.ScalingBehaviorLink))
                heroObject.Add(new JProperty("scalingLinkId", hero.ScalingBehaviorLink));
            if (!FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(hero.SearchText))
                heroObject.Add("searchText", hero.SearchText);
            if (!string.IsNullOrEmpty(hero.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("description", GetTooltip(hero.Description, FileOutputOptions.DescriptionType));
            if (hero.HeroDescriptorsCount > 0)
                heroObject.Add(new JProperty("descriptors", hero.HeroDescriptors.OrderBy(x => x)));
            if (hero.UnitIdsCount > 0)
                heroObject.Add(new JProperty("units", hero.UnitIds.OrderBy(x => x)));

            JProperty portraits = HeroPortraits(hero);
            if (portraits != null)
                heroObject.Add(portraits);

            JProperty life = UnitLife(hero);
            if (life != null)
                heroObject.Add(life);

            JProperty shield = UnitShield(hero);
            if (shield != null)
                heroObject.Add(shield);

            JProperty energy = UnitEnergy(hero);
            if (energy != null)
                heroObject.Add(energy);

            JProperty armor = UnitArmor(hero);
            if (armor != null)
                heroObject.Add(armor);

            if (hero.RolesCount > 0 && !FileOutputOptions.IsLocalizedText)
                heroObject.Add(new JProperty("roles", hero.Roles));

            if (!string.IsNullOrEmpty(hero.ExpandedRole) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add(new JProperty("expandedRole", hero.ExpandedRole));

            JProperty ratings = HeroRatings(hero);
            if (ratings != null)
                heroObject.Add(ratings);

            JProperty weapons = UnitWeapons(hero);
            if (weapons != null)
                heroObject.Add(weapons);

            JProperty abilities = UnitAbilities(hero);
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

        protected override JObject AbilityInfoElement(Ability ability)
        {
            JObject jObject = AbilityTalentInfoElement(ability);

            return jObject;
        }

        protected override JObject TalentInfoElement(Talent talent)
        {
            JObject jObject = AbilityTalentInfoElement(talent);
            jObject.Add(new JProperty("sort", talent.Column));

            if (talent.AbilityTalentLinkIdsCount > 0)
                jObject.Add(new JProperty("abilityTalentLinkIds", talent.AbilityTalentLinkIds));
            if (talent.PrerequisiteTalentIdCount > 0)
                jObject.Add(new JProperty("prerequisiteTalentIds", talent.PrerequisiteTalentIds));

            return jObject;
        }

        protected override JProperty GetTalentsObject(Hero hero)
        {
            JObject talantObject = new JObject();

            SetTalents(talantObject, hero.TierTalents(TalentTier.Level1), "level1");
            SetTalents(talantObject, hero.TierTalents(TalentTier.Level4), "level4");
            SetTalents(talantObject, hero.TierTalents(TalentTier.Level7), "level7");
            SetTalents(talantObject, hero.TierTalents(TalentTier.Level10), "level10");
            SetTalents(talantObject, hero.TierTalents(TalentTier.Level13), "level13");
            SetTalents(talantObject, hero.TierTalents(TalentTier.Level16), "level16");
            SetTalents(talantObject, hero.TierTalents(TalentTier.Level20), "level20");

            return new JProperty("talents", talantObject);
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

        protected override JProperty GetPortraitObject(Hero hero)
        {
            JObject portrait = new JObject();

            if (!string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName))
                portrait.Add("heroSelect", Path.ChangeExtension(hero.HeroPortrait.HeroSelectPortraitFileName?.ToLower(), StaticImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName))
                portrait.Add("leaderboard", Path.ChangeExtension(hero.HeroPortrait.LeaderboardPortraitFileName?.ToLower(), StaticImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName))
                portrait.Add("loading", Path.ChangeExtension(hero.HeroPortrait.LoadingScreenPortraitFileName?.ToLower(), StaticImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName))
                portrait.Add("partyPanel", Path.ChangeExtension(hero.HeroPortrait.PartyPanelPortraitFileName?.ToLower(), StaticImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName))
                portrait.Add("target", Path.ChangeExtension(hero.HeroPortrait.TargetPortraitFileName?.ToLower(), StaticImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.DraftScreenFileName))
                portrait.Add("draftScreen", Path.ChangeExtension(hero.HeroPortrait.DraftScreenFileName?.ToLower(), StaticImageExtension));
            if (hero.HeroPortrait.PartyFrameFileName.Count > 0)
                portrait.Add("partyFrames", new JArray(hero.HeroPortrait.PartyFrameFileName.Select(x => Path.ChangeExtension(x.ToLower(), StaticImageExtension))));

            if (!string.IsNullOrEmpty(hero.UnitPortrait.MiniMapIconFileName))
                portrait.Add("minimap", Path.ChangeExtension(hero.UnitPortrait.MiniMapIconFileName?.ToLower(), StaticImageExtension));
            if (!string.IsNullOrEmpty(hero.UnitPortrait.TargetInfoPanelFileName))
                portrait.Add("targetInfo", Path.ChangeExtension(hero.UnitPortrait.TargetInfoPanelFileName?.ToLower(), StaticImageExtension));

            return new JProperty("portraits", portrait);
        }

        protected override JProperty GetUnitsObject(Hero hero)
        {
            return new JProperty(
                "heroUnits",
                new JArray(
                    from heroUnit in hero.HeroUnits
                    select new JObject(UnitElement(heroUnit))));
        }

        protected void SetAbilities(JObject abilityObject, IEnumerable<Ability> abilities, string propertyName)
        {
            if (abilities.Any())
            {
                abilityObject.Add(new JProperty(
                    propertyName,
                    new JArray(
                        from ability in abilities
                        orderby ability.AbilityTalentId.AbilityType ascending
                        select new JObject(AbilityInfoElement(ability)))));
            }
        }

        protected void SetTalents(JObject talentObject, IEnumerable<Talent> talents, string propertyName)
        {
            if (talents.Any())
            {
                talentObject.Add(new JProperty(
                    propertyName,
                    new JArray(
                        from talent in talents
                        orderby talent.Column ascending
                        select new JObject(TalentInfoElement(talent)))));
            }
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
