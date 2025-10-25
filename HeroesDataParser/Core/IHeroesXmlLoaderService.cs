namespace HeroesDataParser.Core;

public interface IHeroesXmlLoaderService
{
    HeroesXmlLoader HeroesXmlLoader { get; }

    Task Load(PreloadData preloadData);
}
