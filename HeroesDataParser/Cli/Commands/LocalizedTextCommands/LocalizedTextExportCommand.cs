namespace HeroesDataParser.Cli.Commands.LocalizedTextCommands;

public class LocalizedTextExportCommand : AsyncCommand<LocalizedTextExportSettings>
{
    private readonly ILogger<LocalizedTextExportCommand> _logger;
    private readonly LocalizedTextExportOptions _options;
    private readonly IAnsiConsole _console;
    private readonly ILocalizedTextExportService _localizedTextExportService;

    public LocalizedTextExportCommand(
        ILogger<LocalizedTextExportCommand> logger,
        IOptions<LocalizedTextExportOptions> options,
        IAnsiConsole console,
        ILocalizedTextExportService localizedTextExportService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _localizedTextExportService = localizedTextExportService;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, LocalizedTextExportSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(LocalizedTextExportCommand));

        _options.DataFilePath = settings.DataFilePath.FullName;
        _options.GameStringFilePath = settings.GameStringFilePath?.FullName;
        _options.ExtractType = settings.ExtractType;
        _options.AllowOverwrite = settings.Overwrite;
        _options.JsonIndent = !settings.DisableJsonIndent;

        if (settings.OutputDirectory is null)
            _options.OutputDirectory = settings.DataFilePath.DirectoryName ?? ".";
        else
            _options.OutputDirectory = settings.OutputDirectory.FullName;

        await _localizedTextExportService.ExportGameStrings();

        return 0;
    }
}
