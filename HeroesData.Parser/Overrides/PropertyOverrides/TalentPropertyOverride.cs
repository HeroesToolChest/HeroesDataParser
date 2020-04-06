using Heroes.Models.AbilityTalents;
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
                    if (Enum.TryParse(propertyValue, out AbilityTypes abilityTypes))
                        talent.AbilityTalentId.AbilityType = abilityTypes;
                    else
                        talent.AbilityTalentId.AbilityType = AbilityTypes.Unknown;
                });
            }
            else if (propertyName == nameof(Talent.AbilityTalentLinkIds))
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    if (propertyValue.StartsWith('-'))
                        talent.AbilityTalentLinkIds.Remove(propertyValue.Substring(1));
                    else
                        talent.AbilityTalentLinkIds.Add(propertyValue);
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
