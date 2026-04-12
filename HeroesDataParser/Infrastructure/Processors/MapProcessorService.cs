namespace HeroesDataParser.Infrastructure.Processors;

public class MapProcessorService : IMapProcessorService
{
    private readonly IXmlMapDataParserProcessor _xmlMapDataParserProcessor;
    private readonly IJsonGameStringFileWriterProcessor _jsonGameStringFileWriterProcessor;

    public MapProcessorService(
        IXmlMapDataParserProcessor xmlMapDataParserProcessor,
        IJsonGameStringFileWriterProcessor jsonGameStringFileWriterProcessor)
    {
        _xmlMapDataParserProcessor = xmlMapDataParserProcessor;
        _jsonGameStringFileWriterProcessor = jsonGameStringFileWriterProcessor;
    }

    public async Task Start()
    {
        await _xmlMapDataParserProcessor.ExecuteMapParser();

        // write out the files (data and then gamestrings)
        await _xmlMapDataParserProcessor.ExecuteJsonDataFileWriteTask();
        await _jsonGameStringFileWriterProcessor.WriteMapGameStringFile();
    }
}
