using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.PortraitSettings;

public class PortraitExtractSettings : PortraitSettings
{
    [CommandArgument(0, "<rewardportrait-file-path>")]
    [Description("The path of the rewardportrait data json file")]
    public FileInfo FilePath { get; init; } = null!;

    [CommandOption("-i|--texture-sheet-image <IMAGE>")]
    [Description("The texture sheet image name from the reward portrait data json file")]
    public string? TextureSheetImage { get; init; }

    [CommandOption("-c|--cache-texture-sheet-image <FILEPATH>")]
    [Description("The path of the texture sheet image file to extract the portrait images from")]
    public FileInfo? CacheTextureSheetImage { get; init; }

    [CommandOption("--delete-texture-sheet")]
    [Description("Delete the texture sheet after extracting the portraits")]
    public bool DeleteTextureSheet { get; init; }

    [CommandOption("-o|--output-path <PATH>")]
    [Description("The path of the output directory to save the extracted portraits (defaults to the current directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    public override ValidationResult Validate()
    {
        if (!FilePath.Exists)
            return ValidationResult.Error("The provided <rewardportrait-file-path> does not exist");

        if (CacheTextureSheetImage is not null && !CacheTextureSheetImage.Exists)
            return ValidationResult.Error("The provided --cache-texture-sheet-image file does not exist");

        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        return ValidationResult.Success();
    }
}
