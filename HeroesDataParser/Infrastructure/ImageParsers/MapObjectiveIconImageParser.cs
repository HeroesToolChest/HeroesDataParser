namespace HeroesDataParser.Infrastructure.ImageParsers;

public class MapObjectiveIconImageParser : ImageParserBase<Map>
{
    public MapObjectiveIconImageParser(ILogger<MapObjectiveIconImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.MapObjectives;

    protected override string Subdirectory => "mapobjectives";

    protected override void SetImages(Map element)
    {
        foreach (MapObjective mapObjective in element.MapObjectives)
        {
            foreach (MapObjectiveIcon mapObjectiveIcon in mapObjective.Icons)
            {
                if (!string.IsNullOrWhiteSpace(mapObjectiveIcon.Image) && !string.IsNullOrWhiteSpace(mapObjectiveIcon?.ImagePath?.FilePath))
                {
                    AddToFiles(mapObjectiveIcon.Image, element.Id, async (directoryPath) =>
                    {
                        await ProcessStaticImage(mapObjectiveIcon.Image, mapObjectiveIcon.ImagePath, directoryPath);
                    });
                }
            }
        }
    }
}
