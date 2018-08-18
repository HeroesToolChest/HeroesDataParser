using Heroes.Models;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.UnitData.Overrides
{
    public class PortraitOverride : PropertyOverrideBase<HeroPortrait>
    {
        public PortraitOverride(GameData gameData)
            : base(gameData)
        {
        }

        public PortraitOverride(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
        {
        }

        protected override void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<HeroPortrait>> propertyOverrides)
        {
            propertyOverrides.Add(propertyName, (portrait) =>
            {
                portrait.GetType().GetProperty(propertyName).SetValue(portrait, propertyValue);
            });
        }
    }
}
