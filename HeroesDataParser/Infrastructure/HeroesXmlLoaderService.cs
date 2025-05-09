using CASCLib;
using System.Diagnostics;
using System.Xml.Linq;

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

    public async Task Load()
    {
        _logger.LogInformation("Loading heroes data...");

        // TODO: not going to be needed in this service (move to cli validation)
        if (VerifyPath() is false)
        {
            _logger.LogError("Failed to verify path.");
            Environment.Exit(1);
            return;
        }

        if (_options.StorageLoad.Type == StorageType.Mods)
            RunLoader();
        else
            await LoadFromCASC();

        AnsiConsole.MarkupLineInterpolated($"Load time: {_stopwatch.Elapsed.TotalSeconds:0.####} seconds");
        AnsiConsole.WriteLine();

        if (HeroesXmlLoader is null)
            throw new InvalidOperationException("Failed to load from casc or file.");

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("[yellow bold]Initializing[/]", async ctx =>
            {
                _logger.LogInformation("Loading data from stormmods...");

                if (_options.StorageLoad.Type == StorageType.Online)
                    AnsiConsole.MarkupLine("Loading data from stormmods (this might take a while, still downloading files)...");
                else
                    AnsiConsole.MarkupLine("Loading data from stormmods...");

                _stopwatch.Restart();

                HeroesXmlLoader.LoadStormMods();

                AnsiConsole.MarkupLine("Loading data from custommods...");
                LoadCustomStormMod();

                _logger.LogInformation("Loading gamestrings...");
                AnsiConsole.MarkupLine("Loading gamestrings...");
                HeroesXmlLoader.LoadGameStrings();

                _stopwatch.Stop();
                await Task.CompletedTask;
            });



        _logger.LogInformation("Heroes data loaded");

        AnsiConsole.MarkupLineInterpolated($"[green bold]Loading completed in {_stopwatch.Elapsed.TotalSeconds:0.####} seconds[/]");
    }

    private async Task LoadFromCASC()
    {
        await AnsiConsole.Progress()
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

                using BackgroundWorkerEx backgroundWorkerEx = new();

                string currentTask = string.Empty;
                backgroundWorkerEx.DoWork += (_, e) =>
                {
                    RunLoader(backgroundWorkerEx);
                };
                backgroundWorkerEx.ProgressChanged += (_, e) =>
                {
                    if (cdnIndexesTask.Value < 100)
                    {
                        cdnIndexesTask.Value = 100;
                    }
                    else if (_options.StorageLoad.Type != StorageType.Online && localIndexesTask?.Value < 100)
                    {
                        localIndexesTask.IsIndeterminate = false;
                        localIndexesTask.Value = e.ProgressPercentage;
                    }
                    else if (encodingTask.Value < 100)
                    {
                        encodingTask.IsIndeterminate = false;
                        encodingTask.Value = e.ProgressPercentage;
                    }
                    else if (rootTask.Value < 100)
                    {
                        rootTask.IsIndeterminate = false;
                        rootTask.Value = e.ProgressPercentage;
                    }
                    else if (listFileTask.Value < 100)
                    {
                        listFileTask.IsIndeterminate = false;
                        listFileTask.Value = e.ProgressPercentage;
                    }
                };
                backgroundWorkerEx.RunWorkerAsync();

                while (!ctx.IsFinished || backgroundWorkerEx.IsBusy)
                {
                    await Task.Delay(100);
                }
            });
    }

    private void RunLoader(BackgroundWorkerEx? backgroundWorkerEx = null)
    {
        if (_options.StorageLoad.Type == StorageType.Game)
        {
            _logger.LogInformation("Loading heroes data by game storage");

            AnsiConsole.MarkupLine("[aqua]Found 'Heroes of the Storm' directory[/]");
            AnsiConsole.MarkupLine("Loading from local CASC storage... please wait");

            _stopwatch.Start();
            HeroesXmlLoader = HeroesXmlLoader.LoadWithCASC(_options.StorageLoad.Path!, new CASCLoggerOptions(), backgroundWorkerEx);
            _stopwatch.Stop();
        }
        else if (_options.StorageLoad.Type == StorageType.Mods)
        {
            _logger.LogInformation("Loading heroes data by extracted mods directory");

            AnsiConsole.MarkupLine("[aqua]Found 'mods' directory[/]");
            AnsiConsole.MarkupLine("Loading from extracted 'mods' directory... please wait");

            _stopwatch.Start();
            HeroesXmlLoader = HeroesXmlLoader.LoadWithFile(_options.StorageLoad.Path!, backgroundWorkerEx);
            _stopwatch.Stop();
        }
        else if (_options.StorageLoad.Type == StorageType.Online)
        {
            _logger.LogInformation("Loading heroes data by online storage");

            AnsiConsole.MarkupLine("Loading from online CASC storage... please wait");

            _stopwatch.Start();
            HeroesXmlLoader = HeroesXmlLoader.LoadWithOnlineCASC(new CASCLoggerOptions(), backgroundWorkerEx);
            _stopwatch.Stop();
        }
        else
        {
            _logger.LogWarning("Unknown storage load type, defaulting to online storage");

            AnsiConsole.MarkupLine("[yellow]Unknown storage load type, defaulting to online storage.");
            AnsiConsole.MarkupLine("Loading from online CASC storage... please wait");

            _stopwatch.Start();
            HeroesXmlLoader = HeroesXmlLoader.LoadWithOnlineCASC(new CASCLoggerOptions(), backgroundWorkerEx);
            _stopwatch.Stop();
        }
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
        ISet<string> files = _customConfigurationService.SelectedCustomDataFilePaths;

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
