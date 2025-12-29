namespace HeroesDataParser.Infrastructure;

public class MainService : IMainService
{
    private readonly ILogger<MainService> _logger;
    private readonly RootOptions _options;
    private readonly IMainLocalePreProcess _mainLocalePreProcess;
    private readonly IMainLocaleProcess _mainLocaleProcess;
    private readonly IImageWriterService _imageWriterService;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly Stopwatch _stopwatch = new();

    public MainService(
        ILogger<MainService> logger,
        IOptions<RootOptions> options,
        IMainLocalePreProcess mainLocalePreProcess,
        IMainLocaleProcess mainLocaleProcess,
        IImageWriterService imageWriterService,
        IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _logger = logger;
        _options = options.Value;
        _mainLocalePreProcess = mainLocalePreProcess;
        _mainLocaleProcess = mainLocaleProcess;
        _imageWriterService = imageWriterService;
        _heroesXmlLoaderService = heroesXmlLoaderService;
    }

    public async Task Start()
    {
        int count = 1;

        foreach (StormLocale locale in _options.Localizations)
        {
            _mainLocalePreProcess.Run();

            _options.CurrentLocale = locale;
            _logger.LogInformation("Localization: {Locale}", locale);
            AnsiConsole.Write(new Rule($"[[ [greenyellow]Locale: {locale}[/] ... [paleturquoise1]{count} of {_options.Localizations.Count}[/] ]]")
            {
                Justification = Justify.Left,
            });

            LoadGameStrings(locale);

            await _mainLocaleProcess.Run(locale);

            count++;
        }

        // write out all images
        await _imageWriterService.Write();
    }

    private void LoadGameStrings(StormLocale locale)
    {
        _logger.LogInformation("Loading gamestrings...");
        AnsiConsole.Write("Loading gamestrings...");

        _stopwatch.Start();
        _heroesXmlLoaderService.HeroesXmlLoader.LoadGameStrings(locale);
        _stopwatch.Stop();

        _logger.LogInformation("GameStrings {Locale} loaded", locale);
        AnsiConsole.WriteLine($"{_heroesXmlLoaderService.HeroesXmlLoader.GetCountOfGameStringsFiles()} text files (in {_stopwatch.Elapsed.TotalSeconds:0}s {_stopwatch.Elapsed.Milliseconds:0}ms)");
    }
}
