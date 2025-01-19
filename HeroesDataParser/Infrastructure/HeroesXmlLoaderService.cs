using CASCLib;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HeroesDataParser.Infrastructure;

public class HeroesXmlLoaderService : IHeroesXmlLoaderService
{
    private readonly ILogger<HeroesXmlLoaderService> _logger;
    private readonly RootOptions _options;

    public HeroesXmlLoaderService(ILogger<HeroesXmlLoaderService> logger, IOptions<RootOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public HeroesXmlLoader HeroesXmlLoader { get; private set; } = null!;

    public async Task Load()
    {
        _logger.LogInformation("Loading heroes data...");

        if (_options.StorageLoad.Type == StorageType.Mods)
            RunLoader();
        else
            await LoadFromCASC();

        if (HeroesXmlLoader is null)
            throw new InvalidOperationException("Failed to load from casc or file.");

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("[yellow bold]Initializing[/]", async ctx =>
            {
                _logger.LogInformation("Loading data from stormmods...");

                if (_options.StorageLoad.Type == StorageType.Online)
                    AnsiConsole.MarkupLine("Loading data from stormmods (this might take a while)...");
                else
                    AnsiConsole.MarkupLine("Loading data from stormmods...");

                HeroesXmlLoader.LoadStormMods();

                _logger.LogInformation("Loading gamestrings...");
                AnsiConsole.MarkupLine("Loading gamestrings...");
                HeroesXmlLoader.LoadGameStrings();

                await Task.CompletedTask;
            });

        _logger.LogInformation("Heroes data loaded");
        AnsiConsole.MarkupLine("[green bold]Loading completed[/]");
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
                backgroundWorkerEx.DoWork += (_, e) =>
                {
                    RunLoader(backgroundWorkerEx);
                };
                backgroundWorkerEx.ProgressChanged += (_, e) =>
                {
                    if (cdnIndexesTask.Value < 100)
                    {
                        cdnIndexesTask.Value = e.ProgressPercentage;
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
        if (_options.StorageLoad.Type == StorageType.Game && IsValidPath())
        {
            _logger.LogInformation("Loading heroes data by game storage");

            AnsiConsole.MarkupLine("[aqua]Found 'Heroes of the Storm' directory[/]");
            AnsiConsole.MarkupLine("[aqua]Loading local CASC storage... please wait[/]");

            HeroesXmlLoader = HeroesXmlLoader.LoadWithCASC(_options.StorageLoad.Path!, new CASCLoggerOptions(), backgroundWorkerEx);
        }
        else if (_options.StorageLoad.Type == StorageType.Mods && IsValidPath())
        {
            _logger.LogInformation("Loading heroes data by mods storage");

            AnsiConsole.MarkupLine("[aqua]Found 'mods' directory[/]");
            AnsiConsole.WriteLine();

            HeroesXmlLoader = HeroesXmlLoader.LoadWithFile(_options.StorageLoad.Path!, backgroundWorkerEx);
        }
        else if (_options.StorageLoad.Type == StorageType.Online)
        {
            _logger.LogInformation("Loading heroes data by online storage");

            AnsiConsole.MarkupLine("[aqua]Loading online CASC storage... please wait[/]");

            HeroesXmlLoader = HeroesXmlLoader.LoadWithOnlineCASC(new CASCLoggerOptions(), backgroundWorkerEx);
        }
        else
        {
            _logger.LogWarning("Unknown storage load type, defaulting to online storage");

            AnsiConsole.MarkupLine("[yellow]Unknown storage load type, defaulting to online storage.[/]");
            AnsiConsole.MarkupLine("[aqua]Loading online CASC storage... please wait[/]");

            HeroesXmlLoader = HeroesXmlLoader.LoadWithOnlineCASC(new CASCLoggerOptions(), backgroundWorkerEx);
        }
    }

    private bool IsValidPath()
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

        return true;
    }
}
