namespace HeroesDataParser.Infrastructure.ImageParsers;

public class UnitPortraitImageParser : ImageParserBase<Unit>
{
    public UnitPortraitImageParser(ILogger<UnitPortraitImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.UnitPortrait;

    protected override string Subdirectory => "unitportraits";

    protected override void SetImages(Unit element)
    {
        ProcessPortraitImage(element.UnitPortraits.MiniMapIcon, element.UnitPortraits.MiniMapIconPath, element.Id);
        ProcessPortraitImage(element.UnitPortraits.TargetInfoPanel, element.UnitPortraits.TargetInfoPanelPath, element.Id);
    }

    private void ProcessPortraitImage(string? imageName, ImagePath? imagePath, string elementId)
    {
        if (!string.IsNullOrWhiteSpace(imageName) && !string.IsNullOrWhiteSpace(imagePath?.FilePath))
        {
            AddToFiles(imageName, elementId, async (directoryPath) =>
            {
                await ProcessStaticImage(imageName, imagePath, directoryPath);
            });
        }
    }
}
