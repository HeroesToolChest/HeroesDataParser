using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.LocalizedTextSettings;

public class LocalizedTextImportSettings : LocalizedTextSettings
{
    [CommandArgument(0, "<data-file-path>")]
    [Description("Path to the data file")]
    public FileInfo DataFilePath { get; init; } = null!;

    [CommandArgument(1, "<gamestrings-file-path>")]
    [Description("Path to the gamestrings file")]
    public FileInfo GameStringsFilePath { get; init; } = null!;

    [CommandOption("-o|--output-path <PATH>")]
    [Description("Output directory for the created file (defaults to the input data file's directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    [CommandOption("--overwrite")]
    [Description("Allow the created file to overwrite an existing file")]
    [DefaultValue(false)]
    public bool Overwrite { get; init; }

    [CommandOption("--no-indent")]
    [Description("Disable indentation in output JSON files")]
    [DefaultValue(false)]
    public bool DisableJsonIndent { get; init; }

    public override ValidationResult Validate()
    {
        if (!DataFilePath.Exists)
            return ValidationResult.Error("The provided <data-file-path> does not exist");

        if (!GameStringsFilePath.Exists)
            return ValidationResult.Error("The provided <gamestring-file-path> does not exist");

        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        return ValidationResult.Success();
    }
}
