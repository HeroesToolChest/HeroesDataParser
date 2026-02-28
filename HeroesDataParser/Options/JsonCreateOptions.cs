namespace HeroesDataParser.Options;

public class JsonCreateOptions
{
    public string OldJsonFilePath { get; set; } = string.Empty;

    public string NewJsonFilePath { get; set; } = string.Empty;

    public string OutputFilePath { get; set; } = string.Empty;

    public bool AllowOverwrite { get; set; }
}
