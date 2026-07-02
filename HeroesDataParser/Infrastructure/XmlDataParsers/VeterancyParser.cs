namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class VeterancyParser : DataParser<Veterancy>
{
    private readonly ILogger<VeterancyParser> _logger;

    public VeterancyParser(ILogger<VeterancyParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
        _logger = logger;
    }

    public override string DataObjectType => "Behavior";

    protected override string[] AllowedElementTypes => ["CBehaviorVeterancy"];

    protected override void SetProperties(Veterancy elementObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("Flags", out StormElementData? flagsData))
        {
            if (flagsData.TryGetElementDataAt("CombineNumericModifications", out StormElementData? numericModsData) && numericModsData.Value.TryGetInt32(out int numericModsValue))
                elementObject.CombineModifications = numericModsValue == 1;
            if (flagsData.TryGetElementDataAt("CombineXP", out StormElementData? combineXPData) && combineXPData.Value.TryGetInt32(out int combineXPDataValue))
                elementObject.CombineXP = combineXPDataValue == 1;
        }

        if (stormElement.DataValues.TryGetElementDataAt("VeterancyLevelArray", out StormElementData? veterancyLevelArrayData))
        {
            IEnumerable<string> veterandcyLevelArrayIndexes = veterancyLevelArrayData.GetElementDataIndexes();

            foreach (string veterancyLevelArrayIndex in veterandcyLevelArrayIndexes)
            {
                VeterancyLevel veterancyLevel = new();
                StormElementData veterancyLevelData = veterancyLevelArrayData[veterancyLevelArrayIndex];

                if (veterancyLevelData.TryGetElementDataAt("MinVeterancyXP", out StormElementData? minVeterancyXpData))
                    veterancyLevel.MinimumVeterancyXP = minVeterancyXpData.Value.GetInt32();

                if (veterancyLevelData.TryGetElementDataAt("Modification", out StormElementData? modificationData))
                {
                    VeterancyModification veterancyModification = new();

                    if (modificationData.TryGetElementDataAt("KillXpBonus", out StormElementData? killXpBonusData))
                        veterancyModification.KillXpBonus = killXpBonusData.Value.GetDouble();

                    ParseDamageDealtScaled(modificationData, veterancyModification);
                    ParseDamageDealtFraction(modificationData, veterancyModification);
                    ParseVitalMaxArray(modificationData, veterancyModification);
                    ParseVitalMaxFractionArray(modificationData, veterancyModification);
                    ParseVitalRegenArray(modificationData, veterancyModification);
                    ParseVitalRegenFractionArray(modificationData, veterancyModification);

                    veterancyLevel.VeterancyModification = veterancyModification;
                }

                elementObject.VeterancyLevels.Add(veterancyLevel);
            }
        }
    }

    private static void ParseDamageDealtScaled(StormElementData modificationData, VeterancyModification veterancyModification)
    {
        if (modificationData.TryGetElementDataAt("DamageDealtScaled", out StormElementData? damageDealtScaledData))
        {
            if (damageDealtScaledData.TryGetElementDataAt("Basic", out StormElementData? basicData))
            {
                veterancyModification.DamageDealtScaled ??= new();
                veterancyModification.DamageDealtScaled.Basic = basicData.Value.GetDouble();
            }

            if (damageDealtScaledData.TryGetElementDataAt("Ability", out StormElementData? abilityData))
            {
                veterancyModification.DamageDealtScaled ??= new();
                veterancyModification.DamageDealtScaled.Ability = abilityData.Value.GetDouble();
            }

            if (damageDealtScaledData.TryGetElementDataAt("Splash", out StormElementData? splashData))
            {
                veterancyModification.DamageDealtScaled ??= new();
                veterancyModification.DamageDealtScaled.Splash = splashData.Value.GetDouble();
            }
        }
    }

    private static void ParseDamageDealtFraction(StormElementData modificationData, VeterancyModification veterancyModification)
    {
        if (modificationData.TryGetElementDataAt("DamageDealtFraction", out StormElementData? damageDealtFractionData))
        {
            if (damageDealtFractionData.TryGetElementDataAt("Basic", out StormElementData? basicData))
            {
                veterancyModification.DamageDealtFraction ??= new();
                veterancyModification.DamageDealtFraction.Basic = basicData.Value.GetDouble();
            }

            if (damageDealtFractionData.TryGetElementDataAt("Ability", out StormElementData? abilityData))
            {
                veterancyModification.DamageDealtFraction ??= new();
                veterancyModification.DamageDealtFraction.Ability = abilityData.Value.GetDouble();
            }

            if (damageDealtFractionData.TryGetElementDataAt("Splash", out StormElementData? splashData))
            {
                veterancyModification.DamageDealtFraction ??= new();
                veterancyModification.DamageDealtFraction.Splash = splashData.Value.GetDouble();
            }
        }
    }

    private static void ParseVitalMaxArray(StormElementData modificationData, VeterancyModification veterancyModification)
    {
        if (modificationData.TryGetElementDataAt("VitalMaxArray", out StormElementData? vitalMaxArrayData))
        {
            if (vitalMaxArrayData.TryGetElementDataAt("Life", out StormElementData? lifeData))
            {
                veterancyModification.VitalMaxValue ??= new();
                veterancyModification.VitalMaxValue.Life = lifeData.Value.GetDouble();
            }

            if (vitalMaxArrayData.TryGetElementDataAt("Energy", out StormElementData? energyData))
            {
                veterancyModification.VitalMaxValue ??= new();
                veterancyModification.VitalMaxValue.Energy = energyData.Value.GetDouble();
            }

            if (vitalMaxArrayData.TryGetElementDataAt("Shield", out StormElementData? shieldData))
            {
                veterancyModification.VitalMaxValue ??= new();
                veterancyModification.VitalMaxValue.Shield = shieldData.Value.GetDouble();
            }
        }
    }

    private static void ParseVitalMaxFractionArray(StormElementData modificationData, VeterancyModification veterancyModification)
    {
        if (modificationData.TryGetElementDataAt("VitalMaxFractionArray", out StormElementData? vitalMaxFractionArrayData))
        {
            if (vitalMaxFractionArrayData.TryGetElementDataAt("Life", out StormElementData? lifeData))
            {
                veterancyModification.VitalMaxFraction ??= new();
                veterancyModification.VitalMaxFraction.Life = lifeData.Value.GetDouble();
            }

            if (vitalMaxFractionArrayData.TryGetElementDataAt("Energy", out StormElementData? energyData))
            {
                veterancyModification.VitalMaxFraction ??= new();
                veterancyModification.VitalMaxFraction.Energy = energyData.Value.GetDouble();
            }

            if (vitalMaxFractionArrayData.TryGetElementDataAt("Shield", out StormElementData? shieldData))
            {
                veterancyModification.VitalMaxFraction ??= new();
                veterancyModification.VitalMaxFraction.Shield = shieldData.Value.GetDouble();
            }
        }
    }

    private static void ParseVitalRegenArray(StormElementData modificationData, VeterancyModification veterancyModification)
    {
        if (modificationData.TryGetElementDataAt("VitalRegenArray", out StormElementData? vitalRegenArrayData))
        {
            if (vitalRegenArrayData.TryGetElementDataAt("Life", out StormElementData? lifeData))
            {
                veterancyModification.VitalRegeneration ??= new();
                veterancyModification.VitalRegeneration.Life = lifeData.Value.GetDouble();
            }

            if (vitalRegenArrayData.TryGetElementDataAt("Energy", out StormElementData? energyData))
            {
                veterancyModification.VitalRegeneration ??= new();
                veterancyModification.VitalRegeneration.Energy = energyData.Value.GetDouble();
            }

            if (vitalRegenArrayData.TryGetElementDataAt("Shield", out StormElementData? shieldData))
            {
                veterancyModification.VitalRegeneration ??= new();
                veterancyModification.VitalRegeneration.Shield = shieldData.Value.GetDouble();
            }
        }
    }

    private static void ParseVitalRegenFractionArray(StormElementData modificationData, VeterancyModification veterancyModification)
    {
        if (modificationData.TryGetElementDataAt("VitalRegenFractionArray", out StormElementData? vitalRegenFractionArrayData))
        {
            if (vitalRegenFractionArrayData.TryGetElementDataAt("Life", out StormElementData? lifeData))
            {
                veterancyModification.VitalRegenerationFraction ??= new();
                veterancyModification.VitalRegenerationFraction.Life = lifeData.Value.GetDouble();
            }

            if (vitalRegenFractionArrayData.TryGetElementDataAt("Energy", out StormElementData? energyData))
            {
                veterancyModification.VitalRegenerationFraction ??= new();
                veterancyModification.VitalRegenerationFraction.Energy = energyData.Value.GetDouble();
            }

            if (vitalRegenFractionArrayData.TryGetElementDataAt("Shield", out StormElementData? shieldData))
            {
                veterancyModification.VitalRegenerationFraction ??= new();
                veterancyModification.VitalRegenerationFraction.Shield = shieldData.Value.GetDouble();
            }
        }
    }
}
