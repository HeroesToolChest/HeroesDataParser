namespace HeroesDataParser.Cli.Commands.LocalizedTextCommands;

public class LocalizedTextExportCommand : AsyncCommand<LocalizedTextExportSettings>
{
    private readonly ILogger<LocalizedTextExportCommand> _logger;
    private readonly LocalizedTextExportOptions _options;
    private readonly IAnsiConsole _console;

    public LocalizedTextExportCommand(
        ILogger<LocalizedTextExportCommand> logger,
        IOptions<LocalizedTextExportOptions> options,
        IAnsiConsole console)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, LocalizedTextExportSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(LocalizedTextExportCommand));

        _options.DataFilePath = settings.DataFilePath.FullName;
        _options.AllowOverwrite = settings.Overwrite;
        _options.JsonIndent = !settings.DisableJsonIndent;

        if (settings.OutputDirectory is null)
            _options.OutputDirectory = settings.DataFilePath.DirectoryName ?? ".";
        else
            _options.OutputDirectory = settings.OutputDirectory.FullName;

        await Task.CompletedTask;
        return 0;
    }
}
