namespace HeroesDataParser.Cli.Commands.JsonSchemaCommands;

public class JsonSchemaExportGameStringCommand : AsyncCommand<JsonSchemaExportGameStringSettings>
{
    private readonly ILogger<JsonSchemaExportGameStringCommand> _logger;
    private readonly JsonSchemaExportOptions _options;
    private readonly IJsonSchemaExporterService _jsonSchemaExporterService;

    public JsonSchemaExportGameStringCommand(
        ILogger<JsonSchemaExportGameStringCommand> logger,
        IOptions<JsonSchemaExportOptions> options,
        IJsonSchemaExporterService jsonSchemaExporterService)
    {
        _logger = logger;
        _options = options.Value;
        _jsonSchemaExporterService = jsonSchemaExporterService;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, JsonSchemaExportGameStringSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(JsonSchemaExportGameStringCommand));

        string outputDirectory;
        if (settings.OutputDirectory is null)
            outputDirectory = ".";
        else
            outputDirectory = settings.OutputDirectory.FullName;

        _options.AllowOverwrite = settings.Overwrite;
        _options.JsonIndent = !settings.DisableJsonIndent;
        _options.Version = AppVersion.GetAppVersion();

        await _jsonSchemaExporterService.ExportGameStringSchema();

        return 0;
    }
}
