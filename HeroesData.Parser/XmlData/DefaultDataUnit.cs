using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataUnit
    {
        private readonly HashSet<string> UnitAttributeList = new HashSet<string>();

        public DefaultDataUnit(GameData gameData)
        {
            GameData = gameData;

            LoadCUnitDefault();
        }

        /// <summary>
        /// Gets the default unit name text. Contains ##id##. Use with CUnit id.
        /// </summary>
        public string? UnitName { get; private set; }

        /// <summary>
        /// Gets the default life amount.
        /// </summary>
        public double UnitLifeMax { get; private set; }

        /// <summary>
        /// Gets the default radius value.
        /// </summary>
        public double UnitRadius { get; private set; }

        /// <summary>
        /// Gets the default speed value.
        /// </summary>
        public double UnitSpeed { get; private set; }

        /// <summary>
        /// Gets the default sight value.
        /// </summary>
        public double UnitSight { get; private set; }

        /// <summary>
        /// Gets the default energy value.
        /// </summary>
        public double UnitEnergyMax { get; private set; }

        /// <summary>
        /// Gets the default energy regeneration rate.
        /// </summary>
        public double UnitEnergyRegenRate { get; private set; }

        /// <summary>
        /// Gets the default shield value.
        /// </summary>
        public double UnitShieldMax { get; private set; }

        /// <summary>
        /// Gets the default shield regeneration rate.
        /// </summary>
        public double UnitShieldRegenRate { get; private set; }

        /// <summary>
        /// Gets the default shield regeneration delay.
        /// </summary>
        public double UnitShieldRegenDelay { get; private set; }

        /// <summary>
        /// Gets a collection of the default attributes.
        /// </summary>
        public IEnumerable<string> UnitAttributes => UnitAttributeList;

        /// <summary>
        /// Gets the default damage type.
        /// </summary>
        public string? UnitDamageType { get; private set; }

        protected GameData GameData { get; }

        // <CUnit default="1">
        protected void LoadCUnitDefault()
        {
            CUnitElement(GameData.Elements("CUnit").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        protected void CUnitElement(IEnumerable<XElement> elements)
        {
            foreach (XElement element in elements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    UnitName = element.Attribute("value").Value;
                }
                else if (elementName == "LIFEMAX")
                {
                    UnitLifeMax = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "RADIUS")
                {
                    UnitRadius = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "SPEED")
                {
                    UnitSpeed = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "SIGHT")
                {
                    UnitSight = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ENERGYMAX")
                {
                    UnitEnergyMax = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ENERGYREGENRATE")
                {
                    UnitEnergyRegenRate = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ATTRIBUTES")
                {
                    if (element.Attribute("value")?.Value == "1")
                        UnitAttributeList.Add(element.Attribute("index").Value);
                    else if (element.Attribute("value")?.Value == "0")
                        UnitAttributeList.Remove(element.Attribute("index").Value);
                }
                else if (elementName == "UNITDAMAGETYPE")
                {
                    UnitDamageType = element.Attribute("value").Value;
                }
                else if (elementName == "SHIELDSMAX")
                {
                    UnitShieldMax = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "SHIELDREGENRATE")
                {
                    UnitShieldRegenRate = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "SHIELDREGENDELAY")
                {
                    UnitShieldRegenDelay = double.Parse(element.Attribute("value").Value);
                }
            }
        }
    }
}
