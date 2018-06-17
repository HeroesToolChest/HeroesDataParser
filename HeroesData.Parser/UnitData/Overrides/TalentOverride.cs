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
            if (propertyName == "Custom")
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    talent.Tooltip.Custom = propertyValue;
                });
            }
            else if (propertyName == "Tooltip.Energy.EnergyCost")
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    talent.Tooltip.Energy.EnergyCost = (int)GetValue(propertyValue);
                });
            }
            else if (propertyName == "Tooltip.Energy.IsPerCost")
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    if (bool.TryParse(propertyValue, out bool value))
                        talent.Tooltip.Energy.IsPerCost = value;
                    else
                        talent.Tooltip.Energy.IsPerCost = false;
                });
            }
            else if (propertyName == "Tooltip.Cooldown.CooldownValue")
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    talent.Tooltip.Cooldown.CooldownValue = (int)GetValue(propertyValue);
                });
            }
            else if (propertyName == "Tooltip.Life.LifeCost")
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    talent.Tooltip.Life.LifeCost = (int)GetValue(propertyValue);
                });
            }
            else if (propertyName == "Tooltip.Life.IsLifePercentage")
            {
                propertyOverrides.Add(propertyName, (talent) =>
                {
                    if (bool.TryParse(propertyValue, out bool value))
                        talent.Tooltip.Life.IsLifePercentage = value;
                    else
                        talent.Tooltip.Life.IsLifePercentage = false;
                });
            }
        }
    }
}
