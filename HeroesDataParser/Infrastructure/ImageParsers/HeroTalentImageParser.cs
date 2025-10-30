namespace HeroesDataParser.Infrastructure.ImageParsers;

public class HeroTalentImageParser : HeroAbilityTalentImageParser
{
    public HeroTalentImageParser(ILogger<HeroTalentImageParser> logger)
        : base(logger)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Talent;

    protected override string SubDirectory => "talents";

    protected override void SetImages(Hero element, HashSet<ImageWriterPath> imagePaths)
    {
        SetTalentImages(element, imagePaths);
    }
}
