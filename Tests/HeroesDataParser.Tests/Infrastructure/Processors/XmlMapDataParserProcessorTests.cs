namespace HeroesDataParser.Infrastructure.Processors.Tests;

[TestClass]
public class XmlMapDataParserProcessorTests
{
    private readonly IProcessorService _processorService;
    private readonly IMapDataExtractorService _mapDataExtractorService;
    private readonly IJsonDataFileWriterProcessor _jsonDataFileWriterProcessor;
    private readonly IImageParserProcessor _imageParserProcessor;

    private readonly XmlMapDataParserProcessor _processor;

    public XmlMapDataParserProcessorTests()
    {
        _processorService = Substitute.For<IProcessorService>();
        _mapDataExtractorService = Substitute.For<IMapDataExtractorService>();
        _jsonDataFileWriterProcessor = Substitute.For<IJsonDataFileWriterProcessor>();
        _imageParserProcessor = Substitute.For<IImageParserProcessor>();

        _processor = new XmlMapDataParserProcessor(
            _processorService,
            _mapDataExtractorService,
            _jsonDataFileWriterProcessor,
            _imageParserProcessor);
    }

    [TestMethod]
    public async Task ExecuteMapParser_ExtractsDataAndSavesJsonAndImages()
    {
        // arrange
        SortedDictionary<string, Map> extractedMaps = new()
        {
            ["Map1"] = new Map("Map1"),
            ["Map2"] = new Map("Map2"),
        };

        _mapDataExtractorService.Extract(Arg.Any<Func<Map, Task>>())
            .Returns(extractedMaps);

        // act
        await _processor.ExecuteMapParser();

        // assert
        await _mapDataExtractorService.Received(1).Extract(Arg.Any<Func<Map, Task>>());

        _jsonDataFileWriterProcessor.Received(1).SaveJsonDataFileWrite(extractedMaps, null);
        _imageParserProcessor.Received(1).SaveImages(extractedMaps);
    }

    [TestMethod]
    public async Task ExecuteMapParser_PassesStartForMapSpecificDelegate()
    {
        // arrange
        SortedDictionary<string, Map> extractedMaps = [];

        Func<Map, Task>? capturedDelegate = null;

        _mapDataExtractorService.Extract(Arg.Any<Func<Map, Task>>())
            .Returns(callInfo =>
            {
                capturedDelegate = callInfo.Arg<Func<Map, Task>>();
                return extractedMaps;
            });

        // act
        await _processor.ExecuteMapParser();

        // assert
        capturedDelegate.Should().NotBeNull();

        Map testMap = new("TestMap");
        await capturedDelegate!(testMap);

        await _processorService.Received(1).StartForMapSpecific(testMap);
    }

    [TestMethod]
    public async Task ExecuteMapParser_EmptyResult_StillSavesJsonAndImages()
    {
        // arrange
        SortedDictionary<string, Map> extractedMaps = [];

        _mapDataExtractorService.Extract(Arg.Any<Func<Map, Task>>())
            .Returns(extractedMaps);

        // act
        await _processor.ExecuteMapParser();

        // assert
        _jsonDataFileWriterProcessor.Received(1).SaveJsonDataFileWrite(extractedMaps, null);
        _imageParserProcessor.Received(1).SaveImages(extractedMaps);
    }

    [TestMethod]
    public async Task ExecuteJsonDataFileWriteTask_DelegatesToJsonDataFileWriterProcessor()
    {
        // arrange
        _jsonDataFileWriterProcessor.ExecuteJsonDataFileWriteTasks().Returns(Task.CompletedTask);

        // act
        await _processor.ExecuteJsonDataFileWriteTask();

        // assert
        await _jsonDataFileWriterProcessor.Received(1).ExecuteJsonDataFileWriteTasks();
    }
}
