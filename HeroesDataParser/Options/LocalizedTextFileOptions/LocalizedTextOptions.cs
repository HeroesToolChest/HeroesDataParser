namespace HeroesDataParser.Options.LocalizedTextFileOptions;

public class LocalizedTextOptions
{
    public string OutputDirectory { get; set; } = ".";

    public bool AllowOverwrite { get; set; }

    public bool JsonIndent { get; set; }
}
