namespace HeroesDataParser.Cli.Commands.PortraitCommands;

public class PortraitBattleNetCacheCommand : Command<PortraitBattleNetCacheSettings>
{
    private readonly ILogger<PortraitBattleNetCacheCommand> _logger;
    private readonly PortraitBattleNetCacheOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IPortraitBattleNetCacheService _portraitBattleNetCacheService;

    public PortraitBattleNetCacheCommand(ILogger<PortraitBattleNetCacheCommand> logger, IOptions<PortraitBattleNetCacheOptions> options, IAnsiConsole console, IPortraitBattleNetCacheService portraitBattleNetCacheService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _portraitBattleNetCacheService = portraitBattleNetCacheService;
    }

    protected override int Execute(CommandContext context, PortraitBattleNetCacheSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(PortraitBattleNetCacheCommand));

        if (settings.CacheDirectoryPath is null)
            PortraitCommandHelpers.SetBattleNetCacheDirectory(_console, _options);
        else
            _options.BattleNetCacheDirectory = settings.CacheDirectoryPath.FullName;

        string outputDirectory;
        if (settings.OutputDirectory is null)
            outputDirectory = Path.Combine(Path.GetFullPath("."), "battle.net-cache");
        else
            outputDirectory = settings.OutputDirectory.FullName;

        _options.OutputDirectory = outputDirectory;

        _portraitBattleNetCacheService.CopyWaflFiles();

        return 0;
    }
}
