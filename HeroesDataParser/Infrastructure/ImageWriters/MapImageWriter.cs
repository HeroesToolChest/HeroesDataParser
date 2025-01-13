namespace HeroesDataParser.Infrastructure.ImageWriters;

public class MapImageWriter : ImageWriterBase<Map>
{
    private const string _mapDirectory = "maps";
    private const string _replayPreviewDirectory = "replay_previews";
    private const string _loadingScreenDirectory = "loading_screens";
    private const string _mapObjectiveIconDirectory = "map_objective_icons";

    private readonly Dictionary<string, ImageRelativePath> _replayPreviewRelativePathsByFileName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, ImageRelativePath> _loadingScreenRelativePathsByFileName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, ImageRelativePath> _mapObjectiveIconRelativePathsByFileName = new(StringComparer.OrdinalIgnoreCase);

    public MapImageWriter(ILogger<MapImageWriter> logger, IOptions<RootOptions> options, IHeroesDataLoaderService heroesDataLoaderService)
        : base(logger, options, heroesDataLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Map;

    protected override void SetImages(Map element)
    {
        if (!string.IsNullOrWhiteSpace(element.ReplayPreviewImage) && !string.IsNullOrWhiteSpace(element.ReplayPreviewImagePath?.FilePath))
            _replayPreviewRelativePathsByFileName.TryAdd(element.ReplayPreviewImage, new ImageRelativePath(element, element.ReplayPreviewImagePath));

        if (!string.IsNullOrWhiteSpace(element.LoadingScreenImage) && !string.IsNullOrWhiteSpace(element.LoadingScreenImagePath?.FilePath))
            _loadingScreenRelativePathsByFileName.TryAdd(element.LoadingScreenImage, new ImageRelativePath(element, element.LoadingScreenImagePath));

        foreach (MapObjective mapObjective in element.MapObjectives)
        {
            foreach (MapObjectiveIcon mapObjectiveIcon in mapObjective.Icons)
            {
                if (!string.IsNullOrWhiteSpace(mapObjectiveIcon.Image) && !string.IsNullOrWhiteSpace(mapObjectiveIcon.ImagePath?.FilePath))
                    _mapObjectiveIconRelativePathsByFileName.TryAdd(mapObjectiveIcon.Image, new ImageRelativePath(element, mapObjectiveIcon.ImagePath));
            }
        }
    }

    protected override async Task SaveImages()
    {
        await SaveImagesFiles(_replayPreviewRelativePathsByFileName, Path.Combine(_mapDirectory, _replayPreviewDirectory));
        await SaveImagesFiles(_loadingScreenRelativePathsByFileName, Path.Combine(_mapDirectory, _loadingScreenDirectory));
        await SaveImagesFiles(_mapObjectiveIconRelativePathsByFileName, Path.Combine(_mapDirectory, _mapObjectiveIconDirectory));
    }
}
