namespace HeroesDataParser.Options.PortraitOptions;

public class PortraitExtractAutoOptions : PortraitCacheOptions
{
    public string RewardPortraitDataFilePath { get; set; } = string.Empty;

    public string XmlConfigFilePath { get; set; } = string.Empty;

    public bool DeleteTextureSheet { get; set; }
}
