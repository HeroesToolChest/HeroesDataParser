using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class UnitData
    {
        private readonly GameData _gameData;
        private readonly Configuration _configuration;
        private readonly DefaultData _defaultData;
        private readonly WeaponData _weaponData;
        private readonly ArmorData _armorData;
        private readonly AbilityData _abilityData;

        private readonly string _elementType = "CUnit";
        private readonly string _cActorUnit = "CActorUnit";

        private XmlArrayElement? _abilitiesArray;
        private XmlArrayElement? _weaponsArray;
        private XmlArrayElement? _behaviorArray;
        private XmlArrayElement? _cardLayoutButtons;

        private HashSet<AbilityTalentId>? _additionalActorAbilities;

        private Localization _localization = Localization.ENUS;
        private bool _isHeroParsing = false;
        private bool _isAbilityTypeFilterEnabled = false;
        private bool _isAbilityTierFilterEnabled = false;

        public UnitData(GameData gameData, Configuration configuration, DefaultData defaultData, WeaponData weaponData, ArmorData armorData, AbilityData abilityData)
        {
            _gameData = gameData;
            _configuration = configuration;
            _defaultData = defaultData;
            _weaponData = weaponData;
            _armorData = armorData;
            _abilityData = abilityData;
        }

        public Localization Localization
        {
            get => _localization;
            set
            {
                _localization = value;
                _abilityData.Localization = value;
            }
        }

        public bool IsHeroParsing
        {
            get => _isHeroParsing;
            set
            {
                _isHeroParsing = value;
                _weaponData.IsHeroParsing = value;
            }
        }

        public bool IsAbilityTypeFilterEnabled
        {
            get => _isAbilityTypeFilterEnabled;
            set
            {
                _isAbilityTypeFilterEnabled = value;
                _abilityData.IsAbilityTypeFilterEnabled = value;
            }
        }

        public bool IsAbilityTierFilterEnabled
        {
            get => _isAbilityTierFilterEnabled;
            set
            {
                _isAbilityTierFilterEnabled = value;
                _abilityData.IsAbilityTierFilterEnabled = value;
            }
        }

        /// <summary>
        /// Sets the unit data.
        /// </summary>
        /// <param name="unit">The <see cref="Unit"/> object to update.</param>
        /// <param name="unitDataOverride">Optional override.</param>
        /// <param name="isHero">Value incdicating if this is hero data.</param>
        public void SetUnitData(Unit unit, UnitDataOverride? unitDataOverride = null, bool isHero = false)
        {
            if (unit is null)
                throw new ArgumentNullException(nameof(unit));

            _abilitiesArray = new XmlArrayElement();
            _weaponsArray = new XmlArrayElement();
            _behaviorArray = new XmlArrayElement();
            _cardLayoutButtons = new XmlArrayElement();
            _additionalActorAbilities = new HashSet<AbilityTalentId>();

            if (!isHero)
                SetDefaultValues(unit);

            XElement? actorElement = null;
            XElement? unitNameActorUnitElement = GameData.MergeXmlElements(_gameData.Elements(_cActorUnit).Where(x => x.Attribute("unitName")?.Value == unit.CUnitId));
            if (unitNameActorUnitElement != null)
                actorElement = GameData.MergeXmlElements(_gameData.Elements(_cActorUnit).Where(x => x.Attribute("id")?.Value == unitNameActorUnitElement.Attribute("id")?.Value));
            else
                actorElement = GameData.MergeXmlElements(_gameData.Elements(_cActorUnit).Where(x => x.Attribute("id")?.Value == unit.CUnitId));

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
            unit.Radius = _defaultData.UnitData!.UnitRadius;
            unit.Speed = _defaultData.UnitData.UnitSpeed;
            unit.Sight = _defaultData.UnitData.UnitSight;
            unit.Life.LifeMax = _defaultData.UnitData.UnitLifeMax;
            unit.Life.LifeRegenerationRate = 0;
            unit.Energy.EnergyMax = _defaultData.UnitData.UnitEnergyMax;
            unit.Energy.EnergyRegenerationRate = _defaultData.UnitData.UnitEnergyRegenRate;

            foreach (string unitAttribute in _defaultData.UnitData.UnitAttributes)
            {
                unit.Attributes.Add(unitAttribute);
            }

            unit.DamageType = _defaultData.UnitData.UnitDamageType;
            unit.Name = _gameData.GetGameString(_defaultData.UnitData.UnitName!.Replace(DefaultData.IdPlaceHolder, unit.Id, StringComparison.OrdinalIgnoreCase)).Trim();

            if (string.IsNullOrEmpty(unit.Energy.EnergyType))
                unit.Energy.EnergyType = _gameData.GetGameString(DefaultData.HeroEnergyTypeManaText);
        }

        private void SetData(Unit unit, XElement? unitElement = null)
        {
            unitElement ??= GameData.MergeXmlElements(_gameData.Elements(_elementType).Where(x => x.Attribute("id")?.Value == unit.CUnitId));
            if (unitElement == null)
                return;

            if (unit is null)
                throw new ArgumentNullException(nameof(unit));

            // parent lookup
            string? parentValue = unitElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(_gameData.Elements(_elementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetData(unit, parentElement);
            }

            // loop through all elements and set found elements
            foreach (XElement element in unitElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper(CultureInfo.InvariantCulture);

                if (elementName == "LIFEMAX")
                {
                    unit.Life.LifeMax = XmlParse.GetDoubleValue(unit.CUnitId, element, _gameData);

                    double? scaleValue = _gameData.GetScaleValue(("Unit", unit.CUnitId, element.Name.LocalName));
                    if (scaleValue.HasValue)
                        unit.Life.LifeScaling = scaleValue.Value;
                }
                else if (elementName == "LIFEREGENRATE")
                {
                    unit.Life.LifeRegenerationRate = XmlParse.GetDoubleValue(unit.CUnitId, element, _gameData);

                    double? scaleValue = _gameData.GetScaleValue(("Unit", unit.CUnitId, element.Name.LocalName));
                    if (scaleValue.HasValue)
                        unit.Life.LifeRegenerationRateScaling = scaleValue.Value;
                }
                else if (elementName == "SHIELDSMAX")
                {
                    unit.Shield.ShieldMax = XmlParse.GetDoubleValue(unit.CUnitId, element, _gameData);

                    double? scaleValue = _gameData.GetScaleValue(("Unit", unit.CUnitId, element.Name.LocalName));
                    if (scaleValue.HasValue)
                        unit.Shield.ShieldScaling = scaleValue.Value;
                }
                else if (elementName == "SHIELDREGENDELAY")
                {
                    unit.Shield.ShieldRegenerationDelay = XmlParse.GetDoubleValue(unit.CUnitId, element, _gameData);
                }
                else if (elementName == "SHIELDREGENRATE")
                {
                    unit.Shield.ShieldRegenerationRate = XmlParse.GetDoubleValue(unit.CUnitId, element, _gameData);

                    double? scaleValue = _gameData.GetScaleValue(("Unit", unit.CUnitId, element.Name.LocalName));
                    if (scaleValue.HasValue)
                        unit.Shield.ShieldRegenerationRateScaling = scaleValue.Value;
                }
                else if (elementName == "RADIUS")
                {
                    unit.Radius = XmlParse.GetDoubleValue(unit.CUnitId, element, _gameData);
                }
                else if (elementName == "INNERRADIUS")
                {
                    unit.InnerRadius = XmlParse.GetDoubleValue(unit.CUnitId, element, _gameData);
                }
                else if (elementName == "ENERGYMAX")
                {
                    unit.Energy.EnergyMax = XmlParse.GetDoubleValue(unit.CUnitId, element, _gameData);
                }
                else if (elementName == "ENERGYREGENRATE")
                {
                    unit.Energy.EnergyRegenerationRate = XmlParse.GetDoubleValue(unit.CUnitId, element, _gameData);
                }
                else if (elementName == "SPEED")
                {
                    unit.Speed = XmlParse.GetDoubleValue(unit.CUnitId, element, _gameData);
                }
                else if (elementName == "SIGHT")
                {
                    unit.Sight = XmlParse.GetDoubleValue(unit.CUnitId, element, _gameData);
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
                    unit.Name = _gameData.GetGameString(element.Attribute("value").Value);
                }
                else if (elementName == "DESCRIPTION")
                {
                    unit.Description = new TooltipDescription(_gameData.GetGameString(element.Attribute("value").Value));
                }
                else if (elementName == "INFOTEXT")
                {
                    unit.InfoText = new TooltipDescription(_gameData.GetGameString(element.Attribute("value").Value));
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
                    _weaponsArray?.AddElement(element);
                }
                else if (elementName == "ARMORLINK")
                {
                    foreach (UnitArmor armor in _armorData.CreateArmorCollection(element))
                    {
                        unit.Armor.Add(armor);
                    }
                }
                else if (elementName == "BEHAVIORARRAY")
                {
                    _behaviorArray?.AddElement(element);
                }
                else if (elementName == "HEROPLAYSTYLEFLAGS")
                {
                    string descriptor = element.Attribute("index").Value;

                    if (element.Attribute("value")?.Value == "1")
                        unit.HeroDescriptors.Add(descriptor);
                }
                else if (elementName == "KILLXP")
                {
                    unit.KillXP = XmlParse.GetIntValue(unit.CUnitId, element, _gameData);
                }
                else if (elementName == "ABILARRAY")
                {
                    _abilitiesArray?.AddElement(element);
                }
                else if (elementName == "CARDLAYOUTS")
                {
                    foreach (XElement cardLayoutElement in element.Elements())
                    {
                        string cardLayoutElementName = cardLayoutElement.Name.LocalName.ToUpperInvariant();

                        if (cardLayoutElementName == "LAYOUTBUTTONS")
                        {
                            _cardLayoutButtons?.AddElement(cardLayoutElement);
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
                XElement? parentElement = GameData.MergeXmlElements(_gameData.Elements(_cActorUnit).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetActorData(parentElement, unit, unitDataOverride);
            }

            // find special energy type
            foreach (XElement element in actorElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "VITALNAMES")
                {
                    string? indexValue = element.Attribute("index")?.Value?.ToUpperInvariant();
                    string? valueValue = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(indexValue) && valueValue != null)
                    {
                        string type = _gameData.GetGameString(valueValue);

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
                        _additionalActorAbilities?.Add(new AbilityTalentId(value, value));
                    }
                }
                else if (elementName == "GROUPICON")
                {
                    string? imageValue = element.Element("Image")?.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(imageValue))
                    {
                        string fileName = Path.GetFileName(PathHelper.GetFilePath(imageValue)).ToLowerInvariant();
                        if (!_configuration.ContainsDeadImageFileName(fileName))
                            unit.UnitPortrait.TargetInfoPanelFileName = fileName;
                    }
                }
                else if (elementName == "MINIMAPICON")
                {
                    string imageValue = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value ?? string.Empty));

                    if (!string.IsNullOrEmpty(imageValue))
                    {
                        string fileName = Path.GetFileName(PathHelper.GetFilePath(imageValue)).ToLowerInvariant();
                        if (!_configuration.ContainsDeadImageFileName(fileName))
                            unit.UnitPortrait.MiniMapIconFileName = fileName;
                    }
                }
            }
        }

        private void SetAbilities(Unit unit)
        {
            if (_abilitiesArray == null || _cardLayoutButtons == null)
                throw new NullReferenceException("Call SetData() first to set up the abilities and card layout button collections");

            foreach (XElement element in _cardLayoutButtons.Elements)
            {
                AddCreatedAbility(unit, element);
            }

            foreach (XElement element in _abilitiesArray.Elements)
            {
                string? linkValue = element.Attribute("Link")?.Value;
                if (!string.IsNullOrEmpty(linkValue) && !_configuration.ContainsIgnorableExtraAbility(linkValue) && !unit.GetAbilitiesFromReferenceId(linkValue, StringComparison.OrdinalIgnoreCase).Any())
                    AddCreatedAbility(unit, linkValue);
            }

            if (_additionalActorAbilities != null)
            {
                foreach (AbilityTalentId addedAbility in _additionalActorAbilities)
                {
                    AddCreatedAbility(unit, addedAbility);
                }
            }
        }

        private void SetWeapons(Unit unit)
        {
            if (_weaponsArray == null)
                throw new NullReferenceException("Call SetData() first to set up the weapons collections");

            foreach (XElement element in _weaponsArray.Elements)
            {
                string? weaponLink = element.Attribute("Link")?.Value;
                if (!string.IsNullOrEmpty(weaponLink))
                {
                    UnitWeapon? weapon = _weaponData.CreateWeapon(weaponLink);
                    if (weapon != null)
                        unit.Weapons.Add(weapon);
                }
            }
        }

        private void SetBehaviors(Unit unit)
        {
            if (_behaviorArray == null)
                throw new NullReferenceException("Call SetData() first to set up the behavior collections");

            foreach (XElement element in _behaviorArray.Elements)
            {
                string? linkValue = element.Attribute("Link")?.Value;

                if (!string.IsNullOrEmpty(linkValue))
                    ParseBehaviorLink(linkValue, unit);
            }
        }

        private void AddCreatedAbility(Unit unit, string abilLinkValue)
        {
            AddAbility(unit, _abilityData.CreateAbility(unit.CUnitId, abilLinkValue));
        }

        private void AddCreatedAbility(Unit unit, XElement layoutButtonElement)
        {
            AddAbility(unit, _abilityData.CreateAbility(unit.CUnitId, layoutButtonElement));
        }

        private void AddCreatedAbility(Unit unit, AbilityTalentId abilityTalentId)
        {
            AddAbility(unit, _abilityData.CreateAbility(unit.CUnitId, abilityTalentId));
        }

        private void AddAbility(Unit unit, Ability? ability)
        {
            if (ability != null)
            {
                if (!_configuration.ContainsIgnorableExtraAbility(ability.AbilityTalentId.ReferenceId))
                    unit.AddAbility(ability);

                foreach (string createUnit in ability.CreatedUnits)
                    unit.UnitIds.Add(createUnit);
            }
        }

        private void ParseBehaviorLink(string linkValue, Unit unit)
        {
            if (string.IsNullOrEmpty(linkValue))
                throw new ArgumentException("Argument cannot be null or empty", nameof(linkValue));

            XElement? behaviorVeterancyElement = GameData.MergeXmlElements(_gameData.Elements("CBehaviorVeterancy").Where(x => x.Attribute("id")?.Value == linkValue));
            if (behaviorVeterancyElement != null)
            {
                unit.ScalingBehaviorLink = linkValue;
                return;
            }

            XElement? behaviorAbilityElement = GameData.MergeXmlElements(_gameData.Elements("CBehaviorAbility").Where(x => x.Attribute("id")?.Value == linkValue));
            if (behaviorAbilityElement != null)
            {
                foreach (XElement buttonElement in behaviorAbilityElement.Elements("Buttons"))
                {
                    AddAbility(unit, _abilityData.CreateAbility(unit.CUnitId, buttonElement, true));
                }

                return;
            }
        }
    }
}
