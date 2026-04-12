namespace HeroesDataParser.Infrastructure.Processors;

public class XmlMapDataParserProcessor : IXmlMapDataParserProcessor
{
    private readonly IProcessorService _processorService;
    private readonly IMapDataExtractorService _mapDataExtractorService;
    private readonly IJsonDataFileWriterProcessor _jsonDataFileWriterProcessor;
    private readonly IImageParserProcessor _imageParserProcessor;

    public XmlMapDataParserProcessor(
        IProcessorService processorService,
        IMapDataExtractorService mapDataExtractorService,
        IJsonDataFileWriterProcessor jsonDataFileWriterProcessor,
        IImageParserProcessor imageParserProcessor)
    {
        _processorService = processorService;
        _mapDataExtractorService = mapDataExtractorService;
        _jsonDataFileWriterProcessor = jsonDataFileWriterProcessor;
        _imageParserProcessor = imageParserProcessor;
    }

    public async Task ExecuteMapParser()
    {
        // Extract will loop through the maps and for each one will parse the selected data types; the processor service loops through the data types
        // returned is the collection of map data
        SortedDictionary<string, Map> mapItemsToSerialize = await _mapDataExtractorService.Extract(_processorService.StartForMapSpecific);

        // only for the maps
        _jsonDataFileWriterProcessor.SaveJsonDataFileWrite(mapItemsToSerialize, null);
        _imageParserProcessor.SaveImages(mapItemsToSerialize);
    }

    public Task ExecuteJsonDataFileWriteTask()
    {
        // execute the map data file write task
        return _jsonDataFileWriterProcessor.ExecuteJsonDataFileWriteTasks();
    }
}
