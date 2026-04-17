using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.JsonPatchSettings;

public class JsonApplySettings : JsonPatchSettings
{
    [CommandArgument(0, "<file-path>")]
    [Description("Path to the original JSON file")]
    public FileInfo FilePath { get; init; } = null!;

    [CommandArgument(1, "<patch-file-path>")]
    [Description("Path to the JSON patch file")]
    public FileInfo PatchFilePath { get; init; } = null!;

    [CommandOption("-o|--output-path <PATH>")]
    [Description("Output directory for the created file (defaults to the patch file's directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    [CommandOption("--overwrite")]
    [Description("Allow the created file to overwrite an existing file")]
    [DefaultValue(false)]
    public bool Overwrite { get; init; }

    [CommandOption("--delete-patch-file")]
    [Description("Delete the patch file after applying it")]
    [DefaultValue(false)]
    public bool DeletePatchFile { get; init; }

    [CommandOption("--no-indent")]
    [Description("Disable indentation in output JSON files")]
    [DefaultValue(false)]
    public bool DisableJsonIndent { get; init; }

    public override ValidationResult Validate()
    {
        if (!FilePath.Exists)
            return ValidationResult.Error("The provided <file-path> does not exist");

        if (!PatchFilePath.Exists)
            return ValidationResult.Error("The provided <patch-file-path> does not exist");

        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        return ValidationResult.Success();
    }
}
