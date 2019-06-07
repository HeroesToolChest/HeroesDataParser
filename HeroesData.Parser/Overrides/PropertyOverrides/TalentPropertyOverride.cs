using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.PropertyOverrides
{
    internal class TalentPropertyOverride : PropertyOverrideBase<Talent>
    {
        protected override void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<Talent>> propertyOverrides)
        {
            if (propertyName == nameof(Talent.AbilityType))
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    if (Enum.TryParse(propertyValue, out AbilityType abilityType))
                        talent.AbilityType = abilityType;
                    else
                        talent.AbilityType = AbilityType.Q;
                });
            }
            else if (propertyName == nameof(Talent.AbilityTalentLinkIds))
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    if (propertyName.StartsWith("-"))
                        talent.RemoveAbilityTalentLinkId(propertyValue.Substring(1, propertyValue.Length));
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
