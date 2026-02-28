using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.JsonPatchSettings;

public class JsonCreateSettings : JsonPatchSettings
{
    [CommandArgument(0, "<old-file-path>")]
    [Description("The path of the old json file")]
    public FileInfo OldJsonFilePath { get; init; } = null!;

    [CommandArgument(1, "<new-file-path>")]
    [Description("The path of the new json")]
    public FileInfo NewJsonFilePath { get; init; } = null!;

    [CommandOption("-o|--output-path <PATH>")]
    [Description("The path of the output directory where the patch file will be created (defaults to the new file path directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    [CommandOption("--overwrite")]
    [Description("Allow the created file to override an existing file")]
    [DefaultValue(false)]
    public bool Overwrite { get; init; }

    public override ValidationResult Validate()
    {
        if (!OldJsonFilePath.Exists)
            return ValidationResult.Error("The provided <old-file-path> does not exist");

        if (!NewJsonFilePath.Exists)
            return ValidationResult.Error("The provided <new-file-path> does not exist");

        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        return ValidationResult.Success();
    }
}
