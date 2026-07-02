namespace HeroesDataParser.Options.PortraitOptions;

public class PortraitExtractOptions
{
    public string RewardPortraitDataFilePath { get; set; } = string.Empty;

    public bool DeleteTextureSheet { get; set; }

    public string OutputDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the texture sheet image from the reward portrait data json file.
    /// </summary>
    public string RewardPortraitTextureSheetImage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the texture sheet image file path from the battle.net cache to extract portraits from.
    /// </summary>
    public string CacheTextureSheetImageFilePath { get; set; } = string.Empty;
}
