using CASCLib;

namespace HeroesDataParser.Infrastructure;

public class HeroesXmlLoaderService : IHeroesXmlLoaderService
{
    private readonly ILogger<HeroesXmlLoaderService> _logger;
    private readonly RootOptions _options;
    private readonly ICustomConfigurationService _customConfigurationService;
    private readonly Stopwatch _stopwatch = new();

    public HeroesXmlLoaderService(ILogger<HeroesXmlLoaderService> logger, IOptions<RootOptions> options, ICustomConfigurationService customConfigurationService)
    {
        _logger = logger;
        _options = options.Value;
        _customConfigurationService = customConfigurationService;
    }

    public HeroesXmlLoader HeroesXmlLoader { get; private set; } = null!;

    public void Load()
    {
        _logger.LogInformation("Loading heroes data...");

        // TODO: not going to be needed in this service (move to cli validation)
        if (VerifyPath() is false)
        {
            _logger.LogError("Failed to verify path.");
            Environment.Exit(1);
            return;
        }

        ExecuteLoader();
        ExecuteDataLoading();

        AnsiConsole.MarkupLineInterpolated($"{HeroesXmlLoader.GetCountOfXmlDataFiles(),6} xml files loaded");
        AnsiConsole.MarkupLineInterpolated($"{HeroesXmlLoader.GetCountOfFontStyleFiles(),6} storm style files loaded");
        AnsiConsole.MarkupLineInterpolated($"[green bold]Loading completed in {_stopwatch.Elapsed.TotalSeconds:0.####} seconds[/]");
        AnsiConsole.WriteLine();

        SetHeroesVersion();
    }

    private void ExecuteLoader()
    {
        if (_options.StorageLoad.Type == StorageType.Mods)
            RunLoader();
        else
            LoadFromCASC();

        AnsiConsole.MarkupLineInterpolated($"Load time: {_stopwatch.Elapsed.TotalSeconds:0.####} seconds");
        AnsiConsole.WriteLine();

        if (HeroesXmlLoader is null)
            throw new InvalidOperationException("Failed to load from casc or file.");
    }

    private void ExecuteDataLoading()
    {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .Start("[yellow bold]Initializing[/]", ctx =>
            {
                _logger.LogInformation("Loading data from stormmods...");

                if (_options.StorageLoad.Type == StorageType.Online)
                    AnsiConsole.MarkupLine("Loading data from stormmods (this might take a while)...");
                else
                    AnsiConsole.MarkupLine("Loading data from stormmods...");

                _stopwatch.Restart();

                HeroesXmlLoader.LoadStormMods();

                AnsiConsole.MarkupLine("Loading data from custommods...");
                LoadCustomStormMod();

                _stopwatch.Stop();
            });

        _logger.LogInformation("Heroes data loaded");
    }

    private void SetHeroesVersion()
    {
        if (!HeroesDataVersion.TryParse(HeroesXmlLoader.HeroesVersion, out HeroesDataVersion? parsedVersion))
        {
            _logger.LogWarning("Failed to parse heroes version: {Version}", HeroesXmlLoader.HeroesVersion);
            return;
        }

        if (_options.HeroesVersion.IsOverridden)
        {
            _logger.LogInformation("Heroes version {Version} was manually set, skipping setting version {LoadedVersion} from loaded data", _options.HeroesVersion.GetHeroesDataVersionString(), parsedVersion.ToString());
            AnsiConsole.MarkupLineInterpolated($"[yellow]Warning: Version [bold]{_options.HeroesVersion.GetHeroesDataVersionString()}[/] was manually set and is overriding version [bold]{parsedVersion}[/] from loaded data.[/]");
            AnsiConsole.WriteLine();
            return;
        }

        _options.HeroesVersion.Major = parsedVersion.Major;
        _options.HeroesVersion.Minor = parsedVersion.Minor;
        _options.HeroesVersion.Revision = parsedVersion.Revision;
        _options.HeroesVersion.Build = parsedVersion.Build;
        _options.HeroesVersion.IsPtr = parsedVersion.IsPtr;
    }

    private void LoadFromCASC()
    {
        AnsiConsole.Progress()
            .Columns(
            [
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
            ])
            .Start(ctx =>
            {
                ProgressTask cdnIndexesTask = ctx.AddTask("Loading CDN Indexes");

                ProgressTask? localIndexesTask = null;
                if (_options.StorageLoad.Type != StorageType.Online)
                {
                    localIndexesTask = ctx.AddTask("Loading Local Indexes");
                    cdnIndexesTask.IsIndeterminate = true;
                }

                ProgressTask encodingTask = ctx.AddTask("Loading Encoding");
                ProgressTask rootTask = ctx.AddTask("Loading Root");
                ProgressTask listFileTask = ctx.AddTask("Loading ListFile");

                encodingTask.IsIndeterminate = true;
                rootTask.IsIndeterminate = true;
                listFileTask.IsIndeterminate = true;

                Progress<ProgressInfo> progress = new(p =>
                {
                    switch (p.Stage)
                    {
                        case ProgressStage.CDNIndexes:
                            cdnIndexesTask.IsIndeterminate = false;
                            cdnIndexesTask.Value = p.Percentage;
                            break;
                        case ProgressStage.LocalIndexes:
                            if (localIndexesTask is not null)
                            {
                                localIndexesTask.IsIndeterminate = false;
                                localIndexesTask.Value = p.Percentage;
                            }

                            break;
                        case ProgressStage.Encoding:
                            encodingTask.IsIndeterminate = false;
                            encodingTask.Value = p.Percentage;
                            break;
                        case ProgressStage.Root:
                            rootTask.IsIndeterminate = false;
                            rootTask.Value = p.Percentage;
                            break;
                        case ProgressStage.ListFile:
                            listFileTask.IsIndeterminate = false;
                            listFileTask.Value = p.Percentage;
                            break;
                        default:
                            break;
                    }
                });

                RunLoader(new ProgressReporter(progress));
            });
    }

    private void RunLoader(IProgressReporter? progresReporter = null)
    {
        if (_options.StorageLoad.Type == StorageType.Game)
        {
            _logger.LogInformation("Loading heroes data by game storage");

            AnsiConsole.MarkupLine("[aqua]Found 'Heroes of the Storm' directory[/]");
            AnsiConsole.MarkupLine("[gold1 bold]Loading from local CASC storage[/]...");

            _stopwatch.Start();

            CASCConfig cascConfig = HeroesXmlLoader.GetCASCConfig(_options.StorageLoad.Path!, new CASCLoggerOptions());

            _logger.LogInformation("Version {Version} from local CASC storage", cascConfig.VersionName);
            AnsiConsole.MarkupLineInterpolated($"[aqua]Version: [bold]{cascConfig.VersionName}[/][/]");

            HeroesXmlLoader = HeroesXmlLoader.LoadWithCASC(cascConfig, progresReporter);

            _stopwatch.Stop();
        }
        else if (_options.StorageLoad.Type == StorageType.Mods)
        {
            _logger.LogInformation("Loading heroes data by extracted mods directory");

            AnsiConsole.MarkupLine("[aqua]Found 'mods' directory[/]");
            AnsiConsole.MarkupLine("[gold1 bold]Loading from extracted 'mods' directory[/]...");

            _stopwatch.Start();

            HeroesXmlLoader = HeroesXmlLoader.LoadWithFile(_options.StorageLoad.Path!, progresReporter);

            _logger.LogInformation("Version {Version} from local extracted mods storage", HeroesXmlLoader.HeroesVersion ?? "UNKNOWN");
            AnsiConsole.MarkupLineInterpolated($"[aqua]Version: [bold]{HeroesXmlLoader.HeroesVersion ?? "UNKNOWN"}[/][/]");

            _stopwatch.Stop();
        }
        else if (_options.StorageLoad.Type == StorageType.Online)
        {
            _logger.LogInformation("Downloading heroes data by online storage");

            AnsiConsole.MarkupLine("[gold1 bold]Downloading from online CASC storage[/]");

            _stopwatch.Start();
            LoadFromOnlineCASC(progresReporter);
            _stopwatch.Stop();
        }
        else
        {
            _logger.LogWarning("Unknown storage load type, defaulting to online storage");

            AnsiConsole.MarkupLine("[yellow]Unknown storage load type, defaulting to online storage.");
            AnsiConsole.MarkupLine("[gold1 bold]Downloading from online CASC storage[/]");

            _stopwatch.Start();
            LoadFromOnlineCASC(progresReporter);
            _stopwatch.Stop();
        }
    }

    private void LoadFromOnlineCASC(IProgressReporter? progresReporter)
    {
        CASCConfig cascConfig = HeroesXmlLoader.GetOnlineCASCConfig(false, new CASCLoggerOptions());

        _logger.LogInformation("Downloading version {Version} from online storage", cascConfig.VersionName);
        AnsiConsole.MarkupLineInterpolated($"[aqua]Version: [bold]{cascConfig.VersionName}[/][/]");

        HeroesXmlLoader = HeroesXmlLoader.LoadWithCASC(cascConfig, progresReporter);
    }

    private bool VerifyPath()
    {
        if (_options.StorageLoad.Type == StorageType.Game || _options.StorageLoad.Type == StorageType.Mods)
        {
            if (string.IsNullOrWhiteSpace(_options.StorageLoad.Path))
            {
                _logger.LogCritical("StorageLoad path is empty.");
                AnsiConsole.Markup("[red]Error: The storage load path is empty. Please provide a path to the game or mods directory.[/]");

                return false;
            }

            if (!Directory.Exists(_options.StorageLoad.Path))
            {
                _logger.LogCritical("StorageLoad path does not exist.");
                AnsiConsole.Markup("[red]Error: The storage load path does not exist. Please provide a valid path to the game or mods directory.[/]");

                return false;
            }
        }

        return true;
    }

    private void LoadCustomStormMod()
    {
        IReadOnlyList<string> files = _customConfigurationService.SelectedCustomDataFilePaths;

        if (files.Count < 1)
        {
            _logger.LogInformation("No custom configuration files found");
            return;
        }

        ManualModLoader manualModLoader = new("hdp");

        foreach (string relativeFilePath in files)
        {
            if (!Path.Exists(relativeFilePath))
            {
                _logger.LogWarning("Custom configuration file {RelativeFilePath} does not exist", relativeFilePath);
                continue;
            }

            XDocument xDoc = XDocument.Load(relativeFilePath);
            if (xDoc.Root is null)
            {
                _logger.LogWarning("Custom configuration file {RelativeFilePath} root does not exist", relativeFilePath);
                continue;
            }

            manualModLoader.AddElements(xDoc.Root.Elements());
        }

        HeroesXmlLoader.LoadCustomMod(manualModLoader);
    }
}
