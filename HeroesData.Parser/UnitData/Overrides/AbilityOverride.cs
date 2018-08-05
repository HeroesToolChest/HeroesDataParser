using Heroes.Models.AbilityTalents;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.UnitData.Overrides
{
    public class AbilityOverride : PropertyOverrideBase<Ability>
    {
        public AbilityOverride(GameData gameData)
            : base(gameData)
        {
        }

        public AbilityOverride(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
        {
        }

        protected override void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<Ability>> propertyOverrides)
        {
            if (string.IsNullOrEmpty(propertyValue))
                return;

            if (propertyName == nameof(Ability.ParentLink))
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    ability.ParentLink = propertyValue;
                });
            }
            else if (propertyName == "AbilityTier")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    if (Enum.TryParse(propertyValue, out AbilityTier abilityTier))
                        ability.Tier = abilityTier;
                    else
                        ability.Tier = AbilityTier.Basic;
                });
            }
            else if (propertyName == "AbilityType")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    if (Enum.TryParse(propertyValue, out AbilityType abilityType))
                        ability.AbilityType = abilityType;
                    else
                        ability.AbilityType = AbilityType.Q;
                });
            }
            else if (propertyName == "Tooltip.Custom")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    ability.Tooltip.Custom = propertyValue;
                });
            }
            else if (propertyName == "Tooltip.Energy.EnergyCost")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    ability.Tooltip.Energy.EnergyCost = (int)GetValue(propertyValue);
                });
            }
            else if (propertyName == "Tooltip.Energy.IsPerCost")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    if (bool.TryParse(propertyValue, out bool value))
                        ability.Tooltip.Energy.IsPerCost = value;
                    else
                        ability.Tooltip.Energy.IsPerCost = false;
                });
            }
            else if (propertyName == "Tooltip.Cooldown.CooldownValue")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    ability.Tooltip.Cooldown.CooldownValue = (int)GetValue(propertyValue);
                });
            }
            else if (propertyName == "Tooltip.Life.LifeCost")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    ability.Tooltip.Life.LifeCost = (int)GetValue(propertyValue);
                });
            }
            else if (propertyName == "Tooltip.Life.IsLifePercentage")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    if (bool.TryParse(propertyValue, out bool value))
                        ability.Tooltip.Life.IsLifePercentage = value;
                    else
                        ability.Tooltip.Life.IsLifePercentage = false;
                });
            }
        }
    }
}
