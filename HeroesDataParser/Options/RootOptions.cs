namespace HeroesDataParser.Options;

public class RootOptions
{
    public int? BuildNumber { get; set; }

    public StorageLoadOptions StorageLoad { get; set; } = new();

    public string OutputDirectory { get; set; } = ".";

    public Dictionary<string, ExtractorOptions> Extractors { get; } = new Dictionary<string, ExtractorOptions>(StringComparer.OrdinalIgnoreCase);

    public HashSet<StormLocale> Localizations { get; set; } = [];

    public bool LocalizedText { get; set; }

    public DescriptionTextOptions DescriptionText { get; set; } = new();
}
