namespace HeroesDataParser.Cli.Commands;

public class CASCExtractCommand : AsyncCommand<CASCExtractSettings>
{
    private static readonly string[] _specialFilters = ["*", "[hdp]"];

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

        foreach (string directory in settings.Directories)
        {
            if (directory.StartsWith(Path.DirectorySeparatorChar) || directory.StartsWith(Path.AltDirectorySeparatorChar))
            {
                _options.Directories.Add(directory.AsSpan()
                    .TrimStart(Path.DirectorySeparatorChar)
                    .TrimStart(Path.AltDirectorySeparatorChar)
                    .ToString());
            }
            else
            {
                _options.Directories.Add(directory);
            }
        }

        if (settings.Filters.Any(x => x.Contains('*')))
            _options.FileFilters = ["*"];
        else if (settings.Filters.Any(x => x.Contains("[hdp]")))
            _options.FileFilters = [".xml", ".txt", ".s2mv", ".s2ma", ".stormstyle", ".stormlayout"];

        _options.FileFilters.UnionWith(settings.Filters.Except(_specialFilters).Select(x => x.StartsWith('.') ? x : $".{x}"));
        _options.IncludeMapDocumentInfoFile = _options.FileFilters.Contains("*") || _options.FileFilters.Any(x => x.Equals(".s2ma", StringComparison.OrdinalIgnoreCase));
        _options.Threads = settings.Threads;

        if (settings.OutputDirectory is not null)
            _options.OutputDirectory = settings.OutputDirectory.FullName;
    }
}
