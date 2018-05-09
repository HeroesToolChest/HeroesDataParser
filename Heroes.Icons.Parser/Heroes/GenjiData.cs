﻿using Heroes.Icons.Parser.Descriptions;
using Heroes.Icons.Parser.HeroData;
using Heroes.Icons.Parser.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.Heroes
{
    public class GenjiData : DefaultHeroData
    {
        public GenjiData(DataLoader dataLoader, DescriptionParser descriptionParser)
            : base(dataLoader, descriptionParser)
        {
        }

        protected override void HeroWeaponAddDamage(XElement weaponLegacy, HeroWeapon weapon, string weaponNameId)
        {
            if (HeroOverrideLoader.IdRedirectByWeaponId.TryGetValue(weaponNameId, out Dictionary<string, RedirectElement> idRedirects))
            {
                foreach (var redirectElement in idRedirects)
                {
                    if (string.IsNullOrEmpty(redirectElement.Value.Id))
                        continue;

                    var specialElement = HeroDataLoader.XmlData.Root.Elements(redirectElement.Key).Where(x => x.Attribute("id")?.Value == redirectElement.Value.Id).FirstOrDefault();
                    if (specialElement != null)
                    {
                        weapon.Damage = double.Parse(specialElement.Elements("Amount").FirstOrDefault().Attribute("value").Value);
                    }
                }
            }
        }
    }
}
