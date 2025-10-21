using CASCLib;

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

    public PreloadData Preload()
    {
        PreloadData preloadData = new();

        HeroesVersionCheck(preloadData);
        SelectedLocalizations();
        SelectedDataOptions();

        AnsiConsole.MarkupLine("Loading configuration files...");

        _stopwatch.Start();
        LoadParsingConfiguration();
        LoadCustomConfigurationService();
        _stopwatch.Stop();

        AnsiConsole.MarkupLine($"Finished in {_stopwatch.Elapsed.TotalSeconds:0.###} seconds");
        AnsiConsole.WriteLine();

        return preloadData;
    }

    private static string? GetExtractorSubPath(ReadOnlySpan<char> directoryPath)
    {
        Span<Range> paths = stackalloc Range[4];
        int count = directoryPath.Split(paths, Path.DirectorySeparatorChar);

        if (count < 4)
            return null;

        return directoryPath[paths[3]].ToString();
    }

    private static HeroesDataVersion? GetVersionFromCascConfig(CASCConfig cascConfig)
    {
        string version;

        if (cascConfig.BuildUID.Equals(HeroesXmlLoader.ProductPtrName, StringComparison.OrdinalIgnoreCase))
            version = $"{cascConfig.VersionName}_ptr";
        else
            version = cascConfig.VersionName;

        if (!HeroesDataVersion.TryParse(version, out HeroesDataVersion? parsedVersion))
            return null;

        return parsedVersion;
    }

    // check the version from the selected storage vs. the user set version
    private void HeroesVersionCheck(PreloadData preloadData)
    {
        _logger.LogInformation("Load storage type {StorageType}", _options.StorageLoad.Type);

        HeroesDataVersion? heroesDataVersion = GetHeroesDataVersion(preloadData);

        if (heroesDataVersion is null)
        {
            _logger.LogWarning("Could not determine Heroes of the Storm data version from the selected storage");
            AnsiConsole.MarkupLineInterpolated($"[yellow]Version: UNKNOWN (default to [bold]{_options.HeroesVersion.GetAsHeroesDataVersion()}[/][/]");
            return;
        }

        if (_options.HeroesVersion.IsOverridden)
        {
            _logger.LogWarning("Game storage data version {LoadedVersion} is being overridden by user set version {SetVersion}", heroesDataVersion.ToString(), _options.HeroesVersion.GetAsHeroesDataVersion());
            AnsiConsole.MarkupLineInterpolated($"[yellow]Version: [bold]{_options.HeroesVersion.GetAsHeroesDataVersion()}[/] (overriding storage version [bold]{heroesDataVersion})[/][/]");
            return;
        }

        _options.HeroesVersion.Major = heroesDataVersion.Major;
        _options.HeroesVersion.Minor = heroesDataVersion.Minor;
        _options.HeroesVersion.Revision = heroesDataVersion.Revision;
        _options.HeroesVersion.Build = heroesDataVersion.Build;
        _options.HeroesVersion.IsPtr = heroesDataVersion.IsPtr;

        AnsiConsole.MarkupLineInterpolated($"[aqua]Version: [bold]{_options.HeroesVersion.GetAsHeroesDataVersion()}[/][/]");
    }

    private HeroesDataVersion? GetHeroesDataVersion(PreloadData preloadData)
    {
        HeroesDataVersion? heroesDataVersion;
        if (_options.StorageLoad.Type == StorageType.Game)
        {
            AnsiConsole.MarkupLine("[aqua]Storage: 'Heroes of the Storm' directory[/]");

            heroesDataVersion = GetGameVersion(preloadData);
        }
        else if (_options.StorageLoad.Type == StorageType.Mods)
        {
            AnsiConsole.MarkupLine("[aqua]Storage: 'mods' directory[/]");

            heroesDataVersion = GetModsVersion(preloadData);
        }
        else if (_options.StorageLoad.Type == StorageType.Online)
        {
            AnsiConsole.MarkupLine("[aqua]Storage: Online[/]");

            heroesDataVersion = GetOnlineVersion(preloadData);
        }
        else
        {
            AnsiConsole.MarkupLine("[aqua]Storage: Unknown, default to Online[/]");

            heroesDataVersion = GetOnlineVersion(preloadData);
        }

        return heroesDataVersion;
    }

    private HeroesDataVersion? GetGameVersion(PreloadData preloadData)
    {
        preloadData.CascConfig = HeroesXmlLoader.GetCASCConfig(_options.StorageLoad.Path!, new CASCLoggerOptions());

        return GetVersionFromCascConfig(preloadData.CascConfig);
    }

    private HeroesDataVersion? GetModsVersion(PreloadData preloadData)
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

    private HeroesDataVersion? GetOnlineVersion(PreloadData preloadData)
    {
        preloadData.CascConfig = HeroesXmlLoader.GetOnlineCASCConfig(_options.StorageLoad.Ptr, new CASCLoggerOptions());

        return GetVersionFromCascConfig(preloadData.CascConfig);
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
