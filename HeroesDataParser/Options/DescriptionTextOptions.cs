namespace HeroesDataParser.Options;

public class DescriptionTextOptions
{
    public DescriptionType Type { get; set; } = DescriptionType.RawDescription;

    public bool ReplaceFontStyles { get; set; }

    // only enable if ReplaceFontStyles is true
    public PreserveFontOptions PreserveFont { get; set; } = new();
}
