using Heroes.Icons.Parser.Models;
using System.Linq;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.Heroes
{
    public class KerriganData : HeroData
    {
        public KerriganData(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, DescriptionParser descriptionParser, HeroOverrideLoader heroOverrideLoader)
            : base(heroDataLoader, descriptionLoader, descriptionParser, heroOverrideLoader)
        {
        }

        protected override void HeroWeaponAddDamage(XElement weaponLegacy, HeroWeapon weapon, XDocument xmlData, string weaponNameId)
        {
            if (HeroOverrideLoader.IdRedirectByWeaponId.ContainsKey(weaponNameId))
            {
                foreach (var type in HeroOverrideLoader.IdRedirectByWeaponId[weaponNameId])
                {
                    if (string.IsNullOrEmpty(type.Value.Id))
                        continue;

                    var specialElement = xmlData.Descendants(type.Key).Where(x => x.Attribute("id")?.Value == type.Value.Id).FirstOrDefault();
                    if (specialElement != null)
                    {
                        weapon.Damage = double.Parse(specialElement.Descendants("Amount").FirstOrDefault().Attribute("value").Value);
                    }
                }
            }
        }
    }
}
