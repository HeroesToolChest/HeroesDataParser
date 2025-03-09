using Heroes.Element.Models;
using Serilog.Context;

namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public abstract class CollectionParserBase<T> : DataParser<T>
    where T : ElementObject, IElementObject, IHeroesCollectionObject
{
    private readonly ILogger _logger;

    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly HeroesData _heroesData;

    // https://en.wikipedia.org/wiki/Heroes_of_the_Storm
    // https://web.archive.org/web/20140525185554/http://www.heroesofthestorm.com/en-us/news/13290651/the-heroes-of-the-storm-technical-alpha-is-now-live
    private readonly DateOnly _technicalAlphaReleaseDate = new(2014, 3, 13);

    public CollectionParserBase(ILogger logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesXmlLoaderService = heroesXmlLoaderService;
        _heroesData = _heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
    }

    public abstract override string DataObjectType { get; }

    //public override T? Parse(string id)
    //{
    //    _logger.LogTrace("Parsing id {Id}", id);

    //    StormElement? stormElement = _heroesData.GetCompleteStormElement(DataObjectType, id);

    //    if (stormElement is null)
    //    {
    //        _logger.LogWarning("Could not find data for id {Id}", id);
    //        return null;
    //    }

    //    T? collectionObject = (T?)Activator.CreateInstance(typeof(T), id);

    //    if (collectionObject is null)
    //    {
    //        _logger.LogError("Failed to create instance of type {Type} for id {Id}", typeof(T), id);
    //        return null;
    //    }

    //    using (LogContext.PushProperty("XmlPaths", stormElement.OriginalXElements.Select(x => x.StormPath)))
    //    {
    //        SetCommonProperties(collectionObject, stormElement);
    //        SetAdditionalProperties(collectionObject, stormElement);
    //        SetValidatedProperties(collectionObject);

    //        _logger.LogTrace("Parsing id {Id} complete", id);

    //        return collectionObject;
    //    }
    //}

    protected override void SetProperties(T elementObject, StormElement stormElement)
    {
        SetCommonProperties(elementObject, stormElement);
        SetAdditionalProperties(elementObject, stormElement);
        SetValidatedProperties(elementObject);
    }

    protected void SetCommonProperties(T collectionObject, StormElement stormElement)
    {
        SetNameProperty(collectionObject, stormElement);

        if (stormElement.DataValues.TryGetElementDataAt("sortname", out StormElementData? sortNameData))
            collectionObject.SortName = GetTooltipDescriptionFromId(sortNameData.Value.GetString());

        SetHyperlinkIdProperty(collectionObject, stormElement);

        if (stormElement.DataValues.TryGetElementDataAt("attributeid", out StormElementData? attributeIdData))
            collectionObject.AttributeId = attributeIdData.Value.GetString();

        SetRarityProperty(collectionObject, stormElement);

        if (stormElement.DataValues.TryGetElementDataAt("releasedate", out StormElementData? releasetDateData))
        {
            int year = 2014;
            int month = 1;
            int day = 1;

            if (releasetDateData.TryGetElementDataAt("year", out StormElementData? yearData) && yearData.Value.TryGetInt32(out int yearValue))
                year = yearValue;

            if (releasetDateData.TryGetElementDataAt("month", out StormElementData? monthData) && monthData.Value.TryGetInt32(out int monthValue))
                month = monthValue;

            if (releasetDateData.TryGetElementDataAt("day", out StormElementData? dayData) && dayData.Value.TryGetInt32(out int dayValue))
                day = dayValue;

            collectionObject.ReleaseDate = new DateOnly(year, month, day);
        }

        if (stormElement.DataValues.TryGetElementDataAt("collectioncategory", out StormElementData? collectionCategoryData))
            collectionObject.Category = collectionCategoryData.Value.GetString();

        SetEventNameProperty(collectionObject, stormElement);
        SetDescriptionProperty(collectionObject, stormElement);
    }

    protected abstract void SetAdditionalProperties(T collectionObject, StormElement stormElement);

    protected virtual void SetValidatedProperties(T collectionObject)
    {
        if (string.IsNullOrEmpty(collectionObject.HyperlinkId))
            collectionObject.HyperlinkId = collectionObject.Id;

        if (collectionObject.ReleaseDate < _technicalAlphaReleaseDate)
            collectionObject.ReleaseDate = _technicalAlphaReleaseDate;
    }

    protected void SetFranchiseProperty(IFranchise franchiseObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("universe", out StormElementData? universeData))
        {
            string universe = universeData.Value.GetString();

            if (universe.Equals("retro", StringComparison.OrdinalIgnoreCase))
                franchiseObject.Franchise = Franchise.Classic;
            else if (universe.Equals("starcraft", StringComparison.OrdinalIgnoreCase))
                franchiseObject.Franchise = Franchise.Starcraft;
            else if (universe.Equals("warcraft", StringComparison.OrdinalIgnoreCase))
                franchiseObject.Franchise = Franchise.Warcraft;
            else if (universe.Equals("diablo", StringComparison.OrdinalIgnoreCase))
                franchiseObject.Franchise = Franchise.Diablo;
            else if (universe.Equals("overwatch", StringComparison.OrdinalIgnoreCase))
                franchiseObject.Franchise = Franchise.Overwatch;
            else if (universe.Equals("heroes", StringComparison.OrdinalIgnoreCase) || universe.Equals("nexus", StringComparison.OrdinalIgnoreCase))
                franchiseObject.Franchise = Franchise.Nexus;
            else
                franchiseObject.Franchise = Franchise.Unknown;
        }

        if (stormElement.DataValues.TryGetElementDataAt("UniverseIcon", out StormElementData? universeIconData))
        {
            string? iconImageName = Path.GetFileName(universeIconData.Value.GetString());

            if (iconImageName.Equals("UI_GLUES_STORE_GAMEICON_SC2.DDS", StringComparison.OrdinalIgnoreCase))
                franchiseObject.Franchise = Franchise.Starcraft;
            else if (iconImageName.Equals("UI_GLUES_STORE_GAMEICON_WOW.DDS", StringComparison.OrdinalIgnoreCase))
                franchiseObject.Franchise = Franchise.Warcraft;
            else if (iconImageName.Equals("UI_GLUES_STORE_GAMEICON_D3.DDS", StringComparison.OrdinalIgnoreCase))
                franchiseObject.Franchise = Franchise.Diablo;
            else if (iconImageName.Equals("UI_GLUES_STORE_GAMEICON_OW.DDS", StringComparison.OrdinalIgnoreCase))
                franchiseObject.Franchise = Franchise.Overwatch;
            else if (iconImageName.Equals("UI_GLUES_STORE_GAMEICON_RETRO.DDS", StringComparison.OrdinalIgnoreCase))
                franchiseObject.Franchise = Franchise.Classic;
            else if (iconImageName.Equals("UI_GLUES_STORE_GAMEICON_NEXUS.DDS", StringComparison.OrdinalIgnoreCase))
                franchiseObject.Franchise = Franchise.Nexus;
        }
    }
}
