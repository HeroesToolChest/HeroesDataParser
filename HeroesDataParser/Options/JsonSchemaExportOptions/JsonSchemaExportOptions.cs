namespace HeroesDataParser.Options.JsonSchemaExportOptions;

public class JsonSchemaExportOptions
{
    public ExtractDataOptions ExtractDataOptions { get; set; } = ExtractDataOptions.None;

    public bool AllowOverwrite { get; set; }

    public string OutputDirectory { get; set; } = ".";

    public bool JsonIndent { get; set; }

    public string Version { get; set; } = "0.0.0";
}
