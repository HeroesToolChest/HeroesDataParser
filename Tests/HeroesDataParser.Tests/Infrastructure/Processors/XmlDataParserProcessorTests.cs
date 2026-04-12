using HeroesDataParser.Infrastructure.XmlDataParsers;
using Microsoft.Extensions.DependencyInjection;

namespace HeroesDataParser.Infrastructure.Processors.Tests;

[TestClass]
public class XmlDataParserProcessorTests
{
    private readonly IKeyedServiceProvider _keyedServiceProvider;
    private readonly IDataExtractorService _dataExtractorService;
    private readonly IJsonDataFileWriterProcessor _jsonDataFileWriterProcessor;
    private readonly IImageParserProcessor _imageParserProcessor;

    private readonly XmlDataParserProcessor _processor;

    public XmlDataParserProcessorTests()
    {
        _keyedServiceProvider = Substitute.For<IKeyedServiceProvider>();
        _dataExtractorService = Substitute.For<IDataExtractorService>();
        _jsonDataFileWriterProcessor = Substitute.For<IJsonDataFileWriterProcessor>();
        _imageParserProcessor = Substitute.For<IImageParserProcessor>();

        _processor = new XmlDataParserProcessor(
            _keyedServiceProvider,
            _dataExtractorService,
            _jsonDataFileWriterProcessor,
            _imageParserProcessor);
    }

    [TestMethod]
    public void GetAssociatedExtractDataParsers_ReturnsAllRegisteredOptions()
    {
        // act
        IEnumerable<ExtractDataOptions> result = _processor.GetAssociatedExtractDataParsers();

        // assert
        result.Should().BeEquivalentTo(new[]
        {
            ExtractDataOptions.Announcer,
            ExtractDataOptions.Banner,
            ExtractDataOptions.Bundle,
            ExtractDataOptions.Boost,
            ExtractDataOptions.Emoticon,
            ExtractDataOptions.EmoticonPack,
            ExtractDataOptions.Hero,
            ExtractDataOptions.LootChest,
            ExtractDataOptions.Mount,
            ExtractDataOptions.MatchAward,
            ExtractDataOptions.PortraitPack,
            ExtractDataOptions.RewardPortrait,
            ExtractDataOptions.Skin,
            ExtractDataOptions.Spray,
            ExtractDataOptions.TypeDescription,
            ExtractDataOptions.Unit,
            ExtractDataOptions.Veterancy,
            ExtractDataOptions.VoiceLine,
        });
    }

    [TestMethod]
    public void GetAssociatedExtractDataParsers_DoesNotContainSpecificOptions()
    {
        // act
        IEnumerable<ExtractDataOptions> result = _processor.GetAssociatedExtractDataParsers();

        // assert
        result.Should().NotContain([ExtractDataOptions.None, ExtractDataOptions.Map, ExtractDataOptions.All]);
    }

    [TestMethod]
    public void ExecuteDataParser_AnnouncerOption_ExtractsDataAndSavesJsonAndImages()
    {
        // arrange
        SortedDictionary<string, Announcer> extractedItems = new()
        {
            ["Announcer1"] = new Announcer("Announcer1"),
        };

        _dataExtractorService.Extract<Announcer, AnnouncerParser>(Arg.Any<AnnouncerParser>(), Arg.Any<Map?>())
            .Returns(extractedItems);

        // act
        _processor.ExecuteDataParser(ExtractDataOptions.Announcer, null);

        // assert
        _dataExtractorService.Received(1).Extract<Announcer, AnnouncerParser>(Arg.Any<AnnouncerParser>(), null);

        _jsonDataFileWriterProcessor.Received(1).SaveJsonDataFileWrite(extractedItems, null);
        _imageParserProcessor.Received(1).SaveImages(extractedItems);
    }

    [TestMethod]
    public void ExecuteDataParser_WithMap_PassesMapToExtractAndSave()
    {
        // arrange
        Map map = new("TestMap");

        SortedDictionary<string, Announcer> extractedItems = [];

        _dataExtractorService.Extract<Announcer, AnnouncerParser>(Arg.Any<AnnouncerParser>(), Arg.Any<Map?>())
            .Returns(extractedItems);

        // act
        _processor.ExecuteDataParser(ExtractDataOptions.Announcer, map);

        // assert
        _dataExtractorService.Received(1).Extract<Announcer, AnnouncerParser>(Arg.Any<AnnouncerParser>(), map);

        _jsonDataFileWriterProcessor.Received(1).SaveJsonDataFileWrite(extractedItems, map);
        _imageParserProcessor.Received(1).SaveImages(extractedItems);
    }

    [TestMethod]
    public void ExecuteDataParser_InvalidOption_ThrowsKeyNotFoundException()
    {
        // act & assert
        Action act = () => _processor.ExecuteDataParser(ExtractDataOptions.None, null);

        act.Should().Throw<KeyNotFoundException>();
    }

    [TestMethod]
    public async Task ExecuteJsonDataFileWriteTasks_DelegatesToJsonDataFileWriterProcessor()
    {
        // arrange
        _jsonDataFileWriterProcessor.ExecuteJsonDataFileWriteTasks().Returns(Task.CompletedTask);

        // act
        await _processor.ExecuteJsonDataFileWriteTasks();

        // assert
        await _jsonDataFileWriterProcessor.Received(1).ExecuteJsonDataFileWriteTasks();
    }
}
