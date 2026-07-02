namespace HeroesDataParser.Infrastructure.Processors.Tests;

[TestClass]
public class MapProcessorServiceTests
{
    private readonly IXmlMapDataParserProcessor _xmlMapDataParserProcessor;
    private readonly IJsonGameStringFileWriterProcessor _jsonGameStringFileWriterProcessor;

    public MapProcessorServiceTests()
    {
        _xmlMapDataParserProcessor = Substitute.For<IXmlMapDataParserProcessor>();
        _jsonGameStringFileWriterProcessor = Substitute.For<IJsonGameStringFileWriterProcessor>();
    }

    [TestMethod]
    public async Task Start_WhenCalled_ExecutesMapParser()
    {
        // arrange
        MapProcessorService service = new(_xmlMapDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.Start();

        // assert
        await _xmlMapDataParserProcessor.Received(1).ExecuteMapParser();
    }

    [TestMethod]
    public async Task Start_WhenCalled_ExecutesJsonDataFileWriteTask()
    {
        // arrange
        MapProcessorService service = new(_xmlMapDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.Start();

        // assert
        await _xmlMapDataParserProcessor.Received(1).ExecuteJsonDataFileWriteTask();
    }

    [TestMethod]
    public async Task Start_WhenCalled_WritesMapGameStringFile()
    {
        // arrange
        MapProcessorService service = new(_xmlMapDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.Start();

        // assert
        await _jsonGameStringFileWriterProcessor.Received(1).WriteMapGameStringFile();
    }

    [TestMethod]
    public async Task Start_WhenCalled_ExecutesInCorrectOrder()
    {
        // arrange
        List<string> callOrder = [];

        _xmlMapDataParserProcessor.ExecuteMapParser().Returns(x =>
        {
            callOrder.Add(nameof(IXmlMapDataParserProcessor.ExecuteMapParser));
            return Task.CompletedTask;
        });

        _xmlMapDataParserProcessor.ExecuteJsonDataFileWriteTask().Returns(x =>
        {
            callOrder.Add(nameof(IXmlMapDataParserProcessor.ExecuteJsonDataFileWriteTask));
            return Task.CompletedTask;
        });

        _jsonGameStringFileWriterProcessor.WriteMapGameStringFile().Returns(x =>
        {
            callOrder.Add(nameof(IJsonGameStringFileWriterProcessor.WriteMapGameStringFile));
            return Task.CompletedTask;
        });

        MapProcessorService service = new(_xmlMapDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.Start();

        // assert
        callOrder.Should().BeEquivalentTo(
            new[]
            {
                nameof(IXmlMapDataParserProcessor.ExecuteMapParser),
                nameof(IXmlMapDataParserProcessor.ExecuteJsonDataFileWriteTask),
                nameof(IJsonGameStringFileWriterProcessor.WriteMapGameStringFile),
            },
            options => options.WithStrictOrdering());
    }
}
