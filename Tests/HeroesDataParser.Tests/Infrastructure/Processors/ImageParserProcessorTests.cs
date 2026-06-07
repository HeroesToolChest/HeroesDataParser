using HeroesDataParser.Infrastructure.ImageParsers;
using Microsoft.Extensions.DependencyInjection;

namespace HeroesDataParser.Infrastructure.Processors.Tests;

[TestClass]
public class ImageParserProcessorTests
{
    private readonly ILogger<ImageParserProcessor> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IKeyedServiceProvider _keyedServiceProvider;
    private readonly IImageWriterService _imageWriterService;

    public ImageParserProcessorTests()
    {
        _logger = Substitute.For<ILogger<ImageParserProcessor>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _keyedServiceProvider = new ServiceCollection()
            .AddKeyedSingleton<IImageParser<Map>, ReplayPreviewImageParser>(typeof(Map))
            .AddKeyedSingleton<IImageParser<Map>, LoadingScreenImageParser>(typeof(Map))
            .AddSingleton<ILogger<ReplayPreviewImageParser>, ILogger<ReplayPreviewImageParser>>(sp => Substitute.For<ILogger<ReplayPreviewImageParser>>())
            .AddSingleton<ILogger<LoadingScreenImageParser>, ILogger<LoadingScreenImageParser>>(sp => Substitute.For<ILogger<LoadingScreenImageParser>>())
            .AddKeyedSingleton<IImageParser<AnnouncerPack>, AnnouncerPackImageParser>(typeof(AnnouncerPack))
            .AddSingleton<ILogger<AnnouncerPackImageParser>, ILogger<AnnouncerPackImageParser>>(sp => Substitute.For<ILogger<AnnouncerPackImageParser>>())
            .AddSingleton<IHeroesXmlLoaderService, IHeroesXmlLoaderService>(sp => Substitute.For<IHeroesXmlLoaderService>())
            .BuildServiceProvider();
        _imageWriterService = Substitute.For<IImageWriterService>();
    }

    [TestMethod]
    public void SaveImages_NoImageParsersAvailable_LogsInformationAndReturns()
    {
        // arrange
        _options.Value.Returns(new RootOptions());

        ImageParserProcessor processor = new(_logger, _options, _keyedServiceProvider, _imageWriterService);

        SortedDictionary<string, Banner> items = new()
        {
            { "item1", new Banner("item1") },
        };

        // act
        processor.SaveImages(items);

        // assert
        _imageWriterService.DidNotReceive().Save(Arg.Any<HashSet<ImageWriterFile>>());
    }

    [TestMethod]
    public void SaveImages_HasMatchingImageParsers_SavesImages()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractImageOptions = ExtractImageOptions.ReplayPreview | ExtractImageOptions.LoadingScreen,
        });

        ImageParserProcessor processor = new(_logger, _options, _keyedServiceProvider, _imageWriterService);

        SortedDictionary<string, Map> items = new()
        {
            { "item1", new Map("item1") },
        };

        // act
        processor.SaveImages(items);

        // assert
        _imageWriterService.Received(2).Save(Arg.Any<HashSet<ImageWriterFile>>());
    }

    [TestMethod]
    public void SaveImages_HasNonMatchingImageParsers_DoesNotSaveImages()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractImageOptions = ExtractImageOptions.UnitPortrait,
        });

        ImageParserProcessor processor = new(_logger, _options, _keyedServiceProvider, _imageWriterService);

        SortedDictionary<string, AnnouncerPack> items = new()
        {
            { "item1", new AnnouncerPack("item1") },
        };

        // act
        processor.SaveImages(items);

        // assert
        _imageWriterService.DidNotReceive().Save(Arg.Any<HashSet<ImageWriterFile>>());
    }
}
