using Serilog.Context;

namespace HeroesDataParser.Infrastructure.Processors;

public class ProcessorService : IProcessorService
{
    private readonly ILogger<ProcessorService> _logger;
    private readonly RootOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDataExtractorService _dataExtractorService;
    private readonly IJsonDataFileWriterService _jsonDataFileWriterService;
    private readonly IJsonGameStringFileWriterService _jsonGameStringFileWriterService;
    private readonly IImageWriterService _imageWriterService;

    private readonly ExtractDataOptions _extractDataOptions;
    private readonly ExtractImageOptions _extractImageOptions;
    private readonly Dictionary<ExtractDataOptions, Action<Map?>> _processElementByExtractDataOption;

    private readonly List<Func<Task>> _dataWriterTasks = [];

    public ProcessorService(
        ILogger<ProcessorService> logger,
        IOptions<RootOptions> options,
        IServiceProvider serviceProvider,
        IDataExtractorService dataExtractorService,
        IJsonDataFileWriterService jsonFileWriterService,
        IJsonGameStringFileWriterService jsonGameStringFileWriterService,
        IImageWriterService imageWriterService)
    {
        _logger = logger;
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _dataExtractorService = dataExtractorService;
        _jsonDataFileWriterService = jsonFileWriterService;
        _jsonGameStringFileWriterService = jsonGameStringFileWriterService;
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

    private async Task RunElementProcessors(Dictionary<ExtractDataOptions, Action<Map?>> processors, Map? map = null)
    {
        foreach (KeyValuePair<ExtractDataOptions, Action<Map?>> processor in processors)
        {
            if (_extractDataOptions.HasFlag(processor.Key))
            {
                processor.Value(map);
            }
            else
            {
                _logger.LogTrace("Element processor {Processor} was not run, was not selected in options", processor.Key);
            }
        }

        // write out the files (data and gamestrings)
        await WriteDataFiles();
        await WriteGameStringFile(map?.Id);
    }

    private void ProcessElementObject<TElementObject, TParser>(Map? map = null)
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
            SortedDictionary<string, TElementObject> itemsToSerialize = _dataExtractorService.Extract<TElementObject, TParser>((TParser)dataParser, map);

            // delay until after all data extraction is done (done for each map group)
            _dataWriterTasks.Add(() => WriteToJson(itemsToSerialize, map));

            SaveImages(itemsToSerialize);

            _logger.LogInformation("Action processor complete for {HeroesCollectionObject} using parser {Parser}", typeOfElementObjectName, typeOfParserName);
        }
    }

    private async Task WriteToJson<TElementObject>(SortedDictionary<string, TElementObject> itemsToSerialize, Map? map)
        where TElementObject : IElementObject
    {
        if (map is null)
            await _jsonDataFileWriterService.Write(itemsToSerialize);
        else
            await _jsonDataFileWriterService.WriteToMaps(map.Id, itemsToSerialize);
    }

    // parses and save the images for after data extraction/serialization
    private void SaveImages<TElementObject>(SortedDictionary<string, TElementObject> itemsToSerialize)
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

    private async Task WriteDataFiles()
    {
        _logger.LogInformation("Waiting for data file write tasks to complete...");

        foreach (Func<Task> task in _dataWriterTasks)
        {
            await task();
        }

        _dataWriterTasks.Clear();

        _logger.LogInformation("All data file write tasks complete.");
    }

    private async Task WriteGameStringFile(string? mapName)
    {
        if (_options.LocalizedText == LocalizedTextOption.None)
        {
            _logger.LogInformation("LocalizedText option is set to {LocalizedTextOption}. Skipping writing gamestring file(s).", _options.LocalizedText);
            return;
        }

        if (!string.IsNullOrWhiteSpace(mapName))
        {
            // write out the gamestring file for the extracted gamestrings from json serialization
            await _jsonGameStringFileWriterService.WriteForMap(mapName);
        }
        else
        {
            // don't create a gamestring file yet, if we have map parsing available the base gamestring file will be created after all maps are processed
            _jsonGameStringFileWriterService.SerializeOnly();
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
            if (extractDataOption.Value.IsEnabled is false || extractDataOption.Value.Images is false)
                continue;

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

            if (!Enum.TryParse(extractDataOption.Key, true, out ExtractImageOptions result))
                continue;

            selectImageExtractOptions |= result;
        }

        _logger.LogDebug("Selected image extractors: {@ImageOptions}", selectImageExtractOptions);

        return selectImageExtractOptions;
    }

    private Dictionary<ExtractDataOptions, Action<Map?>> GetElementProcessors() => new()
    {
        { ExtractDataOptions.Announcer, ProcessElementObject<Announcer, AnnouncerParser> },
        { ExtractDataOptions.Banner, ProcessElementObject<Banner, BannerParser> },
        { ExtractDataOptions.Bundle, ProcessElementObject<Bundle, BundleParser> },
        { ExtractDataOptions.Boost, ProcessElementObject<Boost, BoostParser> },
        { ExtractDataOptions.Hero, ProcessElementObject<Hero, HeroParser> },
        { ExtractDataOptions.LootChest, ProcessElementObject<LootChest, LootChestParser> },
        { ExtractDataOptions.Mount, ProcessElementObject<Mount, MountParser> },
        { ExtractDataOptions.Skin, ProcessElementObject<Skin, SkinParser> },
        { ExtractDataOptions.Unit, ProcessElementObject<Unit, UnitParser> },
        { ExtractDataOptions.Veterancy, ProcessElementObject<Veterancy, VeterancyParser> },
        { ExtractDataOptions.VoiceLine, ProcessElementObject<VoiceLine, VoiceLineParser> },
    };
}
