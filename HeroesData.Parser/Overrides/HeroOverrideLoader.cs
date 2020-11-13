using Heroes.Models.AbilityTalents;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.Overrides.PropertyOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class HeroOverrideLoader : OverrideLoaderBase<HeroDataOverride>, IOverrideLoader
    {
        public HeroOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public HeroOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"hero-{base.OverrideFileName}";

        protected override string OverrideElementName => "CHero";

        protected override void SetOverride(XElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            HeroDataOverride heroDataOverride = new HeroDataOverride();

            AbilityPropertyOverride abilityOverride = new AbilityPropertyOverride();
            TalentPropertyOverride talentOverride = new TalentPropertyOverride();
            WeaponPropertyOverride weaponOverride = new WeaponPropertyOverride();
            PortraitPropertyOverride portraitOverride = new PortraitPropertyOverride();

            string heroId = element.Attribute("id")?.Value ?? string.Empty;

            foreach (XElement dataElement in element.Elements())
            {
                string elementName = dataElement.Name.LocalName;
                string valueAttribute = dataElement.Attribute("value")?.Value ?? string.Empty;

                XElement? overrideElement;

                switch (elementName)
                {
                    case "Name":
                        heroDataOverride.NameOverride = (true, valueAttribute);
                        break;
                    case "HyperlinkId":
                        heroDataOverride.HyperlinkIdOverride = (true, valueAttribute);
                        break;
                    case "CUnit":
                        heroDataOverride.CUnitOverride = (true, valueAttribute);
                        break;
                    case "EnergyType":
                        heroDataOverride.EnergyTypeOverride = (true, valueAttribute);
                        break;
                    case "Energy":
                        string energyValue = valueAttribute;

                        if (int.TryParse(energyValue, out int value))
                        {
                            if (value < 0)
                                value = 0;

                            heroDataOverride.EnergyOverride = (true, value);
                        }
                        else
                        {
                            heroDataOverride.EnergyOverride = (true, 0);
                        }

                        break;
                    case "ParentLink":
                        heroDataOverride.ParentLinkOverride = (true, valueAttribute ?? string.Empty);
                        break;
                    case "Ability":
                    case "Talent":
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

                        if (elementName == "Ability")
                        {
                            if (bool.TryParse(addedAbility, out bool abilityAddedResult))
                            {
                                heroDataOverride.AddAddedAbility(abilityTalentId, abilityAddedResult);

                                if (!abilityAddedResult)
                                    continue;
                            }
                        }

                        if (!string.IsNullOrEmpty(id))
                        {
                            overrideElement = dataElement.Element("Override");

                            if (overrideElement != null)
                            {
                                if (elementName == "Ability")
                                    abilityOverride.SetOverride(abilityTalentId.ToString(), overrideElement, heroDataOverride.PropertyAbilityOverrideMethodByAbilityId);
                                else
                                    talentOverride.SetOverride(abilityTalentId.ToString(), overrideElement, heroDataOverride.PropertyTalentOverrideMethodByTalentId);
                            }
                        }

                        break;
                    case "Portrait":
                        overrideElement = dataElement.Element("Override");
                        if (overrideElement != null)
                            portraitOverride.SetOverride(heroId, overrideElement, heroDataOverride.PropertyPortraitOverrideMethodByHeroId);
                        break;
                    case "HeroUnit":
                        string? heroUnitId = dataElement.Attribute("id")?.Value;
                        string? removeHeroUnit = dataElement.Attribute("remove")?.Value;

                        if (string.IsNullOrEmpty(heroUnitId))
                            continue;

                        if (bool.TryParse(removeHeroUnit, out bool heroUnitRemoveResult))
                        {
                            if (heroUnitRemoveResult)
                            {
                                heroDataOverride.AddRemovedHeroUnit(heroUnitId);
                                continue;
                            }
                        }

                        heroDataOverride.AddHeroUnit(heroUnitId);
                        SetOverride(dataElement);

                        break;
                    case "Weapon":
                        string? weaponId = dataElement.Attribute("id")?.Value;
                        string? addedWeapon = dataElement.Attribute("add")?.Value;

                        if (string.IsNullOrEmpty(weaponId))
                            continue;

                        if (bool.TryParse(addedWeapon, out bool weaponValidResult))
                        {
                            heroDataOverride.AddAddedWeapon(weaponId, weaponValidResult);

                            if (!weaponValidResult)
                                continue;
                        }

                        overrideElement = dataElement.Element("Override");
                        if (overrideElement != null)
                            weaponOverride.SetOverride(weaponId, overrideElement, heroDataOverride.PropertyWeaponOverrideMethodByWeaponId);
                        break;
                }
            }

            DataOverridesById[heroId] = heroDataOverride;
        }
    }
}
