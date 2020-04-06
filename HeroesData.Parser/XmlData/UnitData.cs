using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class UnitData
    {
        private readonly GameData GameData;
        private readonly Configuration Configuration;
        private readonly DefaultData DefaultData;
        private readonly WeaponData WeaponData;
        private readonly ArmorData ArmorData;
        private readonly AbilityData AbilityData;

        private readonly string ElementType = "CUnit";
        private readonly string CActorUnit = "CActorUnit";

        private XmlArrayElement? AbilitiesArray;
        private XmlArrayElement? WeaponsArray;
        private XmlArrayElement? BehaviorArray;
        private XmlArrayElement? CardLayoutButtons;

        private HashSet<AbilityTalentId>? AdditionalActorAbilities;

        private Localization _localization = Localization.ENUS;
        private bool _isHeroParsing = false;
        private bool _isAbilityTypeFilterEnabled = false;
        private bool _isAbilityTierFilterEnabled = false;

        public UnitData(GameData gameData, Configuration configuration, DefaultData defaultData, WeaponData weaponData, ArmorData armorData, AbilityData abilityData)
        {
            GameData = gameData;
            Configuration = configuration;
            DefaultData = defaultData;
            WeaponData = weaponData;
            ArmorData = armorData;
            AbilityData = abilityData;
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
        /// <param name="unitDataOverride"></param>
        /// <param name="isHero"></param>
        public void SetUnitData(Unit unit, UnitDataOverride? unitDataOverride = null, bool isHero = false)
        {
            AbilitiesArray = new XmlArrayElement();
            WeaponsArray = new XmlArrayElement();
            BehaviorArray = new XmlArrayElement();
            CardLayoutButtons = new XmlArrayElement();
            AdditionalActorAbilities = new HashSet<AbilityTalentId>();

            if (!isHero)
                SetDefaultValues(unit);

            XElement? actorElement = null;
            XElement? unitNameActorUnitElement = GameData.MergeXmlElements(GameData.Elements(CActorUnit).Where(x => x.Attribute("unitName")?.Value == unit.CUnitId));
            if (unitNameActorUnitElement != null)
                actorElement = GameData.MergeXmlElements(GameData.Elements(CActorUnit).Where(x => x.Attribute("id")?.Value == unitNameActorUnitElement.Attribute("id")?.Value));
            else
                actorElement = GameData.MergeXmlElements(GameData.Elements(CActorUnit).Where(x => x.Attribute("id")?.Value == unit.CUnitId));

            if (actorElement != null)
                SetActorData(actorElement, unit, unitDataOverride);

            SetData(unit);
            SetAbilities(unit);
            SetWeapons(unit);
            SetBehaviors(unit);

            if (unit.Energy.EnergyMax < 1)
                unit.Energy.EnergyType = string.Empty;
        }

        private void SetDefaultValues(Unit unit)
        {
            unit.Radius = DefaultData.UnitData!.UnitRadius;
            unit.Speed = DefaultData.UnitData.UnitSpeed;
            unit.Sight = DefaultData.UnitData.UnitSight;
            unit.Life.LifeMax = DefaultData.UnitData.UnitLifeMax;
            unit.Life.LifeRegenerationRate = 0;
            unit.Energy.EnergyMax = DefaultData.UnitData.UnitEnergyMax;
            unit.Energy.EnergyRegenerationRate = DefaultData.UnitData.UnitEnergyRegenRate;

            foreach (string unitAttribute in DefaultData.UnitData.UnitAttributes)
            {
                unit.Attributes.Add(unitAttribute);
            }

            unit.DamageType = DefaultData.UnitData.UnitDamageType;
            unit.Name = GameData.GetGameString(DefaultData.UnitData.UnitName!.Replace(DefaultData.IdPlaceHolder, unit.Id)).Trim();

            if (string.IsNullOrEmpty(unit.Energy.EnergyType))
                unit.Energy.EnergyType = GameData.GetGameString(DefaultData.HeroEnergyTypeManaText);
        }

        private void SetData(Unit unit, XElement? unitElement = null)
        {
            unitElement ??= GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == unit.CUnitId));
            if (unitElement == null)
                return;

            if (unit == null)
                throw new ArgumentNullException();

            // parent lookup
            string? parentValue = unitElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
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

                    double? scaleValue = GameData.GetScaleValue(("Unit", unit.CUnitId, element.Name.LocalName));
                    if (scaleValue.HasValue)
                        unit.Life.LifeScaling = scaleValue.Value;
                }
                else if (elementName == "LIFEREGENRATE")
                {
                    unit.Life.LifeRegenerationRate = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);

                    double? scaleValue = GameData.GetScaleValue(("Unit", unit.CUnitId, element.Name.LocalName));
                    if (scaleValue.HasValue)
                        unit.Life.LifeRegenerationRateScaling = scaleValue.Value;
                }
                else if (elementName == "SHIELDSMAX")
                {
                    unit.Shield.ShieldMax = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);

                    double? scaleValue = GameData.GetScaleValue(("Unit", unit.CUnitId, element.Name.LocalName));
                    if (scaleValue.HasValue)
                        unit.Shield.ShieldScaling = scaleValue.Value;
                }
                else if (elementName == "SHIELDREGENDELAY")
                {
                    unit.Shield.ShieldRegenerationDelay = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "SHIELDREGENRATE")
                {
                    unit.Shield.ShieldRegenerationRate = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);

                    double? scaleValue = GameData.GetScaleValue(("Unit", unit.CUnitId, element.Name.LocalName));
                    if (scaleValue.HasValue)
                        unit.Shield.ShieldRegenerationRateScaling = scaleValue.Value;
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
                    string? enabled = element.Attribute("value")?.Value;
                    string attribute = element.Attribute("index").Value;

                    if (enabled == "0" && unit.Attributes.Contains(attribute))
                        unit.Attributes.Remove(attribute);
                    else if (enabled == "1")
                        unit.Attributes.Add(attribute);
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
                    WeaponsArray?.AddElement(element);
                }
                else if (elementName == "ARMORLINK")
                {
                    foreach (UnitArmor armor in ArmorData.CreateArmorCollection(element))
                    {
                        unit.Armor.Add(armor);
                    }
                }
                else if (elementName == "BEHAVIORARRAY")
                {
                    BehaviorArray?.AddElement(element);
                }
                else if (elementName == "HEROPLAYSTYLEFLAGS")
                {
                    string descriptor = element.Attribute("index").Value;

                    if (element.Attribute("value")?.Value == "1")
                        unit.HeroDescriptors.Add(descriptor);
                }
                else if (elementName == "KILLXP")
                {
                    unit.KillXP = XmlParse.GetIntValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "ABILARRAY")
                {
                    AbilitiesArray?.AddElement(element);
                }
                else if (elementName == "CARDLAYOUTS")
                {
                    foreach (XElement cardLayoutElement in element.Elements())
                    {
                        string cardLayoutElementName = cardLayoutElement.Name.LocalName.ToUpper();

                        if (cardLayoutElementName == "LAYOUTBUTTONS")
                        {
                            CardLayoutButtons?.AddElement(cardLayoutElement);
                        }
                    }
                }
            }
        }

        private void SetActorData(XElement actorElement, Unit unit, UnitDataOverride? unitDataOverride)
        {
            if (actorElement == null || unit == null)
                return;

            // parent lookup
            string? parentValue = actorElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue) && parentValue != "StormUnitBase")
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(CActorUnit).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetActorData(parentElement, unit, unitDataOverride);
            }

            // find special energy type
            foreach (XElement element in actorElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "VITALNAMES")
                {
                    string? indexValue = element.Attribute("index")?.Value?.ToUpper();
                    string? valueValue = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(indexValue) && valueValue != null)
                    {
                        string type = GameData.GetGameString(valueValue);

                        if (indexValue == "ENERGY")
                            unit.Energy.EnergyType = type;
                        else if (indexValue == "LIFE")
                            unit.Life.LifeType = type;
                        else if (indexValue == "SHIELDS")
                            unit.Shield.ShieldType = type;
                    }
                }
                else if (elementName == "UNITBUTTON" || elementName == "UNITBUTTONMULTIPLE")
                {
                    string? value = element.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(value) && unitDataOverride != null)
                    {
                        AdditionalActorAbilities?.Add(new AbilityTalentId(value, value));
                    }
                }
                else if (elementName == "GROUPICON")
                {
                    string? imageValue = element.Element("Image")?.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(imageValue))
                    {
                        string fileName = Path.GetFileName(PathHelper.GetFilePath(imageValue)).ToLower();
                        if (!Configuration.ContainsDeadImageFileName(fileName))
                            unit.UnitPortrait.TargetInfoPanelFileName = fileName;
                    }
                }
                else if (elementName == "MINIMAPICON")
                {
                    string imageValue = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value ?? string.Empty));

                    if (!string.IsNullOrEmpty(imageValue))
                    {
                        string fileName = Path.GetFileName(PathHelper.GetFilePath(imageValue)).ToLower();
                        if (!Configuration.ContainsDeadImageFileName(fileName))
                            unit.UnitPortrait.MiniMapIconFileName = fileName;
                    }
                }
            }
        }

        private void SetAbilities(Unit unit)
        {
            if (AbilitiesArray == null || CardLayoutButtons == null)
                throw new ArgumentNullException("Call SetData() first to set up the abilities and card layout button collections");

            foreach (XElement element in CardLayoutButtons.Elements)
            {
                AddCreatedAbility(unit, element);
            }

            foreach (XElement element in AbilitiesArray.Elements)
            {
                string? linkValue = element.Attribute("Link")?.Value;
                if (!string.IsNullOrEmpty(linkValue) && !Configuration.ContainsIgnorableExtraAbility(linkValue) && !unit.GetAbilitiesFromReferenceId(linkValue, StringComparison.OrdinalIgnoreCase).Any())
                    AddCreatedAbility(unit, linkValue);
            }

            if (AdditionalActorAbilities != null)
            {
                foreach (AbilityTalentId addedAbility in AdditionalActorAbilities)
                {
                    AddCreatedAbility(unit, addedAbility);
                }
            }
        }

        private void SetWeapons(Unit unit)
        {
            if (WeaponsArray == null)
                throw new ArgumentNullException("Call SetData() first to set up the weapons collections");

            foreach (XElement element in WeaponsArray.Elements)
            {
                string? weaponLink = element.Attribute("Link")?.Value;
                if (!string.IsNullOrEmpty(weaponLink))
                {
                    UnitWeapon? weapon = WeaponData.CreateWeapon(weaponLink);
                    if (weapon != null)
                        unit.Weapons.Add(weapon);
                }
            }
        }

        private void SetBehaviors(Unit unit)
        {
            if (BehaviorArray == null)
                throw new ArgumentNullException("Call SetData() first to set up the behavior collections");

            foreach (XElement element in BehaviorArray.Elements)
            {
                string? linkValue = element.Attribute("Link")?.Value;

                if (!string.IsNullOrEmpty(linkValue))
                    ParseBehaviorLink(linkValue, unit);
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

        private void AddCreatedAbility(Unit unit, AbilityTalentId abilityTalentId)
        {
            AddAbility(unit, AbilityData.CreateAbility(unit.CUnitId, abilityTalentId));
        }

        private void AddAbility(Unit unit, Ability? ability)
        {
            if (ability != null)
            {
                if (!Configuration.ContainsIgnorableExtraAbility(ability.AbilityTalentId.ReferenceId))
                    unit.AddAbility(ability);

                foreach (string createUnit in ability.CreatedUnits)
                    unit.UnitIds.Add(createUnit);
            }
        }

        private void ParseBehaviorLink(string linkValue, Unit unit)
        {
            if (string.IsNullOrEmpty(linkValue))
                throw new ArgumentException("Argument cannot be null or empty", nameof(linkValue));

            XElement? behaviorVeterancyElement = GameData.MergeXmlElements(GameData.Elements("CBehaviorVeterancy").Where(x => x.Attribute("id")?.Value == linkValue));
            if (behaviorVeterancyElement != null)
            {
                unit.ScalingBehaviorLink = linkValue;
                return;
            }

            XElement? behaviorAbilityElement = GameData.MergeXmlElements(GameData.Elements("CBehaviorAbility").Where(x => x.Attribute("id")?.Value == linkValue));
            if (behaviorAbilityElement != null)
            {
                foreach (XElement buttonElement in behaviorAbilityElement.Elements("Buttons"))
                {
                    AddAbility(unit, AbilityData.CreateAbility(unit.CUnitId, buttonElement, true));
                }

                return;
            }
        }
    }
}
