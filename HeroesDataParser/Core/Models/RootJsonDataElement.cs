namespace HeroesDataParser.Core.Models;

// for re-serialization of the root json data element when applying json patch
public class RootJsonDataElement
{
    [JsonPropertyName(ElementConstants.RootMetaPropertyName)]
    public MetaDataProperties MetaDataProperties { get; set; } = new();

    [JsonPropertyName(ElementConstants.ItemsPropertyName)]
    public SortedDictionary<string, object> Items { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
