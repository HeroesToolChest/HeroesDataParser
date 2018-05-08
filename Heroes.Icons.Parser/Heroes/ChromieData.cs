﻿using Heroes.Icons.Parser.Descriptions;
using Heroes.Icons.Parser.HeroData;
using Heroes.Icons.Parser.Models;
using System.Collections.Generic;
using System.Linq;

namespace Heroes.Icons.Parser.Heroes
{
    public class ChromieData : DefaultHeroData
    {
        public ChromieData(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, DescriptionParser descriptionParser, HeroOverrideLoader heroOverrideLoader)
            : base(heroDataLoader, descriptionLoader, descriptionParser, heroOverrideLoader)
        {
        }

        protected override void SetTooltipSubInfo(string attributeId, AbilityTalentBase abilityTalentBase, bool allowOverrides = false)
        {
            if (string.IsNullOrEmpty(attributeId))
                return;

            base.SetTooltipSubInfo(attributeId, abilityTalentBase, allowOverrides);

            if (HeroOverrideLoader.IdRedirectByAbilityId.TryGetValue(attributeId, out Dictionary<string, RedirectElement> idRedirects))
            {
                foreach (var redirectElement in idRedirects)
                {
                    if (string.IsNullOrEmpty(redirectElement.Value.Id))
                        continue;

                    // find element in data file by looking up the id
                    var specialElement = HeroDataLoader.XmlData.Descendants().Where(x => x.Attribute("id")?.Value == redirectElement.Value.Id).FirstOrDefault();
                    if (specialElement != null)
                    {
                        if (redirectElement.Key == "VitalArray")
                        {
                            double value = double.Parse(specialElement.Descendants("Change").FirstOrDefault().Attribute("value").Value);
                            if (value < 0)
                                value *= -1;

                            if (redirectElement.Value.InnerElement != null)
                            {
                                var cEffectCreatePersistent = HeroDataLoader.XmlData.Root.Elements().Where(x => x.Attribute("id")?.Value == redirectElement.Value.InnerElement.Id).FirstOrDefault();
                                if (cEffectCreatePersistent != null)
                                {
                                    double periodic = double.Parse(cEffectCreatePersistent.Descendants(redirectElement.Value.InnerElement.Name).FirstOrDefault().Attribute("value").Value);

                                    abilityTalentBase.Tooltip.Energy = (int)(value / periodic);
                                    abilityTalentBase.Tooltip.IsPerEnergyCost = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
