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
        private readonly Configuration Configuration;
        private readonly WeaponData WeaponData;
        private readonly ArmorData ArmorData;
        private readonly AbilityData AbilityData;

        private readonly string ElementType = "CUnit";
        private readonly HashSet<string> BasicAbilities;

        private XmlArrayElement AbilitiesArray;
        private XmlArrayElement CardLayoutButtons;

        private Localization _localization = Localization.ENUS;
        private bool _isHeroParsing = false;
        private bool _isAbilityTypeFilterEnabled = false;
        private bool _isAbilityTierFilterEnabled = false;

        public UnitData(GameData gameData, Configuration configuration, WeaponData weaponData, ArmorData armorData, AbilityData abilityData)
        {
            GameData = gameData;
            Configuration = configuration;
            WeaponData = weaponData;
            ArmorData = armorData;
            AbilityData = abilityData;

            BasicAbilities = Configuration.UnitDataExtraAbilities.ToHashSet();
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

        /// <summary>
        /// Sets the unit data.
        /// </summary>
        /// <param name="unit"></param>
        public void SetUnitData(Unit unit)
        {
            SetData(unit);
            SetAbilities(unit);
        }

        private void SetData(Unit unit, XElement unitElement = null)
        {
            unitElement = unitElement ?? GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == unit.CUnitId));
            AbilitiesArray = AbilitiesArray ?? new XmlArrayElement();
            CardLayoutButtons = CardLayoutButtons ?? new XmlArrayElement();

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
                    SetData(unit, parentElement);
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
                    string link = GetScalingBehaviorLink(element);
                    if (!string.IsNullOrEmpty(link))
                        unit.ScalingBehaviorLink = link;
                }
                else if (elementName == "HEROPLAYSTYLEFLAGS")
                {
                    string descriptor = element.Attribute("index").Value;

                    if (element.Attribute("value")?.Value == "1")
                        unit.AddHeroDescriptor(descriptor);
                }
                else if (elementName == "KILLXP")
                {
                    unit.KillXP = XmlParse.GetIntValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "ABILARRAY")
                {
                    AbilitiesArray.AddElement(element);
                }
                else if (elementName == "CARDLAYOUTS")
                {
                    foreach (XElement cardLayoutElement in element.Elements())
                    {
                        string cardLayoutElementName = cardLayoutElement.Name.LocalName.ToUpper();

                        if (cardLayoutElementName == "LAYOUTBUTTONS")
                        {
                            CardLayoutButtons.AddElement(cardLayoutElement);
                        }
                    }
                }
            }

            if (unit.Energy.EnergyMax < 1)
                unit.Energy.EnergyType = string.Empty;
        }

        private void SetAbilities(Unit unit)
        {
            if (AbilitiesArray == null || CardLayoutButtons == null)
                throw new ArgumentNullException("Call SetData() first to set up the abilities and card layout button collections");

            foreach (XElement element in CardLayoutButtons.Elements)
            {
                string indexValue = element.Attribute("index")?.Value;
                string faceValue = element.Attribute("Face")?.Value;
                string passiveValue = element.Attribute("Type")?.Value;
                string abilCmdValue = element.Attribute("AbilCmd")?.Value;
                string requirementsValue = element.Attribute("Requirements")?.Value;

                if (BasicAbilities.Contains(faceValue, StringComparer.OrdinalIgnoreCase))
                    continue;

                AddCreatedAbility(unit, element);
            }

            foreach (XElement element in AbilitiesArray.Elements)
            {
                string linkValue = element.Attribute("Link")?.Value;
                if (!string.IsNullOrEmpty(linkValue) && !BasicAbilities.Contains(linkValue, StringComparer.OrdinalIgnoreCase) && !unit.ContainsAbility(linkValue))
                    AddCreatedAbility(unit, linkValue);
            }
        }

        private void AddCreatedAbility(Unit unit, string abilLinkValue)
        {
            AddAbility(unit, AbilityData.CreateAbility(unit.CUnitId, abilLinkValue));
        }

        private void AddCreatedAbility(Unit unit, XElement layoutButtonElement)
        {
            AddAbility(unit, AbilityData.CreateAbility(unit.CUnitId, layoutButtonElement));
        }

        private void AddAbility(Unit unit, Ability ability)
        {
            if (ability != null)
            {
                if (!BasicAbilities.Contains(ability.ReferenceNameId, StringComparer.OrdinalIgnoreCase))
                    unit.AddAbility(ability);

                foreach (string createUnit in ability.CreatedUnits)
                    unit.AddUnit(createUnit);
            }
        }

        private string GetScalingBehaviorLink(XElement behaviorArrayElement)
        {
            string behaviorLink = behaviorArrayElement.Attribute("Link")?.Value;
            if (string.IsNullOrEmpty(behaviorLink))
                return string.Empty;

            XElement behaviorVeterancyElement = GameData.MergeXmlElements(GameData.Elements("CBehaviorVeterancy").Where(x => x.Attribute("id")?.Value == behaviorLink));
            if (behaviorVeterancyElement != null)
                return behaviorLink;

            return string.Empty;
        }
    }
}
