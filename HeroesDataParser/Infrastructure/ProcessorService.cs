using Heroes.LocaleText;
using Serilog.Context;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace HeroesDataParser.Infrastructure;

public class ProcessorService : IProcessorService
{
    private readonly ILogger<ProcessorService> _logger;
    private readonly RootOptions _options;
    private readonly IServiceProvider _serviceProvider;
    //private readonly IDataParserService _dataParserService;
    private readonly IDataExtractorService _dataExtractorService;
    private readonly IJsonFileWriterService _jsonFileWriterService;

    private readonly ExtractDataOptions _extractDataOptions;
    private readonly ExtractImageOptions _extractImageOptions;
    private readonly Dictionary<ExtractDataOptions, Func<Map?, Task>> _processElementByExtractDataOption;

    private StormLocale _stormLocale;

    public ProcessorService(ILogger<ProcessorService> logger, IOptions<RootOptions> options, IServiceProvider serviceProvider, IDataExtractorService dataExtractorService, IJsonFileWriterService jsonFileWriterService)
    {
        _logger = logger;
        _options = options.Value;
        _serviceProvider = serviceProvider;
        //_dataParserService = dataParserService;
        _dataExtractorService = dataExtractorService;
        _jsonFileWriterService = jsonFileWriterService;

        _extractDataOptions = GetExtractDataOptions();
        _extractImageOptions = GetExtractImageOptions();

        _processElementByExtractDataOption = GetElementProcessors();
    }

    public ExtractDataOptions ExtractDataOptions => _extractDataOptions;

    public ExtractImageOptions ExtractImageOptions => _extractImageOptions;

    public async Task Start(StormLocale stormLocale)
    {
        _stormLocale = stormLocale;

        _logger.LogTrace("Available element processors {@ActionProcessors}", _processElementByExtractDataOption.Keys);

        await RunElementProcessors(_processElementByExtractDataOption);
    }

    public async Task StartForMap(Map map)
    {
        _logger.LogTrace("Available element processors {@ActionProcessors} for Map {MapId}", _processElementByExtractDataOption.Keys, map.Id);

        await RunElementProcessors(_processElementByExtractDataOption, map);
    }

    public void Test<TElement>(Dictionary<string, TElement> items)
        where TElement : IElementObject
    {
        
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
        foreach (var processor in processors)
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


    //public void Test(Dictionary<string, TElement> elementById)
    //    where TElement : IElementObject
    //{
    //    var writerService = _serviceProvider.GetRequiredService<IJsonFileWriterService<TElement>>();

    //    //await writerService.Write(itemsToSerialize, _stormLocale);
    //}

    //private async Task ProcessDataOptions(ExtractDataOptions selectDataExtractOptions)
    //{
    //    _logger.LogTrace("Available action processors {@ActionProcessors}", _processActionByExtractDataOption.Keys);

    //    foreach (ExtractDataOptions option in _processActionByExtractDataOption.Keys)
    //    {
    //        if (selectDataExtractOptions.HasFlag(option))
    //        {
    //            if (_processActionByExtractDataOption.TryGetValue(option, out Func<Task>? processAction))
    //                await processAction();
    //            else
    //                _logger.LogWarning("No extract data option found for the action processor {ActionProcessor}, is it missing and needs to be added?", option);
    //        }
    //    }
    //}

    //private async Task ProcessMapOption(ExtractDataOptions selectDataExtractOptions)
    //{
    //    if (!selectDataExtractOptions.HasFlag(ExtractDataOptions.Map))
    //    {
    //        _logger.LogTrace("Map option was not selected, no processing map data");
    //        return;
    //    }


    //}

    //private Dictionary<string, IElementObject> ProcessHeroesCollectionObject<THeroesCollectionObject, TParser>()
    //    where THeroesCollectionObject : IHeroesCollectionObject, IElementObject
    //    where TParser : IDataParser<THeroesCollectionObject>
    //{
    //    _logger.LogTrace("Start action processor for {HeroesCollectionObject} using parser {Parser}", typeof(THeroesCollectionObject).Name, typeof(TParser).Name);

    //    var dataParser = _serviceProvider.GetRequiredService<IDataParser<THeroesCollectionObject>>();
    //    var dataExtractorService = _serviceProvider.GetRequiredService<IDataExtractorService<THeroesCollectionObject, TParser>>();

    //    var extractedItems = dataExtractorService.Extract((TParser)dataParser);

    //    _logger.LogTrace("Action processor complete for {HeroesCollectionObject} using parser {Parser}", typeof(THeroesCollectionObject).Name, typeof(TParser).Name);

    //    return extractedItems;
    //}

    //private async Task ProcessHeroesCollectionObject<THeroesCollectionObject, TParser>(Map? map = null)
    //    where THeroesCollectionObject : IHeroesCollectionObject, IElementObject
    //    where TParser : IDataParser<THeroesCollectionObject>
    //{
    //    using (LogContext.PushProperty("ElementType", typeof(THeroesCollectionObject).Name))
    //    using (LogContext.PushProperty("Parser", typeof(TParser).Name))
    //    {
    //        _logger.LogInformation("Start action processor for {HeroesCollectionObject} using parser {Parser}", typeof(THeroesCollectionObject).Name, typeof(TParser).Name);

    //        var dataParser = _serviceProvider.GetRequiredService<IDataParser<THeroesCollectionObject>>();

    //        var itemsToSerialize = _dataExtractorService.Extract<THeroesCollectionObject, TParser>((TParser)dataParser, map);

    //        if (map is null)
    //            await _jsonFileWriterService.Write(itemsToSerialize, _stormLocale);
    //        else
    //            await _jsonFileWriterService.WriteToMaps(map.Id, itemsToSerialize, _stormLocale);

    //        IImageWriter<THeroesCollectionObject>? imageWriter = _serviceProvider.GetService<IImageWriter<THeroesCollectionObject>>();

    //        if (imageWriter is not null && _extractImageOptions.HasFlag(imageWriter.ExtractImageOption))
    //        {
    //            await imageWriter.WriteImages(itemsToSerialize);
    //        }
    //    }

    //    _logger.LogInformation("Action processor complete for {HeroesCollectionObject} using parser {Parser}", typeof(THeroesCollectionObject).Name, typeof(TParser).Name);
    //}

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

            await WriteToJson(map, itemsToSerialize);
            await WriteImages(itemsToSerialize);

            _logger.LogInformation("Action processor complete for {HeroesCollectionObject} using parser {Parser}", typeOfElementObjectName, typeOfParserName);
        }
    }

    private async Task WriteToJson<TElementObject>(Map? map, Dictionary<string, TElementObject> itemsToSerialize)
        where TElementObject : IElementObject
    {
        if (map is null)
            await _jsonFileWriterService.Write(itemsToSerialize, _stormLocale);
        else
            await _jsonFileWriterService.WriteToMaps(map.Id, itemsToSerialize, _stormLocale);
    }

    private async Task WriteImages<TElementObject>(Dictionary<string, TElementObject> itemsToSerialize)
    where TElementObject : IElementObject
    {
        IImageWriter<TElementObject>? imageWriter = _serviceProvider.GetService<IImageWriter<TElementObject>>();

        if (imageWriter is null)
        {
            _logger.LogInformation("No image writer found for {ElementType}", typeof(TElementObject).Name);
            return;
        }

        if (_extractImageOptions.HasFlag(imageWriter.ExtractImageOption))
        {
            await imageWriter.WriteImages(itemsToSerialize);
        }
    }

    //private async Task ProcessMapObject()
    //{
    //    _logger.LogInformation("Start action processor for {MapObject} using parser {Parser}", typeof(Map).Name, typeof(MapParser).Name);

    //    //var dataParser = _serviceProvider.GetRequiredService<IDataParser<Map>>();

    //    //await _mapDataParserService.ParseAndWriteData(dataParser, _stormLocale);

    //    _logger.LogInformation("Action processor complete for {MapObject} using parser {Parser}", typeof(Map).Name, typeof(MapParser).Name);
    //}






    //private async Task ProcessHeroesCollectionObject<THeroesCollectionObject, TParser>()
    //    where THeroesCollectionObject : IHeroesCollectionObject, IElementObject
    //    where TParser : IDataParser<THeroesCollectionObject>
    //{
    //    _logger.LogInformation("Start action processor for {HeroesCollectionObject} using parser {Parser}", typeof(THeroesCollectionObject).Name, typeof(TParser).Name);

    //    var dataParser = _serviceProvider.GetRequiredService<IDataParser<THeroesCollectionObject>>();
    //    var dataExtractorService = _serviceProvider.GetRequiredService<IDataExtractorService<THeroesCollectionObject, TParser>>();

    //    var itemsToSerialize = dataExtractorService.Extract((TParser)dataParser);

    //    var writerService = _serviceProvider.GetRequiredService<IJsonFileWriterService<THeroesCollectionObject>>();

    //    await writerService.Write(itemsToSerialize, _stormLocale);

    //    _logger.LogInformation("Action processor complete for {HeroesCollectionObject} using parser {Parser}", typeof(THeroesCollectionObject).Name, typeof(TParser).Name);
    //}

    //private async Task ProcessMapObject()
    //{
    //    _logger.LogInformation("Start action processor for {MapObject} using parser {Parser}", typeof(Map).Name, typeof(MapParser).Name);

    //    var dataParser = _serviceProvider.GetRequiredService<IDataParser<Map>>();
    //    var dataExtractorService = _serviceProvider.GetRequiredService<IDataExtractorService<Map, IDataParser<Map>>>();

    //    var itemsToSerialize = dataExtractorService.Extract(dataParser);

    //    var writerService = _serviceProvider.GetRequiredService<IJsonFileWriterService<Map>>();

    //    await writerService.Write(itemsToSerialize, _stormLocale);

    //    _logger.LogInformation("Action processor complete for {MapObject} using parser {Parser}", typeof(Map).Name, typeof(MapParser).Name);
    //}

    private ExtractDataOptions GetExtractDataOptions()
    {
        ExtractDataOptions selectDataExtractOptions = ExtractDataOptions.None;

        foreach (var extractDataOption in _options.Extractors)
        {
            if (!Enum.TryParse(extractDataOption.Key, true, out ExtractDataOptions result) || extractDataOption.Value.IsEnabled is false)
                continue;

            selectDataExtractOptions |= result;
        }

        _logger.LogTrace("Selected data extractors: {@DataOptions}", selectDataExtractOptions);

        return selectDataExtractOptions;
    }

    private ExtractImageOptions GetExtractImageOptions()
    {
        ExtractImageOptions selectImageExtractOptions = ExtractImageOptions.None;

        foreach (var extractDataOption in _options.Extractors)
        {
            if (!Enum.TryParse(extractDataOption.Key, true, out ExtractImageOptions result) || extractDataOption.Value.IsEnabled is false || extractDataOption.Value.Images is false)
                continue;

            selectImageExtractOptions |= result;
        }

        _logger.LogTrace("Selected image extractors: {@ImageOptions}", selectImageExtractOptions);

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
    };

   // private Dictionary<ExtractImageOptions, >
}
