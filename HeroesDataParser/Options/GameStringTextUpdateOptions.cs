namespace HeroesDataParser.Options;

public class GameStringTextUpdateOptions
{
    public string FilePath { get; set; } = string.Empty;

    public GameStringTextType GameStringTextType { get; set; }

    public GameStringTextHltRemoveMode GameStringTextHltConstantRemoveMode { get; set; }

    public GameStringTextHltRemoveMode GameStringTextHltStyleRemoveMode { get; set; }

    public string OutputDirectory { get; set; } = ".";

    public bool AllowOverwrite { get; set; }

    public string OutputFilePath => Path.Combine(OutputDirectory, Path.GetFileName(FilePath));

    //public bool ReplaceFontStyles => ;

    public bool PreserveFontStylesVars => !(GameStringTextHltStyleRemoveMode != GameStringTextHltRemoveMode.None);

    public bool PreserveFontStyleConstantVars => !(GameStringTextHltConstantRemoveMode != GameStringTextHltRemoveMode.None);
}
