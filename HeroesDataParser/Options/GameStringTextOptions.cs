namespace HeroesDataParser.Options;

public class GameStringTextOptions
{
    public GameStringTextType Type { get; set; } = GameStringTextType.RawText;

    public bool ReplaceFontStyles { get; set; }

    // only enable if ReplaceFontStyles is true
    public PreserveFontOptions PreserveFont { get; set; } = new();
}
