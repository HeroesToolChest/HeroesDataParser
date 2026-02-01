using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings;

public class RootSettings : CommandSettings
{
    [CommandArgument(0, "<storage-type>")]
    [Description("The type of storage to load from ('mods', 'game', or 'online')")]
    public StorageType StorageType { get; init; }

    [CommandOption("-p|--storage-path <PATH>")]
    [Description("The path of 'Heroes of the Storm' directory or an already extracted 'mods' directory")]
    public DirectoryInfo? StorageDirectory { get; init; }

    [CommandOption("--ptr")]
    [Description("Only for 'online' type, download from the ptr server")]
    public bool IsPtr { get; init; }

    [CommandOption("-e|--extractor <EXTRACTOR>")]
    [Description("Extractors to enable, add ':images' to enabled image extraction (can be specified multiple times)")]
    [DefaultValue("Hero")]
    public string[] Extractors { get; init; } = [];

    [CommandOption("-g|--gamestring-text")]
    [Description("The format of the gamestrings texts")]
    [DefaultValue(GameStringTextType.RawText)]
    public GameStringTextType GameStringTextType { get; init; }

    [CommandOption("-o|--output-path <PATH>")]
    [Description("The path of the output directory (defaults to current directory)")]
    [DefaultValue(typeof(DirectoryInfo), ".")]
    public DirectoryInfo OutputDirectory { get; init; } = null!;

    [CommandOption("--heroes-version")]
    [Description("Manually set the 'Heroes of the Storm' version in the format of major.minor.revision.build_<ptr> (e.g. 1.2.3.4 or 1.2.3.4_ptr)")]
    public string? HeroesVersion { get; init; }

    public override ValidationResult Validate()
    {
        if ((StorageType == StorageType.Game || StorageType == StorageType.Mods) && StorageDirectory is null)
            return ValidationResult.Error("--storage-path is required when storage-type is 'game' or 'mods'");

        if (StorageType == StorageType.Online && StorageDirectory is not null)
            return ValidationResult.Error("--storage-path must not be specified when storage-type is 'online'");

        if (IsPtr && StorageType != StorageType.Online)
            return ValidationResult.Error("--ptr is only valid when storage-type is 'online'");

        if (!string.IsNullOrWhiteSpace(HeroesVersion) && !HeroesDataVersion.TryParse(HeroesVersion, out HeroesDataVersion? heroesDataVersion))
            return ValidationResult.Error("--heroes-version is not in the correct format");

        foreach (string extractor in Extractors)
        {
            ValidationResult result = ParseExtractor(extractor);
            if (!result.Successful)
                return result;
        }

        return ValidationResult.Success();
    }

    private ValidationResult ParseExtractor(ReadOnlySpan<char> extractor)
    {
        Span<Range> keyPair = stackalloc Range[2];

        extractor.Split(keyPair, ':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        ReadOnlySpan<char> key = extractor[keyPair[0]];
        ReadOnlySpan<char> value = extractor[keyPair[1]]; // images

        if (!Enum.TryParse<ExtractDataOptions>(key, true, out _))
            return ValidationResult.Error($"Invalid extractor: {key}");

        if (!value.IsEmpty && !value.Equals("i", StringComparison.OrdinalIgnoreCase) && !value.Equals("images", StringComparison.OrdinalIgnoreCase))
            return ValidationResult.Error($"Invalid extractor option: {value}");

        return ValidationResult.Success();
    }
}
