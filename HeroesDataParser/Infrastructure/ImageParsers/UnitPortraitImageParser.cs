namespace HeroesDataParser.Infrastructure.ImageParsers;

public class UnitPortraitImageParser : ImageParserBase<Unit>
{
    public UnitPortraitImageParser(ILogger<UnitPortraitImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.UnitPortrait;

    protected override string SubDirectory => "unitportraits";

    protected override void SetImages(Unit element)
    {
        ProcessPortraitImage(element.UnitPortraits.MiniMapIcon, element.UnitPortraits.MiniMapIconPath, element.Id);
        ProcessPortraitImage(element.UnitPortraits.TargetInfoPanel, element.UnitPortraits.TargetInfoPanelPath, element.Id);
    }

    private void ProcessPortraitImage(string? imageName, RelativeFilePath? relativeFilePath, string elementId)
    {
        if (!string.IsNullOrWhiteSpace(imageName) && !string.IsNullOrWhiteSpace(relativeFilePath?.FilePath))
        {
            TryAddToFiles(imageName, elementId, async (directoryPath) =>
            {
                await ProcessBasicImage(imageName, relativeFilePath, directoryPath);
            });
        }
    }
}
