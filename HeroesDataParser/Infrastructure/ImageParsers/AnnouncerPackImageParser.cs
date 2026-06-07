namespace HeroesDataParser.Infrastructure.ImageParsers;

public class AnnouncerPackImageParser : ImageParserBase<AnnouncerPack>
{
    public AnnouncerPackImageParser(ILogger<AnnouncerPackImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Announcer;

    protected override string Subdirectory => "announcers";

    protected override void SetImages(AnnouncerPack element)
    {
        AddStaticImage(element);
    }
}
