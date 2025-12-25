namespace HeroesDataParser.Infrastructure.ImageParsers;

public class AnnouncerImageParser : ImageParserBase<Announcer>
{
    public AnnouncerImageParser(ILogger<AnnouncerImageParser> logger)
        : base(logger)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Announcer;

    protected override string SubDirectory => "announcers";

    protected override void SetImages(Announcer element, HashSet<ImageWriterPath> imagePaths)
    {
        AddBasicImage(element, imagePaths);
    }
}
