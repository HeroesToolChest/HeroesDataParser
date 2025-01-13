namespace HeroesDataParser.Core;

public interface IHeroesDataLoaderService
{
    HeroesXmlLoader HeroesXmlLoader { get; }

    Task Load();
}
