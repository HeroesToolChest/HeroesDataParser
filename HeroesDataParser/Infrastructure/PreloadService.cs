using Serilog;
using System.Diagnostics;

namespace HeroesDataParser.Infrastructure;

public class PreloadService : IPreloadService
{
    private readonly ILogger<PreloadService> _logger;
    private readonly RootOptions _options;
    private readonly IParsingConfigurationService _parsingConfigurationService;
    private readonly Stopwatch _stopwatch = new();

    public PreloadService(ILogger<PreloadService> logger, IOptions<RootOptions> options, IParsingConfigurationService parsingConfigurationService)
    {
        _logger = logger;
        _options = options.Value;
        _parsingConfigurationService = parsingConfigurationService;
    }

    public void Preload()
    {
        SelectedLocalizations();

        AnsiConsole.MarkupLine("Loading configuration files...");

        _stopwatch.Start();
        LoadParsingConfiguration();
        LoadOverriders();
        _stopwatch.Stop();

        AnsiConsole.MarkupLine($"Finished in {_stopwatch.Elapsed.TotalSeconds:0.###} seconds");
        AnsiConsole.WriteLine();
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
        AnsiConsole.WriteLine();
    }

    private void LoadParsingConfiguration()
    {
        _parsingConfigurationService.Load();
        AnsiConsole.MarkupLine($"[aqua]Loaded {Path.GetFileName(_parsingConfigurationService.SelectedFilePath)}[/]");
    }

    private void LoadOverriders()
    {
        // TODO: Load overriders
    }
}
