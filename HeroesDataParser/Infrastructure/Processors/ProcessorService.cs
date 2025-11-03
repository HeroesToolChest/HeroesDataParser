using Serilog.Context;

namespace HeroesDataParser.Infrastructure.Processors;

public class ProcessorService : IProcessorService
{
    private readonly ILogger<ProcessorService> _logger;
    private readonly RootOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDataExtractorService _dataExtractorService;
    private readonly IJsonDataFileWriterService _jsonFileWriterService;
    private readonly IImageWriterService _imageWriterService;

    private readonly ExtractDataOptions _extractDataOptions;
    private readonly ExtractImageOptions _extractImageOptions;
    private readonly Dictionary<ExtractDataOptions, Func<Map?, Task>> _processElementByExtractDataOption;

    public ProcessorService(
        ILogger<ProcessorService> logger,
        IOptions<RootOptions> options,
        IServiceProvider serviceProvider,
        IDataExtractorService dataExtractorService,
        IJsonDataFileWriterService jsonFileWriterService,
        IImageWriterService imageWriterService)
    {
        _logger = logger;
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _dataExtractorService = dataExtractorService;
        _jsonFileWriterService = jsonFileWriterService;
        _imageWriterService = imageWriterService;

        _extractDataOptions = GetExtractDataOptions();
        _extractImageOptions = GetExtractImageOptions();

        _processElementByExtractDataOption = GetElementProcessors();
    }

    public ExtractDataOptions ExtractDataOptions => _extractDataOptions;

    public ExtractImageOptions ExtractImageOptions => _extractImageOptions;

    public async Task Start()
    {
        _logger.LogDebug("Available element processors {@ActionProcessors}", _processElementByExtractDataOption.Keys);

        await RunElementProcessors(_processElementByExtractDataOption);
    }

    public async Task StartForMap(Map map)
    {
        _logger.LogDebug("Available element processors {@ActionProcessors} for Map {MapId}", _processElementByExtractDataOption.Keys, map.Id);

        await RunElementProcessors(_processElementByExtractDataOption, map);
    }

    //public async Task RunElementProcessors()
    //{
    //    _logger.LogTrace("Available element processors {@ActionProcessors}", _processElementByExtractDataOption.Keys);

    //    foreach (var processor in _processElementByExtractDataOption)
    //    {
    //        if (_extractDataOptions.HasFlag(processor.Key))
    //        {
    //            await processor.Value();
    //        }
    //        else
    //        {
    //            _logger.LogTrace("Element processor {Processor} was not run, was not selected in options", processor.Key);
    //        }
    //    }
    //}

    //public async Task RunElementProcessorsForMap(Map map)
    //{
    //    _logger.LogTrace("Available element processors {@ActionProcessors}", _processElementByExtractDataOptionForMap.Keys);

    //    foreach (var processor in _processElementByExtractDataOptionForMap)
    //    {
    //        if (_extractDataOptions.HasFlag(processor.Key))
    //        {
    //            await processor.Value();
    //        }
    //        else
    //        {
    //            _logger.LogTrace("Element processor {Processor} was not run, was not selected in options", processor.Key);
    //        }
    //    }
    //}

    private async Task RunElementProcessors(Dictionary<ExtractDataOptions, Func<Map?, Task>> processors, Map? map = null)
    {
        foreach (KeyValuePair<ExtractDataOptions, Func<Map?, Task>> processor in processors)
        {
            if (_extractDataOptions.HasFlag(processor.Key))
            {
                await processor.Value(map);
            }
            else
            {
                _logger.LogTrace("Element processor {Processor} was not run, was not selected in options", processor.Key);
            }
        }
    }

    private async Task ProcessElementObject<TElementObject, TParser>(Map? map = null)
        where TElementObject : IElementObject
        where TParser : IDataParser<TElementObject>
    {
        string typeOfElementObjectName = typeof(TElementObject).Name;
        string typeOfParserName = typeof(TParser).Name;

        using (LogContext.PushProperty("ElementType", typeOfElementObjectName))
        using (LogContext.PushProperty("Parser", typeOfParserName))
        {
            _logger.LogInformation("Start action processor for {HeroesCollectionObject} using parser {Parser}", typeOfElementObjectName, typeOfParserName);

            var dataParser = _serviceProvider.GetRequiredService<IDataParser<TElementObject>>();
            var itemsToSerialize = _dataExtractorService.Extract<TElementObject, TParser>((TParser)dataParser, map);

            await WriteToJson(itemsToSerialize, map);
            SaveImages(itemsToSerialize);

            _logger.LogInformation("Action processor complete for {HeroesCollectionObject} using parser {Parser}", typeOfElementObjectName, typeOfParserName);
        }
    }

    private async Task WriteToJson<TElementObject>(Dictionary<string, TElementObject> itemsToSerialize, Map? map)
        where TElementObject : IElementObject
    {
        if (map is null)
            await _jsonFileWriterService.Write(itemsToSerialize);
        else
            await _jsonFileWriterService.WriteToMaps(map.Id, itemsToSerialize);
    }

    // parses and save the images for after data extraction/serialization
    private void SaveImages<TElementObject>(Dictionary<string, TElementObject> itemsToSerialize)
        where TElementObject : IElementObject
    {
        IEnumerable<IImageParser<TElementObject>> imageParsers = _serviceProvider.GetServices<IImageParser<TElementObject>>();

        if (!imageParsers.Any())
        {
            _logger.LogInformation("No image writers found for {ElementType}", typeof(TElementObject).Name);
            return;
        }

        foreach (IImageParser<TElementObject> imageParser in imageParsers)
        {
            if (_extractImageOptions.HasFlag(imageParser.ExtractImageOption))
            {
                _imageWriterService.Save(imageParser.GetImages(itemsToSerialize));
            }
        }
    }

    private ExtractDataOptions GetExtractDataOptions()
    {
        ExtractDataOptions selectDataExtractOptions = ExtractDataOptions.None;

        foreach (var extractDataOption in _options.Extractors)
        {
            if (!Enum.TryParse(extractDataOption.Key, true, out ExtractDataOptions result) || extractDataOption.Value.IsEnabled is false)
                continue;

            selectDataExtractOptions |= result;
        }

        _logger.LogDebug("Selected data extractors: {@DataOptions}", selectDataExtractOptions);

        return selectDataExtractOptions;
    }

    private ExtractImageOptions GetExtractImageOptions()
    {
        ExtractImageOptions selectImageExtractOptions = ExtractImageOptions.None;

        foreach (var extractDataOption in _options.Extractors)
        {
            if (extractDataOption.Key.Equals("hero", StringComparison.OrdinalIgnoreCase))
            {
                if (_options.Hidden.HeroImages.HeroPortraits)
                    selectImageExtractOptions |= ExtractImageOptions.HeroPortrait;
                if (_options.Hidden.HeroImages.Talents)
                    selectImageExtractOptions |= ExtractImageOptions.Talent;
                if (_options.Hidden.HeroImages.Abilities)
                    selectImageExtractOptions |= ExtractImageOptions.Ability;
                if (_options.Hidden.HeroImages.AbilityTalents)
                    selectImageExtractOptions |= ExtractImageOptions.AbilityTalent;
                if (_options.Hidden.HeroImages.HeroData)
                    selectImageExtractOptions |= ExtractImageOptions.HeroData;
                if (_options.Hidden.HeroImages.HeroDataSplit)
                    selectImageExtractOptions |= ExtractImageOptions.HeroDataSplit;

                continue;
            }

            if (extractDataOption.Key.Equals("map", StringComparison.OrdinalIgnoreCase))
            {
                if (_options.Hidden.MapImages.ReplayPreviews)
                    selectImageExtractOptions |= ExtractImageOptions.ReplayPreview;
                if (_options.Hidden.MapImages.LoadingScreens)
                    selectImageExtractOptions |= ExtractImageOptions.LoadingScreen;
                if (_options.Hidden.MapImages.MapObjectiveIcons)
                    selectImageExtractOptions |= ExtractImageOptions.MapObjectives;

                continue;
            }

            if (!Enum.TryParse(extractDataOption.Key, true, out ExtractImageOptions result) || extractDataOption.Value.IsEnabled is false || extractDataOption.Value.Images is false)
                continue;

            selectImageExtractOptions |= result;
        }

        _logger.LogDebug("Selected image extractors: {@ImageOptions}", selectImageExtractOptions);

        return selectImageExtractOptions;
    }

    //private Dictionary<ExtractDataOptions, Func<Dictionary<string, IElementObject>>> GetElementProcessors() => new()
    //{
    //    { ExtractDataOptions.Announcer, ProcessHeroesCollectionObject<Announcer, AnnouncerParser> },
    //    { ExtractDataOptions.Banner, ProcessHeroesCollectionObject<Banner, BannerParser> },
    //    //{ ExtractDataOptions.Map, ProcessMapObject },
    //};

    private Dictionary<ExtractDataOptions, Func<Map?, Task>> GetElementProcessors() => new()
    {
        { ExtractDataOptions.Announcer, ProcessElementObject<Announcer, AnnouncerParser> },
        { ExtractDataOptions.Banner, ProcessElementObject<Banner, BannerParser> },
        { ExtractDataOptions.Bundle, ProcessElementObject<Bundle, BundleParser> },
        { ExtractDataOptions.Boost, ProcessElementObject<Boost, BoostParser> },
        { ExtractDataOptions.Hero, ProcessElementObject<Hero, HeroParser> },
        { ExtractDataOptions.LootChest, ProcessElementObject<LootChest, LootChestParser> },
        { ExtractDataOptions.Unit, ProcessElementObject<Unit, UnitParser> },
    };

    // private Dictionary<ExtractImageOptions, >
}
