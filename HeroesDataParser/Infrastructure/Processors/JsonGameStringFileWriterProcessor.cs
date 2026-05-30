namespace HeroesDataParser.Infrastructure.Processors;

public class JsonGameStringFileWriterProcessor : IJsonGameStringFileWriterProcessor
{
    private readonly ILogger<JsonGameStringFileWriterProcessor> _logger;
    private readonly RootOptions _options;
    private readonly IJsonGameStringFileWriterService _jsonGameStringFileWriterService;

    public JsonGameStringFileWriterProcessor(ILogger<JsonGameStringFileWriterProcessor> logger, IOptions<RootOptions> options, IJsonGameStringFileWriterService jsonGameStringFileWriterService)
    {
        _logger = logger;
        _options = options.Value;
        _jsonGameStringFileWriterService = jsonGameStringFileWriterService;
    }

    public async Task WriteGameStringFile(Map? map)
    {
        if (_options.LocalizedText == LocalizedTextOption.None)
        {
            _logger.LogInformation("LocalizedText option is set to {LocalizedTextOption}. Skipping writing data gamestring file(s).", _options.LocalizedText);

            return;
        }

        if (map is not null)
        {
            // write out the gamestring file for the extracted gamestrings from json serialization
            await _jsonGameStringFileWriterService.WriteMapSpecific(map);
        }
        else
        {
            await _jsonGameStringFileWriterService.WriteBase();
        }
    }

    public async Task WriteMapGameStringFile()
    {
        if (_options.LocalizedText == LocalizedTextOption.None)
        {
            _logger.LogInformation("LocalizedText option is set to {LocalizedTextOption}. Skipping writing map gamestring file.", _options.LocalizedText);

            return;
        }

        await _jsonGameStringFileWriterService.WriteMap();
    }
}
