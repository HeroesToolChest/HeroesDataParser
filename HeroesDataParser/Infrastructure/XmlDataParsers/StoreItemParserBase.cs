namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public abstract class StoreItemParserBase<T> : DataParser<T>
    where T : ElementObject, IElementObject, IStoreItem
{
    // https://en.wikipedia.org/wiki/Heroes_of_the_Storm
    // https://web.archive.org/web/20140525185554/http://www.heroesofthestorm.com/en-us/news/13290651/the-heroes-of-the-storm-technical-alpha-is-now-live
    private readonly DateOnly _technicalAlphaReleaseDate = new(2014, 3, 13);

    public StoreItemParserBase(ILogger logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public abstract override string DataObjectType { get; }

    protected override void SetProperties(T collectionObject, StormElement stormElement)
    {
        SetCommonProperties(collectionObject, stormElement);
        SetAdditionalProperties(collectionObject, stormElement);
        SetValidatedProperties(collectionObject);
    }

    protected virtual void SetCommonProperties(T collectionObject, StormElement stormElement)
    {
        SetNameProperty(collectionObject, stormElement);
        SetHyperlinkIdProperty(collectionObject, stormElement);
        SetFranchiseProperty(collectionObject, stormElement);
        SetRarityProperty(collectionObject, stormElement);
        SetEventNameProperty(collectionObject, stormElement);
        SetDescriptionProperty(collectionObject, stormElement);

        if (stormElement.DataValues.TryGetElementDataAt("sortname", out StormElementData? sortNameData))
            collectionObject.SortName = GameStringTextService.GetGameStringTextFromId(sortNameData.Value.GetString());

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

        if (stormElement.DataValues.TryGetElementDataAt("AdditionalSearchText", out StormElementData? additionalSearchTextData))
        {
            if (collectionObject.SearchText is not null && !string.IsNullOrWhiteSpace(collectionObject.SearchText.RawText))
            {
                string currentSearchText = collectionObject.SearchText.RawText;

                if (currentSearchText[^1] != ' ')
                    currentSearchText += ' ';

                currentSearchText += GameStringTextService.GetStormGameString(additionalSearchTextData.Value.GetString());

                collectionObject.SearchText = GameStringTextService.GetGameStringTextFromGameString(currentSearchText);
            }
            else
            {
                collectionObject.SearchText = GameStringTextService.GetGameStringTextFromId(additionalSearchTextData.Value.GetString());
            }
        }
    }

    protected abstract void SetAdditionalProperties(T collectionObject, StormElement stormElement);

    protected virtual void SetValidatedProperties(T collectionObject)
    {
        if (string.IsNullOrEmpty(collectionObject.HyperlinkId))
            collectionObject.HyperlinkId = collectionObject.Id;

        if (collectionObject.ReleaseDate < _technicalAlphaReleaseDate)
            collectionObject.ReleaseDate = _technicalAlphaReleaseDate;
    }

    private static void SetFranchiseProperty(IFranchise franchiseObject, StormElement stormElement)
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
    }
}
