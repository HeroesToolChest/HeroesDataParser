using Heroes.Models.AbilityTalents;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.UnitData.Overrides
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
            if (propertyName == "AbilityType")
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    if (Enum.TryParse(propertyValue, out AbilityType abilityType))
                        talent.AbilityType = abilityType;
                    else
                        talent.AbilityType = AbilityType.Q;
                });
            }
        }
    }
}
