using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.LocalizedTextSettings;

public class LocalizedTextExportSettings : LocalizedTextSettings
{
    [CommandArgument(0, "<data-file-path>")]
    [Description("Path to the data file")]
    public FileInfo DataFilePath { get; init; } = null!;

    [CommandArgument(1, "<extract-type>")]
    [Description("Action to perform on gamestring properties")]
    public ExtractType ExtractType { get; init; }

    [CommandOption("-g|--gamestrings-file-path <PATH>")]
    [Description("Path to the gamestrings file")]
    public FileInfo? GameStringFilePath { get; init; }

    [CommandOption("-o|--output-path <PATH>")]
    [Description("Output directory for the created files (defaults to the input data file's directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    [CommandOption("--overwrite")]
    [Description("Allow the created files to overwrite existing files")]
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

        if (GameStringFilePath is not null && !GameStringFilePath.Exists)
            return ValidationResult.Error("The provided --gamestring-file-path does not exist");

        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        if ((int)ExtractType > 3)
            return ValidationResult.Error("<extract-type> must be a value less than 4");

        if (ExtractType == ExtractType.Remove && GameStringFilePath is not null)
            return ValidationResult.Error("--gamestring-file-path cannot be provided when <extract-type> is set to Remove");

        return ValidationResult.Success();
    }
}
