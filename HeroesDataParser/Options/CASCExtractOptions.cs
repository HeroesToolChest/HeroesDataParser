namespace HeroesDataParser.Options;

public class CASCExtractOptions
{
    public HeroesVersionOptions HeroesVersion { get; set; } = new();

    public StorageLoadOptions StorageLoad { get; set; } = new();

    public HashSet<string> Directories { get; set; } = [];

    public HashSet<string> FileFilters { get; set; } = [];

    // for filters, this file does not have a file extension and is needed for maps
    public bool IncludeMapDocumentInfoFile { get; set; }

    public int Threads { get; set; }

    public string OutputDirectory { get; set; } = ".";
}
