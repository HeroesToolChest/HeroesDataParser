namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public abstract class ParserBase<T> : IDataParser<T>
    where T : ElementObject, IElementObject
{
    public const string ImageFileExtension = "png";

    private readonly ILogger _logger;
    private readonly HeroesData _heroesData;

    public ParserBase(ILogger logger, IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
    }

    public abstract string DataObjectType { get; }

    public virtual T? Parse(string id)
    {
        _logger.LogTrace("Parsing id {Id}", id);

        StormElement? stormElement = _heroesData.GetCompleteStormElement(DataObjectType, id);

        if (stormElement is null)
        {
            _logger.LogWarning("Could not find data for id {Id}", id);
            return null;
        }

        T? elementObject = (T?)Activator.CreateInstance(typeof(T), id);

        if (elementObject is null)
        {
            _logger.LogError("Failed to create instance of type {Type} for id {Id}", typeof(T), id);
            return null;
        }

        _logger.LogTrace("Parsing id {Id} complete", id);

        return elementObject;
    }

    protected TooltipDescription? GetTooltipDescription(string id)
    {
        StormGameString? stormGameString = _heroesData.GetStormGameString(id);

        if (stormGameString is null)
            return null;
        else
            return _heroesData.ParseGameString(stormGameString);
    }
}
