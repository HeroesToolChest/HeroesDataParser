namespace HeroesDataParser.Infrastructure;

public class PreloadService : IPreloadService
{
    private readonly ILogger<PreloadService> _logger;
    private readonly RootOptions _options;
    private readonly IParsingConfigurationService _parsingConfigurationService;
    private readonly ICustomConfigurationService _customConfigurationService;
    private readonly Stopwatch _stopwatch = new();

    public PreloadService(ILogger<PreloadService> logger, IOptions<RootOptions> options, IParsingConfigurationService parsingConfigurationService, ICustomConfigurationService customConfigurationService)
    {
        _logger = logger;
        _options = options.Value;
        _parsingConfigurationService = parsingConfigurationService;
        _customConfigurationService = customConfigurationService;
    }

    public void Preload()
    {
        HeroesVersionCheck();
        SelectedLocalizations();
        SelectedDataOptions();

        AnsiConsole.MarkupLine("Loading configuration files...");

        _stopwatch.Start();
        LoadParsingConfiguration();
        LoadCustomConfigurationService();
        _stopwatch.Stop();

        AnsiConsole.MarkupLine($"Finished in {_stopwatch.Elapsed.TotalSeconds:0.###} seconds");
        AnsiConsole.WriteLine();
    }

    private static string? GetExtractorSubPath(ReadOnlySpan<char> directoryPath)
    {
        Span<Range> paths = stackalloc Range[4];
        int count = directoryPath.Split(paths, Path.DirectorySeparatorChar);

        if (count < 4)
            return null;

        return directoryPath[paths[3]].ToString();
    }

    private void HeroesVersionCheck()
    {
        if (!_options.HeroesVersion.IsOverridden)
            return;

        _logger.LogWarning("Heroes version {Version} was manually set and will override version from loaded data", _options.HeroesVersion.GetHeroesDataVersionString());
        AnsiConsole.MarkupLineInterpolated($"[yellow]Version [bold]{_options.HeroesVersion.GetHeroesDataVersionString()}[/] was manually set and will override version from loaded data.[/]");
    }

    private void SelectedLocalizations()
    {
        _logger.LogInformation("Selected localizations: {Locales}", string.Join(',', _options.Localizations));

        AnsiConsole.Markup($"[aqua]Localization(s):[/]");

        foreach (StormLocale locale in _options.Localizations)
        {
            AnsiConsole.Markup($" [aqua]{locale.ToString().ToLowerInvariant()}[/]");
        }

        AnsiConsole.WriteLine();
    }

    private void SelectedDataOptions()
    {
        _logger.LogInformation("Selected data types: {DataOptions}", string.Join(',', _options.Extractors.Keys));

        AnsiConsole.Markup($"[aqua]Data Type(s):[/]");

        foreach (KeyValuePair<string, ExtractorOptions> dataOption in _options.Extractors)
        {
            if (dataOption.Value.IsEnabled)
            {
                AnsiConsole.Markup($" [aqua]{dataOption.Key.ToLowerInvariant()}[/]");

                if (dataOption.Value.Images)
                {
                    AnsiConsole.Markup("[bold palegreen1]+[/]");
                }
            }
        }

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
    }

    private void LoadParsingConfiguration()
    {
        _parsingConfigurationService.Load();
        AnsiConsole.MarkupLine($"Loaded {Path.GetFileName(_parsingConfigurationService.SelectedFilePath)}");
    }

    private void LoadCustomConfigurationService()
    {
        _customConfigurationService.Load();

        IReadOnlyList<string> files = _customConfigurationService.SelectedCustomDataFilePaths;

        Tree root = new($"Loaded {files.Count} custom configuration files");

        foreach (string relativeFilePath in files)
        {
            string? subDirectory = GetExtractorSubPath(Path.GetDirectoryName(relativeFilePath));

            if (!string.IsNullOrWhiteSpace(subDirectory))
                root.AddNode($"{Path.GetFileName(relativeFilePath)} (.{Path.DirectorySeparatorChar}{subDirectory})");
            else
                root.AddNode(Path.GetFileName(relativeFilePath));
        }

        AnsiConsole.Write(root);
    }
}
