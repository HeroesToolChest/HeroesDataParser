namespace HeroesDataParser.Infrastructure.Processors.Tests;

[TestClass]
public class MainLocalePreProcessorTests
{
    private readonly ISerializedDataStoreService _serializedDataStoreService;
    private readonly IGameStringSerializerService _gameStringSerializerService;

    public MainLocalePreProcessorTests()
    {
        _serializedDataStoreService = Substitute.For<ISerializedDataStoreService>();
        _gameStringSerializerService = Substitute.For<IGameStringSerializerService>();
    }

    [TestMethod]
    public void Run_WhenCalled_ClearsAllSerializedData()
    {
        // arrange
        MainLocalePreProcessor processor = new(_serializedDataStoreService, _gameStringSerializerService);

        // act
        processor.Run();

        // assert
        _serializedDataStoreService.Received(1).ClearAllSerializedData();
    }

    [TestMethod]
    public void Run_WhenCalled_ClearsStoredGameStrings()
    {
        // arrange
        MainLocalePreProcessor processor = new(_serializedDataStoreService, _gameStringSerializerService);

        // act
        processor.Run();

        // assert
        _gameStringSerializerService.Received(1).ClearStoredGameStrings();
    }
}