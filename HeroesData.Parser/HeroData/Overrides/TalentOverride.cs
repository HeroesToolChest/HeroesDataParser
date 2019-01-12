using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.HeroData.Overrides
{
    public class TalentOverride : PropertyOverrideBase<Talent>
    {
        public TalentOverride(GameData gameData)
            : base(gameData)
        {
        }

        public TalentOverride(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
        {
        }

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
                        talent.AbilityTalentLinkIds.Remove(propertyValue.Substring(1, propertyValue.Length));
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
