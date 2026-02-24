namespace HeroesDataParser.Infrastructure;

public class GameStringSerializerService : IGameStringSerializerService
{
    private readonly ILogger<GameStringSerializerService> _logger;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    public GameStringSerializerService(ILogger<GameStringSerializerService> logger, IJsonSerializerOptionService jsonSerializerOptionService)
    {
        _logger = logger;
        _jsonSerializerOptionService = jsonSerializerOptionService;
    }

    public GameStringItemDictionary DataGameStringItemDictionary { get; } = [];

    // along with the meta properties, serialize the game strings stored in the GameStringItemDictionary
    // this method does not clear the dictionary after serialization
    public byte[] SerializeGameStrings(MetaGameStringProperties metaGameStringProperties)
    {
        RootJsonGameStringElement rootJsonGameStringElement = new()
        {
            MetaGameStringProperties = metaGameStringProperties,
            Items = DataGameStringItemDictionary,
        };

        return JsonSerializer.SerializeToUtf8Bytes(rootJsonGameStringElement, _jsonSerializerOptionService.JsonSerializerGameStringOptions);
    }

    public void ClearStoredGameStrings()
    {
        DataGameStringItemDictionary.Clear();
        _logger.LogInformation("Cleared all serialized game string data.");
    }
}
