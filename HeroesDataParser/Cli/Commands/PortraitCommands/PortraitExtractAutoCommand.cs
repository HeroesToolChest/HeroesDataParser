namespace HeroesDataParser.Cli.Commands.PortraitCommands;

public class PortraitExtractAutoCommand : Command<PortraitExtractAutoSettings>
{
    private readonly ILogger<PortraitExtractAutoCommand> _logger;
    private readonly PortraitExtractAutoOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IPortraitExtractAutoService _portraitExtractAutoService;

    public PortraitExtractAutoCommand(
        ILogger<PortraitExtractAutoCommand> logger,
        IOptions<PortraitExtractAutoOptions> options,
        IAnsiConsole console,
        IPortraitExtractAutoService portraitExtractAutoService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _portraitExtractAutoService = portraitExtractAutoService;
    }

    protected override int Execute(CommandContext context, PortraitExtractAutoSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(PortraitExtractAutoCommand));

        if (settings.CacheDirectoryPath is null)
            PortraitCommandHelpers.SetBattleNetCacheDirectory(_console, _options);
        else
            _options.BattleNetCacheDirectory = settings.CacheDirectoryPath.FullName;

        if (settings.XmlConfigFilePath is not null)
            _options.XmlConfigFilePath = settings.XmlConfigFilePath.FullName;
        else
            _options.XmlConfigFilePath = Path.Combine(Constants.ConfigFilesDirectory, "portrait-extract.xml");

        string outputDirectory;
        if (settings.OutputDirectory is null)
            outputDirectory = Path.Combine(Path.GetFullPath("."), Constants.ImagesDirectory, Constants.PortraitRewardsDirectory);
        else
            outputDirectory = settings.OutputDirectory.FullName;

        _options.OutputDirectory = outputDirectory;
        _options.RewardPortraitDataFilePath = settings.FilePath.FullName;
        _options.DeleteTextureSheet = settings.DeleteTextureSheet;

        _portraitExtractAutoService.Extract();

        return 0;
    }
}
