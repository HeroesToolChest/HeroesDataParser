namespace HeroesDataParser.Infrastructure;

public class HeroesXmlLoaderService : IHeroesXmlLoaderService
{
    public HeroesXmlLoaderService(HeroesXmlLoader heroesXmlLoader)
    {
        HeroesXmlLoader = heroesXmlLoader;
    }

    public HeroesXmlLoader HeroesXmlLoader { get; }
}
