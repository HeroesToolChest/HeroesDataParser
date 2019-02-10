using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.PropertyOverrides
{
    internal class PortraitPropertyOverride : PropertyOverrideBase<HeroPortrait>
    {
        public PortraitPropertyOverride(GameData gameData)
            : base(gameData)
        {
        }

        public PortraitPropertyOverride(GameData gameData, int? hotsBuild)
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
