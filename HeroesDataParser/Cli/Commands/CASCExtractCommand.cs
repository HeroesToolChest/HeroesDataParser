namespace HeroesDataParser.Cli.Commands;

public class CASCExtractCommand : AsyncCommand<CASCExtractSettings>
{
    private const string _hdpFilter = ":hdp:";
    private static readonly string[] _specialFilters = [CASCExtractSettings.AllIncludePattern, _hdpFilter];

    private readonly ILogger<CASCExtractCommand> _logger;
    private readonly CASCExtractOptions _options;
    private readonly HttpClientOptions _httpClientOptions;
    private readonly ICASCExtractService _cascExtractService;

    public CASCExtractCommand(
        ILogger<CASCExtractCommand> logger,
        IOptions<CASCExtractOptions> options,
        IOptions<HttpClientOptions> httpClientOptions,
        ICASCExtractService cascExtractService)
    {
        _logger = logger;
        _options = options.Value;
        _httpClientOptions = httpClientOptions.Value;
        _cascExtractService = cascExtractService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, CASCExtractSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(CASCExtractCommand));

        SetOptions(settings);

        bool result = await _cascExtractService.RootDirectoryExtract();

        return result ? 0 : 1;
    }

    private void SetOptions(CASCExtractSettings settings)
    {
        _options.StorageLoad.Type = settings.StorageType;
        _options.StorageLoad.Path = settings.StorageDirectory?.FullName;
        _options.StorageLoad.Ptr = settings.IsPtr;
        _options.Flatten = settings.Flatten;

        if (settings.DuplicateHandling is not null)
            _options.DuplicateHandling = settings.DuplicateHandling.Value;

        if (settings.IncludeFilters.Contains(CASCExtractSettings.AllIncludePattern))
        {
            _options.IncludeFilters = [CASCExtractSettings.AllIncludePattern];
        }
        else if (settings.IncludeFilters.Any(x => x.Contains(_hdpFilter)))
        {
            _options.IncludeFilters = [
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

        _options.IncludeFilters.UnionWith(settings.IncludeFilters.Except(_specialFilters).Select(x => x.Replace('\\', '/')));
        _options.ExcludeFilters = new HashSet<string>(settings.ExcludeFilters, StringComparer.OrdinalIgnoreCase);

        _options.Threads = settings.Threads;

        if (settings.OutputDirectory is not null)
            _options.OutputDirectory = settings.OutputDirectory.FullName;

        _httpClientOptions.TimeoutSeconds = settings.HttpTimeout;
    }
}