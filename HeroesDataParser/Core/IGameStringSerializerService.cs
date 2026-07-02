namespace HeroesDataParser.Core;

public interface IGameStringSerializerService
{
    /// <summary>
    /// Gets the saved gamestring texts by their key names. Key names are in the three dictionaries format: element/propertyname/id=value.
    /// </summary>
    GameStringItemDictionary DataGameStringItemDictionary { get; }

    byte[] SerializeGameStrings(MetaGameStringProperties metaGameStringProperties, JsonSerializerOptions jsonSerializerOptions);

    void ClearStoredGameStrings();
}
