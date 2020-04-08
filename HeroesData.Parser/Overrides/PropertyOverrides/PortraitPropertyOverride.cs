using Heroes.Models;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.PropertyOverrides
{
    internal class PortraitPropertyOverride : PropertyOverrideBase<HeroPortrait, string>
    {
        protected override void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<HeroPortrait>> propertyOverrides)
        {
            if (propertyName.StartsWith("PartyFrameFileNameArray", StringComparison.OrdinalIgnoreCase))
            {
                propertyOverrides.Add(propertyName, (portrait) =>
                {
                    if (propertyValue.StartsWith('-'))
                        portrait.PartyFrameFileName.Remove(propertyValue.Substring(1));
                    else
                        portrait.PartyFrameFileName.Add(propertyValue);
                });
            }
            else
            {
                propertyOverrides.Add(propertyName, (portrait) =>
                {
                    portrait.GetType().GetProperty(propertyName)?.SetValue(portrait, propertyValue);
                });
            }
        }
    }
}
