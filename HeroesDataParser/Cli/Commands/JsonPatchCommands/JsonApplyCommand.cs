namespace HeroesDataParser.Cli.Commands.JsonPatchCommands;

public class JsonApplyCommand : AsyncCommand<JsonApplySettings>
{
    private readonly ILogger<JsonApplyCommand> _logger;
    private readonly JsonApplyOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IJsonApplyService _jsonApplyService;

    public JsonApplyCommand(ILogger<JsonApplyCommand> logger, IOptions<JsonApplyOptions> options, IAnsiConsole console, IJsonApplyService jsonApplyService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
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

        _options.OutputFilePath = GetOutputFilePath();
        _options.AllowOverwrite = settings.Overwrite;

        if (!_options.AllowOverwrite)
        {
            if (File.Exists(_options.OutputFilePath))
            {
                _logger.LogError("Output file already exists and overwrite is not allowed: {OutputFilePath}", _options.OutputFilePath);
                _console.MarkupLine($"[red]Output file already exists and overwrite is not allowed: {_options.OutputFilePath}[/]");
                return 1;
            }
        }

        await _jsonApplyService.ApplyJsonPatch();

        return 0;
    }

    private string GetOutputFilePath()
    {
        // get file name based on patch file
        string outputFileName = Path.GetFileNameWithoutExtension(_options.JsonPatchFilePath.Replace(".patch", string.Empty)) + ".json";
        string outputFilePath = Path.Combine(_options.OutputDirectory, outputFileName);

        Directory.CreateDirectory(_options.OutputDirectory);

        return outputFilePath;
    }
}
