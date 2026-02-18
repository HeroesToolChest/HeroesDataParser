namespace HeroesDataParser.Options;

[Flags]
public enum MapSpecificWriterJsonOutputType
{
    None = 0,
    Normal,
    Patch,
    All = Normal | Patch,
}
