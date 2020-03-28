﻿using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.PropertyOverrides
{
    internal class TalentPropertyOverride : PropertyOverrideBase<Talent, string>
    {
        protected override void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<Talent>> propertyOverrides)
        {
            if (propertyName == nameof(Talent.AbilityTalentId.AbilityType))
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    if (Enum.TryParse(propertyValue, out AbilityType abilityType))
                        talent.AbilityTalentId.AbilityType = abilityType;
                    else
                        talent.AbilityTalentId.AbilityType = AbilityType.Unknown;
                });
            }
            else if (propertyName == nameof(Talent.AbilityTalentLinkIds))
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    if (propertyValue.StartsWith('-'))
                        talent.RemoveAbilityTalentLinkId(propertyValue.Substring(1));
                    else
                        talent.AddAbilityTalentLinkId(propertyValue);
                });
            }
            else if (propertyName == nameof(Talent.IsActive))
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    if (bool.TryParse(propertyValue, out bool result))
                        talent.IsActive = result;
                    else
                        talent.IsActive = false;
                });
            }
        }
    }
}
