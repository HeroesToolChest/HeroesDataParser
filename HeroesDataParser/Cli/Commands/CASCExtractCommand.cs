using HeroesDataParser.Options.CASCExtractOptions;

namespace HeroesDataParser.Cli.Commands;

public class CASCExtractCommand : AsyncCommand<CASCExtractSettings>
{
    private readonly CASCExtractOptions _options;
    private readonly ICASCExtractorService _cascExtractorService;

    public CASCExtractCommand(IOptions<CASCExtractOptions> options, ICASCExtractorService cascExtractorService)
    {
        _options = options.Value;
        _cascExtractorService = cascExtractorService;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, CASCExtractSettings settings, CancellationToken cancellationToken)
    {
        SetOptions(settings);

        await _cascExtractorService.RootDirectoryExtract();

        return 0;
    }

    private void SetOptions(CASCExtractSettings settings)
    {
        _options.StorageLoad.Type = settings.StorageType;
        _options.StorageLoad.Path = settings.StorageDirectory?.FullName;
        _options.StorageLoad.Ptr = settings.IsPtr;

        _options.Directories = settings.Directories;

        if (settings.Filters.Any(x => x.Contains('*')))
            _options.FileFilters = ["*"];
        else
            _options.FileFilters = [.. settings.Filters.Select(x => x.StartsWith('.') ? x : $".{x}")];

        _options.Threads = settings.Threads;

        if (settings.OutputDirectory is not null)
            _options.OutputDirectory = settings.OutputDirectory.FullName;
    }
}
