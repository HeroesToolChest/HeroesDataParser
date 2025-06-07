using System.Diagnostics;

namespace HeroesDataParser.Infrastructure;

public class MainService : IMainService
{
    private readonly ILogger<MainService> _logger;
    private readonly RootOptions _options;
    private readonly IProcessorService _processorService;
    private readonly IMapProcessorService _mapProcessorService;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly Stopwatch _stopwatch = new();

    public MainService(ILogger<MainService> logger, IOptions<RootOptions> options, IProcessorService processorService, IMapProcessorService mapProcessorService, IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _logger = logger;
        _options = options.Value;
        _processorService = processorService;
        _mapProcessorService = mapProcessorService;
        _heroesXmlLoaderService = heroesXmlLoaderService;
    }

    public async Task Start()
    {
        int count = 1;

        foreach (StormLocale locale in _options.Localizations)
        {
            _options.CurrentLocale = locale;
            _logger.LogInformation("Localization: {Locale}", locale);
            AnsiConsole.MarkupLineInterpolated($"[[[greenyellow]locale: {locale}[/] ... [paleturquoise1]{count} of {_options.Localizations.Count}[/]]]");

            LoadGameStrings(locale);

            _logger.LogInformation("Starting processor service for {Locale}", locale);
            await _processorService.Start();

            _logger.LogInformation("Starting map processor service for {Locale}", locale);
            await _mapProcessorService.Start();

            count++;
        }
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
