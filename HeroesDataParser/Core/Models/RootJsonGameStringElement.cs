namespace HeroesDataParser.Core.Models;

// for re-serialization of the root json gamestring element when applying json patch
internal class RootJsonGameStringElement
{
    [JsonPropertyName(ElementConstants.RootMetaPropertyName)]
    public MetaGameStringProperties MetaGameStringProperties { get; set; } = new();

    [JsonPropertyName(ElementConstants.ItemsPropertyName)]
    public GameStringItemDictionary Items { get; set; } = [];
}
