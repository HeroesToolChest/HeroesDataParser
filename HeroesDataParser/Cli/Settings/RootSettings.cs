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

    [CommandOption("--download-ptr")]
    [Description("Download from the ptr server instead of live ('online' storage-type only)")]
    public bool IsPtr { get; init; }

    [CommandOption("-e|--extractor <EXTRACTOR>")]
    [Description("Extractors to enable, add ':images' to enabled image extraction (can be specified multiple times)")]
    [DefaultValue("Hero")]
    public string[] Extractors { get; init; } = [];

    [CommandOption("-l|--localization <LOCALE>")]
    [DefaultValue("enUS")]
    [Description("Localizations for gamestrings to process (can be specified multiple times)")]
    public string[] StormLocales { get; init; } = [];

    [CommandOption("-g|--gamestring-text <FORMAT>")]
    [Description("The format of the gamestrings")]
    [DefaultValue(GameStringTextType.RawText)]
    public GameStringTextType GameStringTextType { get; init; }

    [CommandOption("--gs-replace-constant-vars")]
    [Description("Replace constant variables in gamestrings with the color text hex values")]
    public bool GameStringReplaceConstantVars { get; init; }

    [CommandOption("--gs-replace-style-vars")]
    [Description("Replace font variables in gamestrings with the color text hex values")]
    public bool GameStringReplaceStyleVars { get; init; }

    [CommandOption("--gs-preserve-constant-vars")]
    [Description("Preserve constant variables in gamestrings")]
    public bool GameStringPreserveConstantVars { get; init; }

    [CommandOption("--gs-preserve-style-vars")]
    [Description("Preserve style variables in gamestrings")]
    public bool GameStringPreserveStyleVars { get; init; }

    [CommandOption("--localized-text <OPTION>")]
    [Description("Specifies how to handle gamestring properties during JSON serialization")]
    [DefaultValue(LocalizedTextOption.None)]
    public LocalizedTextOption LocalizedTextOption { get; init; }

    [CommandOption("--map-specific-json-output <TYPE>")]
    [Description("Specifies how to handle the map specific JSON file creation")]
    [DefaultValue(MapSpecificWriterJsonOutputType.Patch)]
    public MapSpecificWriterJsonOutputType MapSpecificWriterJsonOutputType { get; init; }

    [CommandOption("--map-specific-empty-patch")]
    [Description("Allows map specific patch files without any item changes to be created")]
    public bool AllowEmptyMapSpecificPatchFiles { get; init; }

    [CommandOption("--map-specific-empty-directories")]
    [Description("Allows map specific empty directories to be created")]
    public bool AllowEmptyMapSpecificDirectories { get; init; }

    [CommandOption("--custom-configs")]
    [Description("Display the loaded custom config files")]
    public bool ShowLoadedCustomConfigFiles { get; init; }

    [CommandOption("-t|--threads <NUMBER>")]
    [Description("The number of threads to use for data parsing and image writing (defaults to max number of processors)")]
    [DefaultValue(-1)]
    public int Threads { get; init; }

    [CommandOption("-o|--output-path <PATH>")]
    [Description("The path of the output directory (defaults to current directory)")]
    public DirectoryInfo? OutputDirectory { get; init; }

    [CommandOption("--heroes-version")]
    [Description("Manually set the 'Heroes of the Storm' version in the format of major.minor.revision.build<_ptr> (e.g. 1.2.3.4 or 1.2.3.4_ptr)")]
    public string? HeroesVersion { get; init; }

    public override ValidationResult Validate()
    {
        if (StorageType == StorageType.Game || StorageType == StorageType.Mods)
        {
            if (StorageDirectory is null)
                return ValidationResult.Error("--storage-path is required when storage-type is 'game' or 'mods'");

            if (!StorageDirectory.Exists)
                return ValidationResult.Error("The provided --storage-path does not exist");
        }

        if (StorageType == StorageType.Online && StorageDirectory is not null)
            return ValidationResult.Error("--storage-path must not be specified when storage-type is 'online'");

        if (IsPtr && StorageType != StorageType.Online)
            return ValidationResult.Error("--ptr is only valid when storage-type is 'online'");

        if (!string.IsNullOrWhiteSpace(HeroesVersion) && !HeroesDataVersion.TryParse(HeroesVersion, out _))
            return ValidationResult.Error("--heroes-version is not in the correct format");

        foreach (string extractor in Extractors)
        {
            ValidationResult result = ParseExtractor(extractor);
            if (!result.Successful)
                return result;
        }

        foreach (string locale in StormLocales)
        {
            if (locale.Equals("all", StringComparison.OrdinalIgnoreCase))
                continue;

            if (!Enum.TryParse(locale, true, out StormLocale _))
                return ValidationResult.Error($"--localization has an invalid locale: {locale}");
        }

        if (GameStringPreserveConstantVars && !GameStringReplaceConstantVars)
            return ValidationResult.Error("--gs-preserve-constant-vars cannot be used without --gs-replace-constant-vars");

        if (GameStringPreserveStyleVars && !GameStringReplaceStyleVars)
            return ValidationResult.Error("--gs-preserve-style-vars cannot be used without --gs-replace-style-vars");

        if ((int)GameStringTextType > 6)
            return ValidationResult.Error("--gamestring-text must be a value less than 7");

        if ((int)LocalizedTextOption > 2)
            return ValidationResult.Error("--localized-text must be a value less than 3");

        if ((int)MapSpecificWriterJsonOutputType > 3)
            return ValidationResult.Error("--map-json-output must be a value less than 4");

        if (Threads == 0 || Threads < -1)
            return ValidationResult.Error("--threads must be -1 or a positive integer");

        if (OutputDirectory is not null && File.Exists(OutputDirectory.FullName))
            return ValidationResult.Error("The provided --output-path is an existing file and not a directory");

        return ValidationResult.Success();
    }

    private ValidationResult ParseExtractor(ReadOnlySpan<char> extractor)
    {
        Span<Range> keyPair = stackalloc Range[2];

        extractor.Split(keyPair, ':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        ReadOnlySpan<char> key = extractor[keyPair[0]];
        ReadOnlySpan<char> value = extractor[keyPair[1]]; // images

        if (!Enum.TryParse<ExtractDataOptions>(key, true, out _))
            return ValidationResult.Error($"--extractor has an invalid extractor: {key}");

        if (!value.IsEmpty && !value.Equals("i", StringComparison.OrdinalIgnoreCase) && !value.Equals("images", StringComparison.OrdinalIgnoreCase))
            return ValidationResult.Error($"--extractor has an invalid extractor option: {value}");

        return ValidationResult.Success();
    }
}
