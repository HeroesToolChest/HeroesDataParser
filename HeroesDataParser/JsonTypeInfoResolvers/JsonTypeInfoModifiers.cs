using Heroes.Element.Models.AbilityTalents;

namespace HeroesDataParser.JsonTypeInfoResolvers;

public class JsonTypeInfoModifiers
{
    public static void SerialiazationModifiers(JsonTypeInfo typeInfo)
    {
        foreach (JsonPropertyInfo propertyInfo in typeInfo.Properties)
        {
            IEnumerableModifier(propertyInfo);

            if (typeInfo.Type == typeof(Hero) || typeInfo.Type == typeof(Unit))
            {
                UnitLifeEnergyShieldModifiers(propertyInfo);
            }
        }
    }

    // only serialize collections if they have items
    private static void IEnumerableModifier(JsonPropertyInfo propertyInfo)
    {
        if (propertyInfo.PropertyType.GetInterface(nameof(IEnumerable)) is not null && propertyInfo.PropertyType != typeof(string))
        {
            propertyInfo.ShouldSerialize = static (_, value) =>
            {
                return value is not IEnumerable enumerable || enumerable.GetEnumerator().MoveNext();
            };
        }
    }

    // only serialize UnitLife, UnitEnergy, and UnitShield if they have "values"
    private static void UnitLifeEnergyShieldModifiers(JsonPropertyInfo propertyInfo)
    {
        if (propertyInfo.PropertyType == typeof(UnitLife))
        {
            propertyInfo.ShouldSerialize = static (_, value) =>
            {
                return value is not null && value is UnitLife unitLife && unitLife.LifeMax > 0;
            };
        }
        else if (propertyInfo.PropertyType == typeof(UnitEnergy))
        {
            propertyInfo.ShouldSerialize = static (_, value) =>
            {
                return value is not null && value is UnitEnergy unitEnergy && (unitEnergy.EnergyMax > 0 || unitEnergy.EnergyType is not null);
            };
        }
        else if (propertyInfo.PropertyType == typeof(UnitShield))
        {
            propertyInfo.ShouldSerialize = static (_, value) =>
            {
                return value is not null && value is UnitShield unitShield && (unitShield.ShieldMax > 0);
            };
        }
    }

    //public static void TooltipDescriptionModifier(JsonTypeInfo typeInfo)
    //{
    //}
}
