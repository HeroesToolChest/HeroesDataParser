namespace HeroesDataParser.Options;

public class GameStringTextOptions
{
    public GameStringTextType Type { get; set; } = GameStringTextType.RawText;

    public bool ReplaceFontConstantVars { get; set; }

    // should be false if ReplaceFontConstantsVars is false
    public bool PreserveFontStyleConstantVars { get; set; }

    public bool ReplaceFontStylesVars { get; set; }

    // should be false if ReplaceFontStylesVars is false
    public bool PreserveFontStyleVars { get; set; }
}
