using Heroes.Models.AbilityTalents;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.Overrides.PropertyOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class UnitOverrideLoader : OverrideLoaderBase<UnitDataOverride>, IOverrideLoader
    {
        public UnitOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public UnitOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"unit-{base.OverrideFileName}";

        protected override string OverrideElementName => "CUnit";

        protected override void SetOverride(XElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            UnitDataOverride unitDataOverride = new UnitDataOverride();

            AbilityPropertyOverride abilityOverride = new AbilityPropertyOverride();
            WeaponPropertyOverride weaponOverride = new WeaponPropertyOverride();

            string unitId = element.Attribute("id").Value;

            foreach (XElement dataElement in element.Elements())
            {
                string elementName = dataElement.Name.LocalName;
                string valueAttribute = dataElement.Attribute("value")?.Value ?? string.Empty;

                XElement? overrideElement;

                switch (elementName)
                {
                    case "Name":
                        unitDataOverride.NameOverride = (true, valueAttribute);
                        break;
                    case "HyperlinkId":
                        unitDataOverride.HyperlinkIdOverride = (true, valueAttribute);
                        break;
                    case "CUnit":
                        unitDataOverride.CUnitOverride = (true, valueAttribute);
                        break;
                    case "EnergyType":
                        unitDataOverride.EnergyTypeOverride = (true, valueAttribute);
                        break;
                    case "Energy":
                        string energyValue = valueAttribute;
                        if (int.TryParse(energyValue, out int value))
                        {
                            if (value < 0)
                                value = 0;

                            unitDataOverride.EnergyOverride = (true, value);
                        }
                        else
                        {
                            unitDataOverride.EnergyOverride = (true, 0);
                        }

                        break;
                    case "ParentLink":
                        unitDataOverride.ParentLinkOverride = (true, valueAttribute);
                        break;
                    case "Ability":
                        string id = dataElement.Attribute("id")?.Value ?? string.Empty;
                        string? abilityType = dataElement.Attribute("abilityType")?.Value;
                        string? passiveAbility = dataElement.Attribute("passive")?.Value;
                        string? addedAbility = dataElement.Attribute("add")?.Value;

                        AbilityTalentId abilityTalentId = new AbilityTalentId(string.Empty, string.Empty);

                        string[] idSplit = id.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        if (idSplit.Length >= 2)
                        {
                            abilityTalentId.ReferenceId = idSplit[0];
                            abilityTalentId.ButtonId = idSplit[1];
                        }
                        else if (idSplit.Length == 1)
                        {
                            abilityTalentId.ReferenceId = idSplit[0];
                            abilityTalentId.ButtonId = idSplit[0];
                        }

                        if (Enum.TryParse(abilityType, true, out AbilityTypes abilityTypeResult))
                            abilityTalentId.AbilityType = abilityTypeResult;

                        if (bool.TryParse(passiveAbility, out bool abilityPassiveResult))
                            abilityTalentId.IsPassive = abilityPassiveResult;

                        if (bool.TryParse(addedAbility, out bool abilityAddedResult))
                        {
                            unitDataOverride.AddAddedAbility(abilityTalentId, abilityAddedResult);

                            if (!abilityAddedResult)
                                continue;
                        }

                        if (!string.IsNullOrEmpty(id))
                        {
                            overrideElement = dataElement.Element("Override");

                            if (overrideElement != null)
                                abilityOverride.SetOverride(abilityTalentId.ToString(), overrideElement, unitDataOverride.PropertyAbilityOverrideMethodByAbilityId);
                        }

                        break;
                    case "Weapon":
                        string? weaponId = dataElement.Attribute("id")?.Value;
                        string? addedWeapon = dataElement.Attribute("add")?.Value;

                        if (string.IsNullOrEmpty(weaponId))
                            continue;

                        if (bool.TryParse(addedWeapon, out bool weaponValidResult))
                        {
                            unitDataOverride.AddAddedWeapon(weaponId, weaponValidResult);

                            if (!weaponValidResult)
                                continue;
                        }

                        overrideElement = dataElement.Element("Override");
                        if (overrideElement != null)
                            weaponOverride.SetOverride(weaponId, overrideElement, unitDataOverride.PropertyWeaponOverrideMethodByWeaponId);
                        break;
                }
            }

            DataOverridesById[unitId] = unitDataOverride;
        }
    }
}
