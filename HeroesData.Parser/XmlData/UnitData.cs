using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class UnitData
    {
        private readonly GameData GameData;
        private readonly DefaultData DefaultData;
        private readonly Configuration Configuration;
        private readonly WeaponData WeaponData;
        private readonly ArmorData ArmorData;
        private readonly AbilityData AbilityData;
        private readonly BehaviorData BehaviorData;

        private readonly string ElementType = "CUnit";

        private Localization _localization = Localization.ENUS;
        private bool _isHeroParsing = false;
        private bool _isAbilityTypeFilterEnabled = false;
        private bool _isAbilityTierFilterEnabled = false;

        public UnitData(GameData gameData, DefaultData defaultData, Configuration configuration, WeaponData weaponData, ArmorData armorData, AbilityData abilityData, BehaviorData behaviorData)
        {
            GameData = gameData;
            DefaultData = defaultData;
            Configuration = configuration;
            WeaponData = weaponData;
            ArmorData = armorData;
            AbilityData = abilityData;
            BehaviorData = behaviorData;
        }

        public Localization Localization
        {
            get => _localization;
            set
            {
                _localization = value;
                AbilityData.Localization = value;
            }
        }

        public bool IsHeroParsing
        {
            get => _isHeroParsing;
            set
            {
                _isHeroParsing = value;
                WeaponData.IsHeroParsing = value;
            }
        }

        public bool IsAbilityTypeFilterEnabled
        {
            get => _isAbilityTypeFilterEnabled;
            set
            {
                _isAbilityTypeFilterEnabled = value;
                AbilityData.IsAbilityTypeFilterEnabled = value;
            }
        }

        public bool IsAbilityTierFilterEnabled
        {
            get => _isAbilityTierFilterEnabled;
            set
            {
                _isAbilityTierFilterEnabled = value;
                AbilityData.IsAbilityTierFilterEnabled = value;
            }
        }

        public void SetUnitData(Unit unit, XElement unitElement = null)
        {
            unitElement = unitElement ?? GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == unit.CUnitId));

            if (unitElement == null)
                return;

            if (unit == null)
                throw new ArgumentNullException();

            // parent lookup
            string parentValue = unitElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue) && parentValue != DefaultDataHero.CUnitDefaultBaseId)
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetUnitData(unit, parentElement);
            }

            // loop through all elements and set found elements
            foreach (XElement element in unitElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "LIFEMAX")
                {
                    unit.Life.LifeMax = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);

                    double? scaleValue = GameData.GetScaleValue(("Unit", unit.CUnitId, "LifeMax"));
                    if (scaleValue.HasValue)
                        unit.Life.LifeScaling = scaleValue.Value;
                }
                else if (elementName == "LIFEREGENRATE")
                {
                    unit.Life.LifeRegenerationRate = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);

                    double? scaleValue = GameData.GetScaleValue(("Unit", unit.CUnitId, "LifeRegenRate"));
                    if (scaleValue.HasValue)
                        unit.Life.LifeRegenerationRateScaling = scaleValue.Value;
                }
                else if (elementName == "RADIUS")
                {
                    unit.Radius = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "INNERRADIUS")
                {
                    unit.InnerRadius = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "ENERGYMAX")
                {
                    unit.Energy.EnergyMax = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "ENERGYREGENRATE")
                {
                    unit.Energy.EnergyRegenerationRate = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "SPEED")
                {
                    unit.Speed = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "SIGHT")
                {
                    unit.Sight = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "ATTRIBUTES")
                {
                    string enabled = element.Attribute("value")?.Value;
                    string attribute = element.Attribute("index").Value;

                    if (enabled == "0" && unit.Attributes.Contains(attribute))
                        unit.RemoveAttribute(attribute);
                    else if (enabled == "1")
                        unit.AddAttribute(attribute);
                }
                else if (elementName == "UNITDAMAGETYPE")
                {
                    unit.DamageType = element.Attribute("value").Value;
                }
                else if (elementName == "NAME")
                {
                    unit.Name = GameData.GetGameString(element.Attribute("value").Value);
                }
                else if (elementName == "DESCRIPTION")
                {
                    unit.Description = new TooltipDescription(GameData.GetGameString(element.Attribute("value").Value));
                }
                else if (elementName == "GENDER")
                {
                    if (Enum.TryParse(element.Attribute("value").Value, out UnitGender unitGender))
                        unit.Gender = unitGender;
                    else
                        unit.Gender = UnitGender.Neutral;
                }
                else if (elementName == "ABILARRAY")
                {
                    AddCreatedAbility(unit, element.Attribute("Link")?.Value);
                }
                else if (elementName == "WEAPONARRAY")
                {
                    UnitWeapon weapon = WeaponData.CreateWeapon(element);
                    if (weapon != null)
                        unit.AddUnitWeapon(weapon);
                }
                else if (elementName == "ARMORLINK")
                {
                    IEnumerable<UnitArmor> armorList = ArmorData.CreateArmorCollection(element);
                    if (armorList != null)
                    {
                        foreach (UnitArmor armor in armorList)
                        {
                            unit.AddUnitArmor(armor);
                        }
                    }
                }
                else if (elementName == "BEHAVIORARRAY")
                {
                    string link = BehaviorData.GetScalingBehaviorLink(element);
                    if (!string.IsNullOrEmpty(link))
                        unit.ScalingBehaviorLink = link;
                }
                else if (elementName == "HEROPLAYSTYLEFLAGS")
                {
                    string descriptor = element.Attribute("index").Value;

                    if (element.Attribute("value")?.Value == "1")
                        unit.AddHeroDescriptor(descriptor);
                }
                else if (elementName == "CARDLAYOUTS")
                {
                    foreach (XElement cardLayoutElement in element.Elements())
                    {
                        string cardLayoutElementName = cardLayoutElement.Name.LocalName.ToUpper();

                        if (cardLayoutElementName == "LAYOUTBUTTONS")
                        {
                            string indexValue = cardLayoutElement.Attribute("index")?.Value;
                            string faceValue = cardLayoutElement.Attribute("Face")?.Value;
                            string passiveValue = cardLayoutElement.Attribute("Type")?.Value;
                            string abilCmdValue = cardLayoutElement.Attribute("AbilCmd")?.Value;
                            string requirementsValue = cardLayoutElement.Attribute("Requirements")?.Value;

                            //if (string.IsNullOrEmpty(indexValue))
                            //{
                            //    LayoutButtonFaceByIndex.Add(CardLayoutButtonIndex, faceValue);
                            //    CardLayoutButtonIndex++;
                            //}
                            //else
                            //{

                            //}

                            //if (abilCmdValue.AsSpan().IsEmpty && passiveValue.AsSpan().Equals("passive", StringComparison.OrdinalIgnoreCase) && !requirementsValue.AsSpan().Equals("UltimateNotUnlocked", StringComparison.OrdinalIgnoreCase))
                            //{
                            //    AddCreatedAbility(unit, faceValue);
                            //}
                        }
                    }
                }
            }

            if (unit.Energy.EnergyMax < 1)
                unit.Energy.EnergyType = string.Empty;
        }

        private void AddCreatedAbility(Unit unit, string abilityId)
        {
            Ability ability = AbilityData.CreateAbility(unit.CUnitId, abilityId);
            if (ability != null)
            {
                unit.AddAbility(ability);

                foreach (string createUnit in ability.CreatedUnits)
                {
                    unit.AddUnit(createUnit);
                }
            }
        }
    }
}
