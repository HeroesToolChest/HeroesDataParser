namespace HeroesDataParser.Options.LocalizedTextFileOptions;

public class LocalizedTextImportOptions : LocalizedTextOptions
{
    public string DataFilePath { get; set; } = string.Empty;

    public string GameStringsFilePath { get; set; } = string.Empty;

    public string OutputFilePath => Path.Combine(OutputDirectory, Path.GetFileName(DataFilePath));

    // is a new file being created (as opposed to updating the existing file)
    public bool IsNewFile { get; set; } = false;
}
