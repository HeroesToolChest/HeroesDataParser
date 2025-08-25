namespace HeroesDataParser.Infrastructure.ImageWriters;

public class HeroPortraitImageWriter : ImageWriterBase<Hero>
{
    private const string _heroPortraitsDirectory = "heroportraits";

    private readonly Dictionary<string, ImageRelativePath> _heroPortraitsRelativePathsByFileName = new(StringComparer.OrdinalIgnoreCase);

    public HeroPortraitImageWriter(ILogger<HeroPortraitImageWriter> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, options, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.HeroPortrait;

    protected override void SetImages(Hero element)
    {
        TryAddPortrait(element.HeroPortraits.HeroSelectPortrait, element.HeroPortraits.HeroSelectPortraitPath, element);
        TryAddPortrait(element.HeroPortraits.LeaderboardPortrait, element.HeroPortraits.LeaderboardPortraitPath, element);
        TryAddPortrait(element.HeroPortraits.LoadingScreenPortrait, element.HeroPortraits.LoadingScreenPortraitPath, element);
        TryAddPortrait(element.HeroPortraits.PartyPanelPortrait, element.HeroPortraits.PartyPanelPortraitPath, element);
        TryAddPortrait(element.HeroPortraits.TargetPortrait, element.HeroPortraits.TargetPortraitPath, element);
        TryAddPortrait(element.HeroPortraits.DraftScreen, element.HeroPortraits.DraftScreenPath, element);
        TryAddPortrait(element.HeroPortraits.MiniMapIcon, element.HeroPortraits.MiniMapIconPath, element);
        TryAddPortrait(element.HeroPortraits.TargetInfoPanel, element.HeroPortraits.TargetInfoPanelPath, element);

        for (int i = 0; i < element.HeroPortraits.PartyFrames.Count; i++)
        {
            TryAddPortrait(element.HeroPortraits.PartyFrames[i], element.HeroPortraits.PartyFramePaths[i], element);
        }
    }

    protected override async Task SaveImages()
    {
        await SaveImagesFiles(_heroPortraitsRelativePathsByFileName, _heroPortraitsDirectory);
    }

    private void TryAddPortrait(string? fileName, RelativeFilePath? relativePath, Hero element)
    {
        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(relativePath?.FilePath))
            return;

        _heroPortraitsRelativePathsByFileName.TryAdd(fileName, new ImageRelativePath(element, relativePath));
    }
}
