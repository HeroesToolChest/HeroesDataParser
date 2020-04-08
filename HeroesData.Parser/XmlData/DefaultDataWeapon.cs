using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataWeapon
    {
        private readonly GameData _gameData;

        public DefaultDataWeapon(GameData gameData)
        {
            _gameData = gameData;

            LoadCWeaponDefault();
        }

        /// <summary>
        /// Gets the defualt weapon name text. Contains ##id##.
        /// </summary>
        public string? WeaponName { get; private set; }

        /// <summary>
        /// Gets the default weapon range value.
        /// </summary>
        public double WeaponRange { get; private set; }

        /// <summary>
        /// Gets the default weapon period value.
        /// </summary>
        public double WeaponPeriod { get; private set; }

        /// <summary>
        /// Gets the default weapon display effect name. Contains ##id##.
        /// </summary>
        public string? WeaponDisplayEffect { get; private set; }

        // <CWeapon default="1">
        private void LoadCWeaponDefault()
        {
            CWeaponElement(_gameData.Elements("CWeapon").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CWeaponElement(IEnumerable<XElement> cWeaponElements)
        {
            foreach (XElement element in cWeaponElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "NAME")
                {
                    WeaponName = element.Attribute("value").Value;
                }
                else if (elementName == "DISPLAYEFFECT")
                {
                    WeaponDisplayEffect = element.Attribute("value").Value;
                }
                else if (elementName == "RANGE")
                {
                    WeaponRange = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "PERIOD")
                {
                    WeaponPeriod = double.Parse(element.Attribute("value").Value);
                }
            }
        }
    }
}
