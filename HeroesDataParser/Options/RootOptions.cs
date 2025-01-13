namespace HeroesDataParser.Options;

public class RootOptions
{
    public StorageLoadOptions StorageLoad { get; set; } = new();

    public string OutputDirectory { get; set; } = ".";

    public Dictionary<string, ExtractorOptions> Extractors { get; } = new Dictionary<string, ExtractorOptions>(StringComparer.OrdinalIgnoreCase);

    public HashSet<StormLocale> Localizations { get; set; } = [];

    public bool LocalizedText { get; set; }

    public int? BuildNumber { get; set; }
}
