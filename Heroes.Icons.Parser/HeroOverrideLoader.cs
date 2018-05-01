using Heroes.Icons.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Heroes.Icons.Parser
{
    public class HeroOverrideLoader
    {
        public HeroOverrideLoader()
        {
        }

        public string HeroDataOverrideXmlFile => "HeroOverrides.xml";

        public Dictionary<string, string> CUnitOverrideByCHero { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, EnergyType> EnergyTypeOverrideByCHero { get; set; } = new Dictionary<string, EnergyType>();
        public Dictionary<string, int> EnergyOverrideByCHero { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, Dictionary<string, Action<Ability>>> ValueOverrideMethodByAbilityId { get; set; } = new Dictionary<string, Dictionary<string, Action<Ability>>>();
        public Dictionary<string, Dictionary<string, RedirectElement>> IdRedirectByAbilityId { get; set; } = new Dictionary<string, Dictionary<string, RedirectElement>>();
        public Dictionary<string, Dictionary<string, RedirectElement>> IdRedirectByWeaponId { get; set; } = new Dictionary<string, Dictionary<string, RedirectElement>>();
        public Dictionary<string, Dictionary<string, string>> LinkedAbilityByCHero { get; set; } = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, Dictionary<string, Action<HeroWeapon>>> ValueOverrideMethodByWeaponId { get; set; } = new Dictionary<string, Dictionary<string, Action<HeroWeapon>>>();
        public Dictionary<string, bool> ValidAbilities { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, bool> ValidWeapons { get; set; } = new Dictionary<string, bool>();

        public void LoadHeroOverride()
        {
            XDocument cHeroDocument = XDocument.Load(HeroDataOverrideXmlFile);
            var cHeroes = cHeroDocument.Descendants("CHero").Where(x => x.Attribute("id") != null);

            foreach (var hero in cHeroes)
            {
                string cHeroId = hero.Attribute("id").Value;

                foreach (var dataElement in hero.Descendants())
                {
                    string elementName = dataElement.Name.LocalName;

                    if (elementName == "CUnit")
                    {
                        CUnitOverrideByCHero.Add(cHeroId, dataElement.Attribute("value").Value);
                    }
                    else if (elementName == "EnergyType")
                    {
                        string energyType = dataElement.Attribute("value").Value;
                        if (Enum.TryParse(energyType, out EnergyType heroEnergyType))
                            EnergyTypeOverrideByCHero.Add(cHeroId, heroEnergyType);
                        else
                            EnergyTypeOverrideByCHero.Add(cHeroId, EnergyType.None);
                    }
                    else if (elementName == "Energy")
                    {
                        string energyValue = dataElement.Attribute("value").Value;
                        if (int.TryParse(energyValue, out int value))
                            EnergyOverrideByCHero.Add(cHeroId, value);
                        else
                            EnergyOverrideByCHero.Add(cHeroId, 0);
                    }
                    else if (elementName == "Ability")
                    {
                        string abilityId = dataElement.Attribute("id")?.Value;
                        string valid = dataElement.Attribute("valid")?.Value;

                        if (string.IsNullOrEmpty(abilityId))
                            continue;

                        if (bool.TryParse(valid, out bool result))
                        {
                            ValidAbilities.Add(abilityId, result);

                            if (!result)
                                continue;
                        }

                        var redirectElement = dataElement.Descendants("Redirect").FirstOrDefault();
                        var overrideElement = dataElement.Descendants("Override").FirstOrDefault();

                        if (redirectElement != null)
                            SetAbilityRedirects(abilityId, redirectElement);

                        if (overrideElement != null)
                            SetAbilityOverrides(abilityId, overrideElement);
                    }
                    else if (elementName == "LinkedAbilities")
                    {
                        SetLinkAbility(cHeroId, dataElement);
                    }
                    else if (elementName == "Weapon")
                    {
                        string weaponId = dataElement.Attribute("id")?.Value;
                        string valid = dataElement.Attribute("valid")?.Value;

                        if (string.IsNullOrEmpty(weaponId))
                            continue;

                        if (bool.TryParse(valid, out bool result))
                        {
                            ValidWeapons.Add(weaponId, result);

                            if (!result)
                                continue;
                        }

                        var redirectElement = dataElement.Descendants("Redirect").FirstOrDefault();
                        var overrideElement = dataElement.Descendants("Override").FirstOrDefault();

                        if (redirectElement != null)
                            SetWeaponRedirects(weaponId, redirectElement);

                        if (overrideElement != null)
                            SetWeaponOverrides(weaponId, overrideElement);
                    }
                }
            }
        }

        private void SetAbilityRedirects(string abilityId, XElement element)
        {
            var propertyRedirects = new Dictionary<string, RedirectElement>();

            // loop through each redirect child element
            foreach (var property in element.Descendants())
            {
                string propertyName = property.Name.LocalName;

                propertyRedirects.Add(propertyName, ReadRedirectElement(property));
            }

            if (!IdRedirectByAbilityId.ContainsKey(abilityId) && propertyRedirects.Count > 0)
                IdRedirectByAbilityId.Add(abilityId, propertyRedirects);
        }

        private void SetWeaponRedirects(string weaponId, XElement element)
        {
            var propertyRedirects = new Dictionary<string, RedirectElement>();

            // loop through each redirect child element
            foreach (var property in element.Descendants())
            {
                string propertyName = property.Name.LocalName;

                propertyRedirects.Add(propertyName, ReadRedirectElement(property));
            }

            if (!IdRedirectByWeaponId.ContainsKey(weaponId) && propertyRedirects.Count > 0)
                IdRedirectByWeaponId.Add(weaponId, propertyRedirects);
        }

        private void SetAbilityOverrides(string abilityId, XElement element)
        {
            var propertyOverrides = new Dictionary<string, Action<Ability>>();

            // loop through each override child element
            foreach (var property in element.Descendants())
            {
                string propertyName = property.Name.LocalName;
                string propertyValue = property.Attribute("value")?.Value;

                // remove existing property override - duplicates will override previous
                if (propertyOverrides.ContainsKey(propertyName))
                    propertyOverrides.Remove(propertyName);

                if (propertyName == "ParentLink")
                {
                    propertyOverrides.Add(propertyName, (ability) =>
                    {
                        ability.ParentLink = propertyValue;
                    });
                }
                else if (propertyName == "AbilityTier")
                {
                    propertyOverrides.Add(propertyName, (ability) =>
                    {
                        if (Enum.TryParse(propertyValue, out AbilityTier abilityTier))
                            ability.Tier = abilityTier;
                        else
                            ability.Tier = AbilityTier.Basic;
                    });
                }
                else if (propertyName == "Custom")
                {
                    propertyOverrides.Add(propertyName, (ability) =>
                    {
                        ability.Tooltip.Custom = propertyValue;
                    });
                }
            }

            if (!ValueOverrideMethodByAbilityId.ContainsKey(abilityId) && propertyOverrides.Count > 0)
                ValueOverrideMethodByAbilityId.Add(abilityId, propertyOverrides);
        }

        private void SetWeaponOverrides(string weaponId, XElement element)
        {
            var propertyOverrides = new Dictionary<string, Action<HeroWeapon>>();

            // loop through each override child element
            foreach (var property in element.Descendants())
            {
                string propertyName = property.Name.LocalName;
                string propertyValue = property.Attribute("value")?.Value;

                // remove existing property override - duplicates will override previous
                if (propertyOverrides.ContainsKey(propertyName))
                    propertyOverrides.Remove(propertyName);

                if (propertyName == "Range")
                {
                    propertyOverrides.Add(propertyName, (weapon) =>
                    {
                        weapon.Range = double.Parse(propertyValue);
                    });
                }
            }

            if (!ValueOverrideMethodByWeaponId.ContainsKey(weaponId) && propertyOverrides.Count > 0)
                ValueOverrideMethodByWeaponId.Add(weaponId, propertyOverrides);
        }

        private void SetLinkAbility(string cHeroId, XElement element)
        {
            var linkAbilities = new Dictionary<string, string>();

            foreach (var linkAbility in element.Descendants())
            {
                string name = linkAbility.Name.LocalName;
                string id = linkAbility.Attribute("id")?.Value;
                if (string.IsNullOrEmpty(id))
                    continue;

                if (linkAbilities.ContainsKey(id))
                    linkAbilities.Remove(id);

                linkAbilities.Add(id, name);
            }

            LinkedAbilityByCHero.Add(cHeroId, linkAbilities);
        }

        private RedirectElement ReadRedirectElement(XElement element)
        {
            var id = element.Attribute("id");
            var value = element.Attribute("value");

            var helperElement = new RedirectElement(id?.Value, value?.Value)
            {
                Name = element.Name.LocalName,
            };

            foreach (var innerElement in element.Descendants())
            {
                helperElement.InnerElement = ReadRedirectElement(innerElement);
            }

            return helperElement;
        }
    }
}
