using Heroes.Models;
using HeroesData.Loader.XmlGameData;
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

        /// <summary>
        /// Sets the unit armor data.
        /// </summary>
        /// <param name="unit"></param>
        public void SetUnitArmorData(Unit unit, XElement armorLink)
        {
            string armorLinkValue = armorLink?.Attribute("value")?.Value;

            if (string.IsNullOrEmpty(armorLinkValue))
                return;

            XElement armorElement = GameData.MergeXmlElements(GameData.Elements("CArmor").Where(x => x.Attribute("id")?.Value == armorLinkValue));
            XElement physicalArmorElement = GameData.MergeXmlElements(GameData.Elements("CArmor").Where(x => x.Attribute("id")?.Value == armorLinkValue));
            XElement spellArmorElement = GameData.MergeXmlElements(GameData.Elements("CArmor").Where(x => x.Attribute("id")?.Value == armorLinkValue));

            if (armorElement != null)
            {
                UnitArmorAddValue(armorElement, unit);
            }

            if (physicalArmorElement != null)
            {
                UnitArmorAddValue(physicalArmorElement, unit);
            }

            if (spellArmorElement != null)
            {
                UnitArmorAddValue(spellArmorElement, unit);
            }
        }

        private void UnitArmorAddValue(XElement armorElement, Unit unit)
        {
            unit.Armor = unit.Armor ?? new UnitArmor();

            XElement basicElement = armorElement.Element("ArmorSet").Elements("ArmorMitigationTable").FirstOrDefault(x => x.Attribute("index")?.Value == "Basic");
            XElement abilityElement = armorElement.Element("ArmorSet").Elements("ArmorMitigationTable").FirstOrDefault(x => x.Attribute("index")?.Value == "Ability");

            if (basicElement != null && int.TryParse(GameData.GetValueFromAttribute(basicElement.Attribute("value").Value), out int armorValue))
            {
                unit.Armor.PhysicalArmor = armorValue;
            }

            if (abilityElement != null && int.TryParse(GameData.GetValueFromAttribute(abilityElement.Attribute("value").Value), out armorValue))
            {
                unit.Armor.SpellArmor = armorValue;
            }
        }
    }
}
