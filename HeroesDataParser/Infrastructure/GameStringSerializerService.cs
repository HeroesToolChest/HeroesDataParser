namespace HeroesDataParser.Infrastructure;

public class GameStringSerializerService : IGameStringSerializerService
{
    private readonly ILogger<GameStringSerializerService> _logger;

    public GameStringSerializerService(ILogger<GameStringSerializerService> logger)
    {
        _logger = logger;
    }

    public GameStringItemDictionary DataGameStringItemDictionary { get; } = [];

    // along with the meta properties, serialize the game strings stored in the GameStringItemDictionary
    // this method does not clear the dictionary after serialization
    // param JsonSerializerOptions is not DI because it would be circular dependency with JsonSerializerOptionService
    public byte[] SerializeGameStrings(MetaGameStringProperties metaGameStringProperties, JsonSerializerOptions jsonSerializerOptions)
    {
        RootJsonGameStringElement rootJsonGameStringElement = new()
        {
            MetaGameStringProperties = metaGameStringProperties,
            Items = DataGameStringItemDictionary,
        };

        return JsonSerializer.SerializeToUtf8Bytes(rootJsonGameStringElement, jsonSerializerOptions);
    }

    public void ClearStoredGameStrings()
    {
        DataGameStringItemDictionary.Clear();
        _logger.LogInformation("Cleared all serialized game string data.");
    }
}
