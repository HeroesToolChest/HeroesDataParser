namespace HeroesDataParser.Options;

public class RootOptions
{
    public string OutputDirectory { get; set; } = ".";

    public Dictionary<string, ExtractorOptions> Extractors { get; } = new Dictionary<string, ExtractorOptions>(StringComparer.OrdinalIgnoreCase);

    public HashSet<StormLocale> Localizations { get; set; } = [];

    public bool LocalizedText { get; set; }
}
