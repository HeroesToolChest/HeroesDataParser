namespace HeroesDataParser.Cli.Commands.JsonSchemaCommands;

public class JsonSchemaExportDataCommand : AsyncCommand<JsonSchemaExportDataSettings>
{
    private readonly ILogger<JsonSchemaExportDataCommand> _logger;
    private readonly JsonSchemaExportDataOptions _options;
    private readonly IJsonSchemaExporterService _jsonSchemaExporterService;

    public JsonSchemaExportDataCommand(
        ILogger<JsonSchemaExportDataCommand> logger,
        IOptions<JsonSchemaExportDataOptions> options,
        IJsonSchemaExporterService jsonSchemaExporterService)
    {
        _logger = logger;
        _options = options.Value;
        _jsonSchemaExporterService = jsonSchemaExporterService;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, JsonSchemaExportDataSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(JsonSchemaExportDataCommand));

        if (settings.OutputDirectory is null)
            _options.OutputDirectory = ".";
        else
            _options.OutputDirectory = settings.OutputDirectory.FullName;

        _options.AllowOverwrite = settings.Overwrite;
        _options.JsonIndent = !settings.DisableJsonIndent;
        _options.Version = AppVersion.GetAppVersion();

        foreach (string extractor in settings.Extractors)
        {
            ParseExtractor(extractor);
        }

        await _jsonSchemaExporterService.ExportDataSchema();

        return 0;
    }

    private void ParseExtractor(string extractor)
    {
        if (!Enum.TryParse(extractor, true, out ExtractDataOptions extractorName))
            return;

        _options.ExtractDataOptions |= extractorName;
    }
}
