using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.UnitData.Overrides
{
    public class WeaponOverride : PropertyOverrideBase<UnitWeapon>
    {
        public WeaponOverride(GameData gameData)
            : base(gameData)
        {
        }

        public WeaponOverride(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
        {
        }

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

                    weapon.Range = GetValue(propertyValue);
                });
            }
            else if (propertyName == nameof(UnitWeapon.Damage))
            {
                propertyOverrides.Add(propertyName, (weapon) =>
                {
                    if (string.IsNullOrEmpty(propertyValue))
                        return;

                    weapon.Damage = GetValue(propertyValue);
                });
            }
        }
    }
}
