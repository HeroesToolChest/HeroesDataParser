using Heroes.Icons.Parser.Models;
using Heroes.Icons.Parser.XmlGameData;
using System;
using System.Collections.Generic;

namespace Heroes.Icons.Parser.UnitData.Overrides
{
    public class WeaponOverride : PropertyOverrideBase<HeroWeapon>
    {
        public WeaponOverride(GameData gameData)
            : base(gameData)
        {
        }

        protected override void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<HeroWeapon>> propertyOverrides)
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
