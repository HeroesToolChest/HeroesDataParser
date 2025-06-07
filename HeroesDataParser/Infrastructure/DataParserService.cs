namespace HeroesDataParser.Infrastructure;

//public class DataParserService : IDataParserService
//{
//    private readonly ILogger<ProcessorService> _logger;
//    private readonly IDataExtractorService _dataExtractorService;
//    private readonly IJsonFileWriterService _jsonFileWriterService;


//    public DataParserService(ILogger<ProcessorService> logger, IDataExtractorService dataExtractorService, IJsonFileWriterService jsonFileWriterService)
//    {
//        _logger = logger;
//        _dataExtractorService = dataExtractorService;
//        _jsonFileWriterService = jsonFileWriterService;
//    }

//    public async Task ParseAndWriteData<TElement, TParser>(TParser parser, StormLocale stormLocale, Map? map = null)
//        where TElement : IElementObject
//        where TParser : IDataParser<TElement>
//    {
//        var itemsToSerialize = _dataExtractorService.Extract<TElement, TParser>(parser, map);

//        //_logger.LogInformation("{Count} items to serialize", itemsToSerialize.Keys.Count);

//        if (map is null)
//            await _jsonFileWriterService.Write(itemsToSerialize, stormLocale);
//        else
//            await _jsonFileWriterService.WriteToMaps(map.Id, itemsToSerialize, stormLocale);
//    }
//}
