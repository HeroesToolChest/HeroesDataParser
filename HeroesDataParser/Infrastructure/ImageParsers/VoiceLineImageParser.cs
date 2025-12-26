namespace HeroesDataParser.Infrastructure.ImageParsers;

public class VoiceLineImageParser : ImageParserBase<VoiceLine>
{
    public VoiceLineImageParser(ILogger<VoiceLineImageParser> logger)
    : base(logger)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.VoiceLine;

    protected override string SubDirectory => "voicelines";

    protected override void SetImages(VoiceLine element, HashSet<ImageWriterPath> imagePaths)
    {
        AddBasicImage(element, imagePaths);
    }
}
