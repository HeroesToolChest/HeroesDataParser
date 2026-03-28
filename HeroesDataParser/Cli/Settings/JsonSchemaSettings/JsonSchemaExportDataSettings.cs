using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.JsonSchemaSettings;

public class JsonSchemaExportDataSettings : JsonSchemaExportSettings
{
    [CommandOption("-e|--extractor <EXTRACTOR>")]
    [Description("Extractor types to extract the json schema (can be specified multiple times)")]
    public string[] Extractors { get; init; } = [];

    [CommandOption("-o|--output-path <PATH>")]
    [Description("The path of the output directory (defaults to current directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    [CommandOption("--overwrite")]
    [Description("Allow the created files to override existing files in the output directory")]
    [DefaultValue(false)]
    public bool Overwrite { get; init; }

    [CommandOption("--no-indent")]
    [Description("Disable indentation in the output JSON files")]
    [DefaultValue(false)]
    public bool DisableJsonIndent { get; init; }

    public override ValidationResult Validate()
    {
        if (Extractors.Length == 0)
            return ValidationResult.Error("At least one --extractor must be specified");

        foreach (string extractor in Extractors)
        {
            if (!Enum.TryParse<ExtractDataOptions>(extractor, true, out _))
                return ValidationResult.Error($"--extractor has an invalid extractor '{extractor}'");
        }

        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        return ValidationResult.Success();
    }
}
