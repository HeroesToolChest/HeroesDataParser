namespace HeroesDataParser.Infrastructure;

public class PreLoaderService : IPreLoaderService
{
    private readonly IConfigurationLoaderService _configurationLoaderService;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public PreLoaderService(
        IConfigurationLoaderService configurationLoaderService,
        IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _configurationLoaderService = configurationLoaderService;
        _heroesXmlLoaderService = heroesXmlLoaderService;
    }

    public async Task Load()
    {
        LoadedConfiguration loadedConfiguration = _configurationLoaderService.LoadConfiguration();

        await _heroesXmlLoaderService.Load(loadedConfiguration);
    }

    public string GetHeroesVersion()
    {
        return _configurationLoaderService.GetHeroesVersion();
    }
}
