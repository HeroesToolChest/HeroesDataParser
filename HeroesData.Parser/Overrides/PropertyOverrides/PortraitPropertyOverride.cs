using Heroes.Models;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.PropertyOverrides
{
    internal class PortraitPropertyOverride : PropertyOverrideBase<HeroPortrait>
    {
        protected override void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<HeroPortrait>> propertyOverrides)
        {
            propertyOverrides.Add(propertyName, (portrait) =>
            {
                portrait.GetType().GetProperty(propertyName).SetValue(portrait, propertyValue);
            });
        }
    }
}
