namespace HeroesDataParser.Infrastructure;

public class HeroesDataService : IHeroesDataService
{
    public HeroesDataService(HeroesData heroesData)
    {
        HeroesData = heroesData;
    }

    public HeroesData HeroesData { get; }
}
