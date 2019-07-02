using Heroes.Models;
using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.PropertyOverrides
{
    internal class AbilityPropertyOverride : PropertyOverrideBase<Ability, AbilityTalentId>
    {
        protected override void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<Ability>> propertyOverrides)
        {
            if (propertyName == nameof(Ability.ParentLink))
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    if (!string.IsNullOrEmpty(propertyValue))
                    {
                        string[] split = propertyValue.Split(',', 2);
                        if (split.Length == 2)
                            ability.ParentLink = new AbilityTalentId(split[0], split[1]);
                        else
                            ability.ParentLink = new AbilityTalentId(propertyValue, propertyValue);
                    }
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
            else if (propertyName == "CooldownTooltip")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    ability.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(propertyValue);
                });
            }
        }
    }
}
