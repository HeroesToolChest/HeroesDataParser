using Heroes.Icons.Parser.Descriptions;
using Heroes.Icons.Parser.HeroData;
using Heroes.Icons.Parser.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.Heroes
{
    public class KerriganData : DefaultHeroData
    {
        public KerriganData(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, DescriptionParser descriptionParser, HeroOverrideLoader heroOverrideLoader)
            : base(heroDataLoader, descriptionLoader, descriptionParser, heroOverrideLoader)
        {
        }

        protected override void HeroWeaponAddDamage(XElement weaponLegacy, HeroWeapon weapon, XDocument xmlData, string weaponNameId)
        {
            if (HeroOverrideLoader.IdRedirectByWeaponId.TryGetValue(weaponNameId, out Dictionary<string, RedirectElement> idRedirects))
            {
                foreach (var redirectElement in idRedirects)
                {
                    if (string.IsNullOrEmpty(redirectElement.Value.Id))
                        continue;

                    var specialElement = xmlData.Descendants(redirectElement.Key).Where(x => x.Attribute("id")?.Value == redirectElement.Value.Id).FirstOrDefault();
                    if (specialElement != null)
                    {
                        weapon.Damage = double.Parse(specialElement.Descendants("Amount").FirstOrDefault().Attribute("value").Value);
                    }
                }
            }
        }
    }
}
