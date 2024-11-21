namespace HeroesDataParser;

public class TempApp
{
    private readonly ILogger<TempApp> _logger;
    private readonly IHeroesXmlLoaderService _heroesDataLoaderService;

    public TempApp(ILogger<TempApp> logger, IHeroesXmlLoaderService heroesDataLoaderService)
    {
        _logger = logger;
        _heroesDataLoaderService = heroesDataLoaderService;
    }
}
