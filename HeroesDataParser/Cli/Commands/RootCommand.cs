namespace HeroesDataParser.Cli.Commands;

public class RootCommand : AsyncCommand<RootSettings>
{
    private readonly RootOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IPreLoaderService _preLoaderService;
    private readonly IMainService _mainService;
    private readonly IPostCleanupService _postCleanupService;
    private readonly IResultSummaryService _resultSummaryService;

    public RootCommand(
        IOptions<RootOptions> options,
        IAnsiConsole console,
        IPreLoaderService preLoaderService,
        IMainService mainService,
        IPostCleanupService postCleanupService,
        IResultSummaryService resultSummaryService)
    {
        _options = options.Value;
        _console = console;
        _preLoaderService = preLoaderService;
        _mainService = mainService;
        _postCleanupService = postCleanupService;
        _resultSummaryService = resultSummaryService;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, RootSettings settings, CancellationToken cancellationToken)
    {
        SetOptions(settings);

        _console.MarkupLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        _console.MarkupLine($"[bold]{Constants.AppName} v{AppVersion.GetAppVersion()}[/]");
        _console.MarkupLine("  --[link]https://github.com/HeroesToolChest/HeroesDataParser[/]");
        _console.MarkupLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        _console.WriteLine();

        await _preLoaderService.Load();

        await _mainService.Start();
        _postCleanupService.Start();
        _resultSummaryService.PrintSummary();

        return 0;
    }

    private void SetOptions(RootSettings settings)
    {
        _options.StorageLoad.Type = settings.StorageType;
        _options.StorageLoad.Path = settings.StorageDirectory?.FullName;
        _options.StorageLoad.Ptr = settings.IsPtr;

        if (settings.OutputDirectory is not null)
            _options.OutputDirectory = settings.OutputDirectory.FullName;

        if (!string.IsNullOrWhiteSpace(settings.HeroesVersion) && HeroesDataVersion.TryParse(settings.HeroesVersion, out HeroesDataVersion? heroesDataVersion))
        {
            HeroesVersionOptions heroesVersionOptions = _options.HeroesVersion;
            heroesVersionOptions.Major = heroesDataVersion.Major;
            heroesVersionOptions.Minor = heroesDataVersion.Minor;
            heroesVersionOptions.Revision = heroesDataVersion.Revision;
            heroesVersionOptions.Build = heroesDataVersion.Build;
            heroesVersionOptions.IsPtr = heroesDataVersion.IsPtr;
        }

        foreach (string extractor in settings.Extractors)
        {
            ParseExtractor(extractor);
        }

        if (settings.StormLocales.Length > 0)
            _options.Localizations.Clear();

        foreach (string locale in settings.StormLocales)
        {
            if (Enum.TryParse(locale, true, out StormLocale stormLocale))
                _options.Localizations.Add(stormLocale);
        }

        _options.GameStringText.Type = settings.GameStringTextType;
        _options.GameStringText.ReplaceFontStyles = settings.GameStringReplaceFontStyles;
        _options.GameStringText.PreserveFont.PreserveFontStyleConstantVars = settings.GameStringPreserveConstantVars;
        _options.GameStringText.PreserveFont.PreserveFontStyleVars = settings.GameStringPreserveStyleVars;

        if (settings.GameStringPreserveConstantVars || settings.GameStringPreserveStyleVars)
            _options.GameStringText.ReplaceFontStyles = true;

        _options.LocalizedText = settings.LocalizedTextOption;
        _options.MapSpecificWriterJsonOutputType = settings.MapSpecificWriterJsonOutputType;
        _options.AllowEmptyMapSpecificDiffFiles = settings.AllowEmptyMapSpecificDiffFiles;
        _options.AllowEmptyMapSpecificDirectories = settings.AllowEmptyMapSpecificDirectories;
        _options.ShowLoadedCustomConfigFiles = settings.ShowLoadedCustomConfigFiles;
        _options.Threads = settings.Threads;
    }

    private void ParseExtractor(ReadOnlySpan<char> extractor)
    {
        Span<Range> keyPair = stackalloc Range[2];

        extractor.Split(keyPair, ':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        ReadOnlySpan<char> key = extractor[keyPair[0]];
        ReadOnlySpan<char> value = extractor[keyPair[1]]; // images

        if (!Enum.TryParse(key, true, out ExtractDataOptions extractorName))
            return;

        _options.Extractors[extractorName] = new ExtractorOptions()
        {
            IsEnabled = true,
            Images = !value.IsEmpty, // should already be validated
        };
    }
}
