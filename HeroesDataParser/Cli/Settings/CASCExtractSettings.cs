using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings;

public class CASCExtractSettings : CommandSettings
{
    [CommandArgument(0, "<storage-type>")]
    [Description("Storage type to load from (game or online)")]
    public StorageType StorageType { get; init; }

    [CommandOption("-s|--storage-path <PATH>")]
    [Description("Path to the Heroes of the Storm directory")]
    public DirectoryInfo? StorageDirectory { get; init; }

    [CommandOption("--download-ptr")]
    [Description("Download from the PTR server instead of live (online storage-type only)")]
    public bool IsPtr { get; init; }

    [CommandOption("-f|--filter <PATTERN>")]
    [Description("Glob pattern to filter file paths for extraction, e.g. '*.xml' (can be specified multiple times")]
    [DefaultValue(new[] { "*" })]
    public string[] Filters { get; init; } = [];

    [CommandOption("-t|--threads <NUMBER>")]
    [Description("Number of threads for file extraction (defaults to max processors)")]
    [DefaultValue(-1)]
    public int Threads { get; init; }

    [CommandOption("-o|--output-path <PATH>")]
    [Description("Output directory for extracted files (defaults to current directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    public override ValidationResult Validate()
    {
        if (StorageType == StorageType.Mods)
            return ValidationResult.Error("storage-type 'mods' is not supported");

        if (StorageType == StorageType.Game)
        {
            if (StorageDirectory is null)
                return ValidationResult.Error("--storage-path is required when storage-type is 'game'");

            if (!StorageDirectory.Exists)
                return ValidationResult.Error("The provided --storage-path does not exist");
        }

        if (StorageType == StorageType.Online && StorageDirectory is not null)
            return ValidationResult.Error("--storage-path must not be specified when storage-type is 'online'");

        if (IsPtr && StorageType != StorageType.Online)
            return ValidationResult.Error("--ptr is only valid when storage-type is 'online'");

        if (Threads == 0 || Threads < -1)
            return ValidationResult.Error("--threads must be -1 or a positive integer");

        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        return ValidationResult.Success();
    }
}
