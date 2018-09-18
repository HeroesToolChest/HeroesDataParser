using Heroes.Models;
using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Data
{
    public class WeaponData
    {
        private readonly double DefaultWeaponPeriod = 1.2;

        private readonly GameData GameData;
        private readonly HeroOverride HeroOverride;

        public WeaponData(GameData gameData, HeroOverride heroOverride)
        {
            GameData = gameData;
            HeroOverride = heroOverride;
        }

        /// <summary>
        /// Sets the hero weapon data.
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="weaponElements"></param>
        public void SetHeroWeaponData(Hero hero, IEnumerable<XElement> weaponElements)
        {
            List<string> weaponsIds = new List<string>();
            foreach (XElement weaponElement in weaponElements)
            {
                string weaponNameId = weaponElement.Attribute("Link")?.Value;
                if (!string.IsNullOrEmpty(weaponNameId))
                    weaponsIds.Add(weaponNameId);
            }

            foreach (string weaponNameId in weaponsIds)
            {
                if (HeroOverride.IsValidWeaponByWeaponId.TryGetValue(weaponNameId, out bool validWeapon))
                {
                    if (!validWeapon)
                        continue;
                }
                else
                {
                    validWeapon = false;
                }

                if (validWeapon || weaponsIds.Count == 1 || weaponNameId.Contains("HeroWeapon") || weaponNameId == hero.CUnitId || weaponNameId == $"{hero.CHeroId}Weapon" || weaponNameId == $"{hero.CHeroId}WeaponMelee")
                {
                    UnitWeapon weapon = GetWeapon(weaponNameId, weaponsIds);

                    if (weapon != null)
                        hero.Weapons.Add(weapon);
                }
            }
        }

        private UnitWeapon GetWeapon(string weaponNameId, List<string> allWeaponIds)
        {
            UnitWeapon weapon = null;

            if (!string.IsNullOrEmpty(weaponNameId))
            {
                XElement weaponLegacy = GameData.XmlGameData.Root.Elements("CWeaponLegacy").FirstOrDefault(x => x.Attribute("id")?.Value == weaponNameId);

                if (weaponLegacy != null)
                {
                    weapon = new UnitWeapon
                    {
                        WeaponNameId = weaponNameId,
                    };

                    WeaponAddRange(weaponLegacy, weapon, weaponNameId);
                    WeaponAddPeriod(weaponLegacy, weapon, weaponNameId);
                    WeaponAddDamage(weaponLegacy, weapon, weaponNameId);
                }
            }

            return weapon;
        }

        private void WeaponAddRange(XElement weaponLegacy, UnitWeapon weapon, string weaponNameId)
        {
            XElement rangeElement = weaponLegacy.Element("Range");
            string parentWeaponId = weaponLegacy.Attribute("parent")?.Value;

            if (rangeElement != null)
            {
                weapon.Range = double.Parse(rangeElement.Attribute("value").Value);
            }
            else if (!string.IsNullOrEmpty(parentWeaponId))
            {
                XElement parentWeaponLegacy = GameData.XmlGameData.Root.Elements("CWeaponLegacy").FirstOrDefault(x => x.Attribute("id")?.Value == parentWeaponId);
                if (parentWeaponLegacy != null)
                    WeaponAddRange(parentWeaponLegacy, weapon, parentWeaponId);
            }
        }

        private void WeaponAddPeriod(XElement weaponLegacy, UnitWeapon weapon, string weaponNameId)
        {
            XElement periodElement = weaponLegacy.Element("Period");
            string parentWeaponId = weaponLegacy.Attribute("parent")?.Value;

            if (periodElement != null)
            {
                weapon.Period = double.Parse(periodElement.Attribute("value").Value);
            }
            else if (!string.IsNullOrEmpty(parentWeaponId))
            {
                XElement parentWeaponLegacy = GameData.XmlGameData.Root.Elements("CWeaponLegacy").FirstOrDefault(x => x.Attribute("id")?.Value == parentWeaponId);
                if (parentWeaponLegacy != null)
                    WeaponAddPeriod(parentWeaponLegacy, weapon, parentWeaponId);
            }
            else
            {
                weapon.Period = DefaultWeaponPeriod;
            }
        }

        private void WeaponAddDamage(XElement weaponLegacy, UnitWeapon weapon, string weaponNameId)
        {
            XElement displayEffectElement = weaponLegacy.Element("DisplayEffect");
            string parentWeaponId = weaponLegacy.Attribute("parent")?.Value;

            if (displayEffectElement != null)
            {
                string displayEffectValue = displayEffectElement.Attribute("value").Value;
                XElement effectDamageElement = GameData.XmlGameData.Root.Elements("CEffectDamage").FirstOrDefault(x => x.Attribute("id")?.Value == displayEffectValue);
                if (effectDamageElement != null)
                {
                    XElement amountElement = effectDamageElement.Element("Amount");
                    if (amountElement != null)
                    {
                        weapon.Damage = double.Parse(amountElement.Attribute("value").Value);
                    }
                }

                double? scaleValue = GameData.GetScaleValue(("Effect", displayEffectValue, "Amount"));
                if (scaleValue.HasValue)
                    weapon.DamageScaling = scaleValue.Value;
            }
            else if (!string.IsNullOrEmpty(parentWeaponId))
            {
                XElement parentWeaponLegacy = GameData.XmlGameData.Root.Elements("CWeaponLegacy").FirstOrDefault(x => x.Attribute("id")?.Value == parentWeaponId);
                if (parentWeaponLegacy != null)
                    WeaponAddDamage(parentWeaponLegacy, weapon, parentWeaponId);
            }
        }
    }
}
