using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.JsonSchemaSettings;

public class JsonSchemaExportGameStringSettings : JsonSchemaExportSettings
{
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
        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        return ValidationResult.Success();
    }
}
