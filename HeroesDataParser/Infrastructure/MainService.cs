namespace HeroesDataParser.Infrastructure;

public class MainService : IMainService
{
    private readonly ILogger<MainService> _logger;
    private readonly RootOptions _options;
    private readonly IProcessorService _processorService;
    private readonly IMapProcessorService _mapProcessorService;
    private readonly IGameStringFileProcessorService _gameStringFileProcessorService;
    private readonly IImageWriterService _imageWriterService;
    private readonly ISerializedElementsService _serializedElementsService;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly Stopwatch _stopwatch = new();

    public MainService(
        ILogger<MainService> logger,
        IOptions<RootOptions> options,
        IProcessorService processorService,
        IMapProcessorService mapProcessorService,
        IGameStringFileProcessorService gameStringFileProcessorService,
        IImageWriterService imageWriterService,
        ISerializedElementsService serializedElementsService,
        IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _logger = logger;
        _options = options.Value;
        _processorService = processorService;
        _mapProcessorService = mapProcessorService;
        _gameStringFileProcessorService = gameStringFileProcessorService;
        _imageWriterService = imageWriterService;
        _serializedElementsService = serializedElementsService;
        _heroesXmlLoaderService = heroesXmlLoaderService;
    }

    public async Task Start()
    {
        int count = 1;

        foreach (StormLocale locale in _options.Localizations)
        {
            _serializedElementsService.ClearSerializedElements();

            _options.CurrentLocale = locale;
            _logger.LogInformation("Localization: {Locale}", locale);
            AnsiConsole.Write(new Rule($"[[ [greenyellow]Locale: {locale}[/] ... [paleturquoise1]{count} of {_options.Localizations.Count}[/] ]]")
            {
                Justification = Justify.Left,
            });

            LoadGameStrings(locale);

            _logger.LogInformation("Starting processor service for {Locale}", locale);
            _processorService.Start();

            _logger.LogInformation("Starting map processor service for {Locale}", locale);
            _mapProcessorService.Start();

            await _processorService.WriteDataFiles();
            await _mapProcessorService.WriteMapDataFile();

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
        AnsiConsole.Write("Loading gamestrings...");

        _stopwatch.Start();
        _heroesXmlLoaderService.HeroesXmlLoader.LoadGameStrings(locale);
        _stopwatch.Stop();

        _logger.LogInformation("GameStrings {Locale} loaded", locale);
        AnsiConsole.WriteLine($"{_heroesXmlLoaderService.HeroesXmlLoader.GetCountOfGameStringsFiles()} text files (in {_stopwatch.Elapsed.TotalSeconds:0}s {_stopwatch.Elapsed.Milliseconds:0}ms)");
    }
}
