namespace HeroesDataParser.Infrastructure.Processors.Tests;

[TestClass]
public class JsonGameStringFileWriterProcessorTests
{
    private readonly IOptions<RootOptions> _options;
    private readonly ILogger<JsonGameStringFileWriterProcessor> _logger;
    private readonly IJsonGameStringFileWriterService _jsonGameStringFileWriterService;

    public JsonGameStringFileWriterProcessorTests()
    {
        _options = Substitute.For<IOptions<RootOptions>>();
        _logger = Substitute.For<ILogger<JsonGameStringFileWriterProcessor>>();
        _jsonGameStringFileWriterService = Substitute.For<IJsonGameStringFileWriterService>();
    }

    [TestMethod]
    public async Task WriteGameStringFile_LocalizedTextIsNone_LogsAndSkips()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            LocalizedText = LocalizedTextOption.None,
        };

        _options.Value.Returns(rootOptions);

        JsonGameStringFileWriterProcessor processor = new(_logger, _options, _jsonGameStringFileWriterService);

        // act
        await processor.WriteGameStringFile(null);

        // assert
        await _jsonGameStringFileWriterService.DidNotReceive().WriteBase();
        await _jsonGameStringFileWriterService.DidNotReceive().WriteMapSpecific(Arg.Any<Map>());
    }

    [TestMethod]
    public async Task WriteGameStringFile_WithMapName_CallsWriteMapSpecific()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            LocalizedText = LocalizedTextOption.Extract,
        };

        _options.Value.Returns(rootOptions);

        JsonGameStringFileWriterProcessor processor = new(_logger, _options, _jsonGameStringFileWriterService);

        // act
        await processor.WriteGameStringFile(new Map("TestMap"));

        // assert
        await _jsonGameStringFileWriterService.Received(1).WriteMapSpecific(new Map("TestMap"));
        await _jsonGameStringFileWriterService.DidNotReceive().WriteBase();
    }

    [TestMethod]
    public async Task WriteGameStringFile_WithNullMapName_CallsWriteBase()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            LocalizedText = LocalizedTextOption.Extract,
        };

        _options.Value.Returns(rootOptions);

        JsonGameStringFileWriterProcessor processor = new(_logger, _options, _jsonGameStringFileWriterService);

        // act
        await processor.WriteGameStringFile(null);

        // assert
        await _jsonGameStringFileWriterService.Received(1).WriteBase();
        await _jsonGameStringFileWriterService.DidNotReceive().WriteMapSpecific(Arg.Any<Map>());
    }

    [TestMethod]
    public async Task WriteMapGameStringFile_LocalizedTextIsNone_LogsAndSkips()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            LocalizedText = LocalizedTextOption.None,
        };

        _options.Value.Returns(rootOptions);

        JsonGameStringFileWriterProcessor processor = new(_logger, _options, _jsonGameStringFileWriterService);

        // act
        await processor.WriteMapGameStringFile();

        // assert
        await _jsonGameStringFileWriterService.DidNotReceive().WriteMap();
    }

    [TestMethod]
    public async Task WriteMapGameStringFile_LocalizedTextIsExtract_CallsWriteMap()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            LocalizedText = LocalizedTextOption.Extract,
        };

        _options.Value.Returns(rootOptions);

        JsonGameStringFileWriterProcessor processor = new(_logger, _options, _jsonGameStringFileWriterService);

        // act
        await processor.WriteMapGameStringFile();

        // assert
        await _jsonGameStringFileWriterService.Received(1).WriteMap();
    }

    [TestMethod]
    public async Task WriteMapGameStringFile_LocalizedTextIsCopy_CallsWriteMap()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            LocalizedText = LocalizedTextOption.Copy,
        };

        _options.Value.Returns(rootOptions);

        JsonGameStringFileWriterProcessor processor = new(_logger, _options, _jsonGameStringFileWriterService);

        // act
        await processor.WriteMapGameStringFile();

        // assert
        await _jsonGameStringFileWriterService.Received(1).WriteMap();
    }
}
