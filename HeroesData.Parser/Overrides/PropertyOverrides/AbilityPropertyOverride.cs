using Heroes.Models;
using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.PropertyOverrides
{
    internal class AbilityPropertyOverride : PropertyOverrideBase<Ability, string>
    {
        protected override void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<Ability>> propertyOverrides)
        {
            if (propertyName == nameof(Ability.ParentLink))
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    if (!string.IsNullOrEmpty(propertyValue))
                    {
                        string[] split = propertyValue.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        if (split.Length == 1)
                        {
                            ability.ParentLink = new AbilityTalentId(split[0], split[0]);
                        }
                        else if (split.Length == 2)
                        {
                            ability.ParentLink = new AbilityTalentId(split[0], split[1]);
                        }
                        else if (split.Length == 3)
                        {
                            ability.ParentLink = new AbilityTalentId(split[0], split[1])
                            {
                                AbilityType = Enum.Parse<AbilityType>(split[2]),
                            };
                        }
                        else if (split.Length == 4)
                        {
                            ability.ParentLink = new AbilityTalentId(split[0], split[1])
                            {
                                AbilityType = Enum.Parse<AbilityType>(split[2]),
                                IsPassive = bool.Parse(split[3]),
                            };
                        }
                    }
                    else
                    {
                        ability.ParentLink = null;
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
                        ability.AbilityTalentId.AbilityType = abilityType;
                    else
                        ability.AbilityTalentId.AbilityType = AbilityType.Q;
                });
            }
            else if (propertyName == "CooldownTooltip")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    ability.Tooltip.Cooldown.CooldownTooltip = new TooltipDescription(propertyValue);
                });
            }
            else if (propertyName == nameof(Ability.AbilityTalentId.IsPassive))
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    if (bool.TryParse(propertyValue, out bool result))
                        ability.AbilityTalentId.IsPassive = result;
                    else
                        ability.AbilityTalentId.IsPassive = false;
                });
            }
            else if (propertyName == nameof(Ability.IsActive))
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    if (bool.TryParse(propertyValue, out bool result))
                        ability.IsActive = result;
                    else
                        ability.IsActive = false;
                });
            }
        }
    }
}
