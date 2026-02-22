namespace HeroesDataParser.Core.Models;

// for re-serialization of the root json element when applying json patch
public class RootJsonElement
{
    [JsonPropertyName(ElementConstants.RootMetaPropertyName)]
    public object MetaDataProperties { get; set; } = new();

    [JsonPropertyName(ElementConstants.ItemsPropertyName)]
    public SortedDictionary<string, object> Items { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
