using Heroes.Icons.Parser.Models;
using System.Linq;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.Heroes
{
    public class AzmodanData : HeroData
    {
        public AzmodanData(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, DescriptionParser descriptionParser, HeroOverrideLoader heroOverrideLoader)
            : base(heroDataLoader, descriptionLoader, descriptionParser, heroOverrideLoader)
        {
        }

        protected override void SetTooltipSubInfo(string attributeId, AbilityTalentBase abilityTalentBase, XDocument xmlData, bool allowOverrides = false)
        {
            if (string.IsNullOrEmpty(attributeId))
                return;

            base.SetTooltipSubInfo(attributeId, abilityTalentBase, xmlData, allowOverrides);

            if (HeroOverrideLoader.IdRedirectByAbilityId.ContainsKey(attributeId))
            {
                foreach (var type in HeroOverrideLoader.IdRedirectByAbilityId[attributeId])
                {
                    if (string.IsNullOrEmpty(type.Value.Id))
                        continue;

                    // find element in data file by looking up the id
                    var specialElement = xmlData.Descendants().Where(x => x.Attribute("id")?.Value == type.Value.Id).FirstOrDefault();
                    if (specialElement != null)
                    {
                        // get the first one
                        var element = specialElement.Descendants(type.Key).FirstOrDefault();
                        if (type.Key == "VitalArray")
                        {
                            double value = double.Parse(specialElement.Descendants("Change").FirstOrDefault().Attribute("value").Value);
                            if (value < 0)
                                value *= -1;

                            if (type.Value.InnerElement != null)
                            {
                                var cEffectCreatePersistent = xmlData.Descendants().Where(x => x.Attribute("id")?.Value == type.Value.InnerElement.Id).FirstOrDefault();
                                if (cEffectCreatePersistent != null)
                                {
                                    double periodic = double.Parse(cEffectCreatePersistent.Descendants(type.Value.InnerElement.Name).FirstOrDefault().Attribute("value").Value);

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
