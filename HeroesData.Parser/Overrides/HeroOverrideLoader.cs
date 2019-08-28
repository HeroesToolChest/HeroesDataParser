using Heroes.Models.AbilityTalents;
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

            AbilityPropertyOverride abilityOverride = new AbilityPropertyOverride();
            TalentPropertyOverride talentOverride = new TalentPropertyOverride();
            WeaponPropertyOverride weaponOverride = new WeaponPropertyOverride();
            PortraitPropertyOverride portraitOverride = new PortraitPropertyOverride();

            string heroId = element.Attribute("id").Value;

            foreach (XElement dataElement in element.Elements())
            {
                string elementName = dataElement.Name.LocalName;
                string valueAttribute = dataElement.Attribute("value")?.Value;

                XElement overrideElement = null;

                switch (elementName)
                {
                    case "Name":
                        if (!string.IsNullOrEmpty(valueAttribute))
                            heroDataOverride.NameOverride = (true, valueAttribute);
                        break;
                    case "HyperlinkId":
                        if (!string.IsNullOrEmpty(valueAttribute))
                            heroDataOverride.HyperlinkIdOverride = (true, valueAttribute);
                        break;
                    case "CUnit":
                        if (!string.IsNullOrEmpty(valueAttribute))
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
                        heroDataOverride.ParentLinkOverride = (true, valueAttribute);
                        break;
                    case "Ability":
                        string abilityId = dataElement.Attribute("id")?.Value ?? string.Empty;
                        string buttonAbilityId = dataElement.Attribute("button")?.Value ?? abilityId;
                        string passiveAbility = dataElement.Attribute("passive")?.Value;
                        string addedAbility = dataElement.Attribute("add")?.Value;

                        if (bool.TryParse(passiveAbility, out bool abilityPassiveResult))
                        {
                            buttonAbilityId = $"{buttonAbilityId}~Passive~";
                        }

                        if (bool.TryParse(addedAbility, out bool abilityAddedResult))
                        {
                            heroDataOverride.AddAddedAbility(new AbilityTalentId(abilityId, buttonAbilityId), abilityAddedResult);

                            if (!abilityAddedResult)
                                continue;
                        }

                        if (!string.IsNullOrEmpty(abilityId))
                        {
                            overrideElement = dataElement.Element("Override");

                            if (overrideElement != null)
                                abilityOverride.SetOverride(new AbilityTalentId(abilityId, buttonAbilityId), overrideElement, heroDataOverride.PropertyAbilityOverrideMethodByAbilityId);
                        }

                        break;
                    case "Talent":
                        string talentId = dataElement.Attribute("id")?.Value;
                        string buttonTalentId = dataElement.Attribute("button")?.Value ?? talentId;

                        if (string.IsNullOrEmpty(talentId))
                            continue;

                        // override
                        overrideElement = dataElement.Element("Override");
                        if (overrideElement != null)
                            talentOverride.SetOverride(new AbilityTalentId(talentId, buttonTalentId), overrideElement, heroDataOverride.PropertyTalentOverrideMethodByTalentId);
                        break;
                    case "Portrait":
                        overrideElement = dataElement.Element("Override");
                        if (overrideElement != null)
                            portraitOverride.SetOverride(heroId, overrideElement, heroDataOverride.PropertyPortraitOverrideMethodByHeroId);
                        break;
                    case "HeroUnit":
                        string heroUnitId = dataElement.Attribute("id")?.Value;
                        string removeHeroUnit = dataElement.Attribute("remove")?.Value;

                        if (bool.TryParse(removeHeroUnit, out bool heroUnitRemoveResult))
                        {
                            if (heroUnitRemoveResult)
                            {
                                heroDataOverride.AddRemovedHeroUnit(heroUnitId);
                                continue;
                            }
                        }

                        if (string.IsNullOrEmpty(heroUnitId))
                            continue;

                        heroDataOverride.AddHeroUnit(heroUnitId);
                        SetOverride(dataElement);

                        break;
                    case "Weapon":
                        string weaponId = dataElement.Attribute("id")?.Value;
                        string validWeapon = dataElement.Attribute("valid")?.Value;

                        if (string.IsNullOrEmpty(weaponId))
                            continue;

                        if (bool.TryParse(validWeapon, out bool weaponValidResult))
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
