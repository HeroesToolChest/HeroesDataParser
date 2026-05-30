namespace HeroesDataParser.Infrastructure.Processors.Tests;

[TestClass]
public class ProcessorServiceTests
{
    private readonly IOptions<RootOptions> _options;
    private readonly IXmlDataParserProcessor _xmlDataParserProcessor;
    private readonly IJsonGameStringFileWriterProcessor _jsonGameStringFileWriterProcessor;

    public ProcessorServiceTests()
    {
        _options = Substitute.For<IOptions<RootOptions>>();
        _xmlDataParserProcessor = Substitute.For<IXmlDataParserProcessor>();
        _jsonGameStringFileWriterProcessor = Substitute.For<IJsonGameStringFileWriterProcessor>();
    }

    [TestMethod]
    public async Task Start_HasMatchingExtractDataOptions_ExecutesMatchingParsers()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractDataOptions = ExtractDataOptions.Hero | ExtractDataOptions.Unit,
        });

        _xmlDataParserProcessor.GetAssociatedExtractDataParsers().Returns(
        [
            ExtractDataOptions.Hero,
            ExtractDataOptions.Unit,
            ExtractDataOptions.Mount,
        ]);

        ProcessorService service = new(_options, _xmlDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.Start();

        // assert
        _xmlDataParserProcessor.Received(1).ExecuteDataParser(ExtractDataOptions.Hero, null);
        _xmlDataParserProcessor.Received(1).ExecuteDataParser(ExtractDataOptions.Unit, null);
        _xmlDataParserProcessor.DidNotReceive().ExecuteDataParser(ExtractDataOptions.Mount, Arg.Any<Map?>());

        await _xmlDataParserProcessor.Received(1).ExecuteJsonDataFileWriteTasks();
        await _jsonGameStringFileWriterProcessor.Received(1).WriteGameStringFile(null);
    }

    [TestMethod]
    public async Task Start_HasNoMatchingExtractDataOptions_DoesNotExecuteAnyParser()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractDataOptions = ExtractDataOptions.None,
        });

        _xmlDataParserProcessor.GetAssociatedExtractDataParsers().Returns(
        [
            ExtractDataOptions.Hero,
            ExtractDataOptions.Unit,
        ]);

        ProcessorService service = new(_options, _xmlDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.Start();

        // assert
        _xmlDataParserProcessor.DidNotReceive().ExecuteDataParser(Arg.Any<ExtractDataOptions>(), Arg.Any<Map?>());
        await _xmlDataParserProcessor.Received(1).ExecuteJsonDataFileWriteTasks();

        await _jsonGameStringFileWriterProcessor.Received(1).WriteGameStringFile(null);
    }

    [TestMethod]
    public async Task Start_HasNoAssociatedParsers_OnlyExecutesWriteTasks()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractDataOptions = ExtractDataOptions.Hero,
        });

        _xmlDataParserProcessor.GetAssociatedExtractDataParsers().Returns([]);

        ProcessorService service = new(_options, _xmlDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.Start();

        // assert
        _xmlDataParserProcessor.DidNotReceive().ExecuteDataParser(Arg.Any<ExtractDataOptions>(), Arg.Any<Map?>());

        await _xmlDataParserProcessor.Received(1).ExecuteJsonDataFileWriteTasks();
        await _jsonGameStringFileWriterProcessor.Received(1).WriteGameStringFile(null);
    }

    [TestMethod]
    public async Task StartForMapSpecific_HasMatchingExtractDataOptions_ExecutesParsersWithMap()
    {
        // arrange
        Map map = new("MapId1");

        _options.Value.Returns(new RootOptions()
        {
            ExtractDataOptions = ExtractDataOptions.Hero | ExtractDataOptions.Spray,
        });

        _xmlDataParserProcessor.GetAssociatedExtractDataParsers().Returns(
        [
            ExtractDataOptions.Hero,
            ExtractDataOptions.Spray,
        ]);

        ProcessorService service = new(_options, _xmlDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.StartForMapSpecific(map);

        // assert
        _xmlDataParserProcessor.Received(1).ExecuteDataParser(ExtractDataOptions.Hero, map);
        _xmlDataParserProcessor.Received(1).ExecuteDataParser(ExtractDataOptions.Spray, map);

        await _xmlDataParserProcessor.Received(1).ExecuteJsonDataFileWriteTasks();
        await _jsonGameStringFileWriterProcessor.Received(1).WriteGameStringFile(map);
    }

    [TestMethod]
    public async Task StartForMapSpecific_HasNoMatchingExtractDataOptions_DoesNotExecuteAnyParser()
    {
        // arrange
        Map map = new("MapId1");

        _options.Value.Returns(new RootOptions()
        {
            ExtractDataOptions = ExtractDataOptions.None,
        });

        _xmlDataParserProcessor.GetAssociatedExtractDataParsers().Returns(
        [
            ExtractDataOptions.Hero,
        ]);

        ProcessorService service = new(_options, _xmlDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.StartForMapSpecific(map);

        // assert
        _xmlDataParserProcessor.DidNotReceive().ExecuteDataParser(Arg.Any<ExtractDataOptions>(), Arg.Any<Map?>());

        await _xmlDataParserProcessor.Received(1).ExecuteJsonDataFileWriteTasks();
        await _jsonGameStringFileWriterProcessor.Received(1).WriteGameStringFile(map);
    }

    [TestMethod]
    public async Task Start_AllExtractDataOptions_ExecutesAllAssociatedParsers()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractDataOptions = ExtractDataOptions.All,
        });

        _xmlDataParserProcessor.GetAssociatedExtractDataParsers().Returns(
        [
            ExtractDataOptions.Hero,
            ExtractDataOptions.Unit,
            ExtractDataOptions.MatchAward,
        ]);

        ProcessorService service = new(_options, _xmlDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.Start();

        // assert
        _xmlDataParserProcessor.Received(1).ExecuteDataParser(ExtractDataOptions.Hero, null);
        _xmlDataParserProcessor.Received(1).ExecuteDataParser(ExtractDataOptions.Unit, null);
        _xmlDataParserProcessor.Received(1).ExecuteDataParser(ExtractDataOptions.MatchAward, null);

        await _xmlDataParserProcessor.Received(1).ExecuteJsonDataFileWriteTasks();
        await _jsonGameStringFileWriterProcessor.Received(1).WriteGameStringFile(null);
    }

    [TestMethod]
    public async Task Start_PartialFlagMatch_ExecutesOnlyMatchingParsers()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractDataOptions = ExtractDataOptions.Skin | ExtractDataOptions.Banner,
        });

        _xmlDataParserProcessor.GetAssociatedExtractDataParsers().Returns(
        [
            ExtractDataOptions.Hero,
            ExtractDataOptions.Skin,
            ExtractDataOptions.Banner,
            ExtractDataOptions.Announcer,
        ]);

        ProcessorService service = new(_options, _xmlDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.Start();

        // assert
        _xmlDataParserProcessor.DidNotReceive().ExecuteDataParser(ExtractDataOptions.Hero, Arg.Any<Map?>());
        _xmlDataParserProcessor.DidNotReceive().ExecuteDataParser(ExtractDataOptions.Announcer, Arg.Any<Map?>());
        _xmlDataParserProcessor.Received(1).ExecuteDataParser(ExtractDataOptions.Skin, null);
        _xmlDataParserProcessor.Received(1).ExecuteDataParser(ExtractDataOptions.Banner, null);

        await _xmlDataParserProcessor.Received(1).ExecuteJsonDataFileWriteTasks();
        await _jsonGameStringFileWriterProcessor.Received(1).WriteGameStringFile(null);
    }

    [TestMethod]
    public async Task StartForMapSpecific_MapWithNullId_PassesNullToWriteGameStringFile()
    {
        // arrange
        Map map = new("MapId1");

        _options.Value.Returns(new RootOptions()
        {
            ExtractDataOptions = ExtractDataOptions.None,
        });

        _xmlDataParserProcessor.GetAssociatedExtractDataParsers().Returns([]);

        ProcessorService service = new(_options, _xmlDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.StartForMapSpecific(map);

        // assert
        await _jsonGameStringFileWriterProcessor.Received(1).WriteGameStringFile(map);
        map.Id.Should().Be("MapId1");
    }

    [TestMethod]
    public async Task Start_ExecuteJsonDataFileWriteTasks_CalledAfterParsers()
    {
        // arrange
        List<string> callOrder = [];

        _options.Value.Returns(new RootOptions()
        {
            ExtractDataOptions = ExtractDataOptions.Hero,
        });

        _xmlDataParserProcessor.GetAssociatedExtractDataParsers().Returns(
        [
            ExtractDataOptions.Hero,
        ]);

        _xmlDataParserProcessor.When(x => x.ExecuteDataParser(Arg.Any<ExtractDataOptions>(), Arg.Any<Map?>()))
            .Do(_ => callOrder.Add(nameof(IXmlDataParserProcessor.ExecuteDataParser)));

        _xmlDataParserProcessor.ExecuteJsonDataFileWriteTasks().Returns(x =>
        {
            callOrder.Add(nameof(IXmlDataParserProcessor.ExecuteJsonDataFileWriteTasks));
            return Task.CompletedTask;
        });

        _jsonGameStringFileWriterProcessor.WriteGameStringFile(Arg.Any<Map?>()).Returns(x =>
        {
            callOrder.Add(nameof(IJsonGameStringFileWriterProcessor.WriteGameStringFile));
            return Task.CompletedTask;
        });

        ProcessorService service = new(_options, _xmlDataParserProcessor, _jsonGameStringFileWriterProcessor);

        // act
        await service.Start();

        // assert
        callOrder.Should().BeEquivalentTo(
            new[]
            {
                nameof(IXmlDataParserProcessor.ExecuteDataParser),
                nameof(IXmlDataParserProcessor.ExecuteJsonDataFileWriteTasks),
                nameof(IJsonGameStringFileWriterProcessor.WriteGameStringFile),
            },
            options => options.WithStrictOrdering());
    }
}
