namespace HeroesDataParser.Options;

public class RootOptions
{
    public HeroesVersionOptions HeroesVersion { get; set; } = new();

    public int? BuildNumber => HeroesVersion.Build < 0 ? null : HeroesVersion.Build;

    public StorageLoadOptions StorageLoad { get; set; } = new();

    public string OutputDirectory { get; set; } = ".";

    public Dictionary<ExtractDataOptions, ExtractorOptions> Extractors { get; set; } = [];

    public HashSet<StormLocale> Localizations { get; set; } = [];

    public LocalizedTextOption LocalizedText { get; set; } = LocalizedTextOption.None;

    public GameStringTextOptions GameStringText { get; set; } = new();

    public MapSpecificWriterJsonOutputType MapSpecificWriterJsonOutputType { get; set; } = MapSpecificWriterJsonOutputType.None;

    public bool AllowEmptyMapSpecificDiffFiles { get; set; }

    public bool AllowEmptyMapSpecificDirectories { get; set; }

    public bool ShowLoadedCustomConfigFiles { get; set; }

    public int Threads { get; set; }

    public HiddenOptions Hidden { get; set; } = new();

    // properties below here a set/overridden during runtime
    public StormLocale CurrentLocale { get; set; } = StormLocale.ENUS;

    public string AppVersion { get; set; } = string.Empty;

    public ExtractDataOptions ExtractDataOptions { get; set; }

    public ExtractImageOptions ExtractImageOptions { get; set; }
}
