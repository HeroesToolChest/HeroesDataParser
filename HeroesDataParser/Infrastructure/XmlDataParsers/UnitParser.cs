using Heroes.Element.Models;

namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class UnitParser : ParserBase<Unit>
{
    private const string ActorDataObjectType = "Actor";

    private readonly ILogger<UnitParser> _logger;
    private readonly HeroesData _heroesData;

    public UnitParser(ILogger<UnitParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
    }

    public override string DataObjectType => "Unit";

    protected override void SetProperties(Unit elementObject, StormElement stormElement)
    {
        SetActorData(elementObject);
        SetUnitData(elementObject, stormElement);
        SetArmorData(elementObject, stormElement);
        SetWeaponData(elementObject, stormElement);

        ParseBehaviorLink(elementObject, stormElement);
    }

    private void SetActorData(Unit elementObject)
    {
        string? unitId = _heroesData.GetStormElementIdByUnitName(elementObject.Id, ActorDataObjectType);

        StormElement? actorElement;
        if (!string.IsNullOrEmpty(unitId))
        {
            actorElement = _heroesData.GetCompleteStormElement(ActorDataObjectType, unitId);
        }
        else
        {
            actorElement = _heroesData.GetCompleteStormElement(ActorDataObjectType, elementObject.Id);
        }

        if (actorElement is null)
        {
            _logger.LogTrace("Actor element not found for Unit {Id}", elementObject.Id);
            return;
        }

        if (actorElement.DataValues.TryGetElementDataAt("VitalNames", out StormElementData? vitalNamesData))
        {
            if (vitalNamesData.TryGetElementDataAt("Life", out StormElementData? lifeData))
                elementObject.Life.LifeType = GetTooltipDescription(lifeData.Value.GetString());
            if (vitalNamesData.TryGetElementDataAt("Shields", out StormElementData? shieldsData))
                elementObject.Shield.ShieldType = GetTooltipDescription(shieldsData.Value.GetString());
            if (vitalNamesData.TryGetElementDataAt("Energy", out StormElementData? energyData))
                elementObject.Energy.EnergyType = GetTooltipDescription(energyData.Value.GetString());
        }

        //// TODO additional actor abilities

        if (actorElement.DataValues.TryGetElementDataAt("GroupIcon", out StormElementData? groupIconData) && groupIconData.TryGetElementDataAt("Image", out StormElementData? groupImageData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(groupImageData);
            if (imageFilePath is not null)
            {
                elementObject.UnitPortrait.TargetInfoPanel = imageFilePath.Image;
                elementObject.UnitPortrait.TargetInfoPanelPath = imageFilePath.FilePath;
            }
        }

        if (actorElement.DataValues.TryGetElementDataAt("MiniMapIcon", out StormElementData? miniMapIconData) && miniMapIconData.TryGetElementDataAt("Image", out StormElementData? miniMapImageData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(miniMapImageData);
            if (imageFilePath is not null)
            {
                elementObject.UnitPortrait.MiniMapIcon = imageFilePath.Image;
                elementObject.UnitPortrait.MiniMapIconPath = imageFilePath.FilePath;
            }
        }
    }

    private void SetUnitData(Unit elementObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("LifeMax", out StormElementData? lifeMaxData))
        {
            elementObject.Life.LifeMax = lifeMaxData.Value.GetDouble();
            elementObject.Life.LifeMaxScaling = GetScaleValue(stormElement.ElementType, elementObject.Id, lifeMaxData.Field);
        }

        if (stormElement.DataValues.TryGetElementDataAt("LifeRegenRate", out StormElementData? lifeRegenRateData))
        {
            elementObject.Life.LifeRegenerationRate = lifeRegenRateData.Value.GetDouble();
            elementObject.Life.LifeRegenerationRateScaling = GetScaleValue(stormElement.ElementType, elementObject.Id, lifeRegenRateData.Field);
        }

        if (stormElement.DataValues.TryGetElementDataAt("ShieldsMax", out StormElementData? shieldsMaxData))
        {
            elementObject.Shield.ShieldMax = shieldsMaxData.Value.GetDouble();
            elementObject.Shield.ShieldScaling = GetScaleValue(stormElement.ElementType, elementObject.Id, shieldsMaxData.Field);
        }

        if (stormElement.DataValues.TryGetElementDataAt("ShieldsRegenDelay", out StormElementData? shieldsRegenDelayData))
            elementObject.Shield.ShieldRegenerationDelay = shieldsRegenDelayData.Value.GetDouble();

        if (stormElement.DataValues.TryGetElementDataAt("ShieldRegenRate", out StormElementData? shieldRegenRateData))
        {
            elementObject.Shield.ShieldRegenerationRate = shieldRegenRateData.Value.GetDouble();
            elementObject.Shield.ShieldRegenerationRateScaling = GetScaleValue(stormElement.ElementType, elementObject.Id, shieldRegenRateData.Field);
        }

        if (stormElement.DataValues.TryGetElementDataAt("EnergyMax", out StormElementData? energyMaxData))
            elementObject.Energy.EnergyMax = energyMaxData.Value.GetDouble();

        if (stormElement.DataValues.TryGetElementDataAt("EnergyRegenRate", out StormElementData? energyRegenRateData))
            elementObject.Energy.EnergyRegenerationRate = energyRegenRateData.Value.GetDouble();

        if (stormElement.DataValues.TryGetElementDataAt("InnerRadius", out StormElementData? innerRadiusData))
            elementObject.InnerRadius = innerRadiusData.Value.GetDouble();

        if (stormElement.DataValues.TryGetElementDataAt("Radius", out StormElementData? radiusData))
            elementObject.Radius = radiusData.Value.GetDouble();

        if (stormElement.DataValues.TryGetElementDataAt("Sight", out StormElementData? sightData))
            elementObject.Sight = sightData.Value.GetDouble();

        if (stormElement.DataValues.TryGetElementDataAt("Speed", out StormElementData? speedData))
            elementObject.Speed = speedData.Value.GetDouble();

        if (stormElement.DataValues.TryGetElementDataAt("UnitDamageType", out StormElementData? unitDamageTypeData))
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

        if (stormElement.DataValues.TryGetElementDataAt("KillXp", out StormElementData? killXpData))
            elementObject.KillXP = killXpData.Value.GetInt();

        if (stormElement.DataValues.TryGetElementDataAt("InfoText", out StormElementData? infoTextData))
            elementObject.InfoText = GetTooltipDescription(infoTextData.Value.GetString());

        if (stormElement.DataValues.TryGetElementDataAt("HeroPlaystyleFlags", out StormElementData? heroPlaystyleFlagsData))
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

        if (stormElement.DataValues.TryGetElementDataAt("Attributes", out StormElementData? attributesData))
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
                        unitWeapon.Name = GetTooltipDescription(nameData.Value.GetString());

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
}
