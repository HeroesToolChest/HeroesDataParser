namespace HeroesDataParser.Options.GameStringTextFileOptions;

public class GameStringTextFormatOptions
{
    public string FilePath { get; set; } = string.Empty;

    public GameStringTextType GameStringTextType { get; set; }

    public GameStringTextHltRemoveMode GameStringTextHltConstantRemoveMode { get; set; }

    public GameStringTextHltRemoveMode GameStringTextHltStyleRemoveMode { get; set; }

    public string OutputDirectory { get; set; } = ".";

    public bool AllowOverwrite { get; set; }

    public string OutputFilePath => Path.Combine(OutputDirectory, Path.GetFileName(FilePath));

    // is a new file being created (as opposed to updating the existing file)
    public bool IsNewFile { get; set; }

    public bool JsonIndent { get; set; }
}
