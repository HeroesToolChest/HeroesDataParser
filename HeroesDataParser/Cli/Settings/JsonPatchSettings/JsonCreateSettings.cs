using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.JsonPatchSettings;

public class JsonCreateSettings : JsonPatchSettings
{
    [CommandArgument(0, "<old-file-path>")]
    [Description("The path of the old json file")]
    public FileInfo OldFilePath { get; init; } = null!;

    [CommandArgument(1, "<new-file-path>")]
    [Description("The path of the new json")]
    public FileInfo NewFilePath { get; init; } = null!;

    [CommandOption("-o|--output-path <PATH>")]
    [Description("The path of the output directory where the new file will be created (defaults to the new file path directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    public override ValidationResult Validate()
    {
        if (!OldFilePath.Exists)
            return ValidationResult.Error("The provided <old-file-path> does not exist");

        if (!NewFilePath.Exists)
            return ValidationResult.Error("The provided <new-file-path> does not exist");

        if (OutputDirectory is not null && !OutputDirectory.Exists)
            return ValidationResult.Error("The provided --output-path does not exist");
        return ValidationResult.Success();
    }
}
