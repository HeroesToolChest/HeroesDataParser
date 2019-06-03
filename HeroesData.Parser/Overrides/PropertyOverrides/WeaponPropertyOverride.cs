using Heroes.Models;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.PropertyOverrides
{
    internal class WeaponPropertyOverride : PropertyOverrideBase<UnitWeapon>
    {
        protected override void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<UnitWeapon>> propertyOverrides)
        {
            if (propertyName == nameof(UnitWeapon.ParentLink))
            {
                propertyOverrides.Add(propertyName, (weapon) =>
                {
                    weapon.ParentLink = propertyValue;
                });
            }

            if (propertyName == nameof(UnitWeapon.Range))
            {
                propertyOverrides.Add(propertyName, (weapon) =>
                {
                    if (string.IsNullOrEmpty(propertyValue))
                        return;

                    weapon.Range = GetDoubleValue(propertyValue);
                });
            }
            else if (propertyName == nameof(UnitWeapon.Damage))
            {
                propertyOverrides.Add(propertyName, (weapon) =>
                {
                    if (string.IsNullOrEmpty(propertyValue))
                        return;

                    weapon.Damage = GetDoubleValue(propertyValue);
                });
            }
        }
    }
}
