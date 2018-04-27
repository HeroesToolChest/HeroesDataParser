using Heroes.Icons.Parser.Models;
using System.Linq;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.Heroes
{
    public class GenjiData : HeroData
    {
        public GenjiData(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, DescriptionParser descriptionParser, HeroOverrideLoader heroOverrideLoader)
            : base(heroDataLoader, descriptionLoader, descriptionParser, heroOverrideLoader)
        {
        }

        protected override void AddHeroWeapon(Hero hero, XElement element, XDocument xmlData)
        {
            string link = element.Attribute("Link")?.Value;
            if (!string.IsNullOrEmpty(link))
            {
                var weaponLegacy = xmlData.Descendants("CWeaponLegacy").Where(x => x.Attribute("id")?.Value == link);
                if (weaponLegacy != null)
                {
                    // range
                    var rangeElement = weaponLegacy.Descendants("Range").FirstOrDefault();
                    if (rangeElement != null)
                        hero.HeroWeapon.Range = double.Parse(rangeElement.Attribute("value").Value);

                    // period
                    var periodElement = weaponLegacy.Descendants("Period").FirstOrDefault();
                    if (periodElement != null)
                        hero.HeroWeapon.Period = double.Parse(periodElement.Attribute("value").Value);
                    else
                        hero.HeroWeapon.Period = DefaultPeriod;

                    if (HeroOverrideLoader.IdRedirectByWeaponId.ContainsKey(link))
                    {
                        foreach (var type in HeroOverrideLoader.IdRedirectByWeaponId[link])
                        {
                            if (string.IsNullOrEmpty(type.Value.Id))
                                continue;

                            var specialElement = xmlData.Descendants(type.Key).Where(x => x.Attribute("id")?.Value == type.Value.Id).FirstOrDefault();
                            if (specialElement != null)
                            {
                                hero.HeroWeapon.Damage = double.Parse(specialElement.Descendants("Amount").FirstOrDefault().Attribute("value").Value);
                            }
                        }
                    }
                }
            }
        }
    }
}
