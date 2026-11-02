using HeroesDataParser.Infrastructure.ImageParsers;

namespace HeroesDataParser.Infrastructure.Processors.Tests;

[TestClass]
public class ImageParserProcessorTests
{
    private readonly ILogger<ImageParserProcessor> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IImageWriterService _imageWriterService;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public ImageParserProcessorTests()
    {
        _logger = Substitute.For<ILogger<ImageParserProcessor>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _imageWriterService = Substitute.For<IImageWriterService>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
    }

    [TestMethod]
    public void SaveImages_NoImageParsersAvailable_LogsInformationAndReturns()
    {
        // arrange
        _options.Value.Returns(new RootOptions());

        // only Map has an image parser, Banner has none
        IServiceProvider serviceProvider = CreateKeyedServiceProvider(new ReplayPreviewImageParser(Substitute.For<ILogger<ReplayPreviewImageParser>>(), _heroesXmlLoaderService));

        ImageParserProcessor processor = new(_logger, _options, serviceProvider, _imageWriterService);

        SortedDictionary<string, Banner> items = new()
        {
            { "item1", new Banner("item1") },
        };

        // act
        processor.SaveImages(items);

        // assert
        _imageWriterService.DidNotReceive().Save(Arg.Any<HashSet<ImageWriterFile>>());
        GetLoggedMessages(_logger).Should().ContainSingle()
            .Which.Should().Be((LogLevel.Information, "No image writers available for Banner"));
    }

    [TestMethod]
    public void SaveImages_HasMatchingImageParsers_SavesImages()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractImageOptions = ExtractImageOptions.ReplayPreview | ExtractImageOptions.LoadingScreen,
        });

        IServiceProvider serviceProvider = CreateKeyedServiceProvider(
            new ReplayPreviewImageParser(Substitute.For<ILogger<ReplayPreviewImageParser>>(), _heroesXmlLoaderService),
            new LoadingScreenImageParser(Substitute.For<ILogger<LoadingScreenImageParser>>(), _heroesXmlLoaderService));

        ImageParserProcessor processor = new(_logger, _options, serviceProvider, _imageWriterService);

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

        IServiceProvider serviceProvider = CreateKeyedServiceProvider(new AnnouncerPackImageParser(Substitute.For<ILogger<AnnouncerPackImageParser>>(), _heroesXmlLoaderService));

        ImageParserProcessor processor = new(_logger, _options, serviceProvider, _imageWriterService);

        SortedDictionary<string, AnnouncerPack> items = new()
        {
            { "item1", new AnnouncerPack("item1") },
        };

        // act
        processor.SaveImages(items);

        // assert
        _imageWriterService.DidNotReceive().Save(Arg.Any<HashSet<ImageWriterFile>>());
    }

    [TestMethod]
    public void SaveImages_HasImageParsers_DoesNotLogNoImageWritersMessage()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractImageOptions = ExtractImageOptions.ReplayPreview,
        });

        IServiceProvider serviceProvider = CreateKeyedServiceProvider(CreateImageParser<Map>(ExtractImageOptions.ReplayPreview));

        ImageParserProcessor processor = new(_logger, _options, serviceProvider, _imageWriterService);

        SortedDictionary<string, Map> items = new()
        {
            { "item1", new Map("item1") },
        };

        // act
        processor.SaveImages(items);

        // assert
        GetLoggedMessages(_logger).Should().BeEmpty();
    }

    [TestMethod]
    public void SaveImages_HasMatchingImageParser_PassesItemsToImageParser()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractImageOptions = ExtractImageOptions.ReplayPreview,
        });

        IImageParser<Map> imageParser = CreateImageParser<Map>(ExtractImageOptions.ReplayPreview);

        ImageParserProcessor processor = new(_logger, _options, CreateKeyedServiceProvider(imageParser), _imageWriterService);

        SortedDictionary<string, Map> items = new()
        {
            { "item1", new Map("item1") },
            { "item2", new Map("item2") },
        };

        // act
        processor.SaveImages(items);

        // assert
        imageParser.Received(1).GetImages(items);
    }

    [TestMethod]
    public void SaveImages_HasMatchingImageParser_SavesImagesReturnedByImageParser()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractImageOptions = ExtractImageOptions.ReplayPreview,
        });

        HashSet<ImageWriterFile> images = CreateImages("replay_preview_1.png", "replay_preview_2.png");
        IImageParser<Map> imageParser = CreateImageParser<Map>(ExtractImageOptions.ReplayPreview, images);

        ImageParserProcessor processor = new(_logger, _options, CreateKeyedServiceProvider(imageParser), _imageWriterService);

        SortedDictionary<string, Map> items = new()
        {
            { "item1", new Map("item1") },
        };

        // act
        processor.SaveImages(items);

        // assert
        _imageWriterService.Received(1).Save(Arg.Any<HashSet<ImageWriterFile>>());
        _imageWriterService.Received(1).Save(images);
    }

    [TestMethod]
    public void SaveImages_HasNonMatchingImageParser_DoesNotGetImagesFromImageParser()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractImageOptions = ExtractImageOptions.LoadingScreen,
        });

        IImageParser<Map> imageParser = CreateImageParser<Map>(ExtractImageOptions.ReplayPreview);

        ImageParserProcessor processor = new(_logger, _options, CreateKeyedServiceProvider(imageParser), _imageWriterService);

        SortedDictionary<string, Map> items = new()
        {
            { "item1", new Map("item1") },
        };

        // act
        processor.SaveImages(items);

        // assert
        imageParser.DidNotReceive().GetImages(Arg.Any<SortedDictionary<string, Map>>());
        _imageWriterService.DidNotReceive().Save(Arg.Any<HashSet<ImageWriterFile>>());
    }

    [TestMethod]
    public void SaveImages_HasMatchingAndNonMatchingImageParsers_OnlySavesImagesFromMatchingImageParsers()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractImageOptions = ExtractImageOptions.ReplayPreview | ExtractImageOptions.MapObjectives,
        });

        HashSet<ImageWriterFile> replayPreviewImages = CreateImages("replay_preview.png");
        HashSet<ImageWriterFile> loadingScreenImages = CreateImages("loading_screen.png");
        HashSet<ImageWriterFile> mapObjectiveImages = CreateImages("map_objective.png");

        IImageParser<Map> replayPreviewImageParser = CreateImageParser<Map>(ExtractImageOptions.ReplayPreview, replayPreviewImages);
        IImageParser<Map> loadingScreenImageParser = CreateImageParser<Map>(ExtractImageOptions.LoadingScreen, loadingScreenImages);
        IImageParser<Map> mapObjectiveImageParser = CreateImageParser<Map>(ExtractImageOptions.MapObjectives, mapObjectiveImages);

        IServiceProvider serviceProvider = CreateKeyedServiceProvider(replayPreviewImageParser, loadingScreenImageParser, mapObjectiveImageParser);

        ImageParserProcessor processor = new(_logger, _options, serviceProvider, _imageWriterService);

        SortedDictionary<string, Map> items = new()
        {
            { "item1", new Map("item1") },
        };

        // act
        processor.SaveImages(items);

        // assert
        replayPreviewImageParser.Received(1).GetImages(items);
        loadingScreenImageParser.DidNotReceive().GetImages(Arg.Any<SortedDictionary<string, Map>>());
        mapObjectiveImageParser.Received(1).GetImages(items);

        _imageWriterService.Received(2).Save(Arg.Any<HashSet<ImageWriterFile>>());
        _imageWriterService.Received(1).Save(replayPreviewImages);
        _imageWriterService.Received(1).Save(mapObjectiveImages);
        _imageWriterService.DidNotReceive().Save(loadingScreenImages);
    }

    [TestMethod]
    [DataRow(ExtractImageOptions.None, ExtractImageOptions.None)]
    [DataRow(ExtractImageOptions.HeroPortrait, ExtractImageOptions.HeroPortrait)]
    [DataRow(ExtractImageOptions.HeroData, ExtractImageOptions.HeroPortrait | ExtractImageOptions.AbilityTalent)]
    [DataRow(ExtractImageOptions.HeroDataSplit, ExtractImageOptions.HeroPortrait | ExtractImageOptions.Ability | ExtractImageOptions.Talent)]
    [DataRow(ExtractImageOptions.UnitData, ExtractImageOptions.AbilityTalent)]
    [DataRow(ExtractImageOptions.AllSplit, ExtractImageOptions.HeroPortrait | ExtractImageOptions.Ability | ExtractImageOptions.Talent)]
    [DataRow(ExtractImageOptions.All, ExtractImageOptions.HeroPortrait | ExtractImageOptions.Ability | ExtractImageOptions.Talent | ExtractImageOptions.AbilityTalent)]
    public void SaveImages_CombinedExtractImageOptions_OnlyRunsImageParsersIncludedInOptions(ExtractImageOptions extractImageOptions, ExtractImageOptions expectedImageParserOptions)
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractImageOptions = extractImageOptions,
        });

        IImageParser<Hero>[] imageParsers =
        [
            CreateImageParser<Hero>(ExtractImageOptions.HeroPortrait),
            CreateImageParser<Hero>(ExtractImageOptions.Ability),
            CreateImageParser<Hero>(ExtractImageOptions.Talent),
            CreateImageParser<Hero>(ExtractImageOptions.AbilityTalent),
        ];

        ImageParserProcessor processor = new(_logger, _options, CreateKeyedServiceProvider(imageParsers), _imageWriterService);

        SortedDictionary<string, Hero> items = new()
        {
            { "hero1", new Hero("hero1") },
        };

        // act
        processor.SaveImages(items);

        // assert
        int expectedSaveCount = 0;

        foreach (IImageParser<Hero> imageParser in imageParsers)
        {
            ExtractImageOptions imageParserOption = imageParser.ExtractImageOption;

            if ((expectedImageParserOptions & imageParserOption) == imageParserOption)
            {
                imageParser.Received(1).GetImages(items);
                expectedSaveCount++;
            }
            else
            {
                imageParser.DidNotReceive().GetImages(Arg.Any<SortedDictionary<string, Hero>>());
            }
        }

        _imageWriterService.Received(expectedSaveCount).Save(Arg.Any<HashSet<ImageWriterFile>>());
    }

    [TestMethod]
    public void SaveImages_HasMultipleMatchingImageParsers_SavesImagesInRegistrationOrder()
    {
        // arrange
        // order matters because the image writer keeps the first image it receives for a duplicate file
        _options.Value.Returns(new RootOptions()
        {
            ExtractImageOptions = ExtractImageOptions.All,
        });

        HashSet<ImageWriterFile> firstImages = CreateImages("first.png");
        HashSet<ImageWriterFile> secondImages = CreateImages("second.png");
        HashSet<ImageWriterFile> thirdImages = CreateImages("third.png");

        IServiceProvider serviceProvider = CreateKeyedServiceProvider(
            CreateImageParser<Map>(ExtractImageOptions.MapObjectives, firstImages),
            CreateImageParser<Map>(ExtractImageOptions.ReplayPreview, secondImages),
            CreateImageParser<Map>(ExtractImageOptions.LoadingScreen, thirdImages));

        ImageParserProcessor processor = new(_logger, _options, serviceProvider, _imageWriterService);

        SortedDictionary<string, Map> items = new()
        {
            { "item1", new Map("item1") },
        };

        // act
        processor.SaveImages(items);

        // assert
        Received.InOrder(() =>
        {
            _imageWriterService.Save(firstImages);
            _imageWriterService.Save(secondImages);
            _imageWriterService.Save(thirdImages);
        });
    }

    [TestMethod]
    public void SaveImages_ImageParserReturnsNoImages_SavesEmptySet()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractImageOptions = ExtractImageOptions.ReplayPreview,
        });

        HashSet<ImageWriterFile> images = [];
        IImageParser<Map> imageParser = CreateImageParser<Map>(ExtractImageOptions.ReplayPreview, images);

        ImageParserProcessor processor = new(_logger, _options, CreateKeyedServiceProvider(imageParser), _imageWriterService);

        SortedDictionary<string, Map> items = [];

        // act
        processor.SaveImages(items);

        // assert
        imageParser.Received(1).GetImages(items);
        _imageWriterService.Received(1).Save(images);
    }

    [TestMethod]
    public void SaveImages_ImageParsersNotKeyedByElementType_AreNotUsed()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            ExtractImageOptions = ExtractImageOptions.All,
        });

        IImageParser<Map> unkeyedImageParser = CreateImageParser<Map>(ExtractImageOptions.ReplayPreview, CreateImages("unkeyed.png"));
        IImageParser<Map> wrongKeyImageParser = CreateImageParser<Map>(ExtractImageOptions.LoadingScreen, CreateImages("wrong_key.png"));

        IServiceProvider serviceProvider = new ServiceCollection()
            .AddSingleton(unkeyedImageParser)
            .AddKeyedSingleton<IImageParser<Map>>(typeof(Hero), wrongKeyImageParser)
            .BuildServiceProvider();

        ImageParserProcessor processor = new(_logger, _options, serviceProvider, _imageWriterService);

        SortedDictionary<string, Map> items = new()
        {
            { "item1", new Map("item1") },
        };

        // act
        processor.SaveImages(items);

        // assert
        unkeyedImageParser.DidNotReceive().GetImages(Arg.Any<SortedDictionary<string, Map>>());
        wrongKeyImageParser.DidNotReceive().GetImages(Arg.Any<SortedDictionary<string, Map>>());
        _imageWriterService.DidNotReceive().Save(Arg.Any<HashSet<ImageWriterFile>>());
        GetLoggedMessages(_logger).Should().ContainSingle()
            .Which.Should().Be((LogLevel.Information, "No image writers available for Map"));
    }

    private static IImageParser<T> CreateImageParser<T>(ExtractImageOptions extractImageOption, HashSet<ImageWriterFile>? images = null)
        where T : IElementObject
    {
        IImageParser<T> imageParser = Substitute.For<IImageParser<T>>();

        imageParser.ExtractImageOption.Returns(extractImageOption);
        imageParser.GetImages(Arg.Any<SortedDictionary<string, T>>()).Returns(images ?? []);

        return imageParser;
    }

    private static ServiceProvider CreateKeyedServiceProvider<T>(params IImageParser<T>[] imageParsers)
        where T : IElementObject
    {
        ServiceCollection services = new();

        foreach (IImageParser<T> imageParser in imageParsers)
        {
            services.AddKeyedSingleton<IImageParser<T>>(typeof(T), imageParser);
        }

        return services.BuildServiceProvider();
    }

    private static HashSet<ImageWriterFile> CreateImages(params string[] fileNames) =>
        fileNames.Select(fileName => new ImageWriterFile()
        {
            SubDirectoryPath = "images",
            FileName = fileName,
            ElementId = "item1",
            ProcessImageFile = _ => Task.CompletedTask,
        }).ToHashSet();

    // the state argument (index 2) of ILogger.Log formats to the final log message
    private static List<(LogLevel LogLevel, string? Message)> GetLoggedMessages(ILogger logger) =>
        logger.ReceivedCalls()
            .Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log))
            .Select(call =>
            {
                object?[] arguments = call.GetArguments();

                return ((LogLevel)arguments[0]!, arguments[2]?.ToString());
            })
            .ToList();
}
