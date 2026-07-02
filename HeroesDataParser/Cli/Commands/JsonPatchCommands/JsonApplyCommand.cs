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

        string outputDirectory;
        if (settings.OutputDirectory is null)
            outputDirectory = settings.PatchFilePath.DirectoryName ?? ".";
        else
            outputDirectory = settings.OutputDirectory.FullName;

        _options.OutputFilePath = GetOutputFilePath(outputDirectory);
        _options.AllowOverwrite = settings.Overwrite;
        _options.DeletePatchFile = settings.DeletePatchFile;
        _options.JsonIndent = !settings.DisableJsonIndent;

        if (!_options.AllowOverwrite)
        {
            if (File.Exists(_options.OutputFilePath))
            {
                _logger.LogError("Output file already exists and overwrite is not allowed: {OutputFilePath}", _options.OutputFilePath);
                _console.MarkupLine($"[red]Output file already exists: {_options.OutputFilePath}[/]");
                return 1;
            }
        }

        await _jsonApplyService.ApplyJsonPatch();

        return 0;
    }

    private string GetOutputFilePath(string outputDirectory)
    {
        // get file name based on patch file
        ReadOnlySpan<char> fileName = Path.GetFileNameWithoutExtension(_options.JsonPatchFilePath.AsSpan());
        if (fileName.EndsWith(".patch", StringComparison.OrdinalIgnoreCase))
        {
            fileName = fileName[..^".patch".Length];
        }

        string outputFileName = $"{fileName}.json";

        return Path.Combine(outputDirectory, outputFileName);
    }
}
