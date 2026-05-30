namespace HeroesDataParser.Infrastructure.Processors.Tests;

[TestClass]
public class JsonDataFileWriterProcessorTests
{
    private readonly ILogger<JsonDataFileWriterProcessor> _logger;
    private readonly IJsonDataFileWriterService _jsonDataFileWriterService;

    public JsonDataFileWriterProcessorTests()
    {
        _logger = Substitute.For<ILogger<JsonDataFileWriterProcessor>>();
        _jsonDataFileWriterService = Substitute.For<IJsonDataFileWriterService>();
    }

    [TestMethod]
    public async Task ExecuteJsonDataFileWriteTasks_NoTasksSaved_LogsWarningAndReturns()
    {
        // arrange
        JsonDataFileWriterProcessor processor = new(_logger, _jsonDataFileWriterService);

        // act
        await processor.ExecuteJsonDataFileWriteTasks();

        // assert
        await _jsonDataFileWriterService.DidNotReceive().Write(Arg.Any<SortedDictionary<string, Announcer>>());
        await _jsonDataFileWriterService.DidNotReceive().WriteToMapSpecific(Arg.Any<Map>(), Arg.Any<SortedDictionary<string, Announcer>>());
    }

    [TestMethod]
    public async Task ExecuteJsonDataFileWriteTasks_WithNullMap_CallsWrite()
    {
        // arrange
        JsonDataFileWriterProcessor processor = new(_logger, _jsonDataFileWriterService);

        SortedDictionary<string, Announcer> items = new()
        {
            { "announcer1", new Announcer("announcer1") },
        };

        processor.SaveJsonDataFileWrite(items, null);

        // act
        await processor.ExecuteJsonDataFileWriteTasks();

        // assert
        await _jsonDataFileWriterService.Received(1).Write(items);
        await _jsonDataFileWriterService.DidNotReceive().WriteToMapSpecific(Arg.Any<Map>(), Arg.Any<SortedDictionary<string, Announcer>>());
    }

    [TestMethod]
    public async Task ExecuteJsonDataFileWriteTasks_WithMap_CallsWriteToMapSpecific()
    {
        // arrange
        JsonDataFileWriterProcessor processor = new(_logger, _jsonDataFileWriterService);

        Map map = new("mapId");

        SortedDictionary<string, Announcer> items = new()
        {
            { "announcer1", new Announcer("announcer1") },
        };

        processor.SaveJsonDataFileWrite(items, map);

        // act
        await processor.ExecuteJsonDataFileWriteTasks();

        // assert
        await _jsonDataFileWriterService.DidNotReceive().Write(Arg.Any<SortedDictionary<string, Announcer>>());
        await _jsonDataFileWriterService.Received(1).WriteToMapSpecific(map, items);
    }

    [TestMethod]
    public async Task ExecuteJsonDataFileWriteTasks_MultipleTasks_ExecutesAll()
    {
        // arrange
        JsonDataFileWriterProcessor processor = new(_logger, _jsonDataFileWriterService);

        Map map = new("mapId");

        SortedDictionary<string, Announcer> items1 = new()
        {
            { "announcer1", new Announcer("announcer1") },
        };

        SortedDictionary<string, Announcer> items2 = new()
        {
            { "announcer2", new Announcer("announcer2") },
        };

        processor.SaveJsonDataFileWrite(items1, null);
        processor.SaveJsonDataFileWrite(items2, map);

        // act
        await processor.ExecuteJsonDataFileWriteTasks();

        // assert
        await _jsonDataFileWriterService.Received(1).Write(items1);
        await _jsonDataFileWriterService.Received(1).WriteToMapSpecific(map, items2);
    }

    [TestMethod]
    public async Task ExecuteJsonDataFileWriteTasks_AfterExecution_ClearsTasks()
    {
        // arrange
        JsonDataFileWriterProcessor processor = new(_logger, _jsonDataFileWriterService);

        SortedDictionary<string, Announcer> items = new()
        {
            { "announcer1", new Announcer("announcer1") },
        };

        processor.SaveJsonDataFileWrite(items, null);

        await processor.ExecuteJsonDataFileWriteTasks();

        _jsonDataFileWriterService.ClearReceivedCalls();

        // act - execute again with no new tasks
        await processor.ExecuteJsonDataFileWriteTasks();

        // assert - should not call Write again since tasks were cleared
        await _jsonDataFileWriterService.DidNotReceive().Write(Arg.Any<SortedDictionary<string, Announcer>>());
    }
}
