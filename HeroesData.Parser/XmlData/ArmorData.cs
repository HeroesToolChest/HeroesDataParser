using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class ArmorData
    {
        private readonly GameData GameData;

        public ArmorData(GameData gameData)
        {
            GameData = gameData;
        }

        public IEnumerable<UnitArmor> CreateArmorCollection(XElement armorLinkElement)
        {
            string armorLink = armorLinkElement.Attribute("value")?.Value;
            if (string.IsNullOrEmpty(armorLink))
                return null;

            XElement armorElement = GameData.MergeXmlElements(GameData.Elements("CArmor").Where(x => x.Attribute("id")?.Value == armorLink));
            if (armorElement == null)
                return null;

            HashSet<UnitArmor> armorList = new HashSet<UnitArmor>();

            foreach (XElement armorSetElement in armorElement.Elements())
            {
                string index = armorSetElement.Attribute("index")?.Value;
                if (string.IsNullOrEmpty(index))
                    continue;

                UnitArmor unitArmor = new UnitArmor
                {
                    Type = index,
                };

                foreach (XElement armorMitigationTableElement in armorSetElement.Elements("ArmorMitigationTable"))
                {
                    string type = armorMitigationTableElement.Attribute("index")?.Value;
                    string value = armorMitigationTableElement.Attribute("value")?.Value;

                    if (type.Equals("basic", StringComparison.OrdinalIgnoreCase) && int.TryParse(value, out int valueInt))
                        unitArmor.BasicArmor = valueInt;
                    else if (type.Equals("ability", StringComparison.OrdinalIgnoreCase) && int.TryParse(value, out valueInt))
                        unitArmor.AbilityArmor = valueInt;
                    else if (type.Equals("splash", StringComparison.OrdinalIgnoreCase) && int.TryParse(value, out valueInt))
                        unitArmor.SplashArmor = valueInt;
                }



                if (unitArmor.BasicArmor > 0 || unitArmor.AbilityArmor > 0 || unitArmor.SplashArmor > 0)
                {
                    if (armorList.Contains(unitArmor))
                        armorList.Remove(unitArmor);

                    armorList.Add(unitArmor);
                }
            }

            return armorList;
        }

        /// <summary>
        /// Sets the unit armor data.
        /// </summary>
        /// <param name="unit"></param>
        public void SetUnitArmorData(Unit unit, XElement armorLink)
        {
            string armorLinkValue = armorLink?.Attribute("value")?.Value;

            if (string.IsNullOrEmpty(armorLinkValue))
                return;

            HashSet<UnitArmor> armorList = new HashSet<UnitArmor>();

            XElement armorElement = GameData.MergeXmlElements(GameData.Elements("CArmor").Where(x => x.Attribute("id")?.Value == armorLinkValue));
            if (armorElement != null)
            {
                foreach (XElement armorSetElement in armorElement.Elements())
                {
                    string index = armorSetElement.Attribute("index")?.Value;
                    if (string.IsNullOrEmpty(index))
                        continue;

                    UnitArmor unitArmor = new UnitArmor();

                    foreach (XElement armorMitigationTableElement in armorSetElement.Elements("ArmorMitigationTable"))
                    {
                        string type = armorMitigationTableElement.Attribute("index")?.Value;
                        string value = armorMitigationTableElement.Attribute("value")?.Value;

                        if (type.Equals("basic", StringComparison.OrdinalIgnoreCase) && int.TryParse(value, out int valueInt))
                            unitArmor.BasicArmor = valueInt;
                        else if (type.Equals("ability", StringComparison.OrdinalIgnoreCase) && int.TryParse(value, out valueInt))
                            unitArmor.AbilityArmor = valueInt;
                        else if (type.Equals("splash", StringComparison.OrdinalIgnoreCase) && int.TryParse(value, out valueInt))
                            unitArmor.SplashArmor = valueInt;
                    }

                    unitArmor.Type = index;

                    if (unitArmor.BasicArmor > 0 || unitArmor.AbilityArmor > 0 || unitArmor.SplashArmor > 0)
                    {
                        if (armorList.Contains(unitArmor))
                            armorList.Remove(unitArmor);

                        armorList.Add(unitArmor);
                    }
                }

                //unit.Armor = armorList;
            }
        }
    }
}
