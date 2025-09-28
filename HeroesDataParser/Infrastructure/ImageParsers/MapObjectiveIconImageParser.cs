namespace HeroesDataParser.Infrastructure.ImageParsers;

public class MapObjectiveIconImageParser : ImageParserBase<Map>
{
    public MapObjectiveIconImageParser(ILogger<MapObjectiveIconImageParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.MapObjectives;

    protected override string SubDirectory => "mapobjectives";

    protected override void SetImages(Map element)
    {
        foreach (MapObjective mapObjective in element.MapObjectives)
        {
            foreach (MapObjectiveIcon mapObjectiveIcon in mapObjective.Icons)
            {
                if (!string.IsNullOrWhiteSpace(mapObjectiveIcon.Image) && !string.IsNullOrWhiteSpace(mapObjectiveIcon.ImagePath?.FilePath))
                    AddImagePath(mapObjectiveIcon.Image, new ImageRelativePath(element, mapObjectiveIcon.ImagePath));
            }
        }
    }
}
