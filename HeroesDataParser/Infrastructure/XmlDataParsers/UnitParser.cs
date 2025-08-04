using Serilog.Context;

namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class UnitParser : DataParser<Unit>, IUnitParser
{
    private const string _actorDataObjectType = "Actor";

    private readonly IAbilityParser _abilityParser;

    public UnitParser(ILogger<UnitParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IAbilityParser abilityParser, ITooltipDescriptionService tooltipDescriptionService)
        : base(logger, options, heroesXmlLoaderService, tooltipDescriptionService)
    {
        _abilityParser = abilityParser;
    }

    public override string DataObjectType => "Unit";

    public bool AllowHiddenAbilities { get; set; } = true;

    public bool AllowSpecialAbilities { get; set; } = true;

    public void Parse(Unit unit)
    {
        Logger.LogTrace("Parsing unit for existing id {Id}", unit.Id);

        StormElement? stormElement = HeroesData.GetCompleteStormElement(DataObjectType, unit.Id);

        if (stormElement is null)
        {
            Logger.LogWarning("Could not find data for id {Id}", unit.Id);
            return;
        }

        using (LogContext.PushProperty("XmlPaths", stormElement.OriginalXElements.Select(x => x.StormPath)))
        {
            SetProperties(unit, stormElement);

            Logger.LogTrace("Parsing unit for existing id {Id} complete", unit.Id);
        }
    }

    protected override void SetProperties(Unit elementObject, StormElement stormElement)
    {
        SetActorData(elementObject, out List<Ability> unitButtonAbilities);
        SetUnitData(elementObject, stormElement);
        SetArmorData(elementObject, stormElement);
        SetWeaponData(elementObject, stormElement);
        SetBehaviors(elementObject, stormElement, out List<StormElement> behaviorAbilityStormElements);

        SetAbilityData(elementObject, stormElement, unitButtonAbilities, behaviorAbilityStormElements);
    }

    private static void AddAbilityByTooltipTalentElementIds(Unit elementObject, Ability ability)
    {
        foreach (string talentElementId in ability.TooltipAppendersTalentElementIds)
        {
            elementObject.AddAbilityByTooltipTalentElementId(talentElementId, ability);
        }
    }

    private static void ProcessAbility(Unit elementObject, Ability ability, List<Ability> abilitiesWithParentIds)
    {
        if (!string.IsNullOrEmpty(ability.ParentAbilityElementId) || ability.ParentAbilityLinkId is not null)
            abilitiesWithParentIds.Add(ability);
        else if (!string.IsNullOrEmpty(ability.ParentTalentElementId) || ability.ParentTalentLinkId is not null)
            elementObject.AddAsLayoutUnknownSubAbility(ability);
        else
            elementObject.AddLayoutAbility(ability);

        AddAbilityByTooltipTalentElementIds(elementObject, ability);
    }

    private void SetActorData(Unit elementObject, out List<Ability> unitButtonAbilities)
    {
        unitButtonAbilities = [];

        string? unitId = HeroesData.GetStormElementIdByUnitName(elementObject.Id, _actorDataObjectType);

        StormElement? actorElement;
        if (!string.IsNullOrEmpty(unitId))
        {
            actorElement = HeroesData.GetCompleteStormElement(_actorDataObjectType, unitId);
        }
        else
        {
            actorElement = HeroesData.GetCompleteStormElement(_actorDataObjectType, elementObject.Id);
        }

        if (actorElement is null)
        {
            Logger.LogTrace("Actor element not found for Unit {Id}", elementObject.Id);
            return;
        }

        if (actorElement.DataValues.TryGetElementDataAt("VitalNames", out StormElementData? vitalNamesData))
        {
            if (vitalNamesData.TryGetElementDataAt("Life", out StormElementData? lifeData))
                elementObject.Life.LifeType = TooltipDescriptionService.GetTooltipDescriptionFromId(lifeData.Value.GetString());
            if (vitalNamesData.TryGetElementDataAt("Shields", out StormElementData? shieldsData))
                elementObject.Shield.ShieldType = TooltipDescriptionService.GetTooltipDescriptionFromId(shieldsData.Value.GetString());
            if (vitalNamesData.TryGetElementDataAt("Energy", out StormElementData? energyData))
                elementObject.Energy.EnergyType = TooltipDescriptionService.GetTooltipDescriptionFromId(energyData.Value.GetString());
        }

        if (actorElement.DataValues.TryGetElementDataAt("GroupIcon", out StormElementData? groupIconData) && groupIconData.TryGetElementDataAt("Image", out StormElementData? groupImageData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(groupImageData);
            if (imageFilePath is not null)
            {
                elementObject.UnitPortraits.TargetInfoPanel = imageFilePath.Image;
                elementObject.UnitPortraits.TargetInfoPanelPath = imageFilePath.FilePath;
            }
        }

        if (actorElement.DataValues.TryGetElementDataAt("MiniMapIcon", out StormElementData? miniMapIconData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(miniMapIconData);
            if (imageFilePath is not null)
            {
                elementObject.UnitPortraits.MiniMapIcon = imageFilePath.Image;
                elementObject.UnitPortraits.MiniMapIconPath = imageFilePath.FilePath;
            }
        }

        if (actorElement.DataValues.TryGetElementDataAt("UnitButton", out StormElementData? unitButtonData))
        {
            AddUnitButtonAbility(unitButtonAbilities, unitButtonData);
        }

        if (actorElement.DataValues.TryGetElementDataAt("UnitButtonMultiple", out StormElementData? unitButtonMultipleData))
        {
            AddUnitButtonAbility(unitButtonAbilities, unitButtonMultipleData);
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

        if (elementData.TryGetElementDataAt("ShieldRegenDelay", out StormElementData? shieldsRegenDelayData))
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
                Logger.LogWarning("Unknown armor set type {ArmorSet} for damage type", unitDamageTypeData.Value.GetString());
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
                Logger.LogWarning("Unknown gender {Gender}", genderValue);
        }
    }

    private void SetBehaviors(Unit elementObject, StormElement stormElement, out List<StormElement> behaviorAbilityElements)
    {
        behaviorAbilityElements = [];

        if (!stormElement.DataValues.TryGetElementDataAt("BehaviorArray", out StormElementData? behaviorArrayData))
            return;

        foreach (string item in behaviorArrayData.GetElementDataIndexes())
        {
            if (!behaviorArrayData.GetElementDataAt(item).TryGetElementDataAt("link", out StormElementData? linkData))
                continue;

            string linkId = linkData.Value.GetString();

            StormElement? behaviorStormElement = HeroesData.GetCompleteStormElement("Behavior", linkId);

            if (behaviorStormElement is null)
                continue;

            if (behaviorStormElement.ElementType.Equals("CBehaviorVeterancy", StringComparison.OrdinalIgnoreCase))
                elementObject.ScalingLinkIds.Add(linkId);
            else if (behaviorStormElement.ElementType.Equals("CBehaviorAbility", StringComparison.OrdinalIgnoreCase))
                behaviorAbilityElements.Add(behaviorStormElement);
        }
    }

    private void SetArmorData(Unit elementObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("ArmorLink", out StormElementData? armorLinkData))
        {
            StormElement? armorStormElement = HeroesData.GetCompleteStormElement("Armor", armorLinkData.Value.GetString());

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
                                Logger.LogWarning("Unknown armor set type {ArmorSet}", type);
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

                    StormElement? weaponStormElement = HeroesData.GetCompleteStormElement("Weapon", linkId);

                    if (weaponStormElement is null)
                    {
                        Logger.LogWarning("Weapon element does not exist for id {LinkId}", linkId);
                        continue;
                    }

                    UnitWeapon unitWeapon = new()
                    {
                        NameId = linkId,
                    };

                    if (weaponStormElement.DataValues.TryGetElementDataAt("Range", out StormElementData? rangeData))
                        unitWeapon.Range = rangeData.Value.GetDouble();

                    if (weaponStormElement.DataValues.TryGetElementDataAt("MinimumRange", out StormElementData? minimumRangeData))
                        unitWeapon.MinimumRange = minimumRangeData.Value.GetDouble();

                    if (weaponStormElement.DataValues.TryGetElementDataAt("Period", out StormElementData? periodData))
                        unitWeapon.Period = periodData.Value.GetDouble();

                    if (weaponStormElement.DataValues.TryGetElementDataAt("Options", out StormElementData? optionsData))
                    {
                        if (optionsData.TryGetElementDataAt("Disabled", out StormElementData? disabledData) && disabledData.Value.GetInt() == 1)
                            unitWeapon.IsDisabled = true;
                    }

                    if (weaponStormElement.DataValues.TryGetElementDataAt("DisplayEffect", out StormElementData? displayEffectData))
                    {
                        string displayEffectId = displayEffectData.Value.GetString();
                        StormElement? effectStormElement = HeroesData.GetCompleteStormElement("Effect", displayEffectId);

                        if (effectStormElement is null)
                        {
                            Logger.LogWarning("Effect element does not exist for id {DisplayEffectId}", displayEffectId);
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

                    if (weaponStormElement.DataValues.TryGetElementDataAt("Cost", out StormElementData? costData))
                        SetWeaponCostData(unitWeapon, costData);

                    elementObject.Weapons.Add(unitWeapon);
                }
            }
        }
    }

    private void SetAbilityData(Unit elementObject, StormElement stormElement, IEnumerable<Ability> unitButtonAbilities, IEnumerable<StormElement> behaviorAbilityStormElements)
    {
        // keep track of abilities that are being added; by their nameId
        // when we add an ability, we remove it from the checklist
        HashSet<string> abilityIdChecklist = new(StringComparer.OrdinalIgnoreCase);

        // keep track of abilities that have parent ids
        List<Ability> abilitiesWithParentIds = [];

        // loop through the abilArray to get the ability ids
        if (stormElement.DataValues.TryGetElementDataAt("AbilArray", out StormElementData? abilArrayData))
        {
            foreach (string abilArrayIndex in abilArrayData.GetElementDataIndexes())
            {
                if (abilArrayData.GetElementDataAt(abilArrayIndex).TryGetElementDataAt("link", out StormElementData? linkData))
                {
                    abilityIdChecklist.Add(linkData.Value.GetString());
                }
            }
        }

        // loop through the cardlayouts to get the abilities
        if (stormElement.DataValues.TryGetElementDataAt("CardLayouts", out StormElementData? cardLayoutsData))
        {
            IEnumerable<string> cardLayoutIndexes = cardLayoutsData.GetElementDataIndexes();

            foreach (string cardLayoutIndex in cardLayoutIndexes)
            {
                StormElementData cardLayoutsElement = cardLayoutsData.GetElementDataAt(cardLayoutIndex);

                if (!cardLayoutsElement.TryGetElementDataAt("LayoutButtons", out StormElementData? layoutButtonsData))
                    continue;

                IEnumerable<string> layoutButtonsIndexes = layoutButtonsData.GetElementDataIndexes();

                foreach (string layoutButtonsIndex in layoutButtonsIndexes)
                {
                    // Face="Move" Type="AbilCmd" AbilCmd="move,Move" Slot="Stop" />
                    StormElementData layoutButtonsElement = layoutButtonsData.GetElementDataAt(layoutButtonsIndex);

                    Ability? ability = _abilityParser.GetAbility(layoutButtonsElement);

                    if (ability is null)
                        continue;

                    if (ability.AbilityType != AbilityType.Passive)
                        abilityIdChecklist.Remove(ability.AbilityElementId);

                    if ((AllowSpecialAbilities is false && ability is { AbilityType: AbilityType.Taunt or AbilityType.Dance or AbilityType.Spray or AbilityType.Voice }) ||
                        (AllowHiddenAbilities is false && ability is { AbilityType: AbilityType.Hidden }))
                        continue;

                    ProcessAbility(elementObject, ability, abilitiesWithParentIds);
                }
            }
        }

        // loop through the unit button abilities
        foreach (Ability unitButtonAbility in unitButtonAbilities)
        {
            ProcessAbility(elementObject, unitButtonAbility, abilitiesWithParentIds);
        }

        // loop through the behavior abilities
        foreach (StormElement behaviorStormElement in behaviorAbilityStormElements)
        {
            if (behaviorStormElement.DataValues.TryGetElementDataAt("buttons", out StormElementData? buttonsData))
            {
                IEnumerable<string> buttonIndexes = buttonsData.GetElementDataIndexes();

                foreach (string buttonIndex in buttonIndexes)
                {
                    Ability? behaviorAbility = _abilityParser.GetBehaviorAbility(buttonsData.GetElementDataAt(buttonIndex));
                    if (behaviorAbility is null)
                        continue;

                    abilityIdChecklist.Remove(behaviorAbility.AbilityElementId);

                    ProcessAbility(elementObject, behaviorAbility, abilitiesWithParentIds);
                }
            }
        }

        if (AllowHiddenAbilities)
        {
            // for any that are left in the ability checklist, we create an ability
            foreach (string abilityId in abilityIdChecklist)
            {
                Ability? ability = _abilityParser.GetAbility(abilityId);

                if (ability is null)
                    continue;

                ProcessAbility(elementObject, ability, abilitiesWithParentIds);
            }
        }

        // for any abilities that have parent abilities, we add them as subabilities
        foreach (Ability ability in abilitiesWithParentIds)
        {
            elementObject.AddAsLayoutSubAbilityToAbility(ability);
        }
    }

    private void SetWeaponCostData(UnitWeapon unitWeapon, StormElementData costElementData)
    {
        if (costElementData.TryGetElementDataAt("Vital", out StormElementData? vitalData))
        {
            IEnumerable<string> vitalIndexes = vitalData.GetElementDataIndexes();

            foreach (string index in vitalIndexes)
            {
                if (vitalData.TryGetElementDataAt(index, out StormElementData? vitalCostData))
                {
                    double vitalCost = vitalCostData.Value.GetDouble();
                    if (Enum.TryParse(index, true, out VitalType vitalType))
                    {
                        unitWeapon.VitalCost[vitalType] = vitalCost;
                    }
                    else
                    {
                        Logger.LogWarning("Unknown vital type {VitalType} for weapon {WeaponId}", index, unitWeapon.NameId);
                        unitWeapon.VitalCost[VitalType.Unknown] = vitalCost;
                    }
                }
            }
        }
    }

    private void AddUnitButtonAbility(List<Ability> unitButtonAbilities, StormElementData unitButtonData)
    {
        string unitButtonId = unitButtonData.Value.GetString();

        // these are just buttons and have no abil
        Ability? ability = _abilityParser.GetUnitButtonAbility(unitButtonId);
        if (ability is not null)
        {
            unitButtonAbilities.Add(ability);
        }
    }
}
