namespace HeroesDataParser.Options;

public class HiddenOptions
{
    public bool AllowHeroHiddenAbilities { get; set; }

    public bool AllowHeroSpecialAbilities { get; set; }

    public HeroImages HeroImages { get; set; } = new();
}
