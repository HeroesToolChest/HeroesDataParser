namespace HeroesDataParser.Infrastructure.ImageParsers;

public class AnnouncerImageParser : ImageParserBase<Announcer>
{
    public AnnouncerImageParser(ILogger<AnnouncerImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Announcer;

    protected override string SubDirectory => "announcers";

    protected override void SetImages(Announcer element)
    {
        AddBasicImage(element);
    }
}
