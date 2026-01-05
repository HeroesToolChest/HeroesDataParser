using SixLabors.ImageSharp;

namespace HeroesDataParser.Infrastructure.ImageParsers;

public class MatchAwardImageParser : ImageParserBase<MatchAward>
{
    public MatchAwardImageParser(ILogger<MatchAwardImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.VoiceLine;

    protected override string SubDirectory => "matchawards";

    protected override void SetImages(MatchAward element)
    {
        if (!string.IsNullOrWhiteSpace(element.ScoreScreenImage))
        {
            if (!string.IsNullOrWhiteSpace(element.ScoreScreenImageBluePath?.FilePath))
            {
                TryAddToFiles($"{element.ScoreScreenImage} (blue)", element.Id, async (directoryPath) =>
                {
                    await ProcessScoreScreenImageFile(element.ScoreScreenImage, element.ScoreScreenImageBluePath, directoryPath, "blue");
                });
            }

            if (!string.IsNullOrWhiteSpace(element.ScoreScreenImageRedPath?.FilePath))
            {
                TryAddToFiles($"{element.ScoreScreenImage} (red)", element.Id, async (directoryPath) =>
                {
                    await ProcessScoreScreenImageFile(element.ScoreScreenImage, element.ScoreScreenImageRedPath, directoryPath, "red");
                });
            }
        }

        if (!string.IsNullOrWhiteSpace(element.MVPScreenImage) && !string.IsNullOrWhiteSpace(element.MVPScreenImagePath?.FilePath))
        {
            TryAddToFiles(element.MVPScreenImage, element.Id, async (directoryPath) =>
            {
                await ProcessMVPImageFile(element.MVPScreenImage, element.MVPScreenImagePath, directoryPath);
            });
        }
    }

    private async Task ProcessScoreScreenImageFile(string scoreScreenImage, RelativeFilePath relativeFilePath, string directoryPath, string color)
    {
        VerifyFileExists(relativeFilePath);

        using Stream stream = HeroesXmlLoaderService.HeroesXmlLoader.GetFile(relativeFilePath.FilePath, relativeFilePath.MpqFilePath);
        using DDSImage ddsImage = new(stream);

        string scoreImagePath = Path.Combine(directoryPath, scoreScreenImage.Replace("%team%", color, StringComparison.OrdinalIgnoreCase));

        await ddsImage.Save(scoreImagePath);
    }

    private async Task ProcessMVPImageFile(string mvpScreenImage, RelativeFilePath relativeFilePath, string directoryPath)
    {
        VerifyFileExists(relativeFilePath);

        using Stream stream = HeroesXmlLoaderService.HeroesXmlLoader.GetFile(relativeFilePath.FilePath, relativeFilePath.MpqFilePath);
        using DDSImage ddsImage = new(stream);

        int newWidth = ddsImage.Width / 3;

        string mvpImageBluePath = Path.Combine(directoryPath, mvpScreenImage.Replace("%color%", "blue", StringComparison.OrdinalIgnoreCase));
        string mvpImageRedPath = Path.Combine(directoryPath, mvpScreenImage.Replace("%color%", "red", StringComparison.OrdinalIgnoreCase));
        string mvpImageGoldPath = Path.Combine(directoryPath, mvpScreenImage.Replace("%color%", "gold", StringComparison.OrdinalIgnoreCase));

        await ddsImage.Save(mvpImageBluePath, new Point(0, 0), new SixLabors.ImageSharp.Size(newWidth, ddsImage.Height));
        await ddsImage.Save(mvpImageRedPath, new Point(newWidth, 0), new SixLabors.ImageSharp.Size(newWidth, ddsImage.Height));
        await ddsImage.Save(mvpImageGoldPath, new Point(newWidth * 2, 0), new SixLabors.ImageSharp.Size(newWidth, ddsImage.Height));
    }
}
