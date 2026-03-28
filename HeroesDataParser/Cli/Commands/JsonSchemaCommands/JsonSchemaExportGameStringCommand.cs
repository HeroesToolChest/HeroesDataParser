namespace HeroesDataParser.Cli.Commands.JsonSchemaCommands;

public class JsonSchemaExportGameStringCommand : AsyncCommand<JsonSchemaExportGameStringSettings>
{
    private readonly ILogger<JsonSchemaExportGameStringCommand> _logger;
    private readonly JsonSchemaExportGameStringOptions _options;

    public JsonSchemaExportGameStringCommand(ILogger<JsonSchemaExportGameStringCommand> logger, IOptions<JsonSchemaExportGameStringOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, JsonSchemaExportGameStringSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(JsonSchemaExportGameStringCommand));

        string outputDirectory;
        if (settings.OutputDirectory is null)
            outputDirectory = ".";
        else
            outputDirectory = settings.OutputDirectory.FullName;

        await Task.CompletedTask;
        return 0;
    }
}
