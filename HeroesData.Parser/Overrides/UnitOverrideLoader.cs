using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.Overrides.PropertyOverrides;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class UnitOverrideLoader : OverrideLoaderBase<UnitDataOverride>, IOverrideLoader
    {
        public UnitOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        protected override string OverrideFileName => $"unit-{base.OverrideFileName}";

        protected override string OverrideElementName => "CUnit";

        protected override void SetOverride(XElement element)
        {
            UnitDataOverride unitDataOverride = new UnitDataOverride();

            AbilityPropertyOverride abilityOverride = new AbilityPropertyOverride();
            //WeaponPropertyOverride weaponOverride = new WeaponPropertyOverride(GameData, HotsBuild);

            string unitId = element.Attribute("id").Value;

            foreach (XElement dataElement in element.Elements())
            {
                string elementName = dataElement.Name.LocalName;
                //string valueAttribute = dataElement.Attribute("value")?.Value;

                //XElement overrideElement = null;

                switch (elementName)
                {
                    case "Ability":
                        string abilityId = dataElement.Attribute("id")?.Value;
                        string buttonId = dataElement.Attribute("buttonId")?.Value;

                        if (!string.IsNullOrEmpty(abilityId))
                        {
                            XElement overrideElement = dataElement.Element("Override");

                            string elementId = abilityId;
                            if (!string.IsNullOrEmpty(buttonId))
                                elementId = $"{abilityId}{buttonId}";

                            if (overrideElement != null)
                                abilityOverride.SetOverride(elementId, overrideElement, unitDataOverride.PropertyAbilityOverrideMethodByAbilityId);
                        }

                        break;
                    //case "Name":
                    //    if (!string.IsNullOrEmpty(valueAttribute))
                    //        unitDataOverride.NameOverride = (true, valueAttribute);
                    //    break;
                    //case "HyperlinkId":
                    //    if (!string.IsNullOrEmpty(valueAttribute))
                    //        unitDataOverride.HyperlinkIdOverride = (true, valueAttribute);
                    //    break;
                    //case "EnergyType":
                    //    string energyType = valueAttribute;
                    //    unitDataOverride.EnergyTypeOverride = (true, energyType);
                    //    break;
                    //case "Energy":
                    //    string energyValue = valueAttribute;
                    //    if (int.TryParse(energyValue, out int value))
                    //    {
                    //        if (value < 0)
                    //            value = 0;

                    //        unitDataOverride.EnergyOverride = (true, value);
                    //    }
                    //    else
                    //    {
                    //        unitDataOverride.EnergyOverride = (true, 0);
                    //    }

                    //    break;
                    //case "Button":
                    //    string buttonId = dataElement.Attribute("id")?.Value;
                    //    string parent = dataElement.Attribute("parent")?.Value;
                    //    string referenceNameId = dataElement.Attribute("referenceNameId")?.Value;

                    //    if (string.IsNullOrEmpty(buttonId))
                    //        continue;

                    //    if (parent == null)
                    //        parent = string.Empty;

                    //    unitDataOverride.AddedAbilityByButtonId.Add(new AddedButtonAbility()
                    //    {
                    //        ButtonId = buttonId,
                    //        ParentValue = parent,
                    //        ReferenceNameId = referenceNameId,
                    //    });

                    //    // override
                    //    overrideElement = dataElement.Element("Override");
                    //    if (overrideElement != null)
                    //        abilityOverride.SetOverride(buttonId, overrideElement, unitDataOverride.PropertyAbilityOverrideMethodByAbilityId);
                    //    break;
                    //case "Weapon":
                    //    string weaponId = dataElement.Attribute("id")?.Value;
                    //    string valid = dataElement.Attribute("valid")?.Value;

                    //    if (string.IsNullOrEmpty(weaponId))
                    //        continue;

                    //    if (bool.TryParse(valid, out bool weaponValidresult))
                    //    {
                    //        unitDataOverride.IsValidWeaponByWeaponId.Add(weaponId, weaponValidresult);

                    //        if (!weaponValidresult)
                    //            continue;
                    //    }

                    //    overrideElement = dataElement.Element("Override");
                    //    if (overrideElement != null)
                    //        weaponOverride.SetOverride(weaponId, overrideElement, unitDataOverride.PropertyWeaponOverrideMethodByWeaponId);
                    //    break;
                    //case "ParentLink":
                    //    unitDataOverride.ParentLinkOverride = (true, dataElement.Attribute("value")?.Value);
                    //    break;
                }
            }

            DataOverridesById[unitId] = unitDataOverride;
        }
    }
}
