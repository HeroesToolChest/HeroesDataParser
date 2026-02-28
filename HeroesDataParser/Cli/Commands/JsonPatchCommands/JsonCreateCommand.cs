namespace HeroesDataParser.Cli.Commands.JsonPatchCommands;

public class JsonCreateCommand : AsyncCommand<JsonCreateSettings>
{
    private readonly ILogger<JsonCreateCommand> _logger;
    private readonly JsonCreateOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IJsonCreateService _jsonCreateService;

    public JsonCreateCommand(ILogger<JsonCreateCommand> logger, IOptions<JsonCreateOptions> options, IAnsiConsole console, IJsonCreateService jsonCreateService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _jsonCreateService = jsonCreateService;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, JsonCreateSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(JsonApplyCommand));

        _options.OldJsonFilePath = settings.OldJsonFilePath.FullName;
        _options.NewJsonFilePath = settings.NewJsonFilePath.FullName;

        string outputDirectory;
        if (settings.OutputDirectory is null)
            outputDirectory = settings.NewJsonFilePath.DirectoryName ?? ".";
        else
            outputDirectory = settings.OutputDirectory.FullName;

        _options.OutputFilePath = GetOutputFilePath(outputDirectory);
        _options.AllowOverwrite = settings.Overwrite;

        if (!_options.AllowOverwrite)
        {
            if (File.Exists(_options.OutputFilePath))
            {
                _logger.LogError("Output file already exists and overwrite is not allowed: {OutputFilePath}", _options.OutputFilePath);
                _console.MarkupLine($"[red]Output file already exists: {_options.OutputFilePath}[/]");
                return 1;
            }
        }

        await _jsonCreateService.CreateJsonPatch();

        return 0;
    }

    private string GetOutputFilePath(string outputDirectory)
    {
        // get file name based on new file
        ReadOnlySpan<char> fileName = Path.GetFileNameWithoutExtension(_options.NewJsonFilePath.AsSpan());
        if (fileName.EndsWith(".patch", StringComparison.OrdinalIgnoreCase))
        {
            fileName = fileName[..^".patch".Length];
        }

        string outputFileName = $"{fileName}.patch.json";

        return Path.Combine(outputDirectory, outputFileName);
    }
}