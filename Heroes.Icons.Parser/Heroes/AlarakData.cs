using Heroes.Icons.Parser.Descriptions;
using Heroes.Icons.Parser.HeroData;
using Heroes.Icons.Parser.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.Heroes
{
    public class AlarakData : DefaultHeroData
    {
        public AlarakData(DataLoader dataLoader, DescriptionParser descriptionParser) 
            : base(dataLoader, descriptionParser)
        {
        }

        protected override void ApplyUniquePropertyLookups(KeyValuePair<string, RedirectElement> element, XElement dataElement, AbilityTalentBase abilityTalentBase)
        {
            if (element.Key == "Cost")
            {
                abilityTalentBase.Tooltip.Cooldown = double.Parse(dataElement.Attribute("CooldownTimeUse").Value);
            }
            else if (element.Key == "VitalArray")
            {
                if (dataElement.Attribute("index").Value == "Energy")
                {
                    int value = int.Parse(dataElement.Elements("Change").FirstOrDefault().Attribute("value").Value);
                    if (value < 0)
                        value *= -1;

                    abilityTalentBase.Tooltip.Energy = value;
                }
            }
        }
    }
}
