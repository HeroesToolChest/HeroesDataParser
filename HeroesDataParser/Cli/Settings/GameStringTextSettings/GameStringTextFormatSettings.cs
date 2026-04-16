using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.GameStringTextSettings;

public class GameStringTextFormatSettings : GameStringTextSettings
{
    [CommandArgument(0, "<file-path>")]
    [Description("The path of the data or gamestring file")]
    public FileInfo FilePath { get; init; } = null!;

    [CommandArgument(1, "<type>")]
    [Description("The target format for the gamestrings")]
    public GameStringTextType GameStringTextType { get; init; }

    [CommandOption("--hlt-constant-mode <MODE>")]
    [Description("The mode for removing 'hlt-name' attributes from constant tags")]
    [DefaultValue(GameStringTextHltRemoveMode.None)]
    public GameStringTextHltRemoveMode HltConstantRemoveMode { get; init; }

    [CommandOption("--hlt-style-mode <MODE>")]
    [Description("The mode for removing 'hlt-name' attributes from style tags")]
    [DefaultValue(GameStringTextHltRemoveMode.None)]
    public GameStringTextHltRemoveMode HltStyleRemoveMode { get; init; }

    [CommandOption("-o|--output-path <PATH>")]
    [Description("The path of the output directory where the new file will be created (defaults to the file directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    [CommandOption("--overwrite")]
    [Description("Allow the created file to override an existing file")]
    [DefaultValue(false)]
    public bool Overwrite { get; init; }

    [CommandOption("--no-indent")]
    [Description("Disable indentation in the output JSON files")]
    [DefaultValue(false)]
    public bool DisableJsonIndent { get; init; }

    public override ValidationResult Validate()
    {
        if (!FilePath.Exists)
            return ValidationResult.Error("The provided <file-path> does not exist");

        if ((int)GameStringTextType > 6)
            return ValidationResult.Error("<type> must be a value less than 7");

        if ((int)HltConstantRemoveMode > 2)
            return ValidationResult.Error("--hlt-constant-mode must be a value less than 3");

        if ((int)HltStyleRemoveMode > 2)
            return ValidationResult.Error("--hlt-style-mode must be a value less than 3");

        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        return ValidationResult.Success();
    }
}
