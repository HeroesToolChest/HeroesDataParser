using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.JsonPatchSettings;

public class JsonApplySettings : JsonPatchSettings
{
    [CommandArgument(0, "<file-path>")]
    [Description("The path of the original json file")]
    public FileInfo FilePath { get; init; } = null!;

    [CommandArgument(1, "<patch-file-path>")]
    [Description("The path of the json patch file")]
    public FileInfo PatchFilePath { get; init; } = null!;

    [CommandOption("-o|--output-path <PATH>")]
    [Description("The path of the output directory where the new file will be created (defaults to the patch file directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    [CommandOption("--overwrite")]
    [Description("Allow the created file to override an existing file")]
    [DefaultValue(false)]
    public bool Overwrite { get; init; }

    [CommandOption("--delete-patch-file")]
    [Description("Delete the patch file after applying it")]
    [DefaultValue(false)]
    public bool DeletePatchFile { get; init; }

    public override ValidationResult Validate()
    {
        if (!FilePath.Exists)
            return ValidationResult.Error("The provided <file-path> does not exist");

        if (!PatchFilePath.Exists)
            return ValidationResult.Error("The provided <patch-file-path> does not exist");

        if (OutputDirectory is not null && !OutputDirectory.Exists)
            return ValidationResult.Error("The provided --output-path does not exist");

        return ValidationResult.Success();
    }
}
