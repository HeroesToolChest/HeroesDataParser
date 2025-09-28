namespace HeroesDataParser.Infrastructure.ImageParsers;

public class AnnouncerImageParser : ImageParserBase<Announcer>
{
    public AnnouncerImageParser(ILogger<AnnouncerImageParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Announcer;

    protected override string SubDirectory => "announcers";

    protected override void SetImages(Announcer element)
    {
        if (!string.IsNullOrWhiteSpace(element.Image) && !string.IsNullOrWhiteSpace(element.ImagePath?.FilePath))
            AddImagePath(element.Image, new ImageRelativePath(element, element.ImagePath));
    }
}
