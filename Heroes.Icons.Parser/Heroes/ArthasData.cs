using Heroes.Icons.Parser.Descriptions;
using Heroes.Icons.Parser.HeroData;
using Heroes.Icons.Parser.Models;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.Heroes
{
    public class ArthasData : DefaultHeroData
    {
        public ArthasData(DataLoader dataLoader, DescriptionParser descriptionParser)
            : base(dataLoader, descriptionParser)
        {
        }

        protected override void ApplyUniquePropertyLookups(KeyValuePair<string, RedirectElement> element, XElement dataElement, AbilityTalentBase abilityTalentBase)
        {
            if (element.Key == "ValidatorArray")
            {
                abilityTalentBase.Tooltip.Energy = ValidatorValues.GetValue(dataElement.Attribute("value").Value);
                abilityTalentBase.Tooltip.IsPerEnergyCost = true;
            }
        }
    }
}
