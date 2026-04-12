namespace HeroesDataParser.Infrastructure.Processors;

public class MainLocalePreProcessor : IMainLocalePreProcessor
{
    private readonly ISerializedDataStoreService _serializedDataStoreService;
    private readonly IGameStringSerializerService _gameStringSerializerService;

    public MainLocalePreProcessor(
        ISerializedDataStoreService serializedDataStoreService,
        IGameStringSerializerService gameStringSerializerService)
    {
        _serializedDataStoreService = serializedDataStoreService;
        _gameStringSerializerService = gameStringSerializerService;
    }

    public void Run()
    {
        _serializedDataStoreService.ClearAllSerializedData();
        _gameStringSerializerService.ClearStoredGameStrings();
    }
}
