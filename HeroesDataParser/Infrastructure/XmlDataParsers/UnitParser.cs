using Heroes.Element.Models;
using Heroes.Element.Models.AbilityTalents;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class UnitParser : DataParser<Unit>
{
    private const string _actorDataObjectType = "Actor";

    private readonly ILogger<UnitParser> _logger;
    private readonly HeroesData _heroesData;
    private readonly IAbilityParser _abilityParser;

    public UnitParser(ILogger<UnitParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService, IAbilityParser abilityParser)
        : base(logger, heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
        _abilityParser = abilityParser;
    }

    public override string DataObjectType => "Unit";

    protected override void SetProperties(Unit elementObject, StormElement stormElement)
    {
        SetActorData(elementObject);
        SetUnitData(elementObject, stormElement);
        SetArmorData(elementObject, stormElement);
        SetWeaponData(elementObject, stormElement);
        SetAbilityData(elementObject, stormElement);

        ParseBehaviorLink(elementObject, stormElement);
    }

    private void SetActorData(Unit elementObject)
    {
        string? unitId = _heroesData.GetStormElementIdByUnitName(elementObject.Id, _actorDataObjectType);

        StormElement? actorElement;
        if (!string.IsNullOrEmpty(unitId))
        {
            actorElement = _heroesData.GetCompleteStormElement(_actorDataObjectType, unitId);
        }
        else
        {
            actorElement = _heroesData.GetCompleteStormElement(_actorDataObjectType, elementObject.Id);
        }

        if (actorElement is null)
        {
            _logger.LogTrace("Actor element not found for Unit {Id}", elementObject.Id);
            return;
        }

        if (actorElement.DataValues.TryGetElementDataAt("VitalNames", out StormElementData? vitalNamesData))
        {
            if (vitalNamesData.TryGetElementDataAt("Life", out StormElementData? lifeData))
                elementObject.Life.LifeType = GetTooltipDescriptionFromId(lifeData.Value.GetString());
            if (vitalNamesData.TryGetElementDataAt("Shields", out StormElementData? shieldsData))
                elementObject.Shield.ShieldType = GetTooltipDescriptionFromId(shieldsData.Value.GetString());
            if (vitalNamesData.TryGetElementDataAt("Energy", out StormElementData? energyData))
                elementObject.Energy.EnergyType = GetTooltipDescriptionFromId(energyData.Value.GetString());
        }

        //// TODO additional actor abilities

        if (actorElement.DataValues.TryGetElementDataAt("GroupIcon", out StormElementData? groupIconData) && groupIconData.TryGetElementDataAt("Image", out StormElementData? groupImageData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(groupImageData);
            if (imageFilePath is not null)
            {
                elementObject.Portraits.TargetInfoPanel = imageFilePath.Image;
                elementObject.Portraits.TargetInfoPanelPath = imageFilePath.FilePath;
            }
        }

        if (actorElement.DataValues.TryGetElementDataAt("MiniMapIcon", out StormElementData? miniMapIconData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(miniMapIconData);
            if (imageFilePath is not null)
            {
                elementObject.Portraits.MiniMapIcon = imageFilePath.Image;
                elementObject.Portraits.MiniMapIconPath = imageFilePath.FilePath;
            }
        }
    }

    private void SetUnitData(Unit elementObject, StormElement stormElement)
    {
        StormElementData elementData = stormElement.DataValues;

        SetNameProperty(elementObject, stormElement);
        SetDescriptionProperty(elementObject, stormElement);

        if (elementData.TryGetElementDataAt("LifeMax", out StormElementData? lifeMaxData))
        {
            elementObject.Life.LifeMax = lifeMaxData.Value.GetDouble();
            elementObject.Life.LifeMaxScaling = GetScaleValue(stormElement.ElementType, elementObject.Id, lifeMaxData.Field);
        }

        if (elementData.TryGetElementDataAt("LifeRegenRate", out StormElementData? lifeRegenRateData))
        {
            elementObject.Life.LifeRegenerationRate = lifeRegenRateData.Value.GetDouble();
            elementObject.Life.LifeRegenerationRateScaling = GetScaleValue(stormElement.ElementType, elementObject.Id, lifeRegenRateData.Field);
        }

        if (elementData.TryGetElementDataAt("ShieldsMax", out StormElementData? shieldsMaxData))
        {
            elementObject.Shield.ShieldMax = shieldsMaxData.Value.GetDouble();
            elementObject.Shield.ShieldMaxScaling = GetScaleValue(stormElement.ElementType, elementObject.Id, shieldsMaxData.Field);
        }

        if (elementData.TryGetElementDataAt("ShieldsRegenDelay", out StormElementData? shieldsRegenDelayData))
            elementObject.Shield.ShieldRegenerationDelay = shieldsRegenDelayData.Value.GetDouble();

        if (elementData.TryGetElementDataAt("ShieldRegenRate", out StormElementData? shieldRegenRateData))
        {
            elementObject.Shield.ShieldRegenerationRate = shieldRegenRateData.Value.GetDouble();
            elementObject.Shield.ShieldRegenerationRateScaling = GetScaleValue(stormElement.ElementType, elementObject.Id, shieldRegenRateData.Field);
        }

        if (elementData.TryGetElementDataAt("EnergyMax", out StormElementData? energyMaxData))
            elementObject.Energy.EnergyMax = energyMaxData.Value.GetDouble();

        if (elementData.TryGetElementDataAt("EnergyRegenRate", out StormElementData? energyRegenRateData))
            elementObject.Energy.EnergyRegenerationRate = energyRegenRateData.Value.GetDouble();

        if (elementData.TryGetElementDataAt("InnerRadius", out StormElementData? innerRadiusData))
            elementObject.InnerRadius = innerRadiusData.Value.GetDouble();

        if (elementData.TryGetElementDataAt("Radius", out StormElementData? radiusData))
            elementObject.Radius = radiusData.Value.GetDouble();

        if (elementData.TryGetElementDataAt("Sight", out StormElementData? sightData))
            elementObject.Sight = sightData.Value.GetDouble();

        if (elementData.TryGetElementDataAt("Speed", out StormElementData? speedData))
            elementObject.Speed = speedData.Value.GetDouble();

        if (elementData.TryGetElementDataAt("UnitDamageType", out StormElementData? unitDamageTypeData))
        {
            string unitDamageTypeValue = unitDamageTypeData.Value.GetString();
            if (Enum.TryParse(unitDamageTypeValue, true, out ArmorSet armorSet))
            {
                elementObject.DamageType = armorSet;
            }
            else
            {
                elementObject.DamageType = ArmorSet.Unknown;
                _logger.LogWarning("Unknown armor set type {ArmorSet} for damage type", unitDamageTypeData.Value.GetString());
            }
        }

        if (elementData.TryGetElementDataAt("KillXp", out StormElementData? killXpData))
            elementObject.KillXP = killXpData.Value.GetInt();

        if (elementData.TryGetElementDataAt("HeroPlaystyleFlags", out StormElementData? heroPlaystyleFlagsData))
        {
            foreach (string flag in heroPlaystyleFlagsData.GetElementDataIndexes())
            {
                int value = heroPlaystyleFlagsData.GetElementDataAt(flag).Value.GetInt();
                if (value == 1)
                    elementObject.HeroPlayStyles.Add(flag);
                else if (value == 0)
                    elementObject.HeroPlayStyles.Remove(flag);
            }
        }

        if (elementData.TryGetElementDataAt("Attributes", out StormElementData? attributesData))
        {
            foreach (string attribute in attributesData.GetElementDataIndexes())
            {
                int value = attributesData.GetElementDataAt(attribute).Value.GetInt();
                if (value == 1)
                    elementObject.Attributes.Add(attribute);
                else if (value == 0)
                    elementObject.Attributes.Remove(attribute);
            }
        }

        if (elementData.TryGetElementDataAt("Gender", out StormElementData? genderData))
        {
            string genderValue = genderData.Value.GetString();

            if (Enum.TryParse(genderValue, out Gender genderResult))
                elementObject.Gender = genderResult;
            else
                _logger.LogWarning("Unknown gender {Gender}", genderValue);
        }
    }

    private void ParseBehaviorLink(Unit elementObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("BehaviorArray", out StormElementData? behaviorArrayData))
        {
            foreach (string item in behaviorArrayData.GetElementDataIndexes())
            {
                if (behaviorArrayData.GetElementDataAt(item).TryGetElementDataAt("link", out StormElementData? linkData))
                {
                    string linkId = linkData.Value.GetString();

                    StormElement? behaviorStormElement = _heroesData.GetCompleteStormElement("Behavior", linkId);

                    if (behaviorStormElement is not null && behaviorStormElement.ElementType.Equals("CBehaviorVeterancy", StringComparison.OrdinalIgnoreCase))
                    {
                        elementObject.ScalingLinkIds.Add(linkId);
                    }

                    // TODO: CBehaviorAbility, Buttons, abilities?
                }
            }
        }
    }

    private void SetArmorData(Unit elementObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("ArmorLink", out StormElementData? armorLinkData))
        {
            StormElement? armorStormElement = _heroesData.GetCompleteStormElement("Armor", armorLinkData.Value.GetString());

            if (armorStormElement is null)
                return;

            // <ArmorSet index="Hero">
            if (armorStormElement.DataValues.TryGetElementDataAt("ArmorSet", out StormElementData? armorSetData))
            {
                foreach (string type in armorSetData.GetElementDataIndexes())
                {
                    StormElementData typeData = armorSetData.GetElementDataAt(type);

                    // <ArmorMitigationTable index="Ability" value="15" />
                    if (typeData.TryGetElementDataAt("ArmorMitigationTable", out StormElementData? armorMitigationTableData))
                    {
                        foreach (string damageType in armorMitigationTableData.GetElementDataIndexes())
                        {
                            UnitArmor unitArmor = new();

                            double armorValue = armorMitigationTableData.GetElementDataAt(damageType).Value.GetDouble();

                            if (damageType.Equals("basic", StringComparison.OrdinalIgnoreCase))
                                unitArmor.BasicArmor = armorValue;
                            else if (damageType.Equals("ability", StringComparison.OrdinalIgnoreCase))
                                unitArmor.AbilityArmor = armorValue;
                            else if (damageType.Equals("splash", StringComparison.OrdinalIgnoreCase))
                                unitArmor.SplashArmor = armorValue;

                            if (Enum.TryParse(type, true, out ArmorSet armorSet))
                            {
                                elementObject.Armor[armorSet] = unitArmor;
                            }
                            else
                            {
                                _logger.LogWarning("Unknown armor set type {ArmorSet}", type);
                                elementObject.Armor[ArmorSet.Unknown] = unitArmor;
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetWeaponData(Unit elementObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("WeaponArray", out StormElementData? weaponArrayData))
        {
            foreach (string item in weaponArrayData.GetElementDataIndexes())
            {
                if (weaponArrayData.GetElementDataAt(item).TryGetElementDataAt("link", out StormElementData? linkData))
                {
                    string linkId = linkData.Value.GetString();

                    StormElement? weaponStormElement = _heroesData.GetCompleteStormElement("Weapon", linkId);

                    if (weaponStormElement is null)
                    {
                        _logger.LogWarning("Weapon element does not exist for id {LinkId}", linkId);
                        continue;
                    }

                    UnitWeapon unitWeapon = new()
                    {
                        WeaponNameId = linkId,
                    };

                    if (weaponStormElement.DataValues.TryGetElementDataAt("Name", out StormElementData? nameData))
                        unitWeapon.Name = GetTooltipDescriptionFromId(nameData.Value.GetString());

                    if (weaponStormElement.DataValues.TryGetElementDataAt("Range", out StormElementData? rangeData))
                        unitWeapon.Range = rangeData.Value.GetDouble();

                    if (weaponStormElement.DataValues.TryGetElementDataAt("Period", out StormElementData? periodData))
                        unitWeapon.Period = periodData.Value.GetDouble();

                    if (weaponStormElement.DataValues.TryGetElementDataAt("Options", out StormElementData? optionsData))
                    {
                        if (optionsData.TryGetElementDataAt("Disabled", out StormElementData? disabledData) && disabledData.Value.GetInt() == 1)
                        {
                            //// TODO: Disabled weapons
                        }
                    }

                    if (weaponStormElement.DataValues.TryGetElementDataAt("DisplayEffect", out StormElementData? displayEffectData))
                    {
                        string displayEffectId = displayEffectData.Value.GetString();
                        StormElement? effectStormElement = _heroesData.GetCompleteStormElement("Effect", displayEffectId);

                        if (effectStormElement is null)
                        {
                            _logger.LogWarning("Effect element does not exist for id {DisplayEffectId}", displayEffectId);
                        }
                        else
                        {
                            if (effectStormElement.DataValues.TryGetElementDataAt("Amount", out StormElementData? amountData))
                            {
                                unitWeapon.Damage = amountData.Value.GetDouble();
                                unitWeapon.DamageScaling = GetScaleValue(effectStormElement.ElementType, displayEffectId, amountData.Field);
                            }

                            if (effectStormElement.DataValues.TryGetElementDataAt("AttributeFactor", out StormElementData? attributeFactorData))
                            {
                                foreach (string attribute in attributeFactorData.GetElementDataIndexes())
                                {
                                    unitWeapon.AttributeFactors[attribute] = attributeFactorData.GetElementDataAt(attribute).Value.GetDouble();
                                }
                            }
                        }
                    }

                    elementObject.Weapons.Add(unitWeapon);
                }
            }
        }
    }

    private void SetAbilityData(Unit elementObject, StormElement stormElement)
    {
        // keep track of added abilities; by their nameId
        HashSet<string> addedAbilities = new(StringComparer.OrdinalIgnoreCase);

        // keep track of abilities that were NOT added; by their nameId
        HashSet<string> rejectedAbilities = new(StringComparer.OrdinalIgnoreCase);

        // loop through the cardlayouts to get the abilities
        if (stormElement.DataValues.TryGetElementDataAt("CardLayouts", out StormElementData? cardLayoutsData))
        {
            foreach (string cardLayoutIndex in cardLayoutsData.GetElementDataIndexes())
            {
                StormElementData layoutButtonsData = cardLayoutsData.GetElementDataAt(cardLayoutIndex);

                foreach (string layoutButtonIndex in layoutButtonsData.GetElementDataIndexes())
                {
                    StormElementData innerData = layoutButtonsData.GetElementDataAt(layoutButtonIndex);

                    foreach (string item in innerData.GetElementDataIndexes())
                    {
                        // Face="Move" Type="AbilCmd" AbilCmd="move,Move" Slot="Stop" />
                        StormElementData innerDataElements = innerData.GetElementDataAt(item);

                        Ability? ability = _abilityParser.GetAbility(innerDataElements);

                        if (ability is null && innerDataElements.TryGetElementDataAt("AbilCmd", out StormElementData? abilCmdData))
                        {
                            rejectedAbilities.Add(AbilityParser.GetAbilCmdSplit(abilCmdData.Value.GetString()).AbilityId);
                        }
                        else if (ability is not null && addedAbilities.Contains(ability.NameId) is false)
                        {
                            elementObject.AddAbility(ability);
                            addedAbilities.Add(ability.NameId);
                        }
                    }
                }
            }
        }

        // loop through the abilArray to get the abilities
        if (stormElement.DataValues.TryGetElementDataAt("AbilArray", out StormElementData? abilArrayData))
        {
            foreach (string abilArrayIndex in abilArrayData.GetElementDataIndexes())
            {
                if (abilArrayData.GetElementDataAt(abilArrayIndex).TryGetElementDataAt("Link", out StormElementData? linkData))
                {
                    string linkValue = linkData.Value.GetString();

                    // check if we already added the ability or rejected it
                    if (addedAbilities.Contains(linkValue) || rejectedAbilities.Contains(linkValue))
                        continue;

                    Ability? ability = _abilityParser.GetAbility(linkValue);

                    if (ability is null)
                    {
                        rejectedAbilities.Add(linkValue);
                    }
                    else if (ability is not null)
                    {
                        elementObject.AddAbility(ability);
                        addedAbilities.Add(ability.NameId);
                    }
                }
            }
        }
    }
}
