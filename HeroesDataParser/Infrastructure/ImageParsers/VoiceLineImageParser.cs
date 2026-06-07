namespace HeroesDataParser.Infrastructure.ImageParsers;

public class VoiceLineImageParser : ImageParserBase<VoiceLine>
{
    public VoiceLineImageParser(ILogger<VoiceLineImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.VoiceLine;

    protected override string Subdirectory => "voicelines";

    protected override void SetImages(VoiceLine element)
    {
        AddStaticImage(element);
    }
}
