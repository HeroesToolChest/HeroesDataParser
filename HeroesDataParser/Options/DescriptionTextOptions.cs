namespace HeroesDataParser.Options;

public class DescriptionTextOptions
{
    public DescriptionType Type { get; set; } = DescriptionType.RawDescription;

    public bool ReplaceFontStyles { get; set; }

    public bool PreserveFontStyleVars { get; set; }

    public bool PreserveFontStyleConstantVars { get; set; }
}
