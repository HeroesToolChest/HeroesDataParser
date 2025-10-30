namespace HeroesDataParser.Infrastructure.ImageParsers;

public class MapObjectiveIconImageParser : ImageParserBase<Map>
{
    public MapObjectiveIconImageParser(ILogger<MapObjectiveIconImageParser> logger)
        : base(logger)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.MapObjectives;

    protected override string SubDirectory => "mapobjectives";

    protected override void SetImages(Map element, HashSet<ImageWriterPath> imagePaths)
    {
        foreach (MapObjective mapObjective in element.MapObjectives)
        {
            foreach (MapObjectiveIcon mapObjectiveIcon in mapObjective.Icons)
            {
                if (!string.IsNullOrWhiteSpace(mapObjectiveIcon.Image) && !string.IsNullOrWhiteSpace(mapObjectiveIcon.ImagePath?.FilePath))
                    TryAddToFiles(imagePaths, mapObjectiveIcon.Image, mapObjectiveIcon.ImagePath, element);
            }
        }
    }
}
