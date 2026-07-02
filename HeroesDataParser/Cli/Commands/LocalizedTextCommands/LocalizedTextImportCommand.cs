namespace HeroesDataParser.Cli.Commands.LocalizedTextCommands;

public class LocalizedTextImportCommand : AsyncCommand<LocalizedTextImportSettings>
{
    private readonly ILogger<LocalizedTextImportCommand> _logger;
    private readonly LocalizedTextImportOptions _options;
    private readonly IAnsiConsole _console;
    private readonly ILocalizedTextImportService _localizedTextImportService;

    public LocalizedTextImportCommand(
        ILogger<LocalizedTextImportCommand> logger,
        IOptions<LocalizedTextImportOptions> options,
        IAnsiConsole console,
        ILocalizedTextImportService localizedTextImportService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _localizedTextImportService = localizedTextImportService;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, LocalizedTextImportSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(LocalizedTextImportCommand));

        _options.DataFilePath = settings.DataFilePath.FullName;
        _options.GameStringsFilePath = settings.GameStringsFilePath.FullName;
        _options.AllowOverwrite = settings.Overwrite;
        _options.JsonIndent = !settings.DisableJsonIndent;

        if (settings.OutputDirectory is null)
            _options.OutputDirectory = settings.DataFilePath.DirectoryName ?? ".";
        else
            _options.OutputDirectory = settings.OutputDirectory.FullName;

        if (File.Exists(_options.OutputFilePath))
        {
            if (!_options.AllowOverwrite)
            {
                _logger.LogError("Output file already exists and overwrite is not allowed: {OutputFilePath}", _options.OutputFilePath);
                _console.MarkupLine($"[red]Output file already exists: {_options.OutputFilePath}[/]");
                return 1;
            }
        }
        else
        {
            _options.IsNewFile = true;
        }

        await _localizedTextImportService.ImportGameStrings();

        return 0;
    }
}
