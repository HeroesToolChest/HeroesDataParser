using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.Overrides.PropertyOverrides;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class HeroOverrideLoader : OverrideLoaderBase<HeroDataOverride>, IOverrideLoader
    {
        public HeroOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        protected override string OverrideFileName => $"hero-{base.OverrideFileName}";

        protected override string OverrideElementName => "CHero";

        protected override void SetOverride(XElement element)
        {
            HeroDataOverride heroDataOverride = new HeroDataOverride();

            //AbilityPropertyOverride abilityOverride = null;// = new AbilityPropertyOverride(GameData, HotsBuild);
            //TalentPropertyOverride talentOverride = null;// new TalentPropertyOverride(GameData, HotsBuild);
            //WeaponPropertyOverride weaponOverride = null;// new WeaponPropertyOverride(GameData, HotsBuild);
            //PortraitPropertyOverride portraitOverride = null;// new PortraitPropertyOverride(GameData, HotsBuild);

            string heroId = element.Attribute("id").Value;

            foreach (XElement dataElement in element.Elements())
            {
                string elementName = dataElement.Name.LocalName;
                string valueAttribute = dataElement.Attribute("value")?.Value;

                //XElement overrideElement = null;

                switch (elementName)
                {
                    case "HeroUnit":
                        string heroUnitId = dataElement.Attribute("id")?.Value;

                        if (string.IsNullOrEmpty(heroUnitId))
                            continue;

                        heroDataOverride.AddHeroUnit(heroUnitId);
                        SetOverride(dataElement);

                        break;

                        //        case "Name":
                        //            if (!string.IsNullOrEmpty(valueAttribute))
                        //                heroDataOverride.NameOverride = (true, valueAttribute);
                        //            break;
                        //        case "HyperlinkId":
                        //            if (!string.IsNullOrEmpty(valueAttribute))
                        //                heroDataOverride.HyperlinkIdOverride = (true, valueAttribute);
                        //            break;
                        //        case "CUnit":
                        //            if (!string.IsNullOrEmpty(valueAttribute))
                        //                heroDataOverride.CUnitOverride = (true, valueAttribute);
                        //            break;
                        //        case "EnergyType":
                        //            string energyType = valueAttribute;
                        //            heroDataOverride.EnergyTypeOverride = (true, energyType);
                        //            break;
                        //        case "Energy":
                        //            string energyValue = valueAttribute;
                        //            if (int.TryParse(energyValue, out int value))
                        //            {
                        //                if (value < 0)
                        //                    value = 0;

                        //                heroDataOverride.EnergyOverride = (true, value);
                        //            }
                        //            else
                        //            {
                        //                heroDataOverride.EnergyOverride = (true, 0);
                        //            }

                        //            break;
                        //        case "Ability":
                        //            string abilityId = dataElement.Attribute("id")?.Value;
                        //            string valid = dataElement.Attribute("valid")?.Value;
                        //            string add = dataElement.Attribute("add")?.Value;
                        //            string button = dataElement.Attribute("button")?.Value;
                        //            string referenceNameId = dataElement.Attribute("referenceNameId")?.Value;
                        //            string remove = dataElement.Attribute("remove")?.Value;
                        //            string newButtonName = dataElement.Element("ButtonName")?.Attribute("value")?.Value;

                        //            if (!string.IsNullOrEmpty(abilityId))
                        //            {
                        //                // valid
                        //                if (bool.TryParse(valid, out bool validResult))
                        //                {
                        //                    heroDataOverride.IsValidAbilityByAbilityId.Add(abilityId, validResult);

                        //                    if (!validResult)
                        //                        continue;
                        //                }

                        //                // add
                        //                if (bool.TryParse(add, out bool addResult))
                        //                {
                        //                    heroDataOverride.AddedAbilityByAbilityId.Add(abilityId, (button, addResult));

                        //                    if (!addResult)
                        //                        continue;
                        //                }

                        //                if (!string.IsNullOrEmpty(button) && !string.IsNullOrEmpty(newButtonName))
                        //                {
                        //                    heroDataOverride.ButtonNameOverrideByAbilityButtonId.Add((abilityId, button), newButtonName);
                        //                }

                        //                // override
                        //                overrideElement = dataElement.Element("Override");
                        //                if (overrideElement != null)
                        //                    abilityOverride.SetOverride(abilityId, overrideElement, heroDataOverride.PropertyAbilityOverrideMethodByAbilityId);
                        //            }
                        //            else
                        //            {
                        //                if (!string.IsNullOrEmpty(referenceNameId) && !string.IsNullOrEmpty(remove))
                        //                {
                        //                    if (bool.TryParse(remove, out bool removeResult))
                        //                    {
                        //                        heroDataOverride.RemovedAbilityByAbilityReferenceNameId.Add(referenceNameId, removeResult);
                        //                    }

                        //                    continue;
                        //                }
                        //            }

                        //            break;
                        //        case "Button":
                        //            string buttonId = dataElement.Attribute("id")?.Value;
                        //            string parent = dataElement.Attribute("parent")?.Value;

                        //            if (string.IsNullOrEmpty(buttonId))
                        //                continue;

                        //            if (parent == null)
                        //                parent = string.Empty;

                        //            heroDataOverride.AddedAbilityByButtonId.Add(new AddedButtonAbility()
                        //            {
                        //                ButtonId = buttonId,
                        //                ParentValue = parent,
                        //            });

                        //            // override
                        //            overrideElement = dataElement.Element("Override");
                        //            if (overrideElement != null)
                        //                abilityOverride.SetOverride(buttonId, overrideElement, heroDataOverride.PropertyAbilityOverrideMethodByAbilityId);
                        //            break;
                        //        case "Talent":
                        //            string talentId = dataElement.Attribute("id")?.Value;
                        //            valid = dataElement.Attribute("valid")?.Value;
                        //            add = dataElement.Attribute("add")?.Value;
                        //            button = dataElement.Attribute("button")?.Value;

                        //            if (string.IsNullOrEmpty(talentId))
                        //                continue;

                        //            // override
                        //            overrideElement = dataElement.Element("Override");
                        //            if (overrideElement != null)
                        //                talentOverride.SetOverride(talentId, overrideElement, heroDataOverride.PropertyTalentOverrideMethodByTalentId);
                        //            break;
                        //        case "Weapon":
                        //            string weaponId = dataElement.Attribute("id")?.Value;
                        //            valid = dataElement.Attribute("valid")?.Value;

                        //            if (string.IsNullOrEmpty(weaponId))
                        //                continue;

                        //            if (bool.TryParse(valid, out bool weaponValidresult))
                        //            {
                        //                heroDataOverride.IsValidWeaponByWeaponId.Add(weaponId, weaponValidresult);

                        //                if (!weaponValidresult)
                        //                    continue;
                        //            }

                        //            overrideElement = dataElement.Element("Override");
                        //            if (overrideElement != null)
                        //                weaponOverride.SetOverride(weaponId, overrideElement, heroDataOverride.PropertyWeaponOverrideMethodByWeaponId);
                        //            break;
                        //        case "HeroUnit":
                        //            string heroUnitId = dataElement.Attribute("id")?.Value;

                        //            if (string.IsNullOrEmpty(heroUnitId))
                        //                continue;

                        //            AddHeroUnits(heroUnitId, dataElement, heroDataOverride);
                        //            break;
                        //        case "ParentLink":
                        //            heroDataOverride.ParentLinkOverride = (true, dataElement.Attribute("value")?.Value);
                        //            break;
                        //        case "Portrait":
                        //            overrideElement = dataElement.Element("Override");
                        //            if (overrideElement != null)
                        //                portraitOverride.SetOverride(cHeroId, overrideElement, heroDataOverride.PropertyPortraitOverrideMethodByCHeroId);
                        //            break;
                        //    }
                        //}

                }

                if (!DataOverridesById.ContainsKey(heroId))
                    DataOverridesById.Add(heroId, heroDataOverride);
            }
        }

        //private void AddHeroUnits(string elementId, XElement element, HeroDataOverride heroDataOverride)
        //{
        //    heroDataOverride.HeroUnits.Add(elementId);
        //    SetOverride(element);
        //}
    }
}
