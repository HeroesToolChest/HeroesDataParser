namespace HeroesDataParser.Cli.Commands.GameStringTextCommands;

public class GameStringTextFormatCommand : AsyncCommand<GameStringTextFormatSettings>
{
    private readonly ILogger<GameStringTextFormatCommand> _logger;
    private readonly GameStringTextFormatOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IGameStringTextUpdateService _gameStringTextUpdateService;

    public GameStringTextFormatCommand(
        ILogger<GameStringTextFormatCommand> logger,
        IOptions<GameStringTextFormatOptions> options,
        IAnsiConsole console,
        IGameStringTextUpdateService gameStringTextUpdateService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _gameStringTextUpdateService = gameStringTextUpdateService;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, GameStringTextFormatSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(GameStringTextFormatCommand));

        _options.FilePath = settings.FilePath.FullName;
        _options.GameStringTextType = settings.GameStringTextType;
        _options.GameStringTextHltConstantRemoveMode = settings.HltConstantRemoveMode;
        _options.GameStringTextHltStyleRemoveMode = settings.HltStyleRemoveMode;

        if (settings.OutputDirectory is null)
            _options.OutputDirectory = settings.FilePath.DirectoryName ?? ".";
        else
            _options.OutputDirectory = settings.OutputDirectory.FullName;

        _options.AllowOverwrite = settings.Overwrite;
        _options.JsonIndent = !settings.DisableJsonIndent;

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

        await _gameStringTextUpdateService.FormatGameStringText();

        return 0;
    }
}
