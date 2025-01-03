using System.Xml.Linq;

namespace HeroesDataParser.Infrastructure.ImageWriters;

public class AnnouncerImageWriter : ImageWriterBase<Announcer>
{
    private const string _announcerDirectory = "announcers";

    private readonly ILogger<AnnouncerImageWriter> _logger;
    private readonly RootOptions _options;
    private readonly HashSet<string> _announcerFilePaths = new(StringComparer.OrdinalIgnoreCase);

    public AnnouncerImageWriter(ILogger<AnnouncerImageWriter> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
        _logger = logger;
        _options = options.Value;
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Announcer;

    protected override void GetImages(Announcer element)
    {
        if (!string.IsNullOrWhiteSpace(element.ImagePath))
            _announcerFilePaths.Add(element.ImagePath);
    }

    protected override async Task SaveImages()
    {
        await SaveImagesFiles(_announcerFilePaths, _announcerDirectory);
    }
}
