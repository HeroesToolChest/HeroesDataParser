namespace HeroesDataParser.Options;

public class HiddenOptions
{
    public bool AllowHeroHiddenAbilities { get; set; }

    public bool AllowHeroSpecialAbilities { get; set; }

    public bool Abilities { get; set; }

    public bool AbilityTalents { get; set; }

    public HeroImagesOptions HeroImages { get; set; } = new();

    public UnitImagesOptions UnitImages { get; set; } = new();

    public MapImagesOptions MapImages { get; set; } = new();
}
