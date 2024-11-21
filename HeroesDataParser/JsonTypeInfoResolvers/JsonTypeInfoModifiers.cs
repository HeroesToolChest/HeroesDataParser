namespace HeroesDataParser.JsonTypeInfoResolvers;

public class JsonTypeInfoModifiers
{
    // only serialize collections if they have items
    public static void IEnumerableModifier(JsonTypeInfo typeInfo)
    {
        foreach (JsonPropertyInfo propertyInfo in typeInfo.Properties)
        {
            if (propertyInfo.PropertyType.GetInterface(nameof(IEnumerable)) is not null && propertyInfo.PropertyType != typeof(string))
            {
                propertyInfo.ShouldSerialize = static (obj, val) =>
                {
                    return val is not IEnumerable enumerable || enumerable.GetEnumerator().MoveNext();
                };
            }
        }
    }

    public static void TooltipDescriptionModifier(JsonTypeInfo typeInfo)
    {
    }
}
