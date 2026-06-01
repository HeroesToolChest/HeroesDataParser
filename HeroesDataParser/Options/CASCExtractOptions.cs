namespace HeroesDataParser.Options;

public class CASCExtractOptions
{
    public HeroesVersionOptions HeroesVersion { get; set; } = new();

    public StorageLoadOptions StorageLoad { get; set; } = new();

    public HashSet<string> FileFilters { get; set; } = [];

    public int Threads { get; set; }

    public string OutputDirectory { get; set; } = ".";
}
