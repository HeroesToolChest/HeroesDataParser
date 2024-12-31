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
        if (_announcerFilePaths.Count < 1)
        {
            _logger.LogInformation("No announcer file paths found");
            return;
        }

        _logger.LogInformation("{Count} announcer images to save", _announcerFilePaths.Count);
        _logger.LogTrace("Announcer file paths: {@FilePaths}", _announcerFilePaths);

        string outputDirectory = Path.Combine(_options.OutputDirectory, ImageDirectory, _announcerDirectory);

        Directory.CreateDirectory(outputDirectory);

        _logger.LogInformation("Saving announcer images to {OutputDirectory}", outputDirectory);

        List<Task> tasks = new(_announcerFilePaths.Count);

        foreach (string relativePath in _announcerFilePaths)
        {
            tasks.Add(SaveStaticImageFile(relativePath, outputDirectory));
        }

        await Task.WhenAll(tasks);
    }
}
