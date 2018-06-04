using HeroesData.Parser.Models;
using HeroesData.Parser.XmlGameData;
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
            if (propertyName == "Range")
            {
                propertyOverrides.Add(propertyName, (weapon) =>
                {
                    weapon.Range = double.Parse(propertyValue);
                });
            }
            else if (propertyName == "Damage")
            {
                propertyOverrides.Add(propertyName, (weapon) =>
                {
                    weapon.Damage = GetValue(propertyValue);
                });
            }
        }
    }
}
