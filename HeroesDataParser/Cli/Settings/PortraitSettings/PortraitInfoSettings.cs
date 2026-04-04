using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.PortraitSettings;

public class PortraitInfoSettings : PortraitSettings
{
    [CommandArgument(0, "<rewardportrait-file-path>")]
    [Description("The path of the rewardportrait data json file")]
    public FileInfo FilePath { get; init; } = null!;

    [CommandOption("-t|--texture-sheets")]
    [Description("Display all the reward portrait texture sheet file names")]
    public bool TextureSheets { get; init; }

    [CommandOption("-s|--icon-slot <SLOT>")]
    [Description("Display all the given icon slot names along with the texture sheet image file name")]
    public int? IconSlot { get; init; }

    [CommandOption("-i|--texture-sheet-image <IMAGE>")]
    [Description("Display all the reward portrait names along with their icon slot associated with the specified texture sheet image file name")]
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
