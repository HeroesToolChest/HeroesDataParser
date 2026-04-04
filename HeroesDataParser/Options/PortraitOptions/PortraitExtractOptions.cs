namespace HeroesDataParser.Options.PortraitOptions;

public class PortraitExtractOptions
{
    public string RewardPortraitDataFilePath { get; set; } = string.Empty;

    public bool DeleteTextureSheet { get; set; }

    public string OutputDirectory { get; set; } = string.Empty;

    public string RewardPortraitTextureSheetImage { get; set; } = string.Empty;

    public string CacheTextureSheetImageFilePath { get; set; } = string.Empty;
}
