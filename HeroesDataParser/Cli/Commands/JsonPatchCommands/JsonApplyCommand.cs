namespace HeroesDataParser.Cli.Commands.JsonPatchCommands;

public class JsonApplyCommand : AsyncCommand<JsonApplySettings>
{
    private readonly ILogger<JsonApplyCommand> _logger;
    private readonly JsonApplyOptions _options;
    private readonly IJsonApplyService _jsonApplyService;

    public JsonApplyCommand(ILogger<JsonApplyCommand> logger, IOptions<JsonApplyOptions> options, IJsonApplyService jsonApplyService)
    {
        _logger = logger;
        _options = options.Value;
        _jsonApplyService = jsonApplyService;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, JsonApplySettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(JsonApplyCommand));

        _options.JsonFilePath = settings.FilePath.FullName;
        _options.JsonPatchFilePath = settings.PatchFilePath.FullName;

        if (settings.OutputDirectory is null)
            _options.OutputDirectory = settings.PatchFilePath.DirectoryName ?? ".";
        else
            _options.OutputDirectory = settings.OutputDirectory.FullName;

        await _jsonApplyService.ApplyJsonPatch();

        return 0;
    }
}
