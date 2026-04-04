using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.PortraitSettings;

public class PortraitBattleNetCacheSettings : PortraitSettings
{
    [CommandOption("-c|--battlenet-cache")]
    [Description("The path of the battle.net cache directory")]
    public DirectoryInfo? CacheDirectoryPath { get; init; }

    [CommandOption("-o|--output-path <PATH>")]
    [Description("The path of the output directory to copy the texture sheets (defaults to the current directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    public override ValidationResult Validate()
    {
        if (CacheDirectoryPath is not null)
        {
            if (!CacheDirectoryPath.Exists)
                return ValidationResult.Error("The provided --battlenet-cache directory does not exist");

            if (File.Exists(CacheDirectoryPath.FullName))
                return ValidationResult.Error("The provided --battlenet-cache is an existing file and not a directory");
        }

        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        return ValidationResult.Success();
    }
}
