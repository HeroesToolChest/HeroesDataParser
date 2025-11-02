namespace HeroesDataParser.Infrastructure;

public class MainService : IMainService
{
    private readonly ILogger<MainService> _logger;
    private readonly RootOptions _options;
    private readonly IProcessorService _processorService;
    private readonly IMapProcessorService _mapProcessorService;
    private readonly IGameStringFileProcessorService _gameStringFileProcessorService;
    private readonly IImageWriterService _imageWriterService;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly Stopwatch _stopwatch = new();

    public MainService(
        ILogger<MainService> logger,
        IOptions<RootOptions> options,
        IProcessorService processorService,
        IMapProcessorService mapProcessorService,
        IGameStringFileProcessorService gameStringFileProcessorService,
        IImageWriterService imageWriterService,
        IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _logger = logger;
        _options = options.Value;
        _processorService = processorService;
        _mapProcessorService = mapProcessorService;
        _gameStringFileProcessorService = gameStringFileProcessorService;
        _imageWriterService = imageWriterService;
        _heroesXmlLoaderService = heroesXmlLoaderService;
    }

    public async Task Start()
    {
        int count = 1;

        foreach (StormLocale locale in _options.Localizations)
        {
            _options.CurrentLocale = locale;
            _logger.LogInformation("Localization: {Locale}", locale);
            AnsiConsole.Write(new Rule($"[[ [greenyellow]Locale: {locale}[/] ... [paleturquoise1]{count} of {_options.Localizations.Count}[/] ]]")
            {
                Justification = Justify.Left,
            });

            LoadGameStrings(locale);

            _logger.LogInformation("Starting processor service for {Locale}", locale);
            await _processorService.Start();

            _logger.LogInformation("Starting map processor service for {Locale}", locale);
            await _mapProcessorService.Start();

            // for gamestring extraction to a separate file
            await _gameStringFileProcessorService.Start();

            count++;
        }

        // write out all images
        await _imageWriterService.Write();
    }

    private void LoadGameStrings(StormLocale locale)
    {
        _logger.LogInformation("Loading gamestrings...");
        AnsiConsole.MarkupLine("Loading gamestrings...");

        _stopwatch.Start();
        _heroesXmlLoaderService.HeroesXmlLoader.LoadGameStrings(locale);
        _stopwatch.Stop();

        _logger.LogInformation("GameStrings {Locale} loaded", locale);
        AnsiConsole.MarkupLineInterpolated($"{_heroesXmlLoaderService.HeroesXmlLoader.GetCountOfGameStringsFiles(),6} text files loaded");
        AnsiConsole.MarkupLineInterpolated($"Finished loading gamestrings in {_stopwatch.Elapsed.TotalSeconds:0.###} seconds");
        AnsiConsole.WriteLine();
    }
}
