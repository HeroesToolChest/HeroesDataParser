using Heroes.Icons.Parser.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.Heroes
{
    public class AlarakData : HeroData
    {
        public AlarakData(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, DescriptionParser descriptionParser, HeroOverrideLoader heroOverrideLoader)
            : base(heroDataLoader, descriptionLoader, descriptionParser, heroOverrideLoader)
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
                    int value = int.Parse(dataElement.Descendants("Change").FirstOrDefault().Attribute("value").Value);
                    if (value < 0)
                        value *= -1;

                    abilityTalentBase.Tooltip.Energy = value;
                }
            }
        }
    }
}
