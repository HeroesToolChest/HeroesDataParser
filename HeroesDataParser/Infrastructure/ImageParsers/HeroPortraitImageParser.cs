namespace HeroesDataParser.Infrastructure.ImageParsers;

public class HeroPortraitImageParser : ImageParserBase<Hero>
{
    public HeroPortraitImageParser(ILogger<HeroPortraitImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.HeroPortrait;

    protected override string Subdirectory => "heroportraits";

    protected override void SetImages(Hero element)
    {
        ProcessPortraitImage(element.HeroPortraits.HeroSelectPortrait, element.HeroPortraits.HeroSelectPortraitPath, element.Id);
        ProcessPortraitImage(element.HeroPortraits.LeaderboardPortrait, element.HeroPortraits.LeaderboardPortraitPath, element.Id);
        ProcessPortraitImage(element.HeroPortraits.LoadingScreenPortrait, element.HeroPortraits.LoadingScreenPortraitPath, element.Id);
        ProcessPortraitImage(element.HeroPortraits.PartyPanelPortrait, element.HeroPortraits.PartyPanelPortraitPath, element.Id);
        ProcessPortraitImage(element.HeroPortraits.TargetPortrait, element.HeroPortraits.TargetPortraitPath, element.Id);
        ProcessPortraitImage(element.HeroPortraits.DraftScreen, element.HeroPortraits.DraftScreenPath, element.Id);
        ProcessPortraitImage(element.HeroPortraits.MiniMapIcon, element.HeroPortraits.MiniMapIconPath, element.Id);
        ProcessPortraitImage(element.HeroPortraits.TargetInfoPanel, element.HeroPortraits.TargetInfoPanelPath, element.Id);

        for (int i = 0; i < element.HeroPortraits.PartyFrames.Count; i++)
        {
            string partyFrame = element.HeroPortraits.PartyFrames[i];
            RelativeFilePath partyFramePath = element.HeroPortraits.PartyFramePaths[i];

            AddToFiles(partyFrame, element.Id, async (directoryPath) =>
            {
                await ProcessStaticImage(partyFrame, partyFramePath, directoryPath);
            });
        }
    }

    private void ProcessPortraitImage(string? imageName, RelativeFilePath? relativeFilePath, string elementId)
    {
        if (!string.IsNullOrWhiteSpace(imageName) && !string.IsNullOrWhiteSpace(relativeFilePath?.FilePath))
        {
            AddToFiles(imageName, elementId, async (directoryPath) =>
            {
                await ProcessStaticImage(imageName, relativeFilePath, directoryPath);
            });
        }
    }
}
