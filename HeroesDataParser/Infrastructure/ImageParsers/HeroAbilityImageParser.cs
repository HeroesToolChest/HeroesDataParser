namespace HeroesDataParser.Infrastructure.ImageParsers;

public class HeroAbilityImageParser : HeroAbilityTalentImageParser
{
    public HeroAbilityImageParser(ILogger<HeroAbilityImageParser> logger)
        : base(logger)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.Ability;

    protected override string SubDirectory => "abilities";

    protected override void SetImages(Hero element, HashSet<ImageWriterPath> imagePaths)
    {
        SetAbilityImages(element, imagePaths);
    }
}
