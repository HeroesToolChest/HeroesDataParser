namespace HeroesDataParser.Infrastructure.ImageParsers;

public class HeroPortraitImageParser : ImageParserBase<Hero>
{
    public HeroPortraitImageParser(ILogger<HeroPortraitImageParser> logger)
        : base(logger)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.HeroPortrait;

    protected override string SubDirectory => "heroportraits";

    protected override void SetImages(Hero element, HashSet<ImageWriterPath> imagePaths)
    {
        TryAddToFiles(imagePaths, element.HeroPortraits.HeroSelectPortrait, element.HeroPortraits.HeroSelectPortraitPath, element);
        TryAddToFiles(imagePaths, element.HeroPortraits.LeaderboardPortrait, element.HeroPortraits.LeaderboardPortraitPath, element);
        TryAddToFiles(imagePaths, element.HeroPortraits.LoadingScreenPortrait, element.HeroPortraits.LoadingScreenPortraitPath, element);
        TryAddToFiles(imagePaths, element.HeroPortraits.PartyPanelPortrait, element.HeroPortraits.PartyPanelPortraitPath, element);
        TryAddToFiles(imagePaths, element.HeroPortraits.TargetPortrait, element.HeroPortraits.TargetPortraitPath, element);
        TryAddToFiles(imagePaths, element.HeroPortraits.DraftScreen, element.HeroPortraits.DraftScreenPath, element);
        TryAddToFiles(imagePaths, element.HeroPortraits.MiniMapIcon, element.HeroPortraits.MiniMapIconPath, element);
        TryAddToFiles(imagePaths, element.HeroPortraits.TargetInfoPanel, element.HeroPortraits.TargetInfoPanelPath, element);

        for (int i = 0; i < element.HeroPortraits.PartyFrames.Count; i++)
        {
            TryAddToFiles(imagePaths, element.HeroPortraits.PartyFrames[i], element.HeroPortraits.PartyFramePaths[i], element);
        }
    }
}
