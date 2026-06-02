namespace HeroesDataParser.Options;

public class CASCExtractOptions
{
    public HeroesVersionOptions HeroesVersion { get; set; } = new();

    public StorageLoadOptions StorageLoad { get; set; } = new();

    public HashSet<string> IncludeFilters { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public HashSet<string> ExcludeFilters { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public int Threads { get; set; }

    public string OutputDirectory { get; set; } = ".";
}
