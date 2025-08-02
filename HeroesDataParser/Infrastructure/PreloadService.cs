using System;
using System.Diagnostics;

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

    public void Preload()
    {
        SelectedLocalizations();

        AnsiConsole.MarkupLine("Loading configuration files...");

        _stopwatch.Start();
        LoadParsingConfiguration();
        LoadCustomConfigurationService();
        _stopwatch.Stop();

        AnsiConsole.MarkupLine($"Finished in {_stopwatch.Elapsed.TotalSeconds:0.###} seconds");
        AnsiConsole.WriteLine();
    }

    private static string? GetExtractorSubPath(ReadOnlySpan<char> directoryPath)
    {
        Span<Range> paths = stackalloc Range[4];
        int count = directoryPath.Split(paths, Path.DirectorySeparatorChar);

        if (count < 4)
            return null;

        return directoryPath[paths[3]].ToString();
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
        AnsiConsole.MarkupLine($"Loaded {Path.GetFileName(_parsingConfigurationService.SelectedFilePath)}");
    }

    private void LoadCustomConfigurationService()
    {
        _customConfigurationService.Load();

        IReadOnlyList<string> files = _customConfigurationService.SelectedCustomDataFilePaths;

        Tree root = new($"Loaded {files.Count} custom configuration files");
        //AnsiConsole.MarkupLine();

        foreach (string relativeFilePath in files)
        {
            string? subDirectory = GetExtractorSubPath(Path.GetDirectoryName(relativeFilePath));

            if (!string.IsNullOrWhiteSpace(subDirectory))
                root.AddNode($"{Path.GetFileName(relativeFilePath)} (.{Path.DirectorySeparatorChar}{subDirectory})");
            else
                root.AddNode(Path.GetFileName(relativeFilePath));
            //AnsiConsole.MarkupLine($"- {Path.GetFileName(relativeFilePath)}");
        }

        AnsiConsole.Write(root);
    }
}
