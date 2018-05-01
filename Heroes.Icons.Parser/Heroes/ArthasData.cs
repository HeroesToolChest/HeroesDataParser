﻿using Heroes.Icons.Parser.Models;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.Heroes
{
    public class ArthasData : HeroData
    {
        public ArthasData(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, DescriptionParser descriptionParser, HeroOverrideLoader heroOverrideLoader)
            : base(heroDataLoader, descriptionLoader, descriptionParser, heroOverrideLoader)
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