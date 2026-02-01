using CASCLib;

namespace HeroesDataParser.Infrastructure;

public class HeroesXmlLoaderService : IHeroesXmlLoaderService
{
    private readonly ILogger<HeroesXmlLoaderService> _logger;
    private readonly RootOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICustomConfigurationService _customConfigurationService;
    private readonly Stopwatch _stopwatch = new();

    public HeroesXmlLoaderService(
        ILogger<HeroesXmlLoaderService> logger,
        IOptions<RootOptions> options,
        IAnsiConsole console,
        IHttpClientFactory httpClientFactory,
        ICustomConfigurationService customConfigurationService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _httpClientFactory = httpClientFactory;
        _customConfigurationService = customConfigurationService;
    }

    public HeroesXmlLoader HeroesXmlLoader { get; private set; } = HeroesXmlLoader.LoadWithEmpty();

    public async Task Load(LoadedConfiguration preloadData)
    {
        _logger.LogInformation("Loading heroes data...");

        if (!preloadData.HasCascConfig && !preloadData.HasModsInfoFile)
        {
            _logger.LogCritical("No valid preload data found to load from.");
            _console.Markup("[red]Error: No valid preload data found to load from.[/]");
            Environment.Exit(1);
            return;
        }

        // TODO: not going to be needed in this service (move to cli validation)
        if (VerifyPath() is false)
        {
            _logger.LogError("Failed to verify path.");
            Environment.Exit(1);
            return;
        }

        await ExecuteLoader(preloadData);
        await ExecuteDataLoading();

        _console.MarkupLineInterpolated($"{HeroesXmlLoader.GetCountOfXmlDataFiles(),6} xml files loaded");
        _console.MarkupLineInterpolated($"{HeroesXmlLoader.GetCountOfFontStyleFiles(),6} storm style files loaded");
        _console.MarkupLineInterpolated($"[green bold]Loading completed in {_stopwatch.Elapsed.TotalSeconds:0.####} seconds[/]");
        _console.WriteLine();
    }

    private async Task ExecuteLoader(LoadedConfiguration preloadData)
    {
        if (_options.StorageLoad.Type == StorageType.Mods)
            await RunLoader(preloadData);
        else
            await LoadFromCASC(preloadData);

        _console.MarkupLineInterpolated($"Load time: {_stopwatch.Elapsed.TotalSeconds:0.####} seconds");
        _console.WriteLine();

        if (HeroesXmlLoader is null)
            throw new InvalidOperationException("Failed to load from casc or file.");
    }

    private async Task ExecuteDataLoading()
    {
        await _console.Status()
            .Spinner(Spinner.Known.Default)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("[yellow bold]Initializing[/]", async ctx =>
            {
                _logger.LogInformation("Loading data from stormmods...");

                if (_options.StorageLoad.Type == StorageType.Online)
                    _console.MarkupLine("Loading data from stormmods (this might take a while)...");
                else
                    _console.MarkupLine("Loading data from stormmods...");

                _stopwatch.Restart();

                await Task.Run(() =>
                {
                    HeroesXmlLoader.LoadStormMods();
                });

                LoadCustomStormMod();

                _stopwatch.Stop();
            });

        _logger.LogInformation("Heroes data loaded");
    }

    private async Task LoadFromCASC(LoadedConfiguration preloadData)
    {
        await _console.Progress()
            .Columns(
            [
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
            ])
            .StartAsync(async ctx =>
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

                await RunLoader(preloadData, new ProgressReporter(progress));
            });
    }

    private async Task RunLoader(LoadedConfiguration preloadData, IProgressReporter? progressReporter = null)
    {
        if (_options.StorageLoad.Type == StorageType.Game && preloadData.HasCascConfig)
            await RunGameLoader(preloadData.CascConfig, progressReporter);
        else if (_options.StorageLoad.Type == StorageType.Mods && preloadData.HasModsInfoFile)
            await RunModsLoader(preloadData.ModsInfoFile, progressReporter);
        else if (preloadData.HasCascConfig)
            await RunOnlineLoader(preloadData.CascConfig, progressReporter);
    }

    private async Task RunGameLoader(CASCConfig cascConfig, IProgressReporter? progressReporter)
    {
        _logger.LogInformation("Loading heroes data by game storage");
        _console.MarkupLine("[gold1 bold]Loading from local CASC storage[/]...");

        _stopwatch.Start();

        await Task.Run(() =>
        {
            HeroesXmlLoader = HeroesXmlLoader.LoadWithCASC(cascConfig, _httpClientFactory.CreateClient(Constants.HttpClientBlizzard), progressReporter: progressReporter);
        });

        _stopwatch.Stop();
    }

    private async Task RunModsLoader(ModsInfoFile modsInfoFile, IProgressReporter? progressReporter)
    {
        _logger.LogInformation("Loading heroes data by extracted mods directory");
        _console.MarkupLine("[gold1 bold]Loading from extracted 'mods' directory[/]...");

        _stopwatch.Start();

        await Task.Run(() =>
        {
            HeroesXmlLoader = HeroesXmlLoader.LoadWithFile(_options.StorageLoad.Path!, modsInfoFile, progressReporter);
        });

        _stopwatch.Stop();
    }

    private async Task RunOnlineLoader(CASCConfig cascConfig, IProgressReporter? progressReporter)
    {
        _logger.LogInformation("Downloading heroes data by online storage");

        _console.MarkupLine("[gold1 bold]Downloading from online CASC storage[/]");

        _stopwatch.Start();

        await Task.Run(() =>
        {
            HeroesXmlLoader = HeroesXmlLoader.LoadWithCASC(cascConfig, _httpClientFactory.CreateClient(Constants.HttpClientBlizzard), progressReporter: progressReporter);
        });

        _stopwatch.Stop();
    }

    private bool VerifyPath()
    {
        if (_options.StorageLoad.Type == StorageType.Game || _options.StorageLoad.Type == StorageType.Mods)
        {
            if (string.IsNullOrWhiteSpace(_options.StorageLoad.Path))
            {
                _logger.LogCritical("StorageLoad path is empty.");
                _console.Markup("[red]Error: The storage load path is empty. Please provide a path to the game or mods directory.[/]");

                return false;
            }

            if (!Directory.Exists(_options.StorageLoad.Path))
            {
                _logger.LogCritical("StorageLoad path does not exist.");
                _console.Markup("[red]Error: The storage load path does not exist. Please provide a valid path to the game or mods directory.[/]");

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

        _console.MarkupLine("Loading data from custommods...");

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
