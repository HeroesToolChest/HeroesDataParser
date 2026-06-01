namespace HeroesDataParser.Cli.Commands;

public class CASCExtractCommand : AsyncCommand<CASCExtractSettings>
{
    private const string _hdpFilter = ":hdp:";
    private static readonly string[] _specialFilters = ["*", _hdpFilter];

    private readonly ILogger<CASCExtractCommand> _logger;
    private readonly CASCExtractOptions _options;
    private readonly ICASCExtractorService _cascExtractorService;

    public CASCExtractCommand(ILogger<CASCExtractCommand> logger, IOptions<CASCExtractOptions> options, ICASCExtractorService cascExtractorService)
    {
        _logger = logger;
        _options = options.Value;
        _cascExtractorService = cascExtractorService;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, CASCExtractSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(CASCExtractCommand));

        SetOptions(settings);

        await _cascExtractorService.RootDirectoryExtract();

        return 0;
    }

    private void SetOptions(CASCExtractSettings settings)
    {
        _options.StorageLoad.Type = settings.StorageType;
        _options.StorageLoad.Path = settings.StorageDirectory?.FullName;
        _options.StorageLoad.Ptr = settings.IsPtr;

        if (settings.Filters.Contains("*"))
        {
            _options.FileFilters = ["*"];
        }
        else if (settings.Filters.Any(x => x.Contains(_hdpFilter)))
        {
            _options.FileFilters = [
                "**/gamestrings.txt",
                "**/buildid.txt",
                "**/assets.txt",
                "**/*.xml",
                "**/*.s2mv",
                "**/*.s2ma",
                "**/*.stormstyle",
                "**/*.stormlayout",
                "**/documentinfo"
            ];
        }

        _options.FileFilters.UnionWith(settings.Filters.Except(_specialFilters).Select(x => x.Replace('\\', '/')));
        _options.Threads = settings.Threads;

        if (settings.OutputDirectory is not null)
            _options.OutputDirectory = settings.OutputDirectory.FullName;
    }
}
