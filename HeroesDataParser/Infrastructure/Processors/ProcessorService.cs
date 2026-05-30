namespace HeroesDataParser.Infrastructure.Processors;

public class ProcessorService : IProcessorService
{
    private readonly RootOptions _options;
    private readonly IXmlDataParserProcessor _xmlDataParserProcessor;
    private readonly IJsonGameStringFileWriterProcessor _jsonGameStringFileWriterProcessor;

    public ProcessorService(
        IOptions<RootOptions> options,
        IXmlDataParserProcessor xmlDataParserProcessor,
        IJsonGameStringFileWriterProcessor jsonGameStringFileWriterProcessor)
    {
        _options = options.Value;
        _xmlDataParserProcessor = xmlDataParserProcessor;
        _jsonGameStringFileWriterProcessor = jsonGameStringFileWriterProcessor;
    }

    public async Task Start()
    {
        await RunDataParsers();
    }

    public async Task StartForMapSpecific(Map map)
    {
        await RunDataParsers(map);
    }

    private async Task RunDataParsers(Map? map = null)
    {
        IEnumerable<ExtractDataOptions> associatedExtractDataParsers = _xmlDataParserProcessor.GetAssociatedExtractDataParsers();

        foreach (ExtractDataOptions option in associatedExtractDataParsers)
        {
            if (_options.ExtractDataOptions.HasFlag(option))
                _xmlDataParserProcessor.ExecuteDataParser(option, map);
        }

        // write out the files (data and then gamestrings)
        await _xmlDataParserProcessor.ExecuteJsonDataFileWriteTasks();
        await _jsonGameStringFileWriterProcessor.WriteGameStringFile(map);
    }
}
