namespace HeroesDataParser.Infrastructure;

public class ConfigurationLoaderService : IConfigurationLoaderService
{
    private readonly ILogger<ConfigurationLoaderService> _logger;
    private readonly RootOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IParsingConfigurationService _parsingConfigurationService;
    private readonly ICustomConfigurationService _customConfigurationService;
    private readonly Stopwatch _stopwatch = new();

    public ConfigurationLoaderService(
        ILogger<ConfigurationLoaderService> logger,
        IOptions<RootOptions> options,
        IAnsiConsole console,
        IHttpClientFactory httpClientFactory,
        IParsingConfigurationService parsingConfigurationService,
        ICustomConfigurationService customConfigurationService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _httpClientFactory = httpClientFactory;
        _parsingConfigurationService = parsingConfigurationService;
        _customConfigurationService = customConfigurationService;
    }

    public LoadedConfiguration LoadConfiguration()
    {
        LoadedConfiguration loadedConfiguration = new();

        HeroesVersionCheck(loadedConfiguration);
        SelectedLocalizations();
        SelectedDataOptions();
        OutputDirectory();

        _console.WriteLine();
        _console.MarkupLine("Loading configuration files...");

        _stopwatch.Start();
        LoadParsingConfiguration();
        LoadCustomConfigurationService();
        _stopwatch.Stop();

        _console.MarkupLine($"Finished in {_stopwatch.Elapsed.TotalSeconds:0.###} seconds");
        _console.WriteLine();

        return loadedConfiguration;
    }

    public string GetHeroesVersion()
    {
        HeroesDataVersion? heroesDataVersion = GetHeroesDataVersion(new LoadedConfiguration());

        if (heroesDataVersion is null)
            return "unknown";
        else
            return heroesDataVersion.GetAsVersionString();
    }

    private static string? GetExtractorSubPath(ReadOnlySpan<char> directoryPath)
    {
        Span<Range> paths = stackalloc Range[4];
        int count = directoryPath.Split(paths, Path.DirectorySeparatorChar);

        if (count < 4)
            return null;

        return directoryPath[paths[3]].ToString();
    }

    // check the version from the selected storage versus the user set version (if any)
    private void HeroesVersionCheck(LoadedConfiguration loadedConfiguration)
    {
        _logger.LogInformation("Load storage type {StorageType}", _options.StorageLoad.Type);

        HeroesDataVersion? heroesDataVersion = GetHeroesDataVersion(loadedConfiguration);

        if (heroesDataVersion is null)
        {
            _logger.LogWarning("Could not determine Heroes of the Storm data version from the selected storage");
            _console.MarkupLineInterpolated($"[yellow]Version: UNKNOWN (default to [bold]{_options.HeroesVersion.GetAsHeroesDataVersion()})[/][/]");
            return;
        }

        if (_options.HeroesVersion.IsOverridden)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Game storage data version {LoadedVersion} is being overridden by user set version {SetVersion}", heroesDataVersion.ToString(), _options.HeroesVersion.GetAsHeroesDataVersion());

            _console.MarkupLineInterpolated($"[yellow]Version: [bold]{_options.HeroesVersion.GetAsHeroesDataVersion()}[/] (overriding storage version [bold]{heroesDataVersion})[/][/]");
            return;
        }

        _options.HeroesVersion.Major = heroesDataVersion.Major;
        _options.HeroesVersion.Minor = heroesDataVersion.Minor;
        _options.HeroesVersion.Revision = heroesDataVersion.Revision;
        _options.HeroesVersion.Build = heroesDataVersion.Build;
        _options.HeroesVersion.IsPtr = heroesDataVersion.IsPtr;

        _console.MarkupLineInterpolated($"[aqua]Version: [bold]{_options.HeroesVersion.GetAsHeroesDataVersion()}[/][/]");
    }

    private HeroesDataVersion? GetHeroesDataVersion(LoadedConfiguration loadedConfiguration)
    {
        HeroesDataVersion? heroesDataVersion;
        if (_options.StorageLoad.Type == StorageType.Game)
        {
            if (!_options.ShowHeroesVersion)
                _console.MarkupLine("[aqua]Storage: 'Heroes of the Storm' directory[/]");

            heroesDataVersion = GetGameVersion(loadedConfiguration);
        }
        else if (_options.StorageLoad.Type == StorageType.Mods)
        {
            if (!_options.ShowHeroesVersion)
                _console.MarkupLine("[aqua]Storage: 'mods' directory[/]");

            heroesDataVersion = GetModsVersion(loadedConfiguration);
        }
        else if (_options.StorageLoad.Type == StorageType.Online)
        {
            if (!_options.ShowHeroesVersion)
                _console.MarkupLine("[aqua]Storage: Online[/]");

            heroesDataVersion = GetOnlineVersion(loadedConfiguration);
        }
        else
        {
            if (!_options.ShowHeroesVersion)
                _console.MarkupLine("[aqua]Storage: Unknown, default to Online[/]");

            _options.StorageLoad.Type = StorageType.Online;
            heroesDataVersion = GetOnlineVersion(loadedConfiguration);
        }

        return heroesDataVersion;
    }

    private HeroesDataVersion? GetGameVersion(LoadedConfiguration preloadData)
    {
        preloadData.CascConfig = HeroesXmlLoader.GetCASCConfig(_options.StorageLoad.Path!, new CASCLoggerOptions());

        return preloadData.CascConfig.GetVersionFromCascConfig();
    }

    private HeroesDataVersion? GetModsVersion(LoadedConfiguration preloadData)
    {
        ModsInfoFile? modsInfoFile = HeroesXmlLoader.GetModsInfoFile(_options.StorageLoad.Path!);
        preloadData.ModsInfoFile = modsInfoFile;

        if (modsInfoFile is null || string.IsNullOrWhiteSpace(modsInfoFile.Version))
            return null;

        string version;

        if (modsInfoFile.IsPtr)
            version = $"{modsInfoFile.Version}_ptr";
        else
            version = modsInfoFile.Version;

        if (!HeroesDataVersion.TryParse(version, out HeroesDataVersion? parsedVersion))
            return null;

        return parsedVersion;
    }

    private HeroesDataVersion? GetOnlineVersion(LoadedConfiguration preloadData)
    {
        preloadData.CascConfig = HeroesXmlLoader.GetOnlineCASCConfig(_httpClientFactory.CreateClient(Constants.HttpClientBlizzard), _options.StorageLoad.Ptr, new CASCLoggerOptions());

        return preloadData.CascConfig.GetVersionFromCascConfig();
    }

    private void SelectedLocalizations()
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Selected localizations: {Locales}", string.Join(',', _options.Localizations));

        _console.Markup($"[aqua]Localization(s):[/]");

        foreach (StormLocale locale in _options.Localizations)
        {
            _console.Markup($" [aqua]{locale.ToString().ToLowerInvariant()}[/]");
        }

        _console.WriteLine();
    }

    private void SelectedDataOptions()
    {
        ExtractDataOptions selectDataExtractOptions = ExtractDataOptions.None;
        ExtractImageOptions selectImageExtractOptions = ExtractImageOptions.None;

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Selected data types: {DataOptions}", string.Join(',', _options.Extractors.Keys));

        _console.Markup($"[aqua]Data Type(s):[/]");

        foreach (KeyValuePair<ExtractDataOptions, ExtractorOptions> extractorDataOption in _options.Extractors)
        {
            if (!extractorDataOption.Value.IsEnabled)
                continue;

            selectDataExtractOptions |= extractorDataOption.Key;

            _console.Markup($" [aqua]{extractorDataOption.Key.ToString().ToLowerInvariant()}[/]");

            if (extractorDataOption.Value.Images)
            {
                _console.Markup("[bold palegreen1]+[/]");

                UpdateExtractImageOptions(ref selectImageExtractOptions, extractorDataOption);
            }
        }

        _console.WriteLine();

        _logger.LogDebug("Selected data extractors: {@DataOptions}", selectDataExtractOptions);
        _logger.LogDebug("Selected image extractors: {@ImageOptions}", selectImageExtractOptions);
        _options.ExtractDataOptions = selectDataExtractOptions;
        _options.ExtractImageOptions = selectImageExtractOptions;
    }

    private void OutputDirectory()
    {
        string fullOutputDirectory = Path.GetFullPath(_options.OutputDirectory);

        _logger.LogInformation("Output directory: {OutputDirectory}", fullOutputDirectory);
        _console.MarkupLineInterpolated($"[aqua]Output Directory: {fullOutputDirectory}[/]");
    }

    private void LoadParsingConfiguration()
    {
        _parsingConfigurationService.Load();
        if (!string.IsNullOrWhiteSpace(_parsingConfigurationService.SelectedFilePath))
            _console.MarkupLine($"Loaded {Path.GetFileName(_parsingConfigurationService.SelectedFilePath)}");
        else
            _console.MarkupLine("No parsing configuration file loaded");
    }

    private void LoadCustomConfigurationService()
    {
        _customConfigurationService.Load();

        IReadOnlyList<string> files = _customConfigurationService.SelectedCustomDataFilePaths;

        Tree root = new($"Loaded {files.Count} custom configuration files");

        if (_options.ShowLoadedCustomConfigFiles)
        {
            foreach (string relativeFilePath in files)
            {
                string? subDirectory = GetExtractorSubPath(Path.GetDirectoryName(relativeFilePath));

                if (!string.IsNullOrWhiteSpace(subDirectory))
                    root.AddNode($"{Path.GetFileName(relativeFilePath)} (.{Path.DirectorySeparatorChar}{subDirectory})");
                else
                    root.AddNode(Path.GetFileName(relativeFilePath));
            }
        }

        _console.Write(root);
    }

    private void UpdateExtractImageOptions(ref ExtractImageOptions selectImageExtractOptions, KeyValuePair<ExtractDataOptions, ExtractorOptions> extractDataOption)
    {
        if (extractDataOption.Value.IsEnabled is false || extractDataOption.Value.Images is false)
            return;

        if (extractDataOption.Key.HasFlag(ExtractDataOptions.Hero))
        {
            if (_options.Hidden.HeroImages.HeroPortraits)
                selectImageExtractOptions |= ExtractImageOptions.HeroPortrait;
            if (_options.Hidden.HeroImages.Talents)
                selectImageExtractOptions |= ExtractImageOptions.Talent;
            if (_options.Hidden.Abilities)
                selectImageExtractOptions |= ExtractImageOptions.Ability;
            if (_options.Hidden.AbilityTalents)
                selectImageExtractOptions |= ExtractImageOptions.AbilityTalent;
            if (_options.Hidden.HeroImages.HeroData)
                selectImageExtractOptions |= ExtractImageOptions.HeroData;
            if (_options.Hidden.HeroImages.HeroDataSplit)
                selectImageExtractOptions |= ExtractImageOptions.HeroDataSplit;

            return;
        }

        if (extractDataOption.Key.HasFlag(ExtractDataOptions.Unit))
        {
            if (_options.Hidden.UnitImages.UnitPortraits)
                selectImageExtractOptions |= ExtractImageOptions.UnitPortrait;
            if (_options.Hidden.Abilities)
                selectImageExtractOptions |= ExtractImageOptions.Ability;
            if (_options.Hidden.AbilityTalents)
                selectImageExtractOptions |= ExtractImageOptions.AbilityTalent;
            if (_options.Hidden.UnitImages.UnitData)
                selectImageExtractOptions |= ExtractImageOptions.UnitData;

            return;
        }

        if (extractDataOption.Key.HasFlag(ExtractDataOptions.Map))
        {
            if (_options.Hidden.MapImages.ReplayPreviews)
                selectImageExtractOptions |= ExtractImageOptions.ReplayPreview;
            if (_options.Hidden.MapImages.LoadingScreens)
                selectImageExtractOptions |= ExtractImageOptions.LoadingScreen;
            if (_options.Hidden.MapImages.MapObjectiveIcons)
                selectImageExtractOptions |= ExtractImageOptions.MapObjectives;

            return;
        }

        if (!Enum.TryParse(extractDataOption.Key.ToString(), true, out ExtractImageOptions result))
            return;

        selectImageExtractOptions |= result;
    }
}
