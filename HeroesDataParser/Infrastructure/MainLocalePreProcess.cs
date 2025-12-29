namespace HeroesDataParser.Infrastructure;

public class MainLocalePreProcess : IMainLocalePreProcess
{
    private readonly ISerializedDataStoreService _serializedDataStoreService;
    private readonly IGameStringSerializerService _gameStringSerializerService;

    public MainLocalePreProcess(
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
