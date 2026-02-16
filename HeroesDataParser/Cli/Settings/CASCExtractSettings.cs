using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings;

public class CASCExtractSettings : CommandSettings
{
    [CommandArgument(0, "<storage-type>")]
    [Description("The type of storage to load from ('game' or 'online')")]
    public StorageType StorageType { get; init; }

    [CommandOption("-p|--storage-path <PATH>")]
    [Description("The path of 'Heroes of the Storm' directory")]
    public DirectoryInfo? StorageDirectory { get; init; }

    [CommandOption("--download-ptr")]
    [Description("Download from the ptr server instead of live ('online' storage-type only)")]
    public bool IsPtr { get; init; }

    [CommandOption("-d|--directory <PATH>")]
    [Description("The directory and it's subdirectories to be extracted, path must start with 'mods' (can be specified multiple times)")]
    [DefaultValue(new[] { "mods" })]
    public string[] Directories { get; init; } = [];

    [CommandOption("-f|--filter <Ext>")]
    [Description("Filter files by extension (can be specified multiple times)")]
    [DefaultValue(new[] { "*" })]
    public string[] Filters { get; init; } = [];

    [CommandOption("-t|--threads <NUMBER>")]
    [Description("The number of threads to use for file extraction (defaults to max number of processors)")]
    [DefaultValue(-1)]
    public int Threads { get; init; }

    [CommandOption("-o|--output-path <PATH>")]
    [Description("The path of the output directory (defaults to current directory)")]
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

        foreach (string directory in Directories)
        {
            if (Path.HasExtension(directory))
                return ValidationResult.Error($"--directory {directory} must be a directory path, not a file");

            ReadOnlySpan<char> rootSegment = directory.AsSpan()
                .TrimStart(Path.DirectorySeparatorChar)
                .TrimStart(Path.AltDirectorySeparatorChar);

            int separatorIndex = rootSegment.IndexOfAny(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (separatorIndex >= 0)
                rootSegment = rootSegment[..separatorIndex];

            if (!rootSegment.Equals("mods", StringComparison.OrdinalIgnoreCase))
                return ValidationResult.Error($"--directory {directory} first directory must be 'mods'");
        }

        if (Threads == 0 || Threads < -1)
            return ValidationResult.Error("--threads must be -1 or a positive integer");

        if (OutputDirectory is not null && !OutputDirectory.Exists)
            return ValidationResult.Error("The provided --output-path does not exist");

        return ValidationResult.Success();
    }
}
