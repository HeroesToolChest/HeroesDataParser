using Heroes.Icons.Parser.Models;
using Heroes.Icons.Parser.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.UnitData.Overrides
{
    public class HeroOverrideData
    {
        private readonly GameData GameData;

        public HeroOverrideData(GameData gameData)
        {
            GameData = gameData;
        }

        public string HeroDataOverrideXmlFile => @"HeroOverrides.xml";

        public Dictionary<string, HeroOverride> HeroOverridesByCHero { get; set; } = new Dictionary<string, HeroOverride>();

        public void LoadHeroOverrideData()
        {
            XDocument cHeroDocument = XDocument.Load(HeroDataOverrideXmlFile);
            IEnumerable<XElement> cHeroes = cHeroDocument.Root.Elements("CHero").Where(x => x.Attribute("id") != null);

            foreach (XElement heroElement in cHeroes)
            {
                SetHeroOverrides(heroElement);
            }
        }

        private void SetHeroOverrides(XElement heroElement)
        {
            HeroOverride heroOverride = new HeroOverride();
            AbilityOverride abilityOverride = new AbilityOverride(GameData);
            WeaponOverride weaponOverride = new WeaponOverride(GameData);
            string cHeroId = heroElement.Attribute("id").Value;

            foreach (var dataElement in heroElement.Elements())
            {
                string elementName = dataElement.Name.LocalName;

                if (elementName == "Name")
                {
                    heroOverride.NameOverride = (true, dataElement.Attribute("value").Value);
                }
                else if (elementName == "ShortName")
                {
                    heroOverride.ShortNameOverride = (true, dataElement.Attribute("value").Value);
                }
                else if (elementName == "CUnit")
                {
                    heroOverride.CUnitOverride = (true, dataElement.Attribute("value").Value);
                }
                else if (elementName == "EnergyType")
                {
                    string energyType = dataElement.Attribute("value").Value;
                    if (Enum.TryParse(energyType, out UnitEnergyType heroEnergyType))
                        heroOverride.EnergyTypeOverride = (true, heroEnergyType);
                    else
                        heroOverride.EnergyTypeOverride = (true, UnitEnergyType.None);
                }
                else if (elementName == "Energy")
                {
                    string energyValue = dataElement.Attribute("value").Value;
                    if (int.TryParse(energyValue, out int value))
                        heroOverride.EnergyOverride = (true, value);
                    else
                        heroOverride.EnergyOverride = (true, 0);
                }
                else if (elementName == "Ability")
                {
                    string abilityId = dataElement.Attribute("id")?.Value;
                    string valid = dataElement.Attribute("valid")?.Value;
                    string add = dataElement.Attribute("add")?.Value;
                    string button = dataElement.Attribute("button")?.Value;

                    if (string.IsNullOrEmpty(abilityId))
                        continue;

                    // valid
                    if (bool.TryParse(valid, out bool result))
                    {
                        heroOverride.IsValidAbilityByAbilityId.Add(abilityId, result);

                        if (!result)
                            continue;
                    }

                    // add
                    if (bool.TryParse(add, out result))
                    {
                        heroOverride.AddedAbilitiesByAbilityId.Add(abilityId, (button, result));

                        if (!result)
                            continue;
                    }

                    // override
                    var overrideElement = dataElement.Elements("Override").FirstOrDefault();
                    if (overrideElement != null)
                        abilityOverride.SetOverride(abilityId, overrideElement, heroOverride.PropertyOverrideMethodByAbilityId);
                }
                else if (elementName == "LinkedAbilities")
                {
                    SetLinkAbilities(dataElement, heroOverride);
                }
                else if (elementName == "Weapon")
                {
                    string weaponId = dataElement.Attribute("id")?.Value;
                    string valid = dataElement.Attribute("valid")?.Value;

                    if (string.IsNullOrEmpty(weaponId))
                        continue;

                    if (bool.TryParse(valid, out bool result))
                    {
                        heroOverride.IsValidWeaponByWeaponId.Add(weaponId, result);

                        if (!result)
                            continue;
                    }

                    var overrideElement = dataElement.Elements("Override").FirstOrDefault();
                    if (overrideElement != null)
                        weaponOverride.SetOverride(weaponId, overrideElement, heroOverride.PropertyOverrideMethodByWeaponId);
                }
                else if (elementName == "HeroUnit")
                {
                    string heroUnitId = dataElement.Attribute("id")?.Value;

                    if (string.IsNullOrEmpty(heroUnitId))
                        continue;

                    AddHeroUnits(heroUnitId, dataElement, heroOverride);
                }
                else if (elementName == "ParentLink")
                {
                    heroOverride.ParentLinkOverride = (true, dataElement.Attribute("value").Value);
                }
            }

            if (!HeroOverridesByCHero.ContainsKey(cHeroId))
                HeroOverridesByCHero.Add(cHeroId, heroOverride);
        }

        private void SetLinkAbilities(XElement element, HeroOverride heroOverride)
        {
            foreach (var linkAbility in element.Elements())
            {
                string id = linkAbility.Attribute("id")?.Value;
                string elementName = linkAbility.Attribute("element")?.Value;
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(elementName))
                    continue;

                if (heroOverride.LinkedElementNamesByAbilityId.ContainsKey(id))
                    heroOverride.LinkedElementNamesByAbilityId.Remove(id);

                heroOverride.LinkedElementNamesByAbilityId.Add(id, elementName);
            }
        }

        private void AddHeroUnits(string elementId, XElement element, HeroOverride heroOverride)
        {
            if (heroOverride.HeroUnits.Contains(elementId))
                heroOverride.HeroUnits.Remove(elementId);

            heroOverride.HeroUnits.Add(elementId);

            SetHeroOverrides(element);
        }
    }
}
