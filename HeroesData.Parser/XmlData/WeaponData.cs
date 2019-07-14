using Heroes.Models;
using HeroesData.Loader.XmlGameData;
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
        private readonly Configuration Configuration;

        public WeaponData(GameData gameData, DefaultData defaultData, Configuration configuration)
        {
            GameData = gameData;
            DefaultData = defaultData;
            Configuration = configuration;
        }

        /// <summary>
        /// Gets or sets if the parsing is for hero units.
        /// </summary>
        public bool IsHeroParsing { get; set; } = false;

        public UnitWeapon CreateWeapon(string weaponLink)
        {
            if (string.IsNullOrEmpty(weaponLink))
            {
                throw new ArgumentException("Argument cannot be null or emtpy", nameof(weaponLink));
            }

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

            return GameData.ElementsIncluded(Configuration.GamestringXmlElements("Weapon"), weaponElementId);
        }

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
                    if (IsHeroParsing && !string.IsNullOrEmpty(indexValue) && !string.IsNullOrEmpty(value) && indexValue.Equals("disabled", StringComparison.OrdinalIgnoreCase) && value == "1")
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
