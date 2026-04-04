using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.PortraitSettings;

public class PortraitExtractAutoSettings : PortraitSettings
{
    [CommandArgument(0, "<rewardportrait-file-path>")]
    [Description("The path of the rewardportrait data json file")]
    public FileInfo FilePath { get; init; } = null!;

    [CommandOption("-c|--battlenet-cache")]
    [Description("The path of the battle.net cache directory")]
    public DirectoryInfo? CacheDirectoryPath { get; init; }

    [CommandOption("-x|--xml-config <FILEPATH>")]
    [Description("Set the xml file used for the auto extracting")]
    public FileInfo? XmlConfigFilePath { get; init; }

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

        if (CacheDirectoryPath is not null)
        {
            if (!CacheDirectoryPath.Exists)
                return ValidationResult.Error("The provided --battlenet-cache directory does not exist");

            if (File.Exists(CacheDirectoryPath.FullName))
                return ValidationResult.Error("The provided --battlenet-cache is an existing file and not a directory");
        }

        if (XmlConfigFilePath is not null && !XmlConfigFilePath.Exists)
            return ValidationResult.Error("The provided --xml-config file does not exist");

        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        return ValidationResult.Success();
    }
}
