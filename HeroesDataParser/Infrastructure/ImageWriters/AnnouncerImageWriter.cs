namespace HeroesDataParser.Infrastructure.ImageWriters;

public class AnnouncerImageWriter : ImageWriterBase<Announcer>
{
    private const string _announcerDirectory = "announcers";

    private readonly Dictionary<string, ImageRelativePath> _announcerRelativePathsByFileName = new(StringComparer.OrdinalIgnoreCase);

    public AnnouncerImageWriter(ILogger<AnnouncerImageWriter> logger, IOptions<RootOptions> options, IHeroesDataLoaderService heroesDataLoaderService)
        : base(logger, options, heroesDataLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Announcer;

    protected override void SetImages(Announcer element)
    {
        if (!string.IsNullOrWhiteSpace(element.Image) && !string.IsNullOrWhiteSpace(element.ImagePath?.FilePath))
            _announcerRelativePathsByFileName.TryAdd(element.Image, new ImageRelativePath(element, element.ImagePath));
    }

    protected override async Task SaveImages()
    {
        await SaveImagesFiles(_announcerRelativePathsByFileName, _announcerDirectory);
    }
}
