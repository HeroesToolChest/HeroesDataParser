using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.PortraitSettings;

public class PortraitInfoSettings : PortraitSettings
{
    [CommandArgument(0, "<rewardportrait-file-path>")]
    [Description("Path to the rewardportrait data JSON file")]
    public FileInfo FilePath { get; init; } = null!;

    [CommandOption("-t|--texture-sheets")]
    [Description("Display all reward portrait texture sheet file names")]
    public bool TextureSheets { get; init; }

    [CommandOption("-s|--icon-slot <SLOT>")]
    [Description("Display all icon slot names along with the texture sheet image file name for the given slot")]
    public int? IconSlot { get; init; }

    [CommandOption("-i|--texture-sheet-image <IMAGE>")]
    [Description("Display all reward portrait names and their icon slots for the specified texture sheet image")]
    public string? TextureSheetImage { get; init; }

    public override ValidationResult Validate()
    {
        if (!FilePath.Exists)
            return ValidationResult.Error("The provided <rewardportrait-file-path> does not exist");

        if (IconSlot.HasValue && IconSlot.Value < 0)
            return ValidationResult.Error("The provided --icon-slot value must be a non-negative integer");

        return ValidationResult.Success();
    }
}
