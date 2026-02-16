namespace HeroesDataParser.Cli.Commands;

public class CASCExtractCommand : AsyncCommand<CASCExtractSettings>
{
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

        _options.Directories = new string[settings.Directories.Length];

        for (int i = 0; i < settings.Directories.Length; i++)
        {
            string directory = settings.Directories[i];

            if (directory.StartsWith(Path.DirectorySeparatorChar) || settings.Directories[i].StartsWith(Path.AltDirectorySeparatorChar))
            {
                _options.Directories[i] = directory.AsSpan()
                    .TrimStart(Path.DirectorySeparatorChar)
                    .TrimStart(Path.AltDirectorySeparatorChar)
                    .ToString();
            }
            else
            {
                _options.Directories[i] = directory;
            }
        }

        if (settings.Filters.Any(x => x.Contains('*')))
            _options.FileFilters = ["*"];
        else
            _options.FileFilters = [.. settings.Filters.Select(x => x.StartsWith('.') ? x : $".{x}")];

        _options.Threads = settings.Threads;

        if (settings.OutputDirectory is not null)
            _options.OutputDirectory = settings.OutputDirectory.FullName;
    }
}
