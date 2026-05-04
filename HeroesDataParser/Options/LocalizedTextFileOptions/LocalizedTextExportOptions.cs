namespace HeroesDataParser.Options.LocalizedTextFileOptions;

public class LocalizedTextExportOptions : LocalizedTextOptions
{
    public string DataFilePath { get; set; } = string.Empty;

    // if one is provided it will be updated (that is added) with the new gamestrings
    public string? GameStringFilePath { get; set; }

    public ExtractType ExtractType { get; set; } = ExtractType.Copy;

    public string OutputDataFilePath => Path.Combine(OutputDirectory, Path.GetFileName(DataFilePath));

    // is a new file being created (as opposed to updating the existing file)
    public bool IsNewDataFile { get; set; }

    // is a new file being created (as opposed to updating the existing file)
    public bool IsNewGameStringFile { get; set; }
}
