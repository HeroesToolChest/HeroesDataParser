using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.PortraitSettings;

public class PortraitBattleNetCacheSettings : PortraitSettings
{
    [CommandOption("-c|--battlenet-cache <PATH>")]
    [Description("Path to the Battle.net cache directory")]
    public DirectoryInfo? CacheDirectoryPath { get; init; }

    [CommandOption("-o|--output-path <PATH>")]
    [Description("Output directory for copied texture sheets (defaults to current directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    public override ValidationResult Validate()
    {
        if (CacheDirectoryPath is not null)
        {
            if (File.Exists(CacheDirectoryPath.FullName))
                return ValidationResult.Error("The provided --battlenet-cache is an existing file and not a directory");

            if (!CacheDirectoryPath.Exists)
                return ValidationResult.Error("The provided --battlenet-cache directory does not exist");
        }

        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        return ValidationResult.Success();
    }
}
