namespace HeroesDataParser.Options;

[Flags]
public enum MapSpecificWriterJsonOutputType
{
    None = 0,
    Normal,
    Diff,
    All = Normal | Diff,
}
