namespace HeroesDataParser.Options;

public class HeroesVersionOptions
{
    public int Major { get; set; }

    public int Minor { get; set; }

    public int Revision { get; set; }

    public int Build { get; set; }

    public bool IsPtr { get; set; }

    public bool IsOverridden => Major > -1 || Minor > -1 || Revision > -1 || Build > -1; // no ptr check

    public HeroesDataVersion GetAsHeroesDataVersion() => new(Major, Minor, Revision, Build, IsPtr);
}
