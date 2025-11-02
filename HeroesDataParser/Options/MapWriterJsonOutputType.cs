namespace HeroesDataParser.Options;

[Flags]
public enum MapWriterJsonOutputType
{
    None = 0,
    Normal,
    Diff,
    All = Normal | Diff,
}
