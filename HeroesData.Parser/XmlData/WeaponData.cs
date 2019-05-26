using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class WeaponData
    {
        private readonly GameData GameData;
        private readonly DefaultData DefaultData;
        private readonly UnitDataOverride UnitDataOverride;
        private readonly Configuration Configuration;

        public WeaponData(GameData gameData, DefaultData defaultData, Configuration configuration)
        {
            GameData = gameData;
            DefaultData = defaultData;
            Configuration = configuration;
        }

        public WeaponData(GameData gameData, DefaultData defaultData, UnitDataOverride unitDataOverride, Configuration configuration)
        {
            GameData = gameData;
            DefaultData = defaultData;
            UnitDataOverride = unitDataOverride;
            Configuration = configuration;
        }

        public UnitWeapon CreateWeapon(XElement weaponArrayElement)
        {
            string weaponLink = weaponArrayElement.Attribute("Link")?.Value;
            if (string.IsNullOrEmpty(weaponLink))
                return null;

            UnitWeapon weapon = new UnitWeapon()
            {
                WeaponNameId = weaponLink,
                Name = GameData.GetGameString(DefaultData.WeaponData.WeaponName.Replace(DefaultData.IdPlaceHolder, weaponLink)),
                Period = DefaultData.WeaponData.WeaponPeriod,
                Range = DefaultData.WeaponData.WeaponRange,
            };

            IEnumerable<XElement> weaponElements = GetWeaponElements(weaponLink);
            if (!weaponElements.Any())
                return null;

            foreach (XElement element in weaponElements)
            {
                SetWeaponData(element, weapon);

                if (string.IsNullOrEmpty(weapon.WeaponNameId))
                    return null;
            }

            return weapon;
        }

        private IEnumerable<XElement> GetWeaponElements(string weaponElementId)
        {
            if (string.IsNullOrEmpty(weaponElementId))
                return new List<XElement>();

            return GameData.ElementsIncluded(Configuration.GamestringXmlElements("Weapon").ToArray(), weaponElementId);
        }

        ///// <summary>
        ///// Add the hero weapon data.
        ///// </summary>
        ///// <param name="hero"></param>
        ///// <param name="weaponElements"></param>
        //public void AddHeroWeapons(Hero hero, IEnumerable<XElement> weaponElements)
        //{
        //    if (weaponElements == null)
        //        return;

        //    AddWeapons(weaponElements, (isValidWeapon, weaponsIds, weaponNameId) =>
        //    {
        //        if (isValidWeapon || weaponsIds.Count == 1 || weaponNameId.Contains("HeroWeapon") || weaponNameId == hero.CUnitId || weaponNameId == $"{hero.CHeroId}Weapon" || weaponNameId == $"{hero.CHeroId}WeaponMelee")
        //        {
        //            AddWeapons(hero, weaponNameId);
        //        }
        //    });
        //}

        ///// <summary>
        ///// Add the unit weapon data.
        ///// </summary>
        ///// <param name="hero"></param>
        ///// <param name="weaponElements"></param>
        //public void AddUnitWeapons(Unit unit, IEnumerable<XElement> weaponElements)
        //{
        //    if (weaponElements == null)
        //        return;

        //    AddWeapons(weaponElements, (isValidWeapon, weaponsIds, weaponNameId) =>
        //    {
        //        if (isValidWeapon || weaponsIds.Count > 0)
        //        {
        //            AddWeapons(unit, weaponNameId);
        //        }
        //    });
        //}

        //private void AddWeapons(IEnumerable<XElement> weaponElements, Action<bool, HashSet<string>, string> addWeapons)
        //{
        //    HashSet<string> weaponsIds = new HashSet<string>();
        //    foreach (XElement weaponElement in weaponElements)
        //    {
        //        string weaponNameId = weaponElement.Attribute("Link")?.Value;
        //        if (!string.IsNullOrEmpty(weaponNameId))
        //            weaponsIds.Add(weaponNameId);
        //    }

        //    foreach (string weaponNameId in weaponsIds)
        //    {
        //        if (UnitDataOverride != null && UnitDataOverride.IsValidWeaponByWeaponId.TryGetValue(weaponNameId, out bool isValidWeapon))
        //        {
        //            if (!isValidWeapon)
        //                continue;
        //        }
        //        else
        //        {
        //            isValidWeapon = false;
        //        }

        //        addWeapons(isValidWeapon, weaponsIds, weaponNameId);
        //    }
        //}

        //[Obsolete("Dont use")]
        //private void AddWeapons(Unit unit, string weaponNameId)
        //{
        //    // defaults
        //    UnitWeapon weapon = new UnitWeapon
        //    {
        //        WeaponNameId = weaponNameId,
        //        Name = GameData.GetGameString(DefaultData.WeaponData.WeaponName.Replace(DefaultData.IdPlaceHolder, weaponNameId)),
        //        Period = DefaultData.WeaponData.WeaponPeriod,
        //        Range = DefaultData.WeaponData.WeaponRange,
        //    };

        //    string displayEffectElementValue = DefaultData.WeaponData.WeaponDisplayEffect.Replace(DefaultData.IdPlaceHolder, weapon.WeaponNameId);
        //    if (!string.IsNullOrEmpty(displayEffectElementValue))
        //    {
        //        XElement effectDamageElement = GameData.MergeXmlElements(GameData.Elements("CEffectDamage").Where(x => x.Attribute("id")?.Value == displayEffectElementValue));
        //        if (effectDamageElement != null)
        //            WeaponAddEffectDamage(effectDamageElement, weapon);
        //    }

        //    XElement weaponLegacy = GameData.MergeXmlElements(GameData.Elements("CWeaponLegacy").Where(x => x.Attribute("id")?.Value == weaponNameId));
        //    if (weaponLegacy != null)
        //        weapon = SetWeaponData(weaponLegacy, weapon);

        //    unit.Weapons.Add(weapon);
        //}

        private void SetWeaponData(XElement weaponElement, UnitWeapon weapon)
        {
            // parent lookup
            string parentValue = weaponElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements(weaponElement.Name.LocalName).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetWeaponData(parentElement, weapon);
            }

            if (weapon == null)
                return;

            // loop through all elements and set found elements
            foreach (XElement element in weaponElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "RANGE")
                {
                    weapon.Range = XmlParse.GetDoubleValue(weapon.WeaponNameId, element, GameData);
                }
                else if (elementName == "PERIOD")
                {
                    weapon.Period = XmlParse.GetDoubleValue(weapon.WeaponNameId, element, GameData);
                }
                else if (elementName == "DISPLAYEFFECT")
                {
                    string displayEffectElementValue = element.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(displayEffectElementValue))
                    {
                        XElement effectDamageElement = GameData.MergeXmlElements(GameData.Elements("CEffectDamage").Where(x => x.Attribute("id")?.Value == displayEffectElementValue));
                        if (effectDamageElement != null)
                            WeaponAddEffectDamage(effectDamageElement, weapon);
                    }
                }
                else if (elementName == "OPTIONS")
                {
                    string indexValue = element.Attribute("index")?.Value;
                    string value = element.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(indexValue) && !string.IsNullOrEmpty(value) && indexValue.Equals("disabled", StringComparison.OrdinalIgnoreCase) && value == "1")
                    {
                        weapon.WeaponNameId = string.Empty;
                        return;
                    }
                }
            }
        }

        private void WeaponAddEffectDamage(XElement effectDamageElement, UnitWeapon weapon)
        {
            // parent lookup
            string parentValue = effectDamageElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements("CEffectDamage").Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    WeaponAddEffectDamage(parentElement, weapon);
            }

            foreach (XElement element in effectDamageElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "AMOUNT")
                {
                    weapon.Damage = XmlParse.GetDoubleValue(weapon.WeaponNameId, element, GameData);
                }
                else if (elementName == "ATTRIBUTEFACTOR")
                {
                    string index = element.Attribute("index")?.Value;
                    string value = element.Attribute("value")?.Value;

                    WeaponAttributeFactor attributeFactor = new WeaponAttributeFactor();

                    if (!string.IsNullOrEmpty(index) && !string.IsNullOrEmpty(value))
                    {
                        attributeFactor.Type = index;
                        attributeFactor.Value = XmlParse.GetDoubleValue(weapon.WeaponNameId, element, GameData);

                        weapon.AddAttributeFactor(attributeFactor);
                    }
                }
            }

            double? scaleValue = GameData.GetScaleValue(("Effect", effectDamageElement.Attribute("id")?.Value, "Amount"));
            if (scaleValue.HasValue)
                weapon.DamageScaling = scaleValue.Value;
        }
    }
}
