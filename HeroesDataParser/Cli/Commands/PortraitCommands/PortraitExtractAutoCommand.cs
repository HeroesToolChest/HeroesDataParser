namespace HeroesDataParser.Cli.Commands.PortraitCommands;

public class PortraitExtractAutoCommand : Command<PortraitExtractAutoSettings>
{
    private readonly ILogger<PortraitExtractAutoCommand> _logger;
    private readonly PortraitExtractAutoOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IFileProvider _fileProvider;
    private readonly IPortraitExtractAutoService _portraitExtractAutoService;

    private readonly string _portraitExtractFile = Path.Combine(Constants.ConfigFilesDirectory, "portrait-extract.xml");

    public PortraitExtractAutoCommand(
        ILogger<PortraitExtractAutoCommand> logger,
        IOptions<PortraitExtractAutoOptions> options,
        IAnsiConsole console,
        IFileProvider fileProvider,
        IPortraitExtractAutoService portraitExtractAutoService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _fileProvider = fileProvider;
        _portraitExtractAutoService = portraitExtractAutoService;
    }

    protected override int Execute(CommandContext context, PortraitExtractAutoSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(PortraitExtractAutoCommand));

        if (settings.CacheDirectoryPath is null)
        {
            if (!PortraitCommandHelpers.SetBattleNetCacheDirectory(_console, _options))
                return 1;
        }
        else
        {
            _options.BattleNetCacheDirectory = settings.CacheDirectoryPath.FullName;
        }

        if (settings.XmlConfigFilePath is not null)
        {
            _options.XmlConfigFilePath = settings.XmlConfigFilePath.FullName;
        }
        else
        {
            string? xmlConfigFilePath = _fileProvider.GetFileInfo(_portraitExtractFile).PhysicalPath;
            if (!string.IsNullOrWhiteSpace(xmlConfigFilePath))
                _options.XmlConfigFilePath = xmlConfigFilePath;
            else
                _options.XmlConfigFilePath = _portraitExtractFile;
        }

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
